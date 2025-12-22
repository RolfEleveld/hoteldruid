<?php
/**
 * Exporter.php
 * Main export orchestrator - creates complete export packages in Zip format
 */

class Exporter {
    private $numconnessione;
    private $phpr_tab_pre;
    private $db_type;
    private $export_dir;
    private $package_dir;
    private $logger;
    private $export_id;
    private $export_timestamp;
    private $last_error;
    private $last_stats;
    private $canonical_mapper;

    public function __construct($numconnessione, $phpr_tab_pre, $db_type, $export_base_path, $logger = null) {
        $this->numconnessione = $numconnessione;
        $this->phpr_tab_pre = $phpr_tab_pre;
        $this->db_type = $db_type;
        // Normalize export base directory and ensure we have a writable location
        $normalized_base = rtrim(str_replace('\\', '/', $export_base_path), '/');
        $this->export_dir = $normalized_base;
        $this->logger = $logger;
        $this->export_id = $this->generateUUID();
        $this->export_timestamp = date('Y-m-d\TH:i:s\Z');
        $this->last_error = null;
        $this->last_stats = null;

        include_once(__DIR__ . '/CanonicalMapper.php');
        // Prefer deployment override under C_DATI_PATH/export-import, fallback to bundled file under hoteldruid/export-import
        $mapping_path = null;
        if (defined('C_DATI_PATH')) {
            $candidate = rtrim(C_DATI_PATH, '/\\') . '/export-import/canonical_mapping.json';
            if (@is_file($candidate)) $mapping_path = $candidate;
        }
        if (!$mapping_path) {
            $mapping_path = dirname(__DIR__) . '/canonical_mapping.json';
        }
        $this->canonical_mapper = new CanonicalMapper($phpr_tab_pre, $mapping_path);
    }

    /**
     * Create complete export package
     */
    public function createExportPackage($options = array()) {
        $this->last_error = null;
        $this->log("Starting export package creation");

        // Ensure export directory exists
        if (!@is_dir($this->export_dir)) {
            if (!@mkdir($this->export_dir, 0755, true)) {
                $this->setError("Cannot create export directory at {$this->export_dir}. Check write permissions.");
                return false;
            }
        }

        // Default options
        $options = array_merge(array(
            'include_configs' => true,
            'include_templates' => true,
            'include_documents' => true,
            'export_type' => 'full',
            'source_name' => 'hoteldruid_export'
        ), $options);

        // Create temp working directory
        $temp_dir = $this->export_dir . '/temp_export_' . uniqid();
        if (!@mkdir($temp_dir, 0755, true)) {
            $this->setError("Could not create temp directory at $temp_dir (check permissions and path)");
            return false;
        }

        try {
            // Create manifest
            $manifest = $this->createManifest($options);
            file_put_contents($temp_dir . '/manifest.json', json_encode($manifest, JSON_PRETTY_PRINT | JSON_UNESCAPED_SLASHES));
            $this->log("Created manifest.json");

            // Create metadata directory and files (exclude mapping files from canonical export package)
            @mkdir($temp_dir . '/metadata', 0755, true);
            $metadata = $this->createMetadata($options);
            file_put_contents($temp_dir . '/metadata/export_metadata.json', json_encode($metadata, JSON_PRETTY_PRINT));
            $this->log("Created export_metadata.json");

            // Export data (tables list fetched once for reuse in schema export)
            @mkdir($temp_dir . '/data/tables', 0755, true);
            $tables = $this->getAllTableNames();
            if (empty($tables)) {
                $this->setError("No tables discovered for export (db_type={$this->db_type}, prefix={$this->phpr_tab_pre}). Check connection, permissions, and prefix.");
                $this->deleteDirectory($temp_dir);
                return false;
            }
                $table_stats = $this->exportDatabaseTables($temp_dir . '/data', $tables);
            $this->log("Exported database tables");

            // Export configurations if requested
            if ($options['include_configs']) {
                @mkdir($temp_dir . '/configs', 0755, true);
                $this->exportConfigurations($temp_dir . '/configs');
                $this->log("Exported configurations");
            }

            // Export templates if requested
            if ($options['include_templates']) {
                @mkdir($temp_dir . '/configs/templates', 0755, true);
                $this->exportTemplates($temp_dir . '/configs/templates');
                $this->log("Exported templates");
            }

            // Create schemas directory and export table schemas
            @mkdir($temp_dir . '/schemas/tables', 0755, true);
            $schema_stats = $this->exportTableSchemas($temp_dir . '/schemas/tables', $tables);

            // Create documentation
            @mkdir($temp_dir . '/docs', 0755, true);
            $this->createDocumentation($temp_dir . '/docs', $manifest);
            $this->log("Created documentation");

            // Create Zip package
            $package_filename = $this->createZipPackage($temp_dir, $options['source_name']);
            if ($package_filename) {
                $this->log("Successfully created export package: $package_filename");
                // Cleanup temp directory
                $this->deleteDirectory($temp_dir);
                $this->last_stats = array(
                    'tables_exported' => $table_stats['exported'] ?? 0,
                    'tables_skipped' => $table_stats['skipped'] ?? 0,
                    'schemas_exported' => $schema_stats['exported'] ?? 0,
                    'tables_list' => $table_stats['tables'] ?? array(),
                    'schemas_list' => $schema_stats['tables'] ?? array(),
                    'failed' => $table_stats['failed'] ?? array(),
                    'schemas_failed' => $schema_stats['failed'] ?? array()
                );
                return $package_filename;
            } else {
                if (!$this->last_error) {
                    $this->setError("Failed to create zip package in {$this->export_dir}");
                }
                $this->deleteDirectory($temp_dir);
                return false;
            }

        } catch (Exception $e) {
            $this->setError($e->getMessage());
            $this->deleteDirectory($temp_dir);
            return false;
        }
    }

