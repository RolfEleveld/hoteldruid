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

#Funzioni per usare il database SQLITE

// Only define the DB helpers once to avoid redeclare errors when the
// same file is included multiple times during a request lifecycle.
if (!function_exists('connetti_db')) {

ignore_user_abort(1);

// variabili per le differenze nella sintassi delle query
//global $ILIKE,$LIKE;
$ILIKE = "LIKE";
$LIKE = "GLOB";
$DATETIME = "text";
$MEDIUMTEXT = "text";



function connetti_db ($database,$host,$port,$user,$password,$estensione) {

if ($estensione == "SI") dl("sqlite3.so");
// Determine the data path
$dati_path = defined("C_PERCORSO_A_DATI") ? C_PERCORSO_A_DATI : C_DATI_PATH;
// Ensure the directory exists before creating SQLite database
if (!is_dir($dati_path)) {
    @mkdir($dati_path, 0755, true);
}
// Construct database file path
if (defined("C_PERCORSO_A_DATI")) {
    $db_file_path = C_PERCORSO_A_DATI."db_".$database;
} else {
    $db_file_path = C_DATI_PATH."/db_".$database;
}
$numconnessione = new SQLite3($db_file_path);
$numconnessione->busyTimeout(60000);
return $numconnessione;

} # fine function connetti_db



function disconnetti_db ($numconnessione) {

$risul = $numconnessione->close();
return $risul;

} # fine function disconnetti_db



function prepara_query_sqlite (&$query) {

if (str_replace(" GLOB '","",$query) != $query) {
$query .= " ";
$q_vett = explode(" GLOB '",$query);
for ($n = 1 ; $n < count($q_vett) ; $n++) {
if (substr(str_replace("''","^'^",$q_vett[$n]),0,1) != "'") {
$arg = str_replace("''","^'^",$q_vett[$n]);
$arg = explode("' ",$arg);
$arg = str_replace("^'^","''",$arg[0]);
if (str_replace("''","",$arg) == str_replace("'","",str_replace("''","",$arg))) {
$query = str_replace(" GLOB '$arg' "," GLOB '".str_replace("%","*",str_replace("_","?",$arg))."' ",$query);
} # fine if (str_replace("''","",$arg) == str_replace("'","",str_replace("''","",$arg)))
} # fine if (substr(str_replace("''","",$q_vett[$n]),0,1) != "'")
} # fine for $n
} # fine if (str_replace(" GLOB '","",$query) != $query)

} # fine function prepara_query_sqlite



function esegui_query_reale ($query,$silenzio = "") {
global $numconnessione;
prepara_query_sqlite($query);

// Retry loop to handle transient SQLITE_BUSY ('database is locked') errors.
$risul = false;
$max_attempts = 6;
for ($attempt = 1; $attempt <= $max_attempts; $attempt++) {
	// Suppress PHP warning emission from SQLite3::query and capture error message ourselves.
	$risul = @$numconnessione->query($query);
	if ($risul !== false) break;
	$errcode = $numconnessione->lastErrorCode();
	// 5 is SQLITE_BUSY
	if ($errcode == 5 && $attempt < $max_attempts) {
		// exponential backoff (in microseconds)
		usleep(50000 * $attempt);
		continue;
	}
	break;
}
if ($risul) {
$risultato = array();
$num1 = 0;
if (strtolower(substr(trim($query),0,6)) == "select" and is_object($risul)) {
while ($risultato[$num1] = $risul->fetchArray(SQLITE3_ASSOC)) $num1++;
$risultato['numcol'] = $risul->numColumns();
for ($num2 = 0 ; $num2 < $risultato['numcol'] ; $num2++) $risultato['col'][$num2] = $risul->columnName($num2);
$risul->finalize();
} # fine if (strtolower(substr(trim($query),0,6)) == "select" and is_object($risul))
$risultato['num'] = $num1;
} # fine if ($risul)
else $risultato = $risul;

if (!$risul and $silenzio != "totale") {
	// Store the last error message for callers to inspect
	global $ULTIMO_ERRORE_SQLITE;
	$ULTIMO_ERRORE_SQLITE = $numconnessione->lastErrorMsg();
	global $PHPR_TAB_PRE;
	// Do not echo raw database errors directly to the page (prevents leaking internals
	// and avoids breaking panel-based UX). Log the full query and DB message instead.
	error_log("IN " . $_SERVER['PHP_SELF'] . " SQLITE ERROR: " . str_replace(" " . $PHPR_TAB_PRE, " ", $query) . " => " . $ULTIMO_ERRORE_SQLITE);
}

return $risultato;

} # fine function esegui_query_reale


function ultimo_errore_sqlite() {
	global $ULTIMO_ERRORE_SQLITE;
	if (isset($ULTIMO_ERRORE_SQLITE)) return $ULTIMO_ERRORE_SQLITE;
	return "";
}



if (substr($PHPR_LOG,0,2) != "SI") {

function esegui_query ($query,$silenzio = "",$idlog = "") {
$risul = esegui_query_reale($query,$silenzio);
return $risul;
} # fine function esegui_query

} # fine if (substr($PHPR_LOG,0,2) != "SI")



else {
if (!function_exists("inserisci_log")) include("./includes/funzioni_log.php");

function esegui_query ($query,$silenzio = "",$idlog = "") {
$risul = esegui_query_reale($query,$silenzio);

if ($idlog != 1) inserisci_log($query,$idlog);

return $risul;

} # fine function esegui_query

} # fine else if (substr($PHPR_LOG,0,2) != "SI")



function risul_query ($query,$riga,$colonna,$tab="") {

//#if ($tab) $colonna = "$tab.$colonna";
// Defensive: if the query failed or is not an array, return empty string
if (!is_array($query)) return "";
if ($colonna === null || $colonna === '') return "";
if (is_integer($colonna)) {
	if (!isset($query['col'][$colonna])) return "";
	$colonna = $query['col'][$colonna];
}
if (!isset($query[$riga]) || !is_array($query[$riga])) return "";
if (!array_key_exists($colonna, $query[$riga])) return "";
$risul = $query[$riga][$colonna];

return $risul;

} # fine function risul_query



function numlin_query ($query) {

if (!is_array($query) || !isset($query['num'])) return 0;
return (int)$query['num'];

} # fine function numlin_query



function aggslashdb ($stringa) {
global $numconnessione;

$risul = $numconnessione->escapeString((string) $stringa);
return $risul;

} # fine function aggslashdb



function arraylin_query ($query,$num) {

if (!is_array($query) || !isset($query['numcol']) || !isset($query['col'])) return array();
$risul = array();
for ($num1 = 0 ; $num1 < $query['numcol'] ; $num1++) {
	$colname = isset($query['col'][$num1]) ? $query['col'][$num1] : null;
	if ($colname !== null && isset($query[$num]) && array_key_exists($colname, $query[$num])) {
		$risul[$num1] = $query[$num][$colname];
	} else {
		$risul[$num1] = null;
	}
}
return $risul;

} # fine function arraylin_query



function numcampi_query ($query) {

if (!is_array($query) || !isset($query['numcol'])) return 0;
return (int)$query['numcol'];

} # fine function numcampi_query



function nomecampo_query ($query,$num) {

if (!is_array($query) || !isset($query['col'][$num])) return "";
return $query['col'][$num];

} # fine function nomecampo_query



function tipocampo_query ($query,$num) {

$risul = "unknown";
return $risul;

} # fine function tipocampo_query



function dimcampo_query ($query,$num) {

$risul = "unknown";
return $risul;

} # fine function dimcampo_query



function chiudi_query (&$query) {

$query = array();

} # fine function chiudi_query



function lock_tabelle ($tabelle,$altre_tab_usate = "") {
global $numconnessione;

$risul = $numconnessione->exec("begin transaction");
return $risul;

} # fine function lock_tabelle



function unlock_tabelle (&$tabelle_lock,$azione = "") {
global $numconnessione;

$numconnessione->exec("commit transaction");
$tabelle_lock = null;

} # fine function unlock_tabelle



function crea_indice ($tabella,$colonne,$nome) {
global $numconnessione;

$numconnessione->exec("create index if not exists $nome on $tabella ($colonne)");

} # fine function crea_indice

} // fine guard for connetti_db

?>
