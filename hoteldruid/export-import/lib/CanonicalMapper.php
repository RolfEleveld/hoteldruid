<?php
/**
 * CanonicalMapper
 * Maintains canonical <-> source mappings for tables and columns.
 * Default behavior is identity mapping when no custom mapping file is present.
 */
class CanonicalMapper {
    private $phpr_tab_pre;
    private $mapping_path;
    private $mapping;

    public function __construct($phpr_tab_pre, $mapping_path = null, $mapping_data = null) {
        $this->phpr_tab_pre = $phpr_tab_pre;
        // Default to the bundled canonical mapping file under export-import/
        if (!$mapping_path) {
            $mapping_path = dirname(__DIR__) . '/canonical_mapping.json';
        }
        $this->mapping_path = $mapping_path;
        $this->mapping = array(
            'tables' => array(
                'source_to_canonical' => array(),
                'canonical_to_source' => array()
            ),
            'columns' => array() // per source table: source_to_canonical / canonical_to_source
        );

        if (is_array($mapping_data) && !empty($mapping_data)) {
            $this->mapping = $this->mergeMapping($this->mapping, $mapping_data);
        } elseif ($mapping_path && is_file($mapping_path)) {
            $file_data = json_decode(file_get_contents($mapping_path), true);
            if (is_array($file_data)) {
                $this->mapping = $this->mergeMapping($this->mapping, $file_data);
            }
        }
    }

    public function exportMapping() {
        return $this->mapping;
    }

    public function getCanonicalTable($source_table) {
        if (isset($this->mapping['tables']['source_to_canonical'][$source_table])) {
            return $this->mapping['tables']['source_to_canonical'][$source_table];
        }
        return $source_table;
    }

    public function getSourceTable($canonical_table) {
        if (isset($this->mapping['tables']['canonical_to_source'][$canonical_table])) {
            return $this->mapping['tables']['canonical_to_source'][$canonical_table];
        }
        return $canonical_table;
    }

    public function getCanonicalColumn($source_table, $source_column) {
        if (isset($this->mapping['columns'][$source_table]['source_to_canonical'][$source_column])) {
            return $this->mapping['columns'][$source_table]['source_to_canonical'][$source_column];
        }
        return $source_column;
    }

    public function getSourceColumn($source_table, $canonical_column) {
        if (isset($this->mapping['columns'][$source_table]['canonical_to_source'][$canonical_column])) {
            return $this->mapping['columns'][$source_table]['canonical_to_source'][$canonical_column];
        }
        return $canonical_column;
    }

    public function registerTable($source_table, $columns = array(), $canonical_table_override = null) {
        $canonical_table = $canonical_table_override ?: $this->getCanonicalTable($source_table);

        // Ensure table mappings are set
        if (!isset($this->mapping['tables']['source_to_canonical'][$source_table])) {
            $this->mapping['tables']['source_to_canonical'][$source_table] = $canonical_table;
        }
        if (!isset($this->mapping['tables']['canonical_to_source'][$canonical_table])) {
            $this->mapping['tables']['canonical_to_source'][$canonical_table] = $source_table;
        }

        // Ensure column mapping containers exist
        if (!isset($this->mapping['columns'][$source_table])) {
            $this->mapping['columns'][$source_table] = array(
                'source_to_canonical' => array(),
                'canonical_to_source' => array()
            );
        }

        foreach ($columns as $col) {
            $name = isset($col['name']) ? $col['name'] : null;
            if (!$name) continue;
            $canonical_col = $this->getCanonicalColumn($source_table, $name);
            // Default to identity if no explicit mapping
            if (!isset($this->mapping['columns'][$source_table]['source_to_canonical'][$name])) {
                $this->mapping['columns'][$source_table]['source_to_canonical'][$name] = $canonical_col;
            }
            if (!isset($this->mapping['columns'][$source_table]['canonical_to_source'][$canonical_col])) {
                $this->mapping['columns'][$source_table]['canonical_to_source'][$canonical_col] = $name;
            }
        }
    }

    public function toCanonicalTableData($table_data) {
        $source_table = $table_data['table_name'];
        $columns = isset($table_data['columns']) ? $table_data['columns'] : array();
        $this->registerTable($source_table, $columns);

        $canonical_table = $this->getCanonicalTable($source_table);
        $canonical_columns = array();
        foreach ($columns as $col) {
            $col_name = isset($col['name']) ? $col['name'] : '';
            $canonical_name = $this->getCanonicalColumn($source_table, $col_name);
            $col['name'] = $canonical_name;
            $canonical_columns[] = $col;
        }

        $canonical_rows = array();
        if (!empty($table_data['rows'])) {
            foreach ($table_data['rows'] as $row) {
                $mapped = array();
                foreach ($row as $field => $value) {
                    $canonical_field = $this->getCanonicalColumn($source_table, $field);
                    $mapped[$canonical_field] = $value;
                }
                $canonical_rows[] = $mapped;
            }
        }

        $table_data['table_name'] = $canonical_table;
        $table_data['columns'] = $canonical_columns;
        $table_data['rows'] = $canonical_rows;
        return $table_data;
    }

    public function toCanonicalSchema($schema) {
        if (!$schema) return $schema;
        $source_table = $schema['table_name'];
        $columns = isset($schema['columns']) ? $schema['columns'] : array();
        $this->registerTable($source_table, $columns);

        $schema['table_name'] = $this->getCanonicalTable($source_table);
        $canon_cols = array();
        foreach ($columns as $col) {
            $col_name = isset($col['name']) ? $col['name'] : '';
            $col['name'] = $this->getCanonicalColumn($source_table, $col_name);
            $canon_cols[] = $col;
        }
        $schema['columns'] = $canon_cols;
        return $schema;
    }

    public function canonicalRowToSource($canonical_table, $row) {
        $source_table = $this->getSourceTable($canonical_table);
        if (!isset($this->mapping['columns'][$source_table])) {
            // Identity mapping fallback
            return $row;
        }
        $mapped = array();
        foreach ($row as $field => $value) {
            $source_field = $this->getSourceColumn($source_table, $field);
            $mapped[$source_field] = $value;
        }
        return $mapped;
    }

    private function mergeMapping($base, $extra) {
        // Merge tables
        if (isset($extra['tables']['source_to_canonical'])) {
            $base['tables']['source_to_canonical'] = array_merge($base['tables']['source_to_canonical'], $extra['tables']['source_to_canonical']);
        }
        if (isset($extra['tables']['canonical_to_source'])) {
            $base['tables']['canonical_to_source'] = array_merge($base['tables']['canonical_to_source'], $extra['tables']['canonical_to_source']);
        }
        // Merge columns per table
        if (isset($extra['columns'])) {
            foreach ($extra['columns'] as $tbl => $maps) {
                if (!isset($base['columns'][$tbl])) {
                    $base['columns'][$tbl] = array('source_to_canonical' => array(), 'canonical_to_source' => array());
                }
                if (isset($maps['source_to_canonical'])) {
                    $base['columns'][$tbl]['source_to_canonical'] = array_merge($base['columns'][$tbl]['source_to_canonical'], $maps['source_to_canonical']);
                }
                if (isset($maps['canonical_to_source'])) {
                    $base['columns'][$tbl]['canonical_to_source'] = array_merge($base['columns'][$tbl]['canonical_to_source'], $maps['canonical_to_source']);
                }
            }
        }
        return $base;
    }
}
?>
