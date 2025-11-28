#!/bin/bash
# Bash script to package the Jellyfin plugin for distribution

set -e

echo "Packaging Jellyfin Easier Life plugin..."

# Build the plugin
echo "Building plugin..."
dotnet build -c Release

# Create output directory
OUTPUT_DIR="release"
ZIP_NAME="Jellyfin-easier-life.zip"

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
echo "Calculating SHA1 checksum..."
CHECKSUM=$(sha1sum "$ZIP_NAME" | awk '{print $1}')
CHECKSUM_FULL="sha1:$CHECKSUM"

echo ""
echo "Plugin packaged successfully!"
echo "ZIP file: $ZIP_NAME"
echo "SHA1 checksum: $CHECKSUM_FULL"
echo ""
echo "Update repository.json with:"
echo "  - sourceUrl: Your GitHub release URL"
echo "  - checksum: $CHECKSUM_FULL"
echo "  - timestamp: Current timestamp"

