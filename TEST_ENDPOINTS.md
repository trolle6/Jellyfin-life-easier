# Testing the Plugin Endpoints

## Quick Test URLs

After installing the plugin, try these URLs in your browser (while logged into Jellyfin):

### Status Endpoint
```
http://your-jellyfin-server:8096/EasierLife/Status
```

### Activity Endpoint
```
http://your-jellyfin-server:8096/EasierLife/Activity
```

### Library Controller (if it works, controllers are being discovered)
```
http://your-jellyfin-server:8096/Library/ReplaceMetadata?libraryId={GUID}&replaceImages=true
```
(Note: This is a POST endpoint, so it won't work in a browser - use a tool like Postman or curl)

## Troubleshooting 405 Errors

If you get a **405 Method Not Allowed** error:

1. **Check if the plugin is loaded:**
   - Dashboard → Plugins → Jellyfin Easier Life
   - Make sure it's enabled

2. **Check Jellyfin logs:**
   - Look for any errors about the plugin
   - Look for "PLUGIN ACTIVE" message on startup

3. **Try the Library endpoint first:**
   - If `/Library/ReplaceMetadata` works, then controllers are being discovered
   - If it doesn't, the plugin might not be loading correctly

4. **Check the route:**
   - The route is `/EasierLife/Status` (not `/Plugins/...`)
   - Make sure you're using GET (not POST)

5. **Authentication:**
   - You must be logged into Jellyfin
   - Or use your API key: `?api_key=YOUR_API_KEY`

## Using curl

```bash
# Get status (replace with your server and API key)
curl -H "X-Emby-Token: YOUR_API_KEY" \
  http://your-jellyfin-server:8096/EasierLife/Status

# Get activity
curl -H "X-Emby-Token: YOUR_API_KEY" \
  http://your-jellyfin-server:8096/EasierLife/Activity
```

## Using PowerShell

```powershell
# Get your API key from Dashboard → API Keys
$apiKey = "YOUR_API_KEY"
$server = "http://your-jellyfin-server:8096"

# Get status
Invoke-RestMethod -Uri "$server/EasierLife/Status" -Headers @{"X-Emby-Token"=$apiKey}

# Get activity
Invoke-RestMethod -Uri "$server/EasierLife/Activity" -Headers @{"X-Emby-Token"=$apiKey}
```

