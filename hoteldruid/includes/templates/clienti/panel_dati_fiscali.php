<?php
/**
 * Template for Dati Fiscali (Tax Data) Panel
 * Variables expected: Tax-related fields
 */
?>
<div class="rbox" style="background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-left: 4px solid #00BCD4; margin: 0 0 16px 0; flex: 1 1 420px; min-width: 320px;">
    <div style="background: linear-gradient(135deg, #00BCD4 0%, #0097A7 100%); color: white; padding: 10px 14px; border-radius: 6px; margin: -5px 0 12px 0; font-weight: 600;">
        <?php echo mex("Dati fiscali", $pag); ?>
    </div>
    
    <span class="wsnw smlscrfnt"><?php echo mex("Codice fiscale", $pag); ?>: 
        <input type="text" name="cod_fiscale" value="<?php echo htmlspecialchars($cod_fiscale); ?>">
    </span>
    
    <span class="wsnw smlscrfnt"><?php echo mex("Partita iva", $pag); ?>: 
        <input type="text" name="partita_iva" value="<?php echo htmlspecialchars($partita_iva); ?>">
    </span>
</div>
