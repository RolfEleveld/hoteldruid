<?php
/**
 * Template for Contatti (Contacts) Panel
 * Variables expected: Contact information fields
 */
?>
<div class="rbox" style="background: white; padding: 25px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.08); border-left: 4px solid #9C27B0; margin: 0 0 16px 0; flex: 1 1 420px; min-width: 320px;">
    <div style="background: linear-gradient(135deg, #9C27B0 0%, #7B1FA2 100%); color: white; padding: 10px 14px; border-radius: 6px; margin: -5px 0 12px 0; font-weight: 600;">
        <?php echo mex("Contatti", $pag); ?>
    </div>
    
    <span class="wsnw smlscrfnt"><?php echo mex("Numero di telefono", $pag); ?>: 
        <input type="text" name="telefono" value="<?php echo htmlspecialchars($telefono); ?>">
    </span>
    
    <span class="wsnw smlscrfnt"><?php echo mex("Secondo telefono", $pag); ?>: 
        <input type="text" name="telefono2" value="<?php echo htmlspecialchars($telefono2); ?>">
    </span>
    
    <span class="wsnw smlscrfnt"><?php echo mex("Terzo telefono", $pag); ?>: 
        <input type="text" name="telefono3" value="<?php echo htmlspecialchars($telefono3); ?>">
    </span>
    
    <input type="hidden" name="fax" value="<?php echo htmlspecialchars($fax); ?>">
    
    <span class="wsnw smlscrfnt"><?php echo mex("E-mail", $pag); ?>: 
        <input type="text" name="email" size="30" value="<?php echo htmlspecialchars($email); ?>">
    </span>
    
    <span class="wsnw smlscrfnt"><?php echo mex("Seconda e-mail", $pag); ?>: 
        <input type="text" name="email2" size="30" value="<?php echo htmlspecialchars($email2); ?>">
    </span>
    
    <span class="wsnw smlscrfnt"><?php echo mex("E-mail certificata (PEC) o codice destinatario", $pag); ?>: 
        <input type="text" name="email_cert" size="30" value="<?php echo htmlspecialchars($email_cert); ?>">
    </span>
</div>
