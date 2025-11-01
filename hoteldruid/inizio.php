<?php

##################################################################################
#    HOTELDRUID
#    Copyright (C) 2001-2023 by Marco Maria Francesco De Santis (marco@digitaldruid.net)
#
#    This program is free software: you can redistribute it and/or modify
#    it under the terms of the GNU Affero General Public License as published by
#    the Free Software Foundation, either version 3 of the License, or
#    any later version accepted by Marco Maria Francesco De Santis, which
#    shall act as a proxy as defined in Section 14 of version 3 of the
#    license.
#
#    This program is distributed in the hope that it will be useful,
#    but WITHOUT ANY WARRANTY; without even the implied warranty of
#    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#    GNU Affero General Public License for more details.
#
#    You should have received a copy of the GNU Affero General Public License
#    along with this program.  If not, see <http://www.gnu.org/licenses/>.
##################################################################################

// Configure secure session
if (session_status() == PHP_SESSION_NONE) {
    // Configure secure session settings before starting
    ini_set('session.cookie_httponly', 1);
    ini_set('session.cookie_secure', isset($_SERVER['HTTPS']));
    ini_set('session.use_only_cookies', 1);
    ini_set('session.cookie_samesite', 'Strict');
    ini_set('session.gc_maxlifetime', 1800); // 30 minutes
    session_start();
}

$pag = "inizio.php";
$titolo = "HotelDruid";

// Include security and template systems
include_once("./includes/security.php");
include_once("./includes/template.php");

$var_pag = array();
$var_pag[0] = 'nuovo_mess';
$var_pag[1] = 'mos_tut_dat';
$var_pag[2] = 'mese';
$var_pag[3] = 'tipo_tabella';
$var_pag[4] = 'torna_indietro';
$var_pag[5] = 'logout';
$var_pag[6] = 'vai_anno';
$var_pag[7] = 'origine_vecchia';
$var_pag[8] = 'indietro';
$var_pag[9] = 'canc_mess_periodi';
$n_var_pag = 10;

include("./costanti.php");

