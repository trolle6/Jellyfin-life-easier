# What Files Are Needed for the Plugin?

## ❌ You CAN'T Just Copy Source Files

Jellyfin plugins need to be **compiled** (built) first. You can't just copy `.cs` files and expect them to work.

## ✅ What Actually Works

A Jellyfin plugin needs **2 files** in a ZIP:

1. **`Jellyfin-easier-life.dll`** - The compiled plugin (built from all the `.cs` files)
2. **`manifest.json`** - The plugin description file

## How It Works

### Step 1: Build (Compile) the Plugin
- Takes all your `.cs` source files
- Compiles them into a single `.dll` file
- Requires Jellyfin server DLLs to compile (that's why we need to build on Linux)

### Step 2: Package
- Put the `.dll` and `manifest.json` in a ZIP file
- That's your installable plugin!

### Step 3: Install
- Either upload ZIP to GitHub release (for repository installation)
- Or extract ZIP and copy files to Jellyfin's plugins folder

## File Structure

```
Jellyfin-easier-life.zip
├── Jellyfin-easier-life.dll    ← Compiled plugin (built from all .cs files)
└── manifest.json               ← Plugin metadata
```

## Installation Locations

**For manual installation, copy the 2 files to:**
- Linux: `/var/lib/jellyfin/plugins/` or `/usr/share/jellyfin/plugins/`
- Windows: `%ProgramData%\Jellyfin\Server\plugins\`
- Docker: `/config/plugins/` (inside container)

**You need BOTH files in the plugins folder!**

## Why You Can't Just Copy Source Files

- Jellyfin doesn't run C# source code directly
- It needs compiled `.dll` files
- The build process combines all your code into one DLL
- Without building, you just have text files that Jellyfin can't use

## So What Do You Do?

1. **Build on Linux** (where Jellyfin DLLs are):
   ```bash
   cd Jellyfin-easier-life
   dotnet build -c Release
   ```
   This creates: `bin/Release/net8.0/Jellyfin-easier-life.dll`

2. **Package it:**
   ```bash
   mkdir release
   cp bin/Release/net8.0/Jellyfin-easier-life.dll release/
   cp manifest.json release/
   cd release
   zip -r ../Jellyfin-easier-life.zip .
   ```

3. **Use the ZIP:**
   - Upload to GitHub release, OR
   - Extract and copy the 2 files to Jellyfin plugins folder

That's it! Just 2 files in a ZIP.