    /**
     * Create manifest.json
     */
    private function createManifest($options) {
        return array(
            'export_format_version' => '1.0.0',
            'export_timestamp' => $this->export_timestamp,
            'export_id' => $this->export_id,
            'source_system' => array(
                'application' => 'HotelDroid',
                'version' => $this->getApplicationVersion(),
                'database_type' => $this->db_type
            ),
            'source_machine' => array(
                'hostname' => gethostname(),
                'system_name' => 'HotelDroid Export',
                'hosting_system' => 'PHP ' . phpversion(),
                'exported_by' => $this->getCurrentUser()
            ),
            'export_metadata' => 'metadata/export_metadata.json',
            'contents' => array(
                'metadata' => 'metadata/',
                'configurations' => 'configs/',
                'data' => 'data/',
                'schemas' => 'schemas/',
                'docs' => 'docs/'
            ),
            'data_summary' => array(
                'format_notes' => 'All data in JSON format. Zip CRC validates integrity.',
                'export_type' => $options['export_type']
            )
        );
    }

    /**
     * Create metadata about the export
     */
    private function createMetadata($options) {
        return array(
            'export_id' => $this->export_id,
            'export_timestamp' => $this->export_timestamp,
            'export_duration_seconds' => 0, // Would track actual duration
            'database_connection' => array(
                'type' => $this->db_type,
                'host' => $this->getDbHost(),
                'database_name' => $this->getDbName()
            ),
            'export_scope' => array(
                'export_type' => $options['export_type'],
                'total_tables' => count($this->getAllTableNames()),
                'total_rows' => 0, // Would calculate actual count
                'total_config_files' => $options['include_configs'] ? 12 : 0
            ),
            'export_options' => array(
                'include_configs' => $options['include_configs'],
                'include_templates' => $options['include_templates'],
                'include_documents' => $options['include_documents'],
                'format' => 'json',
                'compression' => 'zip_deflate'
            ),
            'data_integrity' => array(
                'note' => 'Zip file CRC32 checks integrity. No separate checksums needed.',
                'zip_algorithm' => 'deflate'
            ),
            'notes' => 'HotelDroid export package for data migration and backup'
        );
    }

