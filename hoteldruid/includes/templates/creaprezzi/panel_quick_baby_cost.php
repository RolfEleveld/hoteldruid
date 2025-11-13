<?php
/**
 * Panel: Quick Baby/Infant Cost Insert
 * Redesigned with panel layout for inline feedback
 */
?>

<style>
.rbox.baby-cost {
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    margin-bottom: 20px;
    overflow: hidden;
}

.rbox.baby-cost .rheader {
    background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
    padding: 15px 20px;
    font-size: 18px;
    font-weight: bold;
    color: white;
}

.rbox.baby-cost .rcontent {
    padding: 20px;
    background: white;
}

.baby-cost-section {
    margin-bottom: 20px;
}

.form-row {
    margin-bottom: 15px;
}

.form-row label {
    display: block;
    font-weight: 500;
    margin-bottom: 5px;
    color: #424242;
}

.form-row input[type="text"],
.form-row select {
    padding: 8px 12px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 14px;
}

.form-row input[type="radio"] {
    margin-right: 5px;
}

.radio-group {
    margin-left: 20px;
}

.radio-option {
    margin-bottom: 8px;
}

.help-text {
    font-size: 13px;
    color: #666;
    font-style: italic;
    margin-top: 5px;
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
    background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
    color: white;
    border: none;
}

