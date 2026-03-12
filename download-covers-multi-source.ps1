# Alternative download script using different CDN sources
# Mix of Tiki, Fahasa, and other public sources

Write-Host "📥 Downloading real book covers from multiple sources..." -ForegroundColor Cyan
Write-Host ""

$outputDir = "wwwroot\images\products"

if (!(Test-Path $outputDir)) {
    New-Item -Path $outputDir -ItemType Directory -Force | Out-Null
}

# Alternative URLs from various sources
$books = @(
    @{
        Name = "nha-gia-kim.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/71zHDXu1TaL.jpg",
            "https://m.media-amazon.com/images/I/71zHDXu1TaL._AC_UF1000,1000_QL80_.jpg"
        )
    },
    @{
        Name = "dac-nhan-tam.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/71-EtDXoBZL.jpg",
            "https://m.media-amazon.com/images/I/71-EtDXoBZL._AC_UF1000,1000_QL80_.jpg"
        )
    },
    @{
        Name = "mat-biec.jpg"
        Urls = @(
            "https://cdn0.fahasa.com/media/catalog/product/m/a/mat_biec_1.jpg",
            "https://images-na.ssl-images-amazon.com/images/I/71aFt4%2BjRML.jpg"
        )
    },
    @{
        Name = "sapiens.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/71SMzF%2BfkLL.jpg",
            "https://m.media-amazon.com/images/I/71SMzF+fkLL._AC_UF1000,1000_QL80_.jpg"
        )
    },
    @{
        Name = "atomic-habits.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/51Eqf-URhoL.jpg",
            "https://m.media-amazon.com/images/I/51Eqf-URhoL._SY466_.jpg"
        )
    },
    @{
        Name = "clean-code.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/51E2055ZGUL.jpg",
            "https://m.media-amazon.com/images/I/51E2055ZGUL._SY466_.jpg"
        )
    },
    @{
        Name = "7-thoi-quen.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/51fEzAYgE3L.jpg",
            "https://m.media-amazon.com/images/I/51fEzAYgE3L._SY466_.jpg"
        )
    },
    @{
        Name = "hoang-tu-be.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/51kH8%2BwwXOL.jpg",
            "https://m.media-amazon.com/images/I/51kH8+wwXOL._SY466_.jpg"
        )
    },
    @{
        Name = "design-patterns.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/51kuc0iWoKL.jpg",
            "https://m.media-amazon.com/images/I/51kuc0iWoKL._SY466_.jpg"
        )
    },
    @{
        Name = "tu-duy-nhanh-cham.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/41wI53OEpxL.jpg",
            "https://m.media-amazon.com/images/I/41wI53OEpxL._SY466_.jpg"
        )
    },
    @{
        Name = "hoa-vang-co-xanh.jpg"
        Urls = @(
            "https://cdn0.fahasa.com/media/catalog/product/t/o/toi-thay-hoa-vang-tren-co-xanh.jpg"
        )
    },
    @{
        Name = "de-men.jpg"
        Urls = @(
            "https://cdn0.fahasa.com/media/catalog/product/d/e/de-men-phieu-luu-ky.jpg"
        )
    },
    @{
        Name = "khoi-nghiep-tinh-gon.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/51Zymoq7UnL.jpg",
            "https://m.media-amazon.com/images/I/51Zymoq7UnL._SY466_.jpg"
        )
    },
    @{
        Name = "lam-giau-chung-khoan.jpg"
        Urls = @(
            "https://cdn0.fahasa.com/media/catalog/product/l/a/lam-giau-tu-chung-khoan.jpg"
        )
    },
    @{
        Name = "aspnet-core.jpg"
        Urls = @(
            "https://images-na.ssl-images-amazon.com/images/I/51WD-F3GobL.jpg",
            "https://m.media-amazon.com/images/I/51WD-F3GobL._SY466_.jpg"
        )
    }
)

$successCount = 0
$failCount = 0

foreach ($book in $books) {
    $filename = $book.Name
    $outputPath = Join-Path $outputDir $filename
    
    Write-Host "Trying: $filename" -NoNewline -ForegroundColor White
    
    $downloaded = $false
    foreach ($url in $book.Urls) {
        try {
            $webClient = New-Object System.Net.WebClient
            $webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36")
            $webClient.Headers.Add("Accept", "image/webp,image/apng,image/*,*/*;q=0.8")
            $webClient.Headers.Add("Referer", "https://www.google.com/")
            $webClient.DownloadFile($url, $outputPath)
            
            if (Test-Path $outputPath) {
                $fileSize = (Get-Item $outputPath).Length
                if ($fileSize -gt 1000) {
                    Write-Host " ✅ OK ($([math]::Round($fileSize/1KB, 1)) KB)" -ForegroundColor Green
                    $downloaded = $true
                    $successCount++
                    break
                } else {
                    Remove-Item $outputPath -Force -ErrorAction SilentlyContinue
                }
            }
        }
        catch {
            # Try next URL
        }
        Start-Sleep -Milliseconds 300
    }
    
    if (-not $downloaded) {
        Write-Host " ❌ All sources failed" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Gray
Write-Host "📊 Summary:" -ForegroundColor Cyan
Write-Host "  ✅ Success: $successCount / $($books.Count)" -ForegroundColor Green
Write-Host "  ❌ Failed:  $failCount / $($books.Count)" -ForegroundColor $(if($failCount -gt 0){"Red"}else{"Green"})
Write-Host ""

if ($successCount -gt 0) {
    Write-Host "🎉 Downloaded $successCount book covers!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📝 Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Run SQL below to update database:" -ForegroundColor White
    Write-Host ""
    Write-Host "     UPDATE Products SET ImageUrl = REPLACE(ImageUrl, '.svg', '.jpg');" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  2. Refresh browser (Ctrl+F5)" -ForegroundColor White
    Write-Host "  3. Check: /test-images.html or /Product" -ForegroundColor White
    Write-Host ""
}

if ($failCount -gt 0) {
    Write-Host "⚠️ $failCount images could not be downloaded." -ForegroundColor Yellow
    Write-Host "   They will use SVG placeholders instead." -ForegroundColor Gray
    Write-Host ""
    Write-Host "💡 You can download them manually from:" -ForegroundColor Cyan
    Write-Host "   - Google Images: '[Book name] book cover'" -ForegroundColor White
    Write-Host "   - Amazon.com" -ForegroundColor White
    Write-Host "   - Goodreads.com" -ForegroundColor White
}

Write-Host ""
