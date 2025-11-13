<?php if (!empty($show_rule1b_confirmation) && !empty($rule1b_confirmation_data)): ?>
<div class="rule-confirmation-box" style="background: linear-gradient(135deg, #fff3cd 0%, #ffeaa7 100%); border-left: 5px solid #ffc107; padding: 20px; margin: 20px 0; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);">
    <form accept-charset="utf-8" method="post" action="crearegole.php">
        <div class="linhbox">
            <input type="hidden" name="anno" value="<?php echo htmlspecialchars($anno); ?>">
            <input type="hidden" name="id_sessione" value="<?php echo htmlspecialchars($id_sessione); ?>">
            <input type="hidden" name="inserisci" value="SI">
            <input type="hidden" name="regola_1_tar" value="1">
            <input type="hidden" name="mod_idregola1" value="<?php echo htmlspecialchars($rule1b_confirmation_data['mod_idregola1']); ?>">
            <input type="hidden" name="origine" value="<?php echo htmlspecialchars(fixstr($origine)); ?>">
            <input type="hidden" name="cancella_vecchie_regole" value="1">
            <input type="hidden" name="tipotariffa" value="<?php echo htmlspecialchars($rule1b_confirmation_data['tipotariffa']); ?>">
            <input type="hidden" name="inizioperiodo" value="<?php echo htmlspecialchars($rule1b_confirmation_data['inizioperiodo']); ?>">
            <input type="hidden" name="fineperiodo" value="<?php echo htmlspecialchars($rule1b_confirmation_data['fineperiodo']); ?>">
            <input type="hidden" name="motivazione" value="<?php echo htmlspecialchars(fixstr($rule1b_confirmation_data['motivazione'])); ?>">
            
            <p style="margin-top: 0;"><em><?php echo mex("Regole esistenti",$pag); ?>:</em></p>
            
            <?php 
            $vecchia_regola = $rule1b_confirmation_data['vecchia_regola'];
            $num_vecchia_regola = $rule1b_confirmation_data['num_vecchia_regola'];
            $tariffa_vedi = $rule1b_confirmation_data['tariffa_vedi'];
            $idinizioperiodo = $rule1b_confirmation_data['idinizioperiodo'];
            $idfineperiodo = $rule1b_confirmation_data['idfineperiodo'];
            
            for ($num1 = 0; $num1 < $num_vecchia_regola; $num1++): 
                $idregola_v = risul_query($vecchia_regola, $num1, 'idregole');
                $iddatainizio_v = risul_query($vecchia_regola, $num1, 'iddatainizio');
                $iddatafine_v = risul_query($vecchia_regola, $num1, 'iddatafine');
                $motivazione_v = risul_query($vecchia_regola, $num1, 'motivazione');
                $datainizio_v = esegui_query("select datainizio from $tableperiodi where idperiodi = '$iddatainizio_v'");
                $datainizio_v = formatta_data(risul_query($datainizio_v, 0, 'datainizio'), $stile_data);
                $datafine_v = esegui_query("select datafine from $tableperiodi where idperiodi = '$iddatafine_v'");
                $datafine_v = formatta_data(risul_query($datafine_v, 0, 'datafine'), $stile_data);
            ?>
                <div style="margin: 10px 0; padding: 10px; background: rgba(255,255,255,0.5); border-radius: 4px;">
                    <?php echo ($num1 + 1); ?>- <?php echo mex("Chiudi", $pag); ?> <?php echo htmlspecialchars($tariffa_vedi); ?> <?php echo mex("nel periodo dal", $pag); ?> <?php echo htmlspecialchars($datainizio_v); ?> <?php echo mex("al", $pag); ?> <?php echo htmlspecialchars($datafine_v); ?>
                    <?php if (strcmp(trim($motivazione_v), "")): ?>
                        (<em><?php echo htmlspecialchars($motivazione_v); ?></em>)
                    <?php endif; ?>
                    .<select name="creg<?php echo $idregola_v; ?>">
                        <?php if ($iddatainizio_v < $idinizioperiodo || $iddatafine_v > $idfineperiodo): ?>
                            <option value="" selected><?php echo mex("Ridimensiona", $pag); ?></option>
                        <?php endif; ?>
                        <option value="1"><?php echo mex("Cancella", $pag); ?></option>
                    </select>
                    <input type="hidden" name="nome_creg_passa<?php echo $num1; ?>" value="<?php echo $idregola_v; ?>">
                </div>
            <?php endfor; ?>
            
            <input type="hidden" name="num_creg_passa" value="<?php echo $num_vecchia_regola; ?>">
            
            <button class="cont" type="submit" style="margin-top: 15px;">
                <div><?php echo mex("Cancella o ridimensiona queste regole", $pag); ?></div>
            </button>
        </div>
    </form>
</div>
<?php endif; ?>
