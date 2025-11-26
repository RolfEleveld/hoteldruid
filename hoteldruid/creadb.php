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



$pag = "creadb.php";
$titolo = "HotelDruid: Crea Database";

$var_pag = array();
$var_pag[0] = 'tipo_db';
$var_pag[1] = 'database_phprdb';
$var_pag[2] = 'database_esistente';
$var_pag[3] = 'host_phprdb';
$var_pag[4] = 'port_phprdb';
$var_pag[5] = 'user_phprdb';
$var_pag[6] = 'password_phprdb';
$var_pag[7] = 'tempdatabase';
$var_pag[8] = 'prefisso_tab';
$var_pag[9] = 'nomeappartamenti';
$var_pag[10] = 'numappartamenti';
$var_pag[11] = 'numletti';
$var_pag[12] = 'creabase';
$var_pag[13] = 'lingua';
$var_pag[14] = 'insappartamenti';
$var_pag[15] = 'lista_camere_letti';
$n_var_pag = 16;
$num2 = 0;
if (isset($_POST['numappartamenti'])) $num2 = (int) $_POST['numappartamenti'];
elseif (isset($_GET['numappartamenti'])) $num2 = (int) $_GET['numappartamenti'];
$num3 = 0;
if (isset($_POST['numletti'])) $num3 = (int) $_POST['numletti'];
elseif (isset($_GET['numletti'])) $num3 = (int) $_GET['numletti'];
$num2 += $num3;
for ($num1 = 1 ; $num1 <= $num2 ; $num1++) {
$var_pag[$n_var_pag++] = "numapp$num1";
$var_pag[$n_var_pag++] = "maxoccupanti$num1";
$var_pag[$n_var_pag++] = "piano$num1";
$var_pag[$n_var_pag++] = "numcasa$num1";
$var_pag[$n_var_pag++] = "priorita$num1";
} # fine for $num1

include("./costanti.php");
include_once("./includes/funzioni.php");
if (!defined('C_CREADB_TIPODB')) include("./includes/costanti.php");
if (function_exists('ini_set')) @ini_set('opcache.enable',0);

