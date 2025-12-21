<?php
/**
 * ExportImportUI.php
 * UI components for export/import functionality integrated into crea_backup.php
 */

class ExportImportUI {
    private $id_utente;
    private $anno;
    private $id_sessione;
    private $pag = 'crea_backup.php';

    public function __construct($id_utente, $anno, $id_sessione) {
        $this->id_utente = $id_utente;
        $this->anno = $anno;
        $this->id_sessione = $id_sessione;
    }

    /**
     * Render export/import section
     */
    public function renderExportImportSection() {
        $html = '';

        // Export Section
        $html .= $this->renderExportUI();

        // Import Section
        $html .= $this->renderImportUI();

        return $html;
    }

    /**
     * Render export UI
     */
    private function renderExportUI() {
        $html = '<div class="rbox" style="margin-top: 20px;"><div class="rheader">';
        $html .= '📤 ' . mex('Export Data', $this->pag);
        $html .= '</div><div class="rcontent">';

        $html .= '<p>' . mex('Export your HotelDroid data to a portable JSON-based package', $this->pag) . '</p>';

        $html .= '<form accept-charset="utf-8" method="post" action="./index.php"><div>';
        $html .= '<input type="hidden" name="anno" value="' . $this->anno . '">';
        $html .= '<input type="hidden" name="id_sessione" value="' . $this->id_sessione . '">';
        $html .= '<input type="hidden" name="azione" value="SI">';
        $html .= '<input type="hidden" name="export_import" value="1">';

        $html .= '<table style="width: 100%;">';
        $html .= '<tr><td style="width: 30px;"></td><td>';
        $html .= '<label><input type="checkbox" name="export_include_configs" value="1" checked> ';
        $html .= mex('Include configurations', $this->pag) . '</label><br>';
        $html .= '<label><input type="checkbox" name="export_include_templates" value="1" checked> ';
        $html .= mex('Include templates', $this->pag) . '</label><br>';
        $html .= '<label><input type="checkbox" name="export_include_documents" value="1" checked> ';
        $html .= mex('Include documents', $this->pag) . '</label>';
        $html .= '</td></tr>';
        $html .= '</table>';

        $html .= '<div style="text-align: center; margin-top: 15px;">';
        $html .= '<button class="abkp" type="submit" name="create_export" value="1">';
        $html .= '<div>' . mex('Create Export Package', $this->pag) . '</div>';
        $html .= '</button>';
        $html .= '</div>';

        $html .= '</div></form></div></div>';
        // Recent exports (discover, prune, and list)
        $recent = $this->discoverAndPruneRecentPackages(5);
        $html .= '<div style="margin-top:18px;">';
        $html .= '<h4>' . mex('Recent Exports', $this->pag) . '</h4>';
        if (empty($recent)) {
            $html .= '<p>' . mex('No export packages found', $this->pag) . '</p>';
        } else {
            $html .= '<table style="width:100%; border-collapse:collapse;">';
            $html .= '<tr><th style="text-align:left; padding:6px; border-bottom:1px solid #ddd;">' . mex('File', $this->pag) . '</th>';
            $html .= '<th style="text-align:left; padding:6px; border-bottom:1px solid #ddd;">' . mex('Date', $this->pag) . '</th>';
            $html .= '<th style="text-align:left; padding:6px; border-bottom:1px solid #ddd;">' . mex('Size', $this->pag) . '</th></tr>';
            foreach ($recent as $p) {
                $fname = htmlspecialchars(basename($p['path']));
                $date = date('Y-m-d H:i:s', $p['mtime']);
                $size = $this->humanFilesize($p['size']);
                if (!empty($p['web_url'])) {
                    $fileHtml = '<a href="' . htmlspecialchars($p['web_url']) . '" download>' . $fname . '</a>';
                } else {
                    // Provide a file:// link using forward slashes for files outside web root
                    $realPathForUrl = str_replace('\\', '/', $p['path']);
                    // Encode spaces and other unsafe characters but keep slashes
                    $realPathForUrl = str_replace(' ', '%20', $realPathForUrl);
                    $fileUrl = 'file://' . $realPathForUrl;
                    $fileHtml = '<a href="' . htmlspecialchars($fileUrl) . '">' . $fname . '</a>';
                    $fileHtml .= ' <small>(' . htmlspecialchars($realPathForUrl) . ')</small>';
                }
                $html .= '<tr><td style="padding:6px;">' . $fileHtml . '</td><td style="padding:6px;">' . $date . '</td><td style="padding:6px;">' . $size . '</td></tr>';
            }
            $html .= '</table>';
        }
        $html .= '</div>';

        return $html;
    }

