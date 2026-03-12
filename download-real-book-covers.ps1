# Download real book cover images from Vietnamese bookstores
# Note: Replace URLs with actual working links from Tiki/Fahasa

Write-Host "📥 Downloading real book cover images..." -ForegroundColor Cyan
Write-Host ""

$books = @(
    @{
        Name = "nha-gia-kim.jpg"
        Url = "https://cdn0.fahasa.com/media/catalog/product/n/h/nha_gia_kim_1.jpg"
    },
    @{
        Name = "cay-cam-ngot.jpg"
        Url = "https://cdn0.fahasa.com/media/catalog/product/c/a/cay-cam-ngot-cua-toi.jpg"
    },
    @{
        Name = "mat-biec.jpg"
        Url = "https://cdn0.fahasa.com/media/catalog/product/m/a/mat-biec.jpg"
    },
    @{
        Name = "dac-nhan-tam.jpg"
        Url = "https://cdn0.fahasa.com/media/catalog/product/d/a/dac-nhan-tam.jpg"
    }
    # Add more URLs as needed
)

$outputDir = "wwwroot\images\products"

if (!(Test-Path $outputDir)) {
    New-Item -Path $outputDir -ItemType Directory -Force | Out-Null
}

foreach ($book in $books) {
    $outputPath = Join-Path $outputDir $book.Name
    
    try {
        Write-Host "Downloading: $($book.Name)..." -NoNewline
        Invoke-WebRequest -Uri $book.Url -OutFile $outputPath -ErrorAction Stop
        Write-Host " ✅" -ForegroundColor Green
    }
    catch {
        Write-Host " ❌ Failed" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "✅ Download complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Note: Some downloads may have failed due to:" -ForegroundColor Yellow
Write-Host "  - Invalid URLs" -ForegroundColor Gray
Write-Host "  - Network issues" -ForegroundColor Gray
Write-Host "  - Anti-scraping measures" -ForegroundColor Gray
Write-Host ""
Write-Host "💡 Alternative: Download images manually from:" -ForegroundColor Cyan
Write-Host "  - Tiki.vn" -ForegroundColor White
Write-Host "  - Fahasa.com" -ForegroundColor White
Write-Host "  - Google Images (search: '[Book Title] book cover')" -ForegroundColor White
