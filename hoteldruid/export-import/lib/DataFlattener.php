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
    private $last_error;

    public function __construct($numconnessione, $phpr_tab_pre, $db_type, $logger = null) {
        $this->numconnessione = $numconnessione;
        $this->phpr_tab_pre = $phpr_tab_pre;
        $this->db_type = $db_type;
        $this->logger = $logger;
        $this->last_error = null;
    }

    public function getLastError() {
        return $this->last_error;
    }

    /**
     * Public helper to describe a table (column metadata) using the same logic as flattenTable.
     */
    public function describeTable($table_name) {
        $resolved = $this->resolveTableName($table_name);
        return $this->getTableColumns($resolved);
    }

    /**
     * Flatten a single table to JSON structure
     */
    public function flattenTable($table_name, $table_full_name = null) {
        $this->last_error = null;
        $table_full_name = $this->resolveTableName($table_name, $table_full_name);
        if (!$table_full_name) {
            $this->setError("Table $table_name not found (tried with and without prefix)");
            return null;
        }

        $this->log("Flattening table: $table_full_name");

        // Get table structure
        $columns = $this->getTableColumns($table_full_name);
        if (!$columns) {
            $this->setError("Could not get columns for table $table_full_name");
            return null;
        }

        // Get table data
        $rows = $this->getTableRows($table_full_name);
        if ($rows === null) {
            $this->setError("Could not get rows for table $table_full_name");
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
            } else {
                $this->log('getTableColumns: information_schema query returned no result, will try SHOW COLUMNS fallback');
            }
        } elseif ($this->db_type == 'sqlite' || $this->db_type == 'sqlite3') {
            // For SQLite, use PRAGMA table_info with quoting to tolerate special chars
            $queries = array(
                "PRAGMA table_info('" . str_replace("'", "''", $table_full_name) . "')",
                "PRAGMA table_info(\"" . str_replace("\"", "\"\"", $table_full_name) . "\")",
                "PRAGMA table_info($table_full_name)"
            );

            foreach ($queries as $pragma_query) {
                $result = esegui_query($pragma_query);
                if ($result && numlin_query($result) > 0) {
                    $num_rows = numlin_query($result);
                    for ($i = 0; $i < $num_rows; $i++) {
                        $name = risul_query($result, $i, 'name');
                        $type = risul_query($result, $i, 'type');
                        $notnull = risul_query($result, $i, 'notnull');
                        $default = risul_query($result, $i, 'dflt_value');
                        $columns[] = array(
                            'name' => $name,
                            'type' => $type,
                            'nullable' => $notnull ? false : true,
                            'default' => $default
                        );
                    }
                    chiudi_query($result);
                    break; // got columns
                }
            }

            // Fallback: parse CREATE TABLE statement from sqlite_master when PRAGMA fails
            if (count($columns) === 0) {
                $schema_sql = esegui_query("SELECT sql FROM sqlite_master WHERE type='table' AND name='" . aggslashdb($table_full_name) . "'");
                if ($schema_sql && numlin_query($schema_sql) > 0) {
                    $create_stmt = risul_query($schema_sql, 0, 'sql');
                    chiudi_query($schema_sql);
                    if ($create_stmt) {
                        $columns = $this->parseCreateTableColumns($create_stmt);
                    }
                }
            }
        }

        // Fallback for MySQL/MariaDB if information_schema query fails or returns nothing
        if (($this->db_type == 'mysql' || $this->db_type == 'mysqli') && count($columns) === 0) {
            $fallback_query = "SHOW COLUMNS FROM $table_full_name";
            $this->log('getTableColumns: fallback query ' . $fallback_query);
            $result = esegui_query($fallback_query);
            if ($result) {
                $num_rows = numlin_query($result);
                for ($i = 0; $i < $num_rows; $i++) {
                    $row = risul_query($result, $i);
                    $columns[] = array(
                        'name' => $row['Field'] ?? $row['column_name'] ?? '',
                        'type' => $row['Type'] ?? $row['data_type'] ?? '',
                        'nullable' => (isset($row['Null']) ? ($row['Null'] === 'YES') : true),
                        'default' => $row['Default'] ?? null
                    );
                }
                chiudi_query($result);
            }
        }

        if (count($columns) > 0) return $columns;
        $extra_error = '';
        if (function_exists('ultimo_errore_sqlite') && ($this->db_type == 'sqlite' || $this->db_type == 'sqlite3')) {
            $sqlite_err = ultimo_errore_sqlite();
            if ($sqlite_err) $extra_error = " Last SQLite error: $sqlite_err";
        }
        $this->setError("No columns returned for $table_full_name (db_type={$this->db_type})." . $extra_error);
        return null;
    }

    /**
     * Fallback parser for SQLite CREATE TABLE statements when PRAGMA table_info fails.
     */
    private function parseCreateTableColumns($create_stmt) {
        $columns = array();
        if (!$create_stmt) return $columns;

        // Extract column definitions between the first pair of parentheses
        if (!preg_match('/\((.*)\)/s', $create_stmt, $matches)) {
            return $columns;
        }
        $inside = $matches[1];

        // Split on commas not enclosed in parentheses
        $parts = preg_split('/,(?![^\(]*\))/s', $inside);
        foreach ($parts as $part) {
            $part = trim($part);
            if ($part === '') continue;

            // Skip table-level constraints
            $upper = strtoupper($part);
            if (strpos($upper, 'PRIMARY KEY') === 0 || strpos($upper, 'FOREIGN KEY') === 0 || strpos($upper, 'UNIQUE') === 0 || strpos($upper, 'CHECK') === 0 || strpos($upper, 'CONSTRAINT') === 0) {
                continue;
            }

            // Column name may be quoted with backticks or double quotes or brackets
            if (preg_match('/^(`([^`]+)`|"([^"]+)"|\[([^\]]+)\]|([^\s]+))\s+(.*)$/s', $part, $col_match)) {
                $name = $col_match[2] ?: ($col_match[3] ?: ($col_match[4] ?: $col_match[5]));
                $definition = trim($col_match[6]);
                $type = strtok($definition, " \t\n\r");
                $nullable = (stripos($definition, 'NOT NULL') === false);

                $default = null;
                if (preg_match('/DEFAULT\s+([^\s,]+)/i', $definition, $def_match)) {
                    $default = trim($def_match[1], "'\"");
                }

                $columns[] = array(
                    'name' => $name,
                    'type' => $type,
                    'nullable' => $nullable,
                    'default' => $default
                );
            }
        }

        return $columns;
    }

    /**
     * Get all rows from a table
     */
    private function getTableRows($table_full_name) {
        include_once(__DIR__ . '/../../includes/funzioni.php');

        $rows = array();
        // Quote table name for SQLite to avoid issues with reserved words or special chars
        if ($this->db_type == 'sqlite' || $this->db_type == 'sqlite3') {
            $query = "SELECT * FROM '" . str_replace("'", "''", $table_full_name) . "'";
        } else {
            $query = "SELECT * FROM $table_full_name";
        }
        $result = esegui_query($query);

        if ($result) {
            $num_rows = numlin_query($result);
            for ($i = 0; $i < $num_rows; $i++) {
                $row_data = array();
                // Get all fields for this row (arraylin_query is DB-agnostic across drivers)
                $row_object = arraylin_query($result, $i);
                if (is_array($row_object)) {
                    foreach ($row_object as $idx => $value) {
                        // Map numeric index to column name when available
                        if (isset($result['col'][$idx])) {
                            $field = $result['col'][$idx];
                        } else {
                            $field = is_string($idx) ? $idx : 'col_' . $idx;
                        }
                        $row_data[$field] = $this->standardizeValue($value);
                    }
                }
                $rows[] = $row_data;
                // Periodically report progress every 100 rows if logger available
                if ($this->logger && ($i > 0) && ($i % 100 == 0)) {
                    $this->log("Processed {$i}/{$num_rows} rows for table {$table_full_name}");
                    // allow flushes in long operations
                    if (function_exists('flush')) { @flush(); }
                }
            }
            chiudi_query($result);
            return $rows;
        }

        $extra_error = '';
        if (function_exists('ultimo_errore_sqlite') && ($this->db_type == 'sqlite' || $this->db_type == 'sqlite3')) {
            $sqlite_err = ultimo_errore_sqlite();
            if ($sqlite_err) $extra_error = " Last SQLite error: $sqlite_err";
        }
        $this->setError("Query failed for table $table_full_name." . $extra_error);
        return null;
    }

    /**
     * Resolve the concrete table name to use. Tries prefixed then unprefixed (or supplied full) until a match is found.
     */
    private function resolveTableName($table_name, $explicit_full_name = null) {
        $candidates = array();
        if ($explicit_full_name) {
            $candidates[] = $explicit_full_name;
        } else {
            if ($this->phpr_tab_pre) {
                $candidates[] = $this->phpr_tab_pre . $table_name;
            }
            $candidates[] = $table_name;
        }

        foreach ($candidates as $candidate) {
            if ($this->tableExists($candidate)) {
                return $candidate;
            }
        }

        // If none found, return first candidate to keep error messaging clear
        return isset($candidates[0]) ? $candidates[0] : null;
    }

    /**
     * Check if a table exists for the current driver.
     */
    private function tableExists($table_full_name) {
        include_once(__DIR__ . '/../../includes/funzioni.php');

        if ($this->db_type == 'sqlite' || $this->db_type == 'sqlite3') {
            $query = "SELECT name FROM sqlite_master WHERE type='table' AND name='" . aggslashdb($table_full_name) . "'";
            $res = esegui_query($query);
            $exists = ($res && numlin_query($res) > 0);
            if ($res) chiudi_query($res);
            return $exists;
        }

        if ($this->db_type == 'mysql' || $this->db_type == 'mysqli') {
            $query = "SHOW TABLES LIKE '" . aggslashdb($table_full_name) . "'";
            $res = esegui_query($query);
            $exists = ($res && numlin_query($res) > 0);
            if ($res) chiudi_query($res);
            return $exists;
        }

        if ($this->db_type == 'postgresql') {
            $query = "SELECT to_regclass('" . aggslashdb($table_full_name) . "') as reg";
            $res = esegui_query($query);
            $exists = false;
            if ($res && numlin_query($res) > 0) {
                $exists = (bool) risul_query($res, 0, 'reg');
            }
            if ($res) chiudi_query($res);
            return $exists;
        }

        return false;
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

        $this->log('Starting full database flattening');

        if (!$tables) {
            // Get all tables
            $tables = $this->getAllTables();
        }

        $table_count = is_array($tables) ? count($tables) : 0;
        $this->log('Found ' . $table_count . ' tables to process');

        $flattened_data = array();

        $index = 0;
        foreach ($tables as $table_name) {
            $index++;
            $this->log("Processing table {$index}/{$table_count}: {$table_name}");
            $table_data = $this->flattenTable($table_name);
            if ($table_data) {
                $flattened_data[$table_name] = $table_data;
            }
        }

        $this->log('Completed database flattening');

        return $flattened_data;
    }

    /**
     * Get all tables in database
     */
    public function getAllTables() {
        include_once(__DIR__ . '/../../includes/funzioni.php');

        $tables = array();
        $this->log('getAllTables: starting (db_type=' . ($this->db_type ?? 'NULL') . ')');

        if ($this->db_type == 'postgresql') {
            $query = "SELECT table_name FROM information_schema.tables 
                     WHERE table_schema = 'public' AND table_type = 'BASE TABLE'";
            $this->log('getAllTables: using PostgreSQL query');
        } elseif ($this->db_type == 'mysql' || $this->db_type == 'mysqli') {
            $query = "SELECT TABLE_NAME as table_name FROM information_schema.TABLES 
                     WHERE TABLE_SCHEMA = DATABASE()";
            $this->log('getAllTables: using MySQL query');
        } elseif ($this->db_type == 'sqlite') {
            $query = "SELECT name as table_name FROM sqlite_master 
                     WHERE type='table' AND name NOT LIKE 'sqlite_%'";
            $this->log('getAllTables: using SQLite query');
        }
        if (empty($query)) {
            $this->log('getAllTables: no query constructed (db_type may be unset)', true);
            return $tables;
        }

        $this->log('getAllTables: executing query: ' . $query);
        $result = @esegui_query($query);
        if ($result) {
            $num_rows = numlin_query($result);
            $this->log('getAllTables: query returned ' . $num_rows . ' rows');
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

        $this->log('getAllTables: discovered tables: ' . var_export($tables, true));

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

    private function setError($message) {
        $this->last_error = $message;
        $this->log($message, true);
    }
}
?>
