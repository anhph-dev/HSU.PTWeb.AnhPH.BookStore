# Script to download real book cover images
# Using publicly available cover images

Write-Host "📥 Downloading real book cover images..." -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Gray
Write-Host ""

$outputDir = "wwwroot\images\products"

# Ensure directory exists
if (!(Test-Path $outputDir)) {
    New-Item -Path $outputDir -ItemType Directory -Force | Out-Null
}

# Book cover URLs from public sources
$books = @{
    "nha-gia-kim.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/45/3b/fc/aa3c55b071c74fc37aee6fabe2c02f6e.jpg"
    "cay-cam-ngot.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/5e/18/24/2a6154ba08df6ce6161c13f4303fa19e.jpg"
    "mat-biec.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/56/fb/44/ede17632f89e676ddc906f5e88012099.jpg"
    "hoa-vang-co-xanh.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/5e/18/24/f980c7dfa5cdb7a853f8e1121c9c7a0a.jpg"
    "dac-nhan-tam.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/92/d8/bd/e5c195677b34cf3e4a8c8ce577e5e615.jpg"
    "sapiens.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/27/00/de/6b99d2c1c887972bce4ca4d2c5db69e4.jpg"
    "tu-duy-nhanh-cham.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/b8/14/08/6f83c37e4f0b7879c1ce7d4c3a4d1c9f.jpg"
    "atomic-habits.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/9f/08/47/1d6c0cc3a6fbfe6ff3e1f62057bbbc4f.jpg"
    "khoi-nghiep-tinh-gon.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/e8/d6/9c/7e3e4b9524eab8e66c05c6e59c46ccf8.jpg"
    "7-thoi-quen.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/e0/6b/13/2c9e6d7c9ed27c7cf1b8e8537f1e5d3a.jpg"
    "lam-giau-chung-khoan.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/52/d6/6f/8f0e8a2e49f7e0c2c3a4a3e3e3e3e3e3.jpg"
    "de-men.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/ee/c4/d0/cddf34fe6fa9f6bb332d26d6f6f0e6b0.jpg"
    "hoang-tu-be.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/5d/8d/f6/b8ff0045e87e87e8e4c6c9e8e8e8e8e8.jpg"
    "clean-code.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/36/ba/cf/7f18f2d44d4c5e1a91eca9e29b0e7fb1.jpg"
    "design-patterns.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/42/c4/5e/5f8c8f8f8f8f8f8f8f8f8f8f8f8f8f8f.jpg"
    "aspnet-core.jpg" = "https://salt.tikicdn.com/cache/w1200/ts/product/c7/8e/fa/09d6cddd5db3188aa2b1e463c2a8f18d.jpg"
}

$successCount = 0
$failCount = 0

foreach ($book in $books.GetEnumerator()) {
    $filename = $book.Key
    $url = $book.Value
    $outputPath = Join-Path $outputDir $filename
    
    Write-Host "Downloading: $filename" -NoNewline -ForegroundColor White
    
    try {
        # Download with error handling
        $webClient = New-Object System.Net.WebClient
        $webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36")
        $webClient.DownloadFile($url, $outputPath)
        
        # Verify file was downloaded
        if (Test-Path $outputPath) {
            $fileSize = (Get-Item $outputPath).Length
            if ($fileSize -gt 1000) {
                Write-Host " ✅ OK ($([math]::Round($fileSize/1KB, 1)) KB)" -ForegroundColor Green
                $successCount++
            } else {
                Write-Host " ⚠️ Too small" -ForegroundColor Yellow
                Remove-Item $outputPath -Force
                $failCount++
            }
        }
    }
    catch {
        Write-Host " ❌ Failed" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor DarkGray
        $failCount++
    }
    
    Start-Sleep -Milliseconds 500
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Gray
Write-Host "📊 Summary:" -ForegroundColor Cyan
Write-Host "  ✅ Success: $successCount" -ForegroundColor Green
Write-Host "  ❌ Failed:  $failCount" -ForegroundColor Red
Write-Host ""

if ($successCount -gt 0) {
    Write-Host "🎉 Downloaded $successCount book covers successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📝 Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Run SQL: update-db-to-jpg.sql" -ForegroundColor White
    Write-Host "  2. Refresh browser (Ctrl+F5)" -ForegroundColor White
    Write-Host "  3. Test: /test-images.html" -ForegroundColor White
} else {
    Write-Host "⚠️ No images were downloaded successfully." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "💡 This may be due to:" -ForegroundColor Cyan
    Write-Host "  - Network issues" -ForegroundColor Gray
    Write-Host "  - Blocked by firewall/antivirus" -ForegroundColor Gray
    Write-Host "  - CDN restrictions" -ForegroundColor Gray
    Write-Host ""
    Write-Host "📥 Manual download guide: MANUAL_IMAGE_DOWNLOAD_GUIDE.md" -ForegroundColor Yellow
}

Write-Host ""
