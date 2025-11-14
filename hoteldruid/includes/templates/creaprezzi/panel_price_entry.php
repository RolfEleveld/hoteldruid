<?php
/**
 * Panel: Price Entry by Periods
 * Color: Blue (#2196F3)
 */

// Build the table rows
$price_rows = '';
$p_pers = mex("p",$pag);
for ($numperiodo = 1 ; $numperiodo <= $numcaselle ; $numperiodo++) {
    $inizioperiodo_name = "inizioperiodo".$numperiodo;
    $fineperiodo_name = "fineperiodo".$numperiodo;
    $prezzoperiodo_name = "prezzoperiodo".$numperiodo;
    $prezzoperiodop_name = "prezzoperiodo".$numperiodo."p";
    
    // Date selection for start
    $date_selected_inizio = "";
    if (isset($ultime_sel_ins_prezzi[1]) and $ultime_sel_ins_prezzi[1] == $anno) 
        $date_selected_inizio = $ultime_sel_ins_prezzi[($numperiodo * 2)];
    else 
        $date_selected_inizio = fixset($$inizioperiodo_name);
    
    // Date selection for end
    $date_selected_fine = "";
    if (isset($ultime_sel_ins_prezzi[1]) and $ultime_sel_ins_prezzi[1] == $anno) 
        $date_selected_fine = $ultime_sel_ins_prezzi[(($numperiodo * 2) + 1)];
    else 
        $date_selected_fine = fixset($$fineperiodo_name);
    
    // Start building row HTML
    ob_start();
    ?>
    <tr>
        <td class="row-number"><?php echo $numperiodo; ?></td>
        <td class="date-from">
            <?php mostra_menu_date(C_DATI_PATH."/selperiodimenu$anno.$id_utente.php",$inizioperiodo_name,$date_selected_inizio,"","",$id_utente,$tema); ?>
        </td>
        <td class="date-to">
            <?php mostra_menu_date(C_DATI_PATH."/selperiodimenu$anno.$id_utente.php",$fineperiodo_name,$date_selected_fine,"","",$id_utente,$tema); ?>
        </td>
        <td class="price-period">
            <input type="text" 
                   name="<?php echo $prezzoperiodo_name; ?>" 
                   value="<?php echo htmlspecialchars(fixstr($$prezzoperiodo_name)); ?>" 
                   size="10">
        </td>
        <td class="price-person">
            <input type="text" 
                   name="<?php echo $prezzoperiodop_name; ?>" 
                   value="<?php echo htmlspecialchars(fixstr(${$prezzoperiodop_name})); ?>" 
                   size="8">
        </td>
        <td id="minus<?php echo $numperiodo; ?>" class="action-minus">
            <?php if ($numperiodo == $numcaselle and $numcaselle > 1): ?>
                <button class="sbutton remove-btn" type="submit" name="elimina_casella" onclick="return elim_lin_tar();" title="<?php echo mex("Rimuovi riga",$pag); ?>">-</button>
            <?php endif; ?>
        </td>
        <td id="plus<?php echo $numperiodo; ?>" class="action-plus">
            <?php if ($numperiodo == $numcaselle and $numcaselle < $numcaselle_max): ?>
                <button class="sbutton add-btn" type="submit" name="aggiungi_casella" onclick="return agg_lin_tar();" title="<?php echo mex("Aggiungi riga",$pag); ?>">+</button>
            <?php endif; ?>
        </td>
    </tr>
    <?php
    $price_rows .= ob_get_clean();
}
?>

<div class="rbox" style="border-left-color: #2196F3;">
    <div class="rheader" style="background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);">
        <h5><?php echo mex("Inserzione per $parola_settimane",$pag); ?></h5>
    </div>
    <div class="rcontent">
        <?php
        // Display feedback messages if this panel is active
        if (isset($active_panel) && $active_panel === 'panel_price_entry') {
            if (class_exists('HotelDruidTemplate')) {
                HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
            }
        }
        ?>
        
        <form accept-charset="utf-8" method="post" action="<?php echo $pag; ?>">
            <div>
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="origine" value="<?php echo $pag; ?>">
                <input type="hidden" name="numcaselle" id="numcaselle" value="<?php echo $numcaselle; ?>">
                <input type="hidden" name="modifica" value="1">
                
                <div class="tariff-selector">
                    <label><?php echo mex("Prezzi della",$pag); ?>:</label>
                    <select name="tipotariffa">
                        <?php echo $lista_opt_tariffe_cambia; ?>
                    </select>
                </div>
                
                <table class="price-entry-table" id="tab_prezzi">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th><?php echo mex("Dal",$pag); ?></th>
                            <th><?php echo mex("Al",$pag); ?></th>
                            <th><?php echo mex("Prezzo per $parola_settimana",$pag)." (".$Euro.")"; ?></th>
                            <th><?php echo mex("Prezzo per persona",$pag)." (".$Euro.")"; ?></th>
                            <th colspan="2"><?php echo mex("Azioni",$pag); ?></th>
                        </tr>
                    </thead>
                    <tbody>
                        <?php echo $price_rows; ?>
                    </tbody>
                </table>
                
                <div class="form-actions">
                    <button class="edit" type="submit">
                        <div><?php echo mex("Inserisci i prezzi",$pag); ?></div>
                    </button>
                </div>
            </div>
        </form>
    </div>
