<?php
/**
 * Panel: Rule 4 - User Insertion Assignment
 * Redesigned with label-value pair layout
 */
?>

<style>
.rbox.rule4 {
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    margin-bottom: 20px;
    overflow: hidden;
}

.rbox.rule4 .rheader {
    background: linear-gradient(135deg, #9C27B0 0%, #7B1FA2 100%);
    padding: 15px 20px;
    font-size: 18px;
    font-weight: bold;
    color: white;
}

.rbox.rule4 .rcontent {
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

.rule4-info {
    font-size: 13px;
    color: #666;
    margin-left: 215px;
    margin-top: -8px;
    margin-bottom: 12px;
}
</style>

<script type="text/javascript">
<!--
function aggiorna_val_reg4 () {
var sel_tar = document.getElementById("t4");
var num_sel = sel_tar.selectedIndex;
var tariffa = sel_tar.options[num_sel].value;
var sel_ute = document.getElementById("v4");
if (tariffa == '') sel_ute.disabled = true;
else sel_ute.disabled = false;
sel_ute.selectedIndex = 0;
var ind_val = '';
<?php echo $lista_tar_reg4; ?>
var num_opt = sel_ute.options.length;
for (n1 = 0 ; n1 < num_opt ; n1++) {
if (sel_ute.options[n1].value == ind_val) sel_ute.selectedIndex = n1;
}
}
-->
</script>

<?php if ($num_tutti_utenti > 1): ?>
<div class="rbox rule4">
    <div class="rheader">
        <?php echo mex("Regola di assegnazione",$pag); ?> 4 (<?php echo mex("utente inserimento",$pag); ?>)
    </div>
    <div class="rcontent">
        
        <?php 
        // Display messages at the top of the panel (only for this panel)
        if ((!empty($success_messages) or !empty($error_messages) or !empty($warning_messages)) and (!empty($active_panel) and $active_panel == 'rule4')) {
            HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
        }
        ?>
        
        <form accept-charset="utf-8" method="post" action="crearegole.php">
            <input type="hidden" name="anno" value="<?php echo $anno; ?>">
            <input type="hidden" name="id_sessione" value="<?php echo $id_sessione; ?>">
            <input type="hidden" name="inserisci" value="SI">
            <?php if ($origine): ?>
                <input type="hidden" name="origine" value="<?php echo htmlspecialchars($origine); ?>">
            <?php endif; ?>
            
            <div class="form-row">
                <label><?php echo mex("Tariffa",$pag); ?>:</label>
                <select name="tipotariffa" id="t4" onchange="aggiorna_val_reg4()">
                    <option value="" selected>----</option>
                    <?php for ($numtariffa = 1 ; $numtariffa <= $numero_tariffe ; $numtariffa++): ?>
                        <?php
                        $tariffa = "tariffa".$numtariffa;
                        $tariffa_vedi = mex("tariffa","prenota.php").$numtariffa;
                        $nometariffa = risul_query($rigatariffe,0,$tariffa);
                        $nometariffa_vedi = ($nometariffa != "") ? " ($nometariffa)" : "";
                        ?>
                        <option value="<?php echo $tariffa; ?>"><?php echo $tariffa_vedi.$nometariffa_vedi; ?></option>
                    <?php endfor; ?>
                </select>
            </div>
            
            <div class="form-row">
                <label><?php echo mex("Assegna a utente",$pag); ?>:</label>
                <select id="v4" name="id_utente_inserimento" disabled>
                    <option value="" selected>----</option>
                    <?php echo $option_select_utenti; ?>
                </select>
            </div>
            
            <div class="rule4-info">
                <?php echo mex("Quando l'utente amministratore",$pag); ?> (<?php echo $nome_utente1; ?>) <?php echo mex("sceglie questa tariffa, fai risultare come se l'utente selezionato avesse inserito la prenotazione e l'eventuale cliente",$pag); ?>.
            </div>
            
            <div class="form-actions">
                <button class="irul" type="submit" name="regola_4" value="1">
                    <div><?php echo mex("Inserisci o modifica la regola",$pag); ?> 4</div>
                </button>
            </div>
        </form>
        
    </div>
</div>
<?php endif; ?>