if (!defined('C_CREA_ULTIMO_ACCESSO') or C_CREA_ULTIMO_ACCESSO != "SI" or !@is_file(C_DATI_PATH."/ultimo_accesso")) {

unset($numconnessione);
unset($PHPR_TAB_PRE);
unset($id_sessione);
$nome_utente_phpr = "";
$password_phpr = "";
$id_utente = controlla_login($numconnessione,$PHPR_TAB_PRE,$id_sessione,$nome_utente_phpr,$password_phpr,$anno);
if ($id_utente and $id_utente == 1) {


function mex2 ($messaggio,$pagina,$lingua) {

if (!$lingua) $lingua = "en";
if ($lingua != "ita" and @is_file("./includes/lang/$lingua/$pagina")) {
include("./includes/lang/$lingua/$pagina");
} # fine if ($lingua != "ita" and @is_file("./includes/lang/$lingua/$pagina"))
elseif ($pagina == "unit.php") include("./includes/unit.php");

return $messaggio;

} # fine function mex2


$show_bar = "NO";
$titolo = "HotelDruid: ".mex2("Crea Database",$pag,fixset($lingua));
if ($tema[$id_utente] and $tema[$id_utente] != "base" and @is_dir("./themes/".$tema[$id_utente]."/php")) include("./themes/".$tema[$id_utente]."/php/head.php");
else include("./includes/head.php");

// Check if we were redirected here due to missing database
if (isset($_GET['error']) && $_GET['error'] == 'database_not_found') {
    echo "<div class=\"rbox\" style=\"--rbox-color: #FFC107;\">
        <div class=\"rheader\">⚠️ ".mex2("Database non trovato",'creadb.php',$lingua)."</div>
        <div class=\"rcontent\" style=\"text-align: center;\">
            ".mex2("Il database non esiste o non è accessibile",'creadb.php',$lingua)."<br>
            ".mex2("Utilizzare il modulo sottostante per creare un nuovo database",'creadb.php',$lingua).".
        </div>
    </div>";
}


if (defined('C_UTILIZZA_SEMPRE_DEFAULTS') and C_UTILIZZA_SEMPRE_DEFAULTS == "AUTO") $creabase = 1;




if (!empty($creabase) and !@is_file(C_DATI_PATH."/dati_connessione.php")) {
$mostra_form_iniziale = "NO";
$insappartamenti = "";
$permessi_scrittura_controllati = "";
$torna_indietro = "";

allunga_tempo_limite();

// Set defaults for Docker environment first
if (!isset($database_phprdb) or !$database_phprdb) $database_phprdb = getenv('DB_NAME') ?: "hoteldruid";
if (!isset($host_phprdb) or !$host_phprdb) $host_phprdb = getenv('DB_HOST') ?: "hoteldruid-db";
if (!isset($port_phprdb) or !$port_phprdb) $port_phprdb = getenv('DB_PORT') ?: "3306";
if (!isset($user_phprdb) or !$user_phprdb) $user_phprdb = getenv('DB_USER') ?: "hoteldruid_user";
if (!isset($password_phprdb) or !$password_phprdb) $password_phprdb = getenv('DB_PASSWORD') ?: "hoteldruid_pass_2024";
if (!isset($database_esistente)) $database_esistente = "SI";

if (!isset($tipo_db)) $tipo_db = "";
// Force MySQLi for all MySQL connections - remove deprecated mysql support
if ($tipo_db == "mysql" or !$tipo_db) $tipo_db = "mysqli";
$carica_estensione = "NO";
// Only check for MySQLi and PostgreSQL - mysql deprecated
if (($tipo_db == "postgresql" and !@function_exists('pg_connect')) or ($tipo_db == "mysqli" and !@function_exists('mysqli_connect'))) $carica_estensione = "SI";
if ($tipo_db == "sqlite") {
if (!@class_exists('SQLite3')) $carica_estensione = "SI";
} # fine if ($tipo_db == "sqlite")
if (defined('C_UTILIZZA_SEMPRE_DEFAULTS') and (C_UTILIZZA_SEMPRE_DEFAULTS == "SI" or C_UTILIZZA_SEMPRE_DEFAULTS == "AUTO")) {
$tipo_db = C_CREADB_TIPODB;
// Force MySQLi for all MySQL connections
if ($tipo_db == "mysql" or !$tipo_db) $tipo_db = "mysqli";
$database_phprdb = C_CREADB_NOMEDB;
$database_esistente = C_CREADB_DB_ESISTENTE;
$host_phprdb = C_CREADB_HOST;
$port_phprdb = C_CREADB_PORT;
$user_phprdb = C_CREADB_USER;
if (!$password_phprdb) $password_phprdb = C_CREADB_PASS;
if (C_CREADB_ESTENSIONE) $carica_estensione = C_CREADB_ESTENSIONE;
$tempdatabase = C_CREADB_TEMPDB;
$prefisso_tab = C_CREADB_PREFISSO_TAB;
} # fine if (defined('C_UTILIZZA_SEMPRE_DEFAULTS') and (C_UTILIZZA_SEMPRE_DEFAULTS == "SI" or C_UTILIZZA_SEMPRE_DEFAULTS == "AUTO"))

// Ensure Docker environment variables override constants if available
if (getenv('DB_HOST') && (!isset($host_phprdb) || $host_phprdb == 'localhost')) {
    $host_phprdb = getenv('DB_HOST');
}
if (getenv('DB_NAME') && (!isset($database_phprdb) || !$database_phprdb)) {
    $database_phprdb = getenv('DB_NAME');
}
if (getenv('DB_USER') && (!isset($user_phprdb) || !$user_phprdb)) {
    $user_phprdb = getenv('DB_USER');
}
if (getenv('DB_PASSWORD') && (!isset($password_phprdb) || !$password_phprdb)) {
    $password_phprdb = getenv('DB_PASSWORD');
}
if (getenv('DB_PORT') && (!isset($port_phprdb) || !$port_phprdb)) {
    $port_phprdb = getenv('DB_PORT');
}

if (!controlla_num_pos($numletti) == "NO") $numletti = 0;
$numapp_default = 0;
if ((!$numappartamenti and !$numletti) or controlla_num_pos($numappartamenti) == "NO") {
$numappartamenti = 5;
$numapp_default = 1;
} # fine if ((!$numappartamenti and !$numletti) or controlla_num_pos($numappartamenti) == "NO")

if (defined('C_EXT_DB_DATA_PATH') and C_EXT_DB_DATA_PATH) {
$HOTELD_DB_TYPE = "";
$HOTELD_DB_NAME = "";
$HOTELD_DB_HOST = "";
$HOTELD_DB_PORT = "";
$HOTELD_DB_USER = "";
$HOTELD_DB_PASS = "";
$HOTELD_TAB_PRE = "";
include(C_EXT_DB_DATA_PATH);
if ($HOTELD_DB_TYPE) {
$tipo_db = $HOTELD_DB_TYPE;
if ($tipo_db == "mysql" and @function_exists('mysqli_connect')) $tipo_db = "mysqli";
} # fine if ($HOTELD_DB_TYPE)
if ($HOTELD_DB_NAME) $database_phprdb = $HOTELD_DB_NAME;
if ($HOTELD_DB_HOST) $host_phprdb = $HOTELD_DB_HOST;
if (strcmp((string) $HOTELD_DB_PORT,"")) $port_phprdb = $HOTELD_DB_PORT;
if ($HOTELD_DB_USER) $user_phprdb = $HOTELD_DB_USER;
if (strcmp((string) $HOTELD_DB_PASS,"")) $password_phprdb = $HOTELD_DB_PASS;
if ($HOTELD_TAB_PRE) $prefisso_tab = $HOTELD_TAB_PRE;
} # fine if (defined('C_EXT_DB_DATA_PATH') and C_EXT_DB_DATA_PATH)

if (!$prefisso_tab or preg_match('/^[_a-z][_0-9a-z]*$/',$prefisso_tab)) {
if ($tipo_db == "postgresql") {
if ($carica_estensione == "SI") dl("pgsql.so");
if ($database_esistente == "SI") $tempdatabase = $database_phprdb;
$numconnessione = pg_connect("dbname=$tempdatabase host=$host_phprdb port=$port_phprdb user=$user_phprdb password=$password_phprdb ");
$encoding = " with encoding = 'SQL_ASCII'";
$encoding = "";
} # fine if ($tipo_db == "postgresql")
// MySQL (deprecated) section removed - use MySQLi only
if ($tipo_db == "mysqli") {
if ($carica_estensione == "SI") dl("mysqli.so");
$numconnessione = mysqli_connect($host_phprdb,$user_phprdb,$password_phprdb,"",$port_phprdb);
@mysqli_query($numconnessione,"SET NAMES 'utf8mb4'");
$risul = @mysqli_query($numconnessione,"SET default_storage_engine=MYISAM");
if (!$risul) {
sleep(1);
@mysqli_query($numconnessione,"SET default_storage_engine=MYISAM");
} # fine if (!$risul)
if ($numconnessione and $database_esistente == "SI") {
$query_db = mysqli_select_db($numconnessione,$database_phprdb);
if (!$query_db) $numconnessione = $query_db;
} # fine if ($numconnessione and $database_esistente == "SI")
$encoding = "";
} # fine if ($tipo_db == "mysqli")
if ($tipo_db == "sqlite") {
if ($carica_estensione == "SI") dl("sqlite.so");
$database_phprdb = str_replace("..","",$database_phprdb);
// Ensure the dati directory exists before creating SQLite database
if (!is_dir(C_DATI_PATH)) {
    if (!@mkdir(C_DATI_PATH, 0755, true)) {
        echo "<div class=\"rbox\" style=\"--rbox-color: #F44336;\">
            <div class=\"rheader\">❌ ".mex2("Errore",$pag,$lingua)."</div>
            <div class=\"rcontent\">".mex2("Non è stato possibile creare la directory dati",$pag,$lingua).": ".htmlspecialchars(C_DATI_PATH)."</div>
        </div>";
        $torna_indietro = "SI";
    }
}
$db_file_path = C_DATI_PATH."/db_".$database_phprdb;
$numconnessione = new SQLite3($db_file_path);
$numconnessione->busyTimeout(60000);
$database_esistente = "SI";
} # fine if ($tipo_db == "sqlite")

if ($numconnessione) {
$PHPR_LOG = "";
include("./includes/funzioni_$tipo_db.php");
if ($database_esistente == "NO") {
$link_mysqli = $numconnessione;
// Use CREATE DATABASE IF NOT EXISTS to avoid errors if database already exists
$query = esegui_query("create database if not exists $database_phprdb $encoding");
if ($query) echo mex2("Database creato",$pag,$lingua)."!<br>";
} # fine if ($database_esistente == "NO")
else $query = "SI";
disconnetti_db($numconnessione);

if ($query) {
$character_set_db_orig = "";
$collation_db_orig = "";
$character_set_db = "";
$collation_db = "";
if ($tipo_db == "postgresql") {
$numconnessione = pg_connect("dbname=$database_phprdb host=$host_phprdb port=$port_phprdb user=$user_phprdb password=$password_phprdb ");
} # fine if ($tipo_db == "postgresql")
if ($tipo_db == "mysql" or $tipo_db == "mysqli") {
// Use MySQLi only - mysql deprecated
if ($tipo_db == "mysqli") {
$numconnessione = mysqli_connect($host_phprdb,$user_phprdb,$password_phprdb,$database_phprdb,$port_phprdb);
$link_mysqli = $numconnessione;
@mysqli_query($numconnessione,"SET NAMES 'utf8mb4'");
$risul = @mysqli_query($numconnessione,"SET default_storage_engine=MYISAM");
if (!$risul) {
sleep(1);
@mysqli_query($numconnessione,"SET default_storage_engine=MYISAM");
} # fine if (!$risul)
$character_set_db = "utf8mb4";
$collation_db = "utf8mb4_unicode_520_ci";
} # fine if ($tipo_db == "mysqli")
$character_set = esegui_query("SHOW VARIABLES LIKE 'character_set_database'");
$collation = esegui_query("SHOW VARIABLES LIKE 'collation_database'");
if (numlin_query($character_set) == 1 and numlin_query($collation) == 1) {
$character_set_db_orig = risul_query($character_set,0,"Value");
$collation_db_orig = risul_query($collation,0,"Value");
if ($character_set_db != $character_set_db_orig or $collation_db != $collation_db_orig) {
$disp_err_orig = ini_get('display_errors');
if ($disp_err_orig) ini_set('display_errors','0');
esegui_query("alter database $database_phprdb default character set '$character_set_db' collate '$collation_db'");
if ($disp_err_orig) ini_set('display_errors',$disp_err_orig);
} # fine if ($character_set_db != $character_set_db_orig or $collation_db != $collation_db_orig)
} # fine if (numlin_query($character_set) == 1 and...
} # fine if ($tipo_db == "mysql" or $tipo_db == "mysqli")
if ($tipo_db == "sqlite") {
// Ensure the dati directory exists before creating SQLite database
if (!is_dir(C_DATI_PATH)) {
    if (!@mkdir(C_DATI_PATH, 0755, true)) {
        echo "<div class=\"rbox\" style=\"--rbox-color: #F44336;\">
            <div class=\"rheader\">❌ ".mex2("Errore",$pag,$lingua)."</div>
            <div class=\"rcontent\">".mex2("Non è stato possibile creare la directory dati",$pag,$lingua).": ".htmlspecialchars(C_DATI_PATH)."</div>
        </div>";
        $torna_indietro = "SI";
    }
}
$numconnessione = new SQLite3(C_DATI_PATH."/db_".$database_phprdb);
$numconnessione->busyTimeout(60000);
} # fine if ($tipo_db == "sqlite")

# creo la tabella appartamenti.
$tableappartamenti = $prefisso_tab."appartamenti";
esegui_query("create table $tableappartamenti ( idappartamenti varchar(100) primary key, numpiano text, maxoccupanti integer, numcasa text, app_vicini text, priorita integer, priorita2 integer, letto varchar(1), commento text )");
# creo la tabella clienti.
$tableclienti = $prefisso_tab."clienti";
esegui_query("create table $tableclienti (idclienti integer primary key, cognome varchar(70) not null, nome varchar(70), soprannome varchar(70), sesso char, titolo varchar(30), lingua varchar(14), datanascita date, cittanascita varchar(70), regionenascita varchar(70), nazionenascita varchar(70), documento varchar(70), scadenzadoc date, tipodoc varchar(70), cittadoc varchar(70), regionedoc varchar(70), nazionedoc  varchar(70), nazionalita varchar(70), nazione varchar(70), regione varchar(70), citta varchar(70), via varchar(70), numcivico varchar(30), cap varchar(30), telefono varchar(50), telefono2 varchar(50), telefono3 varchar(50), fax varchar(50), email text, email2 text, email3 text, cod_fiscale varchar(50), partita_iva varchar(50), commento text, max_num_ordine integer, idclienti_compagni text, doc_inviati text, datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
# creo la tabella di relazione tra clienti e dati vari.
$tablerelclienti = $prefisso_tab."relclienti";
esegui_query("create table $tablerelclienti (idclienti integer, numero integer, tipo varchar(12), testo1 text, testo2 text, testo3 text, testo4 text, testo5 text, testo6 text, testo7 text, testo8 text, datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
crea_indice($tablerelclienti,"idclienti",$prefisso_tab."iidprelclienti");
# creo la tabella anni.
$tableanni = $prefisso_tab."anni";
esegui_query("create table $tableanni (idanni integer primary key, tipo_periodi text)");
# creo la tabella versione ed inserisco quella corrente.
$tableversioni = $prefisso_tab."versioni";
esegui_query("create table $tableversioni (idversioni integer primary key, num_versione float4)");
esegui_query("insert into $tableversioni (idversioni, num_versione) values ('1', '".C_PHPR_VERSIONE_NUM."')");
esegui_query("insert into $tableversioni (idversioni, num_versione) values ('2', '100')");
# creo la tabella per la lista delle nazioni.
$tablenazioni = $prefisso_tab."nazioni";
esegui_query("create table $tablenazioni (idnazioni integer primary key, nome_nazione varchar(70), codice_nazione varchar(50), codice2_nazione varchar(50), codice3_nazione varchar(50), datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
# creo la tabella per lista delle regioni (province/stati).
$tableregioni = $prefisso_tab."regioni";
esegui_query("create table $tableregioni (idregioni integer primary key, nome_regione varchar(70), codice_regione varchar(50), codice2_regione varchar(50), codice3_regione varchar(50), datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
# creo la tabella per lista delle città.
$tablecitta = $prefisso_tab."citta";
esegui_query("create table $tablecitta (idcitta integer primary key, nome_citta varchar(70), codice_citta varchar(50), codice2_citta varchar(50), codice3_citta varchar(50), datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
# creo la tabella per lista dei documenti di identità.
$tabledocumentiid = $prefisso_tab."documentiid";
esegui_query("create table $tabledocumentiid (iddocumentiid integer primary key, nome_documentoid varchar(70), codice_documentoid varchar(50), codice2_documentoid varchar(50), codice3_documentoid varchar(50), datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
# creo la tabella per lista delle parentele.
$tableparentele = $prefisso_tab."parentele";
esegui_query("create table $tableparentele (idparentele integer primary key, nome_parentela varchar(70), codice_parentela varchar(50), codice2_parentela varchar(50), codice3_parentela varchar(50), datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
# creo la tabella per le personalizzazioni.
$tablepersonalizza = $prefisso_tab."personalizza";
esegui_query("create table $tablepersonalizza (idpersonalizza varchar(50) not null, idutente integer, valpersonalizza text, valpersonalizza_num integer)");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('col_tab_tutte_prenota','1','nu#@&cg#@&in#@&fi#@&tc#@&ca#@&pa#@&ap#@&pe#@&co')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('rig_tab_tutte_prenota','1','to#@&ta#@&ca#@&pc')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('modo_invio_email','1','locale')");
if (defined("C_MASCHERA_EMAIL") and C_MASCHERA_EMAIL == "SI") $maschera_email = "SI";
else $maschera_email = "NO";
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('maschera_email','1','$maschera_email')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('dati_struttura','1','#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&#@&')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('valuta','1','".aggslashdb(mex2("Euro",$pag,$lingua))."')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('arrotond_predef','1','1')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('arrotond_tasse','1','0.01')");
if ($lingua == "ita" or $lingua == "es" or $lingua == "fr" or $lingua == "de" or $lingua == "pt") $stile_soldi = "europa";
else $stile_soldi = "usa";
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('stile_soldi','1','$stile_soldi')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('costi_agg_in_tab_prenota','1','')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('aggiunta_tronca_nomi_tab1','1','-2')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('linee_ripeti_date_tab_mesi','1','25')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('mostra_giorni_tab_mesi','1','SI')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('colori_tab_mesi','1','#70C6D4,#FFD800,#FF9900,#FF3115')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('num_linee_tab2_prenota','1','30')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('nomi_contratti','1','')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('num_righe_tab_tutte_prenota','1','200')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('selezione_tab_tutte_prenota','1','tutte')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('num_righe_tab_tutti_clienti','1','200')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('num_righe_tab_casse','1','50')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('tot_giornalero_tab_casse','1','gior,mens,tab')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('num_righe_tab_messaggi','1','80')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('num_righe_tab_doc_salvati','1','100')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('num_righe_tab_storia_soldi','1','200')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('stile_data','1','europa')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('stile_nomi','1','ma1or')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('num_categorie_persone','1','1')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('minuti_durata_sessione','1','90')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('usa_cookies','1','0')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('minuti_durata_insprenota','1','10')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza_num) values ('ore_anticipa_periodo_corrente','1','0')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('tutti_fissi','1','10')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('auto_crea_anno','1','SI')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('metodi_pagamento','1','')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('origini_prenota','1','')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('attiva_checkin','1','NO')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('mostra_quadro_disp','1','reg2')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('ultime_sel_ins_prezzi','1','')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('subordinazione','1','NO')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('percorso_cartella_modello','1','".C_DATI_PATH."')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('gest_cvc','1','NO')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('ordine_inventario','1','alf')");
esegui_query("insert into $tablepersonalizza (idpersonalizza,idutente,valpersonalizza) values ('tasti_pos','1','x2;x10;s;+1;+2;+3;+4;+5;+6;+7;+8;+9;s;-1')");
if (defined("C_CARTELLA_CREA_MODELLI") and C_CARTELLA_CREA_MODELLI != "") {
$c_cartella_crea_mod = C_CARTELLA_CREA_MODELLI;
if (substr($c_cartella_crea_mod,-1) == "/") $c_cartella_crea_mod = substr($c_cartella_crea_mod,0,-1);
esegui_query("update $tablepersonalizza set valpersonalizza = '$c_cartella_crea_mod' where idpersonalizza = 'percorso_cartella_modello' and idutente = '1'");
} # fine if (defined("C_CARTELLA_CREA_MODELLI") and C_CARTELLA_CREA_MODELLI != "")
# creo la tabella degli utenti.
$tableutenti = $prefisso_tab."utenti";
esegui_query("create table $tableutenti (idutenti integer primary key, nome_utente text, password text, salt text, tipo_pass varchar(1), datainserimento $DATETIME, hostinserimento varchar(50) )");
esegui_query("insert into $tableutenti (idutenti,nome_utente,tipo_pass) values ('1','admin','n') ");
http_keep_alive();
# creo la tabella dei gruppi.
$tablegruppi = $prefisso_tab."gruppi";
esegui_query("create table $tablegruppi (idgruppi integer primary key, nome_gruppo text )");
# creo la tabella per i privilegi degli utenti.
$tableprivilegi = $prefisso_tab."privilegi";
esegui_query("create table $tableprivilegi (idutente integer, anno integer, regole1_consentite text, tariffe_consentite text, costi_agg_consentiti text, contratti_consentiti text, casse_consentite text, cassa_pagamenti varchar(70), priv_ins_prenota varchar(20), priv_mod_prenota varchar(35), priv_mod_pers varchar(15), priv_ins_clienti varchar(5), prefisso_clienti text, priv_ins_costi varchar(10), priv_vedi_tab varchar(30), priv_ins_tariffe varchar(10), priv_ins_regole varchar(10), priv_messaggi varchar(10), priv_inventario varchar(10) )");
# creo la tabella per le relazioni tra utenti e loro personalizzazioni di liste.
$tablerelutenti = $prefisso_tab."relutenti";
esegui_query("create table $tablerelutenti (idutente integer not null, idnazione integer, idregione integer, idcitta integer, iddocumentoid integer, idparentela integer, idsup integer, predef integer, datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
crea_indice($tablerelutenti,"idutente",$prefisso_tab."iidprelutenti");
$tablerelgruppi = $prefisso_tab."relgruppi";
esegui_query("create table $tablerelgruppi (idutente integer not null, idgruppo integer, datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
crea_indice($tablerelgruppi,"idutente",$prefisso_tab."iidprelgruppi");
$tablesessioni = $prefisso_tab."sessioni";
esegui_query("create table $tablesessioni (idsessioni varchar(42) primary key, idcliente varchar(42), idutente integer, indirizzo_ip text, tipo_conn varchar(12), user_agent text, ultimo_accesso $DATETIME)");
$tabletransazioni = $prefisso_tab."transazioni";
esegui_query("create table $tabletransazioni (idtransazioni varchar(42) primary key, idcliente varchar(42), idsessione varchar(42), tipo_transazione varchar(5), anno integer, spostamenti text, dati_transazione1 text, dati_transazione2 text, dati_transazione3 text, dati_transazione4 text, dati_transazione5 text, dati_transazione6 text, dati_transazione7 text, dati_transazione8 text, dati_transazione9 text, dati_transazione10 text, dati_transazione11 text, dati_transazione12 text, dati_transazione13 text, dati_transazione14 text, dati_transazione15 text, dati_transazione16 text, dati_transazione17 text, dati_transazione18 text, dati_transazione19 text, dati_transazione20 text, dati_transazione21 text, dati_transazione22 text, ultimo_accesso $DATETIME)");
$tabletransazioniweb = $prefisso_tab."transazioniweb";
esegui_query("create table $tabletransazioniweb (idtransazioni varchar(42) primary key, idcliente varchar(42), idsessione varchar(42), tipo_transazione varchar(5), anno integer, spostamenti text, dati_transazione1 text, dati_transazione2 text, dati_transazione3 text, dati_transazione4 text, dati_transazione5 text, dati_transazione6 text, dati_transazione7 text, dati_transazione8 text, dati_transazione9 text, dati_transazione10 text, dati_transazione11 text, dati_transazione12 text, dati_transazione13 text, dati_transazione14 text, dati_transazione15 text, dati_transazione16 text, dati_transazione17 text, dati_transazione18 text, dati_transazione19 text, dati_transazione20 text, dati_transazione21 text, dati_transazione22 text, ultimo_accesso $DATETIME)");
esegui_query("insert into  $tabletransazioniweb (idtransazioni, anno) values ('2', '100')");
$tablemessaggi = $prefisso_tab."messaggi";
esegui_query("create table $tablemessaggi (idmessaggi integer primary key, tipo_messaggio varchar(8), stato varchar(8), idutenti text, idutenti_visto text, datavisione $DATETIME, mittente text, testo text, dati_messaggio1 text, dati_messaggio2 text, dati_messaggio3 text, dati_messaggio4 text, dati_messaggio5 text, dati_messaggio6 text, dati_messaggio7 text, dati_messaggio8 text, dati_messaggio9 text, dati_messaggio10 text, dati_messaggio11 text, dati_messaggio12 text, dati_messaggio13 text, dati_messaggio14 text, dati_messaggio15 text, dati_messaggio16 text, dati_messaggio17 text, dati_messaggio18 text, dati_messaggio19 text, dati_messaggio20 text, dati_messaggio21 text, dati_messaggio22 text, datainserimento $DATETIME )");
$tabledescrizioni = $prefisso_tab."descrizioni";
esegui_query("create table $tabledescrizioni (nome text not null, tipo varchar(16), lingua varchar(3), numero integer, testo $MEDIUMTEXT )");
$tablebeniinventario = $prefisso_tab."beniinventario";
esegui_query("create table $tablebeniinventario (idbeniinventario integer primary key, nome_bene varchar(70), codice_bene varchar(50), descrizione_bene text, datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
$tablemagazzini = $prefisso_tab."magazzini";
esegui_query("create table $tablemagazzini (idmagazzini integer primary key, nome_magazzino varchar(70), codice_magazzino varchar(50), descrizione_magazzino text, numpiano text, numcasa text, datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
$tablerelinventario = $prefisso_tab."relinventario";
esegui_query("create table $tablerelinventario (idbeneinventario integer not null, idappartamento varchar(100), idmagazzino integer, quantita integer, quantita_min_predef integer, richiesto_checkin varchar(2), datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");
crea_indice($tablerelinventario,"idbeneinventario",$prefisso_tab."iidprelinventario");
# Creo la tabella con le casse
$tablecasse = $prefisso_tab."casse";
esegui_query("create table $tablecasse (idcasse integer primary key, nome_cassa varchar(70), stato varchar(8), codice_cassa varchar(50), descrizione_cassa text, datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer) ");
esegui_query("insert into $tablecasse (idcasse,datainserimento,hostinserimento,utente_inserimento) values ('1','".date("Y-m-d H:i:s",(time() + (C_DIFF_ORE * 3600)))."','$HOSTNAME','1')");
# Creo la tabella con i dati dei documenti
$tablecontratti = $prefisso_tab."contratti";
esegui_query("create table $tablecontratti (numero integer, tipo varchar(8), testo $MEDIUMTEXT )");
# Creo la tabella con la cache per interconnessioni, ecc.
$tablecache = $prefisso_tab."cache";
esegui_query("create table $tablecache (numero integer, tipo varchar(8), testo $MEDIUMTEXT, data_modifica $DATETIME, datainserimento $DATETIME )");
# Creo la tabella con i dati delle interconnessioni
$tableinterconnessioni = $prefisso_tab."interconnessioni";
esegui_query("create table $tableinterconnessioni (idlocale integer, idremoto1 text, idremoto2 text, tipoid varchar(12), nome_ic varchar(24), anno integer, datainserimento $DATETIME, hostinserimento varchar(50), utente_inserimento integer )");

include("./includes/funzioni_backup.php");
if (defined("C_CARTELLA_FILES_REALI")) $f_pre = C_CARTELLA_FILES_REALI;
else $f_pre = "";
if ($lingua == "ita") $file_contr_backup = $f_pre."./includes/hoteld_doc_backup.php";
else {
if (@is_file($f_pre."./includes/lang/$lingua/hoteld_doc_backup.php")) $file_contr_backup = $f_pre."./includes/lang/$lingua/hoteld_doc_backup.php";
else {
if (@is_file($f_pre."./includes/lang/en/hoteld_doc_backup.php")) $file_contr_backup = $f_pre."./includes/lang/en/hoteld_doc_backup.php";
else $file_contr_backup = $f_pre."./includes/hoteld_doc_backup.php";
} # fine else if (@is_file($f_pre."./includes/lang/$lingua/hoteld_doc_backup.php"))
} # fine else if ($lingua == "ita")
if ($linee_backup = @file($file_contr_backup)) {
ripristina_backup_contr($linee_backup,"SI","crea_backup.php",$prefisso_tab,"rimpiazza");
} # fine if ($linee_backup = @file($file_contr_backup))


# creo i file permanenti.
if ($fileaperto = fopen(C_DATI_PATH."/dati_connessione.php","a+")) {
if (defined('C_EXT_DB_DATA_PATH') and C_EXT_DB_DATA_PATH) {
if ($HOTELD_DB_TYPE) $tipo_db = "";
if ($HOTELD_DB_NAME) $database_phprdb = "";
if ($HOTELD_DB_HOST) $host_phprdb = "";
if (strcmp((string) $HOTELD_DB_PORT,"")) $port_phprdb = "";
if ($HOTELD_DB_USER) $user_phprdb = "";
if (strcmp((string) $HOTELD_DB_PASS,"")) $password_phprdb = "";
if ($HOTELD_TAB_PRE) $prefisso_tab = "";
} # fine if (defined('C_EXT_DB_DATA_PATH') and C_EXT_DB_DATA_PATH)
$database_scrivi = aggiungi_slash($database_phprdb);
$host_scrivi = aggiungi_slash($host_phprdb);
$user_scrivi = aggiungi_slash($user_phprdb);
$password_scrivi = aggiungi_slash($password_phprdb);
fwrite($fileaperto,"<?php
\$PHPR_DB_TYPE = \"$tipo_db\";
\$PHPR_DB_NAME = \"$database_scrivi\";
\$PHPR_DB_HOST = \"$host_scrivi\";
\$PHPR_DB_PORT = \"$port_phprdb\";
\$PHPR_DB_USER = \"$user_scrivi\";
\$PHPR_DB_PASS = \"$password_scrivi\";
\$PHPR_LOAD_EXT = \"$carica_estensione\";
\$PHPR_TAB_PRE = \"$prefisso_tab\";
\$PHPR_LOG = \"NO\";
");
if (defined('C_EXT_DB_DATA_PATH') and C_EXT_DB_DATA_PATH) fwrite($fileaperto,"
\$HOTELD_DB_TYPE = \"\";
\$HOTELD_DB_NAME = \"\";
\$HOTELD_DB_HOST = \"\";
\$HOTELD_DB_PORT = \"\";
\$HOTELD_DB_USER = \"\";
\$HOTELD_DB_PASS = \"\";
\$HOTELD_TAB_PRE = \"\";
require('".C_EXT_DB_DATA_PATH."');
if (\$HOTELD_DB_TYPE) {
\$PHPR_DB_TYPE = \$HOTELD_DB_TYPE;
if (\$PHPR_DB_TYPE == \"mysql\" and @function_exists('mysqli_connect')) \$PHPR_DB_TYPE = \"mysqli\";
}
if (\$HOTELD_DB_NAME) \$PHPR_DB_NAME = \$HOTELD_DB_NAME;
if (\$HOTELD_DB_HOST) \$PHPR_DB_HOST = \$HOTELD_DB_HOST;
if (strcmp(\$HOTELD_DB_PORT,\"\")) \$PHPR_DB_PORT = \$HOTELD_DB_PORT;
if (\$HOTELD_DB_USER) \$PHPR_DB_USER = \$HOTELD_DB_USER;
if (strcmp(\$HOTELD_DB_PASS,\"\")) \$PHPR_DB_PASS = \$HOTELD_DB_PASS;
if (\$HOTELD_TAB_PRE) \$PHPR_TAB_PRE = \$HOTELD_TAB_PRE;
");
fwrite($fileaperto,"?>");
fclose($fileaperto);
@chmod(C_DATI_PATH."/dati_connessione.php", 0640);
if ($lingua != "ita" and (!@is_dir("./includes/lang/".$lingua) or strlen($lingua) > 3)) $lingua = "en";
$fileaperto = fopen(C_DATI_PATH."/lingua.php","w+");
fwrite($fileaperto,"<?php
\$lingua[1] = \"$lingua\";
?>");
fclose($fileaperto);
if (!isset($nomeappartamenti) or $nomeappartamenti != "appartamenti") $nomeappartamenti = "camere";
$fileaperto = fopen(C_DATI_PATH."/unit.php","w+");
fwrite($fileaperto,"<?php
");
if ($nomeappartamenti == "appartamenti") {
fwrite($fileaperto,"\$unit['s_n'] = \$trad_var['apartment'];
\$unit['p_n'] = \$trad_var['apartments'];
\$unit['gender'] = \$trad_var['apartment_gender'];
");
} # fine if ($nomeappartamenti == "appartamenti")
else {
fwrite($fileaperto,"\$unit['s_n'] = \$trad_var['room'];
\$unit['p_n'] = \$trad_var['rooms'];
\$unit['gender'] = \$trad_var['room_gender'];
");
} # fine else if ($nomeappartamenti == "appartamenti")
fwrite($fileaperto,"\$unit['special'] = 0;
\$car_spec = explode(\",\",\$trad_var['special_characters']);
for (\$num1 = 0 ; \$num1 < count(\$car_spec) ; \$num1++) if (substr(\$unit['p_n'],0,strlen(\$car_spec[\$num1])) == \$car_spec[\$num1]) \$unit['special'] = 1;
?>");
fclose($fileaperto);
$fileaperto = fopen(C_DATI_PATH."/unit_single.php","w+");
fwrite($fileaperto,"<?php
\$unit['s_n'] = \$trad_var['bed'];
\$unit['p_n'] = \$trad_var['beds'];
\$unit['gender'] = \$trad_var['bed_gender'];
\$unit['special'] = 0;
\$car_spec = explode(\",\",\$trad_var['special_characters']);
for (\$num1 = 0 ; \$num1 < count(\$car_spec) ; \$num1++) if (substr(\$unit['p_n'],0,strlen(\$car_spec[\$num1])) == \$car_spec[\$num1]) \$unit['special'] = 1;
?>");
fclose($fileaperto);
$fileaperto = fopen(C_DATI_PATH."/tema.php","w+");
fwrite($fileaperto,"<?php
\$parole_sost = 0;
\$tema[1] = \"blu\";
?>");
fclose($fileaperto);
$fileaperto = fopen(C_DATI_PATH."/versione.php","w+");
fwrite($fileaperto,"<?php
define('C_VERSIONE_ATTUALE',".C_PHPR_VERSIONE_NUM.");
define('C_DIFF_ORE',0);
define('C_MIN_SESSIONE',90);
define('C_USA_COOKIES',0);
?>");
fclose($fileaperto);

include("./includes/funzioni_relutenti.php");
aggiorna_relutenti("","SI","","",$id_utente,$id_utente,"","","","","","","","nazione","nazioni",$tablenazioni,$tablerelutenti);
aggiorna_relutenti("","SI","","",$id_utente,$id_utente,"","","","","","","","regione","regioni",$tableregioni,$tablerelutenti,"nazione","nazioni",$tablenazioni);
if (defined('C_CREADB_CITTA_DEFAULT') and C_CREADB_CITTA_DEFAULT == "SI") aggiorna_relutenti("","SI","","",$id_utente,$id_utente,"","","","","","","","citta","citta",$tablecitta,$tablerelutenti,"regione","regioni",$tableregioni);
aggiorna_relutenti("","SI","","",$id_utente,$id_utente,"","","","","","","","documentoid","documentiid",$tabledocumentiid,$tablerelutenti);
aggiorna_relutenti("","SI","","",$id_utente,$id_utente,"","","","","","","","parentela","parentele",$tableparentele,$tablerelutenti);

if (defined('C_NASCONDI_MARCA') and C_NASCONDI_MARCA == "SI" and defined('C_CARTELLA_CREA_MODELLI') and @is_file(C_CARTELLA_CREA_MODELLI."/index.html")) @unlink(C_CARTELLA_CREA_MODELLI."/index.html");

if (!defined('C_UTILIZZA_SEMPRE_DEFAULTS') or C_UTILIZZA_SEMPRE_DEFAULTS != "AUTO") {
# seconda form di inserimento (appartamenti) - Enhanced Dynamic Interface
echo "<div class=\"rbox\"><div class=\"rheader\">".mex2("Inserisci ora i dati sugli appartamenti",'unit.php',$lingua)." (<b>".mex2("almeno il numero, diverso per ogni appartamento",'unit.php',$lingua)."</b>).</div><br>

<style>
.room-container {
    border: 1px solid #ccc;
    margin: 10px 0;
    padding: 15px;
    border-radius: 5px;
    background-color: #f9f9f9;
    position: relative;
}
.room-header {
    font-weight: bold;
    margin-bottom: 10px;
    color: #333;
}
.room-fields {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 10px;
    margin-bottom: 10px;
}
.field-group {
    display: flex;
    flex-direction: column;
}
.field-group label {
    font-size: 12px;
    color: #666;
    margin-bottom: 2px;
}
.field-group input {
    padding: 5px;
    border: 1px solid #ddd;
    border-radius: 3px;
}
.remove-room-btn {
    position: absolute;
    top: 5px;
    right: 5px;
    background: #dc3545;
    color: white;
    border: none;
    padding: 5px 10px;
    border-radius: 3px;
    cursor: pointer;
    font-size: 12px;
}
.remove-room-btn:hover {
    background: #c82333;
}
.add-room-btn {
    background: #28a745;
    color: white;
    border: none;
    padding: 10px 20px;
    border-radius: 5px;
    cursor: pointer;
    margin: 10px 0;
}
.add-room-btn:hover {
    background: #218838;
}
.submit-btn {
    background: #007bff;
    color: white;
    border: none;
    padding: 15px 30px;
    border-radius: 5px;
    cursor: pointer;
    font-size: 16px;
    margin-top: 20px;
}
.submit-btn:hover {
    background: #0056b3;
}
</style>

<form accept-charset=\"utf-8\" method=\"post\" action=\"creadb.php\"><div>
<input type=\"hidden\" name=\"numappartamenti\" id=\"numappartamenti\" value=\"$numappartamenti\">
<input type=\"hidden\" name=\"numletti\" value=\"$numletti\">

<div id=\"rooms-container\">
</div>

<div style=\"text-align: center; margin: 20px 0;\">
    <button type=\"button\" class=\"add-room-btn\" onclick=\"addRoom()\">
        ".mex2("Aggiungi stanza",'creadb.php',$lingua)."
    </button>
</div>

<hr style=\"width: 95%\">";

// JavaScript for dynamic room management
$js_habitacion = mex2("Stanza",'creadb.php',$lingua);
$js_numero_habitacion = mex2("Numero stanza",'creadb.php',$lingua);
$js_max_ocupantes = mex2("Massimo numero di occupanti",'creadb.php',$lingua);
$js_numero_piso = mex2("Numero piano",'creadb.php',$lingua);
$js_numero_casa = mex2("Numero casa",'creadb.php',$lingua);
$js_prioridad = mex2("Priorità",'creadb.php',$lingua);
$js_remover_stanza = mex2("Rimuovi stanza",'creadb.php',$lingua);

echo "<script>
let roomCount = $numappartamenti;
let currentRoomNumber = roomCount;

function initializeRooms() {
    const container = document.getElementById('rooms-container');
    container.innerHTML = '';
    
    for (let i = 1; i <= roomCount; i++) {
        addRoomToContainer(i);
    }
}

function addRoom() {
    currentRoomNumber++;
    roomCount++;
    document.getElementById('numappartamenti').value = roomCount;
    addRoomToContainer(currentRoomNumber);
}

function addRoomToContainer(roomNum) {
    const container = document.getElementById('rooms-container');
    const roomDiv = document.createElement('div');
    roomDiv.className = 'room-container';
    roomDiv.id = 'room-' + roomNum;
    
    const numDefault = String(roomNum).padStart(2, '0');
    
    roomDiv.innerHTML = '<div class=\"room-header\">$js_habitacion ' + roomNum + '</div>' +
        (roomCount > 1 ? '<button type=\"button\" class=\"remove-room-btn\" onclick=\"removeRoom(' + roomNum + ')\">$js_remover_stanza</button>' : '') +
        '<div class=\"room-fields\">' +
            '<div class=\"field-group\">' +
                '<label>$js_numero_habitacion:</label>' +
                '<input type=\"text\" name=\"numapp' + roomNum + '\" value=\"' + numDefault + '\" size=\"5\" required>' +
            '</div>' +
            '<div class=\"field-group\">' +
                '<label>$js_max_ocupantes:</label>' +
                '<input type=\"text\" name=\"maxoccupanti' + roomNum + '\" size=\"3\" placeholder=\"2\">' +
            '</div>' +
            '<div class=\"field-group\">' +
                '<label>$js_numero_piso:</label>' +
                '<input type=\"text\" name=\"piano' + roomNum + '\" size=\"4\" placeholder=\"1\">' +
            '</div>' +
            '<div class=\"field-group\">' +
                '<label>$js_numero_casa:</label>' +
                '<input type=\"text\" name=\"numcasa' + roomNum + '\" size=\"4\" placeholder=\"A\">' +
            '</div>' +
            '<div class=\"field-group\">' +
                '<label>$js_prioridad:</label>' +
                '<input type=\"text\" name=\"priorita' + roomNum + '\" size=\"3\" placeholder=\"' + roomNum + '\">' +
            '</div>' +
        '</div>';
    
    container.appendChild(roomDiv);
}

function removeRoom(roomNum) {
    if (roomCount <= 1) {
        alert('Debe mantener al menos una habitación');
        return;
    }
    
    const roomDiv = document.getElementById('room-' + roomNum);
    if (roomDiv) {
        roomDiv.remove();
        roomCount--;
        document.getElementById('numappartamenti').value = roomCount;
        updateRoomNumbers();
    }
}

function updateRoomNumbers() {
    const containers = document.querySelectorAll('.room-container');
    containers.forEach((container, index) => {
        const roomNum = index + 1;
        container.id = 'room-' + roomNum;
        
        // Update room header
        const header = container.querySelector('.room-header');
        header.textContent = '$js_habitacion ' + roomNum;
        
        // Update all input names and the remove button
        const inputs = container.querySelectorAll('input[type=\"text\"]');
        inputs.forEach(input => {
            const baseName = input.name.replace(/\\d+$/, '');
            input.name = baseName + roomNum;
        });
        
        // Update remove button
        const removeBtn = container.querySelector('.remove-room-btn');
        if (removeBtn) {
            removeBtn.setAttribute('onclick', 'removeRoom(' + roomNum + ')');
        }
        
        // Update priority placeholder
        const priorityInput = container.querySelector('input[name=\"priorita' + roomNum + '\"]');
        if (priorityInput) {
            priorityInput.placeholder = roomNum;
        }
    });
}

// Initialize rooms when page loads
document.addEventListener('DOMContentLoaded', function() {
    initializeRooms();
});
</script>";

echo "<div style=\"text-align: center;\">
<input class=\"submit-btn\" type=\"submit\" name=\"insappartamenti\" value=\"".mex2("Salva configurazione stanze",'creadb.php',$lingua)."\">
</div><br></div></form></div>";
} # fine if (!defined('C_UTILIZZA_SEMPRE_DEFAULTS') or C_UTILIZZA_SEMPRE_DEFAULTS != "AUTO")
else $insappartamenti = 1;

} # fine if ($fileaperto = @fopen(C_DATI_PATH"/dati_connessione.php","a+"))

else {
esegui_query("drop table $tableappartamenti");
esegui_query("drop table $tableclienti");
esegui_query("drop table $tableanni");
esegui_query("drop table $tableversioni");
esegui_query("drop table $tablenazioni");
esegui_query("drop table $tableregioni");
esegui_query("drop table $tablecitta");
esegui_query("drop table $tabledocumentiid");
esegui_query("drop table $tableparentele");
esegui_query("drop table $tablepersonalizza");
esegui_query("drop table $tableutenti");
esegui_query("drop table $tablegruppi");
esegui_query("drop table $tableprivilegi");
esegui_query("drop table $tablerelutenti");
esegui_query("drop table $tablesessioni");
esegui_query("drop table $tabletransazioni");
esegui_query("drop table $tabletransazioniweb");
esegui_query("drop table $tablecontratti");
esegui_query("drop table $tablecache");
esegui_query("drop table $tableinterconnessioni");
esegui_query("drop table $tablemessaggi");
esegui_query("drop table $tabledescrizioni");
esegui_query("drop table $tableinventario");
esegui_query("drop table $tablemagazzini");
esegui_query("drop table $tablerelinventario");
esegui_query("drop table $tablecasse");
esegui_query("drop table $tablerelclienti");
esegui_query("drop table $tablerelgruppi");
disconnetti_db($numconnessione);
if ($database_esistente == "NO") {
sleep(3);
if ($tipo_db == "postgresql") {
$numconnessione = pg_connect("dbname=$tempdatabase host=$host_phprdb port=$port_phprdb user=$user_phprdb password=$password_phprdb ");
} # fine if ($tipo_db == "postgresql")
if ($tipo_db == "mysql") {
$numconnessione = mysqli_connect($host_phprdb, $user_phprdb, $password_phprdb, "", $port_phprdb);
} # fine if ($tipo_db == "mysql")
esegui_query("drop database $database_phprdb");
} # fine if ($database_esistente == "NO")
echo "<div class=\"rbox\" style=\"--rbox-color: #F44336;\">
    <div class=\"rheader\">⚠️ ".mex2("Errore",$pag,$lingua)."</div>
    <div class=\"rcontent\">".mex2("Non ho i permessi di scrittura sulla directory dati, cambiarli e reiniziare l'installazione",$pag,$lingua)."</div>
</div>";
$permessi_scrittura_controllati = "SI";
$torna_indietro = "SI";
} # fine else if ($fileaperto = @fopen(C_DATI_PATH."/dati_connessione.php","a+"))

if ($database_esistente != "NO" and $character_set_db_orig and ($character_set_db != $character_set_db_orig or $collation_db != $collation_db_orig)) @esegui_query("alter database $database_phprdb default character set '$character_set_db_orig' collate '$collation_db_orig'");
} # fine if ($query)

else {
echo "<div class=\"rbox\" style=\"--rbox-color: #F44336;\">
    <div class=\"rheader\">❌ ".mex2("Errore",$pag,$lingua)."</div>
    <div class=\"rcontent\">".mex2("Non è stato possibile creare il database, controllare i privilegi dell' utente, il nome del database o se esiste già un database chiamato",$pag,$lingua)." $database_phprdb.</div>
</div>";
$torna_indietro = "SI";
} # fine else if ($query)
} # fine if ($numconnessione)
else {
$error_msg = mex2("I dati inseriti per il collegamento al database non sono esatti o il database non è in ascolto",$pag,$lingua);
if ($tipo_db == "postgresql") $error_msg .= " (".mex2("se postgres assicurarsi che venga avviato con -i e di avere i permessi giusti in pg_hba.conf",$pag,$lingua).")";
$error_msg .= ".";
echo "<div class=\"rbox\" style=\"--rbox-color: #F44336;\">
    <div class=\"rheader\">❌ ".mex2("Errore",$pag,$lingua)."</div>
    <div class=\"rcontent\">$error_msg</div>
</div>";
$torna_indietro = "SI";
} # fine else if ($numconnessione)
} # fine if (!$prefisso_tab or preg_match('/^[_a-z][_0-9a-z]*$/',$prefisso_tab))
else {
echo "<div class=\"rbox\" style=\"--rbox-color: #FFC107;\">
    <div class=\"rheader\">⚠️ ".mex2("Avviso",$pag,$lingua)."</div>
    <div class=\"rcontent\">".mex2("Il prefisso del nome delle tabelle è sbagliato (accettate solo lettere minuscole, numeri e _ , primo carattere lettera)",$pag,$lingua)."</div>
</div>";
$torna_indietro = "SI";
} # fine else if (!$prefisso_tab or preg_match('/^[_a-z][_0-9a-z]*$/',$prefisso_tab))
if ($permessi_scrittura_controllati != "SI") {
$fileaperto = @fopen(C_DATI_PATH."/prova.tmp","a+");
if (!$fileaperto) {
    echo "<div class=\"rbox\" style=\"--rbox-color: #F44336;\">
        <div class=\"rheader\">⚠️ ".mex2("Errore",$pag,$lingua)."</div>
        <div class=\"rcontent\">".mex2("Non ho i permessi di scrittura sulla directory dati, cambiarli e reiniziare l'installazione",$pag,$lingua)."</div>
    </div>";
} else {
fclose($fileaperto);
unlink(C_DATI_PATH."/prova.tmp");
} # fine else if (!$fileaperto)
} # fine if ($permessi_scrittura_controllati != "SI")
if ($torna_indietro == "SI") {
echo "<form accept-charset=\"utf-8\" method=\"post\" action=\"creadb.php\"><div>
<input type=\"hidden\" name=\"lingua\" value=\"$lingua\">
<input class=\"sbutton\" type=\"submit\" name=\"torna\" value=\"".mex2("Torna indietro",$pag,$lingua)."\"><br>
</div></form>";
} # fine if ($torna_indietro == "SI")
} # fine if (!empty($creabase) and !@is_file(C_DATI_PATH."/dati_connessione.php"))




// inserisco i dati forniti nella tabella appartamenti e creo il file selezione appartamenti.
if (!empty($insappartamenti) and !@is_file(C_DATI_PATH."/selectappartamenti.php")) {
$mostra_form_iniziale = "NO";
if (!controlla_num_pos($numletti) == "NO") $numletti = 0;
if ((!$numappartamenti and !$numletti) or controlla_num_pos($numappartamenti) == "NO") $numappartamenti = 5;
unset($lingua);
include(C_DATI_PATH."/lingua.php");
$lingua_mex = $lingua[1];
include(C_DATI_PATH."/dati_connessione.php");
include_once("./includes/funzioni_$PHPR_DB_TYPE.php");
$numconnessione = connetti_db($PHPR_DB_NAME,$PHPR_DB_HOST,$PHPR_DB_PORT,$PHPR_DB_USER,$PHPR_DB_PASS,$PHPR_LOAD_EXT);
$tableappartamenti = $PHPR_TAB_PRE."appartamenti";
$fileaperto = fopen(C_DATI_PATH."/selectappartamenti.php","a+");
fwrite($fileaperto,"<?php \necho '\n");
$zeri = (string) "0000000000000000000000000000";
$lettere = (string) "abcdefghijklmnopqrstuvwxyz";
$pos_lettera = 0;
$pos_lettera_tot = 0;
$suff_lettera = "";
$num_dorm = $numappartamenti;
if (!empty($lista_camere_letti)) {
$lista_camere_letti = explode(",",",$lista_camere_letti");
$num_camera_l = 1;
} # fine if (!empty($lista_camere_letti))
else $num_camera_l = 0;
$app_vicini_vett = array();
$numapp_esist = array();
if (!$numletti) $num_app_max = $numappartamenti;
else {
if ($num_camera_l) $num_app_max = $numappartamenti + count($lista_camere_letti) - 1;
else $num_app_max = $numappartamenti + ceil((double) $numletti / 26);
} # fine else if (!$numletti)
if (defined('C_UTILIZZA_SEMPRE_DEFAULTS') and C_UTILIZZA_SEMPRE_DEFAULTS == "AUTO" and $num_app_max < 10) $num_app_max = 10;

for ($num = 1 ; $num <= ($numappartamenti + $numletti) ; $num = $num + 1) {
$numapp = "numapp" . $num;
$numapp = fixstr($$numapp);
if (defined('C_UTILIZZA_SEMPRE_DEFAULTS') and (C_UTILIZZA_SEMPRE_DEFAULTS == "SI" or C_UTILIZZA_SEMPRE_DEFAULTS == "AUTO")) {
if ($num <= $numappartamenti) $numapp = (string) substr($zeri,0,(strlen($num_app_max) - strlen($num))).$num;
else {
if ($pos_lettera_tot == 0) {
$num_dorm++;
$num_dorm = (string) substr($zeri,0,(strlen($num_app_max) - strlen($num_dorm))).$num_dorm;
if ($num_camera_l and $lista_camere_letti[$num_camera_l] > 26) $suff_lettera = "a";
} # fine if ($pos_lettera_tot == 0)
$numapp = $num_dorm.$suff_lettera.substr($lettere,$pos_lettera,1);
if ($num_camera_l) {
$app_vicini_vett['pos'][$num] = $numapp;
if ($pos_lettera > 0) {
if (!isset($app_vicini_vett[$num])) $app_vicini_vett[$num] = "";
if (!isset($app_vicini_vett[($num - 1)])) $app_vicini_vett[($num - 1)] = "";
if ($pos_lettera > 1) {
$app_vicini_vett[($num - 2)] .= ",$numapp";
$app_vicini_vett[$num] .= ",".$app_vicini_vett['pos'][($num - 2)];
} # fine if ($pos_lettera > 1)
$app_vicini_vett[($num - 1)] .= ",$numapp";
$app_vicini_vett[$num] .= ",".$app_vicini_vett['pos'][($num - 1)];
} # fine if ($pos_lettera > 0)
} # fine if ($num_camera_l)
$pos_lettera++;
$pos_lettera_tot++;
if (($pos_lettera == 26 and !$num_camera_l) or ($num_camera_l and $pos_lettera_tot == $lista_camere_letti[$num_camera_l])) {
$pos_lettera = 0;
$pos_lettera_tot = 0;
$suff_lettera = "";
if ($num_camera_l) $num_camera_l++;
} # fine if (($pos_lettera == 26 and !$num_camera_l) or ($num_camera_l and...
if ($pos_lettera >= 26) {
$pos_lettera = 0;
if (!$suff_lettera) $suff_lettera = "a";
else $suff_lettera = substr(strstr($lettere,$suff_lettera),1,1);
} # fine if ($pos_lettera >= 26)
} # fine else if ($num <= $numappartamenti)
} # fine if (defined('C_UTILIZZA_SEMPRE_DEFAULTS') and (C_UTILIZZA_SEMPRE_DEFAULTS == "SI" or C_UTILIZZA_SEMPRE_DEFAULTS == "AUTO"))
$numapp = str_replace (",","",$numapp);
if (str_replace (" ","",$numapp) == "") $numapp = str_replace (" ","_",$numapp);
$numapp = trim($numapp);
if ($numapp == "") $numapp = "_";
$numapp_file = htmlspecialchars(elimina_caratteri_slash($numapp),ENT_COMPAT);
$numapp = aggslashdb($numapp_file);
while (!empty($numapp_esist[$numapp])) $numapp .= "_";
$numapp_esist[$numapp] = 1;
$piano = "piano" . $num;
$piano = aggslashdb(htmlspecialchars(elimina_caratteri_slash($$piano),ENT_COMPAT));
$maxoccupanti = "maxoccupanti" . $num;
$maxoccupanti = fixset($$maxoccupanti);
if ($num > $numappartamenti) $maxoccupanti = 1;
$numcasa = "numcasa" . $num;
$numcasa = aggslashdb(htmlspecialchars(elimina_caratteri_slash($$numcasa),ENT_COMPAT));
$priorita = "priorita" . $num;
$priorita = fixset($$priorita);
$app_vicini = "app_vicini" . $num;
$app_vicini = aggslashdb(htmlspecialchars(fixstr($$app_vicini),ENT_COMPAT));
if (controlla_num($maxoccupanti) != "SI") unset($maxoccupanti);
if (controlla_num($priorita) != "SI") unset($priorita);
esegui_query("insert into $tableappartamenti ( idappartamenti ) values ( '$numapp' )");
fwrite($fileaperto,"<option value=\"$numapp_file\">$numapp_file</option>
");
if ($piano) {
esegui_query("update $tableappartamenti set numpiano = '$piano' where idappartamenti = '$numapp'");
} # fine if ($piano)
if ($maxoccupanti) {
esegui_query("update $tableappartamenti set maxoccupanti = '$maxoccupanti' where idappartamenti = '$numapp'");
} # fine if ($maxoccupanti)
if ($numcasa) {
esegui_query("update $tableappartamenti set numcasa = '$numcasa' where idappartamenti = '$numapp'");
} # fine if ($numcasa)
if ($priorita) {
esegui_query("update $tableappartamenti set priorita = '$priorita' where idappartamenti = '$numapp'");
} # fine if ($priorita)
if ($app_vicini and (!isset($assegna_vicini_nc) or $assegna_vicini_nc != "SI")) {
esegui_query("update $tableappartamenti set app_vicini = '$app_vicini' where idappartamenti = '$numapp'");
} # fine if ($app_vicini and (!isset($assegna_vicini_nc) or $assegna_vicini_nc != "SI"))
if ($num > $numappartamenti) esegui_query("update $tableappartamenti set letto = '1' where idappartamenti = '$numapp'");
} # fine for $num

fwrite($fileaperto,"';\n?>");
fclose($fileaperto);
if (isset($assegna_vicini_nc) and $assegna_vicini_nc == "SI") {
$appart = esegui_query("select * from $tableappartamenti");
for ($num1 = 0 ; $num1 < $numappartamenti ; $num1 = $num1 + 1) {
$idapp = risul_query($appart,$num1,'idappartamenti');
$nc = risul_query($appart,$num1,'numcasa');
$np = risul_query($appart,$num1,'numpiano');
$query = "select idappartamenti from $tableappartamenti where numcasa = '$nc' and idappartamenti != '$idapp'";
if ($assegna_vicini_np == "SI") {
$query = $query." and numpiano = '$np'";
} # fine if ($assegna_vicini_np == "SI")
$av = esegui_query($query);
$num_av = numlin_query($av);
$app_vicini = "";
for ( $num2 = 0; $num2 < $num_av; $num2 = $num2 + 1) {
$id_av = risul_query($av,$num2,'idappartamenti');
if ($app_vicini == "") { $app_vicini = $id_av; }
else { $app_vicini = $app_vicini . "," . $id_av; }
} # fine for $num2
esegui_query("update $tableappartamenti set app_vicini = '$app_vicini' where idappartamenti = '$idapp'");
} # fine for $num1
} # fine if (isset($assegna_vicini_nc) and $assegna_vicini_nc == "SI")
if ($num_camera_l) {
for ($num = 1 ; $num <= ($numappartamenti + $numletti) ; $num = $num + 1) {
if ($app_vicini_vett[$num]) {
$app_vicini = substr($app_vicini_vett[$num],1);
esegui_query("update $tableappartamenti set app_vicini = '$app_vicini' where idappartamenti = '".$app_vicini_vett['pos'][$num]."'");
} # fine if ($app_vicini_vett[$num])
} # fine for $num
} # fine if ($num_camera_l)

if (defined('C_UTILIZZA_SEMPRE_DEFAULTS') and C_UTILIZZA_SEMPRE_DEFAULTS == "AUTO" and @is_file(C_DATI_PATH."/ini.php")) {
include(C_DATI_PATH."/ini.php");
$admin = "";
if (defined('C_ADMIN_NAME')) $admin = C_ADMIN_NAME;
if (htmlspecialchars($admin) != $admin) $admin = "";
if (strcmp((string) $admin,"")) {
esegui_query("update $tableutenti set nome_utente = '".aggslashdb($admin)."' where idutenti = '1'");
$passw = "";
if (defined('C_ADMIN_PASS')) $passw = C_ADMIN_PASS;
if ($passw != str_replace("&","",$passw)) $passw = "";
if (strcmp((string) $passw,"")) {
if (C_ADMIN_MD5P >= "1" and C_ADMIN_MD5P <= "15") $md5p = C_ADMIN_MD5P;
else $md5p = 0;
if (defined('C_ADMIN_SALT')) $salt = C_ADMIN_SALT;
else {
if ($md5p) $salt = "";
else {
srand((double) microtime() * 1000000);
$valori = "=?#@%abcdefghijkmnpqrstuvwxzABCDEFGHJKLMNPQRSTUVWXZ1234567890";
$salt = substr($valori,rand(0,4),1);
for ($num1 = 0 ; $num1 < 19 ; $num1++) $salt .= substr($valori,rand(0,60),1);
} # fine else if ($md5p)
} # fine else if (defined('C_ADMIN_SALT'))
for ($num1 = $md5p ; $num1 < 15 ; $num1++) $passw = md5($passw.substr($salt,0,(20 - $num1)));
esegui_query("update $tableutenti set password = '$passw', salt = '$salt', tipo_pass = '5' where idutenti = '1'");
$fileaperto = fopen(C_DATI_PATH."/abilita_login","w+");
fclose($fileaperto);
} # fine if (strcmp((string) $passw,""))
} # fine if (strcmp((string) $admin,""))
@unlink(C_DATI_PATH."/ini.php");
} # fine if (defined('C_UTILIZZA_SEMPRE_DEFAULTS') and C_UTILIZZA_SEMPRE_DEFAULTS == "AUTO" and @is_file(C_DATI_PATH."/ini.php"))

$tablemessaggi = $PHPR_TAB_PRE."messaggi";
$testo = "<div style=\"max-width: 600px; line-height: 1.1;\">";
if (!defined('C_NASCONDI_MARCA') or C_NASCONDI_MARCA != "SI") $testo .= "<h4>".mex("Benvenuto a HotelDruid!",$pag)."</h4><br>";
$testo .= "".mex("Questi sono alcuni semplici passi che puoi seguire per configurare le funzionalità di base di HotelDruid",$pag).":<br>
<ul style=\"line-height: 1.2;\">
<li>".mex("Inserisci le informazioni sugli appartamenti dalla",'unit.php')."
 <em><b><a href=\"./visualizza_tabelle.php?tipo_tabella=appartamenti&amp;<sessione>\">".mex("tabella appartamenti",'unit.php')."</a></b></em>, 
 ".mex("utilizzando l'apposito tasto al di sotto di essa",$pag).". ".mex("Gli appartamenti possono essere creati, cancellati e rinominati",'unit.php').". 
 ".mex("Si consiglia di inserire almeno la capienza massima per ogni appartamento",'unit.php').".<br><br></li>
<li>".mex("Inserisci il numero di tariffe, un nome per ciascuna di esse ed i prezzi corrispondenti dalla",$pag)." 
 <em><b><a href=\"./creaprezzi.php?<sessione>\">".mex("pagina inserimento prezzi",$pag)."</a></b></em>.
 ".mex("Considera che le tariffe di HotelDruid fungono anche da tipologie di appartamenti",'unit.php')." (".mex("vedi passo successivo",$pag).").<br><br></li>
<li>".mex("Associa una lista di appartamenti ad ogni tariffa, inserendo una regola di assegnazione 2 per ognuna di esse, dalla",'unit.php')." 
 <em><b><a href=\"./crearegole.php?<sessione>#regola2\">".mex("pagina inserimento regole",$pag)."</a></b></em>.
 ".mex("Ogni appatamento può essere associato a più tariffe",'unit.php').".<br><br></li>
<li>".mex("Se questo server web è pubblico si può abilitare il login e creare nuovi utenti dalla",$pag)."
 <em><b><a href=\"./gestione_utenti.php?<sessione>\">".mex("pagina gestione utenti",$pag)."</a></b></em>.<br><br></li>
<li>".mex("Vai alla pagina",$pag)."
 \"<em><b><a href=\"./personalizza.php?<sessione>\">".mex("configura e personalizza",$pag)."</a></b></em>\"
 ".mex("per cambiare il nome della valuta, abilitare la registrazione delle entrate, inserire i metodi di pagamento, ed impostare molte altre opzioni",$pag).".<br><br></li>
</ul></div>";
if (defined('C_NASCONDI_MARCA') and C_NASCONDI_MARCA == "SI") $testo = str_replace("HotelDruid",mex("questo programma",$pag),$testo);
$testo = aggslashdb($testo);
$datainserimento = date("Y-m-d H:i:s",(time() + (C_DIFF_ORE * 3600)));
esegui_query("insert into $tablemessaggi (idmessaggi,tipo_messaggio,idutenti,idutenti_visto,datavisione,mittente,testo,datainserimento) values ('1','sistema',',1,',',1,','$datainserimento','1','$testo','$datainserimento')");

echo "<div class=\"rbox\" style=\"--rbox-color: #4CAF50;\">
    <div class=\"rheader\">✅ ".mex("Dati inseriti",$pag)."</div>
    <div class=\"rcontent\">".mex("Tutti i dati permanenti sono stati inseriti",$pag)."</div>
</div>";
echo "<form accept-charset=\"utf-8\" method=\"post\" action=\"inizio.php\"><div>
<input type=\"hidden\" name=\"nuovo_mess\" value=\"1\">
<input class=\"sbutton\" type=\"submit\" value=\"OK\"><br>
</div></form>";
if (defined('C_CREA_ULTIMO_ACCESSO') and C_CREA_ULTIMO_ACCESSO == "SI") {
$fileaperto = @fopen(C_DATI_PATH."/ultimo_accesso","w+");
@fwrite($fileaperto,date("d-m-Y H:i:s"));
@fclose($fileaperto);
@chmod(C_DATI_PATH."/ultimo_accesso",0644);
} # fine if (defined('C_CREA_ULTIMO_ACCESSO') and C_CREA_ULTIMO_ACCESSO == "SI")
} # fine if (!empty($insappartamenti) and !@is_file(C_DATI_PATH."/selectappartamenti.php"))




if (!isset($mostra_form_iniziale) or $mostra_form_iniziale != "NO") {

// prima form di inserimento
echo "<div class=\"rpanels\"><div class=\"rbox\"><div class=\"rheader\">".mex2("Inserimento dei dati permanenti",$pag,$lingua)."</div><br>
".mex2("Inserisci questi dati per poi creare il database",$pag,$lingua).".<br>
<br>
<form accept-charset=\"utf-8\" method=\"post\" action=\"creadb.php\"><div>
".mex2("Tipo di database",$pag,$lingua).": 
<select name=\"tipo_db\">";
if (C_CREADB_TIPODB == "postgresql") $selected = " selected";
else $selected = "";
echo "<option value=\"postgresql\"$selected>".mex2("Postgresql",$pag,$lingua)."</option>";
// Use MySQLi instead of deprecated MySQL
if (C_CREADB_TIPODB == "mysql" or C_CREADB_TIPODB == "mysqli" or !C_CREADB_TIPODB) $selected = " selected";
else $selected = "";
echo "<option value=\"mysqli\"$selected>".mex2("MySQL (MySQLi)",$pag,$lingua)."</option>";
if (C_CREADB_TIPODB == "sqlite") $selected = " selected";
else $selected = "";
echo "<option value=\"sqlite\"$selected>".mex2("Sqlite",$pag,$lingua)."</option>
</select><br>
".mex2("Nome del database da utilizzare",$pag,$lingua).": 
<input type=\"text\" name=\"database_phprdb\" value=\"".C_CREADB_NOMEDB."\"><br>
".mex2("Database già esistente",$pag,$lingua)."?
<select name=\"database_esistente\">";
if (C_CREADB_DB_ESISTENTE == "SI") $selected = " selected";
else $selected = "";
echo "<option value=\"SI\"$selected>".mex2("Si",$pag,$lingua)."</option>";
if (C_CREADB_DB_ESISTENTE == "NO") $selected = " selected";
else $selected = "";
echo "<option value=\"NO\"$selected>".mex2("No",$pag,$lingua)."</option>
</select><small>(".mex2("Se già esistente e non vuoto usare un prefisso non presente nel database per il nome delle tabelle",$pag,$lingua).")</small><br>
".mex2("Nome del computer a cui collegarsi",$pag,$lingua).":
<input type=\"text\" name=\"host_phprdb\" value=\"".C_CREADB_HOST."\"><br>
".mex2("Numero della porta a cui collegarsi",$pag,$lingua).": 
<input type=\"text\" name=\"port_phprdb\" value=\"".C_CREADB_PORT."\" size=\"7\">(".mex2("Normalmete 5432 o 5433 per Postgresql o 3306 per Mysql",$pag,$lingua).")<br>
".mex2("Nome per l'autenticazione al database",$pag,$lingua).": 
<input type=\"text\" name=\"user_phprdb\" value=\"".C_CREADB_USER."\"><br>
".mex2("Parola segreta per l'autenticazione al database",$pag,$lingua).": 
<input type=\"text\" name=\"password_phprdb\" value=\"".C_CREADB_PASS."\"><br>";
/*echo "".mex2("Caricare la libreria dinamica \"pgsql.so\" o \"mysql.so\"",$pag,$lingua)."?
<select name=\"carica_estensione\">";
if (C_CREADB_ESTENSIONE == "SI") $selected = " selected";
else $selected = "";
echo "<option value=\"SI\"$selected>".mex2("Si",$pag,$lingua)."</option>";
if (C_CREADB_ESTENSIONE == "NO") $selected = " selected";
else $selected = "";
echo "<option value=\"NO\"$selected>".mex2("No",$pag,$lingua)."</option>
</select> <small>(".mex2("scegliere si se non viene caricata automaticamente da php",$pag,$lingua).")</small><br>";*/
$messaggio = "";
$unit = array('gender' => 'm', 'special' => '', 's_n' => '', 'p_n' => '');
if ($lingua == "ita") include("./includes/unit.php");
else include("./includes/lang/$lingua/unit.php");
echo "".mex2("Nome del database a cui collegarsi temporaneamente",$pag,$lingua).":
<input type=\"text\" name=\"tempdatabase\" value=\"".C_CREADB_TEMPDB."\"><small>(".mex2("solo per Postgresql con database non esistente",$pag,$lingua).")</small><br>
".mex2("Prefisso nel nome delle tabelle",$pag,$lingua).":
<input type=\"text\" name=\"prefisso_tab\" value=\"".C_CREADB_PREFISSO_TAB."\" maxlength=\"8\" size=\"9\"><small>(".mex2("opzionale, utile per più installazioni di HotelDruid nello stesso database",$pag,$lingua).")</small><br>
<div style=\"height: 8px\"></div>
".mex2("Nome delle unità da gestire",$pag,$lingua).": <select name=\"nomeappartamenti\">
<option value=\"camere\">".$trad_var['rooms']."</option>
<option value=\"appartamenti\">".$trad_var['apartments']."</option>
</select><br>
".mex2("Numero di unità da gestire",$pag,$lingua).": 
<input type=\"text\" name=\"numappartamenti\" size=\"5\"><br>
<div style=\"height: 6px\"></div>
".mex2("Nome delle unità singole da gestire",$pag,$lingua).": <em>".$trad_var['beds']."</em> <small>(".mex2("non incluse nelle unità normali",$pag,$lingua).")</small><br>
".mex2("Numero di unità singole da gestire",$pag,$lingua).": 
<input type=\"text\" name=\"numletti\" value=\"0\" size=\"5\"><br>
<div style=\"text-align: center;\"><input class=\"sbutton\" type=\"submit\" name=\"creabase\" value=\"".mex2("Crea il database",$pag,$lingua)."\"></div><br>
<input type=\"hidden\" name=\"lingua\" value=\"$lingua\">
</div></form></div></div>";

} # fine if (!isset($mostra_form_iniziale) or $mostra_form_iniziale != "NO")




if ($tema[$id_utente] and $tema[$id_utente] != "base" and @is_dir("./themes/".$tema[$id_utente]."/php")) include("./themes/".$tema[$id_utente]."/php/foot.php");
else include("./includes/foot.php");


} # fine if ($id_utente and $id_utente == 1)


} # fine if (!defined('C_CREA_ULTIMO_ACCESSO') or CREA_ULTIMO_ACCESSO != "SI" or !@is_file(C_DATI_PATH."/ultimo_accesso"))


?>
