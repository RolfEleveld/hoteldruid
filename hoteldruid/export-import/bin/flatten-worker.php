<?php
// CLI worker: processes an export run and writes per-table JSON files, updates state, and creates ZIP
// Usage: php flatten-worker.php --run-id=RUNID

error_reporting(E_ALL);
ini_set('display_errors', 1);

$opts = getopt('', ['run-id:']);
$runId = $opts['run-id'] ?? null;
if (!$runId) {
    fwrite(STDERR, "Usage: php flatten-worker.php --run-id=RUNID\n");
    exit(1);
}

// resolve data path
$dataPath = defined('C_DATI_PATH') ? C_DATI_PATH : (__DIR__ . '/../../dati');
if (!is_dir($dataPath)) {
    // fallback
    $dataPath = __DIR__ . '/../../dati';
}

$runFolder = rtrim($dataPath, "\/") . '/export-runs/' . $runId;
$stateFile = $runFolder . '/state.json';
$logFile = $runFolder . '/run.log';

if (!is_dir($runFolder)) {
    mkdir($runFolder, 0755, true);
}

function logLine($msg) {
    global $logFile;
    $line = '[' . date('Y-m-d H:i:s') . '] ' . $msg . PHP_EOL;
    file_put_contents($logFile, $line, FILE_APPEND | LOCK_EX);
}

logLine("Worker starting for run {$runId}");

// helper for atomic state writes
function write_state($state) {
    global $stateFile;
    $tmp = $stateFile . '.tmp';
    file_put_contents($tmp, json_encode($state, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE), LOCK_EX);
    @rename($tmp, $stateFile);
}

// load state
if (!file_exists($stateFile)) {
    logLine('State file not found: ' . $stateFile);
    exit(1);
}
$state = json_decode(file_get_contents($stateFile), true);
if (!$state) {
    logLine('Failed to parse state.json');
    exit(1);
}

// include DataFlattener and database connection helpers
require_once(__DIR__ . '/../lib/DataFlattener.php');
// attempt to include connection files similar to web script
if (file_exists(__DIR__ . '/../../dati/dati_connessione.php')) {
    include_once(__DIR__ . '/../../dati/dati_connessione.php');
}
if (file_exists(__DIR__ . '/../../includes/funzioni.php')) {
    include_once(__DIR__ . '/../../includes/funzioni.php');
}

// connect (if function available)
if (function_exists('connetti_db') && empty($numconnessione) && !empty($PHPR_DB_TYPE) && !empty($PHPR_DB_NAME)) {
    $numconnessione = @connetti_db(
        $PHPR_DB_NAME,
        $PHPR_DB_HOST ?? 'localhost',
        $PHPR_DB_PORT ?? 3306,
        $PHPR_DB_USER ?? '',
        $PHPR_DB_PASS ?? '',
        $PHPR_LOAD_EXT ?? ''
    );
}

$flattener = new DataFlattener($numconnessione ?? null, $PHPR_TAB_PRE ?? '', $PHPR_DB_TYPE ?? 'sqlite', null);

// Ensure tables exist in state
if (empty($state['tables']) || !is_array($state['tables'])) {
    // try to discover tables
    $tables = $flattener->getAllTables();
    $state['tables'] = [];
    foreach ($tables as $t) {
        $state['tables'][] = ['name' => $t, 'status' => 'pending', 'row_count' => 0, 'exported_count' => 0];
    }
    $state['status'] = 'running';
    write_state($state);
}

// per-table processing
foreach ($state['tables'] as $idx => $table) {
    if (($table['status'] ?? '') === 'done') {
        continue;
    }
    $tname = $table['name'];
    $state['current_table'] = $tname;
    $state['status'] = 'running';
    write_state($state);
    logLine("Processing table: {$tname}");

    try {
        $tableData = $flattener->flattenTable($tname);
        if ($tableData === null) {
            throw new Exception('flattenTable returned null for ' . $tname);
        }

        // write table JSON
        $tablesDir = $runFolder . '/tables';
        if (!is_dir($tablesDir)) { mkdir($tablesDir, 0755, true); }
        $outfile = $tablesDir . '/' . $tname . '.json';
        file_put_contents($outfile, json_encode($tableData, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE), LOCK_EX);

        // update state
        $state['tables'][$idx]['status'] = 'done';
        $state['tables'][$idx]['row_count'] = $tableData['row_count'] ?? count($tableData['rows'] ?? []);
        $state['tables'][$idx]['exported_count'] = $state['tables'][$idx]['row_count'];
        $state['last_updated'] = date('c');
        write_state($state);
        logLine("Completed table: {$tname}");
    } catch (Throwable $e) {
        $state['tables'][$idx]['status'] = 'failed';
        $state['tables'][$idx]['error'] = $e->getMessage();
        $state['last_updated'] = date('c');
        write_state($state);
        logLine('Error processing ' . $tname . ': ' . $e->getMessage());
        // continue with next table
    }
}

// finalize: create manifest and zip
$allDone = true;
foreach ($state['tables'] as $t) {
    if (($t['status'] ?? '') !== 'done') { $allDone = false; break; }
}

if ($allDone) {
    $state['status'] = 'finalizing';
    write_state($state);
    $manifest = [
        'run_id' => $runId,
        'created' => date('c'),
        'tables' => array_map(function($t){ return ['name'=>$t['name'],'row_count'=>$t['row_count'] ?? 0]; }, $state['tables'])
    ];
    file_put_contents($runFolder . '/manifest.json', json_encode($manifest, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE), LOCK_EX);

    $zipPath = $runFolder . '/' . $runId . '.zip';
    $zip = new ZipArchive();
    if ($zip->open($zipPath, ZipArchive::CREATE | ZipArchive::OVERWRITE) === true) {
        // add all files under runFolder
        $files = new RecursiveIteratorIterator(new RecursiveDirectoryIterator($runFolder));
        foreach ($files as $file) {
            if ($file->isDir()) continue;
            // skip zip itself while creating
            if ($file->getRealPath() === realpath($zipPath)) continue;
            $localName = substr($file->getRealPath(), strlen(realpath($runFolder)) + 1);
            $zip->addFile($file->getRealPath(), $localName);
        }
        $zip->close();
        $state['status'] = 'completed';
        $state['package_path'] = $zipPath;
        $state['completed_at'] = date('c');
        write_state($state);
        logLine('Run completed, package created: ' . $zipPath);
    } else {
        $state['status'] = 'failed';
        write_state($state);
        logLine('Failed to create zip package');
    }
} else {
    $state['status'] = 'partial';
    write_state($state);
    logLine('Run completed with partial results (some tables failed)');
}

logLine('Worker exiting');
exit(0);

?>
