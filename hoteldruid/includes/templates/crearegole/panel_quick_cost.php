<?php
/**
 * Panel: Quick Insert Additional Cost
 * Redesigned with label-value pair layout
 */
?>

<style>
.rbox.quick-cost {
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    margin-bottom: 20px;
    overflow: hidden;
}

.rbox.quick-cost .rheader {
    background: linear-gradient(135deg, #673AB7 0%, #512DA8 100%);
    padding: 15px 20px;
    font-size: 18px;
    font-weight: bold;
    color: white;
}

.rbox.quick-cost .rcontent {
    padding: 20px;
    background: white;
}

.form-row {
    display: flex;
    align-items: center;
    margin-bottom: 12px;
    gap: 15px;
}

.form-row label {
    min-width: 200px;
    font-weight: 500;
    color: #424242;
    flex-shrink: 0;
}

.form-row select {
    flex: 1;
    padding: 8px 12px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 14px;
    max-width: 400px;
}

.form-actions {
    margin-top: 20px;
    padding-top: 15px;
    border-top: 1px solid #f0f0f0;
    text-align: center;
}

.form-actions button {
    padding: 10px 24px;
    border-radius: 4px;
    font-size: 14px;
    cursor: pointer;
}
</style>

<div class="rbox quick-cost">
    <div class="rheader">
        <?php echo mex("Inserimento rapido costo aggiuntivo",$pag); ?>
    </div>
    <div class="rcontent">
        <?php
        // Display feedback messages if this panel is active
        if (isset($active_panel) && $active_panel === 'panel_quick_cost') {
            if (class_exists('HotelDruidTemplate')) {
                HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
            }
        }
        ?>
        
        <form accept-charset="utf-8" method="post" action="creaprezzi.php">
            <input type="hidden" name="anno" value="<?php echo $anno; ?>">
            <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
            <input type="hidden" name="ins_rapido_costo" value="SI">
            <input type="hidden" name="origine" value="crearegole.php">
            
            <div class="form-row">
                <label><?php echo mex("Tipo di costo aggiuntivo",'creaprezzi.php'); ?>:</label>
                <select name="tipocostoagg">
                    <option value="perm_min"><?php echo mex("permanenza minima",'creaprezzi.php'); ?></option>
                    <option value="num_bamb"><?php echo mex("numero di neonati",'creaprezzi.php'); ?></option>
                    <option value="letto_agg"><?php echo mex("letto aggiuntivo",'creaprezzi.php'); ?></option>
                    <option value="off_spec"><?php echo mex("offerta speciale",'creaprezzi.php'); ?></option>
                </select>
            </div>
            
            <div class="form-actions">
                <button class="aexc" type="submit">
                    <div><?php echo mex("inserisci",'creaprezzi.php'); ?></div>
                </button>
            </div>
        </form>
    </div>
</div>
