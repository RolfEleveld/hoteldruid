<?php
/**
 * Panel: Import Tariffs
 * Color: Purple (#9C27B0)
 */
?>

<div class="rbox" style="border-left-color: #9C27B0;">
    <div class="rheader" style="background: linear-gradient(135deg, #9C27B0 0%, #7B1FA2 100%);">
        <h5><?php echo mex("Importa i prezzi",$pag); ?></h5>
    </div>
    <div class="rcontent">
        <?php
        // Display feedback messages if this panel is active
        if (isset($active_panel) && $active_panel === 'panel_import_tariffs') {
            if (class_exists('HotelDruidTemplate')) {
                HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
            }
        }
        ?>
        
        <form accept-charset="utf-8" method="post" action="creaprezzi.php">
            <div>
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="origine" value="<?php echo $pag; ?>">
                <input type="hidden" name="importa_tariffa" value="1">
                <?php if ($tar_imp_mod) { ?>
                    <input type="hidden" name="tar_importa_mod" value="1">
                    <input type="hidden" name="tar_importa_canc" value="<?php echo $tar_importa_canc; ?>">
                    <input type="hidden" name="per_importa_canc" value="<?php echo $per_importa_canc; ?>">
                <?php } ?>
                
                <div class="import-form">
                    <div class="import-row">
                        <?php if ($tar_imp_mod) { ?>
                            <span class="import-label"><?php echo mex("Importa sempre",$pag); ?></span>
                        <?php } else { ?>
                            <select id="s_tim_t" name="tipo_importa" onchange="agg_select_tar_a()">
                                <option value="ora"<?php echo ($tipo_importa == "sempre") ? "" : " selected"; ?>><?php echo mex("Importa ora",$pag); ?></option>
                                <option value="sempre"<?php echo ($tipo_importa == "sempre") ? " selected" : ""; ?>><?php echo mex("Importa sempre",$pag); ?></option>
                            </select>
                        <?php } ?>
                        
                        <span><?php echo mex("i prezzi della",$pag); ?></span>
                        
                        <select id="s_tar_a" name="tariffa_a">
                            <?php 
                            if ($tariffa_a) echo str_replace("$tariffa_a\">","$tariffa_a\" selected>",$lista_opt_tariffe_no_esporta);
                            else echo $lista_opt_tariffe;
                            ?>
                        </select>
                        
                        <span><?php echo mex("dalla",$pag); ?></span>
                        
                        <select name="tariffa_da">
                            <?php 
                            if ($tariffa_da) echo str_replace("$tariffa_da\">","$tariffa_da\" selected>",$lista_opt_tariffe_cambia_tutti);
                            else echo $lista_opt_tariffe_cambia_tutti;
                            ?>
                        </select>
                    </div>
                    
                    <div class="import-row">
                        <span><?php echo mex("aggiungendo",$pag); ?></span>
                        
                        <span class="input-group">
                            <input type="text" name="importa_fisso" size="3" value="<?php echo $importa_fisso; ?>">
                            <select name="tipo_fisso" id="tip_per" onchange="att_dis_arrotond()">
                                <option value="euro_g"<?php echo ($tipo_fisso == "g") ? " selected" : ""; ?>><?php echo $Euro; ?> <?php echo mex("$parola_alla $parola_settimana",$pag); ?></option>
                                <?php if ($tipo_periodi == "g") { ?>
                                    <option value="euro_s"<?php echo ($tipo_fisso == "s") ? " selected" : ""; ?>><?php echo $Euro; ?> <?php echo mex("alla settimana",$pag); ?></option>
                                <?php } ?>
                            </select>
                        </span>
                        
                        <span>+</span>
                        
                        <span class="input-group">
                            <input type="text" name="importa_percent" size="3" value="<?php echo $importa_percent; ?>">%
                            (<?php echo mex("arrotondato a",$pag); ?>
                            <input type="text" name="importa_arrotond" id="imp_arr" value="<?php echo $importa_arrotond; ?>" size="4">)
                        </span>
                    </div>
                    
                    <div class="import-row">
                        <select name="parte_prezzo" class="full-width">
                            <option value="f"<?php echo ($parte_prezzo == "f") ? " selected" : ""; ?>><?php echo mex("al prezzo fisso",$pag); ?></option>
                            <option value="fn"<?php echo ($parte_prezzo == "fn") ? " selected" : ""; ?>><?php echo mex("al prezzo fisso",$pag); ?>, <?php echo mex("per persona nullo",$pag); ?></option>
                            <option value="p"<?php echo ($parte_prezzo == "p") ? " selected" : ""; ?>><?php echo mex("al prezzo per persona",$pag); ?></option>
                            <option value="pn"<?php echo ($parte_prezzo == "pn") ? " selected" : ""; ?>><?php echo mex("al prezzo per persona",$pag); ?>, <?php echo mex("fisso nullo",$pag); ?></option>
                            <option value="2"<?php echo ($parte_prezzo == "2") ? " selected" : ""; ?>><?php echo mex("ad entrambi i prezzi",$pag); ?></option>
                            <option value="fp"<?php echo ($parte_prezzo == "fp") ? " selected" : ""; ?>><?php echo mex("al prezzo fisso",$pag); ?> <?php echo mex("da per persona",$pag); ?></option>
                            <option value="fpf"<?php echo ($parte_prezzo == "fpf") ? " selected" : ""; ?>><?php echo mex("al prezzo fisso",$pag); ?> <?php echo mex("da per persona",$pag); ?> <?php echo mex("più fisso",$pag); ?></option>
                            <option value="pf"<?php echo ($parte_prezzo == "pf") ? " selected" : ""; ?>><?php echo mex("al prezzo per persona",$pag); ?> <?php echo mex("dal fisso",$pag); ?></option>
                        </select>
                    </div>
                    
                    <div class="import-row">
                        <div class="radio-group">
                            <label>
                                <input type="radio" name="periodi_importa" value="t"<?php echo ($periodi_importa == "t") ? " checked" : ""; ?>>
                                <?php echo mex("in tutti i periodi in modo predefinito",$pag); ?>
                            </label>
                        </div>
                    </div>
                    
                    <div class="import-row period-selector">
                        <label onclick="document.getElementById('perimp_s').checked='1'">
                            <input type="radio" id="perimp_s" name="periodi_importa" value="s"<?php echo ($periodi_importa == "s") ? " checked" : ""; ?>>
                            <?php echo mex("dal",$pag); ?>
                        </label>
                        <?php mostra_menu_date(C_DATI_PATH."/selectperiodi$anno.$id_utente.php","iniper_imp",$iniper_imp,"","",$id_utente,$tema); ?>
                        <span><?php echo mex("al",$pag); ?></span>
                        <?php mostra_menu_date(C_DATI_PATH."/selectperiodi$anno.$id_utente.php","fineper_imp",$fineper_imp,"","",$id_utente,$tema); ?>
                    </div>
                    
                    <div class="form-actions">
                        <button class="xpri" type="submit">
                            <div><?php echo $tar_imp_mod ? mex("modifica",$pag) : mex("importa",$pag); ?></div>
                        </button>
                    </div>
                </div>
            </div>
        </form>
        
        <?php
        // Display existing import rules
        $has_imports = false;
        for ($num1 = 1 ; $num1 <= $dati_tariffe['num'] ; $num1++) {
            if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[$num1])) {
                $tariffa = "tariffa".$num1;
                if ($dati_tariffe[$tariffa]['imp_prezzi_int']) {
                    $has_imports = true;
                    break;
                }
            }
        }
        
        if ($has_imports) {
            echo '<div class="import-list-header">'.mex("Importazioni configurate",$pag).':</div>';
            echo '<div class="import-list">';
            
            for ($num1 = 1 ; $num1 <= $dati_tariffe['num'] ; $num1++) {
                if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[$num1])) {
                    $tariffa = "tariffa".$num1;
                    if ($dati_tariffe[$tariffa]['imp_prezzi_int']) {
                        $vett_ord = array();
                        $vett_ord[0] = 0;
                        if ($dati_tariffe[$tariffa]['num_per_importa'] > 1) {
                            $periodi_ord = array();
                            for ($num2 = 1 ; $num2 < $dati_tariffe[$tariffa]['num_per_importa'] ; $num2++) 
                                $periodi_ord[$dati_tariffe[$tariffa]['periodo_importa_i'][$num2]] = $num2;
                            ksort($periodi_ord);
                            $num_ord = 1;
                            foreach ($periodi_ord as $per => $num2) {
                                $vett_ord[$num_ord] = $num2;
                                $num_ord++;
                            }
                        }
                        
                        for ($num_ord = 0 ; $num_ord < $dati_tariffe[$tariffa]['num_per_importa'] ; $num_ord++) {
                            $num2 = $vett_ord[$num_ord];
                            if ($dati_tariffe[$tariffa]['importa_prezzi'][$num2]) {
                                $is_editing = ($tar_imp_mod == $tariffa and $num2 == $ord_imp_mod);
                                echo '<div class="import-item'.($is_editing ? ' editing' : '').'">';
                                echo '<div class="import-desc">';
                                
                                if (isset($dati_tariffe[$tariffa]['periodo_importa_i'][$num2])) {
                                    $ini_imp = esegui_query("select * from $tableperiodi where idperiodi = '".$dati_tariffe[$tariffa]['periodo_importa_i'][$num2]."' ");
                                    $ini_imp = formatta_data(risul_query($ini_imp,0,'datainizio'),$stile_data);
                                    $fine_imp = esegui_query("select * from $tableperiodi where idperiodi = '".$dati_tariffe[$tariffa]['periodo_importa_f'][$num2]."' ");
                                    $fine_imp = formatta_data(risul_query($fine_imp,0,'datafine'),$stile_data);
                                    echo ucfirst(mex("dal",$pag))." <strong>$ini_imp</strong> ".mex("al",$pag)." <strong>$fine_imp</strong> ";
                                } else {
                                    if ($dati_tariffe[$tariffa]['num_per_importa'] < 2) echo mex("Importa sempre",$pag)." ";
                                    else echo mex("In modo predefinito importa",$pag)." ";
                                }
                                
                                echo mex("i prezzi della",$pag)." <strong>".mex("tariffa",$pag)."$num1";
                                if ($dati_tariffe[$tariffa]['nome']) echo " (".$dati_tariffe[$tariffa]['nome'].")";
                                echo "</strong> ".mex("dalla",$pag)." <em>".mex("tariffa",$pag).$dati_tariffe[$tariffa]['importa_prezzi'][$num2];
                                if (!empty($dati_tariffe['tariffa'.$dati_tariffe[$tariffa]['importa_prezzi'][$num2]]['nome'])) 
                                    echo " (".$dati_tariffe['tariffa'.$dati_tariffe[$tariffa]['importa_prezzi'][$num2]]['nome'].")";
                                echo "</em>";
                                
                                if ($dati_tariffe[$tariffa]['val_importa'][$num2] or $dati_tariffe[$tariffa]['perc_importa'][$num2]) {
                                    echo " +";
                                    if ($dati_tariffe[$tariffa]['val_importa'][$num2]) echo " ".$dati_tariffe[$tariffa]['val_importa'][$num2].$Euro;
                                    if ($dati_tariffe[$tariffa]['perc_importa'][$num2]) echo " ".$dati_tariffe[$tariffa]['perc_importa'][$num2]."%";
                                }
                                
                                echo '</div>';
                                echo '<div class="import-actions">';
                                echo '<form method="post" action="creaprezzi.php#imp_pre" style="display:inline;">';
                                echo '<input type="hidden" name="anno" value="'.$anno.'">';
                                echo '<input type="hidden" name="id_sessione" value="'.$id_sessione.'">';
                                echo '<input type="hidden" name="tar_importa_canc" value="'.$num1.'">';
                                if (isset($dati_tariffe[$tariffa]['periodo_importa_i'][$num2])) {
                                    echo '<input type="hidden" name="per_importa_canc" value="'.$dati_tariffe[$tariffa]['periodo_importa_i'][$num2]."-".$dati_tariffe[$tariffa]['periodo_importa_f'][$num2].'">';
                                }
                                echo '<button type="submit" class="btn-small">';
                                if ($is_editing) echo mex("annulla",$pag); else echo mex("modifica",$pag);
                                echo '</button>';
                                echo '</form>';
                                echo '</div>';
                                echo '</div>';
                            }
                        }
                    }
                }
            }
            echo '</div>';
        }
        ?>
    </div>