    /**
     * Create entity mapping for cross-platform imports
     */
    private function createEntityMapping() {
        return array(
            'entity_mapping' => array(
                'table_translations' => array(
                    'clienti' => array(
                        'international_name' => 'guests',
                        'description' => 'Client/Guest records',
                        'primary_key' => 'idclienti'
                    ),
                    'contratti' => array(
                        'international_name' => 'contracts',
                        'description' => 'Client contracts',
                        'primary_key' => 'idcontratti'
                    ),
                    'appartamenti' => array(
                        'international_name' => 'properties',
                        'description' => 'Apartment/Room definitions',
                        'primary_key' => 'idappartamenti'
                    )
                ),
                'field_translations' => array(
                    'clienti' => array(
                        'idclienti' => 'id',
                        'cognome' => 'last_name',
                        'nome' => 'first_name',
                        'email' => 'email_address',
                        'datainserimento' => 'created_date'
                    ),
                    'contratti' => array(
                        'idcontratti' => 'id',
                        'idclienti' => 'guest_id',
                        'descrizione' => 'description'
                    )
                )
            ),
            'implementation_notes' => array(
                'export_implementation' => 'HotelDroid-PHP',
                'export_locale' => 'it-IT',
                'export_version' => $this->getApplicationVersion(),
                'mapping_version' => '1.0'
            )
        );
    }

    /**
     * Export all database tables
     */
    private function exportDatabaseTables($data_dir, $tables = null) {
        include_once(__DIR__ . '/DataFlattener.php');

        $flattener = new DataFlattener($this->numconnessione, $this->phpr_tab_pre, $this->db_type, $this->logger);
        $all_tables = $tables ?: $this->getAllTableNames();

        $exported = 0;
        $skipped = 0;
        $table_list = array();
        $fail_list = array();

        foreach ($all_tables as $table_name) {
            $table_list[] = $this->canonical_mapper->getCanonicalTable($table_name);
            $table_data = $flattener->flattenTable($table_name);
            if ($table_data) {
                $canonical_data = $this->canonical_mapper->toCanonicalTableData($table_data);
                $filename = $data_dir . '/tables/' . $canonical_data['table_name'] . '.json';
                file_put_contents($filename, json_encode($canonical_data, JSON_PRETTY_PRINT));
                $this->log("Exported table: $table_name as {$canonical_data['table_name']}");
                $exported++;
            } else {
                $reason = $flattener->getLastError();
                $this->log("Skipping table: $table_name. Reason: " . ($reason ?: 'no data/columns'), true);
                if ($reason) {
                    $fail_list[] = $this->canonical_mapper->getCanonicalTable($table_name) . ": $reason";
                } else {
                    $fail_list[] = $this->canonical_mapper->getCanonicalTable($table_name);
                }
                $skipped++;
            }
        }

        return array('exported' => $exported, 'skipped' => $skipped, 'tables' => $table_list, 'failed' => $fail_list);
    }

    private function exportTableSchemas($schema_dir, $tables = null) {
        include_once(__DIR__ . '/DataFlattener.php');
        $flattener = new DataFlattener($this->numconnessione, $this->phpr_tab_pre, $this->db_type, $this->logger);
        $all_tables = $tables ?: $this->getAllTableNames();

        $exported = 0;
        $table_list = array();
        $fail_list = array();

        foreach ($all_tables as $table_name) {
            $table_list[] = $this->canonical_mapper->getCanonicalTable($table_name);
            $columns = $flattener->describeTable($table_name);
            if ($columns) {
                $schema = array(
                    'table_name' => $table_name,
                    'columns' => $columns,
                    'generated_at' => $this->export_timestamp,
                    'db_type' => $this->db_type
                );
                $canonical_schema = $this->canonical_mapper->toCanonicalSchema($schema);
                $filename = $schema_dir . '/' . $canonical_schema['table_name'] . '.json';
                file_put_contents($filename, json_encode($canonical_schema, JSON_PRETTY_PRINT));
                $this->log("Exported schema: $table_name as {$canonical_schema['table_name']}");
                $exported++;
            }
            else {
                $reason = $flattener->getLastError();
                if ($reason) {
                    $fail_list[] = $this->canonical_mapper->getCanonicalTable($table_name) . ": $reason";
                } else {
                    $fail_list[] = $this->canonical_mapper->getCanonicalTable($table_name);
                }
            }
        }

        return array('exported' => $exported, 'tables' => $table_list, 'failed' => $fail_list);
    }

