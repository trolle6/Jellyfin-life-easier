# How to Verify the Plugin is Working

## ✅ Quick Check - Status Endpoint

The easiest way to see if the plugin is working is to check the status endpoint:

1. **Open your browser** and go to:
   ```
   http://your-jellyfin-server:8096/EasierLife/Status
   ```
   
   You'll need to be logged in or use your API key.

2. **What you should see:**
   ```json
   {
     "pluginName": "Jellyfin Easier Life",
     "pluginVersion": "1.0.0",
     "isEnabled": true,
     "configuration": {
       "replaceAllMetadata": true,
       "replaceImages": true,
       "forceMetadataRefresh": true,
       "combineAllSeasons": false
     },
     "statistics": {
       "totalItemsProcessed": 42,
       "totalItemsSucceeded": 42,
       "totalItemsFailed": 0,
       "lastActivityTime": "2024-11-29T12:30:00Z",
       "pluginStartTime": "2024-11-29T10:00:00Z",
       "recentActivityCount": 20
     },
     "message": "✅ Plugin is ACTIVE - All library refreshes will replace ALL metadata automatically!"
   }
   ```

## 📊 Check Activity Log

See recent items that were processed:

```
http://your-jellyfin-server:8096/EasierLife/Activity
```

## 📝 Check Jellyfin Logs

The plugin now logs with **clear, visible markers** so you can easily spot it:

1. **Go to Dashboard → Logs** in Jellyfin
2. **Look for these messages:**

   - **On startup:**
     ```
     ✅ [Jellyfin Easier Life] PLUGIN ACTIVE - Metadata replacement is ENABLED
        → All library refreshes will automatically replace ALL metadata
        → Replace Images: True
     ```

   - **When processing items:**
     ```
     🔄 [Jellyfin Easier Life] REPLACING ALL METADATA for: Movie Name (Movie, ID: ...)
     ✅ [Jellyfin Easier Life] SUCCESSFULLY REPLACED metadata for: Movie Name
     ```

   - **When new items are added:**
     ```
     🆕 [Jellyfin Easier Life] NEW ITEM DETECTED: Movie Name - Triggering full metadata replacement
     ```

   - **When items are updated (library refresh):**
     ```
     🔄 [Jellyfin Easier Life] ITEM UPDATED: Movie Name (Reason: None) - Intercepting and replacing ALL metadata
     ```

## 🧪 Test It

1. **Enable the plugin:**
   - Dashboard → Plugins → Jellyfin Easier Life → Configure
   - Make sure **"Replace All Metadata"** is ✅ enabled
   - Make sure **"Force Metadata Refresh"** is ✅ enabled
   - Make sure **"Replace Images"** is ✅ enabled (optional)

2. **Trigger a library refresh:**
   - Go to a library (Movies, TV Shows, etc.)
   - Click the **three dots (⋮)** → **"Refresh Metadata"**
   - Or use **"Scan Library"** from the library settings

3. **Watch the logs:**
   - Go to Dashboard → Logs
   - You should see messages like:
     ```
     🔄 [Jellyfin Easier Life] ITEM UPDATED: ...
     🔄 [Jellyfin Easier Life] REPLACING ALL METADATA for: ...
     ✅ [Jellyfin Easier Life] SUCCESSFULLY REPLACED metadata for: ...
     ```

4. **Check the status endpoint again:**
   - The `totalItemsProcessed` count should increase
   - The `lastActivityTime` should update

## ❓ Troubleshooting

### Plugin not showing activity?

1. **Check if plugin is enabled:**
   - Dashboard → Plugins → Jellyfin Easier Life
   - Make sure it's enabled (not just installed)

2. **Check configuration:**
   - Dashboard → Plugins → Jellyfin Easier Life → Configure
   - **"Replace All Metadata"** must be ✅ enabled
   - **"Force Metadata Refresh"** must be ✅ enabled (for existing items)

3. **Check logs for errors:**
   - Look for any ❌ error messages
   - Check if the plugin loaded: look for "PLUGIN ACTIVE" message

4. **Try the API endpoint manually:**
   ```
   POST http://your-jellyfin-server:8096/Library/ReplaceMetadata?libraryId={LIBRARY_ID}&replaceImages=true
   ```
   (You'll need authentication - use your API key or login token)

### Still not working?

- Make sure you're refreshing an **existing library** (not just scanning for new files)
- Try adding a **new item** to the library - that should definitely trigger it
- Check that the plugin version matches your Jellyfin version (targetAbi in manifest.json)

