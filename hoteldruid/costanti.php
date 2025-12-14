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


// Path to dati folder and database data file
// Priority order:
// 1. External config file (hoteldruid-config.php) - set during deployment
// 2. Default (./dati) - fallback

if (!defined('C_DATI_PATH')) {
    $dati_path = null;
    
    // Check external config file (hoteldruid-config.php)
    if (file_exists(__DIR__ . '/hoteldruid-config.php')) {
        include(__DIR__ . '/hoteldruid-config.php');
        if (defined('C_DATI_PATH_EXTERNAL') && C_DATI_PATH_EXTERNAL !== "" && C_DATI_PATH_EXTERNAL !== null) {
            $dati_path = C_DATI_PATH_EXTERNAL;
        }
    }
    
    // Normalize and set the path
    if ($dati_path) {
        // Normalize the path (convert backslashes to forward slashes, remove trailing slashes)
        $dati_path = rtrim(str_replace('\\', '/', $dati_path), '/');
        // If it's a relative path, make it relative to the application directory
        // Check for absolute paths: Windows drive letter (C:/), Unix root (/), or UNC (\\)
        if (!preg_match('/^[A-Za-z]:\/|^\/|^\\\\/', $dati_path)) {
            $dati_path = __DIR__ . '/' . $dati_path;
        }
        // Normalize again after potential concatenation
        $dati_path = str_replace('\\', '/', $dati_path);
        define('C_DATI_PATH', $dati_path);
    } else {
        define('C_DATI_PATH', "./dati");
    }
}
// #define('C_EXT_DB_DATA_PATH',"");

// #define('C_CARTELLA_CREA_MODELLI',"");
// #define('C_URL_CREA_MODELLI',"");

// costanti generali
if (!defined('C_GIORNI_NUOVO_ANNO')) define('C_GIORNI_NUOVO_ANNO',"9");
// #define('C_PERCORSO_PHPMAILER',"");




?>
