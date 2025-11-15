<?php
// Template: Income Entry Panel (Entrate)
// Color: Indigo (#3F51B5)
if (!defined('C_DATI_PATH')) die();
?>
<div class="rbox" style="background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-left: 4px solid #3F51B5; margin: 0 0 16px 0; flex: 1 1 420px; min-width: 320px;">
    <div style="background: linear-gradient(135deg, #3F51B5 0%, #5C6BC0 100%); color: white; padding: 10px 14px; border-radius: 6px; margin: -5px 0 12px 0; font-weight: 600;">
        <?php echo mex("Inserisci le entrate in cassa per l'anno",$pag)." ".$anno; ?>.
    </div>
    
    <form accept-charset="utf-8" method="post" action="costi.php">
        <div>
            <input type="hidden" name="anno" value="<?php echo $anno; ?>">
            <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
            
            <span class="wsnw">
                <?php echo mex("Cassa",$pag); ?>: 
                <?php 
                if ($num_casse_attive == 1) {
                    echo $hidden_cassa;
                } else {
                    echo "<select name=\"id_cassa\">".$opt_casse."</select>";
                }
                ?>
            </span>
            
            <span class="wsnw">
                <?php echo mex("Natura entrata",$pag); ?>: 
                <input type="text" name="nome_costo" size="30" value="<?php echo isset($nome_costo) ? htmlspecialchars(stripslashes($nome_costo)) : ''; ?>">
            </span>
            
            <span class="wsnw">
                <?php echo mex("Importo",$pag); ?>: 
                <input type="text" name="val_costo" size="10" value="<?php echo isset($val_costo) ? htmlspecialchars($val_costo) : ''; ?>"> <?php echo $valute_txt; ?>.
            </span>
            
            <?php if (!empty($metodo_pagamento_txt)) { ?>
            <span class="wsnw">
                <?php echo str_replace('<tr><td>', '', str_replace('</td></tr>', '', $metodo_pagamento_txt)); ?>
            </span>
            <?php } ?>
            
            <?php if (!empty($data_txt)) { ?>
            <span class="wsnw">
                <?php echo str_replace('<tr><td>', '', str_replace('</td></tr>', '', $data_txt)); ?>
            </span>
            <?php } ?>
            
            <?php if ($priv_persona_ins_costi == "c") { ?>
            <span class="wsnw">
                <?php echo mex("Persona che inserisce",$pag); ?>: 
                <input type="text" name="persona_costo" size="20" value=""> (<?php echo mex("opzionale",$pag); ?>).
            </span>
            <?php } ?>
            
            <?php if ($priv_entrate_da_prenota == "c") { ?>
            <span class="wsnw">
                <label>
                    <input type="checkbox" name="entrata_da_prenota" value="SI" checked>
                    <?php echo mex("Sottrai l'importo dal totale delle prenotazioni",$pag); ?>.
                </label>
            </span>
            <?php } ?>
            <div style="text-align: center;">
                <button class="iinc" type="submit" name="inserisci_entrata" value="1">
                    <div><?php echo mex("Inserisci l' entrata",$pag); ?></div>
                </button><br>
            </div>
        </div>
    </form>
</div>
