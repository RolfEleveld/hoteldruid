<?php
/**
 * Panel: Rule 1 - Closures (Apartments and Tariffs)
 * Redesigned with label-value pair layout
 */
?>

<style>
.rbox.rule1 {
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    margin-bottom: 20px;
    overflow: hidden;
}

.rbox.rule1 .rheader {
    background: linear-gradient(135deg, #4CAF50 0%, #388E3C 100%);
    padding: 15px 20px;
    font-size: 18px;
    font-weight: bold;
    color: white;
}

.rbox.rule1 .rcontent {
    padding: 20px;
    background: white;
}

.rule1-section {
    margin-bottom: 25px;
    padding-bottom: 20px;
    border-bottom: 1px solid #e0e0e0;
}

.rule1-section:last-child {
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

<div class="rbox rule1">
    <div class="rheader">
        <?php echo mex("Regola di assegnazione",$pag); ?> 1 (<?php echo mex("chiusure",$pag); ?>)
    </div>
    <div class="rcontent">
        
        <?php 
        // Display messages at the top of the panel (only for this panel)
        if ((!empty($success_messages) or !empty($error_messages) or !empty($warning_messages)) and (!empty($active_panel) and $active_panel == 'rule1')) {
            HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
        }
        
        // Display confirmation forms for overlapping rules if needed
        HotelDruidTemplate::getInstance()->display('crearegole/rule1_confirmation', get_defined_vars());
        HotelDruidTemplate::getInstance()->display('crearegole/rule1b_confirmation', get_defined_vars());
        HotelDruidTemplate::getInstance()->display('crearegole/delete_rule1_confirmation', get_defined_vars());
        ?>
        
        <?php if (($priv_mod_reg1 == "s" or $priv_mod_reg1 == "a") and $tipo_mod_reg1 != "t"): ?>
        <!-- Apartment Closures Form -->
        <div class="rule1-section">
            <form accept-charset="utf-8" method="post" action="crearegole.php">
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="inserisci" value="SI">
                <?php if ($tipo_mod_reg1): ?>
                    <input type="hidden" name="mod_idregola1" value="<?php echo $mod_idregola1; ?>">
                <?php endif; ?>
                <?php if ($origine): ?>
                    <input type="hidden" name="origine" value="<?php echo htmlspecialchars($origine); ?>">
                <?php endif; ?>
                
                <div class="form-row">
                    <label><?php echo mex("Azione",$pag); ?>:</label>
                    <select name="chiudi_app">
                        <option value="1"<?php echo (isset($motivazione2) and $motivazione2 != "x") ? "" : " selected"; ?>><?php echo mex("Chiudi",$pag); ?></option>
                        <?php if ($attiva_regole1_consentite == "n"): ?>
                            <option value=""<?php echo (isset($motivazione2) and $motivazione2 != "x") ? " selected" : ""; ?>><?php echo mex("Chiedi prima di assegnare",$pag); ?></option>
                        <?php endif; ?>
                    </select>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("Appartamento",'unit.php'); ?>:</label>
                    <select name="appartamento">
                        <option value=""<?php echo (!isset($app_agenzia) or is_array($app_agenzia) or !strcmp((string) $app_agenzia,"")) ? " selected" : ""; ?>>--</option>
                        <?php if ($tutti_app_consentiti and !$tipo_mod_reg1): ?>
                            <?php include(C_DATI_PATH."/selectappartamenti.php"); ?>
                        <?php else: ?>
                            <?php for ($num1 = 0 ; $num1 < $num_appartamenti ; $num1++): ?>
                                <?php
                                $idapp = risul_query($appartamenti,$num1,'idappartamenti');
                                if ($tutti_app_consentiti or $appartamenti_consentiti[$idapp] == "SI"):
                                ?>
                                    <option value="<?php echo $idapp; ?>"<?php echo (isset($app_agenzia) and !is_array($app_agenzia) and $idapp == $app_agenzia) ? " selected" : ""; ?>><?php echo $idapp; ?></option>
                                <?php endif; ?>
                            <?php endfor; ?>
                        <?php endif; ?>
                    </select>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("Periodo dal",$pag); ?>:</label>
                    <?php mostra_menu_date(C_DATI_PATH."/selectperiodi$anno.$id_utente.php","inizioperiodo",$inizio_select,"","",$id_utente,$tema); ?>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("al",$pag); ?>:</label>
                    <?php mostra_menu_date(C_DATI_PATH."/selectperiodi$anno.$id_utente.php","fineperiodo",$fine_select,"","",$id_utente,$tema); ?>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("Motivazione",$pag); ?>:</label>
                    <input type="text" name="motivazione" value="<?php echo $motivazione; ?>" size="30" maxlength="30" placeholder="<?php echo mex("opzionale",$pag); ?>">
                </div>
                
                <div class="form-actions">
                    <button class="irul" type="submit" name="regola_1" value="1">
                        <div><?php echo $tipo_mod_reg1 ? mex("Modifica la regola",$pag) : mex("Inserisci la regola",$pag); ?> 1</div>
                    </button>
                </div>
            </form>
        </div>
        <?php endif; ?>
        
        <?php if (($priv_mod_reg1 == "s" or $priv_mod_reg1 == "t") and $tipo_mod_reg1 != "a"): ?>
        <!-- Tariff Closures Form -->
        <div class="rule1-section">
            <form accept-charset="utf-8" method="post" action="crearegole.php">
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="inserisci" value="SI">
                <input type="hidden" name="regola_1_tar" value="1">
                <?php if ($origine): ?>
                    <input type="hidden" name="origine" value="<?php echo htmlspecialchars($origine); ?>">
                <?php endif; ?>
                <?php if ($tipo_mod_reg1): ?>
                    <input type="hidden" name="mod_idregola1" value="<?php echo $mod_idregola1; ?>">
                <?php endif; ?>
                
                <div class="form-row">
                    <label><?php echo mex("Tariffa",$pag); ?>:</label>
                    <select name="tipotariffa" id="t1">
                        <option value=""<?php echo empty($tariffa_chiusa) ? " selected" : ""; ?>>----</option>
                        <?php
                        $tutte_tar_consentite = 1;
                        for ($numtariffa = 1 ; $numtariffa <= $numero_tariffe ; $numtariffa++):
                            if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[$numtariffa])):
                                $tariffa = "tariffa".$numtariffa;
                                $tariffa_vedi = mex("tariffa","prenota.php").$numtariffa;
                                $nometariffa = risul_query($rigatariffe,0,$tariffa);
                                $nometariffa_vedi = ($nometariffa != "") ? " ($nometariffa)" : "";
                        ?>
                                <option value="<?php echo $tariffa; ?>"<?php echo (isset($tariffa_chiusa) and $tariffa == $tariffa_chiusa) ? " selected" : ""; ?>><?php echo $tariffa_vedi.$nometariffa_vedi; ?></option>
                        <?php
                            endif;
                            if (!($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[$numtariffa]))) $tutte_tar_consentite = 0;
                        endfor;
                        ?>
                    </select>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("Periodo dal",$pag); ?>:</label>
                    <?php mostra_menu_date(C_DATI_PATH."/selectperiodi$anno.$id_utente.php","inizioperiodo",$inizio_select,"","",$id_utente,$tema); ?>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("al",$pag); ?>:</label>
                    <?php mostra_menu_date(C_DATI_PATH."/selectperiodi$anno.$id_utente.php","fineperiodo",$fine_select,"","",$id_utente,$tema); ?>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("Motivazione",$pag); ?>:</label>
                    <input type="text" name="motivazione" value="<?php echo $motivazione; ?>" size="30" maxlength="30" placeholder="<?php echo mex("opzionale",$pag); ?>">
                </div>
                
                <div class="form-actions">
                    <button class="irul" type="submit">
                        <div><?php echo $tipo_mod_reg1 ? mex("Modifica la regola",$pag) : mex("Inserisci la regola",$pag); ?> 1 <?php echo mex("per le tariffe",$pag); ?></div>
                    </button>
                </div>
            </form>
        </div>
        <?php endif; ?>
        
        <?php if (!empty($tutti_app_consentiti) and $attiva_regole1_consentite == "n" and !empty($tutte_tar_consentite) and $priv_mod_reg1 == "s" and !$mostra_solo_regola): ?>
        <!-- Delete All Rules Button -->
        <div class="rule1-section">
            <form accept-charset="utf-8" method="post" action="crearegole.php">
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="inserisci" value="SI">
                <div class="form-actions">
                    <button class="drule" type="submit" name="canc_regola_1" value="1">
                        <div><?php echo mex("Cancella tutte le regole di questo tipo",$pag); ?></div>
                    </button>
                </div>
            </form>
        </div>
        <?php endif; ?>
        
    </div>
</div>
