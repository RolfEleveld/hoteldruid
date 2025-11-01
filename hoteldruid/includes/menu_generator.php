<?php
/**
 * Enhanced main menu generation for HotelDruid
 * Modern, responsive menu with better organization
 */

function generate_modern_main_menu($anno, $id_sessione, $privileges, $language = 'ita', $user_name = '', $subordination_comment = '') {
    $menu_sections = array();
    $nav_items = array();
    
    // Reservations section
    if ($privileges['priv_ins_nuove_prenota'] == 's') {
        $reservation_items = array();
        
        $reservation_items[] = array(
            'title' => mex("Inserisci una nuova prenotazione", 'inizio.php'),
            'description' => mex("Crea una nuova prenotazione per i tuoi ospiti", 'inizio.php'),
            'url' => "prenota.php?anno=$anno&id_sessione=$id_sessione",
            'icon' => '<svg fill="currentColor" viewBox="0 0 20 20"><path d="M8 9a3 3 0 100-6 3 3 0 000 6zM8 11a6 6 0 616 6H2a6 6 0 616-6zM16 7a1 1 0 10-2 0v1h-1a1 1 0 100 2h1v1a1 1 0 102 0v-1h1a1 1 0 100-2h-1V7z"/></svg>',
            'privileges' => array('priv_ins_nuove_prenota' => 's')
        );
        
        $menu_sections['reservations'] = array(
            'title' => mex("Prenotazioni", 'inizio.php'),
            'items' => $reservation_items
        );
        
        $nav_items[] = array(
            'title' => mex("Prenotazioni", 'inizio.php'),
            'url' => "prenota.php?anno=$anno&id_sessione=$id_sessione"
        );
    }
    
    // Tables and Views section
    if ($privileges['priv_vedi_tab_mesi'] != 'n') {
        $table_items = array();
        
        $table_items[] = array(
            'title' => mex("Tabella Prenotazioni", 'inizio.php'),
            'description' => mex("Visualizza le prenotazioni per mese", 'inizio.php'),
            'url' => "tabella.php?anno=$anno&id_sessione=$id_sessione&mese=" . date("n"),
            'icon' => '<svg fill="currentColor" viewBox="0 0 20 20"><path d="M3 4a1 1 0 011-1h12a1 1 0 011 1v2a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM3 10a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H4a1 1 0 01-1-1v-6zM14 9a1 1 0 00-1 1v6a1 1 0 001 1h2a1 1 0 001-1v-6a1 1 0 00-1-1h-2z"/></svg>'
        );
        
        if ($privileges['priv_vedi_tab_periodi'] != 'n') {
            $table_items[] = array(
                'title' => mex("Visualizza tabelle", 'inizio.php'),
                'description' => mex("Accedi a tutte le tabelle del sistema", 'inizio.php'),
                'url' => "visualizza_tabelle.php?anno=$anno&id_sessione=$id_sessione",
                'icon' => '<svg fill="currentColor" viewBox="0 0 20 20"><path d="M5 3a2 2 0 00-2 2v2a2 2 0 002 2h2a2 2 0 002-2V5a2 2 0 00-2-2H5zM5 11a2 2 0 00-2 2v2a2 2 0 002 2h2a2 2 0 002-2v-2a2 2 0 00-2-2H5zM11 5a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V5zM11 13a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z"/></svg>'
            );
        }
        
        $menu_sections['tables'] = array(
            'title' => mex("Tabelle e Visualizzazioni", 'inizio.php'),
            'items' => $table_items
        );
        
        $nav_items[] = array(
            'title' => mex("Tabelle", 'inizio.php'),
            'url' => "visualizza_tabelle.php?anno=$anno&id_sessione=$id_sessione"
        );
    }
    
    // Clients section
    if ($privileges['inserimento_nuovi_clienti'] == 'SI' || $privileges['vedi_clienti'] == 'SI') {
        $client_items = array();
        
        if ($privileges['inserimento_nuovi_clienti'] == 'SI') {
            $client_items[] = array(
                'title' => mex("Inserisci cliente", 'inizio.php'),
                'description' => mex("Aggiungi un nuovo cliente al database", 'inizio.php'),
                'url' => "clienti.php?anno=$anno&id_sessione=$id_sessione",
                'icon' => '<svg fill="currentColor" viewBox="0 0 20 20"><path d="M8 9a3 3 0 100-6 3 3 0 000 6zM8 11a6 6 0 616 6H2a6 6 0 616-6zM16 7a1 1 0 10-2 0v1h-1a1 1 0 100 2h1v1a1 1 0 102 0v-1h1a1 1 0 100-2h-1V7z"/></svg>'
            );
        }
        
        if ($privileges['vedi_clienti'] == 'SI') {
            $client_items[] = array(
                'title' => mex("Visualizza clienti", 'inizio.php'),
                'description' => mex("Gestisci e visualizza i clienti esistenti", 'inizio.php'),
                'url' => "visualizza_tabelle.php?tipo_tabella=clienti&anno=$anno&id_sessione=$id_sessione",
                'icon' => '<svg fill="currentColor" viewBox="0 0 20 20"><path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>'
            );
        }
        
        $menu_sections['clients'] = array(
            'title' => mex("Clienti", 'inizio.php'),
            'items' => $client_items
        );
        
        $nav_items[] = array(
            'title' => mex("Clienti", 'inizio.php'),
            'url' => "clienti.php?anno=$anno&id_sessione=$id_sessione"
        );
    }
    
    // Financial section
    if ($privileges['priv_ins_spese'] == 's' || $privileges['priv_ins_entrate'] == 's') {
        $financial_items = array();
        
        if ($privileges['priv_ins_spese'] == 's') {
            $financial_items[] = array(
                'title' => mex("Inserisci spese", 'inizio.php'),
                'description' => mex("Registra le spese dell'hotel", 'inizio.php'),
                'url' => "storia_soldi.php?tipo=spese&anno=$anno&id_sessione=$id_sessione",
                'icon' => '<svg fill="currentColor" viewBox="0 0 20 20"><path d="M4 4a2 2 0 00-2 2v4a2 2 0 002 2V6h10a2 2 0 00-2-2H4zM14 6a2 2 0 712 2v4a2 2 0 01-2 2H8a2 2 0 01-2-2V8a2 2 0 712-2h6zM4 14a2 2 0 002 2h8a2 2 0 002-2v-2a2 2 0 00-2-2H6a2 2 0 00-2 2v2z"/></svg>'
            );
        }
        
        if ($privileges['priv_ins_entrate'] == 's') {
            $financial_items[] = array(
                'title' => mex("Inserisci entrate", 'inizio.php'),
                'description' => mex("Registra le entrate dell'hotel", 'inizio.php'),
                'url' => "storia_soldi.php?tipo=entrate&anno=$anno&id_sessione=$id_sessione",
                'icon' => '<svg fill="currentColor" viewBox="0 0 20 20"><path d="M4 4a2 2 0 00-2 2v4a2 2 0 002 2V6h10a2 2 0 00-2-2H4zM14 6a2 2 0 712 2v4a2 2 0 01-2 2H8a2 2 0 01-2-2V8a2 2 0 712-2h6zM4 14a2 2 0 002 2h8a2 2 0 002-2v-2a2 2 0 00-2-2H6a2 2 0 00-2 2v2z"/></svg>'
            );
        }
        
        $menu_sections['financial'] = array(
            'title' => mex("Gestione Finanziaria", 'inizio.php'),
            'items' => $financial_items
        );
        
        $nav_items[] = array(
            'title' => mex("Finanze", 'inizio.php'),
            'url' => "storia_soldi.php?anno=$anno&id_sessione=$id_sessione"
        );
    }
    
    // Configuration section
    if ($privileges['modifica_pers'] != 'NO' || $privileges['priv_crea_backup'] == 's') {
        $config_items = array();
        
        if ($privileges['modifica_pers'] != 'NO') {
            $config_items[] = array(
                'title' => mex("Configura e personalizza", 'inizio.php'),
                'description' => mex("Modifica le impostazioni del sistema", 'inizio.php'),
                'url' => "personalizza.php?anno=$anno&id_sessione=$id_sessione",
                'icon' => '<svg fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M11.49 3.17c-.38-1.56-2.6-1.56-2.98 0a1.532 1.532 0 01-2.286.948c-1.372-.836-2.942.734-2.106 2.106.54.886.061 2.042-.947 2.287-1.561.379-1.561 2.6 0 2.978a1.532 1.532 0 01.947 2.287c-.836 1.372.734 2.942 2.106 2.106a1.532 1.532 0 012.287.947c.379 1.561 2.6 1.561 2.978 0a1.533 1.533 0 712.287-.947c1.372.836 2.942-.734 2.106-2.106a1.533 1.533 0 01.947-2.287c1.561-.379 1.561-2.6 0-2.978a1.532 1.532 0 01-.947-2.287c.836-1.372-.734-2.942-2.106-2.106a1.532 1.532 0 01-2.287-.947zM10 13a3 3 0 100-6 3 3 0 000 6z" clip-rule="evenodd"/></svg>'
            );
        }
        
        if ($privileges['priv_crea_backup'] == 's') {
            $config_items[] = array(
                'title' => mex("Backup", 'inizio.php'),
                'description' => mex("Gestisci i backup del sistema", 'inizio.php'),
                'url' => "crea_backup.php?anno=$anno&id_sessione=$id_sessione",
                'icon' => '<svg fill="currentColor" viewBox="0 0 20 20"><path d="M7.707 10.293a1 1 0 10-1.414 1.414l3 3a1 1 0 001.414 0l3-3a1 1 0 00-1.414-1.414L11 11.586V6h5a2 2 0 712 2v7a2 2 0 01-2 2H4a2 2 0 01-2-2V8a2 2 0 712-2h5v5.586l-1.293-1.293zM9 4a1 1 0 712 0v2H9V4z"/></svg>'
            );
        }
        
        $menu_sections['configuration'] = array(
            'title' => mex("Configurazione", 'inizio.php'),
            'items' => $config_items
        );
        
        $nav_items[] = array(
            'title' => mex("Configurazione", 'inizio.php'),
            'url' => "personalizza.php?anno=$anno&id_sessione=$id_sessione"
        );
    }
    
    return array(
        'menu_sections' => $menu_sections,
        'nav_items' => $nav_items,
        'user_name' => $user_name,
        'year' => $anno,
        'subordination_comment' => $subordination_comment
    );
}

