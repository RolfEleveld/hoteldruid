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

#Funzioni per usare il database MYSQL - Modernized for PHP 8.1 compatibility

ignore_user_abort(1);

# variabili per le differenze nella sintassi delle query
#global $ILIKE,$LIKE;
$ILIKE = "LIKE";
$LIKE = "LIKE BINARY";
$DATETIME = "datetime";
$MEDIUMTEXT = "mediumtext";

# Global connection variable for MySQLi
global $hoteldruid_mysqli_connection;
$hoteldruid_mysqli_connection = null;

function connetti_db ($database,$host,$port,$user,$password,$estensione) {
global $hoteldruid_mysqli_connection;

// MySQLi connection with error handling
$hoteldruid_mysqli_connection = new mysqli($host, $user, $password, $database, $port);

// Check connection
if ($hoteldruid_mysqli_connection->connect_error) {
    die("Connection failed: " . $hoteldruid_mysqli_connection->connect_error);
}

// Set charset to UTF-8
$hoteldruid_mysqli_connection->set_charset("utf8");

return $hoteldruid_mysqli_connection;

} # fine function connetti_db



function disconnetti_db ($numconnessione) {
global $hoteldruid_mysqli_connection;

if ($hoteldruid_mysqli_connection) {
    $risul = $hoteldruid_mysqli_connection->close();
    $hoteldruid_mysqli_connection = null;
    return $risul;
}
return true;

} # fine function disconnetti_db



if (substr($PHPR_LOG,0,2) != "SI") {

function esegui_query ($query,$silenzio = "",$idlog = "") {
global $hoteldruid_mysqli_connection;

$risul = $hoteldruid_mysqli_connection->query($query);
if (!$risul and !$silenzio) {
global $PHPR_TAB_PRE;
echo "<br>ERROR IN: ".str_replace(" ".$PHPR_TAB_PRE," ",$query)." <br>".$hoteldruid_mysqli_connection->errno.": ".$hoteldruid_mysqli_connection->error."<br>";
} # fine if (!$risul and !$silenzio)

return $risul;

} # fine function esegui_query

} # fine if (substr($PHPR_LOG,0,2) != "SI")


else {
if (!function_exists("inserisci_log")) include("./includes/funzioni_log.php");

function esegui_query ($query,$silenzio = "",$idlog = "") {
global $hoteldruid_mysqli_connection;

$risul = $hoteldruid_mysqli_connection->query($query);
if (!$risul and !$silenzio) {
global $PHPR_TAB_PRE;
echo "<br>ERROR IN: ".str_replace(" ".$PHPR_TAB_PRE," ",$query)." <br>".$hoteldruid_mysqli_connection->errno.": ".$hoteldruid_mysqli_connection->error."<br>";
} # fine if (!$risul and !$silenzio)

if ($idlog != 1) inserisci_log($query,$idlog);

return $risul;

} # fine function esegui_query

} # fine else if (substr($PHPR_LOG,0,2) != "SI")



function risul_query ($query,$riga,$colonna,$tab="") {

// Handle MySQLi result object
if ($query instanceof mysqli_result) {
    $query->data_seek($riga);
    $row = $query->fetch_row();
    $risul = $row ? $row[$colonna] : null;
} else {
    $risul = null;
}
#if (!$risul) echo "<br>Nessun risultato in riga $riga colonna $colonna<br>";

return $risul;

} # fine function risul_query



function numlin_query ($query) {

if ($query instanceof mysqli_result) {
    $risul = $query->num_rows;
} else {
    $risul = 0;
}
return $risul;

} # fine function numlin_query



function aggslashdb ($stringa) {

$risul = addslashes($stringa);
return $risul;

} # fine function aggslashdb



function arraylin_query ($query,$num) {

if ($query instanceof mysqli_result) {
    $query->data_seek($num);
    $risul = $query->fetch_row();
} else {
    $risul = array();
}
return $risul;

} # fine function arraylin_query



function numcampi_query ($query) {

if ($query instanceof mysqli_result) {
    $risul = $query->field_count;
} else {
    $risul = 0;
}
return $risul;

} # fine function numcampi_query



function nomecampo_query ($query,$num) {

if ($query instanceof mysqli_result) {
    $fields = $query->fetch_fields();
    $risul = isset($fields[$num]) ? $fields[$num]->name : "";
} else {
    $risul = "";
}
return $risul;

} # fine function nomecampo_query



function tipocampo_query ($query,$num) {

if ($query instanceof mysqli_result) {
    $fields = $query->fetch_fields();
    $risul = isset($fields[$num]) ? $fields[$num]->type : "";
} else {
    $risul = "";
}
return $risul;

} # fine function tipocampo_query



function dimcampo_query ($query,$num) {

if ($query instanceof mysqli_result) {
    $fields = $query->fetch_fields();
    $risul = isset($fields[$num]) ? $fields[$num]->length : 0;
} else {
    $risul = 0;
}
return $risul;

} # fine function dimcampo_query



function chiudi_query (&$query) {

if ($query instanceof mysqli_result) {
    $query->free();
}
$query = "";

} # fine function chiudi_query



function lock_tabelle ($tabelle,$altre_tab_usate = "") {
global $hoteldruid_mysqli_connection;

$lista_tabelle = "";
if (@is_array($tabelle)) {
for ($num1 = 0 ; $num1 < count($tabelle); $num1++) {
$lista_tabelle .= $tabelle[$num1]." write,";
} # fine for $num1
} # fine if (@is_array($tabelle))
if (@is_array($altre_tab_usate)) {
for ($num1 = 0 ; $num1 < count($altre_tab_usate); $num1++) {
$lista_tabelle .= $altre_tab_usate[$num1]." read,";
} # fine for $num1
} # fine if (@is_array($altre_tab_usate))
$lista_tabelle = substr($lista_tabelle,0,-1);
$risul = $hoteldruid_mysqli_connection->query("lock tables $lista_tabelle");
if (!$risul) echo "<br>ERROR IN: lock tables $lista_tabelle<br>".$hoteldruid_mysqli_connection->errno.": ".$hoteldruid_mysqli_connection->error."<br>";

return $risul;

} # fine function lock_tabelle



function unlock_tabelle (&$tabelle_lock,$azione = "") {
global $hoteldruid_mysqli_connection;

$risul = $hoteldruid_mysqli_connection->query("unlock tables");
$tabelle_lock = null;

} # fine function unlock_tabelle



function crea_indice ($tabella,$colonne,$nome) {
global $hoteldruid_mysqli_connection;

$hoteldruid_mysqli_connection->query("alter table $tabella add index $nome ($colonne)");

} # fine function crea_indice



?>