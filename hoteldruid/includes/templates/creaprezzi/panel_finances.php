<?php
/**
 * Panel: Finances (Deposits & Commissions)
 * Stage 6 of creaprezzi.php refactoring
 */

// Escaping helper for JavaScript strings
function esc_js($str) {
    return addslashes($str);
}
?>

<style>
.rbox.finances {
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    margin-bottom: 20px;
    overflow: hidden;
}

.rbox.finances .rheader {
    background: linear-gradient(135deg, #3F51B5 0%, #303F9F 100%);
    padding: 15px 20px;
    border-bottom: none;
    font-size: 18px;
    font-weight: bold;
    color: white;
}

.rbox.finances .rcontent {
    padding: 20px;
    background: white;
    border-radius: 0 0 8px 8px;
}

.finances-section {
    margin-bottom: 40px;
}

.finances-section:last-child {
    margin-bottom: 0;
}

.finances-section h5 {
    color: #3F51B5;
    font-size: 16px;
    margin: 0 0 15px 0;
    padding-bottom: 10px;
    border-bottom: 2px solid #E8EAF6;
}

.finances-table {
    width: 100%;
    border-collapse: collapse;
}

.finances-table td {
    padding: 10px 15px;
    vertical-align: middle;
}

.finances-table td:first-child {
    text-align: right;
    width: 40%;
    color: #555;
}

.finances-table td:last-child {
    text-align: left;
    width: 60%;
}

.finances-table select,
.finances-table input[type="text"] {
    padding: 6px 10px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 14px;
}

.finances-table label {
    cursor: pointer;
    color: #555;
}

.finances-table input[type="radio"] {
    cursor: pointer;
    margin-right: 5px;
}

.btn-submit-finances {
    background: #3F51B5;
    color: white;
    border: none;
    padding: 12px 30px;
    font-size: 15px;
    border-radius: 5px;
    cursor: pointer;
    transition: background 0.3s;
}

.btn-submit-finances:hover {
    background: #303F9F;
}

.section-divider {
    height: 1px;
    background: #E0E0E0;
    margin: 30px 0;
}
</style>

<div class="rbox finances">
    <div class="rheader">
        <?php echo mex("Gestione Finanziaria",$pag); ?>
    </div>
    <div class="rcontent">
        <?php
        // Display feedback messages if this panel is active
        if (isset($active_panel) && $active_panel === 'panel_finances') {
            if (class_exists('HotelDruidTemplate')) {
                HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
            }
        }
        ?>
        
        <!-- DEPOSITS SECTION -->
        <div class="finances-section">
            <a name="mod_cap"></a>
            <form accept-charset="utf-8" method="post" action="creaprezzi.php">
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="modificacaparra" value="1">
                <input type="hidden" name="origine" value="<?php echo $pag; ?>">
                
                <h5><?php echo mex("Caparra",$pag); ?></h5>
                
                <table class="finances-table">
                    <tr>
                        <td>
                            <?php echo mex("La caparra normale per",$pag); ?>
                            <select id="ttcap" name="tipotariffa" onchange="agg_sel_cap();">
                                <?php if ($num_opt_tariffe > 1): ?>
                                    <option value="tutte" selected><?php echo mex("tutte le tariffe",$pag); ?></option>
                                <?php endif; ?>
                                <?php echo $lista_capa_esist; ?>
                                <?php echo $lista_opt_tariffe; ?>
                            </select>
                            <?php echo mex("è",$pag); ?>:
                        </td>
                        <td onclick="document.getElementById('tipo_cap_perc').checked='1'">
                            <label>
                                <input type="radio" id="tipo_cap_perc" name="tipo_caparra" value="perc" checked>
                                <?php echo mex("il",$pag); ?>
                            </label>
                            <input type="text" id="cap_perc" name="caparra_percent" size="2" maxlength="3">
                            <label for="tipo_cap_perc">
                                % <?php echo mex("della tariffa arrotondato a",$pag); ?>
                            </label>
                            <input type="text" id="cap_arr" name="caparra_arrotond" value="<?php echo $arrotond_cap; ?>" size="5">
                            <label for="tipo_cap_perc">
                                <?php echo $Euro; ?>.
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td onclick="document.getElementById('tipo_cap_val').checked='1'">
                            <input type="radio" id="tipo_cap_val" name="tipo_caparra" value="val">
                            <input type="text" id="cap_val" name="caparra_val" size="5">
                            <label for="tipo_cap_val"><?php echo $Euro; ?>.</label>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td onclick="document.getElementById('tipo_cap_gio').checked='1'">
                            <label>
                                <input type="radio" id="tipo_cap_gio" name="tipo_caparra" value="gio">
                                <?php echo mex("il prezzo della tariffa per $parola_le prim$lettera_e",$pag); ?>
                            </label>
                            <select id="cap_gio" name="caparra_gio">
                                <?php for ($num1 = 1 ; $num1 <= 10 ; $num1++): ?>
                                    <option value="<?php echo $num1; ?>"><?php echo $num1; ?></option>
                                <?php endfor; ?>
                            </select>
                            <label for="tipo_cap_gio"><?php echo mex("$parola_settimane",$pag); ?>.</label>
                        </td>
                    </tr>
                </table>
                
                <div style="text-align: center; margin-top: 20px;">
                    <button class="btn-submit-finances" type="submit">
                        <?php echo mex("Inserisci o modifica la caparra",$pag); ?>
                    </button>
                </div>
            </form>
            
            <script type="text/javascript">
            <!--
            function agg_sel_cap () {
                opt_corr = document.getElementById('ttcap');
                opt_corr = opt_corr.options[opt_corr.selectedIndex].value;
                <?php echo $funz_sel_cap; ?>
            }
            agg_sel_cap();
            -->
            </script>
        </div>
        
        <div class="section-divider"></div>
        
        <!-- COMMISSIONS SECTION (Default) -->
        <div class="finances-section">
            <a name="mod_com"></a>
            <form accept-charset="utf-8" method="post" action="creaprezzi.php">
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="modificacommissioni" value="1">
                <input type="hidden" name="origine" value="<?php echo $pag; ?>">
                
                <h5><?php echo mex("Commissioni",$pag); ?></h5>
                
                <table class="finances-table">
                    <tr>
                        <td>
                            <?php echo mex("Le commissioni normali su",$pag); ?>
                            <select id="ttcom" name="tipotariffa" onchange="agg_sel_com();">
                                <?php if ($num_opt_tariffe > 1): ?>
                                    <option value="tutte"><?php echo mex("tutte le tariffe",$pag); ?></option>
                                <?php endif; ?>
                                <?php echo $lista_com_esist; ?>
                                <?php echo $lista_opt_tariffe; ?>
                            </select>
                            <?php echo mex("sono",$pag); ?>:
                        </td>
                        <td onclick="document.getElementById('tipo_com_perc').checked='1'">
                            <label>
                                <input type="radio" id="tipo_com_perc" name="tipo_commissioni" value="perc" checked>
                                <?php echo mex("il",$pag); ?>
                            </label>
                            <input type="text" id="com_perc" name="commissioni_percent" size="2" maxlength="3">
                            <label for="tipo_com_perc">
                                % <?php echo mex("della",$pag); ?>
                            </label>
                            <select id="com_base" name="commissioni_base">
                                <option value="t"><?php echo mex("tariffa",$pag); ?></option>
                                <option value="ts"><?php echo mex("tariffa + sconto",$pag); ?></option>
                                <option value="tsc"><?php echo mex("tariffa + sconto + costi agg.",$pag); ?></option>
                            </select>
                            <label for="tipo_com_perc">
                                <?php echo mex("arrotondato a",$pag); ?>
                            </label>
                            <input type="text" id="com_arr" name="commissioni_arrotond" value="<?php echo $arrotond_com; ?>" size="5">
                            <label for="tipo_com_perc"><?php echo $Euro; ?>.</label>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td onclick="document.getElementById('tipo_com_val').checked='1'">
                            <input type="radio" id="tipo_com_val" name="tipo_commissioni" value="val">
                            <input type="text" id="com_val" name="commissioni_val" size="5">
                            <label for="tipo_com_val">
                                <?php echo $Euro; ?> <?php echo mex("$parola_alla $parola_settimana",$pag); ?>.
                            </label>
                        </td>
                    </tr>
                </table>
                
                <div style="text-align: center; margin-top: 20px;">
                    <button class="btn-submit-finances" type="submit">
                        <?php echo mex("Inserisci o modifica le commissioni normali",$pag); ?>
                    </button>
                </div>
            </form>
            
            <script type="text/javascript">
            <!--
            function agg_sel_com () {
                opt_corr = document.getElementById('ttcom');
                opt_corr = opt_corr.options[opt_corr.selectedIndex].value;
                <?php echo $funz_sel_com; ?>
            }
            agg_sel_com();
            -->
            </script>
        </div>
        
        <div class="section-divider"></div>
        
        <!-- COMMISSIONS SECTION (Per Period) -->
        <div class="finances-section">
            <form accept-charset="utf-8" method="post" action="creaprezzi.php">
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="modificacommper" value="1">
                <input type="hidden" name="origine" value="<?php echo $pag; ?>">
                
                <table class="finances-table">
                    <tr>
                        <td>
                            <?php echo mex("Sulla",$pag); ?>
                            <select name="tipotariffa">
                                <?php echo $lista_opt_tariffe; ?>
                            </select>
                            <?php echo mex("dal",$pag); ?>
                            <?php mostra_menu_date(C_DATI_PATH."/selectperiodi$anno.1.php","comm_dal",fixset($comm_dal),"","",$id_utente,$tema); ?>
                            <?php echo mex("al",$pag); ?>
                            <?php mostra_menu_date(C_DATI_PATH."/selectperiodi$anno.1.php","comm_al",fixset($comm_al),"","",$id_utente,$tema); ?>:
                        </td>
                        <td onclick="document.getElementById('tipo_com_percp').checked='1'">
                            <label>
                                <input type="radio" id="tipo_com_percp" name="tipo_commissioni" value="perc" checked>
                                <?php echo mex("il",$pag); ?>
                            </label>
                            <input type="text" name="commissioni_percent" size="2" maxlength="3">
                            <label for="tipo_com_percp">
                                % <?php echo mex("della",$pag); ?>
                            </label>
                            <select name="commissioni_base">
                                <option value="t"><?php echo mex("tariffa",$pag); ?></option>
                                <option value="ts"><?php echo mex("tariffa + sconto",$pag); ?></option>
                                <option value="tsc"><?php echo mex("tariffa + sconto + costi agg.",$pag); ?></option>
                            </select>
                            <label for="tipo_com_percp">
                                <?php echo mex("arrotondato a",$pag); ?>
                            </label>
                            <input type="text" name="commissioni_arrotond" value="<?php echo $arrotond_com; ?>" size="5">
                            <label for="tipo_com_percp"><?php echo $Euro; ?>.</label>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td onclick="document.getElementById('tipo_com_valp').checked='1'">
                            <input type="radio" id="tipo_com_valp" name="tipo_commissioni" value="val">
                            <input type="text" name="commissioni_val" size="5">
                            <label for="tipo_com_valp">
                                <?php echo $Euro; ?> <?php echo mex("$parola_alla $parola_settimana",$pag); ?>.
                            </label>
                        </td>
                    </tr>
                </table>
                
                <div style="text-align: center; margin-top: 20px;">
                    <button class="btn-submit-finances" type="submit">
                        <?php echo mex("Inserisci o modifica le commissioni in questo periodo",$pag); ?>
                    </button>
                </div>
            </form>
        </div>
        
    </div>
</div>
