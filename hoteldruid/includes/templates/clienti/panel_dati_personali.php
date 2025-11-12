<?php
/**
 * Template for Dati Personali (Personal Data) Panel
 * Variables expected: All client personal data fields
 */
?>
<div class="rbox" style="background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-left: 4px solid #4CAF50; margin: 0 0 16px 0; flex: 1 1 420px; min-width: 320px;">
    <div style="background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%); color: white; padding: 10px 14px; border-radius: 6px; margin: -5px 0 12px 0; font-weight: 600;">
        <?php echo mex("Dati personali", $pag); ?>
    </div>
    
    <span class="wsnw"><?php echo mex("Cognome", $pag); ?>: 
        <?php if ($attiva_prefisso_clienti == "p") echo $prefisso_clienti; ?>
        <input type="text" id="cognome" name="cognome" value="<?php echo htmlspecialchars($cognome_mostra); ?>">
        <?php if ($attiva_prefisso_clienti == "s") echo $prefisso_clienti; ?>
    </span>
    
    <span class="wsnw"><?php echo mex("nome", $pag); ?>: 
        <input type="text" name="nome" value="<?php echo htmlspecialchars($nome_mostra); ?>">
    </span>
    
    <span class="wsnw"><?php echo mex("soprannome", $pag); ?>: 
        <input type="text" name="soprannome" value="<?php echo htmlspecialchars($soprannome); ?>">
    </span>
    
    <span class="wsnw"><?php echo mex("sesso", $pag); ?>: 
        <select name="sesso">
            <option value="" selected>-</option>
            <option value="m"<?php echo $sel_m; ?>>m</option>
            <option value="f"<?php echo $sel_f; ?>>f</option>
        </select>
    </span>
    
    <span class="wsnw"><?php echo mex("cittadinanza", $pag); ?>: 
        <?php echo mostra_lista_relutenti("nazionalita", $nazionalita, $id_utente, "nome_nazione", "idnazioni", "idnazione", $tablenazioni, $tablerelutenti); ?>
        <input type="button" class="cpbutton" onclick="cp_val('nazionalita','nazione','')" value="#">
    </span>
    
    <span class="wsnw"><?php echo mex("lingua", $pag); ?>: 
        <select name="lingua_cli">
            <?php echo $opt_lingue; ?>
        </select>
    </span>
    
    <?php if (!empty($datiprenota)): ?>
    .&nbsp;&nbsp; <span class="wsnw"><label><input name="cliente_ospite" value="SI" type="checkbox" checked>
        <?php echo mex("Ospite della prenotazione", $pag); ?></label>
        <?php if ($num_tipologie > 1 or $num_app_richiesti1 > 1): ?>
            <select name="prenota_cli_osp" selected>
                <?php 
                for ($n_t = 1; $n_t <= $num_tipologie; $n_t++) {
                    for ($num1 = 1; $num1 <= ${"num_app_richiesti".$n_t}; $num1++) {
                        echo "<option value=\"p$num1"."_$n_t\">$num1";
                        if ($num_tipologie > 1) echo " ".mex("tipologia", $pag)." $n_t";
                        echo "</option>";
                    }
                }
                ?>
            </select>
        <?php else: ?>
            <input type="hidden" name="prenota_cli_osp" value="p1_1">
        <?php endif; ?>
    </span>
    <?php endif; ?>
    
    <?php echo mostra_funzjs_dati_rel("", "", $id_sessione, $anno); ?>
    
    <span class="wsnw"><?php echo mex("Data di nascita", $pag); ?>:
        <?php 
        if ($stile_data == "usa") echo "$sel_mnascita/$sel_gnascita";
        else echo "$sel_gnascita/$sel_mnascita";
        ?>
        /<input type="text" name="annonascita" size="5" maxlength="4" value="<?php echo htmlspecialchars($annonascita); ?>">
    (<?php echo mex("anno con 4 cifre", $pag); ?>)</span>
    
    <span class="wsnw smlscrfnt"><?php echo mex("nazione di nascita", $pag); ?>: 
        <?php echo mostra_lista_relutenti("nazionenascita", $nazionenascita, $id_utente, "nome_nazione", "idnazioni", "idnazione", $tablenazioni, $tablerelutenti, "", "", "", "regione", "regionenascita"); ?>
        <input type="button" class="cpbutton" onclick="cp_val('nazionenascita','nazionalita','')" value="#">
    </span>
    
    <span class="wsnw smlscrfnt"><?php echo mex("reg./prov. di nascita", $pag); ?>: 
        <?php echo mostra_lista_relutenti("regionenascita", $regionenascita, $id_utente, "nome_regione", "idregioni", "idregione", $tableregioni, $tablerelutenti, "", "", "", "citta", "cittanascita", "nazione"); ?>
        <input type="button" class="cpbutton" onclick="cp_val('regionenascita','regione','')" value="#">
    </span>
    
    <span class="wsnw smlscrfnt"><?php echo mex("città di nascita", $pag); ?>: 
        <?php echo mostra_lista_relutenti("cittanascita", $cittanascita, $id_utente, "nome_citta", "idcitta", "idcitta", $tablecitta, $tablerelutenti, "", "", "", "", "", "regione"); ?>
        <input type="button" class="cpbutton" onclick="cp_val('cittanascita','citta','')" value="#">
    </span>
</div>
