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

$pag = "crearegole.php";
$titolo = "HotelDruid: Crea Regole";

$var_pag = array();
$var_pag[0] = 'tipotariffa_regola2';
$var_pag[1] = 'origine';
$var_pag[2] = 'inserisci';
$var_pag[3] = 'regola_2';
$var_pag[4] = 'tipotariffa';
$var_pag[5] = 'num_apti';
$var_pag[6] = 'v_apti';
$var_pag[7] = 'lista_app';
$var_pag[8] = 'chiudi_app';
$var_pag[9] = 'appartamento';
$var_pag[10] = 'inizioperiodo';
$var_pag[11] = 'fineperiodo';
$var_pag[12] = 'motivazione';
$var_pag[13] = 'regola_1';
$var_pag[14] = 'mod_idregola1';
$var_pag[15] = 'regola_1_tar';
$var_pag[16] = 'tipotariffa_regola3';
$var_pag[17] = 'num_persone';
$var_pag[18] = 'regola_3';
$var_pag[19] = 'regola_3m';
$var_pag[20] = 'nor3m';
$var_pag[21] = 'nor3';
$var_pag[22] = 'id_utente_inserimento';
$var_pag[23] = 'regola_4';
$var_pag[24] = 'cancella_vecchie_regole';
$var_pag[25] = 'num_creg_passa';
$var_pag[26] = 'canc_regola_1';
$var_pag[27] = 'gia_stato';
$var_pag[28] = 'regola_2b';
$var_pag[29] = 'num_giorni';
$var_pag[30] = 'ini_fine';
$n_var_pag = 31;
$num2 = 0;
if (isset($_POST['num_creg_passa'])) $num2 = (int) $_POST['num_creg_passa'];
elseif (isset($_GET['num_creg_passa'])) $num2 = (int) $_GET['num_creg_passa'];
for ($num1 = 0 ; $num1 < $num2 ; $num1++) {
$var_pag[$n_var_pag++] = "nome_creg_passa$num1";
$num3 = 0;
if (isset($_POST["nome_creg_passa$num1"])) $num3 = (int) $_POST["nome_creg_passa$num1"];
elseif (isset($_GET["nome_creg_passa$num1"])) $num3 = (int) $_GET["nome_creg_passa$num1"];
if ($num3) $var_pag[$n_var_pag++] = "creg$num3";
} # fine if for $num1

include("./costanti.php");
include(C_DATI_PATH."/dati_connessione.php");
include("./includes/funzioni_$PHPR_DB_TYPE.php");
$numconnessione = connetti_db($PHPR_DB_NAME,$PHPR_DB_HOST,$PHPR_DB_PORT,$PHPR_DB_USER,$PHPR_DB_PASS,$PHPR_LOAD_EXT);
include("./includes/funzioni.php");

// Initialize message arrays for success, error, and warning messages
$success_messages = array();
$error_messages = array();
$warning_messages = array();

// Panel-specific message arrays
$rule1_success_messages = array();
$rule1_error_messages = array();
$rule1_warning_messages = array();

$rule2_success_messages = array();
$rule2_error_messages = array();
$rule2_warning_messages = array();

$rule3_success_messages = array();
$rule3_error_messages = array();
$rule3_warning_messages = array();

$rule4_success_messages = array();
$rule4_error_messages = array();
$rule4_warning_messages = array();

$tableregole = $PHPR_TAB_PRE."regole".$anno;
$tableperiodi = $PHPR_TAB_PRE."periodi".$anno;
$tablenometariffe = $PHPR_TAB_PRE."ntariffe".$anno;
$tableappartamenti = $PHPR_TAB_PRE."appartamenti";
$tableutenti = $PHPR_TAB_PRE."utenti";
$tableprenota = $PHPR_TAB_PRE."prenota".$anno;
$tabletransazioni = $PHPR_TAB_PRE."transazioni";
$tableversioni = $PHPR_TAB_PRE."versioni";


