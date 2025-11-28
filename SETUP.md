# Quick Setup Guide

## Step 1: Get Jellyfin Server Assemblies

You need to reference Jellyfin server DLLs to build this plugin. Choose one method:

### Method A: From Jellyfin Installation

1. Find your Jellyfin server installation directory:
   - Windows: `C:\Program Files\Jellyfin\Server\`
   - Linux: `/usr/lib/jellyfin/` or `/opt/jellyfin/`
   - macOS: Check your installation location

2. Edit `Jellyfin-easier-life.csproj` and uncomment the `<Reference>` items, updating the paths:

```xml
<ItemGroup>
  <Reference Include="MediaBrowser.Common">
    <HintPath>C:\Program Files\Jellyfin\Server\MediaBrowser.Common.dll</HintPath>
    <Private>False</Private>
  </Reference>
  <Reference Include="MediaBrowser.Controller">
    <HintPath>C:\Program Files\Jellyfin\Server\MediaBrowser.Controller.dll</HintPath>
    <Private>False</Private>
  </Reference>
  <Reference Include="MediaBrowser.Model">
    <HintPath>C:\Program Files\Jellyfin\Server\MediaBrowser.Model.dll</HintPath>
    <Private>False</Private>
  </Reference>
  <Reference Include="Jellyfin.Controller">
    <HintPath>C:\Program Files\Jellyfin\Server\Jellyfin.Controller.dll</HintPath>
    <Private>False</Private>
  </Reference>
</ItemGroup>
```

### Method B: From Jellyfin Source Code

1. Clone the Jellyfin repository:
   ```bash
   git clone https://github.com/jellyfin/jellyfin.git
   ```

2. Edit `Jellyfin-easier-life.csproj` and use ProjectReference:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\jellyfin\MediaBrowser.Common\MediaBrowser.Common.csproj" />
  <ProjectReference Include="..\..\jellyfin\MediaBrowser.Controller\MediaBrowser.Controller.csproj" />
  <ProjectReference Include="..\..\jellyfin\MediaBrowser.Model\MediaBrowser.Model.csproj" />
  <ProjectReference Include="..\..\jellyfin\Jellyfin.Controller\Jellyfin.Controller.csproj" />
</ItemGroup>
```

## Step 2: Build the Plugin

```bash
cd Jellyfin-easier-life
dotnet build -c Release
```

## Step 3: Install the Plugin

Copy the built files from `bin/Release/net8.0/` to your Jellyfin plugins directory:

**Required files:**
- `Jellyfin-easier-life.dll` (the plugin assembly)
- `manifest.json` (the plugin manifest)

**Plugin directories:**
- **Windows**: `%ProgramData%\Jellyfin\Server\plugins\`
- **Linux**: `/var/lib/jellyfin/plugins/` or `/usr/share/jellyfin/plugins/`
- **macOS**: `~/.local/share/jellyfin/plugins/`

**Note:** Both the DLL and manifest.json must be in the same plugins directory for Jellyfin to recognize the plugin.

## Step 4: Restart Jellyfin

Restart your Jellyfin server to load the plugin.

## Step 5: Configure the Plugin

1. Open Jellyfin web interface
2. Go to Dashboard → Plugins
3. Find "Jellyfin Easier Life" and configure:
   - Enable "Replace All Metadata" to convert soft scans to full metadata replacement
   - Enable "Combine All Seasons" if you want to combine seasons (use with caution!)
   - Configure other settings as needed

## Troubleshooting

### Build Errors

If you get errors about missing types or assemblies:
- Verify the paths in your `.csproj` file point to the correct Jellyfin installation
- Make sure you're using the same .NET version as your Jellyfin server
- Check that all required DLLs are present in the Jellyfin server directory

### Plugin Not Loading

- Check Jellyfin server logs for plugin loading errors
- Verify the DLL is in the correct plugins directory
- Ensure the plugin DLL and all dependencies are present
- Make sure the plugin GUID is unique (change it if needed)

### Runtime Errors

- Check Jellyfin server logs for detailed error messages
- Verify plugin configuration is correct
- Test with a small library first before running on large libraries

## Next Steps

- Test the metadata replacement on a test library first
- Use the API endpoints to manually trigger operations
- Monitor Jellyfin logs for any issues
- Report bugs or suggest improvements!

