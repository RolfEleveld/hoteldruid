<?php
/**
 * Template for Documento (Document) Panel
 * Variables expected: Document-related fields
 */
?>
<div class="rbox" style="background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-left: 4px solid #FF9800; margin: 0 0 16px 0; flex: 1 1 420px; min-width: 320px;">
    <div style="background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%); color: white; padding: 10px 14px; border-radius: 6px; margin: -5px 0 12px 0; font-weight: 600;">
        <?php echo mex("Documento", $pag); ?>
    </div>
    
    <span class="wsnw smlscrfnt"><?php echo mex("Documento", $pag); ?>: 
        <?php echo mostra_lista_relutenti("tipodoc", $tipodoc, $id_utente, "nome_documentoid", "iddocumentiid", "iddocumentoid", $tabledocumentiid, $tablerelutenti, "", "", "SI"); ?>
        <input type="text" name="documento" value="<?php echo htmlspecialchars($documento); ?>">
    </span>
    
    <span class="wsnw"><?php echo mex("scadenza", $pag); ?>: 
        <?php 
        if ($stile_data == "usa") echo "$sel_mscaddoc/$sel_gscaddoc";
        else echo "$sel_gscaddoc/$sel_mscaddoc";
        ?>
        /<select name="annoscaddoc">
            <?php 
            $anno_corr = date("Y", (time() + (C_DIFF_ORE * 3600)));
            for ($num1 = 0; $num1 < 12; $num1++) {
                $num = $anno_corr - 12 + $num1;
                echo "<option value=\"$num\">$num</option>";
            }
            echo "<option value=\"\" selected>--</option>";
            for ($num1 = 0; $num1 < 16; $num1++) {
                $num = $anno_corr + $num1;
                echo "<option value=\"$num\">$num</option>";
            }
            ?>
        </select>
    </span>
    
    <span class="wsnw smlscrfnt"><?php echo mex("nazione di rilascio", $pag); ?>: 
        <?php echo mostra_lista_relutenti("nazionedoc", $nazionedoc, $id_utente, "nome_nazione", "idnazioni", "idnazione", $tablenazioni, $tablerelutenti, "", "", "", "regione", "regionedoc"); ?>
        <input type="button" class="cpbutton" onclick="cp_val('nazionedoc','nazionalita','')" value="#">
    </span>
    
    <span class="wsnw"><?php echo mex("reg./prov.", $pag); ?>: 
        <?php echo mostra_lista_relutenti("regionedoc", $regionedoc, $id_utente, "nome_regione", "idregioni", "idregione", $tableregioni, $tablerelutenti, "", "", "", "citta", "cittadoc", "nazione"); ?>
        <input type="button" class="cpbutton" onclick="cp_val('regionedoc','regione','')" value="#">
    </span>
    
    <span class="wsnw"><?php echo mex("città", $pag); ?>: 
        <?php echo mostra_lista_relutenti("cittadoc", $cittadoc, $id_utente, "nome_citta", "idcitta", "idcitta", $tablecitta, $tablerelutenti, "", "", "", "", "", "regione"); ?>
        <input type="button" class="cpbutton" onclick="cp_val('cittadoc','citta','')" value="#">
    </span>
</div>
