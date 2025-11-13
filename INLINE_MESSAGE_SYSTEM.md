# Inline Message System - Implementation Guide

## Overview
The inline message system provides consistent user feedback across all HotelDruid pages by displaying success, error, and warning messages at the top of the page without redirects or intermediate pages.

## Benefits
- ✅ User stays on the same page after form submission
- ✅ Consistent look and feel across all pages
- ✅ No "headers already sent" errors
- ✅ No blank intermediate pages
- ✅ Supports multiple messages of each type
- ✅ Animated, color-coded message boxes
- ✅ Easy to implement in any page

## Implementation Steps

### Step 1: Initialize Message Arrays
Add these lines near the top of your PHP file (after database connection):

```php
// Initialize message arrays for success, error, and warning messages
$success_messages = array();
$error_messages = array();
$warning_messages = array();
```

### Step 2: Include Template System and Display Messages
After `head.php` is included, add:

```php
// Include template system
require_once("./includes/template.php");

// Display messages at the top of the page
if (!empty($success_messages) or !empty($error_messages) or !empty($warning_messages)) {
    HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
}
```

### Step 3: Replace Echo Statements with Message Arrays
Instead of echoing messages directly:

```php
// OLD WAY - Don't do this
echo "La regola è stata inserita.<br>";
echo "<span class=\"colwarn\">Errore: dati mancanti</span><br>";
```

Use message arrays:

```php
// NEW WAY - Do this
$success_messages[] = mex("La regola è stata inserita", $pag);
$error_messages[] = mex("Errore: dati mancanti", $pag);
$warning_messages[] = mex("Attenzione: verificare i dati", $pag);
```

### Step 4: Modify Form Processing Flow
Ensure the page shows the main form after processing instead of redirecting:

```php
if (!empty($inserisci)) {
    // Process form here
    // Add success/error messages to arrays
    
    // Set flag to show main form after processing
    $show_main_form = true;
}

// Show the main form (whether after processing or initial load)
if (empty($inserisci) or !empty($show_main_form)) {
    // Display your form here
}
```

## Message Types

### Success Messages (Green)
Use for successful operations:
```php
$success_messages[] = "Operation completed successfully";
$success_messages[] = mex("La regola è stata inserita", $pag);
```

### Error Messages (Red)
Use for errors and validation failures:
```php
$error_messages[] = "Please fill in all required fields";
$error_messages[] = mex("Si deve scegliere la tariffa", $pag);
```

### Warning Messages (Yellow/Orange)
Use for warnings and confirmations needed:
```php
$warning_messages[] = "This action requires confirmation";
$warning_messages[] = mex("Esiste già una regola in questo periodo", $pag);
```

## Confirmation Forms
For confirmations that need user action (e.g., overlapping rules), create a template and display it in the appropriate panel:

```php
// In processing section
if ($overlapping_rule_found && empty($user_confirmed)) {
    $show_confirmation = true;
    $confirmation_data = array(/* ... data ... */);
    $warning_messages[] = mex("Overlap detected", $pag);
}

// In template display
HotelDruidTemplate::getInstance()->display('yourpage/confirmation', get_defined_vars());
```

## Example: Complete Implementation

```php
<?php
// ... standard includes ...

// Initialize message arrays
$success_messages = array();
$error_messages = array();
$warning_messages = array();

// ... database setup ...

// FORM PROCESSING
if (!empty($inserisci)) {
    if (empty($required_field)) {
        $error_messages[] = mex("Campo obbligatorio mancante", $pag);
    }
    else {
        // Process the form
        esegui_query("INSERT INTO ...");
        $success_messages[] = mex("Dati salvati con successo", $pag);
    }
    
    // Always show main form after processing
    $show_main_form = true;
}

// INCLUDE HEAD
include("./includes/head.php");

// DISPLAY MESSAGES
require_once("./includes/template.php");
if (!empty($success_messages) or !empty($error_messages) or !empty($warning_messages)) {
    HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
}

// SHOW FORM
if (empty($inserisci) or !empty($show_main_form)) {
    // Display your main form here
    ?>
    <form method="post" action="yourpage.php">
        <!-- form fields -->
    </form>
    <?php
}
?>
```

## Testing Checklist

After implementing the message system:

- [ ] Submit valid form → Success message appears at top, stay on same page
- [ ] Submit invalid form → Error message appears at top, stay on same page
- [ ] Trigger confirmation → Warning appears with confirmation form inline
- [ ] Multiple messages → All display correctly stacked
- [ ] No "headers already sent" errors
- [ ] No blank or intermediate pages
- [ ] Messages are properly translated using mex()

## Files Reference

- **Common message template:** `includes/templates/common/messages.php`
- **Example implementation:** `crearegole.php`
- **Confirmation template example:** `includes/templates/crearegole/rule1_confirmation.php`

## Migration Tips

1. Search for `echo mex(` statements in form processing sections
2. Replace with `$success_messages[]`, `$error_messages[]`, or `$warning_messages[]`
3. Search for `header("Location:")` redirects and remove them
4. Ensure form displays after processing by setting `$show_main_form = true`
5. Test thoroughly with different form submissions

## Notes

- The message system uses CSS animations for smooth appearance
- Messages are automatically HTML-escaped for security
- Multiple messages of the same type are displayed in sequence
- The template is responsive and works on mobile devices
- Icons are Unicode characters (✓, ✕, ⚠) - no image dependencies
