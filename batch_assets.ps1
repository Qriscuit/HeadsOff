# batch_assets.ps1 - Batched Asset Upload for Unity

Write-Host "==> Starting Incremental Asset Upload..." -ForegroundColor Green

# 1. Get all top-level subfolders inside Assets/
$subfolders = Get-ChildItem -Path "Assets" -Directory

foreach ($folder in $subfolders) {
    $folderPath = "Assets/$($folder.Name)"
    Write-Host "--> Processing subfolder: $folderPath" -ForegroundColor Yellow

    # Stage only this subfolder
    git add "$folderPath"

    # Check if anything was staged
    $status = git status --porcelain
    if ($status) {
        git commit -m "Feat: Upload $folderPath assets"
        Write-Host "==> Pushing $folderPath to GitHub..." -ForegroundColor Cyan
        git push origin main
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "ERROR: Push failed on $folderPath. Script paused so you can inspect." -ForegroundColor Red
            break
        }
        Write-Host "SUCCESS: $folderPath is on GitHub!" -ForegroundColor Green
    } else {
        Write-Host "--> No untracked changes in $folderPath, skipping." -ForegroundColor DarkGray
    }
}

# 2. Final catch-all for loose files directly under Assets/ (scenes, materials, root .meta files)
Write-Host "==> Staging loose files directly under Assets/ and Root..." -ForegroundColor Cyan
git add Assets/ .
$finalStatus = git status --porcelain
if ($finalStatus) {
    git commit -m "Chore: Finalize remaining loose project assets"
    git push origin main
    Write-Host "SUCCESS: Loose assets pushed!" -ForegroundColor Green
}

Write-Host "==> ALL ASSETS SUCCESSFULLY PUSHED TO GITHUB!" -ForegroundColor Green