    /**
     * Discover recent package files from known locations and prune older ones.
     * Returns an array of up-to-$limit packages with keys: path, mtime, size, web_url|null
     */
    private function discoverAndPruneRecentPackages($limit = 5) {
        // Determine hoteldruid root (two levels up from this file)
        $hoteldruidRoot = realpath(__DIR__ . '/../../');

        // Candidate locations
        $packagesDir = null;
        $runBase = null;
        if (defined('C_DATI_PATH') && C_DATI_PATH) {
            $packagesDir = realpath(rtrim(C_DATI_PATH, '\\/') . '/export-import/packages') ?: (rtrim(C_DATI_PATH, '\\/') . '/export-import/packages');
            $runBase = realpath(rtrim(C_DATI_PATH, '\\/') . '/export-runs') ?: (rtrim(C_DATI_PATH, '\\/') . '/export-runs');
        } else {
            // fall back to paths under hoteldruid
            $packagesDir = realpath($hoteldruidRoot . '/export-import/packages') ?: ($hoteldruidRoot . '/export-import/packages');
            $runBase = realpath($hoteldruidRoot . '/dati/export-runs') ?: ($hoteldruidRoot . '/dati/export-runs');
        }

        $candidates = [];

        // Collect zips from packagesDir
        if ($packagesDir && is_dir($packagesDir)) {
            $files = scandir($packagesDir);
            foreach ($files as $f) {
                if (stripos($f, '.zip') === false) continue;
                $path = $packagesDir . '/' . $f;
                if (is_file($path)) {
                    $candidates[] = ['path' => $path, 'mtime' => filemtime($path), 'size' => filesize($path), 'source' => 'packages'];
                }
            }
        }

        // Collect zips created by worker under runBase (one level deep)
        if ($runBase && is_dir($runBase)) {
            $runs = scandir($runBase);
            foreach ($runs as $r) {
                if ($r === '.' || $r === '..') continue;
                $rf = $runBase . '/' . $r;
                if (is_dir($rf)) {
                    // find any zip file inside
                    $files = glob($rf . '/*.zip');
                    foreach ($files as $path) {
                        if (is_file($path)) {
                            $candidates[] = ['path' => $path, 'mtime' => filemtime($path), 'size' => filesize($path), 'source' => 'runs', 'run_folder' => $rf];
                        }
                    }
                }
            }
        }

        // Sort by mtime desc
        usort($candidates, function($a, $b){ return $b['mtime'] <=> $a['mtime']; });

        // Build result list and prune older than $limit
        $result = [];
        foreach ($candidates as $i => $c) {
            if ($i < $limit) {
                // compute web_url if file is under hoteldruid root
                $realHot = $hoteldruidRoot ? $hoteldruidRoot : '';
                $realPath = realpath($c['path']) ?: $c['path'];
                $webUrl = null;
                if ($realHot && strpos($realPath, $realHot) === 0) {
                    $rel = substr($realPath, strlen($realHot));
                    $rel = str_replace('\\', '/', $rel);
                    $webUrl = '.' . $rel;
                }
                $result[] = ['path'=>$realPath,'mtime'=>$c['mtime'],'size'=>$c['size'],'web_url'=>$webUrl];
            } else {
                // Prune older: if source is 'runs' remove entire run folder, else remove file
                if (!empty($c['source']) && $c['source'] === 'runs' && !empty($c['run_folder'])) {
                    $this->recursiveDelete($c['run_folder']);
                } else {
                    @unlink($c['path']);
                }
            }
        }

        return $result;
    }

