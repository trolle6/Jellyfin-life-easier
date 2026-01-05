# Simple Explanation: What Files Do You Need?

## ❌ What WON'T Work

**You CAN'T just copy all the `.cs` source files to your NAS and expect it to work!**

Jellyfin doesn't understand C# source code - it needs **compiled** code (a `.dll` file).

## ✅ What Actually Works

**Only 2 files are needed for the plugin:**

1. **`Jellyfin-easier-life.dll`** ← This is the compiled plugin (built from all your `.cs` files)
2. **`manifest.json`** ← This already exists in your project

## The Process

```
Source Files (.cs) 
    ↓
[BUILD/COMPILE on Linux]
    ↓
Jellyfin-easier-life.dll
    ↓
[Package with manifest.json]
    ↓
Jellyfin-easier-life.zip
    ↓
[Install in Jellyfin]
```

## Step-by-Step

### 1. Build on Linux (where Jellyfin is installed)
```bash
cd Jellyfin-easier-life
dotnet build -c Release
```
This creates: `bin/Release/net8.0/Jellyfin-easier-life.dll`

### 2. Create ZIP with just 2 files
```bash
mkdir release
cp bin/Release/net8.0/Jellyfin-easier-life.dll release/
cp manifest.json release/
cd release
zip -r ../Jellyfin-easier-life.zip .
```

### 3. Use the ZIP
- **Option A:** Upload to GitHub release (for repository installation)
- **Option B:** Extract ZIP and copy the 2 files to Jellyfin's plugins folder:
  - Linux: `/var/lib/jellyfin/plugins/`
  - Or wherever your Jellyfin plugins are stored

## What About All Those Other Files?

- **`.cs` files** = Source code (needed to BUILD, but not to RUN)
- **`.csproj`** = Project file (needed to BUILD, but not to RUN)
- **`obj/` folder** = Build artifacts (not needed)
- **`bin/` folder** = Contains the DLL after building

**For installation, you ONLY need:**
- The `.dll` file (from `bin/Release/net8.0/`)
- The `manifest.json` file

That's it! Just 2 files!

## Can You Copy to NAS?

**Yes, but only after building!**

1. Build on Linux → creates the `.dll`
2. Copy the `.dll` and `manifest.json` to your NAS
3. Copy those 2 files from NAS to Jellyfin plugins folder
4. Restart Jellyfin

**But you MUST build first!** The source files alone won't work.



