# PowerShell script to build plugin and prepare for GitHub release
# Run this on Windows to create the ZIP file for release

Write-Host "🔨 Building Jellyfin Easier Life Plugin..." -ForegroundColor Cyan
Write-Host ""

# Check if we're in the right directory
if (-not (Test-Path "Jellyfin-easier-life\Jellyfin-easier-life.csproj")) {
    Write-Host "❌ Error: Jellyfin-easier-life.csproj not found!" -ForegroundColor Red
    Write-Host "   Make sure you're in the repository root directory." -ForegroundColor Yellow
    exit 1
}

# Check if .NET SDK is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK found: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Error: .NET SDK not found!" -ForegroundColor Red
    Write-Host "   Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "⚠️  IMPORTANT: You need Jellyfin server DLLs to build the plugin!" -ForegroundColor Yellow
Write-Host "   The DLLs should be referenced in Jellyfin-easier-life.csproj" -ForegroundColor Yellow
Write-Host "   If you haven't set this up, see SETUP.md for instructions." -ForegroundColor Yellow
Write-Host ""
$continue = Read-Host "Continue anyway? (y/n)"
if ($continue -ne "y" -and $continue -ne "Y") {
    Write-Host "Build cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "📦 Building plugin..." -ForegroundColor Cyan
dotnet build Jellyfin-easier-life\Jellyfin-easier-life.csproj -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "❌ Build failed! Check the errors above." -ForegroundColor Red
    Write-Host "   Common issues:" -ForegroundColor Yellow
    Write-Host "   - Jellyfin DLLs not referenced (see SETUP.md)" -ForegroundColor Gray
    Write-Host "   - .NET SDK version mismatch" -ForegroundColor Gray
    exit 1
}

Write-Host ""
Write-Host "✅ Build successful!" -ForegroundColor Green

# Find the output directory
$dllPath = "Jellyfin-easier-life\bin\Release\net8.0\Jellyfin-easier-life.dll"
$manifestPath = "Jellyfin-easier-life\manifest.json"

if (-not (Test-Path $dllPath)) {
    Write-Host "❌ Error: DLL not found at $dllPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $manifestPath)) {
    Write-Host "❌ Error: manifest.json not found at $manifestPath" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "📦 Creating ZIP file..." -ForegroundColor Cyan

# Create a temp directory for the ZIP contents
$tempDir = "release-temp"
if (Test-Path $tempDir) {
    Remove-Item -Recurse -Force $tempDir
}
New-Item -ItemType Directory -Path $tempDir | Out-Null

# Copy files
Copy-Item $dllPath "$tempDir\Jellyfin-easier-life.dll"
Copy-Item $manifestPath "$tempDir\manifest.json"

# Create ZIP
$zipPath = "Jellyfin-easier-life.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($tempDir, $zipPath)

# Clean up temp directory
Remove-Item -Recurse -Force $tempDir

# Calculate SHA1 checksum
Write-Host ""
Write-Host "🔐 Calculating SHA1 checksum..." -ForegroundColor Cyan
$sha1 = Get-FileHash -Path $zipPath -Algorithm SHA1
$checksum = $sha1.Hash.ToLower()

Write-Host ""
Write-Host "✅ Release package created!" -ForegroundColor Green
Write-Host ""
Write-Host "📁 Files created:" -ForegroundColor Cyan
Write-Host "   - $zipPath" -ForegroundColor White
Write-Host "   - Size: $([math]::Round((Get-Item $zipPath).Length / 1KB, 2)) KB" -ForegroundColor White
Write-Host ""
Write-Host "🔑 SHA1 Checksum:" -ForegroundColor Cyan
Write-Host "   $checksum" -ForegroundColor Yellow
Write-Host ""
Write-Host "📤 Next steps:" -ForegroundColor Cyan
Write-Host "   1. Go to: https://github.com/trolle6/Jellyfin-life-easier/releases/edit/1.0.0" -ForegroundColor White
Write-Host "   2. Drag and drop $zipPath to upload it" -ForegroundColor White
Write-Host "   3. Click 'Update release'" -ForegroundColor White
Write-Host "   4. Update repository.json with the checksum above" -ForegroundColor White
Write-Host ""
Write-Host "💡 Or use this checksum in repository.json:" -ForegroundColor Cyan
Write-Host '   "checksum": "' + $checksum + '"' -ForegroundColor Yellow

