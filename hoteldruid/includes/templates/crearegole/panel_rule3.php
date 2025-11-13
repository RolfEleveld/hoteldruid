<?php
/**
 * Panel: Rule 3 - Number of Persons Auto-Assignment
 * Redesigned with label-value pair layout
 */
?>

<style>
.rbox.rule3 {
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    margin-bottom: 20px;
    overflow: hidden;
}

.rbox.rule3 .rheader {
    background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
    padding: 15px 20px;
    font-size: 18px;
    font-weight: bold;
    color: white;
}

.rbox.rule3 .rcontent {
    padding: 20px;
    background: white;
}

.rule3-section {
    margin-bottom: 25px;
    padding-bottom: 20px;
    border-bottom: 1px solid #e0e0e0;
}

.rule3-section:last-child {
    margin-bottom: 0;
    border-bottom: none;
    padding-bottom: 0;
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

.form-row select,
.form-row input[type="text"] {
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

<script type="text/javascript">
<!--
function aggiorna_val_reg3 () {
var sel_tar = document.getElementById("t3");
var num_sel = sel_tar.selectedIndex;
var tariffa = sel_tar.options[num_sel].value;
var txt_box = document.getElementById("v3");
if (tariffa == '') txt_box.disabled = true;
else txt_box.disabled = false;
txt_box.value = "";
<?php echo $lista_tar_reg3; ?>
}
function aggiorna_val_reg3m () {
var sel_tar = document.getElementById("t3m");
var num_sel = sel_tar.selectedIndex;
var tariffa = sel_tar.options[num_sel].value;
var txt_box = document.getElementById("v3m");
if (tariffa == '') txt_box.disabled = true;
else txt_box.disabled = false;
txt_box.value = "";
<?php echo $lista_tar_reg3min; ?>
}
-->
</script>

<div class="rbox rule3">
    <div class="rheader">
        <?php echo mex("Regola di assegnazione",$pag); ?> 3 (<?php echo mex("numero di persone",$pag); ?>)
    </div>
    <div class="rcontent">
        
            <div class="rcontent">
        
        <?php 
        // Display messages at the top of the panel (only for this panel)
        if ((!empty($success_messages) or !empty($error_messages) or !empty($warning_messages)) and (!empty($active_panel) and $active_panel == 'rule3')) {
            HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
        }
        ?>
        
        <?php if ($priv_mod_reg2 == "s" and $numero_tariffe >= 1): ?>
        <!-- Auto-assign Number of Persons Form -->
        <div class="rule3-section">
            <form accept-charset="utf-8" method="post" action="crearegole.php">
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="inserisci" value="SI">
                <?php if ($origine): ?>
                    <input type="hidden" name="origine" value="<?php echo htmlspecialchars($origine); ?>">
                <?php endif; ?>
                
                <div class="form-row">
                    <label><?php echo mex("Quando si sceglie la tariffa",$pag); ?>:</label>
                    <select name="tipotariffa" id="t3" onchange="aggiorna_val_reg3()">
                        <option value="" selected>----</option>
                        <?php echo $lista_tariffe3; ?>
                    </select>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("Numero di persone",$pag); ?>:</label>
                    <input type="text" id="v3" name="num_persone" size="5" disabled>
                </div>
                
                <div style="font-size: 13px; color: #666; margin-left: 215px; margin-top: -8px; margin-bottom: 12px;">
                    <?php echo mex("se non si inserisce nessun altro numero",$pag); ?>
                </div>
                
                <div class="form-actions">
                    <button class="irul" type="submit" name="regola_3" value="1">
                        <div><?php echo mex("Inserisci o modifica la regola",$pag); ?> 3</div>
                    </button>
                </div>
            </form>
        </div>
        <?php endif; ?>
        
        <?php if (empty($tipotariffa_regola3) or empty($nor3m)): ?>
        <!-- Minimum Number of Persons Form -->
        <div class="rule3-section">
            <form accept-charset="utf-8" method="post" action="crearegole.php">
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="inserisci" value="SI">
                <?php if ($origine): ?>
                    <input type="hidden" name="origine" value="<?php echo htmlspecialchars($origine); ?>">
                <?php endif; ?>
                
                <div class="form-row">
                    <label><?php echo mex("Quando si sceglie la tariffa",$pag); ?>:</label>
                    <select name="tipotariffa" id="t3m" onchange="aggiorna_val_reg3m()">
                        <option value="" selected>----</option>
                        <?php echo $lista_tariffe3; ?>
                    </select>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("Minimo persone",$pag); ?>:</label>
                    <input type="text" id="v3m" name="num_persone" size="5" disabled>
                </div>
                
                <div style="font-size: 13px; color: #666; margin-left: 215px; margin-top: -8px; margin-bottom: 12px;">
                    <?php echo mex("non permettere l'inserimento di prenotazioni con meno di",$pag); ?> N <?php echo mex("persone",$pag); ?>
                </div>
                
                <div class="form-actions">
                    <button class="irul" type="submit" name="regola_3m" value="1">
                        <div><?php echo mex("Inserisci o modifica la regola",$pag); ?> 3</div>
                    </button>
                </div>
            </form>
        </div>
        <?php endif; ?>
        
        <?php if (!empty($tipotariffa_regola3) and preg_replace("/tariffa[0-9]+/","",$tipotariffa_regola3) == ""): ?>
        <script type="text/javascript">
        <!--
        <?php if (empty($nor3)): ?>
        var sel_tr3 = document.getElementById("t3");
        sel_tr3.value = '<?php echo $tipotariffa_regola3; ?>';
        aggiorna_val_reg3();
        <?php endif; ?>
        <?php if (empty($nor3m)): ?>
        var sel_tr3m = document.getElementById("t3m");
        sel_tr3m.value = '<?php echo $tipotariffa_regola3; ?>';
        aggiorna_val_reg3m();
        <?php endif; ?>
        -->
        </script>
        <?php endif; ?>
        
    </div>
</div>
