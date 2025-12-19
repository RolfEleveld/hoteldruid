<?php
/**
 * export-import-handlers.php
 * Integration file for export/import functionality in crea_backup.php
 * Include this file in crea_backup.php to add export/import features
 */

// Only process if export_import flag is set
if (!isset($_REQUEST['export_import']) || $_REQUEST['export_import'] != '1') {
    return;
}

include_once('./export-import/lib/Exporter.php');
include_once('./export-import/lib/Importer.php');
include_once('./export-import/lib/ExportImportUI.php');

// Initialize UI
$export_import_ui = new ExportImportUI($id_utente, $anno, $id_sessione);

// Handle export
if (!empty($_REQUEST['create_export'])) {
    if ($id_utente == 1) { // Only admin can export
        try {
            $export_dir = C_DATI_PATH . '/../export-import/packages';
            if (!@is_dir($export_dir)) {
                @mkdir($export_dir, 0755, true);
            }

            $exporter = new Exporter($numconnessione, $PHPR_TAB_PRE, $PHPR_DB_TYPE, $export_dir);
            $export_options = array(
                'include_configs' => isset($_REQUEST['export_include_configs']) ? true : false,
                'include_templates' => isset($_REQUEST['export_include_templates']) ? true : false,
                'include_documents' => isset($_REQUEST['export_include_documents']) ? true : false,
                'export_type' => 'full',
                'source_name' => 'prod'
            );

            $package_file = $exporter->createExportPackage($export_options);

            if ($package_file) {
                echo '<div class="rbox" style="border-left: 4px solid #4CAF50;">';
                echo '<div class="rheader" style="background-color: #4CAF50; color: white;">✓ ' . mex('Export Successfully Created', $pag) . '</div>';
                echo '<div class="rcontent">';
                echo '<p>' . mex('Package file', $pag) . ': <strong>' . basename($package_file) . '</strong></p>';
                echo '<p><a href="' . str_replace(dirname(__FILE__), '', $package_file) . '" download style="button">';
                echo mex('Download Package', $pag) . '</a></p>';
                echo '</div></div>';
            } else {
                echo '<div class="rbox" style="border-left: 4px solid #FF6B6B;">';
                echo '<div class="rheader" style="background-color: #FF6B6B; color: white;">✗ ' . mex('Export Failed', $pag) . '</div>';
                echo '<div class="rcontent">' . mex('Could not create export package', $pag) . '</div></div>';
            }
        } catch (Exception $e) {
            echo '<div class="rbox" style="border-left: 4px solid #FF6B6B;">';
            echo '<div class="rheader" style="background-color: #FF6B6B; color: white;">✗ ' . mex('Export Error', $pag) . '</div>';
            echo '<div class="rcontent">' . htmlspecialchars($e->getMessage()) . '</div></div>';
        }
    }
}