    /**
     * Expose table discovery for diagnostics
     */
    public function debugTableDiscovery() {
        return array(
            'db_type' => $this->db_type,
            'prefix' => $this->phpr_tab_pre,
            'tables' => $this->getAllTableNames()
        );
    }

    /**
     * Export configuration files
     */
    private function exportConfigurations($config_dir) {
        include_once(__DIR__ . '/../../costanti.php');

        $configs = array();

        // Export lingua.php
        if (@is_file(C_DATI_PATH . '/lingua.php')) {
            include(C_DATI_PATH . '/lingua.php');
            $configs['lingua'] = array(
                'source_file' => 'dati/lingua.php',
                'description' => 'Language settings',
                'data' => isset($lingua) ? $lingua : array()
            );
        }

        // Export tema.php
        if (@is_file(C_DATI_PATH . '/tema.php')) {
            include(C_DATI_PATH . '/tema.php');
            $configs['tema'] = array(
                'source_file' => 'dati/tema.php',
                'description' => 'UI theme settings',
                'data' => isset($tema) ? $tema : array()
            );
        }

        file_put_contents($config_dir . '/configurations.json', json_encode(
            array('configurations' => $configs),
            JSON_PRETTY_PRINT
        ));
    }

    /**
     * Export template files
     */
    private function exportTemplates($template_dir) {
        include_once(__DIR__ . '/../../costanti.php');

        $template_paths = array(
            C_DATI_PATH . '/../includes/templates/',
            C_DATI_PATH . '/../includes/hoteld_doc_backup.php'
        );

        foreach ($template_paths as $path) {
            if (@is_file($path)) {
                $filename = basename($path);
                @copy($path, $template_dir . '/' . $filename);
                $this->log("Copied template: $filename");
            }
        }
    }

    /**
     * Create documentation files
     */
    private function createDocumentation($docs_dir, $manifest) {
        // Create EXPORT_INFO.txt
        $export_info = "HotelDroid Export Package\n";
        $export_info .= "========================\n\n";
        $export_info .= "Export ID: " . $manifest['export_id'] . "\n";
        $export_info .= "Created: " . $manifest['export_timestamp'] . "\n";
        $export_info .= "Source System: " . $manifest['source_system']['application'] . " v" . $manifest['source_system']['version'] . "\n";
        $export_info .= "Database Type: " . $manifest['source_system']['database_type'] . "\n";
        $export_info .= "Format Version: " . $manifest['export_format_version'] . "\n\n";

        file_put_contents($docs_dir . '/EXPORT_INFO.txt', $export_info);

        // Create IMPORT_GUIDE.txt
        $import_guide = "How to Import This Package\n";
        $import_guide .= "===========================\n\n";
        $import_guide .= "1. Extract the zip file to a safe location\n";
        $import_guide .= "2. Review manifest.json for source information\n";
        $import_guide .= "3. Use the Import functionality in HotelDroid\n";
        $import_guide .= "4. Select this package and review the data mapping\n";
        $import_guide .= "5. Click Import to proceed\n\n";

        file_put_contents($docs_dir . '/IMPORT_GUIDE.txt', $import_guide);
    }