</div>

<script type="text/javascript">
<!--
function att_dis_arrotond () {
    var select = document.getElementById('tip_per');
    var arrotond = document.getElementById('imp_arr');
    if (select.selectedIndex == 0) arrotond.disabled = false;
    else arrotond.disabled = true;
}

function agg_select_tar_a () {
    var select = document.getElementById('s_tim_t');
    var sel_tar_a = document.getElementById('s_tar_a');
    var sel_val = sel_tar_a.options[sel_tar_a.selectedIndex].value;
    if (select.selectedIndex == 0) sel_tar_a.innerHTML = '<?php echo str_replace("'","\\'",$lista_opt_tariffe); ?>';
    else sel_tar_a.innerHTML = '<?php echo str_replace("'","\\'",$lista_opt_tariffe_no_esporta); ?>';
    selizona_opt_con_val('s_tar_a',sel_val);
}
-->
</script>

<style>
.import-form {
    display: flex;
    flex-direction: column;
    gap: 15px;
}

.import-row {
    display: flex;
    align-items: center;
    gap: 8px;
    flex-wrap: wrap;
}

.import-row select,
.import-row input[type="text"] {
    padding: 6px 10px;
    border: 1px solid #ddd;
    border-radius: 3px;
}

.import-row select.full-width {
    flex: 1;
    min-width: 300px;
}

