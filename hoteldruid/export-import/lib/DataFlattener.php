<?php
/**
 * DataFlattener.php
 * Converts database tables to JSON format with proper structure and metadata
 */

class DataFlattener {
    private $numconnessione;
    private $phpr_tab_pre;
    private $db_type;
    private $logger;

    public function __construct($numconnessione, $phpr_tab_pre, $db_type, $logger = null) {
        $this->numconnessione = $numconnessione;
        $this->phpr_tab_pre = $phpr_tab_pre;
        $this->db_type = $db_type;
        $this->logger = $logger;
    }

    /**
     * Flatten a single table to JSON structure
     */
    public function flattenTable($table_name, $table_full_name = null) {
        if (!$table_full_name) {
            $table_full_name = $this->phpr_tab_pre . $table_name;
        }

        $this->log("Flattening table: $table_full_name");

        // Get table structure
        $columns = $this->getTableColumns($table_full_name);
        if (!$columns) {
            $this->log("ERROR: Could not get columns for table $table_full_name", true);
            return null;
        }

        // Get table data
        $rows = $this->getTableRows($table_full_name);
        if ($rows === null) {
            $this->log("ERROR: Could not get rows for table $table_full_name", true);
            return null;
        }

        // Build JSON structure
        $json_structure = array(
            'table_name' => $table_name,
            'schema_version' => '1.0.0',
            'row_count' => count($rows),
            'columns' => $columns,
            'relationships' => $this->getTableRelationships($table_name),
            'rows' => $rows,
            'metadata' => array(
                'exported_count' => count($rows),
                'filtered_out' => 0,
                'notes' => 'Data flattened for JSON export'
            )
        );

        return $json_structure;
    }

    /**
     * Get column definitions for a table
     */
    private function getTableColumns($table_full_name) {
        include_once(__DIR__ . '/../../includes/funzioni.php');

        $columns = array();

        if ($this->db_type == 'postgresql' || $this->db_type == 'mysql' || $this->db_type == 'mysqli') {
            // Get column info from information schema
            if ($this->db_type == 'postgresql') {
                $query = "SELECT column_name, data_type, is_nullable, column_default 
                         FROM information_schema.columns 
                         WHERE table_name = '$table_full_name' 
                         ORDER BY ordinal_position";
            } else { // mysql/mysqli
                $query = "SELECT COLUMN_NAME as column_name, COLUMN_TYPE as data_type, 
                                IS_NULLABLE as is_nullable, COLUMN_DEFAULT as column_default
                         FROM information_schema.COLUMNS 
                         WHERE TABLE_NAME = '$table_full_name' AND TABLE_SCHEMA = DATABASE()
                         ORDER BY ORDINAL_POSITION";
            }

            $result = esegui_query($query);
            if ($result) {
                $num_rows = numlin_query($result);
                for ($i = 0; $i < $num_rows; $i++) {
                    $row = risul_query($result, $i);
                    $columns[] = array(
                        'name' => $row['column_name'],
                        'type' => $row['data_type'],
                        'nullable' => ($row['is_nullable'] == 'YES' || $row['is_nullable'] == 't') ? true : false,
                        'default' => $row['column_default']
                    );
                }
                chiudi_query($result);
            }
        } elseif ($this->db_type == 'sqlite') {
            // For SQLite, use PRAGMA table_info
            $query = "PRAGMA table_info($table_full_name)";
            $result = esegui_query($query);
            if ($result) {
                $num_rows = numlin_query($result);
                for ($i = 0; $i < $num_rows; $i++) {
                    $row = risul_query($result, $i);
                    $columns[] = array(
                        'name' => $row['name'],
                        'type' => $row['type'],
                        'nullable' => $row['notnull'] ? false : true,
                        'default' => $row['dflt_value']
                    );
                }
                chiudi_query($result);
            }
        }

        return count($columns) > 0 ? $columns : null;
    }

    /**
     * Get all rows from a table
     */
    private function getTableRows($table_full_name) {
        include_once(__DIR__ . '/../../includes/funzioni.php');

        $rows = array();
        $query = "SELECT * FROM $table_full_name";
        $result = esegui_query($query);

        if ($result) {
            $num_rows = numlin_query($result);
            for ($i = 0; $i < $num_rows; $i++) {
                $row_data = array();
                // Get all fields for this row
                $row_object = risul_query($result, $i);
                if (is_array($row_object)) {
                    // Clean and standardize data types
                    foreach ($row_object as $field => $value) {
                        $row_data[$field] = $this->standardizeValue($value);
                    }
                }
                $rows[] = $row_data;
            }
            chiudi_query($result);
            return $rows;
        }
        return null;
    }

    /**
     * Standardize data values for JSON export
     */
    private function standardizeValue($value) {
        if ($value === null || $value === '') {
            return null;
        }
        // Check if it looks like a date/timestamp
        if (preg_match('/^\d{4}-\d{2}-\d{2}/', $value)) {
            return $value; // Keep as-is, ISO format
        }
        // Check if numeric
        if (is_numeric($value)) {
            return is_float($value) ? (float)$value : (int)$value;
        }
        return (string)$value;
    }

    /**
     * Get relationships for a table (simplified for now)
     */
    private function getTableRelationships($table_name) {
        // This would need to be enhanced to actually read FK constraints
        // For now, return empty array
        return array();
    }

    /**
     * Flatten entire database
     */
    public function flattenDatabase($tables = null) {
        include_once(__DIR__ . '/../../includes/funzioni.php');

        if (!$tables) {
            // Get all tables
            $tables = $this->getAllTables();
        }

        $flattened_data = array();

        foreach ($tables as $table_name) {
            $table_data = $this->flattenTable($table_name);
            if ($table_data) {
                $flattened_data[$table_name] = $table_data;
            }
        }

        return $flattened_data;
    }

    /**
     * Get all tables in database
     */
    private function getAllTables() {
        include_once(__DIR__ . '/../../includes/funzioni.php');

        $tables = array();

        if ($this->db_type == 'postgresql') {
            $query = "SELECT table_name FROM information_schema.tables 
                     WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";
        } elseif ($this->db_type == 'mysql' || $this->db_type == 'mysqli') {
            $query = "SELECT TABLE_NAME as table_name FROM information_schema.TABLES 
                     WHERE TABLE_SCHEMA = DATABASE()";
        } elseif ($this->db_type == 'sqlite') {
            $query = "SELECT name as table_name FROM sqlite_master 
                     WHERE type='table' AND name NOT LIKE 'sqlite_%'";
        }

        $result = esegui_query($query);
        if ($result) {
            $num_rows = numlin_query($result);
            for ($i = 0; $i < $num_rows; $i++) {
                $row = risul_query($result, $i);
                $table_name = $row['table_name'];
                // Remove table prefix if present
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
