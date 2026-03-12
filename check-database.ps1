# PowerShell script to check BookStore database
# Run: .\check-database.ps1

Write-Host "`n🔍 Checking BookStore Database..." -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Gray

# Connection string (adjust if needed)
$server = ".\SQLEXPRESS"
$database = "BookStoreDb"
$connectionString = "Server=$server;Database=$database;Trusted_Connection=True;MultipleActiveResultSets=true"

try {
    # Load SQL Client
    Add-Type -AssemblyName "System.Data"
    
    # Create connection
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "✅ Connected to: $server\$database" -ForegroundColor Green
    Write-Host ""
    
    # Function to execute query
    function Execute-Query {
        param($query)
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = $query
        return $cmd.ExecuteScalar()
    }
    
    # Check counts
    Write-Host "📊 Database Statistics:" -ForegroundColor Yellow
    Write-Host "-" * 60 -ForegroundColor Gray
    
    $userCount = Execute-Query "SELECT COUNT(*) FROM Users"
    Write-Host "  Users:      $userCount" -ForegroundColor $(if($userCount -gt 0){"Green"}else{"Red"})
    
    $categoryCount = Execute-Query "SELECT COUNT(*) FROM Categories"
    Write-Host "  Categories: $categoryCount" -ForegroundColor $(if($categoryCount -gt 0){"Green"}else{"Red"})
    
    $productCount = Execute-Query "SELECT COUNT(*) FROM Products"
    Write-Host "  Products:   $productCount" -ForegroundColor $(if($productCount -gt 0){"Green"}else{"Red"})
    
    $orderCount = Execute-Query "SELECT COUNT(*) FROM Orders"
    Write-Host "  Orders:     $orderCount" -ForegroundColor Cyan
    
    Write-Host ""
    
    # Check if data exists
    if ($productCount -eq 0) {
        Write-Host "⚠️  WARNING: No products found!" -ForegroundColor Red
        Write-Host "   Database needs to be seeded." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "   Run these commands:" -ForegroundColor Yellow
        Write-Host "   1. dotnet ef database drop --force" -ForegroundColor White
        Write-Host "   2. dotnet ef database update" -ForegroundColor White
        Write-Host "   3. dotnet run" -ForegroundColor White
    }
    else {
        Write-Host "📚 Sample Products:" -ForegroundColor Yellow
        Write-Host "-" * 60 -ForegroundColor Gray
        
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = @"
SELECT TOP 5 
    ProductId,
    ProductName, 
    Author, 
    Price, 
    Stock,
    CASE 
        WHEN IsBestSeller = 1 THEN '🔥'
        WHEN IsNewArrival = 1 THEN '🆕'
        WHEN IsFeatured = 1 THEN '⭐'
        ELSE ''
    END as Badge
FROM Products
ORDER BY ProductId
"@
        
        $reader = $cmd.ExecuteReader()
        while ($reader.Read()) {
            $id = $reader['ProductId']
            $name = $reader['ProductName']
            $author = $reader['Author']
            $price = [decimal]$reader['Price']
            $stock = $reader['Stock']
            $badge = $reader['Badge']
            
            $priceFormatted = "{0:N0}" -f $price
            Write-Host "  [$id] $badge $name" -ForegroundColor White
            Write-Host "      by $author | $priceFormatted VND | Stock: $stock" -ForegroundColor Gray
        }
        $reader.Close()
        
        Write-Host ""
        Write-Host "✅ Database is properly seeded!" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "📝 Admin Credentials:" -ForegroundColor Yellow
    Write-Host "-" * 60 -ForegroundColor Gray
    
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = "SELECT Email, Password, Role FROM Users WHERE Role = 'Admin'"
    $reader = $cmd.ExecuteReader()
    
    if ($reader.Read()) {
        Write-Host "  Email:    $($reader['Email'])" -ForegroundColor Cyan
        Write-Host "  Password: $($reader['Password'])" -ForegroundColor Cyan
        Write-Host "  Role:     $($reader['Role'])" -ForegroundColor Green
    }
    else {
        Write-Host "  ⚠️  No admin user found!" -ForegroundColor Red
    }
    $reader.Close()
    
}
catch {
    Write-Host ""
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    
    if ($_.Exception.Message -like "*Login failed*" -or $_.Exception.Message -like "*network*") {
        Write-Host "💡 Troubleshooting:" -ForegroundColor Yellow
        Write-Host "   1. Check SQL Server is running" -ForegroundColor White
        Write-Host "   2. Verify connection string:" -ForegroundColor White
        Write-Host "      Server: $server" -ForegroundColor Gray
        Write-Host "      Database: $database" -ForegroundColor Gray
        Write-Host "   3. Try different server names:" -ForegroundColor White
        Write-Host "      - (localdb)\MSSQLLocalDB" -ForegroundColor Gray
        Write-Host "      - .\SQLEXPRESS" -ForegroundColor Gray
        Write-Host "      - localhost" -ForegroundColor Gray
    }
    elseif ($_.Exception.Message -like "*Cannot open database*") {
        Write-Host "💡 Database doesn't exist yet." -ForegroundColor Yellow
        Write-Host "   Run: dotnet ef database update" -ForegroundColor White
    }
}
finally {
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Gray
Write-Host ""
