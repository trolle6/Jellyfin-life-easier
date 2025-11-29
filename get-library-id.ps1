# Script to get your library IDs from Jellyfin

param(
    [Parameter(Mandatory=$true)]
    [string]$JellyfinUrl = "http://localhost:8096",
    
    [Parameter(Mandatory=$true)]
    [string]$ApiKey
)

Write-Host "Fetching libraries from Jellyfin..." -ForegroundColor Green

$headers = @{
    "Authorization" = "MediaBrowser Token=`"$ApiKey`""
    "Content-Type" = "application/json"
}

$url = "$JellyfinUrl/Library/VirtualFolders"

try {
    $libraries = Invoke-RestMethod -Uri $url -Method Get -Headers $headers
    
    Write-Host "`nFound Libraries:" -ForegroundColor Green
    Write-Host "=================" -ForegroundColor Cyan
    
    foreach ($lib in $libraries) {
        Write-Host "`nName: $($lib.Name)" -ForegroundColor Yellow
        Write-Host "  ID: $($lib.ItemId)" -ForegroundColor Gray
        Write-Host "  Path: $($lib.Locations -join ', ')" -ForegroundColor Gray
    }
    
    Write-Host "`nUse the 'ItemId' as the LibraryId in the test script." -ForegroundColor Cyan
}
catch {
    Write-Host "`n❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
}