.form-actions button:hover {
    background: linear-gradient(135deg, #FB8C00 0%, #EF6C00 100%);
}

.limit-section table {
    width: 100%;
}

.limit-section td {
    vertical-align: top;
    padding: 5px;
}

.error-indicator {
    color: #d32f2f;
    font-weight: bold;
    margin-right: 5px;
}
</style>

<div class="rbox baby-cost">
    <div class="rheader">
        <?php echo ucfirst($nomecostoagg_orig); ?>
    </div>
    <div class="rcontent">
        
        <?php 
        // Display messages at the top of the panel - only if this panel was active
        if ((!empty($success_messages) or !empty($error_messages) or !empty($warning_messages)) and (!empty($active_panel) and $active_panel == 'quick_baby_cost')) {
            HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
        }
        ?>
        
        <?php if ($tipocostoagg == "num_bamb"): ?>
            <div class="help-text" style="text-align: center; margin-bottom: 20px;">
                (<?php echo mex("con questo costo il numero di neonati non è incluso nel numero di persone",$pag); ?>)
            </div>
        <?php endif; ?>
        
        <form accept-charset="utf-8" method="post" action="creaprezzi.php">
            <input type="hidden" name="anno" value="<?php echo $anno; ?>">
            <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
            <input type="hidden" name="ins_rapido_costo" value="1">
            <input type="hidden" name="tipocostoagg" value="<?php echo $tipocostoagg; ?>">
            <input type="hidden" name="inseriscicosti" value="1">
            <input type="hidden" name="origine" value="<?php echo htmlspecialchars(fixstr($origine)); ?>">
            
            <div class="baby-cost-section">
                <div class="form-row">
                    <?php if (!empty($nom_err)): ?>
                        <span class="error-indicator">&gt;</span>
                    <?php endif; ?>
                    <label for="nomecostoagg">
                        <?php echo mex("Nome del nuovo costo aggiuntivo",$pag); ?>:
                    </label>
                    <input type="text" id="nomecostoagg" name="nomecostoagg" size="24" value="<?php echo htmlspecialchars($nomecostoagg); ?>">
                </div>

                <div class="form-row">
                    <?php if (!empty($val_err)): ?>
                        <span class="error-indicator">&gt;</span>
                    <?php endif; ?>
                    <label><?php echo mex("Valore per ogni neonato",$pag); ?>:</label>
                    
                    <div class="radio-group">
                        <div class="radio-option">
                            <label>
                                <input type="radio" id="tsb_f" name="tipo_val_rapido" value="f"<?php echo (!isset($tipo_val_rapido) or ($tipo_val_rapido != "t" and $tipo_val_rapido != "p")) ? " checked" : ""; ?>>
                                <?php echo mex("fisso di",$pag); ?>
                                <input type="text" name="valore_f_ca" value="<?php echo htmlspecialchars($valore_f_ca); ?>" size="12" onclick="document.getElementById('tsb_f').checked='1'">
                                <?php echo $Euro." ".mex("al giorno",$pag); ?>
                            </label>
                        </div>
                        
                        <div class="radio-option">
                            <label>
                                <input type="radio" id="tsb_t" name="tipo_val_rapido" value="t"<?php echo (isset($tipo_val_rapido) and $tipo_val_rapido == "t") ? " checked" : ""; ?>>
                                <input type="text" name="valore_p_ca_t" value="<?php echo htmlspecialchars($valore_p_ca_t); ?>" size="4" onclick="document.getElementById('tsb_t').checked='1'">
                                % <?php echo mex("della tariffa",$pag)." ".mex("arrotondato a",$pag); ?>
                                <input type="text" name="arrotonda_ca_t" value="<?php echo htmlspecialchars($arrotonda_ca_t); ?>" size="6" onclick="document.getElementById('tsb_t').checked='1'">
                                <?php echo $Euro; ?>
                            </label>
                        </div>
                        
                        <div class="radio-option">
                            <label>
                                <input type="radio" id="tsb_p" name="tipo_val_rapido" value="p"<?php echo (isset($tipo_val_rapido) and $tipo_val_rapido == "p") ? " checked" : ""; ?>>
                                <input type="text" name="valore_p_ca_p" value="<?php echo htmlspecialchars($valore_p_ca_p); ?>" size="4" onclick="document.getElementById('tsb_p').checked='1'">
                                % <?php echo mex("del prezzo di una persona",$pag)." ".mex("arrotondato a",$pag); ?>
                                <input type="text" name="arrotonda_ca_p" value="<?php echo htmlspecialchars($arrotonda_ca_p); ?>" size="6" onclick="document.getElementById('tsb_p').checked='1'">
                                <?php echo $Euro; ?>
                            </label>
                            <div class="help-text">
                                (<?php echo mex("solo per tariffe con prezzi a persona",$pag); ?>)
                            </div>
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <?php if (!empty($tas_err)): ?>
                        <span class="error-indicator">&gt;</span>
                    <?php endif; ?>
                    <label for="tasseperc_ca">
                        <?php echo mex("Tasse",$pag); ?>:
                    </label>
                    <input type="text" id="tasseperc_ca" name="tasseperc_ca" value="<?php echo htmlspecialchars($tasseperc_ca); ?>" size="4">%
                    <div class="help-text">
                        (<?php echo mex("il valore del costo si intente con tasse già incluse",$pag); ?>)
                    </div>
                </div>

                <div class="form-row limit-section">
                    <table>
                        <tr>
                            <td valign="top">
                                <?php if (!empty($lim_err)): ?>
                                    <span class="error-indicator">&gt;</span>
                                <?php endif; ?>
                                <?php echo mex("Limitarne il numero che è possibile avere contemporaneamente in uno stesso periodo",$pag); ?>?<br>
                                &nbsp;&nbsp;<small>(<?php echo mex("limite non considerato per le persone aggiuntive nelle interconnessioni",$pag); ?>)</small>
                            </td>
                            <td style="width: 130px;">
                                <label>
                                    <input type="radio" name="limite_ca" value="n"<?php echo (empty($limite_ca) or $limite_ca == "n") ? " checked" : ""; ?>>
                                    <?php echo mex("No",$pag); ?>
                                </label><br>
                                <label>
                                    <input type="radio" id="li_s" name="limite_ca" value="s"<?php echo (!empty($limite_ca) and $limite_ca == "s") ? " checked" : ""; ?>>
                                    <?php echo mex("Si",$pag); ?>:
                                </label>
                                <input type="text" name="numlimite_ca" value="<?php echo htmlspecialchars(fixset($numlimite_ca)); ?>" size="4" onclick="document.getElementById('li_s').checked='1';">
                            </td>
                        </tr>
                    </table>
                </div>

                <div class="form-row">
                    <?php if (!empty($moltmax_err)): ?>
                        <span class="error-indicator">&gt;</span>
                    <?php endif; ?>
                    <label for="moltmax">
                        <?php echo mex("Numero massimo",$pag)." ".mex("per appartamento",'unit.php'); ?>:
                    </label>
                    <input type="text" id="moltmax" name="moltmax" value="<?php echo htmlspecialchars(fixset($moltmax)); ?>" size="3">
                </div>
            </div>

            <div class="form-actions">
                <button type="submit">
                    <?php echo mex("Inserisci il costo aggiuntivo",$pag); ?>
                </button>
            </div>
        </form>
    </div>
</div>
