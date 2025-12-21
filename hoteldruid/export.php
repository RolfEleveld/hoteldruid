<?php
// Lightweight page to show Export UI
$pag = 'export.php';
$titolo = 'HotelDruid: Export';

include_once('./costanti.php');
include_once('./includes/funzioni.php');

// Basic auth/permission check similar to crea_backup.php
include_once(C_DATI_PATH . '/dati_connessione.php');
// determine user session if present
$id_sessione = $_REQUEST['id_sessione'] ?? '';
// include header
if (!defined('C_NASCONDI_MARCA') || C_NASCONDI_MARCA != 'SI') {
    if (@is_dir('./themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php')) include('./themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php/head.php');
    else include('./includes/head.php');
}

// prepare request to render export UI via handlers
$_REQUEST['export_import'] = '1';
$_REQUEST['azione'] = 'SI';

include_once('./export-import/export-import-handlers.php');

// include footer
if (!defined('C_NASCONDI_MARCA') || C_NASCONDI_MARCA != 'SI') {
    if (@is_dir('./themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php')) include('./themes/' . ($tema[$_REQUEST['id_utente']] ?? '') . '/php/foot.php');
    else include('./includes/foot.php');
}

?>
