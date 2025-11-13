<?php if (!empty($show_delete_rule1_confirmation)): ?>
<div class="rule-confirmation-box" style="background: linear-gradient(135deg, #ffebee 0%, #ffcdd2 100%); border-left: 5px solid #f44336; padding: 20px; margin: 20px 0; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);">
    <form accept-charset="utf-8" method="post" action="crearegole.php">
        <div>
            <input type="hidden" name="anno" value="<?php echo htmlspecialchars($anno); ?>">
            <input type="hidden" name="id_sessione" value="<?php echo htmlspecialchars($id_sessione); ?>">
            <input type="hidden" name="inserisci" value="SI">
            <input type="hidden" name="canc_regola_1" value="1">
            
            <button class="crul" type="submit" name="gia_stato" value="1" style="margin-top: 10px;">
                <div>  <?php echo mex("SI",$pag); ?>  </div>
            </button>
        </div>
    </form>
</div>
<?php endif; ?>
