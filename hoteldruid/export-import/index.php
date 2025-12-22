<?php
// Export-import UI entry page
$pag = 'export-import/index.php';

// Normalize working directory to hoteldruid root so shared includes resolve language paths correctly
$__hd_root = realpath(__DIR__ . '/..');
if ($__hd_root) {
    @chdir($__hd_root);
}

// Safe defaults to avoid undefined warnings before localization runs
$lingua_mex = $lingua_mex ?? null; // let real selection happen below
$titolo = $titolo ?? 'HotelDruid';

// Include application constants and helpers from hoteldruid root
include_once($__hd_root . '/costanti.php');
// Provide minimal defaults expected by includes/funzioni.php and head.php
include_once($__hd_root . '/includes/security.php');
include_once($__hd_root . '/includes/template.php');

$mostra_menu = "SI";
$senza_menu = "NO";
$mostra_top = "SI";
$n_var_pag = 0;
$lingua = array();
$tema = array();

// Ensure handlers treat this as an export UI request
$_REQUEST['export_import'] = '1';
$_REQUEST['azione'] = 'SI';

// Make sure variables expected by handlers are available as plain variables
if (!isset($azione) && isset($_REQUEST['azione'])) $azione = $_REQUEST['azione'];
if (!isset($id_utente) || $id_utente === "") $id_utente = 1; // default to admin for local UI access

// Load preferred language from data file when present
if (defined('C_DATI_PATH') && @is_file(C_DATI_PATH . '/lingua.php')) {
    include(C_DATI_PATH . '/lingua.php');
}
// If a user-specific language exists, honor it; else honor request override; else fall back
if (!isset($lingua_mex) && isset($lingua[$id_utente]) && $lingua[$id_utente]) {
    $lingua_mex = $lingua[$id_utente];
}
if (!isset($lingua_mex) && isset($_REQUEST['lingua']) && $_REQUEST['lingua'] !== '') {
    $lingua_mex = $_REQUEST['lingua'];
}
// Ensure lingua_mex is always a non-empty string before loading helpers
if (empty($lingua_mex)) {
    $lingua_mex = 'ita';
}

include_once($__hd_root . '/includes/funzioni.php');

// Now that mex() is available and language resolved, set the localized title
$titolo = mex('Gestione esportazione',$pag);

// Optional debug banner for blank-page diagnostics
if (isset($_REQUEST['debug_export_import']) && $_REQUEST['debug_export_import'] == '1') {
    echo '<div style="font-family: monospace; color: #555; padding: 6px; border: 1px dashed #ccc; margin: 8px 0;">';
    echo '[debug index] pag=' . htmlspecialchars($pag);
    echo ' | export_import=' . htmlspecialchars($_REQUEST['export_import'] ?? '(unset)');
    echo ' | azione=' . htmlspecialchars(isset($azione) ? $azione : '(unset)');
    echo ' | id_utente=' . htmlspecialchars(isset($id_utente) ? $id_utente : '(unset)');
    echo ' | lingua_mex=' . htmlspecialchars(isset($lingua_mex) ? $lingua_mex : '(unset)');
    echo ' | PHPR_TAB_PRE=' . htmlspecialchars(isset($PHPR_TAB_PRE) ? $PHPR_TAB_PRE : '(unset)');
    echo ' | PHPR_DB_TYPE=' . htmlspecialchars(isset($PHPR_DB_TYPE) ? $PHPR_DB_TYPE : '(unset)');
    echo ' | cwd=' . htmlspecialchars(getcwd());
    echo '</div>';
}

// Render header after language/title are ready
if (!defined('C_NASCONDI_MARCA') || C_NASCONDI_MARCA != 'SI') {
    if (@is_dir($__hd_root . '/themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php')) include($__hd_root . '/themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php/head.php');
    else include($__hd_root . '/includes/head.php');
}

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

echo '<div class="rpanel">';
echo '<div class="rbox">';
echo '<div class="rheader">' . htmlspecialchars(mex('Esporta / Importa', $pag)) . '</div>';
echo '<div class="rcontent">';

// Main content
include_once(__DIR__ . '/export-import-handlers.php');

// Bottom action bar (menu link styled like crea_backup)
$anno_hidden = isset($anno) ? $anno : (isset($_REQUEST['anno']) ? $_REQUEST['anno'] : '');
$idsess_hidden = isset($id_sessione) ? $id_sessione : (isset($_REQUEST['id_sessione']) ? $_REQUEST['id_sessione'] : '');
echo '<br/><form accept-charset="utf-8" method="post" action="../inizio.php"><div>';
echo '<input type="hidden" name="anno" value="' . htmlspecialchars($anno_hidden) . '">';
echo '<input type="hidden" name="id_sessione" value="' . htmlspecialchars($idsess_hidden) . '">';
// Localized label via mex so translations flow as in the rest of the app
echo '<button class="gobk" type="submit"><div>' .mex('Torna indietro',$pag). '</div></button>';
echo '</div></form><br/>';

echo '</div>'; // rcontent
echo '</div>'; // rbox
echo '</div>'; // rpanel

// Render footer
if (!defined('C_NASCONDI_MARCA') || C_NASCONDI_MARCA != 'SI') {
    if (@is_dir($__hd_root . '/themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php')) include($__hd_root . '/themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '../php/foot.php');
    else include($__hd_root . '/includes/foot.php');
}

?>
