# How to Build and Release the Plugin

## The Problem

We can't build the plugin on Windows because it requires Jellyfin server DLLs that are only available where Jellyfin is installed (your Linux server).

## The Solution

Build the plugin on your Linux server where Jellyfin is installed, then create the GitHub release.

## Quick Steps

### 1. On Your Linux Server

```bash
# Clone the repository
git clone https://github.com/trolle6/Jellyfin-life-easier.git
cd Jellyfin-life-easier

# Make the script executable
chmod +x build-and-release.sh

# Run the build and release script
./build-and-release.sh
```

The script will:
- ✅ Find your Jellyfin installation automatically
- ✅ Update the project file with correct paths
- ✅ Build the plugin
- ✅ Package it into a ZIP file
- ✅ Create/update the GitHub release (if GitHub CLI is installed and authenticated)
- ✅ Show you the checksum to update repository.json

### 2. If GitHub CLI is Not Available

If the script can't create the release automatically:

1. **Upload the ZIP manually:**
   - Go to: https://github.com/trolle6/Jellyfin-life-easier/releases
   - Edit the v1.0.0 release
   - Upload `Jellyfin-easier-life.zip` from the `Jellyfin-easier-life` directory

2. **Update repository.json:**
   - Copy the SHA1 checksum from the script output
   - Update `repository.json` with the checksum and current timestamp
   - Commit and push:
     ```bash
     git add repository.json
     git commit -m "Update repository.json for v1.0.0"
     git push origin main
     ```

### 3. Install in Jellyfin

Once the release is created:
1. Go to **Dashboard** → **Plugins** → **Repositories**
2. Add: `https://raw.githubusercontent.com/trolle6/Jellyfin-life-easier/main/repository.json`
3. Go to **Plugins** → **Catalog**
4. Install "Jellyfin Easier Life"
5. Restart Jellyfin

## Manual Build (If Script Doesn't Work)

If you prefer to build manually:

```bash
cd Jellyfin-easier-life

# Find Jellyfin DLLs
find /usr -name "MediaBrowser.Common.dll" 2>/dev/null

# Edit Jellyfin-easier-life.csproj
# Uncomment the <Reference> items and update paths

# Build
dotnet build -c Release

# Package
mkdir -p release
cp bin/Release/net8.0/Jellyfin-easier-life.dll release/
cp manifest.json release/
cd release
zip -r ../Jellyfin-easier-life.zip .
cd ..

# Get checksum
sha1sum Jellyfin-easier-life.zip
```

## That's It!

Once the ZIP is uploaded to GitHub and repository.json is updated, the plugin will be installable via the repository URL in Jellyfin!



