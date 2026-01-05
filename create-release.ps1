# PowerShell script to create/update GitHub release
# Note: You need a GitHub Personal Access Token with 'repo' scope
# Create one at: https://github.com/settings/tokens

param(
    [string]$Token = "",
    [string]$Tag = "v1.0.0",
    [string]$ZipFile = "Jellyfin-easier-life.zip"
)

$ErrorActionPreference = "Stop"

$repo = "trolle6/Jellyfin-life-easier"
$apiUrl = "https://api.github.com/repos/$repo/releases"

Write-Host "Creating/Updating GitHub release..." -ForegroundColor Green

# Check if token is provided
if ([string]::IsNullOrEmpty($Token)) {
    Write-Host "⚠️  GitHub Personal Access Token required!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To create a token:" -ForegroundColor Cyan
    Write-Host "1. Go to: https://github.com/settings/tokens" -ForegroundColor Gray
    Write-Host "2. Click 'Generate new token (classic)'" -ForegroundColor Gray
    Write-Host "3. Select 'repo' scope" -ForegroundColor Gray
    Write-Host "4. Copy the token" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Then run:" -ForegroundColor Cyan
    Write-Host "  .\create-release.ps1 -Token YOUR_TOKEN_HERE" -ForegroundColor White
    Write-Host ""
    Write-Host "Or set it as an environment variable:" -ForegroundColor Cyan
    Write-Host "  `$env:GITHUB_TOKEN='YOUR_TOKEN'; .\create-release.ps1" -ForegroundColor White
    exit 1
}

# Check if ZIP file exists
if (-not (Test-Path $ZipFile)) {
    Write-Host "⚠️  ZIP file not found: $ZipFile" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "You need to build the plugin first!" -ForegroundColor Cyan
    Write-Host "On your Linux server, run:" -ForegroundColor Gray
    Write-Host "  cd Jellyfin-easier-life" -ForegroundColor White
    Write-Host "  dotnet build -c Release" -ForegroundColor White
    Write-Host "  # Then package it (see package-plugin-linux.sh)" -ForegroundColor White
    exit 1
}

# Get release info
$headers = @{
    "Authorization" = "token $Token"
    "Accept" = "application/vnd.github.v3+json"
}

# Check if release exists
Write-Host "Checking for existing release..." -ForegroundColor Yellow
try {
    $existingRelease = Invoke-RestMethod -Uri "$apiUrl/tags/$Tag" -Headers $headers -ErrorAction SilentlyContinue
    Write-Host "Found existing release: $($existingRelease.name)" -ForegroundColor Green
    
    # Upload asset if it doesn't exist
    if ($existingRelease.assets.Count -eq 0) {
        Write-Host "Uploading ZIP file..." -ForegroundColor Yellow
        
        $uploadUrl = $existingRelease.upload_url -replace '\{.*$', "?name=$ZipFile"
        $fileBytes = [System.IO.File]::ReadAllBytes((Resolve-Path $ZipFile))
        $fileEnc = [System.Text.Encoding]::GetEncoding('iso-8859-1').GetString($fileBytes)
        
        $uploadHeaders = @{
            "Authorization" = "token $Token"
            "Content-Type" = "application/zip"
        }
        
        Invoke-RestMethod -Uri $uploadUrl -Method Post -Headers $uploadHeaders -Body $fileBytes | Out-Null
        Write-Host "✅ ZIP file uploaded successfully!" -ForegroundColor Green
    } else {
        Write-Host "Release already has assets. Skipping upload." -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "✅ Release is ready!" -ForegroundColor Green
    Write-Host "   URL: $($existingRelease.html_url)" -ForegroundColor Cyan
    
} catch {
    # Release doesn't exist, create it
    Write-Host "Creating new release..." -ForegroundColor Yellow
    
    $releaseBody = @{
        tag_name = $Tag
        name = "v1.0.0 - Initial Release"
        body = @"
Initial release of Jellyfin Easier Life plugin.

## Features
- Converts soft library scans to full metadata replacement
- Universal settings support
- Season combination functionality

## Installation
Add this repository URL to Jellyfin:
\`\`\`
https://raw.githubusercontent.com/trolle6/Jellyfin-life-easier/main/repository.json
\`\`\`
"@
        draft = $false
        prerelease = $false
    } | ConvertTo-Json
    
    $release = Invoke-RestMethod -Uri $apiUrl -Method Post -Headers $headers -Body $releaseBody
    
    Write-Host "✅ Release created!" -ForegroundColor Green
    
    # Upload ZIP file
    Write-Host "Uploading ZIP file..." -ForegroundColor Yellow
    $uploadUrl = $release.upload_url -replace '\{.*$', "?name=$ZipFile"
    $fileBytes = [System.IO.File]::ReadAllBytes((Resolve-Path $ZipFile))
    
    $uploadHeaders = @{
        "Authorization" = "token $Token"
        "Content-Type" = "application/zip"
    }
    
    Invoke-RestMethod -Uri $uploadUrl -Method Post -Headers $uploadHeaders -Body $fileBytes | Out-Null
    Write-Host "✅ ZIP file uploaded!" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "✅ Release created and published!" -ForegroundColor Green
    Write-Host "   URL: $($release.html_url)" -ForegroundColor Cyan
}

# Calculate checksum
Write-Host ""
Write-Host "Calculating SHA1 checksum..." -ForegroundColor Yellow
$hash = Get-FileHash -Path $ZipFile -Algorithm SHA1
$checksum = "sha1:$($hash.Hash.ToLower())"
$timestamp = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

Write-Host ""
Write-Host "📋 Update repository.json with:" -ForegroundColor Cyan
Write-Host "   checksum: $checksum" -ForegroundColor White
Write-Host "   timestamp: $timestamp" -ForegroundColor White
Write-Host ""