    /**
     * Create Zip package
     */
    private function createZipPackage($source_dir, $source_name) {
        $timestamp = str_replace(array('-', ':', 'T', 'Z'), '', $this->export_timestamp);
        $package_filename = "export_hoteldruid_{$timestamp}_{$source_name}_v1.zip";
        $package_path = $this->export_dir . '/' . $package_filename;

        // Create zip using system zip command or PHP ZipArchive
        if (class_exists('ZipArchive')) {
            $zip = new ZipArchive();
            $open_res = $zip->open($package_path, ZipArchive::CREATE);
            if ($open_res === true) {
                $this->addDirToZip($source_dir, $zip, '');
                $zip->close();
                @chmod($package_path, 0644);
                return $package_path;
            } else {
                $this->setError("ZipArchive could not create package at $package_path (code: $open_res). Ensure the php_zip/ZipArchive extension is enabled and the target directory is writable.");
            }
        } else {
            $this->setError("ZipArchive extension not available; attempting system zip fallback");
        }

        // Fallback to system zip command
        $cwd = getcwd();
        chdir(dirname($source_dir));
        $zip_cmd = "zip -r \"" . basename($package_path) . "\" \"" . basename($source_dir) . "\"";
        $exit_code = 0;
        @system($zip_cmd, $exit_code);
        chdir($cwd);

        if (is_file($package_path) && filesize($package_path) > 0) {
            return $package_path;
        }

        // On Windows the zip CLI is often missing; recommend enabling ZipArchive
        if (stripos(PHP_OS, 'WIN') === 0) {
            $this->setError("System zip command is not available on Windows. Enable the ZipArchive extension (php_zip) or install a zip utility.");
        } else {
            $this->setError("System zip command failed (exit $exit_code) creating $package_path");
        }
        return false;
    }

    /**
     * Recursively add directory contents to zip
     */
    private function addDirToZip($dir, $zip, $base_path) {
        $files = scandir($dir);
        foreach ($files as $file) {
            if ($file == '.' || $file == '..') continue;

            $file_path = $dir . '/' . $file;
            $zip_path = $base_path ? $base_path . '/' . $file : $file;

            if (is_dir($file_path)) {
                $zip->addEmptyDir($zip_path);
                $this->addDirToZip($file_path, $zip, $zip_path);
            } else {
                $zip->addFile($file_path, $zip_path);
            }
        }
    }

