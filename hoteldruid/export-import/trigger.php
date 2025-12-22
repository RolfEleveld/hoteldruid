<?php
/**
 * Non-blocking launcher endpoint
 * - Creates (or reuses) a run folder and state.json
 * - Spawns the CLI worker in the background
 */

error_reporting(E_ALL);
ini_set('display_errors', 1);

header('Content-Type: application/json; charset=utf-8');

/**
 * --------------------------------------------------------------------------
 * Basic debug trace
 * --------------------------------------------------------------------------
 */

@file_put_contents(
    __DIR__ . '/../trigger_debug.log',
    date('c') . " trigger.php executed\n",
    FILE_APPEND | LOCK_EX
);

/**
 * --------------------------------------------------------------------------
 * Resolve dati path
 * --------------------------------------------------------------------------
 *
 * Priority:
 *  1. Use C_DATI_PATH if defined
 *  2. Fallback to an existing ../../dati folder (do NOT auto-create it)
 */

// Try to load application constants which may define C_DATI_PATH (hoteldruid/costanti.php)
$costanti = realpath(__DIR__ . '/../costanti.php');
if ($costanti && file_exists($costanti)) {
    // include once; costanti.php handles defining C_DATI_PATH based on hoteldruid-config.php
    @include_once $costanti;
}

if (defined('C_DATI_PATH') && C_DATI_PATH) {
    $dataPath = rtrim(str_replace('\\', '/', C_DATI_PATH), '/');
    // If relative path provided, make it absolute relative to hoteldruid dir
    if (!preg_match('/^[A-Za-z]:\/|^\/|^\\\\/', $dataPath)) {
        $dataPath = realpath(__DIR__ . '/../' . $dataPath) ?: (__DIR__ . '/../' . $dataPath);
    }
    $dataPath = rtrim(str_replace('\\', '/', $dataPath), '/');
} else {
    // Fallback candidates: prefer hoteldruid/dati then repo-root dati
    $candidates = [
        realpath(__DIR__ . '/../dati'),
        realpath(__DIR__ . '/../../dati'),
    ];
    $found = false;
    foreach ($candidates as $candidate) {
        if ($candidate !== false && is_dir($candidate)) {
            $dataPath = rtrim($candidate, "\\/");
            $found = true;
            break;
        }
    }
    if (!$found) {
        http_response_code(500);
        echo json_encode([
            'error'   => 'dati_folder_not_found',
            'message' => 'Could not resolve the dati folder. Ensure it exists and adjust the relative path if needed.',
            'checked' => [
                __DIR__ . '/../dati',
                __DIR__ . '/../../dati',
            ],
        ]);
        exit;
    }
}

/**
 * --------------------------------------------------------------------------
 * Ensure export-runs base folder exists (under the existing dati folder)
 * --------------------------------------------------------------------------
 */

$runBase = $dataPath . '/export-runs';

if (!is_dir($runBase)) {
    if (!@mkdir($runBase, 0755, true)) {
        http_response_code(500);
        echo json_encode([
            'error'   => 'export_runs_folder_creation_failed',
            'message' => 'Failed to create export-runs folder under dati.',
            'dati'    => $dataPath,
            'runBase' => $runBase,
        ]);
        exit;
    }
}

/**
 * --------------------------------------------------------------------------
 * Single-run marker-file mechanism
 * - A single marker file (`export.marker`) stores: `yyyy.mm.dd|/path/to/export-run`
 * - If marker date == today -> report export in progress (include `state.json` if present)
 * - If marker older than a day -> attempt cleanup of previous run and remove marker
 * - Otherwise create a fresh run folder and write marker
 * --------------------------------------------------------------------------
 */

$markerFile = $dataPath . '/export.marker';

// Recursive remove for run folders (used carefully only for runBase subpaths)
function rrmdir($dir)
{
    if (!is_dir($dir)) {
        return;
    }
    $objects = scandir($dir);
    foreach ($objects as $object) {
        if ($object === '.' || $object === '..') continue;
        $path = $dir . DIRECTORY_SEPARATOR . $object;
        if (is_dir($path)) {
            rrmdir($path);
        } else {
            @unlink($path);
        }
    }
    @rmdir($dir);
}

$createdFolder = false;

if (file_exists($markerFile)) {
    $raw = trim(@file_get_contents($markerFile));
    $parts = explode('|', $raw, 2);
    $markerDate = $parts[0] ?? '';
    $markerPath = $parts[1] ?? '';

    // If marker is for today -> report export in progress and include state.json if available
    if ($markerDate === date('Y.m.d')) {
        $stateInfo = null;
        $stateFile = rtrim($markerPath, "\/") . '/state.json';
        if ($markerPath && file_exists($stateFile)) {
            $stateInfo = @json_decode(@file_get_contents($stateFile), true);
        }

        http_response_code(200);
        echo json_encode([
            'status'      => 'export_in_progress',
            'marker_file' => $markerFile,
            'marker_date' => $markerDate,
            'export_path' => $markerPath,
            'state'       => $stateInfo,
        ]);
        exit;
    }

    // Marker older than today -> try to clean up previous run (only if it's under runBase)
    if ($markerPath) {
        $realMarkerPath = realpath($markerPath) ?: $markerPath;
        $realRunBase = realpath($runBase) ?: $runBase;
        if ($realMarkerPath && strpos($realMarkerPath, $realRunBase) === 0) {
            rrmdir($realMarkerPath);
        }
    }

    @unlink($markerFile);
}

// Create new run and write marker
$runId = date('Ymd_His') . '_' . substr(bin2hex(random_bytes(4)), 0, 6);
$runFolder = $runBase . '/' . $runId;

