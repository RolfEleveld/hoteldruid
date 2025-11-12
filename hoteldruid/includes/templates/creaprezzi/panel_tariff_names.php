<?php
/**
 * Panel: Tariff Names Management
 * Color: Green (#4CAF50)
 */

// Build the tariff list dynamically
$tariff_rows = '';
for ($num1 = 1 ; $num1 <= $dati_tariffe['num'] ; $num1++) {
    if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[$num1])) {
        $tariffa = "tariffa".$num1;
        $current_name = $dati_tariffe[$tariffa]['nome'];
        $tariff_label = mex("tariffa",$pag).$num1;
        
        $tariff_rows .= "
        <tr>
            <td class=\"label-col\">$tariff_label:</td>
            <td class=\"input-col\">
                <input type=\"text\" 
                       name=\"nometariffa_$num1\" 
                       id=\"nometariffa_$num1\" 
                       value=\"".htmlspecialchars($current_name)."\" 
                       size=\"30\"
                       class=\"tariff-name-input\">
            </td>
            <td class=\"action-col\">
                <button class=\"edit\" type=\"submit\" name=\"cambia_nome_tariffa\" value=\"$num1\">
                    <div>".mex("Cambia",$pag)."</div>
                </button>
            </td>
        </tr>";
    }
}
?>

<div class="rbox" style="border-left-color: #4CAF50;">
    <div class="rheader" style="background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);">
        <h5><?php echo mex("Nomi delle tariffe",$pag); ?></h5>
    </div>
    <div class="rcontent">
        <table class="tariff-names-table">
            <thead>
                <tr>
                    <th><?php echo mex("Tariffa",$pag); ?></th>
                    <th><?php echo mex("Nome",$pag); ?></th>
                    <th><?php echo mex("Azione",$pag); ?></th>
                </tr>
            </thead>
            <tbody>
                <?php echo $tariff_rows; ?>
            </tbody>
        </table>
        
        <?php if ($id_utente == 1): ?>
        <div class="admin-actions">
            <hr style="margin: 20px 0;">
            <h6><?php echo mex("Impostazioni amministratore",$pag); ?></h6>
            <div class="admin-buttons">
                <form accept-charset="utf-8" method="post" action="./personalizza.php" style="display: inline-block; margin-right: 10px;">
                    <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                    <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                    <input type="hidden" name="aggiorna_qualcosa" value="SI">
                    <input type="hidden" name="origine" value="./creaprezzi.php">
                    <input type="hidden" name="cambianumerotariffe" value="1">
                    <?php echo mex("Numero tariffe","personalizza.php"); ?>: 
                    <input type="text" name="nuovo_numero_tariffe" size="5" value="<?php echo $dati_tariffe['num']; ?>">
                    <button class="ipri" type="submit"><div><?php echo mex("Cambia","personalizza.php"); ?></div></button>
                </form>
                
                <form accept-charset="utf-8" method="post" action="./personalizza.php" style="display: inline-block; margin-right: 10px;">
                    <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                    <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                    <input type="hidden" name="aggiorna_qualcosa" value="SI">
                    <input type="hidden" name="cambia_ord_tariffe" value="SI">
                    <input type="hidden" name="origine" value="./creaprezzi.php">
                    <button class="xpri" type="submit"><div><?php echo ucfirst(mex("cambia l'ordine delle tariffe","personalizza.php")); ?></div></button>
                </form>
                
                <form accept-charset="utf-8" method="post" action="./visualizza_tabelle.php" style="display: inline-block;">
                    <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                    <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                    <input type="hidden" name="tipo_tabella" value="periodi">
                    <input type="hidden" name="mostra_form_agg_per" value="1">
                    <input type="hidden" name="origine" value="./creaprezzi.php">
                    <button class="amon" type="submit"><div><?php echo mex("Aggiungi periodi","visualizza_tabelle.php"); ?></div></button>
                </form>
            </div>
        </div>
        <?php endif; ?>
    </div>
</div>

<style>
.tariff-names-table {
    width: 100%;
    border-collapse: collapse;
    margin: 10px 0;
}

.tariff-names-table thead th {
    background: #f5f5f5;
    padding: 10px;
    text-align: left;
    font-weight: 600;
    border-bottom: 2px solid #ddd;
}

.tariff-names-table tbody td {
    padding: 8px 10px;
    border-bottom: 1px solid #eee;
    vertical-align: middle;
}

.tariff-names-table .label-col {
    font-weight: 500;
    width: 15%;
}

.tariff-names-table .input-col {
    width: 60%;
}

.tariff-names-table .action-col {
    width: 25%;
    text-align: right;
}

.tariff-name-input {
    width: 100%;
    padding: 6px 10px;
    border: 1px solid #ddd;
    border-radius: 3px;
}

.admin-actions {
    margin-top: 20px;
}

.admin-actions h6 {
    margin: 10px 0;
    font-size: 14px;
    color: #666;
}

.admin-buttons {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
    align-items: center;
}
</style>