.import-row .input-group {
    display: flex;
    align-items: center;
    gap: 4px;
    white-space: nowrap;
}

.radio-group label {
    display: flex;
    align-items: center;
    gap: 6px;
    cursor: pointer;
}

.period-selector {
    padding-left: 24px;
}

.period-selector label {
    display: flex;
    align-items: center;
    gap: 6px;
    cursor: pointer;
}

.import-list-header {
    margin-top: 20px;
    padding: 10px 0;
    font-weight: 600;
    border-top: 2px solid #f0f0f0;
}

.import-list {
    display: flex;
    flex-direction: column;
    gap: 8px;
}

.import-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 10px;
    background: #f9f9f9;
    border-radius: 4px;
    border-left: 3px solid #9C27B0;
}

.import-item.editing {
    background: #f3e5f5;
    border-left-color: #7B1FA2;
}

.import-desc {
    flex: 1;
    line-height: 1.5;
}

.import-desc strong {
    color: #333;
}

.import-desc em {
    color: #666;
    font-style: normal;
}

.import-actions {
    display: flex;
    gap: 6px;
}

.btn-small {
    padding: 4px 12px;
    font-size: 0.9em;
    background: #9C27B0;
    color: white;
    border: none;
    border-radius: 3px;
    cursor: pointer;
}

.btn-small:hover {
    background: #7B1FA2;
}

.form-actions {
    text-align: left;
    margin-top: 10px;
}
</style>
