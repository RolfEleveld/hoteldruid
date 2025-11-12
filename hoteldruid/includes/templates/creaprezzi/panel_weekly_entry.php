<?php
/**
 * Panel: Weekly Price Entry
 * Color: Orange (#FF9800)
 * Only shown when tipo_periodi == "g" (daily periods)
 */

// Build day price inputs
$day_price_rows = '';
$p_pers = mex("p",$pag);
$giorno_temp = $giorno_vedi_ini_sett;
for ($num1 = 1 ; $num1 <= 7 ; $num1++) {
    $day_label = '';
    if ($giorno_temp == 0) $day_label = mex("Dom/Lun",$pag);
    if ($giorno_temp == 1) $day_label = mex("Lun/Mar",$pag);
    if ($giorno_temp == 2) $day_label = mex("Mar/Mer",$pag);
    if ($giorno_temp == 3) $day_label = mex("Mer/Gio",$pag);
    if ($giorno_temp == 4) $day_label = mex("Gio/Ven",$pag);
    if ($giorno_temp == 5) $day_label = mex("Ven/Sab",$pag);
    if ($giorno_temp == 6) $day_label = mex("Sab/Dom",$pag);
    
    $day_price_rows .= '<tr>
        <td class="day-label">'.$day_label.'</td>
        <td>
            <input type="text" name="prezzoperiodo'.$num1.'" size="10" onfocus="document.getElementById(\'tipo_prezzo_gio\').checked=\'1\'">
        </td>
        <td>
            <input type="text" name="prezzoperiodo'.$num1.'p" size="8" onfocus="document.getElementById(\'tipo_prezzo_gio\').checked=\'1\'">
        </td>
    </tr>';
    
    $giorno_temp++;
    if ($giorno_temp == 7) $giorno_temp = 0;
}
?>

<div class="rbox" style="border-left-color: #FF9800;">
    <div class="rheader" style="background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);">
        <h5><?php echo mex("Inserzione per settimane",$pag); ?></h5>
    </div>
    <div class="rcontent">
        <form accept-charset="utf-8" method="post" action="creaprezzi.php">
            <div>
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="origine" value="<?php echo $pag; ?>">
                <input type="hidden" name="inserisci_settimanalmente" value="1">
                
                <div class="tariff-selector">
                    <label><?php echo mex("Prezzi della",$pag); ?>:</label>
                    <select name="tipotariffa">
                        <?php echo str_replace("tariffa".fixset($tariffa_selected)."\">","tariffa".fixset($tariffa_selected)."\" selected>",$lista_opt_tariffe_cambia); ?>
                    </select>
                </div>
                
                <div class="date-range-selector">
                    <label><?php echo mex("Settimane dal",$pag); ?>:</label>
                    <select name="inizioperiodosett1" id="id_sdm149" onChange="update_selected_dates('149')">
                        <?php echo $option_domeniche1; ?>
                    </select>
                    <label><?php echo mex("al",$pag); ?>:</label>
                    <select name="fineperiodosett1" id="id_sdm150" onChange="update_selected_dates('150')">
                        <?php echo $option_domeniche2; ?>
                    </select>
                </div>
                
                <div class="price-options">
                    <div class="weekly-price-option">
                        <label>
                            <input name="tipo_prezzo" value="sett" id="tipo_prezzo_sett" <?php echo $checked_sett; ?> type="radio">
                            <?php echo mex("Prezzo dell'intera settimana",$pag); ?>
                        </label>
                        <table class="weekly-price-table">
                            <thead>
                                <tr>
                                    <th><?php echo mex("Periodo",$pag); ?></th>
                                    <th><?php echo mex("Prezzo",$pag); ?> (<?php echo $Euro; ?>)</th>
                                    <th><?php echo mex("Per persona",$pag); ?> (<?php echo $Euro; ?>)</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td><?php echo mex("Settimana",$pag); ?></td>
                                    <td>
                                        <input type="text" name="prezzosett" size="12" onfocus="document.getElementById('tipo_prezzo_sett').checked='1'">
                                    </td>
                                    <td>
                                        <input type="text" name="prezzosettp" size="10" onfocus="document.getElementById('tipo_prezzo_sett').checked='1'">
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    
                    <div class="daily-prices-option">
                        <label>
                            <input name="tipo_prezzo" value="gio" id="tipo_prezzo_gio" <?php echo $checked_gio; ?> type="radio">
                            <?php echo mex("Prezzi dei giorni",$pag); ?>
                        </label>
                        <table class="daily-prices-table">
                            <thead>
                                <tr>
                                    <th><?php echo mex("Giorno",$pag); ?></th>
                                    <th><?php echo mex("Prezzo",$pag); ?> (<?php echo $Euro; ?>)</th>
                                    <th><?php echo mex("Per persona",$pag); ?> (<?php echo $Euro; ?>)</th>
                                </tr>
                            </thead>
                            <tbody>
                                <?php echo $day_price_rows; ?>
                            </tbody>
                        </table>
                    </div>
                </div>
                
                <div class="form-actions">
                    <button class="ipri" type="submit">
                        <div><?php echo mex("inserisci o modifica i prezzi",$pag); ?></div>
                    </button>
                </div>
            </div>
        </form>
    </div>
</div>

<style>
.tariff-selector {
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    gap: 10px;
}

.tariff-selector label {
    font-weight: 500;
}

.tariff-selector select {
    flex: 1;
    max-width: 300px;
    padding: 6px 10px;
    border: 1px solid #ddd;
    border-radius: 3px;
}

.date-range-selector {
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    gap: 10px;
    flex-wrap: wrap;
}

.date-range-selector label {
    font-weight: 500;
}

.date-range-selector select {
    padding: 6px 10px;
    border: 1px solid #ddd;
    border-radius: 3px;
}

.price-options {
    background: #f9f9f9;
    padding: 15px;
    border-radius: 4px;
    margin-bottom: 20px;
}

.weekly-price-option,
.daily-prices-option {
    margin-bottom: 20px;
}

.weekly-price-option:last-child,
.daily-prices-option:last-child {
    margin-bottom: 0;
}

.weekly-price-option > label,
.daily-prices-option > label {
    display: flex;
    align-items: center;
    gap: 8px;
    font-weight: 600;
    margin-bottom: 10px;
}

.weekly-price-table,
.daily-prices-table {
    width: 100%;
    border-collapse: collapse;
    margin-left: 24px;
    margin-top: 10px;
    max-width: 600px;
}

.weekly-price-table thead th,
.daily-prices-table thead th {
    background: #f5f5f5;
    padding: 10px;
    text-align: left;
    font-weight: 600;
    border-bottom: 2px solid #ddd;
}

.weekly-price-table tbody td,
.daily-prices-table tbody td {
    padding: 8px 10px;
    border-bottom: 1px solid #eee;
    vertical-align: middle;
}

.weekly-price-table .day-label,
.daily-prices-table .day-label {
    font-weight: 500;
}

.weekly-price-table input[type="text"],
.daily-prices-table input[type="text"] {
    padding: 6px 8px;
    border: 1px solid #ddd;
    border-radius: 3px;
}

.form-actions {
    text-align: left;
    margin-top: 10px;
}
</style>
