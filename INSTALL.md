# Installation Instructions

## For End Users (Installing from GitHub Repository)

### Step 1: Add the Repository to Jellyfin

1. Open Jellyfin web interface
2. Go to **Dashboard** → **Plugins** → **Repositories**
3. Click the **+** (plus) button to add a new repository
4. Enter:
   - **Name**: `Jellyfin Easier Life` (or any name you prefer)
   - **URL**: 
     ```
     https://raw.githubusercontent.com/trolle6/Jellyfin-life-easier/main/repository.json
     ```
5. Click **OK** to save

### Step 2: Install the Plugin

1. Go to **Dashboard** → **Plugins** → **Catalog**
2. Find **"Jellyfin Easier Life"** in the list
3. Click **Install**
4. Wait for installation to complete
5. **Restart Jellyfin** (important!)

### Step 3: Configure the Plugin

1. Go to **Dashboard** → **Plugins** → **Jellyfin Easier Life**
2. Click **Configure** (gear icon)
3. Enable the options you want:
   - ✅ **Replace All Metadata** - Converts soft scans to full metadata replacement
   - ✅ **Force Metadata Refresh** - Forces refresh even if metadata exists
   - ✅ **Replace Images** - Replace images during metadata refresh
   - ⚠️ **Combine All Seasons** - Combines all seasons into one (use with caution!)

4. Click **Save**

### Step 4: Verify Installation

1. Check **Dashboard** → **Logs**
2. Look for: `"Library scan hook initialized. Metadata replacement is enabled"`
3. If you see this, the plugin is working!

## Repository Manifest URL

The official repository manifest URL is:
```
https://raw.githubusercontent.com/trolle6/Jellyfin-life-easier/main/repository.json
```

## Manual Installation (Alternative)

If you prefer to install manually:

1. Download the plugin ZIP from [Releases](https://github.com/trolle6/Jellyfin-life-easier/releases)
2. Extract the ZIP file
3. Copy both files to your Jellyfin plugins directory:
   - `Jellyfin-easier-life.dll`
   - `manifest.json`
   
   **Plugins directory locations:**
   - Windows: `%ProgramData%\Jellyfin\Server\plugins\`
   - Linux: `/var/lib/jellyfin/plugins/` or `/usr/share/jellyfin/plugins/`
   - macOS: `~/.local/share/jellyfin/plugins/`

4. Restart Jellyfin

## Troubleshooting

- **Plugin not showing in Catalog**: Make sure you've added the repository URL correctly and restarted Jellyfin
- **Plugin not loading**: Check Jellyfin logs for errors
- **Features not working**: Verify the plugin is enabled and configured in Dashboard → Plugins


