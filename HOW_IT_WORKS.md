# How the Plugin Works

## Core Functionality

When you click **"Refresh Library"** in Jellyfin, this plugin automatically converts it into a **"Replace All Metadata"** operation with both boxes checked:

✅ **Replace all metadata** - Always enabled  
✅ **Replace images** - Enabled if configured in plugin settings

## How It Works

### 1. Event Interception

The plugin hooks into Jellyfin's library events:

- **ItemAdded**: When NEW items are added to the library
  - Automatically triggers full metadata replacement

- **ItemUpdated**: When items are updated during library refresh
  - If `ForceMetadataRefresh` is enabled (default: true), catches ALL metadata-related updates
  - This includes:
    - `MetadataDownload` - When metadata is downloaded
    - `MetadataEdit` - When metadata is edited
    - `ImageUpdate` - When images are updated
    - `None` - Sometimes library refresh uses this

### 2. Automatic Metadata Replacement

When an item update is detected, the plugin:

1. **Forces Full Refresh**: Sets `MetadataRefreshMode = FullRefresh` (not soft scan)
2. **Replaces All Metadata**: Sets `ReplaceAllMetadata = true` (always!)
3. **Replaces Images**: Sets `ReplaceImages` based on plugin config (default: true)

### 3. Configuration

Default settings (all enabled by default):
- ✅ **Replace All Metadata**: `true` - Core feature, always on
- ✅ **Force Metadata Refresh**: `true` - Ensures existing items get refreshed
- ✅ **Replace Images**: `true` - Replaces images during refresh

## What Gets Processed

The plugin processes **ALL** media types:
- Movies
- TV Shows
- Episodes
- Seasons
- Music
- Books
- Everything in your library!

## Important Notes

1. **Library Refresh Behavior**: 
   - When you click "Refresh Library", Jellyfin scans for changes
   - Items that get updated during the scan trigger the `ItemUpdated` event
   - The plugin intercepts these and forces full metadata replacement

2. **If It Doesn't Work**:
   - Make sure `ForceMetadataRefresh` is enabled in plugin settings
   - Check Jellyfin logs for "Item metadata updated" messages
   - Some library refresh operations might not trigger events for unchanged items
   - Use the API endpoint `/Library/ReplaceMetadata` for guaranteed full replacement

3. **Performance**:
   - Processing all items can take time
   - Check Jellyfin logs to see progress
   - The plugin processes items one at a time to avoid overwhelming the server

## Verification

To verify it's working:

1. Check Jellyfin logs for:
   ```
   "Item metadata updated: {ItemName} (Reason: {Reason}), triggering FULL metadata replacement"
   "Replacing metadata for item: {ItemName}"
   "Successfully replaced metadata for item: {ItemName}"
   ```

2. After a library refresh, check if metadata/images changed
3. New items should automatically get full metadata replacement



