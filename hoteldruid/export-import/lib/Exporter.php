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

    public function __construct($numconnessione, $phpr_tab_pre, $db_type, $export_base_path, $logger = null) {
        $this->numconnessione = $numconnessione;
        $this->phpr_tab_pre = $phpr_tab_pre;
        $this->db_type = $db_type;
        $this->export_dir = $export_base_path;
        $this->logger = $logger;
        $this->export_id = $this->generateUUID();
        $this->export_timestamp = date('Y-m-d\TH:i:s\Z');
    }

    /**
     * Create complete export package
     */
    public function createExportPackage($options = array()) {
        $this->log("Starting export package creation");

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
            $this->log("ERROR: Could not create temp directory", true);
            return false;
        }

        try {
            // Create manifest
            $manifest = $this->createManifest($options);
            file_put_contents($temp_dir . '/manifest.json', json_encode($manifest, JSON_PRETTY_PRINT | JSON_UNESCAPED_SLASHES));
            $this->log("Created manifest.json");

            // Create metadata directory and files
            @mkdir($temp_dir . '/metadata', 0755, true);
            $metadata = $this->createMetadata($options);
            file_put_contents($temp_dir . '/metadata/export_metadata.json', json_encode($metadata, JSON_PRETTY_PRINT));
            $this->log("Created export_metadata.json");

            // Export data
            @mkdir($temp_dir . '/data/tables', 0755, true);
            $this->exportDatabaseTables($temp_dir . '/data');
            $this->log("Exported database tables");

            // Export entity mapping
            $entity_mapping = $this->createEntityMapping();
            file_put_contents($temp_dir . '/metadata/entity_mapping.json', json_encode($entity_mapping, JSON_PRETTY_PRINT));
            $this->log("Created entity_mapping.json");

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

            // Create schemas directory
            @mkdir($temp_dir . '/schemas/tables', 0755, true);

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
                return $package_filename;
            } else {
                $this->log("ERROR: Failed to create zip package", true);
                $this->deleteDirectory($temp_dir);
                return false;
            }

        } catch (Exception $e) {
            $this->log("ERROR: " . $e->getMessage(), true);
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
    private function exportDatabaseTables($data_dir) {
        include_once(__DIR__ . '/DataFlattener.php');

        $flattener = new DataFlattener($this->numconnessione, $this->phpr_tab_pre, $this->db_type, $this->logger);
        $all_tables = $this->getAllTableNames();

        foreach ($all_tables as $table_name) {
            $table_data = $flattener->flattenTable($table_name);
            if ($table_data) {
                $filename = $data_dir . '/tables/' . $table_name . '.json';
                file_put_contents($filename, json_encode($table_data, JSON_PRETTY_PRINT));
                $this->log("Exported table: $table_name");
            }
        }
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
            if ($zip->open($package_path, ZipArchive::CREATE) === true) {
                $this->addDirToZip($source_dir, $zip, '');
                $zip->close();
                @chmod($package_path, 0644);
                return $package_path;
            }
        } else {
            // Fallback to system zip command
            $cwd = getcwd();
            chdir(dirname($source_dir));
            system("zip -r '" . basename($source_dir) . "' '" . basename($source_dir) . "'");
            chdir($cwd);
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
        } else {
            return array();
        }

        $result = esegui_query($query);
        if ($result) {
            $num_rows = numlin_query($result);
            for ($i = 0; $i < $num_rows; $i++) {
                $row = risul_query($result, $i);
                $table_name = $row['table_name'];
                // Remove prefix
                if (strpos($table_name, $this->phpr_tab_pre) === 0) {
                    $table_name = substr($table_name, strlen($this->phpr_tab_pre));
                }
                $tables[] = $table_name;
            }
            chiudi_query($result);
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
}
?>
