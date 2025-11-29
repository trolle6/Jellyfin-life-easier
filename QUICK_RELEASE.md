# Quick Release Guide

## TL;DR - Create Release in 3 Steps

### 1. Build on Linux Server

SSH into your Linux server where Jellyfin is installed, then:

```bash
# Clone the repo
git clone https://github.com/trolle6/Jellyfin-life-easier.git
cd Jellyfin-life-easier/Jellyfin-easier-life

# Find Jellyfin DLLs location
find /usr -name "MediaBrowser.Common.dll" 2>/dev/null
# or
find /opt -name "MediaBrowser.Common.dll" 2>/dev/null

# Edit Jellyfin-easier-life.csproj - uncomment Reference items and update paths
nano Jellyfin-easier-life.csproj

# Build
chmod +x ../package-plugin-linux.sh
../package-plugin-linux.sh
```

This creates `Jellyfin-easier-life.zip`

### 2. Create GitHub Release

1. Go to: https://github.com/trolle6/Jellyfin-life-easier/releases/new
2. **Tag:** `v1.0.0`
3. **Title:** `v1.0.0 - Initial Release`
4. **Upload:** `Jellyfin-easier-life.zip`
5. Click **"Publish release"**

### 3. Update repository.json

Copy the SHA1 checksum from the build output, then:

```bash
# On your Windows machine (or wherever you have the repo)
# Edit repository.json and update:
# - sourceUrl: https://github.com/trolle6/Jellyfin-life-easier/releases/download/v1.0.0/Jellyfin-easier-life.zip
# - checksum: sha1:YOUR_HASH_HERE (from build output)
# - timestamp: 2025-11-29T13:00:00Z (current UTC time)

git add repository.json
git commit -m "Update repository.json for v1.0.0 release"
git push origin main
```

Done! The plugin will now be installable via the repository URL in Jellyfin.

