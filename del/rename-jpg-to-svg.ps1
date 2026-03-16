# Rename all .jpg files to .svg (because they are actually SVG files)
Get-ChildItem -Path "wwwroot\images\products\*.jpg" | ForEach-Object {
    $newName = $_.Name -replace '\.jpg$', '.svg'
    Rename-Item -Path $_.FullName -NewName $newName
    Write-Host "✅ Renamed: $($_.Name) → $newName" -ForegroundColor Green
}

Write-Host ""
Write-Host "✅ All files renamed to .svg" -ForegroundColor Cyan
