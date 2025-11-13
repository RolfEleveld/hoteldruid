<?php
/**
 * Panel: Rule 2 - Apartment Types Assignment to Tariffs
 * Redesigned with label-value pair layout
 */
?>

<style>
.rbox.rule2 {
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    margin-bottom: 20px;
    overflow: hidden;
}

.rbox.rule2 .rheader {
    background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
    padding: 15px 20px;
    font-size: 18px;
    font-weight: bold;
    color: white;
}

.rbox.rule2 .rcontent {
    padding: 20px;
    background: white;
}

.rule2-section {
    margin-bottom: 25px;
    padding-bottom: 20px;
    border-bottom: 1px solid #e0e0e0;
}

.rule2-section:last-child {
    margin-bottom: 0;
    padding-bottom: 0;
    border-bottom: none;
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

.form-row input[type="radio"] {
    margin-right: 5px;
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

.rule2-apartment-selection {
    margin: 15px 0;
    padding: 15px;
    background: #f9f9f9;
    border-radius: 4px;
}

.rule2-help-text {
    font-size: 13px;
    color: #666;
    margin-left: 215px;
    margin-top: -8px;
    margin-bottom: 12px;
}

#tab_app {
    margin-top: 10px;
}

.rule2-occupancy-link {
    margin-left: 215px;
    font-size: 13px;
}
</style>

<script type="text/javascript">
<!--
function mos_app(nreg) {
    var tab_div = document.getElementById("tab_app");
    if (tab_div) {
        tab_div.style.display = "";
        <?php
        if ($num_app_consentiti > 0) {
            echo "tab_div.innerHTML = '<table border=1 style=\"background-color: $t1color;\"><tr><td>";
            for ($num1 = 0; $num1 < $num_appartamenti; $num1++) {
                $app = risul_query($appartamenti, $num1, 'idappartamenti');
                if ($appartamenti_consentiti[$app] == "SI") {
                    echo "<small><input id=\"reg".str_replace(" ","_",$app)."\" type=\"checkbox\" name=\"app$app\" value=\"SI\" onclick=\"javascript:agg_da_tab_a_txt(nreg)\">".str_replace(" ","&nbsp;",htmlspecialchars($app))."</small> ";
                }
            }
            echo "<\/table><br>';";
            echo "\nagg_da_txt_a_tab('bold',nreg);";
        }
        ?>
    }
}

function nasc_app(nreg) {
    var tab_div = document.getElementById("tab_app");
    if (tab_div) tab_div.style.display = "none";
}

function agg_da_txt_a_tab(bold,nreg) {
    var txt_var = document.getElementById("lista_app" + nreg);
    if (txt_var) {
        var app_lista = txt_var.value;
        if (app_lista) app_lista = "," + app_lista + ",";
        <?php
        for ($num1 = 0; $num1 < $num_appartamenti; $num1++) {
            $app = risul_query($appartamenti, $num1, 'idappartamenti');
            if ($appartamenti_consentiti[$app] == "SI") {
                echo "var app_cbox = document.getElementById('reg".str_replace(" ","_",$app)."');\n";
                echo "if (app_cbox) {\n";
                echo "if (app_lista.indexOf(',$app,') != -1) app_cbox.checked = true;\n";
                echo "else app_cbox.checked = false;\n";
                echo "}\n";
            }
        }
        ?>
    }
}

function agg_da_tab_a_txt(nreg) {
    var txt_var = document.getElementById("lista_app" + nreg);
    if (txt_var) {
        var app_lista = "";
        <?php
        for ($num1 = 0; $num1 < $num_appartamenti; $num1++) {
            $app = risul_query($appartamenti, $num1, 'idappartamenti');
            if ($appartamenti_consentiti[$app] == "SI") {
                echo "var app_cbox = document.getElementById('reg".str_replace(" ","_",$app)."');\n";
                echo "if (app_cbox && app_cbox.checked) {\n";
                echo "if (app_lista) app_lista += ',';\n";
                echo "app_lista += '$app';\n";
                echo "}\n";
            }
        }
        ?>
        txt_var.value = app_lista;
    }
}

function aggiorna_val_reg2() {
    var sel_tar = document.getElementById("t2");
    var num_sel = sel_tar.selectedIndex;
    var tariffa = sel_tar.options[num_sel].value;
    var lista_var = document.getElementById("lista_app2");
    if (lista_var) {
        if (tariffa == '') lista_var.disabled = true;
        else lista_var.disabled = false;
        lista_var.value = '';
        var ind_val = '';
        <?php echo $lista_tar_reg2; ?>
        lista_var.value = ind_val;
        agg_da_txt_a_tab('',2);
    }
    var apti_var = document.getElementById("num_apti2");
    if (apti_var) {
        if (tariffa == '') apti_var.disabled = true;
        else apti_var.disabled = false;
        apti_var.value = '1';
        <?php echo $lista_tar_reg2n; ?>
    }
    var apti_var_v = document.getElementById("v_num_apti2");
    if (apti_var_v) {
        <?php echo $lista_tar_reg2v; ?>
    }
}

function aggiorna_val_reg2b() {
    var sel_tar = document.getElementById("t2b");
    var num_sel = sel_tar.selectedIndex;
    var tariffa = sel_tar.options[num_sel].value;
    var lista_var = document.getElementById("lista_app2b");
    if (lista_var) {
        if (tariffa == '') lista_var.disabled = true;
        else lista_var.disabled = false;
        lista_var.value = '';
        var ind_val = '';
        <?php echo $lista_tar_reg2b; ?>
        lista_var.value = ind_val;
        agg_da_txt_a_tab('',2.1);
    }
    var giorni_var = document.getElementById("num_giorni2b");
    if (giorni_var) {
        if (tariffa == '') giorni_var.disabled = true;
        else giorni_var.disabled = false;
        giorni_var.value = '';
        <?php echo $lista_tar_reg2b_gg; ?>
    }
    var ini_fine_i = document.getElementById("ini_fine_i");
    var ini_fine_f = document.getElementById("ini_fine_f");
    if (ini_fine_i && ini_fine_f) {
        <?php echo $lista_tar_reg2b_if; ?>
    }
}

function sel_app_maxocc(nreg) {
    var sel_mo = document.getElementById("sel_mo");
    if (sel_mo) {
        var num_sel = sel_mo.selectedIndex;
        var max_occ = sel_mo.options[num_sel].value;
        if (max_occ != '') {
            var app_lista = "";
            <?php
            for ($num1 = 0; $num1 < $num_appartamenti; $num1++) {
                $app = risul_query($appartamenti, $num1, 'idappartamenti');
                if ($appartamenti_consentiti[$app] == "SI") {
                    $maxocc = @risul_query($appartamenti, $num1, 'maxoccupanti');
                    if ($maxocc !== false && $maxocc !== null) {
                    echo "if (max_occ == '$maxocc') {\n";
                    echo "if (app_lista) app_lista += ',';\n";
                    echo "app_lista += '$app';\n";
                    echo "}\n";
                    }
                }
            }
            ?>
            var txt_var = document.getElementById("lista_app" + nreg);
            if (txt_var) txt_var.value = app_lista;
            agg_da_txt_a_tab('',nreg);
        }
    }
}
-->
</script>

<div class="rbox rule2">
    <div class="rheader">
        <?php echo mex("Regola di assegnazione",$pag); ?> 2 (<?php echo mex("tipologie di appartamenti",'unit.php'); ?>)
    </div>
    <div class="rcontent">
        
        <?php 
        // Display messages at the top of the panel (only for this panel)
        if ((!empty($success_messages) or !empty($error_messages) or !empty($warning_messages)) and (!empty($active_panel) and $active_panel == 'rule2')) {
            HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
        }
        ?>
        
        <!-- Main Rule 2 Form -->
        <div class="rule2-section">
            <form accept-charset="utf-8" method="post" action="crearegole.php">
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="inserisci" value="SI">
                <?php if ($origine): ?>
                    <input type="hidden" name="origine" value="<?php echo htmlspecialchars($origine); ?>">
                <?php endif; ?>
                
                <div class="form-row">
                    <label><?php echo mex("Tariffa",$pag); ?>:</label>
                    <select name="tipotariffa" id="t2" onchange="javascript:aggiorna_val_reg2()">
                        <option value="">--</option>
                        <?php for ($numtariffa = 1; $numtariffa <= $numero_tariffe; $numtariffa++): ?>
                            <?php if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[$numtariffa])): ?>
                                <option value="tariffa<?php echo $numtariffa; ?>"><?php echo mex("tariffa",'prenota.php').$numtariffa; ?></option>
                            <?php endif; ?>
                        <?php endfor; ?>
                    </select>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("Appartamenti",'unit.php'); ?>:</label>
                    <div style="flex: 1;">
                        <input type="text" name="lista_app" id="lista_app2" size="50" maxlength="500" placeholder="<?php echo mex("Lista di appartamenti separati da virgola",'unit.php'); ?>">
                        <div class="rule2-help-text">
                            <a href="#" onclick="javascript:mos_app(2); return false;"><?php echo mex("mostra tabella",$pag); ?></a> |
                            <a href="#" onclick="javascript:nasc_app(2); return false;"><?php echo mex("nascondi tabella",$pag); ?></a>
                        </div>
                    </div>
                </div>
                
                <div id="tab_app" class="rule2-apartment-selection" style="display: none;"></div>
                
                <?php if (!empty($lista_maxocc)): ?>
                <div class="rule2-occupancy-link">
                    <a href="#" onclick="javascript:sel_app_maxocc(2); return false;">
                        <?php echo mex("seleziona gli appartamenti per numero massimo di occupanti",'unit.php'); ?>
                    </a>:
                    <select id="sel_mo" name="sel_maxocc">
                        <option value="">--</option>
                        <?php
                        $vett_mo = explode(",",$lista_maxocc);
                        foreach ($vett_mo as $mo) {
                            echo "<option value=\"$mo\">$mo</option>";
                        }
                        ?>
                    </select>
                </div>
                <?php endif; ?>
                
                <div class="form-row">
                    <label><?php echo mex("Numero di appartamenti",'unit.php'); ?>:</label>
                    <div style="flex: 1; display: flex; align-items: center; gap: 10px;">
                        <input type="text" name="num_apti" id="num_apti2" value="1" size="3" style="max-width: 80px;">
                        <label style="min-width: auto; margin: 0; display: flex; align-items: center;">
                            <input type="checkbox" name="v_apti" id="v_num_apti2" value="v" style="margin-right: 5px;">
                            <?php echo mex("variabile",$pag); ?>
                        </label>
                    </div>
                </div>
                <div class="rule2-help-text">
                    <?php echo mex("da assegnare per ogni prenotazione",$pag); ?>
                </div>
                
                <div class="form-actions">
                    <button class="irul" type="submit" name="regola_2" value="1">
                        <div><?php echo mex("Inserisci o modifica la regola",$pag); ?> 2</div>
                    </button>
                </div>
            </form>
        </div>
        
        <!-- Exception Rule 2b Form -->
        <div class="rule2-section">
            <h4 style="margin-top: 0;"><?php echo mex("Eccezione alla regola 2 per periodo limitato",$pag); ?></h4>
            
            <form accept-charset="utf-8" method="post" action="crearegole.php">
                <input type="hidden" name="anno" value="<?php echo $anno; ?>">
                <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
                <input type="hidden" name="inserisci" value="SI">
                <?php if ($origine): ?>
                    <input type="hidden" name="origine" value="<?php echo htmlspecialchars($origine); ?>">
                <?php endif; ?>
                
                <div class="form-row">
                    <label><?php echo mex("Tariffa",$pag); ?>:</label>
                    <select name="tipotariffa" id="t2b" onchange="javascript:aggiorna_val_reg2b()">
                        <option value="">--</option>
                        <?php for ($numtariffa = 1; $numtariffa <= $numero_tariffe; $numtariffa++): ?>
                            <?php if ($attiva_tariffe_consentite == "n" or isset($tariffe_consentite_vett[$numtariffa])): ?>
                                <option value="tariffa<?php echo $numtariffa; ?>"><?php echo mex("tariffa",'prenota.php').$numtariffa; ?></option>
                            <?php endif; ?>
                        <?php endfor; ?>
                    </select>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("Appartamenti",'unit.php'); ?>:</label>
                    <div style="flex: 1;">
                        <input type="text" name="lista_app" id="lista_app2b" size="50" maxlength="500" placeholder="<?php echo mex("Lista di appartamenti separati da virgola",'unit.php'); ?>">
                        <div class="rule2-help-text">
                            <a href="#" onclick="javascript:mos_app(2.1); return false;"><?php echo mex("mostra tabella",$pag); ?></a> |
                            <a href="#" onclick="javascript:nasc_app(2.1); return false;"><?php echo mex("nascondi tabella",$pag); ?></a>
                        </div>
                    </div>
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("Numero di giorni",$pag); ?>:</label>
                    <input type="text" name="num_giorni" id="num_giorni2b" value="" size="3" maxlength="3">
                </div>
                
                <div class="form-row">
                    <label><?php echo mex("dall'inizio o dalla fine del periodo",$pag); ?>:</label>
                    <div style="flex: 1;">
                        <label style="min-width: auto; margin-right: 20px;">
                            <input type="radio" name="ini_fine" id="ini_fine_i" value="ini" checked>
                            <?php echo mex("dall'inizio",$pag); ?>
                        </label>
                        <label style="min-width: auto;">
                            <input type="radio" name="ini_fine" id="ini_fine_f" value="fine">
                            <?php echo mex("dalla fine",$pag); ?>
                        </label>
                    </div>
                </div>
                
                <div class="form-actions">
                    <button class="irul" type="submit" name="regola_2b" value="1">
                        <div><?php echo mex("Inserisci o modifica l'eccezione alla regola",$pag); ?> 2</div>
                    </button>
                </div>
            </form>
        </div>
        
    </div>
</div>

<?php if (!empty($tipotariffa_regola2) and preg_replace("/tariffa[0-9]+/","",$tipotariffa_regola2) == ""): ?>
<script type="text/javascript">
<!--
var sel_tr2 = document.getElementById("t2");
if (sel_tr2) {
    sel_tr2.value = '<?php echo $tipotariffa_regola2; ?>';
    aggiorna_val_reg2();
}
var sel_tr2b = document.getElementById("t2b");
if (sel_tr2b) {
    sel_tr2b.value = '<?php echo $tipotariffa_regola2; ?>';
    aggiorna_val_reg2b();
}
-->
</script>
<?php endif; ?>
