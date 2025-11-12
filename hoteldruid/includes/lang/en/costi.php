<?php

switch ($messaggio) {

case "Costi Gestione":  			$messaggio = "Financial Operations"; break;
case "Si deve inserire un nome per l' entrata":	$messaggio = "You must enter a description for the income"; break;
case "Si deve inserire il valore dell' entrata":	$messaggio = "You must enter the income amount"; break;
case "L'entrata è stata inserita":  		$messaggio = "Income has been recorded"; break;
case "Si deve inserire un nome per la spesa":	$messaggio = "You must enter a description for the expense"; break;
case "Si deve inserire il valore della spesa":	$messaggio = "You must enter the expense amount"; break;
case "La spesa è stata inserita":  		$messaggio = "Expense has been recorded"; break;
case "Inserisci i costi di gestione per l'anno":	$messaggio = "Record operating expenses for year"; break;
case "Natura spesa":  				$messaggio = "Expense description"; break;
case "Importo":  				$messaggio = "Amount"; break;
case "Persona che inserisce":  			$messaggio = "Person inserting"; break;
case "opzionale":  				$messaggio = "optional"; break;
case "Inserisci la spesa":  			$messaggio = "Record expense"; break;
case "Inserisci le entrate in cassa per l'anno":	$messaggio = "Record income for year"; break;
case "Natura entrata":  			$messaggio = "Income description"; break;
case "Inserisci l' entrata":  			$messaggio = "Record income"; break;
case "Sottrai l'importo dal totale delle prenotazioni":	$messaggio = "Subtract the amount from total of the reservations"; break;
case "Visualizza la tabella con tutte le spese e le entrate":	$messaggio = "View all income and expenses"; break;
case "Torna al menù principale":  		$messaggio = "Back to main menu"; break;
case "Il valore dell' entrata è sbagliato":  	$messaggio = "The income amount is invalid"; break;
case "Il valore della spesa è sbagliato":  	$messaggio = "The expense amount is invalid"; break;
case "Nessuna cassa disponibile":  		$messaggio = "No cash register available"; break;
case "Cassa":  					$messaggio = "Cash Register"; break;
case "cassa principale":  			$messaggio = "main register"; break;
case "Metodo di pagamento":  			$messaggio = "Payment method"; break;
case "Data operazione":  			$messaggio = "Transaction date"; break;
case "id op.":  				$messaggio = "trans. ID"; break;
case "OK":  					$messaggio = "OK"; break;
case "":  		$messaggio = ""; break;
case "":  		$messaggio = ""; break;

} # fine switch ($messaggio)

?>