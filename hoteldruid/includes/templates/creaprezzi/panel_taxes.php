<?php
/**
 * Panel: Taxes
 * Stage 7 of creaprezzi.php refactoring
 */
?>

<style>
.rbox.taxes {
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    margin-bottom: 20px;
    overflow: hidden;
}

.rbox.taxes .rheader {
    background: linear-gradient(135deg, #FF5722 0%, #E64A19 100%);
    padding: 15px 20px;
    border-bottom: none;
    font-size: 18px;
    font-weight: bold;
    color: white;
}

.rbox.taxes .rcontent {
    padding: 20px;
    background: white;
    border-radius: 0 0 8px 8px;
}

.taxes-form-row {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 15px;
    margin-bottom: 20px;
}

.taxes-form-row select,
.taxes-form-row input[type="text"] {
    padding: 8px 12px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 14px;
}

.taxes-form-row input[type="text"] {
    width: 80px;
    text-align: center;
}

.btn-submit-taxes {
    background: #FF5722;
    color: white;
    border: none;
    padding: 12px 30px;
    font-size: 15px;
    border-radius: 5px;
    cursor: pointer;
    transition: background 0.3s;
}

.btn-submit-taxes:hover {
    background: #E64A19;
}

.taxes-note {
    text-align: center;
    color: #666;
    font-size: 13px;
    margin-top: 20px;
    font-style: italic;
}

.taxes-settings {
    margin-top: 30px;
    padding-top: 20px;
    border-top: 1px solid #E0E0E0;
}

.taxes-settings-row {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 10px;
}

.taxes-settings-row input[type="text"] {
    width: 100px;
    padding: 6px 10px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 14px;
}

.btn-change-settings {
    background: #757575;
    color: white;
    border: none;
    padding: 8px 20px;
    font-size: 14px;
    border-radius: 4px;
    cursor: pointer;
    transition: background 0.3s;
}

.btn-change-settings:hover {
    background: #616161;
}
</style>

<div class="rbox taxes">
    <div class="rheader">
        <?php echo mex("Tasse",$pag); ?>
    </div>
    <div class="rcontent">
        <?php
        // Display feedback messages if this panel is active
        if (isset($active_panel) && $active_panel === 'panel_taxes') {
            if (class_exists('HotelDruidTemplate')) {
                HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
            }
        }
        ?>
        
        <a name="mod_tas"></a>
        <form accept-charset="utf-8" method="post" action="creaprezzi.php">
            <input type="hidden" name="anno" value="<?php echo $anno; ?>">
            <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
            <input type="hidden" name="modificatasse" value="1">
            <input type="hidden" name="origine" value="<?php echo $pag; ?>">
            
            <div class="taxes-form-row">
                <label><?php echo mex("Tasse applicate su",$pag); ?></label>
                <select id="tttas" name="tipotariffa" onchange="agg_sel_tas();">
                    <?php if ($num_opt_tariffe > 1): ?>
                        <option value="tutte"><?php echo mex("tutte le tariffe",$pag); ?></option>
                    <?php endif; ?>
                    <?php echo $lista_tasse_esist; ?>
                    <?php echo $lista_opt_tariffe; ?>
                </select>
                <span>:</span>
                <input type="text" id="tas_perc" name="tasse_percent" size="3" maxlength="6">
                <span>%</span>
                <button class="btn-submit-taxes" type="submit">
                    <?php echo mex("Inserisci o modifica le tasse",$pag); ?>
                </button>
            </div>
        </form>
        
        <div class="taxes-note">
            <small>(<?php echo mex("tutti i prezzi delle tariffe si intendono con tasse già incluse",$pag); ?>)</small>
        </div>
        
        <script type="text/javascript">
        <!--
        function agg_sel_tas () {
            opt_corr = document.getElementById('tttas');
            opt_corr = opt_corr.options[opt_corr.selectedIndex].value;
            <?php echo $funz_sel_tas; ?>
        }
        agg_sel_tas();
        -->
        </script>
        
        <?php if ($modifica_pers != "NO"): ?>
            <div class="taxes-settings">
                <?php
                if ($id_utente == 1) $id_utente_mod = "tutti";
                else $id_utente_mod = $id_utente;
                ?>
                <form accept-charset="utf-8" method="post" action="./personalizza.php">
                    <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                    <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                    <input type="hidden" name="id_utente_mod" value="<?php echo $id_utente_mod; ?>">
                    <input type="hidden" name="aggiorna_qualcosa" value="SI">
                    <input type="hidden" name="origine" value="./creaprezzi.php">
                    <input type="hidden" name="cambiaarrtasse" value="SI">
                    
                    <div class="taxes-settings-row">
                        <label><?php echo ucfirst(mex("valore a cui arrotondare le percentuali delle tasse e delle valute","personalizza.php")); ?>:</label>
                        <input type="text" name="nuovo_arrotond_tasse" size="4" value="<?php echo $dati_tariffe['tasse_arrotond']; ?>">
                        <button class="btn-change-settings" type="submit">
                            <?php echo mex("Cambia","personalizza.php"); ?>
                        </button>
                    </div>
                </form>
            </div>
        <?php endif; ?>
        
    </div>
</div>