    /**
     * Get all table names (excluding system tables)
     */
    private function getAllTableNames() {
        include_once(__DIR__ . '/../../includes/funzioni.php');

        $tables = array();
        if ($this->db_type == 'postgresql') {
            $query = "SELECT table_name FROM information_schema.tables 
                     WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";
        } elseif ($this->db_type == 'mysql' || $this->db_type == 'mysqli') {
            $query = "SELECT TABLE_NAME as table_name FROM information_schema.TABLES 
                     WHERE TABLE_SCHEMA = DATABASE()";
        } elseif ($this->db_type == 'sqlite' || $this->db_type == 'sqlite3') {
            $query = "SELECT name as table_name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
        } else {
            return array();
        }

        $result = esegui_query($query);
        if ($result) {
            $num_rows = numlin_query($result);
            for ($i = 0; $i < $num_rows; $i++) {
                // risul_query in Hoteldruid expects the field name as third param
                $table_name = risul_query($result, $i, 'table_name');
                if ($table_name === null || $table_name === '') {
                    // Fallback for drivers returning associative row
                    $row = risul_query($result, $i);
                    if (is_array($row) && isset($row['table_name'])) {
                        $table_name = $row['table_name'];
                    }
                }
                if ($table_name === null || $table_name === '') continue;
                // Remove prefix
                if ($this->phpr_tab_pre && strpos($table_name, $this->phpr_tab_pre) === 0) {
                    $table_name = substr($table_name, strlen($this->phpr_tab_pre));
                }
                $tables[] = $table_name;
            }
            chiudi_query($result);
        }

        // Fallback for MySQL/MariaDB if information_schema is blocked or empty
        if (($this->db_type == 'mysql' || $this->db_type == 'mysqli') && empty($tables)) {
            $fallback_query = "SHOW TABLES LIKE '" . $this->phpr_tab_pre . "%'";
            $result = @esegui_query($fallback_query);
            if ($result) {
                $num_rows = numlin_query($result);
                for ($i = 0; $i < $num_rows; $i++) {
                    $row = risul_query($result, $i);
                    $row_values = array_values($row);
                    $table_name = $row_values[0] ?? '';
                    if (strpos($table_name, $this->phpr_tab_pre) === 0) {
                        $table_name = substr($table_name, strlen($this->phpr_tab_pre));
                    }
                    if ($table_name !== '') $tables[] = $table_name;
                }
                chiudi_query($result);
            }
        }

        // Final fallback for MySQL/MariaDB: take all tables from current DB and then strip/filter prefix
        if (($this->db_type == 'mysql' || $this->db_type == 'mysqli') && empty($tables)) {
            $fallback_query_all = "SHOW TABLES";
            $result = @esegui_query($fallback_query_all);
            if ($result) {
                $num_rows = numlin_query($result);
                for ($i = 0; $i < $num_rows; $i++) {
                    $row = risul_query($result, $i);
                    $row_values = array_values($row);
                    $table_name = $row_values[0] ?? '';
                    if ($table_name === '') continue;
                    if ($this->phpr_tab_pre && strpos($table_name, $this->phpr_tab_pre) === 0) {
                        $table_name = substr($table_name, strlen($this->phpr_tab_pre));
                        $tables[] = $table_name;
                    } elseif (!$this->phpr_tab_pre) {
                        $tables[] = $table_name;
                    }
                }
                chiudi_query($result);
            }
        }

        // Fallback for SQLite: ensure tables list if initial query failed
        if (($this->db_type == 'sqlite' || $this->db_type == 'sqlite3') && empty($tables)) {
            $fallback_query_sqlite = "SELECT name as table_name FROM sqlite_master WHERE type='table'";
            $result = @esegui_query($fallback_query_sqlite);
            if ($result) {
                $num_rows = numlin_query($result);
                for ($i = 0; $i < $num_rows; $i++) {
                    $row = risul_query($result, $i);
                    $table_name = $row['table_name'];
                    if ($table_name === '' || strpos($table_name, 'sqlite_') === 0) continue;
                    if ($this->phpr_tab_pre && strpos($table_name, $this->phpr_tab_pre) === 0) {
                        $table_name = substr($table_name, strlen($this->phpr_tab_pre));
                    }
                    $tables[] = $table_name;
                }
                chiudi_query($result);
            }
        }

        return $tables;
    }

    /**
     * Get application version
     */
    private function getApplicationVersion() {
        if (@is_file(__DIR__ . '/../../versione.php')) {
            include(__DIR__ . '/../../versione.php');
            return isset($versione) ? $versione : '3.0';
        }
        return '3.0';
    }

    /**
     * Get database host
     */
    private function getDbHost() {
        return $_SERVER['DB_HOST'] ?? 'localhost';
    }

    /**
     * Get database name
     */
    private function getDbName() {
        return $_SERVER['DB_NAME'] ?? 'hoteldruid';
    }

    /**
     * Get current user
     */
    private function getCurrentUser() {
        return $_SERVER['REMOTE_USER'] ?? $_SERVER['USER'] ?? 'system';
    }

    /**
     * Generate UUID v4
     */
    private function generateUUID() {
        return sprintf(
            '%04x%04x-%04x-%04x-%04x-%04x%04x%04x',
            mt_rand(0, 0xffff), mt_rand(0, 0xffff),
            mt_rand(0, 0xffff),
            mt_rand(0, 0x0fff) | 0x4000,
            mt_rand(0, 0x3fff) | 0x8000,
            mt_rand(0, 0xffff), mt_rand(0, 0xffff), mt_rand(0, 0xffff)
        );
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

    /**
     * Expose last error for UI messages
     */
    public function getLastError() {
        return $this->last_error;
    }

    /**
     * Expose last export stats (tables/schemas counts)
     */
    public function getLastStats() {
        return $this->last_stats;
    }

    private function setError($message) {
        $this->last_error = $message;
        $this->log("ERROR: " . $message, true);
    }
}
?>
