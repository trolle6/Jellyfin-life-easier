# Plugin Repository Setup

To make your plugin installable via URL in Jellyfin, you need to host a repository manifest file.

## Option 1: GitHub Releases (Recommended)

1. **Create a GitHub repository** (if you haven't already)

2. **Create a release** with your plugin ZIP file:
   - Build your plugin: `dotnet build -c Release`
   - Create a ZIP file containing:
     - `Jellyfin-easier-life.dll`
     - `manifest.json`
   - Create a GitHub release (e.g., v1.0.0) and upload the ZIP

3. **Host the repository manifest** on GitHub:
   - Create a file called `repository.json` in your repo root
   - Use GitHub's raw URL: `https://raw.githubusercontent.com/yourusername/Jellyfin-easier-life/main/repository.json`
   - Update the `sourceUrl` and `checksum` in `repository.json` with your actual release URL

4. **Add to Jellyfin**:
   - Go to Dashboard → Plugins → Repositories
   - Click the **+** button
   - Enter a name (e.g., "Jellyfin Easier Life")
   - Enter the URL: `https://raw.githubusercontent.com/yourusername/Jellyfin-easier-life/main/repository.json`
   - Save

## Option 2: GitHub Pages

1. Enable GitHub Pages in your repository settings
2. Put `repository.json` in the `docs` folder or root
3. Use the GitHub Pages URL: `https://yourusername.github.io/Jellyfin-easier-life/repository.json`

## Option 3: Any Web Server

1. Upload `repository.json` to any web-accessible location
2. Upload your plugin ZIP to a web-accessible location
3. Update the URLs in `repository.json`
4. Use the manifest URL in Jellyfin

## Generating Checksums

For the `checksum` field in `repository.json`, you need the SHA1 hash of your ZIP file:

**Windows PowerShell:**
```powershell
Get-FileHash -Path "Jellyfin-easier-life.zip" -Algorithm SHA1
```

**Linux/Mac:**
```bash
sha1sum Jellyfin-easier-life.zip
```

Then format it as: `"sha1:YOUR_HASH_HERE"`

## Updating repository.json

After creating a release, update `repository.json` with:
- The actual release ZIP URL
- The SHA1 checksum of the ZIP file
- The correct timestamp
- Your actual GitHub username/repo

## Example repository.json URLs

Once hosted, your manifest URL will look like:
- `https://raw.githubusercontent.com/yourusername/Jellyfin-easier-life/main/repository.json`
- `https://yourusername.github.io/Jellyfin-easier-life/repository.json`
- `https://yourdomain.com/jellyfin-plugins/repository.json`

Users can then add this URL to their Jellyfin server to install your plugin!

