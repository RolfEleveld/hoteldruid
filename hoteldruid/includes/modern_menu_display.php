<?php
// Modern main menu replacement for inizio.php

// Generate the modern menu using our new system
$user_privileges = get_user_privileges();
$user_display_name = $nome_utente_login ?? '';
$subordination_comment = $commento_subordinazione ?? '';

$menu_data = generate_modern_main_menu(
    $anno, 
    $id_sessione, 
    $user_privileges, 
    'ita', 
    $user_display_name, 
    $subordination_comment
);

// Include the modern CSS framework
echo '<link rel="stylesheet" href="./includes/modern.css">';

if (!defined('C_URL_LOGO') or C_URL_LOGO == "") {
    echo "<div id=\"mmenu\" class=\"modern-menu-container\">";
} else {
    echo "<div class=\"modern-menu-container\" style=\"background: url(".C_URL_LOGO.") no-repeat right top;\">";
}

// User info and logout
if ($nome_utente_login) {
    echo "<div class=\"logout modern-user-info\">";
    echo "<span class=\"user-name\">".mex("Utente",$pag).": $nome_utente_login</span>";
    echo " <a href=\"inizio.php?id_sessione=$id_sessione&amp;logout=SI\" class=\"logout-link\">".mex("Esci",$pag)."</a>";
    echo "</div>";
}

// Year info and main title
$anno_succ = $anno + 1;
if ($anno_corrente == $anno_succ and (!defined('C_CREA_ANNO_MANUALMENTE') or C_CREA_ANNO_MANUALMENTE != "SI") and !@is_file(C_DATI_PATH."/selectperiodi$anno_corrente.1.php")) {
    $anno_menu = $anno_corrente;
} else {
    $anno_menu = $anno;
}

echo "<div class=\"modern-menu-header\">";
echo "<h3 id=\"h_mm\" class=\"main-menu-title\">";
echo "<span>".mex("Menù principale dell'anno",$pag)." $anno_menu";
if (isset($commento_subordinazione)) echo " ($commento_subordinazione)";
echo "</span></h3>";
echo "</div>";

// Load the modern main menu template
$template = HotelDruidTemplate::getInstance();
$template->renderTemplate('main_menu', $menu_data);

// Year selection form (keeping the original functionality)
if ($vai_anno) {
    echo "<div class=\"year-selection-container\">";
    echo "<form accept-charset=\"utf-8\" method=\"post\" action=\"inizio.php\" class=\"year-form\">";
    echo "<input type=\"hidden\" name=\"id_sessione\" value=\"$id_sessione\">";
    echo $template->getCsrfTokenInput();
    echo "<div class=\"form-group\">";
    echo "<label for=\"anno\">".mex("Anno",$pag).":</label>";
    echo "<input type=\"text\" name=\"anno\" id=\"anno\" size=\"4\" maxlength=\"4\" value=\"$vai_anno\" class=\"form-input\">";
    echo "<button type=\"submit\" class=\"btn btn-primary\">".mex("Vai",$pag)."</button>";
    echo "</div></form>";
    
    if ($vai_anno) {
        echo "<div class=\"alert alert-info\">";
        echo "<span class=\"colinfo\">".mex("Nota",$pag)."</span>: ";
        echo mex("ricordarsi di tornare al",$pag)." $anno ".mex("una volta conclusa la consultazione del",$pag)." $vai_anno.";
        echo "</div>";
    }
    echo "</div>";
}

echo "</div>"; // Close mmenu container
?>