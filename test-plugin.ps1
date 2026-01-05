# Test script for Jellyfin Easier Life plugin
# This tests the API endpoint directly

param(
    [Parameter(Mandatory=$true)]
    [string]$JellyfinUrl = "http://localhost:8096",
    
    [Parameter(Mandatory=$true)]
    [string]$ApiKey,
    
    [Parameter(Mandatory=$true)]
    [string]$LibraryId
)

Write-Host "Testing Jellyfin Easier Life Plugin API..." -ForegroundColor Green
Write-Host "Jellyfin URL: $JellyfinUrl" -ForegroundColor Cyan
Write-Host "Library ID: $LibraryId" -ForegroundColor Cyan

$headers = @{
    "Authorization" = "MediaBrowser Token=`"$ApiKey`""
    "Content-Type" = "application/json"
}

$url = "$JellyfinUrl/Library/ReplaceMetadata?libraryId=$LibraryId&replaceImages=true"

Write-Host "`nCalling: $url" -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers
    
    Write-Host "`n✅ Success! Metadata replacement started." -ForegroundColor Green
    Write-Host "Check Jellyfin logs for progress." -ForegroundColor Cyan
}
catch {
    Write-Host "`n❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
}

Write-Host "`nTo get your API key:" -ForegroundColor Yellow
Write-Host "1. Go to Dashboard → API Keys" -ForegroundColor Gray
Write-Host "2. Create a new API key" -ForegroundColor Gray
Write-Host "3. Use it in this script" -ForegroundColor Gray
Write-Host "`nTo get your Library ID:" -ForegroundColor Yellow
Write-Host "1. Go to Dashboard → Libraries" -ForegroundColor Gray
Write-Host "2. Click on a library" -ForegroundColor Gray
Write-Host "3. Look at the URL or use the API to list libraries" -ForegroundColor Gray