</div>

<script type="text/javascript">
<!--
var numcaselle = <?php echo $numcaselle; ?>;
var numcaselle_max = <?php echo $numcaselle_max; ?>;

function agg_lin_tar() {
    if (numcaselle < numcaselle_max) {
        var tab_prezzi = document.getElementById('tab_prezzi').getElementsByTagName('tbody')[0];
        var minus_prec = document.getElementById('minus'+numcaselle);
        var plus_prec = document.getElementById('plus'+numcaselle);
        numcaselle++;
        
        var nlinea = tab_prezzi.insertRow(-1);
        
        var cella = nlinea.insertCell(0);
        cella.className = 'row-number';
        cella.innerHTML = numcaselle;
        
        cella = nlinea.insertCell(1);
        cella.className = 'date-from';
        var cell_html = '';
        <?php 
        ob_start();
        mostra_menu_date(C_DATI_PATH."/selperiodimenu$anno.$id_utente.php","inizioperiodo","","","",$id_utente,$tema,"","","cell_html");
        $date_picker_code = ob_get_clean();
        echo $date_picker_code;
        ?>
        cella.innerHTML = cell_html.replace(/inizioperiodo/g, 'inizioperiodo'+numcaselle);
        
        cella = nlinea.insertCell(2);
        cella.className = 'date-to';
        cell_html = '';
        <?php 
        ob_start();
        mostra_menu_date(C_DATI_PATH."/selperiodimenu$anno.$id_utente.php","fineperiodo","","","",$id_utente,$tema,"","","cell_html");
        $date_picker_code = ob_get_clean();
        echo $date_picker_code;
        ?>
        cella.innerHTML = cell_html.replace(/fineperiodo/g, 'fineperiodo'+numcaselle);
        
        cella = nlinea.insertCell(3);
        cella.className = 'price-period';
        cella.innerHTML = '<input type="text" name="prezzoperiodo'+numcaselle+'" value="" size="12">';
        
        cella = nlinea.insertCell(4);
        cella.className = 'price-person';
        cella.innerHTML = '<input type="text" name="prezzoperiodo'+numcaselle+'p" value="" size="10"><span class="multiplier">*<?php echo $p_pers; ?></span>';
        
        cella = nlinea.insertCell(5);
        cella.id = 'minus'+numcaselle;
        cella.className = 'action-minus';
        cella.innerHTML = minus_prec.innerHTML;
        minus_prec.innerHTML = '';
        
        cella = nlinea.insertCell(6);
        cella.id = 'plus'+numcaselle;
        cella.className = 'action-plus';
        cella.innerHTML = plus_prec.innerHTML;
        plus_prec.innerHTML = '';
        
        document.getElementById('numcaselle').value = numcaselle;
    }
    return false;
}

function elim_lin_tar() {
    if (numcaselle > 1) {
        var tab_prezzi = document.getElementById('tab_prezzi').getElementsByTagName('tbody')[0];
        var minus_prec = document.getElementById('minus'+numcaselle);
        var plus_prec = document.getElementById('plus'+numcaselle);
        numcaselle--;
        
        var cella = document.getElementById('minus'+numcaselle);
        cella.innerHTML = minus_prec.innerHTML;
        cella = document.getElementById('plus'+numcaselle);
        cella.innerHTML = plus_prec.innerHTML;
        
        tab_prezzi.deleteRow(-1);
        document.getElementById('numcaselle').value = numcaselle;
    }
    return false;
}
-->
</script>

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
}

.price-entry-table {
    width: 100%;
    border-collapse: collapse;
    margin: 20px 0;
}

.price-entry-table thead th {
    background: #f5f5f5;
    padding: 12px 8px;
    text-align: left;
    font-weight: 600;
    border-bottom: 2px solid #ddd;
    font-size: 13px;
}

.price-entry-table tbody td {
    padding: 10px 8px;
    border-bottom: 1px solid #eee;
    vertical-align: middle;
}

.price-entry-table .row-number {
    width: 40px;
    font-weight: 500;
    text-align: center;
}

.price-entry-table .date-from,
.price-entry-table .date-to {
    white-space: nowrap;
}

.price-entry-table .price-period input,
.price-entry-table .price-person input {
    width: 100%;
}

.price-entry-table .multiplier {
    margin-left: 5px;
    color: #666;
}

.price-entry-table .action-minus,
.price-entry-table .action-plus {
    width: 40px;
    text-align: center;
}

.price-entry-table .sbutton {
    width: 30px;
    height: 30px;
    border: 1px solid #ddd;
    background: white;
    border-radius: 4px;
    cursor: pointer;
    font-weight: bold;
    transition: all 0.2s;
}

.price-entry-table .sbutton.add-btn {
    color: #4CAF50;
    border-color: #4CAF50;
}

.price-entry-table .sbutton.add-btn:hover {
    background: #4CAF50;
    color: white;
}

.price-entry-table .sbutton.remove-btn {
    color: #f44336;
    border-color: #f44336;
}

.price-entry-table .sbutton.remove-btn:hover {
    background: #f44336;
    color: white;
}

.form-actions {
    text-align: right;
    margin-top: 15px;
}
</style>
