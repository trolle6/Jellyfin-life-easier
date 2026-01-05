# Download Plugin from GitHub Release

## The Goal

You want to download the DLL and manifest.json directly from GitHub releases, so you can just:
1. Download the ZIP
2. Extract it
3. Copy to your Jellyfin plugins folder

## Current Status

❌ **The release exists but has NO ZIP file yet**  
✅ **Once we upload the ZIP, you can download it directly!**

## How to Get the ZIP File

### Option 1: Build on Windows (If You Have Jellyfin DLLs)

1. **Configure Jellyfin DLL paths** in `Jellyfin-easier-life.csproj`:
   - Uncomment the `<Reference>` sections
   - Update paths to point to your Jellyfin installation

2. **Run the build script:**
   ```powershell
   .\prepare-release.ps1
   ```

3. **Upload to GitHub:**
   - Go to: https://github.com/trolle6/Jellyfin-life-easier/releases/edit/1.0.0
   - Drag and drop `Jellyfin-easier-life.zip`
   - Click "Update release"

4. **Download and use:**
   - Download: https://github.com/trolle6/Jellyfin-life-easier/releases/download/v1.0.0/Jellyfin-easier-life.zip
   - Extract the ZIP
   - Copy `Jellyfin-easier-life.dll` and `manifest.json` to your plugins folder

### Option 2: Build on Linux Server (Easier - Jellyfin DLLs Already There)

1. **SSH into your TrueNAS/Linux server**

2. **Clone the repo (or copy files):**
   ```bash
   git clone https://github.com/trolle6/Jellyfin-life-easier.git
   cd Jellyfin-life-easier
   ```

3. **Run the build script:**
   ```bash
   chmod +x build-and-release.sh
   ./build-and-release.sh
   ```

4. **The script will:**
   - Find Jellyfin DLLs automatically
   - Build the plugin
   - Create `Jellyfin-easier-life.zip`
   - Optionally upload to GitHub (if GitHub CLI is configured)

5. **If GitHub CLI is not configured:**
   - Download the ZIP from your server (via SCP/SMB)
   - Upload it manually to: https://github.com/trolle6/Jellyfin-life-easier/releases/edit/1.0.0

6. **Then download and use:**
   - Download: https://github.com/trolle6/Jellyfin-life-easier/releases/download/v1.0.0/Jellyfin-easier-life.zip
   - Extract and copy to plugins folder

## After ZIP is Uploaded

Once the ZIP is on GitHub, you can:

1. **Download it:**
   ```
   https://github.com/trolle6/Jellyfin-life-easier/releases/download/v1.0.0/Jellyfin-easier-life.zip
   ```

2. **Extract it:**
   - You'll get: `Jellyfin-easier-life.dll` and `manifest.json`

3. **Copy to plugins folder:**
   - Via SMB: Copy both files to `\\your-server\jellyfin\plugins\`
   - Or via SSH: `cp *.dll *.json /var/lib/jellyfin/plugins/`

4. **Restart Jellyfin**

5. **Enable in Dashboard → Plugins**

## Quick Summary

**Right now:** Release exists, but ZIP is missing  
**What you need:** Build the plugin and upload the ZIP  
**After that:** You can download ZIP directly from GitHub releases! 🎉

