<?php
/**
 * Importer.php
 * Import orchestrator - handles import with data mapping and validation
 */

class Importer {
    private $numconnessione;
    private $phpr_tab_pre;
    private $db_type;
    private $logger;
    private $package_path;
    private $manifest;
    private $entity_mapping;
    private $field_mapping = array(); // User-defined field mappings

    public function __construct($numconnessione, $phpr_tab_pre, $db_type, $package_path, $logger = null) {
        $this->numconnessione = $numconnessione;
        $this->phpr_tab_pre = $phpr_tab_pre;
        $this->db_type = $db_type;
        $this->package_path = $package_path;
        $this->logger = $logger;
    }

    /**
     * Validate import package
     */
    public function validatePackage() {
        $this->log("Validating export package: " . $this->package_path);

        if (!file_exists($this->package_path)) {
            $this->log("ERROR: Package file not found: " . $this->package_path, true);
            return array('valid' => false, 'errors' => array('Package file not found'));
        }

        $errors = array();

        // Extract and read manifest
        $temp_dir = sys_get_temp_dir() . '/import_' . uniqid();
        @mkdir($temp_dir, 0755, true);

        $zip = new ZipArchive();
        if ($zip->open($this->package_path) === true) {
            $zip->extractTo($temp_dir);
            $zip->close();

            // Read manifest
            $manifest_file = $temp_dir . '/manifest.json';
            if (file_exists($manifest_file)) {
                $this->manifest = json_decode(file_get_contents($manifest_file), true);
                if (!$this->manifest) {
                    $errors[] = 'Invalid manifest.json format';
                }
            } else {
                $errors[] = 'manifest.json not found in package';
            }

            // Read entity mapping
            $mapping_file = $temp_dir . '/metadata/entity_mapping.json';
            if (file_exists($mapping_file)) {
                $this->entity_mapping = json_decode(file_get_contents($mapping_file), true);
            }

            // Check required directories
            $required_dirs = array('data/tables', 'metadata');
            foreach ($required_dirs as $dir) {
                if (!is_dir($temp_dir . '/' . $dir)) {
                    $errors[] = "Missing required directory: $dir";
                }
            }

            // Cleanup
            $this->deleteDirectory($temp_dir);

            return array(
                'valid' => count($errors) === 0,
                'errors' => $errors,
                'manifest' => $this->manifest,
                'entity_mapping' => $this->entity_mapping
            );
        } else {
            return array('valid' => false, 'errors' => array('Could not open zip file'));
        }
    }

    /**
     * Get preview of data to be imported
     */
    public function getImportPreview($max_rows = 5) {
        $this->log("Generating import preview");

        $temp_dir = sys_get_temp_dir() . '/import_preview_' . uniqid();
        @mkdir($temp_dir, 0755, true);

        $zip = new ZipArchive();
        if ($zip->open($this->package_path) !== true) {
            return array('error' => 'Could not open zip file');
        }

        $zip->extractTo($temp_dir);
        $zip->close();

        $preview = array();

        // List tables and row counts
        $tables_dir = $temp_dir . '/data/tables';
        if (is_dir($tables_dir)) {
            $files = scandir($tables_dir);
            foreach ($files as $file) {
                if (strpos($file, '.json') !== false) {
                    $table_file = $tables_dir . '/' . $file;
                    $table_data = json_decode(file_get_contents($table_file), true);
                    if ($table_data) {
                        $table_name = str_replace('.json', '', $file);
                        $preview[$table_name] = array(
                            'row_count' => $table_data['row_count'],
                            'columns' => array_column($table_data['columns'], 'name'),
                            'sample_rows' => array_slice($table_data['rows'], 0, $max_rows)
                        );
                    }
                }
            }
        }

        // Cleanup
        $this->deleteDirectory($temp_dir);

        return $preview;
    }

    /**
     * Set field mapping for import
     */
    public function setFieldMapping($source_table, $target_table, $mapping) {
        $this->field_mapping[$source_table] = array(
            'target_table' => $target_table,
            'field_mapping' => $mapping
        );
        $this->log("Set field mapping for table: $source_table -> $target_table");
    }

    /**
     * Import data with validation and mapping
     */
    public function importData($dry_run = false, $import_configs = false) {
        $this->log("Starting import process (dry_run: " . ($dry_run ? 'YES' : 'NO') . ")");

        $temp_dir = sys_get_temp_dir() . '/import_' . uniqid();
        @mkdir($temp_dir, 0755, true);

        $zip = new ZipArchive();
        if ($zip->open($this->package_path) !== true) {
            $this->log("ERROR: Could not open zip file", true);
            return false;
        }

        $zip->extractTo($temp_dir);
        $zip->close();

        $import_stats = array(
            'tables_processed' => 0,
            'rows_imported' => 0,
            'errors' => array()
        );

        try {
            // Import tables
            $tables_dir = $temp_dir . '/data/tables';
            if (is_dir($tables_dir)) {
                $files = scandir($tables_dir);
                foreach ($files as $file) {
                    if (strpos($file, '.json') !== false) {
                        $table_file = $tables_dir . '/' . $file;
                        $table_data = json_decode(file_get_contents($table_file), true);
                        if ($table_data) {
                            $result = $this->importTable($table_data, $dry_run);
                            if ($result) {
                                $import_stats['tables_processed']++;
                                $import_stats['rows_imported'] += $result['rows_imported'];
                            } else {
                                $import_stats['errors'][] = "Failed to import table: " . $table_data['table_name'];
                            }
                        }
                    }
                }
            }

            // Import configurations if requested
            if ($import_configs && !$dry_run) {
                $configs_file = $temp_dir . '/configs/configurations.json';
                if (file_exists($configs_file)) {
                    $configs = json_decode(file_get_contents($configs_file), true);
                    $this->importConfigurations($configs);
                    $this->log("Imported configurations");
                }
            }

            $this->log("Import completed. Tables: " . $import_stats['tables_processed'] . 
                      ", Rows: " . $import_stats['rows_imported']);

        } catch (Exception $e) {
            $this->log("ERROR: " . $e->getMessage(), true);
            $import_stats['errors'][] = $e->getMessage();
        } finally {
            // Cleanup
            $this->deleteDirectory($temp_dir);
        }

        return $import_stats;
    }