    private function recursiveDelete($dir) {
        if (!is_dir($dir)) return;
        $objs = scandir($dir);
        foreach ($objs as $obj) {
            if ($obj === '.' || $obj === '..') continue;
            $path = $dir . '/' . $obj;
            if (is_dir($path)) {
                $this->recursiveDelete($path);
            } else {
                @unlink($path);
            }
        }
        @rmdir($dir);
    }

    private function humanFilesize($bytes, $decimals = 1) {
        $sz = array('B','KB','MB','GB','TB');
        $factor = floor((strlen($bytes) - 1) / 3);
        if ($factor == 0) return $bytes . ' ' . $sz[$factor];
        return sprintf("%.{$decimals}f %s", $bytes / pow(1024, $factor), $sz[$factor]);
    }

    /**
     * Render import UI
     */
    private function renderImportUI() {
        $html = '<div class="rbox" style="margin-top: 20px;"><div class="rheader">';
        $html .= '📥 ' . mex('Import Data', $this->pag);
        $html .= '</div><div class="rcontent">';

        $html .= '<p>' . mex('Import data from a previously exported package', $this->pag) . '</p>';

        $html .= '<form accept-charset="utf-8" enctype="multipart/form-data" method="post" action="./index.php"><div>';
        $html .= '<input type="hidden" name="MAX_FILE_SIZE" value="500000000">';
        $html .= '<input type="hidden" name="anno" value="' . $this->anno . '">';
        $html .= '<input type="hidden" name="id_sessione" value="' . $this->id_sessione . '">';
        $html .= '<input type="hidden" name="azione" value="SI">';
        $html .= '<input type="hidden" name="export_import" value="1">';

        $html .= '<table style="width: 100%;">';
        $html .= '<tr><td style="width: 30px;"></td><td>';
        $html .= '<label><input type="radio" name="import_mode" value="preview" checked> ';
        $html .= mex('Preview before importing', $this->pag) . '</label><br>';
        $html .= '<label><input type="radio" name="import_mode" value="direct"> ';
        $html .= mex('Import directly', $this->pag) . '</label>';
        $html .= '</td></tr>';
        $html .= '<tr><td style="width: 30px;"></td><td>';
        $html .= '<label><input type="checkbox" name="import_configs" value="1" checked> ';
        $html .= mex('Import configurations', $this->pag) . '</label>';
        $html .= '</td></tr>';
        $html .= '</table>';

        $html .= '<div style="margin-top: 15px;">';
        $html .= mex('Select package file', $this->pag) . ': ';
        $html .= '<input name="import_package" type="file" accept=".zip">';
        $html .= '</div>';

        $html .= '<div style="text-align: center; margin-top: 15px;">';
        $html .= '<button class="ubkp" type="submit" name="start_import" value="1">';
        $html .= '<div>' . mex('Continue', $this->pag) . '</div>';
        $html .= '</button>';
        $html .= '</div>';

        $html .= '</div></form></div></div>';

        return $html;
    }

    /**
     * Render import preview
     */
    public function renderImportPreview($preview_data, $mapping_suggestions) {
        $html = '<div class="rbox"><div class="rheader">';
        $html .= mex('Import Preview', $this->pag);
        $html .= '</div><div class="rcontent">';

        $html .= '<h4>' . mex('Data to be imported:', $this->pag) . '</h4>';
        $html .= '<table style="width: 100%; border-collapse: collapse;">';
        $html .= '<tr><th style="border-bottom: 1px solid #ccc; padding: 8px; text-align: left;">';
        $html .= mex('Table', $this->pag) . '</th>';
        $html .= '<th style="border-bottom: 1px solid #ccc; padding: 8px; text-align: left;">';
        $html .= mex('Rows', $this->pag) . '</th>';
        $html .= '<th style="border-bottom: 1px solid #ccc; padding: 8px; text-align: left;">';
        $html .= mex('Columns', $this->pag) . '</th></tr>';

        foreach ($preview_data as $table_name => $table_info) {
            $mapped_table = $table_name;
            $css_class = 'style="border-bottom: 1px solid #ddd; padding: 8px;"';

            // Check if there's a suggested mapping
            if (isset($mapping_suggestions[$table_name])) {
                $mapped_table = $mapping_suggestions[$table_name]['international_name'];
            }

            $html .= '<tr>';
            $html .= '<td ' . $css_class . '>' . htmlspecialchars($table_name) . '</td>';
            $html .= '<td ' . $css_class . '>' . $table_info['row_count'] . '</td>';
            $html .= '<td ' . $css_class . '>' . implode(', ', $table_info['columns']) . '</td>';
            $html .= '</tr>';
        }

        $html .= '</table>';

        $html .= '<h4 style="margin-top: 20px;">' . mex('Field Mapping', $this->pag) . '</h4>';
        $html .= $this->renderFieldMappingUI($preview_data, $mapping_suggestions);

        return $html;
    }

