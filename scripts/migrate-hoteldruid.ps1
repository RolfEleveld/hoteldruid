#Requires -Version 7.0
<#
.SYNOPSIS
Migrate codebase from HotelDruid to HotelDruid naming convention.

.DESCRIPTION
Performs case-sensitive find/replace on file contents and renames files/folders:
- HotelDruid -> HotelDruid (PascalCase)
- HotelDruid -> HotelDruid (lowercase)
- HotelDruid -> HotelDruid (UPPERCASE)

.PARAMETER WhatIf
Preview changes without applying them.
#>
param(
    [switch]$WhatIf
)

$ErrorActionPreference = "Stop"
$WarningPreference = "Continue"

# Define replacement rules (order matters for overlapping patterns)
$replacements = @(
    @{ Old = "HotelDruid"; New = "HotelDruid"; Description = "PascalCase project names" },
    @{ Old = "HotelDruid"; New = "HotelDruid"; Description = "lowercase docker/env names" },
    @{ Old = "HotelDruid"; New = "HotelDruid"; Description = "UPPERCASE env vars" }
)

$script:filesUpdated = 0
$script:filesRenamed = 0
$script:dirsRenamed = 0

function Update-FileContent {
    param(
        [string]$FilePath,
        [array]$Replacements
    )
    
    # Skip binary files and git internals
    $skipExtensions = @(".dll", ".exe", ".png", ".jpg", ".jpeg", ".gif", ".zip", ".tar", ".gz", ".pdb", ".bin", ".pyc")
    $skipFolders = @(".git", ".vs", ".vscode", "node_modules", "obj", "bin", ".packages", "artifacts", ".nuget")
    
    # Check if in skip folder
    foreach ($skip in $skipFolders) {
        if ($FilePath -match [regex]::Escape($skip)) {
            return
        }
    }
    
    if ($skipExtensions -contains [System.IO.Path]::GetExtension($FilePath)) {
        return
    }
    
    try {
        $content = Get-Content -Path $FilePath -Raw -ErrorAction SilentlyContinue
        if ($null -eq $content) { return }
        
        $originalContent = $content
        foreach ($replacement in $Replacements) {
            # Case-sensitive replace
            $content = $content -replace ([regex]::Escape($replacement.Old)), $replacement.New
        }
        
        if ($content -ne $originalContent) {
            if ($WhatIf) {
                Write-Host "  [CHANGE] $FilePath" -ForegroundColor Green
            } else {
                Set-Content -Path $FilePath -Value $content -NoNewline -Encoding UTF8
                Write-Host "  ✓ $FilePath" -ForegroundColor Green
            }
            $script:filesUpdated++
        }
    }
    catch {
        Write-Warning "Failed to process $FilePath : $_"
    }
}

function Rename-ItemIfNeeded {
    param(
        [System.IO.FileSystemInfo]$Item,
        [array]$Replacements
    )
    
    $newName = $Item.Name
    foreach ($replacement in $Replacements) {
        $newName = $newName -replace ([regex]::Escape($replacement.Old)), $replacement.New
    }
    
    if ($newName -ne $Item.Name) {
        $newPath = Join-Path -Path $Item.DirectoryName -ChildPath $newName
        
        if ($WhatIf) {
            Write-Host "  [RENAME] $($Item.Name) -> $newName" -ForegroundColor Cyan
        } else {
            Rename-Item -Path $Item.FullName -NewName $newName -Force -ErrorAction SilentlyContinue
            if ($?) {
                if ($Item -is [System.IO.DirectoryInfo]) {
                    Write-Host "  ✓ DIR:  $($Item.Name) -> $newName" -ForegroundColor Cyan
                    $script:dirsRenamed++
                } else {
                    Write-Host "  ✓ FILE: $($Item.Name) -> $newName" -ForegroundColor Cyan
                    $script:filesRenamed++
                }
            }
        }
        return $true
    }
    return $false
}

# Main execution
Write-Host "`n=== HotelDruid Migration Script ===" -ForegroundColor Yellow
Write-Host "Repository: $(Split-Path -Parent (Split-Path $PSScriptRoot))" -ForegroundColor Gray

if ($WhatIf) {
    Write-Host "[WhatIf] Preview mode - no changes will be applied`n" -ForegroundColor Yellow
} else {
    Write-Host "[Live] Applying changes...`n" -ForegroundColor Yellow
}

# Phase 1: Update file contents
Write-Host "Phase 1: Updating file contents..." -ForegroundColor Blue
$allFiles = @(Get-ChildItem -Path (Split-Path $PSScriptRoot) -Recurse -File -ErrorAction SilentlyContinue)
Write-Host "  Scanning $($allFiles.Count) files..." -ForegroundColor Gray
foreach ($file in $allFiles) {
    Update-FileContent -FilePath $file.FullName -Replacements $replacements
}

# Phase 2: Rename items (process deepest first to avoid path issues)
Write-Host "`nPhase 2: Renaming files and directories..." -ForegroundColor Blue
$allItems = @(Get-ChildItem -Path (Split-Path $PSScriptRoot) -Recurse -ErrorAction SilentlyContinue | 
    Where-Object { $_.FullName -notmatch "\.git" } |
    Sort-Object -Property { $_.FullName.Length } -Descending)

Write-Host "  Processing $($allItems.Count) items..." -ForegroundColor Gray
foreach ($item in $allItems) {
    Rename-ItemIfNeeded -Item $item -Replacements $replacements
}

# Summary
Write-Host "`n=== Migration Summary ===" -ForegroundColor Yellow
Write-Host "  Files updated: $script:filesUpdated" -ForegroundColor Green
Write-Host "  Files renamed: $script:filesRenamed" -ForegroundColor Cyan
Write-Host "  Dirs renamed:  $script:dirsRenamed" -ForegroundColor Cyan

if ($WhatIf) {
    Write-Host "`n[WhatIf] Preview complete. Run without -WhatIf to apply changes." -ForegroundColor Yellow
} else {
    Write-Host "`n✓ Migration complete!" -ForegroundColor Green
    Write-Host "`nNext steps:" -ForegroundColor Yellow
    Write-Host "  1. git status" -ForegroundColor Gray
    Write-Host "  2. git add -A" -ForegroundColor Gray
    Write-Host "  3. git commit -m 'Rename HotelDruid -> HotelDruid'" -ForegroundColor Gray
    Write-Host "  4. Rename parent folder from 'HotelDruid' to 'HotelDruid'" -ForegroundColor Gray
}