$id_utente = controlla_login($numconnessione,$PHPR_TAB_PRE,$id_sessione,$nome_utente_phpr,$password_phpr,$anno);
if ($id_utente) {

if ($id_utente != 1) {
$tableprivilegi = $PHPR_TAB_PRE."privilegi";
$tablerelgruppi = $PHPR_TAB_PRE."relgruppi";
$privilegi_annuali_utente = esegui_query("select * from $tableprivilegi where idutente = '$id_utente' and anno = '$anno'");
if (numlin_query($privilegi_annuali_utente) == 0) $anno_utente_attivato = "NO";
else {
$anno_utente_attivato = "SI";
$privilegi_globali_utente = esegui_query("select * from $tableprivilegi where idutente = '$id_utente' and anno = '1'");
$priv_ins_clienti = risul_query($privilegi_globali_utente,0,'priv_ins_clienti');
$vedi_clienti = "NO";
if (substr($priv_ins_clienti,2,1) == "s") $vedi_clienti = "SI";
if (substr($priv_ins_clienti,2,1) == "p") $vedi_clienti = "PROPRI";
if (substr($priv_ins_clienti,2,1) == "g") { $vedi_clienti = "GRUPPI"; $prendi_gruppi = "SI"; }
$priv_ins_prenota = risul_query($privilegi_annuali_utente,0,'priv_ins_prenota');
$priv_ins_periodi_passati = substr($priv_ins_prenota,8,1);
$priv_mod_prenota = risul_query($privilegi_annuali_utente,0,'priv_mod_prenota');
$priv_mod_prenotazioni = substr($priv_mod_prenota,0,1);
if ($priv_mod_prenotazioni == "g") $prendi_gruppi = "SI";
$priv_mod_date = substr($priv_mod_prenota,1,1);
$priv_mod_assegnazione_app = substr($priv_mod_prenota,2,1);
$priv_mod_commento = substr($priv_mod_prenota,5,1);
$priv_mod_tariffa = substr($priv_mod_prenota,3,1);
$priv_mod_sconto = substr($priv_mod_prenota,6,1);
$priv_mod_caparra = substr($priv_mod_prenota,7,1);
$priv_mod_pagato = substr($priv_mod_prenota,10,1);
$priv_mod_prenota_iniziate = substr($priv_mod_prenota,11,1);
$priv_mod_prenota_ore = substr($priv_mod_prenota,12,3);
$priv_mod_checkin = substr($priv_mod_prenota,20,1);
$priv_mod_prenota_comp = substr($priv_mod_prenota,23,1);
$priv_mod_orig_prenota = substr($priv_mod_prenota,24,1);
$priv_vedi_commento = substr($priv_mod_prenota,25,1);
$priv_vedi_commenti_pers = substr($priv_mod_prenota,26,1);
$priv_ins_tariffe = risul_query($privilegi_annuali_utente,0,'priv_ins_tariffe');
$priv_ins_costi_agg = substr($priv_ins_tariffe,1,1);
$priv_mod_reg1 = substr($priv_ins_tariffe,4,1);
$priv_mod_reg2 = substr($priv_ins_tariffe,5,1);
$priv_prenota_gruppi = "NO";
$priv_app_gruppi = "NO";
$regole1_consentite = risul_query($privilegi_annuali_utente,0,'regole1_consentite');
$attiva_regole1_consentite = substr($regole1_consentite,0,1);
if ($attiva_regole1_consentite != "n") $regole1_consentite = explode("#@^",substr($regole1_consentite,3));
$tariffe_consentite = risul_query($privilegi_annuali_utente,0,'tariffe_consentite');
$attiva_tariffe_consentite = substr($tariffe_consentite,0,1);
if ($attiva_tariffe_consentite == "s") {
$tariffe_consentite = explode(",",substr($tariffe_consentite,2));
$tariffe_consentite_vett = array();
for ($num1 = 0 ; $num1 < count($tariffe_consentite) ; $num1++) if ($tariffe_consentite[$num1]) $tariffe_consentite_vett[$tariffe_consentite[$num1]] = "SI";
} # fine if ($attiva_tariffe_consentite == "s")
$priv_ins_nuove_prenota = substr($priv_ins_prenota,0,1);
$priv_ins_assegnazione_app = substr($priv_ins_prenota,1,1);
$priv_mod_prenotazioni_v = $priv_mod_prenotazioni;
} # fine else if (numlin_query($privilegi_annuali_utente) == 0)

/*if ($priv_app_gruppi == "SI") {
$attiva_regole1_consentite_gr[$id_utente] = $attiva_regole1_consentite;
$regole1_consentite_gr[$id_utente] = $regole1_consentite;
$attiva_tariffe_consentite_gr[$id_utente] = $attiva_tariffe_consentite;
$tariffe_consentite_vett_gr[$id_utente] = $tariffe_consentite_vett;
$priv_ins_nuove_prenota_gr[$id_utente] = $priv_ins_nuove_prenota;
$priv_ins_assegnazione_app_gr[$id_utente] = $priv_ins_assegnazione_app;
$priv_mod_prenotazioni_gr[$id_utente] = $priv_mod_prenotazioni;
$priv_mod_assegnazione_app_gr[$id_utente] = $priv_mod_assegnazione_app;
} # fine if ($priv_app_gruppi == "SI")
unset($utenti_gruppi);
$utenti_gruppi[$id_utente] = 1;
if ($prendi_gruppi == "SI") {
$gruppi_utente = esegui_query("select idgruppo from $tablerelgruppi where idutente = '$id_utente' and idgruppo is not NULL ");
$num_gruppi_utente = numlin_query($gruppi_utente);
for ($num1 = 0 ; $num1 < $num_gruppi_utente ; $num1++) {
$idgruppo = risul_query($gruppi_utente,$num1,'idgruppo');
$utenti_gruppo = esegui_query("select idutente from $tablerelgruppi where idgruppo = '$idgruppo' ");
$num_utenti_gruppo = numlin_query($utenti_gruppo);
for ($num2 = 0 ; $num2 < $num_utenti_gruppo ; $num2++) {
$idutente_gruppo = risul_query($utenti_gruppo,$num2,'idutente');
if ($idutente_gruppo != $id_utente and !$utenti_gruppi[$idutente_gruppo]) {
$utenti_gruppi[$idutente_gruppo] = 1;

if ($priv_app_gruppi == "SI") {
$priv_anno_ut_gr = esegui_query("select * from $tableprivilegi where idutente = '$idutente_gruppo' and anno = '$anno'");
if (numlin_query($priv_anno_ut_gr) == 1) {
$regole1_consentite_gr[$idutente_gruppo] = risul_query($priv_anno_ut_gr,0,'regole1_consentite');
$attiva_regole1_consentite_gr[$idutente_gruppo] = substr($regole1_consentite_gr[$idutente_gruppo],0,1);
if ($attiva_regole1_consentite_gr[$idutente_gruppo] != "n") $regole1_consentite_gr[$idutente_gruppo] = explode("#@^",substr($regole1_consentite_gr[$idutente_gruppo],3));
$tariffe_consentite_tmp = risul_query($priv_anno_ut_gr,0,'tariffe_consentite');
$attiva_tariffe_consentite_gr[$idutente_gruppo] = substr($tariffe_consentite_tmp,0,1);
if ($attiva_tariffe_consentite_gr[$idutente_gruppo] == "s") {
$tariffe_consentite_tmp = explode(",",substr($tariffe_consentite_tmp,2));
$tariffe_consentite_vett_gr[$idutente_gruppo] = "";
for ($num3 = 0 ; $num3 < count($tariffe_consentite_tmp) ; $num3++) if ($tariffe_consentite_tmp[$num3]) $tariffe_consentite_vett_gr[$idutente_gruppo][$tariffe_consentite_tmp[$num3]] = "SI";
} # fine if ($attiva_tariffe_consentite_gr[$idutente_gruppo] == "s")
$costi_agg_consentiti_tmp = risul_query($priv_anno_ut_gr,0,"costi_agg_consentiti");
$attiva_costi_agg_consentiti_tmp = substr($costi_agg_consentiti_tmp,0,1);
if ($attiva_costi_agg_consentiti_tmp == "n") $attiva_costi_agg_consentiti_gr = "n";
if ($attiva_costi_agg_consentiti_gr == "s") {
$costi_agg_consentiti_tmp = explode(",",substr($costi_agg_consentiti_tmp,2));
for ($num3 = 0 ; $num3 < count($costi_agg_consentiti_tmp) ; $num3++) if ($costi_agg_consentiti_tmp[$num3]) $costi_agg_consentiti_vett_gr[$costi_agg_consentiti_tmp[$num3]] = "SI";
} # fine if ($attiva_costi_agg_consentiti_gr == "s")
$priv_ins_prenota_tmp = risul_query($priv_anno_ut_gr,0,'priv_ins_prenota');
$priv_ins_nuove_prenota_gr[$idutente_gruppo] = substr($priv_ins_prenota_tmp,0,1);
$priv_ins_assegnazione_app_gr[$idutente_gruppo] = substr($priv_ins_prenota_tmp,1,1);
$priv_mod_prenota_tmp = risul_query($priv_anno_ut_gr,0,'priv_mod_prenota');
$priv_mod_prenotazioni_gr[$idutente_gruppo] = substr($priv_mod_prenota_tmp,0,1);
$priv_mod_assegnazione_app_gr[$idutente_gruppo] = substr($priv_mod_prenota_tmp,2,1);
} # fine if (numlin_query($priv_anno_ut_gr) == 1)
else {
$priv_ins_nuove_prenota_gr[$idutente_gruppo] = "n";
$priv_mod_prenotazioni_gr[$idutente_gruppo] = "n";
} # fine else if (numlin_query($priv_anno_ut_gr) == 1)
} # fine if ($priv_app_gruppi == "SI")

} # fine if ($idutente_gruppo != $id_utente)
} # fine for $num2
} # fine for $num1
} # fine if ($prendi_gruppi == "SI")*/

} # fine if ($id_utente != 1)
else {
$anno_utente_attivato = "SI";
$vedi_clienti = "SI";
$priv_ins_periodi_passati = "s";
$priv_ins_assegnazione_app = "s";
$priv_mod_prenotazioni = "s";
$priv_mod_date = "s";
$priv_mod_assegnazione_app = "s";
$priv_mod_commento = "s";
$priv_mod_sconto = "s";
$priv_mod_caparra = "s";
$priv_mod_pagato = "s";
$priv_mod_prenota_iniziate = "s";
$priv_mod_prenota_ore = "000";
$priv_mod_checkin = "s";
$priv_mod_prenota_comp = "s";
$priv_mod_orig_prenota = "s";
$priv_vedi_commento = "s";
$priv_vedi_commenti_pers = "s";
$attiva_tariffe_consentite = "n";
$attiva_regole1_consentite = "n";
$priv_ins_costi_agg = "s";
$priv_mod_reg1 = "s";
$priv_mod_reg2 = "s";
} # fine else if ($id_utente != 1)
if ($anno_utente_attivato == "SI" and ($priv_mod_reg1 != "n" or $priv_mod_reg2 != "n")) {

if ($priv_vedi_commenti_pers = "s") $priv_mod_commenti_pers = "s";
else $priv_mod_commenti_pers = "n";

if (@is_file(C_DATI_PATH."/dati_subordinazione.php")) {
$installazione_subordinata = "SI";
$inserimento_nuovi_clienti = "NO";
$modifica_clienti = "NO";
$priv_ins_nuove_prenota = "n";
$priv_mod_date = "n";
$priv_mod_assegnazione_app = "n";
$priv_mod_commento = "n";
$priv_mod_commenti_pers = "n";
$priv_mod_sconto = "n";
$priv_mod_caparra = "n";
$priv_mod_pagato = "n";
$priv_mod_checkin = "n";
$priv_mod_prenota_comp = "n";
$priv_mod_orig_prenota = "n";
$priv_ins_spese = "n";
$priv_ins_entrate = "n";
$priv_ins_costi_agg = "n";
$priv_mod_reg1 = "n";
$priv_mod_reg2 = "n";
} # fine if (@is_file(C_DATI_PATH."/dati_subordinazione.php"))



$titolo = "HotelDruid: ".mex("Crea Regole",$pag);
if ($tema[$id_utente] and $tema[$id_utente] != "base" and @is_dir("./themes/".$tema[$id_utente]."/php")) include("./themes/".$tema[$id_utente]."/php/head.php");
else include("./includes/head.php");

// Include template system
require_once("./includes/template.php");

// Messages are now displayed at the top of each panel instead of globally

$stile_data = stile_data();

$appartamenti = esegui_query("select * from $tableappartamenti order by idappartamenti");
$num_appartamenti = numlin_query($appartamenti);
include("./includes/funzioni_appartamenti.php");
#if ($priv_app_gruppi != "SI") $appartamenti_consentiti = trova_app_consentiti($appartamenti,$num_appartamenti,$attiva_regole1_consentite,$regole1_consentite,$priv_mod_assegnazione_app,$priv_mod_prenotazioni,$priv_ins_assegnazione_app,$priv_ins_nuove_prenota,$attiva_tariffe_consentite,$tariffe_consentite_vett,$id_utente,$tableregole,$tablenometariffe);
#else $appartamenti_consentiti = trova_app_consentiti($appartamenti,$num_appartamenti,$attiva_regole1_consentite_gr,$regole1_consentite_gr,$priv_mod_assegnazione_app_gr,$priv_mod_prenotazioni_gr,$priv_ins_assegnazione_app_gr,$priv_ins_nuove_prenota_gr,$attiva_tariffe_consentite_gr,$tariffe_consentite_vett_gr,$id_utente,$tableregole,$tablenometariffe);
$appartamenti_consentiti = trova_app_consentiti($appartamenti,$num_appartamenti,$attiva_regole1_consentite,fixset($regole1_consentite),$priv_mod_assegnazione_app,$priv_mod_prenotazioni,$priv_ins_assegnazione_app,$priv_ins_nuove_prenota,$attiva_tariffe_consentite,fixset($tariffe_consentite_vett),$id_utente,$tableregole,$tablenometariffe);
$tutti_app_consentiti = 1;
if ($id_utente != 1) {
for ($num1 = 0 ; $num1 < $num_appartamenti ; $num1++) {
if ($appartamenti_consentiti[risul_query($appartamenti,$num1,'idappartamenti')] != "SI") {
$tutti_app_consentiti = 0;
break;
} # fine if ($appartamenti_consentiti[risul_query($appartamenti,$num1,'idappartamenti')] != "SI") 
} # fine for $num1
} # fine if ($id_utente != 1)



if (!empty($inserisci)) {
$aggiorna_ic_disp = 0;
$aggiorna_ic_tar = 0;
$tabelle_lock = array($tableregole);
$altre_tab_lock = array($tablenometariffe,$tableperiodi,$tableappartamenti,$tableutenti);
$tabelle_lock = lock_tabelle($tabelle_lock,$altre_tab_lock);


if (!empty($regola_1) and ($priv_mod_reg1 == "s" or $priv_mod_reg1 == "a")) {
$active_panel = 'rule1';
$regola_1_tar = "";
if (empty($inizioperiodo) or empty($fineperiodo) or empty($appartamento)) {
$error_messages[] = mex("Non sono stati inseriti tutti i dati necessari",$pag)."!";
} # fine if (empty($inizioperiodo) or empty($fineperiodo) or empty($appartamento))
else {
$idinizioperiodo = esegui_query("select idperiodi from $tableperiodi where datainizio = '".aggslashdb($inizioperiodo)."' ");
if (numlin_query($idinizioperiodo) == 0) $idinizioperiodo = 9999999;
else $idinizioperiodo = risul_query($idinizioperiodo,0,'idperiodi');
$idfineperiodo = esegui_query("select idperiodi from $tableperiodi where datafine = '".aggslashdb($fineperiodo)."' ");
if (numlin_query($idfineperiodo) == 0) $idfineperiodo = -1;
else $idfineperiodo = risul_query($idfineperiodo,0,'idperiodi');
if ($idfineperiodo < $idinizioperiodo) {
$error_messages[] = mex("Le date sono sbagliate",$pag)."";
} # fine if ($idfineperiodo < $idinizioperiodo)
else {
if ($motivazione == " ") $error_messages[] = mex("Motivazione non valida",$pag)."";
else {
$appartamento = htmlspecialchars($appartamento,ENT_COMPAT);
$app_esistente = esegui_query("select idappartamenti from $tableappartamenti where idappartamenti = '".aggslashdb($appartamento)."' ");
if (!numlin_query($app_esistente) or $appartamenti_consentiti[$appartamento] != "SI") $error_messages[] = mex("Non sono stati inseriti tutti i dati necessari",$pag).".";  
else {
if (!empty($mod_idregola1) and controlla_num_pos($mod_idregola1) == "SI") {
$reg_esist = esegui_query("select * from $tableregole where idregole = '$mod_idregola1' ");
if (numlin_query($reg_esist)) {
$app_agenzia = risul_query($reg_esist,0,'app_agenzia');
if (!$app_agenzia) $mod_idregola1 = "";
} # fine if (numlin_query($reg_esist))
else $mod_idregola1 = "";
} # fine if (!empty($mod_idregola1) and controlla_num_pos($mod_idregola1) == "SI")
else $mod_idregola1 = "";
if ($mod_idregola1) $cond_reg_mod = " and idregole != '$mod_idregola1'";
else $cond_reg_mod = "";
$vecchia_regola = esegui_query("select * from $tableregole where app_agenzia = '".aggslashdb($appartamento)."' and iddatainizio <= '$idfineperiodo' and iddatafine >= '$idinizioperiodo'$cond_reg_mod order by iddatainizio");
$num_vecchia_regola = numlin_query($vecchia_regola);
if ($attiva_regole1_consentite != "n") $cancella_vecchie_regole = "";
if ($num_vecchia_regola != 0 and empty($cancella_vecchie_regole)) {
// Store confirmation data to display later in panel
$show_rule1_confirmation = true;
$rule1_confirmation_data = array(
'vecchia_regola' => $vecchia_regola,
'num_vecchia_regola' => $num_vecchia_regola,
'appartamento' => $appartamento,
'inizioperiodo' => $inizioperiodo,
'fineperiodo' => $fineperiodo,
'motivazione' => $motivazione,
'mod_idregola1' => $mod_idregola1,
'chiudi_app' => $chiudi_app,
'idinizioperiodo' => $idinizioperiodo,
'idfineperiodo' => $idfineperiodo
);
$warning_messages[] = mex("Esiste già una regola di questo tipo nell'appartamento e nel periodo selezionato",'unit.php')." (".formatta_data($inizioperiodo,$stile_data)." ".mex("al",$pag)." ".formatta_data($fineperiodo,$stile_data).")";
} # fine if ($num_vecchia_regola != 0 and empty($cancella_vecchie_regole))
else {
$idregole = esegui_query("select max(idregole) from $tableregole");
$idregole = risul_query($idregole,0,0);
$idregole = $idregole + 1;
for ($num1 = 0 ; $num1 < $num_vecchia_regola ; $num1++) {
$idregola_v = risul_query($vecchia_regola,$num1,'idregole');
$iddatainizio_v = risul_query($vecchia_regola,$num1,'iddatainizio');
$iddatafine_v = risul_query($vecchia_regola,$num1,'iddatafine');
if (($iddatainizio_v >= $idinizioperiodo and $iddatafine_v <= $idfineperiodo) or ${"creg$idregola_v"}) esegui_query("delete from $tableregole where idregole = '$idregola_v' ");
else {
if ($iddatainizio_v >= $idinizioperiodo) esegui_query("update $tableregole set iddatainizio = '".($idfineperiodo + 1)."' where idregole = '$idregola_v' ");
else {
esegui_query("update $tableregole set iddatafine = '".($idinizioperiodo - 1)."' where idregole = '$idregola_v' ");
if ($iddatafine_v > $idfineperiodo) {
$motivazione_v = risul_query($vecchia_regola,$num1,'motivazione');
$motivazione2_v = risul_query($vecchia_regola,$num1,'motivazione2');
esegui_query("insert into $tableregole (idregole,app_agenzia,iddatainizio,iddatafine) values ($idregole,'".aggslashdb($appartamento)."','".($idfineperiodo + 1)."','$iddatafine_v')");
if ($motivazione2_v == "x") esegui_query("update $tableregole set motivazione2 = 'x' where idregole = '$idregole' ");
if ($motivazione_v) esegui_query("update $tableregole set motivazione = '".aggslashdb($motivazione_v)."' where idregole = '$idregole' ");
$idregole++;
} # fine if ($iddatafine_v > $idfineperiodo)
} # fine else if ($iddatainizio_v >= $idinizioperiodo)
} # fine else if (($iddatainizio_v >= $idinizioperiodo and $iddatafine_v <= $idfineperiodo) or ${"creg$idregola_v"})
} # fine for $num1
if ($mod_idregola1) {
esegui_query("delete from $tableregole where idregole = '$mod_idregola1' ");
$idregole = $mod_idregola1;
} # fine if ($mod_idregola1)
esegui_query("insert into $tableregole (idregole,app_agenzia,iddatainizio,iddatafine) values ($idregole,'".aggslashdb($appartamento)."','$idinizioperiodo','$idfineperiodo')");
if (@get_magic_quotes_gpc()) $motivazione = stripslashes($motivazione);
$motivazione = htmlspecialchars($motivazione,ENT_COMPAT);
if ($motivazione) esegui_query("update $tableregole set motivazione = '".aggslashdb($motivazione)."' where idregole = '$idregole' ");
if ($attiva_regole1_consentite != "n") $chiudi_app = 1;
if ($chiudi_app) {
esegui_query("update $tableregole set motivazione2 = 'x' where idregole = '$idregole' ");
unlock_tabelle($tabelle_lock);
include("./includes/liberasettimane.php");
$tabelle_lock = array($tableprenota);
$altre_tab_lock = array($tableperiodi,$tableappartamenti,$tableregole,$tablepersonalizza);
$tabelle_lock = lock_tabelle($tabelle_lock,$altre_tab_lock);

unset($limiti_var);
unset($profondita);
unset($dati_app);
$limiti_var['idperiodocorrente'] = calcola_id_periodo_corrente($anno);
if ($idinizioperiodo > $limiti_var['idperiodocorrente']) $n_ini = $idinizioperiodo;
else {
if (($limiti_var['idperiodocorrente'] + 1) <= $idfineperiodo) $n_ini = ($limiti_var['idperiodocorrente'] + 1);
else $n_ini = $limiti_var['idperiodocorrente'];
} # fine else if ($idinizioperiodo > $limiti_var['idperiodocorrente'])
$limiti_var['n_ini'] = $n_ini;
$limiti_var['n_fine'] = $idfineperiodo;
$profondita['iniziale'] = "";
$profondita['attuale'] = 1;
$max_prenota = esegui_query("select max(idprenota) from $tableprenota");
$tot_prenota = risul_query($max_prenota,0,0);
$profondita['tot_prenota_ini'] = $tot_prenota;
$profondita['tot_prenota_attuale'] = $tot_prenota;
tab_a_var($limiti_var,$app_prenota_id,$app_orig_prenota_id,$inizio_prenota_id,$fine_prenota_id,$app_assegnabili_id,$prenota_in_app_sett,$anno,$dati_app,$profondita,$PHPR_TAB_PRE."prenota");
$fatto_libera = "";
$app_agenzia = esegui_query("select * from $tableregole where app_agenzia != '' and idregole != '$idregole' ");
$num_app_agenzia = numlin_query($app_agenzia);
if ($num_app_agenzia != 0) {
$info_periodi_ag['numero'] = $num_app_agenzia;
for ($num1 = 0 ; $num1 < $num_app_agenzia ; $num1++) {
$info_periodi_ag['app'][$num1] = risul_query($app_agenzia,$num1,'app_agenzia');
$info_periodi_ag['ini'][$num1] = risul_query($app_agenzia,$num1,'iddatainizio');
$info_periodi_ag['fine'][$num1] = risul_query($app_agenzia,$num1,'iddatafine');
} # fine for $num1
inserisci_prenota_fittizie($info_periodi_ag,$profondita,$app_prenota_id,$inizio_prenota_id,$fine_prenota_id,$prenota_in_app_sett,$app_assegnabili_id);
} # fine if ($num_app_agenzia != 0)
unset($app_richiesti);
$app_richiesti[',numero,'] = 1;
$app_richiesti[1] = $appartamento;
$idinizioperiodo_vett[1] = $n_ini;
$idfineperiodo_vett[1] = $idfineperiodo;
liberasettimane($idinizioperiodo_vett,$idfineperiodo_vett,$limiti_var,$anno,$fatto_libera,$app_liberato_vett,$profondita,$app_richiesti,$app_prenota_id,$app_orig_prenota_id,$inizio_prenota_id,$fine_prenota_id,$app_assegnabili_id,$prenota_in_app_sett,$dati_app,$PHPR_TAB_PRE."prenota");
if ($fatto_libera != "SI") $risul_agg = 0;
else $risul_agg = aggiorna_tableprenota($app_prenota_id,$app_orig_prenota_id,$tableprenota);
if ($risul_agg) {
$success_messages[] = mex("Il periodo chiuso è stato liberato dalle prenotazioni",$pag)."";
} # fine if ($risul_agg)
else $error_messages[] = mex("Non è stato possibile liberare dalle prenotazioni il periodo chiuso",$pag)."";
$aggiorna_ic_disp = 1;
} # fine if ($chiudi_app)
unlock_tabelle($tabelle_lock);
#unset($tabelle_lock);
$success_messages[] = mex("La regola è stata inserita",$pag)."";
$tabelle_lock = array($tableversioni,$tabletransazioni);
$tabelle_lock = lock_tabelle($tabelle_lock);
$ultimo_accesso = date("Y-m-d H:i:s",(time() + (C_DIFF_ORE * 3600)));
$transaz_esistente = esegui_query("select idtransazioni from $tabletransazioni where idsessione = '$id_sessione' and tipo_transazione = 'cr_1a' ");
if (numlin_query($transaz_esistente)) {
$transaz_esistente = risul_query($transaz_esistente,0,'idtransazioni');
esegui_query("update $tabletransazioni set dati_transazione1 = '".aggslashdb($inizioperiodo)."', dati_transazione2 = '".aggslashdb($fineperiodo)."', dati_transazione3 = '".aggslashdb($motivazione)."', dati_transazione4 = '".aggslashdb($chiudi_app)."', ultimo_accesso = '$ultimo_accesso' where idtransazioni = '$transaz_esistente' ");
} # fine if (numlin_query($transaz_esistente))
else {
$id_transazione = crea_id_sessione("",$tableversioni,8);
esegui_query("insert into $tabletransazioni (idtransazioni,idsessione,tipo_transazione,anno,dati_transazione1,dati_transazione2,dati_transazione3,dati_transazione4,ultimo_accesso) 
values ('$id_transazione','$id_sessione','cr_1a','$anno','".aggslashdb($inizioperiodo)."','".aggslashdb($fineperiodo)."','".aggslashdb($motivazione)."','".aggslashdb($chiudi_app)."','$ultimo_accesso')");
} # fine else if (numlin_query($transaz_esistente))
unlock_tabelle($tabelle_lock);
} # fine else if ($num_vecchia_regola != 0 and !$cancella_vecchie_regole)
} # fine else if (!numlin_query($app_esistente))
} # fine else if ($motivazione == " ")
} # fine else if ($idfineperiodo < $idinizioperiodo)
} # fine else if (empty($inizioperiodo) or empty($fineperiodo) or empty($appartamento))
} # fine if (!empty($regola_1) and ($priv_mod_reg1 == "s" or $priv_mod_reg1 == "a"))


if (!empty($regola_1_tar) and ($priv_mod_reg1 == "s" or $priv_mod_reg1 == "t")) {
$active_panel = 'rule1';
if (empty($inizioperiodo) or empty($fineperiodo) or empty($tipotariffa) or substr($tipotariffa,0,7) != "tariffa" or controlla_num_pos(substr($tipotariffa,7)) == "NO" or ($attiva_tariffe_consentite != "n" and !isset($tariffe_consentite_vett[substr($tipotariffa,7)]))) {
$error_messages[] = mex("Non sono stati inseriti tutti i dati necessari",$pag)."!";
} # fine if (empty($inizioperiodo) or empty($fineperiodo) or empty($tipotariffa) or...
else {
$idinizioperiodo = esegui_query("select idperiodi from $tableperiodi where datainizio = '".aggslashdb($inizioperiodo)."' ");
if (numlin_query($idinizioperiodo) == 0) $idinizioperiodo = 9999999;
else $idinizioperiodo = risul_query($idinizioperiodo,0,'idperiodi');
$idfineperiodo = esegui_query("select idperiodi from $tableperiodi where datafine = '".aggslashdb($fineperiodo)."' ");
if (numlin_query($idfineperiodo) == 0) $idfineperiodo = -1;
else $idfineperiodo = risul_query($idfineperiodo,0,'idperiodi');
if ($idfineperiodo < $idinizioperiodo) {
$error_messages[] = mex("Le date sono sbagliate",$pag)."";
} # fine if ($idfineperiodo < $idinizioperiodo)
else {
if ($motivazione == " ") $error_messages[] = mex("Motivazione non valida",$pag)."";
else {
if (!empty($mod_idregola1) and controlla_num_pos($mod_idregola1) == "SI") {
$reg_esist = esegui_query("select * from $tableregole where idregole = '$mod_idregola1' ");
if (numlin_query($reg_esist)) {
$tariffa_chiusa = risul_query($reg_esist,0,'tariffa_chiusa');
if (!$tariffa_chiusa) $mod_idregola1 = "";
} # fine if (numlin_query($reg_esist))
else $mod_idregola1 = "";
} # fine if (!empty($mod_idregola1) and controlla_num_pos($mod_idregola1) == "SI")
else $mod_idregola1 = "";
if ($mod_idregola1) $cond_reg_mod = " and idregole != '$mod_idregola1'";
else $cond_reg_mod = "";
$vecchia_regola = esegui_query("select * from $tableregole where tariffa_chiusa = '$tipotariffa' and iddatainizio <= '$idfineperiodo' and iddatafine >= '$idinizioperiodo'$cond_reg_mod order by iddatainizio ");
$num_vecchia_regola = numlin_query($vecchia_regola);
if ($num_vecchia_regola != 0 and empty($cancella_vecchie_regole)) {
// Store confirmation data to display later in panel
$show_rule1b_confirmation = true;
$tariffa_vedi = mex("tariffa","prenota.php").substr($tipotariffa,7);
$rule1b_confirmation_data = array(
'vecchia_regola' => $vecchia_regola,
'num_vecchia_regola' => $num_vecchia_regola,
'tipotariffa' => $tipotariffa,
'tariffa_vedi' => $tariffa_vedi,
'inizioperiodo' => $inizioperiodo,
'fineperiodo' => $fineperiodo,
'motivazione' => $motivazione,
'mod_idregola1' => $mod_idregola1,
'idinizioperiodo' => $idinizioperiodo,
'idfineperiodo' => $idfineperiodo
);
$warning_messages[] = mex("Esiste già una regola di questo tipo nel periodo selezionato",$pag)." (".formatta_data($inizioperiodo,$stile_data)." ".mex("al",$pag)." ".formatta_data($fineperiodo,$stile_data).")";
} # fine if ($num_vecchia_regola != 0 and empty($cancella_vecchie_regole))
else {
$idregole = esegui_query("select max(idregole) from $tableregole");
$idregole = risul_query($idregole,0,0);
$idregole = $idregole + 1;
for ($num1 = 0 ; $num1 < $num_vecchia_regola ; $num1++) {
$idregola_v = risul_query($vecchia_regola,$num1,'idregole');
$iddatainizio_v = risul_query($vecchia_regola,$num1,'iddatainizio');
$iddatafine_v = risul_query($vecchia_regola,$num1,'iddatafine');
if (($iddatainizio_v >= $idinizioperiodo and $iddatafine_v <= $idfineperiodo) or ${"creg$idregola_v"}) esegui_query("delete from $tableregole where idregole = '$idregola_v' ");
else {
if ($iddatainizio_v >= $idinizioperiodo) esegui_query("update $tableregole set iddatainizio = '".($idfineperiodo + 1)."' where idregole = '$idregola_v' ");
else {
esegui_query("update $tableregole set iddatafine = '".($idinizioperiodo - 1)."' where idregole = '$idregola_v' ");
if ($iddatafine_v > $idfineperiodo) {
$motivazione_v = risul_query($vecchia_regola,$num1,'motivazione');
esegui_query("insert into $tableregole (idregole,tariffa_chiusa,iddatainizio,iddatafine) values ($idregole,'$tipotariffa','".($idfineperiodo + 1)."','$iddatafine_v')");
if ($motivazione_v) esegui_query("update $tableregole set motivazione = '".aggslashdb($motivazione_v)."' where idregole = '$idregole' ");
$idregole++;
} # fine if ($iddatafine_v > $idfineperiodo)
} # fine else if ($iddatainizio_v >= $idinizioperiodo)
} # fine else if (($iddatainizio_v >= $idinizioperiodo and $iddatafine_v <= $idfineperiodo) or ${"creg$idregola_v"})
} # fine for $num1
if ($mod_idregola1) {
esegui_query("delete from $tableregole where idregole = '$mod_idregola1' ");
$idregole = $mod_idregola1;
} # fine if ($mod_idregola1)
esegui_query("insert into $tableregole (idregole,tariffa_chiusa,iddatainizio,iddatafine) values ('$idregole','$tipotariffa','$idinizioperiodo','$idfineperiodo')");
if (@get_magic_quotes_gpc()) $motivazione = stripslashes($motivazione);
$motivazione = htmlspecialchars($motivazione,ENT_COMPAT);
if ($motivazione) esegui_query("update $tableregole set motivazione = '".aggslashdb($motivazione)."' where idregole = '$idregole' ");
$success_messages[] = mex("La regola è stata inserita",$pag)."";
$aggiorna_ic_tar = 1;
unlock_tabelle($tabelle_lock);
$tabelle_lock = array($tableversioni,$tabletransazioni);
$tabelle_lock = lock_tabelle($tabelle_lock);
$ultimo_accesso = date("Y-m-d H:i:s",(time() + (C_DIFF_ORE * 3600)));
$transaz_esistente = esegui_query("select idtransazioni from $tabletransazioni where idsessione = '$id_sessione' and tipo_transazione = 'cr_1t' ");
if (numlin_query($transaz_esistente)) {
$transaz_esistente = risul_query($transaz_esistente,0,'idtransazioni');
esegui_query("update $tabletransazioni set dati_transazione1 = '".aggslashdb($inizioperiodo)."', dati_transazione2 = '".aggslashdb($fineperiodo)."', dati_transazione3 = '".aggslashdb($motivazione)."', dati_transazione4 = '".aggslashdb($tipotariffa)."', ultimo_accesso = '$ultimo_accesso' where idtransazioni = '$transaz_esistente' ");
} # fine if (numlin_query($transaz_esistente))
else {
$id_transazione = crea_id_sessione("",$tableversioni,8);
esegui_query("insert into $tabletransazioni (idtransazioni,idsessione,tipo_transazione,anno,dati_transazione1,dati_transazione2,dati_transazione3,dati_transazione4,ultimo_accesso) 
values ('$id_transazione','$id_sessione','cr_1t','$anno','".aggslashdb($inizioperiodo)."','".aggslashdb($fineperiodo)."','".aggslashdb($motivazione)."','".aggslashdb($tipotariffa)."','$ultimo_accesso')");
} # fine else if (numlin_query($transaz_esistente))
unlock_tabelle($tabelle_lock);
} # fine else if ($num_vecchia_regola != 0 and empty($cancella_vecchie_regole))
} # fine else if ($motivazione == " ")
} # fine else if ($idfineperiodo < $idinizioperiodo)
} # fine else if (empty($inizioperiodo) or empty($fineperiodo) or empty($tipotariffa) or...
} # fine if (!empty($regola_1_tar) and ($priv_mod_reg1 == "s" or $priv_mod_reg1 == "t"))


if (!empty($canc_regola_1) and $priv_mod_reg1 == "s" and $tutti_app_consentiti and $attiva_regole1_consentite == "n") {
$active_panel = 'rule1';
$tutte_tar_consentite = 1;
if ($attiva_tariffe_consentite != "n") {
$rigatariffe = esegui_query("select * from $tablenometariffe where idntariffe = 1 ");
$numero_tariffe = risul_query($rigatariffe,0,'nomecostoagg');
for ($numtariffa = 1 ; $numtariffa <= $numero_tariffe ; $numtariffa++) {
if (!isset($tariffe_consentite_vett[$numtariffa])) {
$tutte_tar_consentite = 0;
break;
} # fine if (!isset($tariffe_consentite_vett[$numtariffa]))
} # fine for $numtariffa
} # fine if ($attiva_tariffe_consentite != "n")
if ($tutte_tar_consentite) {
if (!empty($gia_stato)) {
esegui_query("delete from $tableregole where (app_agenzia != '' and app_agenzia is not NULL) or (tariffa_chiusa != '' and tariffa_chiusa is not NULL) ");
$success_messages[] = mex("Le regole sono state cancellate",$pag)."";
$aggiorna_ic_disp = 1;
$aggiorna_ic_tar = 1;
} # fine if (!empty($gia_stato))
else {
// Store delete confirmation to display later in panel
$show_delete_rule1_confirmation = true;
$warning_messages[] = mex("Sei sicuro di voler cancellare tutte le regole del tipo 1",$pag)."?";
} # fine else if (!empty($gia_stato))
} # fine if ($tutte_tar_consentite)
} # fine if (!empty($canc_regola_1) and $priv_mod_reg1 == "s" and $tutti_app_consentiti and $attiva_regole1_consentite == "n")


if ((!empty($regola_2) or !empty($regola_2b)) and $priv_mod_reg2 == "s") {
$active_panel = 'rule2';
$inserire = "";
if (substr($tipotariffa,0,7) != "tariffa" or controlla_num_pos(substr($tipotariffa,7)) == "NO" or ($attiva_tariffe_consentite != "n" and !isset($tariffe_consentite_vett[substr($tipotariffa,7)]))) {
$inserire = "NO";
$error_messages[] = mex("Si deve scegliere la tariffa",$pag)."";
$num_regole_esistente = 0;
} # fine if (substr($tipotariffa,0,7) != "tariffa" or...
else {
$regola_esistente = esegui_query("select * from $tableregole where tariffa_per_app = '".aggslashdb($tipotariffa)."'");
$num_regole_esistente = numlin_query($regola_esistente);
/*
if ($num_regole_esistente != 0) {
$inserire = "NO";
$error_messages[] = mex("La tariffa scelta ha già degli appartamenti associati, cancella la regola prima di inserirne una nuova",'unit.php')."";
} # fine if ($num_regole_esistente != 0)
*/
} # fine else if (substr($tipotariffa,0,7) != "tariffa" or...
$num_app_reali = 0;
if (empty($lista_app)) {
$inserire = "NO";
$error_messages[] = mex("Si deve inserire almeno un appartamento da associare",'unit.php')."";
} # fine if (empty($lista_app))
else {
if ($num_regole_esistente) {
if (!empty($regola_2b)) $reg_corr = risul_query($regola_esistente,0,'motivazione2');
else $reg_corr = risul_query($regola_esistente,0,'motivazione');
} # fine if ($num_regole_esistente)
else $reg_corr = "";
$appartamenti = esegui_query("select idappartamenti from $tableappartamenti");
$num_appartamenti = numlin_query($appartamenti);
$app_esistente = array();
for ($num1 = 0 ; $num1 < $num_appartamenti ; $num1 = $num1 + 1) {
$appartamento = risul_query($appartamenti,$num1,'idappartamenti');
if ($appartamenti_consentiti[$appartamento] == "SI" or strstr(",$reg_corr,",",$appartamento,")) $app_esistente[$appartamento] = "SI";
} # fine for $num1
$vett_app = explode(",",$lista_app);
$num_app = count($vett_app);
$app_trovato = array();
for ($num1 = 0 ; $num1 < $num_app ; $num1 = $num1 + 1) {
if (!isset($app_esistente[$vett_app[$num1]]) or $app_esistente[$vett_app[$num1]] != "SI") {
$inserire = "NO";
$error_messages[] = mex("L'appartamento",'unit.php')." ".htmlspecialchars($vett_app[$num1])." ".mex(" non esiste",$pag)."";
} # fine if (!isset($app_esistente[$vett_app[$num1]]) or $app_esistente[$appartamento] != "SI")
else {
if (!empty($app_trovato[$vett_app[$num1]])) $lista_app = substr(str_replace(",".$vett_app[$num1].",",",",",$lista_app,"),1,-1).",".$vett_app[$num1];
else $num_app_reali++;
$app_trovato[$vett_app[$num1]] = 1;
} # fine else if (!isset($app_esistente[$vett_app[$num1]]) or $app_esistente[$appartamento] != "SI")
} # fine for $num1
} # fine else if (empty($lista_app))
$num_da_assegnare = 1;
if (!empty($regola_2)) {
$num_da_assegnare = $num_apti;
if ($num_apti < 1 or $num_apti > $num_appartamenti) $inserire = "NO";
if ($num_regole_esistente) {
$regola_2b_esist = risul_query($regola_esistente,0,'motivazione2');
if (strcmp((string) $regola_2b_esist,"")) {
$num_regola_2b_esist = explode(",",$regola_2b_esist);
$num_regola_2b_esist = count($num_regola_2b_esist);
if ($num_regola_2b_esist < $num_app_reali) $num_app_reali = $num_regola_2b_esist;
} # fine if if (strcmp((string) $regola_2b_esist,""))
} # fine if ($num_regole_esistente)
} # fine if (!empty($regola_2))
elseif ($num_regole_esistente) {
$num_da_assegnare = (string) risul_query($regola_esistente,0,'motivazione3');
if (substr($num_da_assegnare,0,1) == "v") $num_da_assegnare = substr($num_da_assegnare,1);
} # fine elseif ($num_regole_esistente)
if ($num_da_assegnare and $num_da_assegnare > $num_app_reali and $inserire != "NO") {
$inserire = "NO";
$error_messages[] = mex("Numero di appartamenti",'unit.php')." ".mex("da assegnare troppo alto, supera quello presente nella lista",$pag)."";
} # fine if ($num_da_assegnare and $num_da_assegnare > $num_app_reali and...
if (!empty($regola_2b)) {
if (!$num_giorni or controlla_num_pos($num_giorni) == "NO") {
$inserire = "NO";
$error_messages[] = mex("Si deve inserire il numero di giorni",$pag)."";
} # fine if (!$num_giorni or controlla_num_pos($num_giorni) == "NO")
if ($ini_fine != "ini" and $ini_fine != "fine") $inserire = "NO";
if ($ini_fine == "ini") {
$num_giorni_ini = "'$num_giorni'";
$num_giorni_fine = "NULL";
} # fine if ($ini_fine == "ini")
if ($ini_fine == "fine") {
$num_giorni_ini = "NULL";
$num_giorni_fine = "'$num_giorni'";
} # fine if ($ini_fine == "fine")
} # fine if (!empty($regola_2b))
if (!isset($v_apti) or $v_apti != "v") $v_apti = "";

if ($inserire != "NO") {
if ($num_regole_esistente != 0) {
if (!empty($regola_2b)) {
esegui_query("update $tableregole set motivazione2 = '$lista_app', iddatainizio = $num_giorni_ini, iddatafine = $num_giorni_fine where tariffa_per_app = '".aggslashdb($tipotariffa)."'");
$lista_app_norm = risul_query($regola_esistente,0,'motivazione');
$lista_app_ecc = $lista_app;
} # fine if (!empty($regola_2b))
else {
esegui_query("update $tableregole set motivazione = '$lista_app' where tariffa_per_app = '".aggslashdb($tipotariffa)."'");
if ($num_apti > 1) esegui_query("update $tableregole set motivazione3 = '$v_apti$num_apti' where tariffa_per_app = '".aggslashdb($tipotariffa)."'");
else esegui_query("update $tableregole set motivazione3 = NULL where tariffa_per_app = '".aggslashdb($tipotariffa)."'");
$lista_app_norm = $lista_app;
$lista_app_ecc = risul_query($regola_esistente,0,'motivazione2');
} # fine else if (!empty($regola_2b))
if ($lista_app_norm and $lista_app_ecc) {
$vett_app = explode(",",$lista_app_norm);
$num_app = count($vett_app);
$app_mancanti_da_ecc = "";
for ($num1 = 0 ; $num1 < $num_app ; $num1 = $num1 + 1) {
if (str_replace(",".$vett_app[$num1].",","",",$lista_app_ecc,") == ",$lista_app_ecc,") $app_mancanti_da_ecc .= ", ".$vett_app[$num1];
} # fine for $num1
if ($app_mancanti_da_ecc) $warning_messages[] = mex("Attenzione",$pag).": ".mex("ci sono appartamenti",'unit.php')." (".substr($app_mancanti_da_ecc,2).") ".mex("della regola 2 mancanti nella eccezione alla regola",$pag)."";
} # fine if ($lista_app_norm and $lista_app_ecc)
$success_messages[] = mex("La regola di assegnazione",$pag)." 2 ".mex("è stata modificata",$pag)."";
} # fine if ($num_regole_esistente != 0)
else {
$idregole = esegui_query("select max(idregole) from $tableregole");
$idregole = risul_query($idregole,0,0);
$idregole = $idregole + 1;
if (!empty($regola_2b)) esegui_query("insert into $tableregole (idregole,tariffa_per_app,iddatainizio,motivazione2) values ('$idregole','".aggslashdb($tipotariffa)."','$num_giorni','$lista_app')");
else {
esegui_query("insert into $tableregole (idregole,tariffa_per_app,motivazione) values ('$idregole','".aggslashdb($tipotariffa)."','$lista_app')");
if ($num_apti > 1) esegui_query("update $tableregole set motivazione3 = '$v_apti$num_apti' where tariffa_per_app = '".aggslashdb($tipotariffa)."'");
} # fine else if (!empty($regola_2b))
$success_messages[] = mex("La regola di assegnazione",$pag)." 2 ".mex("è stata inserita",$pag)."";
} # fine else if ($num_regole_esistente != 0)
$aggiorna_ic_disp = 1;
} # fine if ($inserire != "NO")
} # fine if ((!empty($regola_2) or !empty($regola_2b)) and $priv_mod_reg2 == "s")


if ((!empty($regola_3) or !empty($regola_3m)) and $priv_mod_reg2 == "s") {
$active_panel = 'rule3';
$inserire = "";
if (substr($tipotariffa,0,7) != "tariffa" or controlla_num_pos(substr($tipotariffa,7)) == "NO" or ($attiva_tariffe_consentite != "n" and !isset($tariffe_consentite_vett[substr($tipotariffa,7)]))) {
$inserire = "NO";
$error_messages[] = mex("Si deve scegliere la tariffa",$pag)."";
} # fine if (substr($tipotariffa,0,7) != "tariffa" or...
else {
$regola_esistente = esegui_query("select * from $tableregole where tariffa_per_persone = '".aggslashdb($tipotariffa)."'");
$num_regole_esistente = numlin_query($regola_esistente);
/*
if ($num_regole_esistente != 0) {
$inserire = "NO";
$error_messages[] = mex("La tariffa scelta ha già un numero di persone associato, cancella la regola prima di inserirne una nuova",$pag)."";
} # fine if ($num_regole_esistente != 0)
*/
if (controlla_num_pos($num_persone) != "SI" or (!$num_persone and !$num_regole_esistente)) {
$inserire = "NO";
$error_messages[] = mex("Si deve inserire il numero di persone da associare",$pag)."";
} # fine if (controlla_num_pos($num_persone) != "SI" or...
} # fine else if (substr($tipotariffa,0,7) != "tariffa" or...
if ((!isset($inserire) or $inserire != "NO") and $num_regole_esistente) {
if (!empty($regola_3m)) {
$pers_min = $num_persone;
$pers_def = (string) risul_query($regola_esistente,0,'iddatainizio');
} # fine if (!empty($regola_3m))
else {
$pers_min = (string) risul_query($regola_esistente,0,'iddatafine');
$pers_def = $num_persone;
} # fine else if (!empty($regola_3m))
if ($pers_def and $pers_min > $pers_def) {
$inserire = "NO";
$error_messages[] = mex("Il numero di persone minimo",$pag)." ($pers_min) ".mex("è maggiore del numero di persone predefinito",$pag)." ($pers_def)";
} # fine if ($pers_def and $pers_min > $pers_def)
} # fine if ((!isset($inserire) or $inserire != "NO") and $num_regole_esistente)
if ($inserire != "NO") {
$regola2_esistente = esegui_query("select * from $tableregole where tariffa_per_app = '".aggslashdb($tipotariffa)."'");
if (numlin_query($regola2_esistente)) {
$lista_app2 = risul_query($regola2_esistente,0,'motivazione');
$lista_app2_ecc = risul_query($regola2_esistente,0,'motivazione2');
if ($lista_app2_ecc) {
if ($lista_app2) $lista_app2 .= ",";
$lista_app2 .= $lista_app2_ecc;
} # fine if ($lista_app2_ecc)
$vett_app2 = explode(",",$lista_app2);
$num_app2 = count($vett_app2);
$app_trovato = array();
$app_piccoli = "";
$app_compatibile = 0;
for ($num1 = 0 ; $num1 < $num_app2 ; $num1++) {
if (empty($app_trovato[$vett_app2[$num1]])) {
$app_trovato[$vett_app2[$num1]] = 1;
$app = esegui_query("select maxoccupanti from $tableappartamenti where idappartamenti = '".aggslashdb($vett_app2[$num1])."' ");
if (numlin_query($app)) {
$pers_max = risul_query($app,0,'maxoccupanti');
if ($pers_max < $num_persone) $app_piccoli .= ", ".$vett_app2[$num1];
else $app_compatibile = 1;
} # fine if (numlin_query($app))
} # fine if (empty($app_trovato[$vett_app2[$num1]]))
} # fine for $num1
if (!$app_compatibile) echo "<br><b class=\"colwarn\">".mex("Attenzione",$pag)."</b>: ".lcfirst(mex("Non ci sono appartamenti con le caratteristiche richieste",'unit.php'))." ".mex("nella regola di assegnazione 2 di questa tariffa",$pag).".<br>";
elseif ($app_piccoli) echo "<br><b class=\"colwarn\">".mex("Attenzione",$pag)."</b>: ".mex("ci sono appartamenti",'unit.php')." (<b>".substr($app_piccoli,2)."</b>) ".mex("nella regola 2 di questa tariffa che non possono ospitare",$pag)." $num_persone ".mex("persone",$pag).".<br>";
} # fine if (numlin_query($regola2_esistente))
if (!empty($regola_3m)) $campo = 'iddatafine';
else $campo = 'iddatainizio';
if ($num_regole_esistente != 0) {
if (!$num_persone) {
if (!$pers_def and !$pers_min) esegui_query("delete from $tableregole where tariffa_per_persone = '".aggslashdb($tipotariffa)."'");
else esegui_query("update $tableregole set $campo = NULL where tariffa_per_persone = '".aggslashdb($tipotariffa)."'");
$success_messages[] = mex("La regola di assegnazione",$pag)." 3 ".mex("è stata cancellata",$pag)."";
} # fine if (!$num_persone)
else {
esegui_query("update $tableregole set $campo = '".aggslashdb($num_persone)."' where tariffa_per_persone = '".aggslashdb($tipotariffa)."'");
$success_messages[] = mex("La regola di assegnazione",$pag)." 3 ".mex("è stata modificata",$pag)."";
} # fine else if (!$num_persone)
} # fine if ($num_regole_esistente != 0)
else {
$idregole = esegui_query("select max(idregole) from $tableregole");
$idregole = risul_query($idregole,0,0);
$idregole = $idregole + 1;
esegui_query("insert into $tableregole (idregole,tariffa_per_persone,$campo) values ('$idregole','".aggslashdb($tipotariffa)."', '".aggslashdb($num_persone)."')");
$success_messages[] = mex("La regola di assegnazione",$pag)." 3 ".mex("è stata inserita",$pag)."";
} # fine else if ($num_regole_esistente != 0)
$aggiorna_ic_tar = 1;
} # fine if ($inserire != "NO")
} # fine if ((!empty($regola_3) or !empty($regola_3m)) and $priv_mod_reg2 == "s")


if (!empty($regola_4) and $id_utente == 1) {
$active_panel = 'rule4';
$inserire = "";
if (substr($tipotariffa,0,7) != "tariffa" or controlla_num_pos(substr($tipotariffa,7)) == "NO") {
$inserire = "NO";
$error_messages[] = mex("Si deve scegliere la tariffa",$pag)."";
} # fine if (substr($tipotariffa,0,7) != "tariffa" or...
else {
$regola_esistente = esegui_query("select * from $tableregole where tariffa_per_utente = '".aggslashdb($tipotariffa)."'");
$num_regole_esistente = numlin_query($regola_esistente);
/*
if ($num_regole_esistente != 0) {
$inserire = "NO";
$error_messages[] = mex("La tariffa scelta ha già un utente associato, cancella la regola prima di inserirne una nuova",$pag)."";
} # fine if ($num_regole_esistente != 0)
*/
} # fine else if (substr($tipotariffa,0,7) != "tariffa" or...
if (!$id_utente_inserimento) {
$inserire = "NO";
$error_messages[] = mex("Si deve inserire l'utente da associare",$pag)."";
} # fine if (!$id_utente_inserimento)
else {
$id_utente_inserimento = aggslashdb($id_utente_inserimento);
$utente_tariffa = esegui_query("select nome_utente from $tableutenti where idutenti = '$id_utente_inserimento'");
if (numlin_query($utente_tariffa) != 1) {
$inserire = "NO";
$error_messages[] = mex("L'utente ",$pag).$id_utente_inserimento.mex(" non esiste",$pag)."";
} # fine if ($numlin_query($utente_tariffa) != 1)
} # fine else if (!$id_utente_inserimento)
if ($inserire != "NO") {
if ($num_regole_esistente != 0) {
esegui_query("update $tableregole set iddatainizio = '$id_utente_inserimento' where tariffa_per_utente = '".aggslashdb($tipotariffa)."'");
$success_messages[] = mex("La regola di assegnazione",$pag)." 4 ".mex("è stata modificata",$pag)."";
} # fine if ($num_regole_esistente != 0)
else {
$idregole = esegui_query("select max(idregole) from $tableregole");
$idregole = risul_query($idregole,0,0);
$idregole = $idregole + 1;
esegui_query("insert into $tableregole (idregole,tariffa_per_utente,iddatainizio) values ('$idregole','".aggslashdb($tipotariffa)."', '$id_utente_inserimento')");
$success_messages[] = mex("La regola di assegnazione",$pag)." 4 ".mex("è stata inserita",$pag)."";
} # fine else if ($num_regole_esistente != 0)
} # fine if ($inserire != "NO")
} # fine if (!empty($regola_4) and $id_utente == 1)


if ($tabelle_lock) unlock_tabelle($tabelle_lock);

// Check if we need to show confirmations
$show_confirmations = (!empty($show_rule1_confirmation) || !empty($show_rule1b_confirmation) || !empty($show_delete_rule1_confirmation));

// Run interconnect updates if needed
if (!$show_confirmations && ($aggiorna_ic_disp or $aggiorna_ic_tar)) {
    $lock = 1;
    $aggiorna_disp = $aggiorna_ic_disp;
    $aggiorna_tar = $aggiorna_ic_tar;
    if (@function_exists('pcntl_fork')) include("./includes/interconnect/aggiorna_ic_fork.php");
    else include("./includes/interconnect/aggiorna_ic.php");
}

// After processing, always show the main form (not intermediate "go back" page)
$show_main_form = true;

} # fine if (!empty($inserisci))

// Show the main form (whether after processing or initial load)
if (empty($inserisci) or !empty($show_main_form)) {




# Form iniziale di inserimento
echo "<br><h4 id=\"h_irul\"><span>".mex("Inserisci le regole di assegnazione per le prenotazioni dell'anno",$pag)." $anno
</span></h4><br><hr style=\"width: 95%\">";

$mostra_solo_regola = 0;
$origine = htmlspecialchars(fixstr($origine));
if ($origine == "tab_reg1") {
$mostra_solo_regola = 1;
$origine = "visualizza_tabelle.php?tipo_tabella=regole#hreg1";
} # fine if ($origine == "tab_reg1")
if ($origine == "tab_reg2") {
$mostra_solo_regola = 2;
$origine = "visualizza_tabelle.php?tipo_tabella=regole#hreg2";
} # fine if ($origine == "tab_reg2")
if ($origine == "tab_reg3") {
$mostra_solo_regola = 3;
$origine = "visualizza_tabelle.php?tipo_tabella=regole#hreg3";
} # fine if ($origine == "tab_reg3")
if ($origine == "tab_tariffa") {
if (!empty($tipotariffa_regola2)) $mostra_solo_regola = 2;
if (!empty($tipotariffa_regola3)) $mostra_solo_regola = 3;
$origine = "tab_tariffe.php?numtariffa1=".htmlspecialchars(substr(fixstr($tipotariffa_regola2).fixstr($tipotariffa_regola3),7));
} # fine if ($origine == "tab_tariffa")
if (substr($origine,0,3) == "ic_") {
$orig = explode("_",$origine);
if ($orig[1] == "r2") $mostra_solo_regola = 2;
if ($orig[1] == "r3") $mostra_solo_regola = 3;
$origine = "interconnessioni.php#".$orig[2];
} # fine if (substr($origine,0,3) == "ic_")
$rigatariffe = esegui_query("select * from $tablenometariffe where idntariffe = 1 ");
$numero_tariffe = risul_query($rigatariffe,0,'nomecostoagg');


if ((!$mostra_solo_regola or $mostra_solo_regola == 1) and $priv_mod_reg1 != "n") {
$tipo_mod_reg1 = "";
$inizio_select_orig = "";
$fine_select_orig = "";
if (!empty($mod_idregola1) and controlla_num_pos($mod_idregola1) == "SI") {
$reg_esist = esegui_query("select * from $tableregole where idregole = '$mod_idregola1' ");
if (numlin_query($reg_esist)) {
$app_agenzia = risul_query($reg_esist,0,'app_agenzia');
$tariffa_chiusa = risul_query($reg_esist,0,'tariffa_chiusa');
if ($app_agenzia) $tipo_mod_reg1 = "a";
elseif ($tariffa_chiusa) $tipo_mod_reg1 = "t";
if ($tipo_mod_reg1) {
$iddatainizio = risul_query($reg_esist,0,'iddatainizio');
$iddatafine = risul_query($reg_esist,0,'iddatafine');
$motivazione = risul_query($reg_esist,0,'motivazione');
$motivazione2 = (string) risul_query($reg_esist,0,'motivazione2');
$inizio_select = esegui_query("select datainizio from $PHPR_TAB_PRE"."periodi$anno where idperiodi = '$iddatainizio' ");
$inizio_select = risul_query($inizio_select,0,'datainizio');
$fine_select = esegui_query("select datafine from $PHPR_TAB_PRE"."periodi$anno where idperiodi = '$iddatafine' ");
$fine_select = risul_query($fine_select,0,'datafine');
} # fine if ($tipo_mod_reg1)
} # fine if (numlin_query($reg_esist))
} # fine if (!empty($mod_idregola1) and controlla_num_pos($mod_idregola1) == "SI")
if (!$tipo_mod_reg1) {
$transaz_esist_1a = esegui_query("select * from $tabletransazioni where idsessione = '$id_sessione' and tipo_transazione = 'cr_1a' ");
$transaz_esist_1t = esegui_query("select * from $tabletransazioni where idsessione = '$id_sessione' and tipo_transazione = 'cr_1t' ");
if (!numlin_query($transaz_esist_1a) or !numlin_query($transaz_esist_1t)) {
$inizio_select = "";
$fine_select = "";
$motivazione_orig = htmlspecialchars(fixstr($motivazione),ENT_COMPAT);
$oggi = date("Y-m-d",(time() + (C_DIFF_ORE * 3600)));
$date_select = esegui_query("select datainizio,datafine from $PHPR_TAB_PRE"."periodi$anno where datainizio <= '$oggi' and datafine > '$oggi' ");
if (numlin_query($date_select) != 0) {
$inizio_select_orig = risul_query($date_select,0,'datainizio');
$fine_select_orig = risul_query($date_select,0,'datafine');
} # fine if (numlin_query($date_select) != 0)
} # fine if (!numlin_query($transaz_esist_1a) or !numlin_query($transaz_esist_1t))
if (numlin_query($transaz_esist_1a)) {
$inizio_select = risul_query($transaz_esist_1a,0,'dati_transazione1');
$fine_select = risul_query($transaz_esist_1a,0,'dati_transazione2');
$motivazione = risul_query($transaz_esist_1a,0,'dati_transazione3');
$motivazione2 = (string) risul_query($transaz_esist_1a,0,'dati_transazione4');
if ($motivazione2) $motivazione2 = "x";
} # fine if (numlin_query($transaz_esist_1a))
else {
$inizio_select = $inizio_select_orig;
$fine_select = $fine_select_orig;
$motivazione = $motivazione_orig;
unset($motivazione2);
} # fine else if (numlin_query($transaz_esist_1a))
} # fine if (!$tipo_mod_reg1)

// Load and restore transaction data for tariff closures before template display
if (($priv_mod_reg1 == "s" or $priv_mod_reg1 == "t") and $tipo_mod_reg1 != "a" and !$tipo_mod_reg1) {
if (numlin_query($transaz_esist_1t)) {
$inizio_select = risul_query($transaz_esist_1t,0,'dati_transazione1');
$fine_select = risul_query($transaz_esist_1t,0,'dati_transazione2');
$motivazione = risul_query($transaz_esist_1t,0,'dati_transazione3');
$tariffa_chiusa = risul_query($transaz_esist_1t,0,'dati_transazione4');
} # fine if (numlin_query($transaz_esist_1t))
else {
$inizio_select = $inizio_select_orig;
$fine_select = $fine_select_orig;
$motivazione = $motivazione_orig;
} # fine else if (numlin_query($transaz_esist_1a))
} # fine else if (!$tipo_mod_reg1)

// Initialize $tutte_tar_consentite before template (needed for delete button visibility)
$tutte_tar_consentite = 1;
for ($numtariffa = 1 ; $numtariffa <= $numero_tariffe ; $numtariffa = $numtariffa + 1) {
if (!($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[$numtariffa]))) $tutte_tar_consentite = 0;
}

// Display Rule 1 Panel: Closures (Apartments and Tariffs)
HotelDruidTemplate::getInstance()->display('crearegole/panel_rule1', get_defined_vars());
} # fine if ((!$mostra_solo_regola or $mostra_solo_regola == 1) and $priv_mod_reg1 != "n")


if ((!$mostra_solo_regola or $mostra_solo_regola == 2) and $priv_mod_reg2 == "s") {
echo "<a name=\"regola2\"></a>";

// Prepare data for Rule 2 Panel: Apartment Types Assignment
$lista_maxocc = "";
$lista_maxocc_arr = array();
for ($num1 = 0 ; $num1 < $num_appartamenti ; $num1++) {
$app = risul_query($appartamenti,$num1,'idappartamenti');
if ($appartamenti_consentiti[$app] == "SI") {
$maxocc = @risul_query($appartamenti,$num1,'maxoccupanti');
if ($maxocc !== false && $maxocc !== null) {
if (!isset($lista_maxocc_arr[$maxocc])) {
$lista_maxocc_arr[$maxocc] = 1;
if ($lista_maxocc) $lista_maxocc .= ",";
$lista_maxocc .= $maxocc;
}
}
}
}

$regole2 = esegui_query("select * from $tableregole where tariffa_per_app != ''");
$num_regole2 = numlin_query($regole2);
$lista_tar_reg2 = "";
$lista_tar_reg2n = "";
$lista_tar_reg2v = "";
$lista_tar_reg2b = "";
$lista_tar_reg2b_gg = "";
$lista_tar_reg2b_if = "";
for ($num1 = 0 ; $num1 < $num_regole2 ; $num1++) {
$tariffa = risul_query($regole2,$num1,'tariffa_per_app');
if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[substr($tariffa,7)])) {
$lista_app = risul_query($regole2,$num1,'motivazione');
if ($lista_app) $lista_tar_reg2 .= "if (tariffa == \"$tariffa\") ind_val = \"$lista_app\";
";
$num_apti = (string) risul_query($regole2,$num1,'motivazione3');
if ($num_apti) {
if (substr($num_apti,0,1) == "v") {
$num_apti = substr($num_apti,1);
$lista_tar_reg2v .= "if (tariffa == \"$tariffa\") apti_var_v.checked = true;
";
}
$lista_tar_reg2n .= "if (tariffa == \"$tariffa\") apti_var.value = \"$num_apti\";
";
}
$lista_app_b = risul_query($regole2,$num1,'motivazione2');
if ($lista_app_b) $lista_tar_reg2b .= "if (tariffa == \"$tariffa\") ind_val = \"$lista_app_b\";
";
$num_giorni_b_i = risul_query($regole2,$num1,'iddatainizio');
$num_giorni_b_f = risul_query($regole2,$num1,'iddatafine');
if ($num_giorni_b_i) {
$lista_tar_reg2b_gg .= "if (tariffa == \"$tariffa\") giorni_var.value = \"$num_giorni_b_i\";
";
$lista_tar_reg2b_if .= "if (tariffa == \"$tariffa\") ini_fine_i.checked = true;
";
}
if ($num_giorni_b_f) {
$lista_tar_reg2b_gg .= "if (tariffa == \"$tariffa\") giorni_var.value = \"$num_giorni_b_f\";
";
$lista_tar_reg2b_if .= "if (tariffa == \"$tariffa\") ini_fine_f.checked = true;
";
}
}
}

// Display Rule 2 Panel: Apartment Types Assignment
HotelDruidTemplate::getInstance()->display('crearegole/panel_rule2', get_defined_vars());
} # fine if ((!$mostra_solo_regola or $mostra_solo_regola == 2) and $priv_mod_reg2 == "s")


if ((!$mostra_solo_regola or $mostra_solo_regola == 3) and $priv_mod_reg2 == "s") {
// Prepare data for Rule 3 Panel: Number of Persons Auto-Assignment
$lista_tariffe3 = "";
for ($numtariffa = 1 ; $numtariffa <= $numero_tariffe ; $numtariffa = $numtariffa + 1) {
if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[$numtariffa])) {
$tariffa = "tariffa".$numtariffa;
$tariffa_vedi = mex("tariffa","prenota.php").$numtariffa;
$nometariffa = risul_query($rigatariffe,0,$tariffa);
if ($nometariffa != "") $nometariffa_vedi = " ($nometariffa)";
else $nometariffa_vedi = "";
$lista_tariffe3 .= "<option value=\"$tariffa\">$tariffa_vedi$nometariffa_vedi</option>";
} # fine if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[$numtariffa]))
} # fine for $numtariffa
$lista_tar_reg3 = "";
$lista_tar_reg3min = "";
$regole3 = esegui_query("select * from $tableregole where tariffa_per_persone != ''");
$num_regole3 = numlin_query($regole3);
for ($num1 = 0 ; $num1 < $num_regole3 ; $num1++) {
$tariffa = risul_query($regole3,$num1,'tariffa_per_persone');
if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[substr($tariffa,7)])) {
$persone = risul_query($regole3,$num1,'iddatainizio');
if ($persone) $lista_tar_reg3 .= "if (tariffa == \"$tariffa\") txt_box.value = \"$persone\";
";
$minpers = risul_query($regole3,$num1,'iddatafine');
if ($minpers) $lista_tar_reg3min .= "if (tariffa == \"$tariffa\") txt_box.value = \"$minpers\";
";
} # fine if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[substr($tariffa,7)]))
} # fine for $num1

