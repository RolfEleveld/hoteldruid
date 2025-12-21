<?php
// Export-import UI entry page
$pag = 'export-import/index.php';
$titolo = 'HotelDruid: Export Management';

// Include application constants and helpers from hoteldruid root
include_once(__DIR__ . '/../costanti.php');
// Provide minimal defaults expected by includes/funzioni.php and head.php
$n_var_pag = 0;
$lingua_mex = 'ita';
$lingua = array();
$tema = array();
include_once(__DIR__ . '/../includes/funzioni.php');

// Render header if available
if (!defined('C_NASCONDI_MARCA') || C_NASCONDI_MARCA != 'SI') {
    if (@is_dir(__DIR__ . '/../themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php')) include(__DIR__ . '/../themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php/head.php');
    else include(__DIR__ . '/../includes/head.php');
}

// Ensure handlers treat this as an export UI request
$_REQUEST['export_import'] = '1';
$_REQUEST['azione'] = 'SI';

// Make sure variables expected by handlers are available as plain variables
if (!isset($azione) && isset($_REQUEST['azione'])) $azione = $_REQUEST['azione'];
if (!isset($id_utente) || $id_utente === "") $id_utente = 1; // default to admin for local UI access

// Ensure DB connection and prefixes are available for handlers
if (!isset($PHPR_DB_TYPE) && defined('C_PHPR_DB_TYPE')) $PHPR_DB_TYPE = C_PHPR_DB_TYPE;
if (!isset($PHPR_TAB_PRE) && defined('C_PHPR_TAB_PRE')) $PHPR_TAB_PRE = C_PHPR_TAB_PRE;

// Try to load DB config from common locations (using same conventions as the app)
$dbConfigFiles = array();
if (defined('C_DATI_PATH')) {
    $dbConfigFiles[] = C_DATI_PATH . '/dati_connessione.php';
    $dbConfigFiles[] = C_DATI_PATH . '/connessione_db.php';
}
$dbConfigFiles[] = __DIR__ . '/../dati/dati_connessione.php';
$dbConfigFiles[] = __DIR__ . '/../dati/connessione_db.php';

$dbConfigUsed = null;
foreach ($dbConfigFiles as $cfg) {
    if (@is_file($cfg)) {
        include($cfg);
        $dbConfigUsed = $cfg;
        break;
    }
}

// Apply file-provided defaults when present
if (!isset($PHPR_DB_TYPE) && isset($PHPR_DB_TYPE_FILE)) $PHPR_DB_TYPE = $PHPR_DB_TYPE_FILE;
if (!isset($PHPR_TAB_PRE) && isset($PHPR_TAB_PRE_FILE)) $PHPR_TAB_PRE = $PHPR_TAB_PRE_FILE;
if (!isset($PHPR_DB_TYPE)) $PHPR_DB_TYPE = $PHPR_DB_TYPE_FILE ?? '';
if (!isset($PHPR_TAB_PRE)) $PHPR_TAB_PRE = $PHPR_TAB_PRE_FILE ?? '';

// Fallback defaults if still empty (matches common HotelDruid defaults)
if (empty($PHPR_TAB_PRE)) $PHPR_TAB_PRE = 'phpr_';
if (empty($PHPR_DB_TYPE)) $PHPR_DB_TYPE = 'mysqli';

// Establish DB connection if not already available
if (!isset($numconnessione) || !$numconnessione) {
    $dbFunzioni = __DIR__ . '/../includes/funzioni_' . $PHPR_DB_TYPE . '.php';
    if (@is_file($dbFunzioni)) include_once($dbFunzioni);
    if (function_exists('connetti_db') && isset($PHPR_DB_NAME)) {
        $numconnessione = @connetti_db($PHPR_DB_NAME, $PHPR_DB_HOST, $PHPR_DB_PORT, $PHPR_DB_USER, $PHPR_DB_PASS, $PHPR_LOAD_EXT ?? '');
    }
}

// Expose diagnostic hints for handler error messages
$export_import_db_config_used = $dbConfigUsed;

include_once(__DIR__ . '/export-import-handlers.php');

// Render footer
if (!defined('C_NASCONDI_MARCA') || C_NASCONDI_MARCA != 'SI') {
    if (@is_dir(__DIR__ . '/../themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php')) include(__DIR__ . '/../themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php/foot.php');
    else include(__DIR__ . '/../includes/foot.php');
}

?>