// Handle import start
if (!empty($_REQUEST['start_import'])) {
    if ($id_utente == 1) { // Only admin can import
        if (!empty($_FILES['import_package']['tmp_name'])) {
            try {
                $temp_upload = C_DATI_PATH . '/../export-import/temp_' . uniqid() . '.zip';
                if (move_uploaded_file($_FILES['import_package']['tmp_name'], $temp_upload)) {

                    $importer = new Importer($numconnessione, $PHPR_TAB_PRE, $PHPR_DB_TYPE, $temp_upload);

                    // Validate package
                    $validation = $importer->validatePackage();

                    if ($validation['valid']) {
                        // Get preview and mapping suggestions
                        $preview = $importer->getImportPreview();
                        $mapping_suggestions = $importer->getMappingSuggestions();

                        // Check import mode
                        $import_mode = isset($_REQUEST['import_mode']) ? $_REQUEST['import_mode'] : 'preview';

                        if ($import_mode == 'preview') {
                            echo $export_import_ui->renderImportPreview($preview, $mapping_suggestions);
                            // Store temp file path for next step
                            echo '<input type="hidden" name="import_package_temp" value="' . htmlspecialchars($temp_upload) . '">';
                        } else {
                            // Direct import
                            $import_configs = isset($_REQUEST['import_configs']) ? true : false;
                            $stats = $importer->importData(false, $import_configs);
                            echo $export_import_ui->renderImportResult($stats);
                            @unlink($temp_upload);
                        }
                    } else {
                        echo '<div class="rbox" style="border-left: 4px solid #FF6B6B;">';
                        echo '<div class="rheader" style="background-color: #FF6B6B; color: white;">✗ ' . mex('Invalid Package', $pag) . '</div>';
                        echo '<div class="rcontent">';
                        echo '<ul>';
                        foreach ($validation['errors'] as $error) {
                            echo '<li>' . htmlspecialchars($error) . '</li>';
                        }
                        echo '</ul>';
                        echo '</div></div>';
                        @unlink($temp_upload);
                    }
                } else {
                    echo '<div class="rbox" style="border-left: 4px solid #FF6B6B;">';
                    echo '<div class="rheader" style="background-color: #FF6B6B; color: white;">✗ ' . mex('Upload Failed', $pag) . '</div>';
                    echo '<div class="rcontent">' . mex('Could not upload package file', $pag) . '</div></div>';
                }
            } catch (Exception $e) {
                echo '<div class="rbox" style="border-left: 4px solid #FF6B6B;">';
                echo '<div class="rheader" style="background-color: #FF6B6B; color: white;">✗ ' . mex('Import Error', $pag) . '</div>';
                echo '<div class="rcontent">' . htmlspecialchars($e->getMessage()) . '</div></div>';
            }
        } else {
            echo '<div class="rbox" style="border-left: 4px solid #FF6B6B;">';
            echo '<div class="rheader" style="background-color: #FF6B6B; color: white;">✗ ' . mex('No File Selected', $pag) . '</div>';
            echo '<div class="rcontent">' . mex('Please select a package file to import', $pag) . '</div></div>';
        }
    }
}

// Handle import confirmation
if (!empty($_REQUEST['confirm_import'])) {
    if ($id_utente == 1 && !empty($_REQUEST['import_package_temp'])) {
        try {
            $temp_file = $_REQUEST['import_package_temp'];

            if (file_exists($temp_file)) {
                $importer = new Importer($numconnessione, $PHPR_TAB_PRE, $PHPR_DB_TYPE, $temp_file);

                // Get field mappings from request
                $field_mappings = array();
                foreach ($_REQUEST as $key => $value) {
                    if (strpos($key, 'mapping_') === 0) {
                        // Parse: mapping_table_field -> value
                        $parts = explode('_', $key, 3); // mapping, table, field
                        if (count($parts) >= 3) {
                            $table = $parts[1];
                            $field = $parts[2];
                            if (!isset($field_mappings[$table])) {
                                $field_mappings[$table] = array();
                            }
                            $field_mappings[$table][$field] = $value;
                        }
                    }
                }

                // Set field mappings
                foreach ($field_mappings as $table => $mappings) {
                    $importer->setFieldMapping($table, $table, $mappings);
                }

                // Execute import
                $import_configs = isset($_REQUEST['import_configs']) ? true : false;
                $stats = $importer->importData(false, $import_configs);
                echo $export_import_ui->renderImportResult($stats);

                @unlink($temp_file);
            } else {
                echo '<div class="rbox" style="border-left: 4px solid #FF6B6B;">';
                echo '<div class="rheader" style="background-color: #FF6B6B; color: white;">✗ ' . mex('File Not Found', $pag) . '</div>';
                echo '<div class="rcontent">' . mex('Package file not found or expired', $pag) . '</div></div>';
            }
        } catch (Exception $e) {
            echo '<div class="rbox" style="border-left: 4px solid #FF6B6B;">';
            echo '<div class="rheader" style="background-color: #FF6B6B; color: white;">✗ ' . mex('Import Error', $pag) . '</div>';
            echo '<div class="rcontent">' . htmlspecialchars($e->getMessage()) . '</div></div>';
        }
    }
}

// Show export/import UI if not processing
if (empty($_REQUEST['create_export']) && empty($_REQUEST['start_import']) && empty($_REQUEST['confirm_import'])) {
    if (fixset($azione) == "SI" && $id_utente == 1) {
        echo $export_import_ui->renderExportImportSection();
    }
}
?>