    /**
     * Render field mapping UI
     */
    private function renderFieldMappingUI($preview_data, $mapping_suggestions) {
        $html = '<form accept-charset="utf-8" method="post" action="./index.php">';
        $html .= '<input type="hidden" name="anno" value="' . $this->anno . '">';
        $html .= '<input type="hidden" name="id_sessione" value="' . $this->id_sessione . '">';
        $html .= '<input type="hidden" name="azione" value="SI">';
        $html .= '<input type="hidden" name="export_import" value="1">';
        $html .= '<input type="hidden" name="confirm_import" value="1">';

        $html .= '<p>' . mex('Review and adjust field mappings if needed', $this->pag) . ':</p>';

        foreach ($preview_data as $table_name => $table_info) {
            $html .= '<fieldset style="border: 1px solid #ccc; padding: 10px; margin: 10px 0;">';
            $html .= '<legend>' . htmlspecialchars($table_name) . '</legend>';

            $fields = $table_info['columns'];
            foreach ($fields as $field) {
                $mapped_field = $field;

                // Check for suggestion
                if (isset($mapping_suggestions[$table_name]) && 
                    isset($mapping_suggestions[$table_name]['suggested_field_mapping'][$field])) {
                    $mapped_field = $mapping_suggestions[$table_name]['suggested_field_mapping'][$field];
                }

                $html .= '<div style="margin: 8px 0;">';
                $html .= '<label>' . htmlspecialchars($field) . ' → ';
                $html .= '<input type="text" name="mapping_' . $table_name . '_' . $field . '" ';
                $html .= 'value="' . htmlspecialchars($mapped_field) . '" style="width: 200px;">';
                $html .= '</label>';
                $html .= '</div>';
            }

            $html .= '</fieldset>';
        }

        $html .= '<div style="text-align: center; margin-top: 20px;">';
        $html .= '<button class="gbk" type="submit">';
        $html .= '<div>' . mex('Confirm Import', $this->pag) . '</div>';
        $html .= '</button>';
        $html .= '</div>';

        $html .= '</form>';

        return $html;
    }

    /**
     * Render import result
     */
    public function renderImportResult($import_stats) {
        $html = '<div class="rbox" style="border-left: 4px solid #4CAF50;">';
        $html .= '<div class="rheader" style="background-color: #4CAF50; color: white;">';
        $html .= '✓ ' . mex('Import Completed', $this->pag);
        $html .= '</div><div class="rcontent">';

        $html .= '<p><strong>' . mex('Import Summary', $this->pag) . ':</strong></p>';
        $html .= '<ul>';
        $html .= '<li>' . mex('Tables processed', $this->pag) . ': ' . $import_stats['tables_processed'] . '</li>';
        $html .= '<li>' . mex('Rows imported', $this->pag) . ': ' . $import_stats['rows_imported'] . '</li>';
        if (!empty($import_stats['errors'])) {
            $html .= '<li style="color: red;">' . mex('Errors', $this->pag) . ': ';
            $html .= '<ul>';
            foreach ($import_stats['errors'] as $error) {
                $html .= '<li>' . htmlspecialchars($error) . '</li>';
            }
            $html .= '</ul></li>';
        }
        $html .= '</ul>';

        $html .= '</div></div>';

        return $html;
    }
}
?>