if (!is_dir($runFolder)) {
    $createdFolder = @mkdir($runFolder, 0755, true);
}

$runFolderReal = realpath($runFolder) ?: $runFolder;
@file_put_contents($markerFile, date('Y.m.d') . '|' . $runFolderReal, LOCK_EX);

/**
 * --------------------------------------------------------------------------
 * Prepare initial state.json (lightweight; worker will populate details)
 * --------------------------------------------------------------------------
 */

$state = [
    'run_id'        => $runId,
    'status'        => 'pending',
    'created_at'    => date('c'),
    'tables'        => [],
    'current_table' => null,
    'logfile'       => $runFolder . '/run.log',
    'output_folder' => $runFolder,
];

/**
 * --------------------------------------------------------------------------
 * Diagnostics collection
 * --------------------------------------------------------------------------
 */

$diagnostics = [];

$diagnostics['C_DATI_PATH']        = defined('C_DATI_PATH') ? C_DATI_PATH : null;
$diagnostics['resolved_data_path'] = $dataPath;
$diagnostics['run_base']           = $runBase;
$diagnostics['run_folder']         = $runFolder;
$diagnostics['folder_created']     = (is_dir($runFolder) ? true : $createdFolder);
$diagnostics['php_binary']         = PHP_BINARY;
$diagnostics['sapi']               = PHP_SAPI;

$diagnostics['disable_functions'] = ini_get('disable_functions');
$disabled = array_filter(array_map('trim', explode(',', (string) ini_get('disable_functions'))));
$diagnostics['disabled_functions_list'] = $disabled;

// Check basic writability
$diagnostics['is_writable'] = is_dir($runFolder) && is_writable($runFolder);

$testWrite = false;
if ($diagnostics['is_writable']) {
    $testFile  = $runFolder . '/.write_test';
    $testWrite = (@file_put_contents($testFile, 'ok') !== false);
    if ($testWrite) {
        @unlink($testFile);
    }
}
$diagnostics['test_write_success'] = $testWrite;

// Check available spawn functions
$can_popen     = function_exists('popen') && !in_array('popen', $disabled, true);
$can_exec      = function_exists('exec') && !in_array('exec', $disabled, true);
$can_proc_open = function_exists('proc_open') && !in_array('proc_open', $disabled, true);

$diagnostics['can_popen']     = $can_popen;
$diagnostics['can_exec']      = $can_exec;
$diagnostics['can_proc_open'] = $can_proc_open;

/**
 * --------------------------------------------------------------------------
 * Write state.json atomically
 * --------------------------------------------------------------------------
 */

$stateFile = $runFolder . '/state.json';
$tmpFile   = $stateFile . '.tmp';

if (@file_put_contents($tmpFile, json_encode($state, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE), LOCK_EX) === false
    || !@rename($tmpFile, $stateFile)
) {
    http_response_code(500);
    echo json_encode([
        'error'       => 'state_file_write_failed',
        'message'     => 'Could not write state.json in run folder.',
        'run_id'      => $runId,
        'run_folder'  => $runFolder,
        'state_file'  => $stateFile,
        'diagnostics' => $diagnostics,
    ]);
    exit;
}

/**
 * --------------------------------------------------------------------------
 * If we cannot write or spawn, return diagnostics and stop
 * --------------------------------------------------------------------------
 */

if (
    !$diagnostics['folder_created']
    || !$diagnostics['test_write_success']
    || !($can_exec || $can_popen || $can_proc_open)
) {
    http_response_code(200);
    echo json_encode([
        'run_id'      => $runId,
        'status'      => 'created_but_not_started',
        'state_file'  => $stateFile,
        'logfile'     => $runFolder . '/run.log',
        'diagnostics' => $diagnostics,
    ]);
    exit;
}

/**
 * --------------------------------------------------------------------------
 * Spawn background worker (non-blocking)
 * --------------------------------------------------------------------------
 */

$phpBin     = PHP_BINARY ?: 'php';
$workerFile = __DIR__ . '/bin/flatten-worker.php';
$runLog     = $runFolder . '/run.log';

try {
    if (stripos(PHP_OS, 'WIN') === 0) {
        // Windows: use cmd /c start to detach
        $phpQuoted    = '"' . str_replace('"', '\\"', $phpBin) . '"';
        $workerQuoted = '"' . str_replace('"', '\\"', $workerFile) . '"';
        $cmd = $phpQuoted
            . ' ' . $workerQuoted
            . ' --run-id=' . escapeshellarg($runId)
            . ' > "' . $runLog . '" 2>&1';

        pclose(popen('cmd /c start "" /B ' . $cmd, 'r'));
    } else {
        // Unix-like: background process with output redirected
        $phpEsc    = escapeshellarg($phpBin);
        $workerEsc = escapeshellarg($workerFile);
        $cmd = $phpEsc
            . ' ' . $workerEsc
            . ' --run-id=' . escapeshellarg($runId)
            . ' > ' . escapeshellarg($runLog) . ' 2>&1 &';

        exec($cmd);
    }

    http_response_code(200);
    echo json_encode([
        'run_id'      => $runId,
        'status'      => 'started',
        'state_file'  => $stateFile,
        'logfile'     => $runLog,
        'diagnostics' => $diagnostics,
    ]);
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode([
        'error'       => 'failed_to_spawn_worker',
        'message'     => $e->getMessage(),
        'run_id'      => $runId,
        'state_file'  => $stateFile,
        'logfile'     => $runLog,
        'diagnostics' => $diagnostics,
    ]);
}

?>