// Display Rule 3 Panel: Number of Persons Auto-Assignment
HotelDruidTemplate::getInstance()->display('crearegole/panel_rule3', get_defined_vars());
} # fine if ((!$mostra_solo_regola or $mostra_solo_regola == 3) and $priv_mod_reg2 == "s")


if ((!$mostra_solo_regola or $mostra_solo_regola == 4) and $id_utente == 1) {
// Prepare data for Rule 4 Panel: User Insertion Assignment
$regole4 = esegui_query("select * from $tableregole where tariffa_per_utente != ''");
$num_regole4 = numlin_query($regole4);
$lista_tar_reg4 = "";
for ($num1 = 0 ; $num1 < $num_regole4 ; $num1++) {
$tariffa = risul_query($regole4,$num1,'tariffa_per_utente');
$utente_t = risul_query($regole4,$num1,'iddatainizio');
$lista_tar_reg4 .= "if (tariffa == \"$tariffa\") ind_val = \"$utente_t\";
";
} # fine for $num1

$tutti_utenti = esegui_query("select * from $tableutenti order by idutenti");
$num_tutti_utenti = numlin_query($tutti_utenti);
$option_select_utenti = "";
$nome_utente1 = "";
for ($num1 = 0 ; $num1 < $num_tutti_utenti ; $num1++) {
$idutenti = risul_query($tutti_utenti,$num1,'idutenti');
if ($idutenti != 1) {
$nome_utente_option = risul_query($tutti_utenti,$num1,'nome_utente');
$option_select_utenti .= "<option value=\"$idutenti\">$nome_utente_option</option>";
} # fine if ($idutenti != $id_utente_inserimento)
else $nome_utente1 = risul_query($tutti_utenti,$num1,'nome_utente');
} # fine for $num1

// Display Rule 4 Panel: User Insertion Assignment
HotelDruidTemplate::getInstance()->display('crearegole/panel_rule4', get_defined_vars());
} # fine if ((!$mostra_solo_regola or $mostra_solo_regola == 4) and $id_utente == 1)


