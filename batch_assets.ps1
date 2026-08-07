# push_files_individually.ps1 - Pushes Assets File-by-File

Write-Host "==> Gathering all files inside Assets/..." -ForegroundColor Cyan

# Fetch all individual files inside Assets (recursively)
$files = Get-ChildItem -Path "Assets" -Recurse -File

$total = $files.Count
$current = 0
$failedFiles = @()

Write-Host "==> Found $total individual files. Starting file-by-file upload..." -ForegroundColor Green

foreach ($file in $files) {
    $current++
    # Get the relative path (e.g., "Assets/Textures/Rock_Diffuse.png")
    $relativePath = Resolve-Path -Relative $file.FullName

    # Check if Git sees this file as needing an update/upload
    git add "$relativePath"
    $status = git status --porcelain "$relativePath"

    if ($status) {
        Write-Host "[$current/$total] Pushing file: $relativePath" -ForegroundColor Yellow
        
        git commit -m "Feat: Upload $relativePath" | Out-Null
        git push origin main

        if ($LASTEXITCODE -eq 0) {
            Write-Host "  └--> SUCCESS: $relativePath is live on GitHub!" -ForegroundColor Green
        } else {
            Write-Host "  └--> ERROR: Failed to push $relativePath!" -ForegroundColor Red
            $failedFiles += $relativePath
            # Reset stage for this file so it doesn't block the next ones
            git reset "$relativePath" | Out-Null
        }
    } else {
        Write-Host "[$current/$total] Skipped (already on server or ignored): $relativePath" -ForegroundColor DarkGray
    }
}

Write-Host "`n==========================================" -ForegroundColor Cyan
Write-Host "FILE-BY-FILE UPLOAD PROCESS COMPLETE!" -ForegroundColor Green
Write-Host "Total Processed: $total" -ForegroundColor Green

if ($failedFiles.Count -gt 0) {
    Write-Host "`nThe following $($failedFiles.Count) file(s) failed to push (check if any individual file is > 100MB):" -ForegroundColor Red
    foreach ($failed in $failedFiles) {
        Write-Host " - $failed" -ForegroundColor Red
    }
} else {
    Write-Host "All files were uploaded cleanly with zero errors!" -ForegroundColor Green
}