# PowerShell Script to FORCE reset and seed database
# This will DROP and RECREATE everything

Write-Host "`n" -NoNewline
Write-Host "=" * 70 -ForegroundColor Cyan
Write-Host "  🔄 FORCE DATABASE RESET & SEED" -ForegroundColor Yellow
Write-Host "=" * 70 -ForegroundColor Cyan
Write-Host ""

Write-Host "⚠️  WARNING: This will DELETE ALL DATA in the database!" -ForegroundColor Red
Write-Host ""
$confirm = Read-Host "Type 'YES' to continue"

if ($confirm -ne "YES") {
    Write-Host "`n❌ Aborted." -ForegroundColor Yellow
    exit
}

Write-Host ""
Write-Host "Step 1: Dropping existing database..." -ForegroundColor Cyan
dotnet ef database drop --force

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to drop database" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Database dropped successfully`n" -ForegroundColor Green

Write-Host "Step 2: Running migrations..." -ForegroundColor Cyan
dotnet ef database update

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to run migrations" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Migrations applied successfully`n" -ForegroundColor Green

Write-Host "Step 3: Building project..." -ForegroundColor Cyan
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful`n" -ForegroundColor Green

Write-Host "=" * 70 -ForegroundColor Cyan
Write-Host "  ✅ DATABASE RESET COMPLETE!" -ForegroundColor Green
Write-Host "=" * 70 -ForegroundColor Cyan
Write-Host ""
Write-Host "📝 Next steps:" -ForegroundColor Yellow
Write-Host "   1. Press F5 in Visual Studio to run the app" -ForegroundColor White
Write-Host "   2. DbInitializer will auto-seed on startup" -ForegroundColor White
Write-Host "   3. Check console output for seed messages" -ForegroundColor White
Write-Host ""
Write-Host "Or run manually:" -ForegroundColor Yellow
Write-Host "   dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "Then verify:" -ForegroundColor Yellow
Write-Host "   .\check-database.ps1" -ForegroundColor White
Write-Host ""