# Check if database connection file exists first - if not, show language selection for database setup
if (@is_file(C_DATI_PATH."/dati_connessione.php") != true) {
$show_bar = "NO";
$lingua_mex = "ita"; // Default to Italian
if (@is_dir("./includes/lang/en")) $lingua_mex = "en";
if (@isset($_SERVER['HTTP_ACCEPT_LANGUAGE'])) {
$lingua_browser = explode(",",$_SERVER['HTTP_ACCEPT_LANGUAGE']);
foreach ($lingua_browser as $lang) {
if ($lang == "en") break;
if ($lang == "it") {
$lingua_mex = "ita";
break;
} # fine if ($lang == "it")
if (strlen($lang) == 2 and @is_dir("./includes/lang/$lang")) {
$lingua_mex = $lang;
break;
} # fine if (strlen($lang) == 2 and @is_dir("./includes/lang/$lang"))
} # fine foreach ($lingua_browser as $lang)
} # fine if (@isset($_SERVER['HTTP_ACCEPT_LANGUAGE']))

// Simple HTML output without includes that might trigger database connections
echo "<!DOCTYPE html>
<html>
<head>
<meta charset=\"utf-8\">
<title>HotelDruid</title>
<style>
body { font-family: Arial, sans-serif; margin: 40px; background-color: #f5f5f5; }
.container { max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
.header { text-align: center; margin-bottom: 30px; }
.form-group { margin: 20px 0; }
select { padding: 8px 12px; font-size: 14px; border: 1px solid #ccc; border-radius: 4px; }
.sbutton { background: #007cba; color: white; padding: 12px 24px; border: none; border-radius: 4px; cursor: pointer; font-size: 14px; }
.sbutton:hover { background: #005a8a; }
</style>
</head>
<body>
<div class=\"container\">
<div class=\"header\">
<h3>Benvenuto a HOTELDRUID</h3><br>
HOTELDRUID version 3.0.7, Copyright (C) 2001-2024 Marco M. F. De Santis<br>
HotelDruid comes with ABSOLUTELY NO WARRANTY; <br>for details see the <a href=\"http://www.gnu.org/licenses/agpl-3.0.html\">AGPLv3</a> License.<br>
This is free software, and you are welcome to redistribute it<br>
 under certain conditions; see the AGPLv3 License for details.<br>
</div>
<hr>
<form accept-charset=\"utf-8\" method=\"post\" action=\"creadb.php\">
<div class=\"form-group\">
<label for=\"lingua\">Scegli la lingua / Choose language / Elige idioma:</label><br><br>
<select name=\"lingua\" id=\"lingua\">";
if ($lingua_mex == "ita") $sel = " selected";
else $sel = "";
echo "<option value=\"ita\"$sel>italiano</option>";
$lang_dir = opendir("./includes/lang/");
while ($ini_lingua = readdir($lang_dir)) {
if ($ini_lingua != "." && $ini_lingua != "..") {
$nome_lingua = @file("./includes/lang/$ini_lingua/l_n");
if ($nome_lingua) {
$nome_lingua = trim($nome_lingua[0]);
if ($ini_lingua == $lingua_mex) $sel = " selected";
else $sel = "";
echo "<option value=\"$ini_lingua\"$sel>$nome_lingua</option>";
}
} # fine if ($file != "." && $file != "..")
} # fine while ($file = readdir($lang_dig))
closedir($lang_dir);
echo "</select><br><br>
<input class=\"sbutton\" type=\"submit\" value=\"Crea il database / Create database / Crear base de datos\">
</div>
</form>
</div>
</body>
</html>";
exit; // Stop execution here so we don't try to connect to database
} # fine if (@is_file(C_DATI_PATH."/dati_connessione.php") != true)

include("./includes/funzioni.php");

// Configure secure session
if (session_status() == PHP_SESSION_NONE) {
    // Configure secure session settings before starting
    ini_set('session.cookie_httponly', 1);
    ini_set('session.cookie_secure', isset($_SERVER['HTTPS']));
    ini_set('session.use_only_cookies', 1);
    ini_set('session.cookie_samesite', 'Strict');
    ini_set('session.gc_maxlifetime', 1800); // 30 minutes
    session_start();
}

// Include security and template systems
include_once("./includes/security.php");
include_once("./includes/template.php");
include_once("./includes/menu_generator.php");


unset($numconnessione);

// Enhanced login validation with security features
$login_errors = array();
if ($_SERVER['REQUEST_METHOD'] === 'POST' && !empty($nome_utente_phpr) && !empty($password_phpr)) {
    $csrf_token = $_POST['csrf_token'] ?? '';
    
    $validation_result = enhanced_login_validation($nome_utente_phpr, $password_phpr, $csrf_token);
    
    if (!$validation_result['valid']) {
        $login_errors = $validation_result['errors'];
        // Clear sensitive data
        unset($password_phpr);
    } else {
        $nome_utente_phpr = $validation_result['username'];
        $password_phpr = $validation_result['password'];
    }
}

# Database connection file exists, proceed with normal login logic
$id_utente = controlla_login($numconnessione,$PHPR_TAB_PRE,$id_sessione,$nome_utente_phpr,$password_phpr,$anno);
if ($id_utente and $numconnessione and isset($logout)) {
$tabelle_lock = array($PHPR_TAB_PRE."sessioni");
$tabelle_lock = lock_tabelle($tabelle_lock);
esegui_query("delete from $PHPR_TAB_PRE"."sessioni where idsessioni = '$id_sessione'");
unlock_tabelle($tabelle_lock);
unset($id_sessione);
$id_utente = controlla_login($numconnessione,$PHPR_TAB_PRE,$id_sessione,$nome_utente_phpr,$password_phpr,$anno);
} # fine if ($id_utente and $numconnessione and isset($logout))
if ($id_utente) {




# Controllo se sono stati inseriti i dati permanenti.
if (@is_file(C_DATI_PATH."/dati_connessione.php") != true) {
$show_bar = "NO";
if ($tema[$id_utente] != "base") include("./themes/".$tema[$id_utente]."/php/head.php");
else include("./includes/head.php");
if (@is_dir("./includes/lang/en")) $lingua_mex = "en";
else $lingua_mex = "ita";
if (@isset($_SERVER['HTTP_ACCEPT_LANGUAGE'])) {
$lingua_browser = explode(",",$_SERVER['HTTP_ACCEPT_LANGUAGE']);
foreach ($lingua_browser as $lang) {
if ($lang == "en") break;
if ($lang == "it") {
$lingua_mex = "ita";
break;
} # fine if ($lang == "it")
if (strlen($lang) == 2 and @is_dir("./includes/lang/$lang")) {
$lingua_mex = $lang;
break;
} # fine if (strlen($lang) == 2 and @is_dir("./includes/lang/$lang"))
} # fine foreach ($lingua_browser as $lang)
} # fine if (@isset($_SERVER['HTTP_ACCEPT_LANGUAGE']))
if (@is_file("./COPYING")) $file_copying = "file <a href=\"COPYING\">COPYING</a>";
else $file_copying = "<a href=\"http://www.gnu.org/licenses/agpl-3.0.html\">AGPLv3</a> License";
echo "<div style=\"text-align: center;\"><h3>".mex("Benvenuto a HOTELDRUID",$pag).".</h3><br><br>
HOTELDRUID version ".C_PHPR_VERSIONE_TXT.", Copyright (C) 2001-2024 Marco M. F. De Santis<br>
HotelDruid comes with ABSOLUTELY NO WARRANTY; <br>for details see the $file_copying.<br>
This is free software, and you are welcome to redistribute it<br>
 under certain conditions; see the $file_copying for details.<br>
</div><hr style=\"width: 95%\">
<form accept-charset=\"utf-8\" method=\"post\" action=\"creadb.php\"><div>
<br><br>
".mex("Scegli la lingua",$pag).": <select name=\"lingua\">";
if ($lingua_mex == "ita") $sel = " selected";
else $sel = "";
echo "<option value=\"ita\"$sel>italiano</option>";
$lang_dir = opendir("./includes/lang/");
while ($ini_lingua = readdir($lang_dir)) {
if ($ini_lingua != "." && $ini_lingua != "..") {
$nome_lingua = file("./includes/lang/$ini_lingua/l_n");
$nome_lingua = togli_acapo($nome_lingua[0]);
if ($ini_lingua == $lingua_mex) $sel = " selected";
else $sel = "";
echo "<option value=\"$ini_lingua\"$sel>$nome_lingua</option>";
} # fine if ($file != "." && $file != "..")
} # fine while ($file = readdir($lang_dig))
closedir($lang_dir);
echo "</select><br>
<input class=\"sbutton\" type=\"submit\" value=\"".mex("crea il database",$pag)."\"><br>
</div></form>";
if ($tema[$id_utente] != "base") include("./themes/".$tema[$id_utente]."/php/foot.php");
else include("./includes/foot.php");
} # fine if (@is_file(C_DATI_PATH."/dati_connessione.php") != true)

else {

if (defined('C_CREA_ULTIMO_ACCESSO') and C_CREA_ULTIMO_ACCESSO == "SI") {
$fileaperto = @fopen(C_DATI_PATH."/ultimo_accesso","w+");
@fwrite($fileaperto,date("d-m-Y H:i:s"));
@fclose($fileaperto);
} # fine if (defined('C_CREA_ULTIMO_ACCESSO') and C_CREA_ULTIMO_ACCESSO == "SI")
if (is_file(C_DATI_PATH."/selectperiodi$anno.1.php")) $anno_esistente = "SI";
else {
$anno_esistente = "NO";
$anno_attuale = date("Y",(time() + (C_DIFF_ORE * 3600) - (C_GIORNI_NUOVO_ANNO * 86400)));
} # fine else if (is_file(C_DATI_PATH."/selectperiodi$anno.1.php"))

if ($id_utente != 1) {
$tableprivilegi = $PHPR_TAB_PRE."privilegi";
$tablerelgruppi = $PHPR_TAB_PRE."relgruppi";
$prendi_gruppi = "";
if ($anno_esistente == "NO" and $anno == $anno_attuale and is_file(C_DATI_PATH."/selectperiodi".((int) $anno - 1).".1.php")) $privilegi_annuali_utente = esegui_query("select * from $tableprivilegi where idutente = '$id_utente' and anno = '".((int) $anno - 1)."'");
else $privilegi_annuali_utente = esegui_query("select * from $tableprivilegi where idutente = '$id_utente' and anno = '$anno'");
if (numlin_query($privilegi_annuali_utente) == 0) $anno_utente_attivato = "NO";
else {
$anno_utente_attivato = "SI";
$privilegi_globali_utente = esegui_query("select * from $tableprivilegi where idutente = '$id_utente' and anno = '1'");
$priv_mod_pers = risul_query($privilegi_globali_utente,0,'priv_mod_pers');
if (substr($priv_mod_pers,0,1) != "s") $modifica_pers = "NO";
else $modifica_pers = "SI";
$priv_crea_backup = substr($priv_mod_pers,1,1);
$priv_crea_interconnessioni = substr($priv_mod_pers,3,1);
$priv_gest_pass_cc = substr($priv_mod_pers,5,1);
$priv_ins_clienti = risul_query($privilegi_globali_utente,0,'priv_ins_clienti');
if (substr($priv_ins_clienti,0,1) != "s") $inserimento_nuovi_clienti = "NO";
else $inserimento_nuovi_clienti = "SI";
if (substr($priv_ins_clienti,1,1) != "s" and substr($priv_ins_clienti,1,1) != "p" and substr($priv_ins_clienti,1,1) != "g") $modifica_clienti = "NO";
$vedi_clienti = "NO";
if (substr($priv_ins_clienti,2,1) == "s") $vedi_clienti = "SI";
if (substr($priv_ins_clienti,2,1) == "p") $vedi_clienti = "PROPRI";
if (substr($priv_ins_clienti,2,1) == "g") { $vedi_clienti = "GRUPPI"; $prendi_gruppi = "SI"; }
$priv_messaggi = risul_query($privilegi_globali_utente,0,'priv_messaggi');
$priv_vedi_messaggi = substr($priv_messaggi,0,1);
$priv_inventario = risul_query($privilegi_globali_utente,0,'priv_inventario');
$priv_vedi_beni_inv = substr($priv_inventario,0,1);
$priv_vedi_inv_mag = substr($priv_inventario,2,1);
$priv_vedi_inv_app = substr($priv_inventario,6,1);
$priv_ins_prenota = risul_query($privilegi_annuali_utente,0,'priv_ins_prenota');
$priv_ins_nuove_prenota = substr($priv_ins_prenota,0,1);
$priv_ins_costi = risul_query($privilegi_annuali_utente,0,'priv_ins_costi');
$priv_ins_spese = substr($priv_ins_costi,0,1);
$priv_ins_entrate = substr($priv_ins_costi,1,1);
$priv_mod_prenota = risul_query($privilegi_annuali_utente,0,'priv_mod_prenota');
$priv_mod_prenotazioni = substr($priv_mod_prenota,0,1);
if ($priv_mod_prenotazioni == "g") $prendi_gruppi = "SI";
$priv_mod_costi_agg = substr($priv_mod_prenota,8,1);
$priv_mod_pagato = substr($priv_mod_prenota,10,1);
$priv_mod_prenota_iniziate = substr($priv_mod_prenota,11,1);
$priv_mod_prenota_ore = substr($priv_mod_prenota,12,3);
$priv_mod_checkin = substr($priv_mod_prenota,20,1);
$priv_vedi_tab = risul_query($privilegi_annuali_utente,0,'priv_vedi_tab');
$priv_vedi_tab_mesi = substr($priv_vedi_tab,0,1);
$priv_vedi_tab_prenotazioni = substr($priv_vedi_tab,1,1);
if ($priv_vedi_tab_prenotazioni == "g") $prendi_gruppi = "SI";
$priv_vedi_tab_costi = substr($priv_vedi_tab,2,1);
$priv_vedi_tab_periodi = substr($priv_vedi_tab,3,1);
$priv_vedi_tab_regole = substr($priv_vedi_tab,4,1);
$priv_vedi_tab_appartamenti = substr($priv_vedi_tab,5,1);
$priv_vedi_tab_stat = substr($priv_vedi_tab,6,1);
$priv_vedi_tab_doc = substr($priv_vedi_tab,7,1);
$priv_ins_tariffe = risul_query($privilegi_annuali_utente,0,'priv_ins_tariffe');
$priv_mod_tariffe = substr($priv_ins_tariffe,0,1);
$priv_ins_costi_agg = substr($priv_ins_tariffe,1,1);
$priv_mod_reg1 = substr($priv_ins_tariffe,4,1);
$priv_mod_reg2 = substr($priv_ins_tariffe,5,1);
} # fine else if (numlin_query($privilegi_annuali_utente) == 0)
unset($utenti_gruppi);
$utenti_gruppi[$id_utente] = 1;
if ($prendi_gruppi == "SI") {
$gruppi_utente = esegui_query("select idgruppo from $tablerelgruppi where idutente = '$id_utente' and idgruppo is not NULL ");
$num_gruppi_utente = numlin_query($gruppi_utente);
for ($num1 = 0 ; $num1 < $num_gruppi_utente ; $num1++) {
$idgruppo = risul_query($gruppi_utente,$num1,'idgruppo');
$utenti_gruppo = esegui_query("select idutente from $tablerelgruppi where idgruppo = '$idgruppo' ");
$num_utenti_gruppo = numlin_query($utenti_gruppo);
for ($num2 = 0 ; $num2 < $num_utenti_gruppo ; $num2++) $utenti_gruppi[risul_query($utenti_gruppo,$num2,'idutente')] = 1;
} # fine for $num1
} # fine if ($prendi_gruppi == "SI")
} # fine if ($id_utente != 1)
else {
$anno_utente_attivato = "SI";
$modifica_pers = "SI";
$priv_crea_backup = "s";
$priv_crea_interconnessioni = "s";
$priv_gest_pass_cc = "s";
$inserimento_nuovi_clienti = "SI";
$modifica_clienti = "SI";
$vedi_clienti = "SI";
$priv_vedi_messaggi = "s";
$priv_vedi_beni_inv = "s";
$priv_vedi_inv_mag = "s";
$priv_vedi_inv_app = "s";
$priv_ins_nuove_prenota = "s";
$priv_ins_spese = "s";
$priv_ins_entrate = "s";
$priv_mod_prenotazioni = "s";
$priv_mod_costi_agg = "s";
$priv_mod_pagato = "s";
$priv_mod_prenota_iniziate = "s";
$priv_mod_prenota_ore = "000";
$priv_mod_checkin = "s";
$priv_vedi_tab_mesi = "s";
$priv_vedi_tab_prenotazioni = "s";
$priv_vedi_tab_costi = "s";
$priv_vedi_tab_periodi = "s";
$priv_vedi_tab_regole = "s";
$priv_vedi_tab_appartamenti = "s";
$priv_vedi_tab_doc = "s";
$priv_vedi_tab_stat = "s";
$priv_mod_tariffe = "s";
$priv_mod_reg1 = "s";
$priv_mod_reg2 = "s";
$priv_ins_costi_agg = "s";
} # fine else if ($id_utente != 1)




if (@is_file(C_DATI_PATH."/dati_subordinazione.php")) {
include(C_DATI_PATH."/dati_subordinazione.php");
$installazione_subordinata = "SI";
if (!$numconnessione) {
include(C_DATI_PATH."/dati_connessione.php");
include("./includes/funzioni_$PHPR_DB_TYPE.php");
$numconnessione = connetti_db($PHPR_DB_NAME,$PHPR_DB_HOST,$PHPR_DB_PORT,$PHPR_DB_USER,$PHPR_DB_PASS,$PHPR_LOAD_EXT);
} # fine if (!$numconnessione)
$tablepersonalizza = $PHPR_TAB_PRE."personalizza";
$stile_data = stile_data();
$form_aggiorna_sub = "<form accept-charset=\"utf-8\" method=\"post\" action=\"interconnessioni.php\"><div>
<small>".mex("Ultimo aggiornamento",$pag).": ".formatta_data($ultimo_aggiornamento,$stile_data)."
<input type=\"hidden\" name=\"anno\" value=\"$anno\">
<input type=\"hidden\" name=\"id_sessione\" value=\"$id_sessione\">
<input type=\"hidden\" name=\"azione_ic\" value=\"SI\">
<input type=\"hidden\" name=\"aggiorna_subordinazione\" value=\"SI\">
<input class=\"sbutton\" type=\"submit\" value=\"".mex("Aggiorna",$pag)."\">
</small></div></form>";
$inserimento_nuovi_clienti = "NO";
$modifica_clienti = "NO";
$priv_ins_nuove_prenota = "n";
$priv_ins_spese = "n";
$priv_ins_entrate = "n";
$priv_mod_tariffe = "n";
$priv_mod_reg1 = "n";
$priv_mod_reg2 = "n";
$priv_ins_costi_agg = "n";
$priv_mod_costi_agg = "n";
$priv_mod_checkin = "n";
} # fine if (@is_file(C_DATI_PATH."/dati_subordinazione.php"))
else $form_aggiorna_sub = "";


if ($anno_esistente == "NO" and $anno == $anno_attuale) {
if (!$numconnessione) {
include(C_DATI_PATH."/dati_connessione.php");
include("./includes/funzioni_$PHPR_DB_TYPE.php");
$numconnessione = connetti_db($PHPR_DB_NAME,$PHPR_DB_HOST,$PHPR_DB_PORT,$PHPR_DB_USER,$PHPR_DB_PASS,$PHPR_LOAD_EXT);
} # fine if (!$numconnessione)
$tablepersonalizza = $PHPR_TAB_PRE."personalizza";
if (defined('C_CREA_ANNO_MANUALMENTE') and C_CREA_ANNO_MANUALMENTE == "SI") {
$auto_crea_anno = esegui_query("select valpersonalizza from $tablepersonalizza where idpersonalizza = 'auto_crea_anno' and idutente = '1'");
$auto_crea_anno = risul_query($auto_crea_anno,0,'valpersonalizza');
} # fine if (defined('C_CREA_ANNO_MANUALMENTE') and C_CREA_ANNO_MANUALMENTE == "SI")
else $auto_crea_anno = "SI";
if ($auto_crea_anno == "SI") {
$tableanni = $PHPR_TAB_PRE."anni";
$ultimi_anni = esegui_query("select * from $tableanni order by idanni desc");
$num_ultimi_anni = numlin_query($ultimi_anni);
if ($num_ultimi_anni) $ultimo_anno = risul_query($ultimi_anni,0,'idanni');
else {
$ultimo_anno = "-2";
include_once("./includes/costanti.php");
} # fine else if ($num_ultimi_anni)
if ($anno == ($ultimo_anno + 1) or (!$num_ultimi_anni and defined('C_UTILIZZA_SEMPRE_DEFAULTS') and C_UTILIZZA_SEMPRE_DEFAULTS == "AUTO")) {
if ($num_ultimi_anni) {
$tipo_periodi_prec = risul_query($ultimi_anni,0,'tipo_periodi');
$importa_anno_prec = "SI";
$mese_fine = 4;
} # fine if ($num_ultimi_anni)
else {
$tipo_periodi_prec = "g";
$importa_anno_prec = "NO";
if (date("n") > 8) $mese_fine = 24;
else $mese_fine = 12;
} # fine else if ($num_ultimi_anni)
if ($tipo_periodi_prec == "s") {
$tableperiodi_ua = $PHPR_TAB_PRE."periodi".$ultimo_anno;
$giorno_ini_fine = esegui_query("select datainizio from $tableperiodi_ua where idperiodi = '1'");
$giorno_ini_fine = risul_query($giorno_ini_fine,0,'datainizio');
$giorno_ini_fine = explode("-",$giorno_ini_fine);
$giorno_ini_fine = date("w",mktime(0,0,0,$giorno_ini_fine[1],$giorno_ini_fine[2],$giorno_ini_fine[0]));
} # fine if ($tipo_periodi_prec == "s")
else $giorno_ini_fine = "";
include("./includes/funzioni_costi_agg.php");
include("./includes/funzioni_anno.php");
# metto l'utente come 1 per evitare rallentamenti per la scrittura dei log
$id_utente_orig = $id_utente;
$id_utente = 1;
crea_nuovo_anno($anno,$PHPR_TAB_PRE,$DATETIME,$tipo_periodi_prec,$giorno_ini_fine,"1",$mese_fine,$importa_anno_prec,"SI",$pag);
$id_utente = $id_utente_orig;
$anno_esistente = "SI";
} # fine if ($anno == ($ultimo_anno + 1) or (!$num_ultimi_anni and defined('C_UTILIZZA_SEMPRE_DEFAULTS') and C_UTILIZZA_SEMPRE_DEFAULTS == "AUTO"))
} # fine if ($auto_crea_anno == "SI")
} # fine if ($anno_esistente == "NO" and $anno == $anno_attuale)


if ($anno_esistente == "SI") {
//esiste l'anno richiesto
if ($anno_utente_attivato == "SI") {


if ($numconnessione and $anno and $id_sessione and substr($id_sessione,0,4) != $anno) {
$n_id_sessione = $anno.substr($id_sessione,4);
esegui_query("update $PHPR_TAB_PRE"."sessioni set idsessioni = '$n_id_sessione' where idsessioni = '$id_sessione' ");
$id_sessione = $n_id_sessione;
} # fine if ($anno and $id_sessione and substr($id_sessione,0,4) != $anno)

if ($tema[$id_utente] != "base") include("./themes/".$tema[$id_utente]."/php/head.php");
else include("./includes/head.php");

// Load user information for dashboard display
$nome_utente = array();
if ($numconnessione) {
    $utenti_query = esegui_query("select idutenti, nome_utente from $tableutenti order by idutenti");
    $num_utenti = numlin_query($utenti_query);
    for ($i = 0; $i < $num_utenti; $i++) {
        $uid = risul_query($utenti_query, $i, 'idutenti');
        $nome = risul_query($utenti_query, $i, 'nome_utente');
        $nome_utente[$uid] = $nome;
    }
}
// Fallback for when user data is not available
if (!isset($nome_utente[$id_utente]) || empty($nome_utente[$id_utente])) {
    $nome_utente[$id_utente] = 'Admin';
}

// Get hotel name from database or use default
$nome_hotel = 'Villa Annunziata'; // Default fallback
if ($numconnessione) {
    // Try to get hotel name from personalizza table
    $query_nome_hotel = esegui_query("select valpersonalizza from $tablepersonalizza where idpersonalizza = 'nome_hotel' and idutente = '1'");
    if (numlin_query($query_nome_hotel) > 0) {
        $nome_dal_db = trim(risul_query($query_nome_hotel, 0, 'valpersonalizza'));
        if (!empty($nome_dal_db) && $nome_dal_db != '#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&') {
            $nome_hotel = $nome_dal_db;
        }
    }
    // If not found, try dati_struttura field 
    if ($nome_hotel == 'Villa Annunziata') {
        $query_dati_struttura = esegui_query("select valpersonalizza from $tablepersonalizza where idpersonalizza = 'dati_struttura' and idutente = '1'");
        if (numlin_query($query_dati_struttura) > 0) {
            $dati_struttura = risul_query($query_dati_struttura, 0, 'valpersonalizza');
            // Parse structured data to extract hotel name (if format allows)
            if (!empty($dati_struttura) && $dati_struttura != '#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&') {
                $parti_dati = explode('#@&', $dati_struttura);
                if (count($parti_dati) > 0 && !empty(trim($parti_dati[0]))) {
                    $nome_hotel = trim($parti_dati[0]);
                }
            }
        }
    }
}

if (!$numconnessione and (!defined('C_MOSTRA_COPYRIGHT') or C_MOSTRA_COPYRIGHT != "NO")) echo "<table style=\"background-color: #ffffff; width: 99%; height: 97%; margin-right: auto; margin-left: auto;\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td style=\"vertical-align: top;\">";

echo "$form_aggiorna_sub";

if ($tema[$id_utente] != "base") include("./themes/".$tema[$id_utente]."/php/menu.php");
else $hide_default_menu = 0;
if (!$hide_default_menu) {
    // Initialize variables for modern menu system
    $vai_anno = isset($_GET['vai_anno']) ? $_GET['vai_anno'] : '';
    $menu_data = array(); // Initialize menu data if needed
    
    // Use the modern menu system
    include("./includes/modern_menu_display.php");


} # fine if (!$hide_default_menu)

// Modern Hotel Management Dashboard
echo '<div style="padding: 20px; max-width: 1200px; margin: 0 auto;">
    <div style="background: linear-gradient(135deg, #007cba 0%, #005a8a 100%); color: white; padding: 30px; border-radius: 12px; margin-bottom: 30px; text-align: center; box-shadow: 0 4px 15px rgba(0,0,0,0.1);">
        <h2 style="margin: 0 0 10px 0; font-size: 28px; font-weight: 300;">'.mex("Benvenuto a", $pag).' '.$nome_hotel.'</h2>
        <p style="margin: 0; font-size: 16px; opacity: 0.9;">'.mex("Centro di Controllo Gestione Hotel", $pag).' - '.date("d F Y").'</p>
    </div>
    
    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 20px; margin-bottom: 30px;">
        <div style="background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-left: 4px solid #4CAF50;">
            <h3 style="margin: 0 0 15px 0; color: #333;">🏨 '.mex("Camere", $pag).'</h3>
            <p style="margin: 0 0 15px 0; color: #666;">'.mex("Gestione appartamenti", $pag).'</p>
            <a href="visualizza_tabelle.php?anno='.$anno.'&id_sessione='.$id_sessione.'&tipo_tabella=appartamenti" 
               style="display: inline-block; background: #4CAF50; color: white; padding: 8px 16px; text-decoration: none; border-radius: 6px;">
                '.mex("Gestisci Camere", $pag).'
            </a>
        </div>
        
        <div style="background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-left: 4px solid #2196F3;">
            <h3 style="margin: 0 0 15px 0; color: #333;">📅 '.mex("Prenotazioni", $pag).'</h3>
            <p style="margin: 0 0 15px 0; color: #666;">'.mex("Gestione reservas", $pag).'</p>
            <a href="visualizza_tabelle.php?anno='.$anno.'&id_sessione='.$id_sessione.'&tipo_tabella=prenotazioni" 
               style="display: inline-block; background: #2196F3; color: white; padding: 8px 16px; text-decoration: none; border-radius: 6px;">
                '.mex("Ver Prenotazioni", $pag).'
            </a>
        </div>
        
        <div style="background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-left: 4px solid #FF9800;">
            <h3 style="margin: 0 0 15px 0; color: #333;">👥 '.mex("Clienti", $pag).'</h3>
            <p style="margin: 0 0 15px 0; color: #666;">'.mex("Gestione ospiti", $pag).'</p>
            <a href="clienti.php?anno='.$anno.'&id_sessione='.$id_sessione.'" 
               style="display: inline-block; background: #FF9800; color: white; padding: 8px 16px; text-decoration: none; border-radius: 6px;">
                '.mex("Gestisci Clienti", $pag).'
            </a>
        </div>
        
        <div style="background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-left: 4px solid #9C27B0;">
            <h3 style="margin: 0 0 15px 0; color: #333;">📊 '.mex("Calendario", $pag).'</h3>
            <p style="margin: 0 0 15px 0; color: #666;">'.mex("Vista calendario", $pag).'</p>
            <a href="visualizza_tabelle.php?anno='.$anno.'&id_sessione='.$id_sessione.'&tipo_tabella=mesi" 
               style="display: inline-block; background: #9C27B0; color: white; padding: 8px 16px; text-decoration: none; border-radius: 6px;">
                '.mex("Vedi Calendario", $pag).'
            </a>
        </div>
    </div>
    
    <div style="background: white; padding: 30px; border-radius: 12px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); margin-bottom: 30px;">
        <h3 style="margin: 0 0 20px 0; color: #333; text-align: center;">'.mex("Azioni Rapide", $pag).'</h3>
        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px;">
            <a href="inserimento.php?anno='.$anno.'&id_sessione='.$id_sessione.'" 
               style="display: block; background: linear-gradient(135deg, #4CAF50, #45a049); color: white; padding: 20px; text-decoration: none; border-radius: 8px; text-align: center;">
                <div style="font-size: 24px; margin-bottom: 8px;">➕</div>
                <div style="font-weight: bold;">'.mex("Nuova Prenotazione", $pag).'</div>
            </a>
            <a href="visualizza_tabelle.php?anno='.$anno.'&id_sessione='.$id_sessione.'&tipo_tabella=appartamenti" 
               style="display: block; background: linear-gradient(135deg, #2196F3, #1976D2); color: white; padding: 20px; text-decoration: none; border-radius: 8px; text-align: center;">
                <div style="font-size: 24px; margin-bottom: 8px;">🏨</div>
                <div style="font-weight: bold;">'.mex("Aggiungi Camera", $pag).'</div>
            </a>
            <a href="clienti.php?anno='.$anno.'&id_sessione='.$id_sessione.'&inserimento_nuovo_cliente=SI" 
               style="display: block; background: linear-gradient(135deg, #FF9800, #F57C00); color: white; padding: 20px; text-decoration: none; border-radius: 8px; text-align: center;">
                <div style="font-size: 24px; margin-bottom: 8px;">👤</div>
                <div style="font-weight: bold;">'.mex("Nuovo Cliente", $pag).'</div>
            </a>
            <a href="visualizza_tabelle.php?anno='.$anno.'&id_sessione='.$id_sessione.'&tipo_tabella=statistiche" 
               style="display: block; background: linear-gradient(135deg, #9C27B0, #7B1FA2); color: white; padding: 20px; text-decoration: none; border-radius: 8px; text-align: center;">
                <div style="font-size: 24px; margin-bottom: 8px;">📊</div>
                <div style="font-weight: bold;">'.mex("Report", $pag).'</div>
            </a>
        </div>
    </div>
    
    <div style="background: white; padding: 25px; border-radius: 12px; box-shadow: 0 2px 10px rgba(0,0,0,0.08);">
        <h3 style="margin: 0 0 15px 0; color: #333;">'.mex("Stato Sistema", $pag).'</h3>
        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px;">
            <div style="padding: 15px; background: #f8f9fa; border-radius: 6px; border-left: 3px solid #4CAF50;">
                <div style="font-weight: bold; color: #333; margin-bottom: 5px;">'.mex("Anno Attivo", $pag).'</div>
                <div style="color: #666; font-size: 24px; font-weight: bold;">'.$anno.'</div>
            </div>
            <div style="padding: 15px; background: #f8f9fa; border-radius: 6px; border-left: 3px solid #2196F3;">
                <div style="font-weight: bold; color: #333; margin-bottom: 5px;">'.mex("Utente", $pag).'</div>
                <div style="color: #666;">'.$nome_utente[$id_utente].'</div>
            </div>
            <div style="padding: 15px; background: #f8f9fa; border-radius: 6px; border-left: 3px solid #FF9800;">
                <div style="font-weight: bold; color: #333; margin-bottom: 5px;">'.mex("Versione", $pag).'</div>
                <div style="color: #666;">HotelDruid 3.0.7</div>
            </div>
        </div>
    </div>
</div>

<style>
@media (max-width: 768px) {
    .dashboard-container { padding: 15px !important; }
    .stats-grid { grid-template-columns: 1fr !important; }
    .welcome-section h2 { font-size: 22px !important; }
    .stat-card { padding: 20px !important; }
}
</style>';

# You are not authorized to remove the following copyright notice. Ask for permission info@digitaldruid.net
if (!$numconnessione and (!defined('C_MOSTRA_COPYRIGHT') or C_MOSTRA_COPYRIGHT != "NO")) {
echo "</td></tr>
<tr><td style=\"background-color: #ffffff; height: 57px; color: #000000; font-size: 11px; text-align: center; vertical-align: bottom;\">
Website <a style=\"color: #000000;\" href=\"./mostra_sorgente.php\">engine code</a> is copyright © by DigitalDruid.Net.
<a style=\"color: #000000;\" href=\"http://www.hoteldruid.com\">HotelDruid</a> is a free software released under the GNU/AGPL.
</td></tr></table>";
} # fine if (!$numconnessione and (!defined('C_MOSTRA_COPYRIGHT') or C_MOSTRA_COPYRIGHT != "NO"))

if ($tema[$id_utente] != "base") include("./themes/".$tema[$id_utente]."/php/foot.php");
else include("./includes/foot.php");




} # fine if ($anno_utente_attivato == "SI")
} # fine if ($anno_esistente == "SI")



else {
# Non esiste l'anno richiesto

$show_bar = "NO";
if ($tema[$id_utente] != "base") include("./themes/".$tema[$id_utente]."/php/head.php");
else include("./includes/head.php");
echo "$form_aggiorna_sub";
$anno_attuale = date("Y",(time() + (C_DIFF_ORE * 3600) - (C_GIORNI_NUOVO_ANNO * 86400)));
if (@is_file(C_DATI_PATH."/selectperiodi$anno_attuale.1.php")) $anno_attuale_esist = 1;
else $anno_attuale_esist = 0;
if ($anno_corrente == ($anno_attuale + 1) and @is_file(C_DATI_PATH."/selectperiodi$anno_corrente.1.php")) {
$anno_attuale = $anno_corrente;
$anno_attuale_esist = 1;
} # fine if ($anno_corrente == ($anno_attuale + 1) and @is_file(C_DATI_PATH."/selectperiodi$anno_corrente.1.php"))

if (controlla_anno($anno) == "NO" or $id_utente != 1 or (isset($installazione_subordinata) and $installazione_subordinata == "SI") or (defined('C_CREA_ANNO_NON_ATTUALE') and C_CREA_ANNO_NON_ATTUALE == "NO" and $anno != $anno_corrente)) {
if (controlla_anno($anno) == "SI" and $id_utente != 1) echo mex("Questo utente non ha i privilegi per creare nuovi anni",$pag).".<br>";
else echo mex("Il formato dell'anno richiesto è sbagliato",$pag).".<br>";
if ($anno_attuale == $anno) $anno_attuale = "";
} # fine if (controlla_anno($anno) == "NO" or $id_utente != 1 or (isset($installazione_subordinata) and...
else {

echo "<br> ".mex("Non esiste l'anno ",$pag).$anno.mex(" nel database",$pag).". <br>";
if ($anno_attuale_esist and $anno > $anno_attuale) {
$data_crea_anno = formatta_data(date("Y-m-d",mktime(0,0,0,1,(C_GIORNI_NUOVO_ANNO + 1),($anno_attuale + 1))));
if (defined('C_CREA_ANNO_MANUALMENTE') and (C_CREA_ANNO_MANUALMENTE == "SI" or (C_CREA_ANNO_MANUALMENTE == "NUOVO" and $anno == $anno_corrente))) echo "<br>".mex("<span class=\"colred\">Avviso</span>: è consigliabile attendere fino al",$pag)." $data_crea_anno ".mex("per creare il nuovo anno, nel frattempo si possono aggiungere periodi oltre il",$pag);
else echo "<br><span class=\"colinfo\">".mex("Nota",$pag)."</span>: ".mex("l'anno",$pag)." $anno_attuale ".mex("verrà archiviato automaticamente il",$pag)." $data_crea_anno, ".mex("nel frattempo si possono aggiungere periodi oltre il",$pag);
echo " $anno_attuale ".mex("dalla",$pag)."
 <a href=\"./visualizza_tabelle.php?anno=$anno_attuale&amp;id_sessione=$id_sessione&amp;tipo_tabella=periodi#agg_per\">".mex("tabella con i periodi e le tariffe",$pag)."</a> ".mex("anche senza creare un nuovo anno",$pag).".<br>";
} # fine if ($anno_attuale_esist and $anno > $anno_attuale)
if ((!$anno_attuale_esist and $anno == $anno_attuale) or $anno < $anno_attuale or (defined('C_CREA_ANNO_MANUALMENTE') and (C_CREA_ANNO_MANUALMENTE == "SI") or (C_CREA_ANNO_MANUALMENTE == "NUOVO" and $anno == $anno_corrente))) {
echo "<br><form accept-charset=\"utf-8\" method=\"post\" action=\"creaanno.php\"><div>
<input type=\"hidden\" name=\"anno\" value=\"$anno\">
<input type=\"hidden\" name=\"id_sessione\" value=\"$id_sessione\">
<input type=\"hidden\" name=\"nuovo_mess\" value=\"".htmlspecialchars(fixstr($nuovo_mess))."\">
<input class=\"sbutton\" type=\"submit\" name=\"creaanno\" value=\"".mex("Crea l'anno",$pag)." $anno \">
 ".mex("con periodi",$pag).":<br>";
unset($tipo_periodi_obbligati);
$checked_g = "";
$checked_s = "";
if (!$numconnessione) {
include(C_DATI_PATH."/dati_connessione.php");
include("./includes/funzioni_$PHPR_DB_TYPE.php");
$numconnessione = connetti_db($PHPR_DB_NAME,$PHPR_DB_HOST,$PHPR_DB_PORT,$PHPR_DB_USER,$PHPR_DB_PASS,$PHPR_LOAD_EXT);
} # fine if (!$numconnessione)
$tableanni = $PHPR_TAB_PRE."anni";
$tipo_periodi_esistenti = esegui_query("select * from $tableanni order by idanni desc");
if (numlin_query($tipo_periodi_esistenti) != 0) $tipo_periodi_prec = risul_query($tipo_periodi_esistenti,0,'tipo_periodi');
else $tipo_periodi_prec = "";
if ($tipo_periodi_prec == "s") $checked_s = " checked";
else $checked_g = " checked";
if (defined('C_CAMBIA_TIPO_PERIODI') and C_CAMBIA_TIPO_PERIODI == "NO") $tipo_periodi_obbligati = $tipo_periodi_prec;
if (empty($tipo_periodi_obbligati) or $tipo_periodi_obbligati == "s") {
echo "<label><input type=\"radio\" name=\"tipo_periodi\" value=\"s\"$checked_s>
".mex("settimanali",$pag)."</label> (<em>".mex("obsoleti",$pag)."</em>): 
 <select name=\"giorno_ini_fine\">
<option value=\"0\">".mex("Domenica",$pag)."</option>
<option value=\"1\">".mex("Lunedì",$pag)."</option>
<option value=\"2\">".mex("Martedì",$pag)."</option>
<option value=\"3\">".mex("Mercoledì",$pag)."</option>
<option value=\"4\">".mex("Giovedì",$pag)."</option>
<option value=\"5\">".mex("Venerdì",$pag)."</option>
<option value=\"6\" selected>".mex("Sabato",$pag)."</option>
</select> ".mex("come giorno di inizio/fine locazione",$pag)."<br>";
} # fine if (empty($tipo_periodi_obbligati) or $tipo_periodi_obbligati == "s")
if (empty($tipo_periodi_obbligati) or $tipo_periodi_obbligati == "g") {
echo "<label><input type=\"radio\" name=\"tipo_periodi\" value=\"g\"$checked_g>
".mex("giornalieri",$pag)."</label><br>";
} # fine if (empty($tipo_periodi_obbligati) or $tipo_periodi_obbligati == "g")
$sel_12 = "";
$sel_24 = "";
if (date("n") > 8) $sel_24 = " selected";
else $sel_12 = " selected";
echo "".mex("e prenotazioni da",$pag)."
 <select name=\"mese_ini\">
<option value=\"1\" selected>".mex("Gennaio",$pag)."</option>
<option value=\"2\">".mex("Febbraio",$pag)."</option>
<option value=\"3\">".mex("Marzo",$pag)."</option>
<option value=\"4\">".mex("Aprile",$pag)."</option>
<option value=\"5\">".mex("Maggio",$pag)."</option>
<option value=\"6\">".mex("Giugno",$pag)."</option>
<option value=\"7\">".mex("Luglio",$pag)."</option>
<option value=\"8\">".mex("Agosto",$pag)."</option>
<option value=\"9\">".mex("Settembre",$pag)."</option>
<option value=\"10\">".mex("Ottobre",$pag)."</option>
<option value=\"11\">".mex("Novembre",$pag)."</option>
<option value=\"12\">".mex("Dicembre",$pag)."</option>
</select> ".mex("a",$pag)."
 <select name=\"mese_fine\">
<option value=\"1\">".mex("Gennaio",$pag)."</option>
<option value=\"2\">".mex("Febbraio",$pag)."</option>
<option value=\"3\">".mex("Marzo",$pag)."</option>
<option value=\"4\">".mex("Aprile",$pag)."</option>
<option value=\"5\">".mex("Maggio",$pag)."</option>
<option value=\"6\">".mex("Giugno",$pag)."</option>
<option value=\"7\">".mex("Luglio",$pag)."</option>
<option value=\"8\">".mex("Agosto",$pag)."</option>
<option value=\"9\">".mex("Settembre",$pag)."</option>
<option value=\"10\">".mex("Ottobre",$pag)."</option>
<option value=\"11\">".mex("Novembre",$pag)."</option>
<option value=\"12\"$sel_12>".mex("Dicembre",$pag)."</option>
<option value=\"13\">".mex("Gen",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"14\">".mex("Feb",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"15\">".mex("Mar",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"16\">".mex("Apr",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"17\">".mex("Mag",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"18\">".mex("Giu",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"19\">".mex("Lug",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"20\">".mex("Ago",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"21\">".mex("Set",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"22\">".mex("Ott",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"23\">".mex("Nov",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>
<option value=\"24\"$sel_24>".mex("Dic",'giorni_mesi.php').". ".mex("anno successivo",$pag)."</option>";
$n_mese[1] = mex("Gen",'giorni_mesi.php');
$n_mese[2] = mex("Feb",'giorni_mesi.php');
$n_mese[3] = mex("Mar",'giorni_mesi.php');
$n_mese[4] = mex("Apr",'giorni_mesi.php');
$n_mese[5] = mex("Mag",'giorni_mesi.php');
$n_mese[6] = mex("Giu",'giorni_mesi.php');
$n_mese[7] = mex("Lug",'giorni_mesi.php');
$n_mese[8] = mex("Ago",'giorni_mesi.php');
$n_mese[9] = mex("Set",'giorni_mesi.php');
$n_mese[10] = mex("Ott",'giorni_mesi.php');
$n_mese[11] = mex("Nov",'giorni_mesi.php');
$n_mese[12] = mex("Dic",'giorni_mesi.php');
for ($num1 = 2 ; $num1 <= 3 ; $num1++) {
for ($num2 = 1 ; $num2 <= 12 ; $num2++) {
echo "<option value=\"".(($num1*12) + $num2)."\">".$n_mese[$num2].". $num1 ".mex("anni successivi",$pag)."</option>";
} # fine for $num2
} # fine for $num1
echo "</select>.<br>";
$anno_prec = $anno -1;
if (@is_file(C_DATI_PATH."/selectperiodi$anno_prec.1.php")) {
echo "<label><input type=\"checkbox\" name=\"importa_anno_prec\" value=\"SI\" checked>
".mex("Importa dall'anno precedente prenotazioni, tariffe (compresi costi aggiuntivi), privilegi degli utenti e regole d'assegnazione.",$pag)."</label>";
} # fine if (@is_file(C_DATI_PATH."/selectperiodi$anno_prec.1.php"))
echo "</div></form>";
} # fine ((!$anno_attuale_esist and $anno == $anno_attuale) or $anno < $anno_attuale or...

} # fine else if (controlla_anno($anno) == "NO" or $id_utente != 1 or...
if (!$anno_attuale_esist) $anno_attuale = "";
echo "<br><hr style=\"width: 95%\"><br>
<form accept-charset=\"utf-8\" method=\"post\" action=\"inizio.php\"><div>
<input type=\"hidden\" name=\"id_sessione\" value=\"$id_sessione\">
<br> ".mex("richiedi l'anno",$pag)."
<input type=\"text\" name=\"anno\" size=\"4\" maxlength=\"4\" value=\"$anno_attuale\">
<input class=\"sbutton\" type=\"submit\" value=\"".mex("vai",$pag)."\"><br>
</div></form>";
if ($tema[$id_utente] != "base") include("./themes/".$tema[$id_utente]."/php/foot.php");
else include("./includes/foot.php");

} # fine else if ($anno_esistente == "SI")

} # fine else if (@is_file(C_DATI_PATH."/dati_connessione.php") != true)






} # fine if ($id_utente)

# If no user is logged in and no database connection file exists, show language selection for database creation
else if (@is_file(C_DATI_PATH."/dati_connessione.php") != true) {
$show_bar = "NO";
include("./includes/head.php");
if (@is_dir("./includes/lang/en")) $lingua_mex = "en";
else $lingua_mex = "ita";
if (@isset($_SERVER['HTTP_ACCEPT_LANGUAGE'])) {
$lingua_browser = explode(",",$_SERVER['HTTP_ACCEPT_LANGUAGE']);
foreach ($lingua_browser as $lang) {
if ($lang == "en") break;
if ($lang == "it") {
$lingua_mex = "ita";
break;
} # fine if ($lang == "it")
if (strlen($lang) == 2 and @is_dir("./includes/lang/$lang")) {
$lingua_mex = $lang;
break;
} # fine if (strlen($lang) == 2 and @is_dir("./includes/lang/$lang"))
} # fine foreach ($lingua_browser as $lang)
} # fine if (@isset($_SERVER['HTTP_ACCEPT_LANGUAGE']))
if (@is_file("./COPYING")) $file_copying = "file <a href=\"COPYING\">COPYING</a>";
else $file_copying = "<a href=\"http://www.gnu.org/licenses/agpl-3.0.html\">AGPLv3</a> License";
echo "<div style=\"text-align: center;\"><h3>".mex("Benvenuto a HOTELDRUID",$pag).".</h3><br><br>
HOTELDRUID version ".C_PHPR_VERSIONE_TXT.", Copyright (C) 2001-2024 Marco M. F. De Santis<br>
HotelDruid comes with ABSOLUTELY NO WARRANTY; <br>for details see the $file_copying.<br>
This is free software, and you are welcome to redistribute it<br>
 under certain conditions; see the $file_copying for details.<br>
</div><hr style=\"width: 95%\">
<form accept-charset=\"utf-8\" method=\"post\" action=\"creadb.php\"><div>
<br><br>
".mex("Scegli la lingua",$pag).": <select name=\"lingua\">";
if ($lingua_mex == "ita") $sel = " selected";
else $sel = "";
echo "<option value=\"ita\"$sel>italiano</option>";
$lang_dir = opendir("./includes/lang/");
while ($ini_lingua = readdir($lang_dir)) {
if ($ini_lingua != "." && $ini_lingua != "..") {
$nome_lingua = file("./includes/lang/$ini_lingua/l_n");
$nome_lingua = togli_acapo($nome_lingua[0]);
if ($ini_lingua == $lingua_mex) $sel = " selected";
else $sel = "";
echo "<option value=\"$ini_lingua\"$sel>$nome_lingua</option>";
} # fine if ($file != "." && $file != "..")
} # fine while ($file = readdir($lang_dig))
closedir($lang_dir);
echo "</select><br>
<input class=\"sbutton\" type=\"submit\" value=\"".mex("crea il database",$pag)."\"><br>
</div></form>";
include("./includes/foot.php");
} # fine else if (@is_file(C_DATI_PATH."/dati_connessione.php") != true)




?>


