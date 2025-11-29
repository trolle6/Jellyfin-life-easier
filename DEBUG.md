# Debugging the Plugin

## Step 1: Check if Plugin is Loaded

1. **Check Jellyfin Logs:**
   - Go to Dashboard → Logs
   - Look for: `"Library scan hook initialized. Metadata replacement is enabled/disabled"`
   - If you see this, the plugin is loaded!

2. **Check Plugin Status:**
   - Dashboard → Plugins
   - Find "Jellyfin Easier Life"
   - Make sure it's enabled and shows no errors

## Step 2: Check Plugin Configuration

1. **Dashboard → Plugins → Jellyfin Easier Life → Configure**
2. Make sure:
   - ✅ **Replace All Metadata** is enabled
   - ✅ **Force Metadata Refresh** is enabled (if you want it to work on existing items)
   - ✅ **Replace Images** is enabled (optional)

## Step 3: Understanding How It Works

The current hook works on:
- **ItemAdded**: When NEW items are added to the library
- **ItemUpdated**: When items are updated (but only if `UpdateReason == MetadataDownload`)

**The problem:** A "Refresh Library" on existing items might not trigger these events!

## Step 4: Test the API Endpoint Instead

The plugin has an API endpoint you can use to manually trigger metadata replacement:

1. Get your library ID:
   - Go to a library in Jellyfin
   - Look at the URL or use the API

2. Call the API:
   ```
   POST http://your-jellyfin-server:8096/Library/ReplaceMetadata?libraryId={YOUR_LIBRARY_ID}&replaceImages=true
   ```
   
   You'll need to authenticate with your API key or user token.

## Step 5: Check Logs for Errors

Look in Jellyfin logs for:
- Any errors from "Jellyfin Easier Life" or "LibraryScanHook"
- Any errors from "MetadataReplacementService"

## Step 6: Test with a New Item

The hook should definitely work when you add a NEW item to the library. Try:
1. Add a new movie/show to your library
2. Watch the logs - you should see "Item added: {ItemName}, triggering metadata replacement"


