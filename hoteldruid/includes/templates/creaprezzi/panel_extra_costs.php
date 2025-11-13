<?php
/**
 * Panel: Extra Costs
 * Color: Teal (#009688)
 */
?>

<div class="rbox" style="border-left-color: #009688;">
    <div class="rheader" style="background: linear-gradient(135deg, #009688 0%, #00796B 100%);">
        <h5><?php echo mex("Costi aggiuntivi",$pag); ?></h5>
    </div>
    <div class="rcontent">
        <?php if (!isset($num_costi_max)) { ?>
            <!-- New Extra Cost Form -->
            <form accept-charset="utf-8" method="post" action="creaprezzi.php">
                <div>
                    <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                    <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                    <input type="hidden" name="passo" value="1">
                    <input type="hidden" name="avanti" value="SI">
                    <input type="hidden" name="inseriscicosti" value="1">
                    <input type="hidden" name="origine" value="<?php echo $pag; ?>">
                    
                    <div class="form-section">
                        <label>
                            <?php echo mex("Nome del nuovo costo aggiuntivo",$pag); ?>:
                            <input type="text" name="nomecostoagg" size="24" value="<?php echo htmlspecialchars(fixset($nomecostoagg)); ?>">
                        </label>
                    </div>
                    
                    <div class="form-section">
                        <label>
                            <?php echo mex("Categoria",$pag); ?>:
                            <input type="text" name="categoria_ca" value="<?php echo htmlspecialchars(fixstr($categoria_ca)); ?>" size="18">
                            <span class="hint">(<?php echo mex("opzionale",$pag); ?>)</span>
                        </label>
                    </div>
                    
                    <div class="form-section">
                        <label>
                            <?php echo mex("Tipo di costo aggiuntivo",$pag); ?>:
                            <select name="tipo_ca">
                                <option value="u"<?php echo (isset($tipo_ca) and $tipo_ca == "u") ? " selected" : ""; ?>><?php echo mex("unico",$pag); ?></option>
                                <option value="s"<?php echo (isset($tipo_ca) and $tipo_ca == "s") ? " selected" : ""; ?>><?php echo mex("$parola_settimanale",$pag); ?></option>
                            </select>
                        </label>
                    </div>
                    
                    <div class="form-actions">
                        <button class="aexc" type="submit">
                            <div><?php echo mex("Procedi nell'inserimento del nuovo costo aggiuntivo",$pag); ?></div>
                        </button>
                    </div>
                </div>
            </form>
            
            <div class="divider"></div>
            
            <?php if ($opt_costi_agg) { ?>
                <!-- Import Cost Form -->
                <form accept-charset="utf-8" method="post" action="creaprezzi.php">
                    <div>
                        <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                        <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                        <input type="hidden" name="importa_costo" value="1">
                        <input type="hidden" name="origine" value="<?php echo $pag; ?>">
                        
                        <div class="form-inline">
                            <?php echo mex("Inserisci un nuovo costo aggiuntivo chiamato",$pag); ?>
                            <input type="text" name="nomecostoagg" size="20">
                            <?php echo mex("importando le caratteristiche da",$pag); ?>
                            <select name="costo_importa">
                                <?php echo $opt_costi_agg; ?>
                            </select>
                            <button class="xexc" type="submit">
                                <div><?php echo mex("importa",$pag); ?></div>
                            </button>
                        </div>
                    </div>
                </form>
                
                <div class="divider"></div>
            <?php } ?>
            
            <!-- Quick Insert Form -->
            <form accept-charset="utf-8" method="post" action="creaprezzi.php" id="ins_rap_c">
                <div>
                    <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                    <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                    <input type="hidden" name="ins_rapido_costo" value="1">
                    <input type="hidden" name="origine" value="<?php echo $pag; ?>">
                    
                    <div class="form-inline">
                        <?php echo mex("Inserimento rapido di un nuovo costo aggiuntivo per",$pag); ?>
                        <select name="tipocostoagg">
                            <option value="perm_min"><?php echo mex("permanenza minima",$pag); ?></option>
                            <option value="num_bamb"><?php echo mex("numero di neonati",$pag); ?></option>
                            <option value="letto_agg"><?php echo mex("letto aggiuntivo",$pag); ?></option>
                            <option value="off_spec"><?php echo mex("offerta speciale",$pag); ?></option>
                        </select>
                        <button class="aexc" type="submit">
                            <div><?php echo mex("inserisci",$pag); ?></div>
                        </button>
                    </div>
                </div>
            </form>
        <?php } else { ?>
            <div class="max-costs-message">
                <?php echo mex("Hai raggiunto il numero massimo di costi aggiuntivi consentiti",$pag); ?>
            </div>
        <?php } ?>
    </div>
</div>

<style>
.form-section {
    margin-bottom: 15px;
}

.form-section label {
    display: flex;
    flex-direction: column;
    gap: 5px;
}

.form-section input[type="text"],
.form-section select {
    padding: 6px 10px;
    border: 1px solid #ddd;
    border-radius: 3px;
}

.hint {
    font-size: 0.9em;
    color: #666;
    font-style: italic;
}

.form-actions {
    text-align: center;
    margin-top: 20px;
}

.divider {
    height: 1px;
    background: #e0e0e0;
    margin: 20px 0;
}

.form-inline {
    display: flex;
    align-items: center;
    gap: 8px;
    flex-wrap: wrap;
}

.form-inline input[type="text"],
.form-inline select {
    padding: 6px 10px;
    border: 1px solid #ddd;
    border-radius: 3px;
}

.max-costs-message {
    text-align: center;
    padding: 20px;
    background: #fff3cd;
    border-radius: 4px;
    color: #856404;
}
</style>
