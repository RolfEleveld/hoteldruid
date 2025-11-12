<?php
/**
 * Template for Residenza (Residence) Panel
 * Variables expected: Address-related fields
 */
?>
<div class="rbox" style="background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-left: 4px solid #2196F3; margin: 0 0 16px 0; flex: 1 1 420px; min-width: 320px;">
    <div style="background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%); color: white; padding: 10px 14px; border-radius: 6px; margin: -5px 0 12px 0; font-weight: 600;">
        <?php echo mex("Residenza", $pag); ?>
    </div>
    
    <div style="display:flex;gap:8px;align-items:center;margin:8px 0">
        <?php echo mex("Residenza", $pag); ?>:
        <select name="via">
            <option value="<?php echo mex("Via", $pag); ?>"><?php echo mex("Via", $pag); ?></option>
            <option value="<?php echo mex("Piazza", $pag); ?>"><?php echo mex("Piazza", $pag); ?></option>
            <option value="<?php echo mex("Viale", $pag); ?>"><?php echo mex("Viale", $pag); ?></option>
            <option value="<?php echo mex("Piazzale", $pag); ?>"><?php echo mex("Piazzale", $pag); ?></option>
            <option value="<?php echo mex("Vicolo", $pag); ?>"><?php echo mex("Vicolo", $pag); ?></option>
            <?php 
            if ($via) $sel = " selected";
            else $sel = "";
            ?>
            <option value=""<?php echo $sel; ?>>-----</option>
        </select>
        <input type="text" name="nomevia" value="<?php echo htmlspecialchars($via); ?>">
        <span style="white-space:nowrap">
            Nº: <input type="text" name="numcivico" size="4" value="<?php echo htmlspecialchars($numcivico); ?>">
        </span>
    </div>
    
    <span class="wsnw"><?php echo mex("CAP", $pag); ?>: 
        <input type="text" name="cap" size="6" value="<?php echo htmlspecialchars($cap); ?>">
    </span>
    
    <span class="wsnw"><?php echo mex("nazione", $pag); ?>: 
        <?php echo mostra_lista_relutenti("nazione", $nazione, $id_utente, "nome_nazione", "idnazioni", "idnazione", $tablenazioni, $tablerelutenti, "", "", "", "regione", "regione"); ?>
        <input type="button" class="cpbutton" onclick="cp_val('nazione','nazionalita','')" value="#">
    </span>
    
    <span class="wsnw"><?php echo mex("reg./prov.", $pag); ?>: 
        <?php echo mostra_lista_relutenti("regione", $regione, $id_utente, "nome_regione", "idregioni", "idregione", $tableregioni, $tablerelutenti, "", "", "", "citta", "citta", "nazione"); ?>
        <input type="button" class="cpbutton" onclick="cp_val('regione','regionenascita','')" value="#">
    </span>
    
    <span class="wsnw"><?php echo mex("città", $pag); ?>: 
        <?php echo mostra_lista_relutenti("citta", $citta, $id_utente, "nome_citta", "idcitta", "idcitta", $tablecitta, $tablerelutenti, "", "", "", "", "", "regione"); ?>
        <input type="button" class="cpbutton" onclick="cp_val('citta','cittanascita','')" value="#">
    </span>
</div>