/**
 * Generate privileges array from the old global variables
 */
function get_user_privileges() {
    global $priv_ins_nuove_prenota, $priv_vedi_tab_mesi, $priv_vedi_tab_periodi,
           $inserimento_nuovi_clienti, $vedi_clienti, $modifica_clienti,
           $priv_ins_spese, $priv_ins_entrate, $modifica_pers, $priv_crea_backup,
           $priv_crea_interconnessioni, $priv_gest_pass_cc;
    
    return array(
        'priv_ins_nuove_prenota' => $priv_ins_nuove_prenota ?? 'n',
        'priv_vedi_tab_mesi' => $priv_vedi_tab_mesi ?? 'n',
        'priv_vedi_tab_periodi' => $priv_vedi_tab_periodi ?? 'n',
        'inserimento_nuovi_clienti' => $inserimento_nuovi_clienti ?? 'NO',
        'vedi_clienti' => $vedi_clienti ?? 'NO',
        'modifica_clienti' => $modifica_clienti ?? 'NO',
        'priv_ins_spese' => $priv_ins_spese ?? 'n',
        'priv_ins_entrate' => $priv_ins_entrate ?? 'n',
        'modifica_pers' => $modifica_pers ?? 'NO',
        'priv_crea_backup' => $priv_crea_backup ?? 'n',
        'priv_crea_interconnessioni' => $priv_crea_interconnessioni ?? 'n',
        'priv_gest_pass_cc' => $priv_gest_pass_cc ?? 'n'
    );
}

?>