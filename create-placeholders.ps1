# PowerShell Script to create placeholder book cover images
# Run this in your project root directory

$books = @(
    @{Name="nha-gia-kim"; Color="#E63946"; Title="Nhà Giả Kim"},
    @{Name="cay-cam-ngot"; Color="#F77F00"; Title="Cây Cam Ngọt"},
    @{Name="mat-biec"; Color="#06AED5"; Title="Mắt Biếc"},
    @{Name="hoa-vang-co-xanh"; Color="#FFBE0B"; Title="Hoa Vàng Cỏ Xanh"},
    @{Name="dac-nhan-tam"; Color="#8338EC"; Title="Đắc Nhân Tâm"},
    @{Name="sapiens"; Color="#3A86FF"; Title="Sapiens"},
    @{Name="tu-duy-nhanh-cham"; Color="#06FFA5"; Title="Tư Duy Nhanh Chậm"},
    @{Name="atomic-habits"; Color="#FB5607"; Title="Atomic Habits"},
    @{Name="khoi-nghiep-tinh-gon"; Color="#FF006E"; Title="Khởi Nghiệp Tinh Gọn"},
    @{Name="7-thoi-quen"; Color="#8338EC"; Title="7 Thói Quen"},
    @{Name="lam-giau-chung-khoan"; Color="#219EBC"; Title="Làm Giàu CK"},
    @{Name="de-men"; Color="#FFB703"; Title="Dế Mèn"},
    @{Name="hoang-tu-be"; Color="#023E8A"; Title="Hoàng Tử Bé"},
    @{Name="clean-code"; Color="#2B2D42"; Title="Clean Code"},
    @{Name="design-patterns"; Color="#8D99AE"; Title="Design Patterns"},
    @{Name="aspnet-core"; Color="#512DA8"; Title="ASP.NET Core"}
)

# Create directory if not exists
$dir = "wwwroot\images\products"
if (!(Test-Path $dir)) {
    New-Item -Path $dir -ItemType Directory -Force | Out-Null
}

foreach ($book in $books) {
    $svg = @"
<svg xmlns="http://www.w3.org/2000/svg" width="300" height="400" viewBox="0 0 300 400">
  <defs>
    <linearGradient id="grad_$($book.Name)" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" style="stop-color:$($book.Color);stop-opacity:1" />
      <stop offset="100%" style="stop-color:#ffffff;stop-opacity:0.7" />
    </linearGradient>
  </defs>
  
  <!-- Background -->
  <rect width="300" height="400" fill="url(#grad_$($book.Name))"/>
  
  <!-- Book spine shadow -->
  <rect x="0" y="0" width="30" height="400" fill="#000000" opacity="0.1"/>
  
  <!-- Decorative lines -->
  <line x1="40" y1="50" x2="260" y2="50" stroke="#ffffff" stroke-width="2" opacity="0.5"/>
  <line x1="40" y1="350" x2="260" y2="350" stroke="#ffffff" stroke-width="2" opacity="0.5"/>
  
  <!-- Title -->
  <text x="150" y="200" font-family="Arial, sans-serif" font-size="24" font-weight="bold" fill="#ffffff" text-anchor="middle">
    $($book.Title)
  </text>
  
  <!-- Book icon -->
  <g transform="translate(150, 280)">
    <rect x="-25" y="-35" width="50" height="70" fill="none" stroke="#ffffff" stroke-width="2" rx="3" opacity="0.7"/>
    <line x1="-25" y1="-35" x2="-25" y2="35" stroke="#ffffff" stroke-width="1.5" opacity="0.7"/>
  </g>
</svg>
"@
    
    $filePath = Join-Path $dir "$($book.Name).jpg"
    $svg | Out-File -FilePath $filePath -Encoding UTF8
    Write-Host "✅ Created: $($book.Name).jpg" -ForegroundColor Green
}

Write-Host "`n🎉 All placeholder images created successfully!" -ForegroundColor Cyan
Write-Host "Location: $dir" -ForegroundColor Yellow
