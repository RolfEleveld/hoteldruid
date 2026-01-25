<#
List CurrentUser localhost certificates
#>
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store('My','CurrentUser')
$store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
$found = $store.Certificates | Where-Object { $_.Subject -like '*CN=localhost*' -or ($_.Subject -like '*localhost*') }
if ($found -and $found.Count -gt 0) {
    foreach ($c in $found) {
        Write-Host "Subject: $($c.Subject)"
        Write-Host "Thumbprint: $($c.Thumbprint)"
        Write-Host "HasPrivateKey: $($c.HasPrivateKey)"
        Write-Host "NotAfter: $($c.NotAfter)"
        Write-Host '---'
    }
} else {
    Write-Host 'No CurrentUser localhost certificates found.'
}
$store.Close()
