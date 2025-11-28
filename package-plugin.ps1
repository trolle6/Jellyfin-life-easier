# PowerShell script to package the Jellyfin plugin for distribution

$ErrorActionPreference = "Stop"

Write-Host "Packaging Jellyfin Easier Life plugin..." -ForegroundColor Green

# Build the plugin
Write-Host "Building plugin..." -ForegroundColor Yellow
dotnet build -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Create output directory
$outputDir = "release"
$zipName = "Jellyfin-easier-life.zip"

if (Test-Path $outputDir) {
    Remove-Item -Recurse -Force $outputDir
}
New-Item -ItemType Directory -Path $outputDir | Out-Null

# Copy required files
$dllPath = "bin\Release\net8.0\Jellyfin-easier-life.dll"
$manifestPath = "manifest.json"

if (-not (Test-Path $dllPath)) {
    Write-Host "DLL not found at $dllPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $manifestPath)) {
    Write-Host "manifest.json not found at $manifestPath" -ForegroundColor Red
    exit 1
}

Copy-Item $dllPath -Destination $outputDir
Copy-Item $manifestPath -Destination $outputDir

# Create ZIP file
Write-Host "Creating ZIP file..." -ForegroundColor Yellow
if (Test-Path $zipName) {
    Remove-Item $zipName
}

Compress-Archive -Path "$outputDir\*" -DestinationPath $zipName -Force

# Calculate SHA1 checksum
Write-Host "Calculating SHA1 checksum..." -ForegroundColor Yellow
$hash = Get-FileHash -Path $zipName -Algorithm SHA1
$checksum = "sha1:$($hash.Hash.ToLower())"

Write-Host ""
Write-Host "Plugin packaged successfully!" -ForegroundColor Green
Write-Host "ZIP file: $zipName" -ForegroundColor Cyan
Write-Host "SHA1 checksum: $checksum" -ForegroundColor Cyan
Write-Host ""
Write-Host "Update repository.json with:" -ForegroundColor Yellow
Write-Host "  - sourceUrl: Your GitHub release URL" -ForegroundColor Gray
Write-Host "  - checksum: $checksum" -ForegroundColor Gray
Write-Host "  - timestamp: Current timestamp" -ForegroundColor Gray