    /**
     * Import a single table
     */
    private function importTable($table_data, $dry_run = false) {
        include_once(__DIR__ . '/../../includes/funzioni.php');

        $source_table = $table_data['table_name'];
        $target_table = $this->phpr_tab_pre . $source_table;

        // Check if mapping exists
        if (isset($this->field_mapping[$source_table])) {
            $mapping = $this->field_mapping[$source_table];
            $target_table = $this->phpr_tab_pre . $mapping['target_table'];
            $field_map = $mapping['field_mapping'];
        } else {
            $field_map = array(); // No mapping, use source field names
        }

        $rows_imported = 0;

        try {
            foreach ($table_data['rows'] as $row) {
                // Transform row according to field mapping
                if (!empty($field_map)) {
                    $mapped_row = array();
                    foreach ($field_map as $source_field => $target_field) {
                        if (isset($row[$source_field])) {
                            $mapped_row[$target_field] = $row[$source_field];
                        }
                    }
                    $row = $mapped_row;
                }

                // Build INSERT query
                $fields = array_keys($row);
                $values = array_values($row);

                $field_list = implode(',', $fields);
                $value_placeholders = implode(',', array_fill(0, count($values), '?'));

                $query = "INSERT INTO $target_table ($field_list) VALUES ($value_placeholders)";

                if (!$dry_run) {
                    // Execute query
                    esegui_query($query);
                }

                $rows_imported++;
            }

            $this->log("Imported table: $source_table -> $target_table (" . $rows_imported . " rows)");
            return array('rows_imported' => $rows_imported);

        } catch (Exception $e) {
            $this->log("ERROR importing table $source_table: " . $e->getMessage(), true);
            return false;
        }
    }

    /**
     * Import configurations
     */
    private function importConfigurations($configs) {
        include_once(__DIR__ . '/../../costanti.php');

        if (!isset($configs['configurations'])) {
            return;
        }

        foreach ($configs['configurations'] as $config_name => $config_data) {
            if (!isset($config_data['source_file'])) {
                continue;
            }

            // Reconstruct PHP config file from JSON
            $php_content = "<?php\n";
            $php_content .= "// Auto-generated from export package\n";
            $php_content .= "// Source: " . $config_data['source_file'] . "\n\n";

            // Generate PHP variable assignment
            $var_name = str_replace('.php', '', basename($config_data['source_file']));
            $var_name = str_replace('/', '_', $var_name);

            $php_content .= '$' . $var_name . ' = ' . var_export($config_data['data'], true) . ";\n";
            $php_content .= "?>";

            // Write file
            $target_file = C_DATI_PATH . '/' . basename($config_data['source_file']);
            file_put_contents($target_file, $php_content);
            $this->log("Imported configuration: " . $config_data['source_file']);
        }
    }

    /**
     * Get mapping suggestions based on entity mapping
     */
    public function getMappingSuggestions() {
        if (!$this->entity_mapping) {
            return array();
        }

        $suggestions = array();

        if (isset($this->entity_mapping['entity_mapping']['table_translations'])) {
            $table_translations = $this->entity_mapping['entity_mapping']['table_translations'];

            foreach ($table_translations as $source_table => $translation) {
                $suggestions[$source_table] = array(
                    'international_name' => $translation['international_name'],
                    'suggested_field_mapping' => isset($this->entity_mapping['entity_mapping']['field_translations'][$source_table]) 
                        ? $this->entity_mapping['entity_mapping']['field_translations'][$source_table]
                        : array()
                );
            }
        }

        return $suggestions;
    }

    /**
     * Delete directory recursively
     */
    private function deleteDirectory($dir) {
        if (!is_dir($dir)) return true;
        $files = scandir($dir);
        foreach ($files as $file) {
            if ($file != '.' && $file != '..') {
                $path = $dir . '/' . $file;
                if (is_dir($path)) {
                    $this->deleteDirectory($path);
                } else {
                    @unlink($path);
                }
            }
        }
        return @rmdir($dir);
    }

    /**
     * Log message
     */
    private function log($message, $is_error = false) {
        if ($this->logger) {
            if ($is_error) {
                $this->logger->error($message);
            } else {
                $this->logger->info($message);
            }
        }
    }
}
?>