if (!$mostra_solo_regola and $priv_ins_costi_agg != "n") {
// Display Quick Insert Additional Cost Panel
HotelDruidTemplate::getInstance()->display('crearegole/panel_quick_cost', get_defined_vars());
} # fine if (!$mostra_solo_regola and $priv_ins_costi_agg != "n")


echo "<div style=\"text-align: center;\"><br>";
if (!$origine) {
echo "<form accept-charset=\"utf-8\" method=\"post\" action=\"visualizza_tabelle.php\"><div>
<input type=\"hidden\" name=\"anno\" value=\"$anno\">
<input type=\"hidden\" name=\"id_sessione\" value=\"$id_sessione\">
<input type=\"hidden\" name=\"tipo_tabella\" value=\"regole\">
<button class=\"rule\" type=\"submit\"><div>".mex("Vedi le regole già inserite",$pag)."</div></button>
</div></form><br>
<form accept-charset=\"utf-8\" method=\"post\" action=\"inizio.php\"><div>
<input type=\"hidden\" name=\"anno\" value=\"$anno\">
<input type=\"hidden\" name=\"id_sessione\" value=\"$id_sessione\">
<button class=\"bkmm\" type=\"submit\"><div>".mex("Torna al menù principale",$pag)."</div></button>
<br></div></form>";
} # fine if (!$origine)
else {
echo "<form accept-charset=\"utf-8\" method=\"post\" action=\"".controlla_pag_origine($origine)."\"><div>
<input type=\"hidden\" name=\"anno\" value=\"$anno\">
<input type=\"hidden\" name=\"id_sessione\" value=\"$id_sessione\">
<button class=\"gobk\" type=\"submit\"><div>".mex("Torna indietro",$pag)."</div></button>
<br></div></form>";
} # fine else if (!$origine)
echo "<br></div>";



} # fine else if ($inserisci)



if ($tema[$id_utente] and $tema[$id_utente] != "base" and @is_dir("./themes/".$tema[$id_utente]."/php")) include("./themes/".$tema[$id_utente]."/php/foot.php");
else include("./includes/foot.php");


} # fine if ($anno_utente_attivato == "SI" and ($priv_mod_reg1 != "n" or $priv_mod_reg2 != "n"))
} # fine if ($id_utente)



?>
