<?php
// Simple utility to change the hotel name
// Access: http://localhost:8080/change_hotel_name.php

include("./costanti.php");
include(C_DATI_PATH."/dati_connessione.php");
include("./includes/funzioni_$PHPR_DB_TYPE.php");

$numconnessione = connetti_db($PHPR_DB_NAME,$PHPR_DB_HOST,$PHPR_DB_PORT,$PHPR_DB_USER,$PHPR_DB_PASS,$PHPR_LOAD_EXT);
$tablepersonalizza = $PHPR_TAB_PRE."personalizza";

// Get current hotel name
$query_nome_hotel = esegui_query("select valpersonalizza from $tablepersonalizza where idpersonalizza = 'nome_hotel' and idutente = '1'");
$nome_hotel_attuale = 'Hoteldruid'; // Default
if (numlin_query($query_nome_hotel) > 0) {
    $nome_dal_db = trim(risul_query($query_nome_hotel, 0, 'valpersonalizza'));
    if (!empty($nome_dal_db)) {
        $nome_hotel_attuale = $nome_dal_db;
    }
}

$message = '';
if ($_SERVER['REQUEST_METHOD'] === 'POST' && !empty($_POST['nuovo_nome_hotel'])) {
    $nuovo_nome = trim($_POST['nuovo_nome_hotel']);
    if (!empty($nuovo_nome)) {
        // Update or insert the hotel name
        $esegui = esegui_query("insert into $tablepersonalizza (idpersonalizza, idutente, valpersonalizza) values ('nome_hotel', '1', '".aggslashdb($nuovo_nome)."') on duplicate key update valpersonalizza = '".aggslashdb($nuovo_nome)."'");
        if ($esegui) {
            $message = '<div style="color: green; padding: 10px; background: #f0fff0; border: 1px solid #00cc00; border-radius: 4px; margin: 10px 0;">Nome hotel aggiornato con successo!</div>';
            $nome_hotel_attuale = $nuovo_nome;
        } else {
            $message = '<div style="color: red; padding: 10px; background: #fff0f0; border: 1px solid #cc0000; border-radius: 4px; margin: 10px 0;">Errore durante l\'aggiornamento.</div>';
        }
    }
}
?>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Cambia Nome Hotel - HotelDruid</title>
    <style>
        body { font-family: Arial, sans-serif; max-width: 600px; margin: 50px auto; padding: 20px; background: #f5f5f5; }
        .container { background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        .form-group { margin: 20px 0; }
        label { display: block; margin-bottom: 8px; font-weight: bold; color: #333; }
        input[type="text"] { width: 100%; padding: 12px; border: 1px solid #ccc; border-radius: 4px; font-size: 16px; }
        .btn { background: #007cba; color: white; padding: 12px 24px; border: none; border-radius: 4px; cursor: pointer; font-size: 16px; }
        .btn:hover { background: #005a8a; }
        .current-name { background: #f0f8ff; padding: 15px; border-radius: 4px; margin: 15px 0; border-left: 4px solid #007cba; }
        .back-link { display: inline-block; margin-top: 20px; color: #007cba; text-decoration: none; }
        .back-link:hover { text-decoration: underline; }
    </style>
</head>
<body>
    <div class="container">
        <h2>🏨 Cambia Nome Hotel</h2>
        
        <?php echo $message; ?>
        
        <div class="current-name">
            <strong>Nome attuale:</strong> <?php echo htmlspecialchars($nome_hotel_attuale); ?>
        </div>
        
        <form method="post" action="">
            <div class="form-group">
                <label for="nuovo_nome_hotel">Nuovo nome hotel:</label>
                <input type="text" 
                       id="nuovo_nome_hotel" 
                       name="nuovo_nome_hotel" 
                       value="<?php echo htmlspecialchars($nome_hotel_attuale); ?>"
                       placeholder="Inserisci il nome del tuo hotel..."
                       required>
            </div>
            
            <button type="submit" class="btn">Aggiorna Nome Hotel</button>
        </form>
        
        <a href="inizio.php" class="back-link">← Torna alla Dashboard</a>
        
        <div style="margin-top: 30px; padding: 15px; background: #fff8dc; border-radius: 4px; font-size: 14px;">
            <strong>Nota:</strong> Il nome del hotel verrà mostrato nella dashboard principale e in altri punti dell'applicazione.
            Questo sostituisce il nome "Hoteldruid" che era precedentemente codificato nel sistema.
        </div>
    </div>
</body>
</html>