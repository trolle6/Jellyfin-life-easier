#!/bin/bash
# Linux build script for Jellyfin Easier Life plugin
# Run this on your Linux server where Jellyfin is installed

set -e

echo "=========================================="
echo "Jellyfin Easier Life - Build & Package"
echo "=========================================="
echo ""

# Check if we're in the right directory
if [ ! -f "Jellyfin-easier-life.csproj" ]; then
    echo "Error: Please run this script from the Jellyfin-easier-life directory"
    exit 1
fi

# Find Jellyfin installation
echo "Looking for Jellyfin server DLLs..."
JELLYFIN_PATHS=(
    "/usr/lib/jellyfin"
    "/opt/jellyfin"
    "/usr/share/jellyfin"
    "/var/lib/jellyfin"
)

JELLYFIN_DIR=""
for path in "${JELLYFIN_PATHS[@]}"; do
    if [ -f "$path/MediaBrowser.Common.dll" ]; then
        JELLYFIN_DIR="$path"
        echo "Found Jellyfin at: $JELLYFIN_DIR"
        break
    fi
done

if [ -z "$JELLYFIN_DIR" ]; then
    echo "Warning: Could not find Jellyfin installation automatically."
    echo "Please edit Jellyfin-easier-life.csproj and uncomment the Reference items,"
    echo "then update the paths to point to your Jellyfin installation."
    echo ""
    echo "Common locations:"
    for path in "${JELLYFIN_PATHS[@]}"; do
        echo "  - $path"
    done
    echo ""
    read -p "Enter Jellyfin installation path (or press Enter to continue anyway): " JELLYFIN_DIR
fi

# Update .csproj if JELLYFIN_DIR is set
if [ -n "$JELLYFIN_DIR" ] && [ -f "$JELLYFIN_DIR/MediaBrowser.Common.dll" ]; then
    echo "Updating project file with Jellyfin paths..."
    # This is a simple approach - you may need to manually edit the .csproj
    echo "Please manually edit Jellyfin-easier-life.csproj and update the HintPath values to:"
    echo "  $JELLYFIN_DIR"
fi

# Build the plugin
echo ""
echo "Building plugin..."
dotnet build -c Release

if [ $? -ne 0 ]; then
    echo "Build failed! Please check the errors above."
    exit 1
fi

# Create output directory
OUTPUT_DIR="release"
ZIP_NAME="Jellyfin-easier-life.zip"

echo ""
echo "Packaging plugin..."
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

# Copy required files
DLL_PATH="bin/Release/net8.0/Jellyfin-easier-life.dll"
MANIFEST_PATH="manifest.json"

if [ ! -f "$DLL_PATH" ]; then
    echo "Error: DLL not found at $DLL_PATH"
    exit 1
fi

if [ ! -f "$MANIFEST_PATH" ]; then
    echo "Error: manifest.json not found at $MANIFEST_PATH"
    exit 1
fi

cp "$DLL_PATH" "$OUTPUT_DIR/"
cp "$MANIFEST_PATH" "$OUTPUT_DIR/"

# Create ZIP file
echo "Creating ZIP file..."
rm -f "$ZIP_NAME"
cd "$OUTPUT_DIR"
zip -r "../$ZIP_NAME" .
cd ..

# Calculate SHA1 checksum
echo ""
echo "Calculating SHA1 checksum..."
CHECKSUM=$(sha1sum "$ZIP_NAME" | awk '{print $1}')
CHECKSUM_FULL="sha1:$CHECKSUM"
TIMESTAMP=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

echo ""
echo "=========================================="
echo "✅ Plugin packaged successfully!"
echo "=========================================="
echo ""
echo "ZIP file: $ZIP_NAME"
echo "SHA1 checksum: $CHECKSUM_FULL"
echo "Timestamp: $TIMESTAMP"
echo ""
echo "Next steps:"
echo "1. Create a GitHub release at: https://github.com/trolle6/Jellyfin-life-easier/releases"
echo "2. Upload $ZIP_NAME to the release"
echo "3. Update repository.json with:"
echo "   - sourceUrl: https://github.com/trolle6/Jellyfin-life-easier/releases/download/v1.0.0/$ZIP_NAME"
echo "   - checksum: $CHECKSUM_FULL"
echo "   - timestamp: $TIMESTAMP"
echo "4. Commit and push repository.json"
echo ""



