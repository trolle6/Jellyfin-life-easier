# Creating a GitHub Release

## Step 1: Build the Plugin on Your Linux Server

Since your Jellyfin server is on Linux, you need to build the plugin there.

### On your Linux server:

1. **Clone or copy the repository:**
   ```bash
   git clone https://github.com/trolle6/Jellyfin-life-easier.git
   cd Jellyfin-life-easier/Jellyfin-easier-life
   ```

2. **Set up the project references:**
   - Edit `Jellyfin-easier-life.csproj`
   - Uncomment the `<Reference>` items and update paths to your Jellyfin installation:
     - Linux: Usually `/usr/lib/jellyfin/` or `/opt/jellyfin/`
     - Find your Jellyfin DLLs: `find /usr -name "MediaBrowser.Common.dll" 2>/dev/null`

3. **Build the plugin:**
   ```bash
   dotnet build -c Release
   ```

4. **Package the plugin:**
   ```bash
   chmod +x package-plugin.sh
   ./package-plugin.sh
   ```

   This creates `Jellyfin-easier-life.zip`

5. **Get the SHA1 checksum:**
   ```bash
   sha1sum Jellyfin-easier-life.zip
   ```
   Note the hash (format: `sha1:YOUR_HASH_HERE`)

## Step 2: Create GitHub Release

### Option A: Using GitHub Web Interface

1. Go to: https://github.com/trolle6/Jellyfin-life-easier/releases
2. Click **"Create a new release"**
3. Fill in:
   - **Tag:** `v1.0.0`
   - **Release title:** `v1.0.0 - Initial Release`
   - **Description:** 
     ```
     Initial release of Jellyfin Easier Life plugin.
     
     Features:
     - Converts soft library scans to full metadata replacement
     - Universal settings support
     - Season combination functionality
     ```
4. **Upload** `Jellyfin-easier-life.zip` as an asset
5. Click **"Publish release"**

### Option B: Using GitHub CLI (if installed)

```bash
gh release create v1.0.0 Jellyfin-easier-life.zip \
  --title "v1.0.0 - Initial Release" \
  --notes "Initial release of Jellyfin Easier Life plugin."
```

## Step 3: Update repository.json

After creating the release, update `repository.json` with:
- The actual release URL
- The SHA1 checksum from step 1
- Current timestamp

Then commit and push:
```bash
git add repository.json
git commit -m "Update repository.json with release v1.0.0"
git push origin main
```

## Step 4: Install in Jellyfin

1. Go to **Dashboard** → **Plugins** → **Repositories**
2. Add repository URL: `https://raw.githubusercontent.com/trolle6/Jellyfin-life-easier/main/repository.json`
3. Go to **Plugins** → **Catalog**
4. Find "Jellyfin Easier Life" and click **Install**
5. Restart Jellyfin

