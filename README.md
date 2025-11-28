# Jellyfin Easier Life Plugin

A Jellyfin plugin that enhances library scanning to replace all metadata (instead of soft scans) and provides universal settings like combining all seasons into one.

## Features

1. **Full Metadata Replacement**: Converts "soft" library scans into full metadata replacement operations
2. **Universal Settings**: Provides settings that apply across all libraries, such as:
   - Combine all seasons into a single season
   - Force metadata refresh even if metadata exists
   - Replace images during metadata refresh

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- Jellyfin Server installation (for referencing assemblies)
- Access to Jellyfin server DLLs

### Building the Plugin

1. **Reference Jellyfin Server Assemblies**

   You need to reference the Jellyfin server assemblies. There are two approaches:

   **Option A: Reference from Jellyfin Installation**
   
   Update `Jellyfin-easier-life.csproj` to reference the DLLs from your Jellyfin installation:
   ```xml
   <ItemGroup>
     <Reference Include="MediaBrowser.Common">
       <HintPath>C:\Program Files\Jellyfin\Server\MediaBrowser.Common.dll</HintPath>
     </Reference>
     <Reference Include="MediaBrowser.Controller">
       <HintPath>C:\Program Files\Jellyfin\Server\MediaBrowser.Controller.dll</HintPath>
     </Reference>
     <Reference Include="MediaBrowser.Model">
       <HintPath>C:\Program Files\Jellyfin\Server\MediaBrowser.Model.dll</HintPath>
     </Reference>
     <Reference Include="Jellyfin.Controller">
       <HintPath>C:\Program Files\Jellyfin\Server\Jellyfin.Controller.dll</HintPath>
     </Reference>
   </ItemGroup>
   ```

   **Option B: Use Jellyfin Source Code**
   
   Clone the Jellyfin repository and reference the projects directly:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\jellyfin\MediaBrowser.Common\MediaBrowser.Common.csproj" />
     <ProjectReference Include="..\jellyfin\MediaBrowser.Controller\MediaBrowser.Controller.csproj" />
     <ProjectReference Include="..\jellyfin\MediaBrowser.Model\MediaBrowser.Model.csproj" />
     <ProjectReference Include="..\jellyfin\Jellyfin.Controller\Jellyfin.Controller.csproj" />
   </ItemGroup>
   ```

2. **Build the Plugin**
   ```bash
   dotnet build -c Release
   ```

3. **Install the Plugin**
   
   Copy the built DLL to your Jellyfin plugins directory:
   - Windows: `%ProgramData%\Jellyfin\Server\plugins\`
   - Linux: `/var/lib/jellyfin/plugins/`
   - macOS: `~/.local/share/jellyfin/plugins/`

4. **Restart Jellyfin Server**

## Configuration

After installation, configure the plugin through the Jellyfin web interface:
- Dashboard → Plugins → Jellyfin Easier Life

### Settings

- **Replace All Metadata**: Enable to replace all metadata during library scans (default: enabled)
- **Combine All Seasons**: Enable to combine all seasons of a series into one season (default: disabled)
- **Force Metadata Refresh**: Force metadata refresh even if metadata already exists (default: enabled)
- **Replace Images**: Replace images during metadata refresh (default: enabled)

## API Endpoints

The plugin provides the following API endpoints:

### Replace Metadata for Library
```
POST /Library/ReplaceMetadata?libraryId={guid}&replaceImages=true
```

### Combine Seasons for Series
```
POST /Library/CombineSeasons?seriesId={guid}
```

### Combine Seasons for All Series in Library
```
POST /Library/CombineSeasonsForLibrary?libraryId={guid}
```

## How It Works

### Metadata Replacement

The plugin intercepts library scan operations and replaces the default "soft scan" behavior with a full metadata refresh. This ensures that:

- All metadata is fetched fresh from providers
- Existing metadata is replaced, not just updated
- Images are replaced if configured

### Season Combination

When enabled, the plugin can combine all seasons of a series into a single season. This is useful for:

- Anime series with multiple seasons that should be treated as one
- Series where season boundaries are arbitrary
- Simplifying library organization

**Note**: Season combination is a destructive operation. Episodes from multiple seasons will be renumbered and moved to season 1. Use with caution and backup your library first.

## Development

### Project Structure

```
Jellyfin-easier-life/
├── Plugin.cs                          # Main plugin class
├── PluginConfiguration.cs              # Plugin configuration
├── PluginServiceRegistration.cs       # Service registration
├── Controllers/
│   └── LibraryController.cs           # API endpoints
├── Services/
│   ├── MetadataReplacementService.cs  # Metadata replacement logic
│   └── SeasonCombinationService.cs    # Season combination logic
└── Hooks/
    └── LibraryScanHook.cs             # Library scan interception
```

## Distribution

### Packaging the Plugin

Use the provided scripts to package the plugin for distribution:

**Windows (PowerShell):**
```powershell
.\package-plugin.ps1
```

**Linux/Mac (Bash):**
```bash
chmod +x package-plugin.sh
./package-plugin.sh
```

This will create a `Jellyfin-easier-life.zip` file containing the DLL and manifest.json.

### Setting Up a Plugin Repository

To make your plugin installable via URL in Jellyfin, see [REPOSITORY_SETUP.md](REPOSITORY_SETUP.md) for detailed instructions on:
- Hosting the repository manifest
- Creating GitHub releases
- Adding the repository to Jellyfin

The `repository.json` file is already created - just update it with your actual URLs and checksums after packaging.

## License

This plugin is provided as-is. Use at your own risk.

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

