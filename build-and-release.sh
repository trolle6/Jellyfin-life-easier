#!/bin/bash
# Complete build and release script for Linux
# Run this on your Linux server where Jellyfin is installed

set -e

echo "=========================================="
echo "Jellyfin Easier Life - Build & Release"
echo "=========================================="
echo ""

# Check if we're in the right directory
if [ ! -f "Jellyfin-easier-life/Jellyfin-easier-life.csproj" ]; then
    echo "Error: Please run this from the repository root directory"
    exit 1
fi

cd Jellyfin-easier-life

# Find Jellyfin installation
echo "🔍 Looking for Jellyfin server DLLs..."
JELLYFIN_PATHS=(
    "/usr/lib/jellyfin"
    "/opt/jellyfin"
    "/usr/share/jellyfin"
    "/var/lib/jellyfin"
    "/usr/lib/jellyfin-ffmpeg/../server"
)

JELLYFIN_DIR=""
for path in "${JELLYFIN_PATHS[@]}"; do
    if [ -f "$path/MediaBrowser.Common.dll" ] 2>/dev/null; then
        JELLYFIN_DIR="$path"
        echo "✅ Found Jellyfin at: $JELLYFIN_DIR"
        break
    fi
done

if [ -z "$JELLYFIN_DIR" ]; then
    echo "❌ Could not find Jellyfin installation automatically."
    echo ""
    echo "Please find your Jellyfin DLLs location:"
    echo "  find /usr -name 'MediaBrowser.Common.dll' 2>/dev/null"
    echo "  find /opt -name 'MediaBrowser.Common.dll' 2>/dev/null"
    echo ""
    read -p "Enter Jellyfin installation path: " JELLYFIN_DIR
    
    if [ ! -f "$JELLYFIN_DIR/MediaBrowser.Common.dll" ]; then
        echo "❌ Invalid path. DLL not found."
        exit 1
    fi
fi

# Update .csproj with Jellyfin paths
echo ""
echo "📝 Updating project file..."
PROJECT_FILE="Jellyfin-easier-life.csproj"

# Create backup
cp "$PROJECT_FILE" "$PROJECT_FILE.bak"

# Uncomment and update Reference items
sed -i "s|<!-- <Reference Include=\"MediaBrowser.Common\">|<Reference Include=\"MediaBrowser.Common\">|g" "$PROJECT_FILE"
sed -i "s|<!-- <Reference Include=\"MediaBrowser.Controller\">|<Reference Include=\"MediaBrowser.Controller\">|g" "$PROJECT_FILE"
sed -i "s|<!-- <Reference Include=\"MediaBrowser.Model\">|<Reference Include=\"MediaBrowser.Model\">|g" "$PROJECT_FILE"
sed -i "s|<!-- <Reference Include=\"Jellyfin.Controller\">|<Reference Include=\"Jellyfin.Controller\">|g" "$PROJECT_FILE"
sed -i "s|<!-- <Reference Include=\"Microsoft.Extensions.Logging.Abstractions\">|<Reference Include=\"Microsoft.Extensions.Logging.Abstractions\">|g" "$PROJECT_FILE"

# Update paths
sed -i "s|C:\\\\Program Files\\\\Jellyfin\\\\Server|$JELLYFIN_DIR|g" "$PROJECT_FILE"
sed -i "s|C:/Program Files/Jellyfin/Server|$JELLYFIN_DIR|g" "$PROJECT_FILE"

# Remove closing comment tags
sed -i "s|</Reference> -->|</Reference>|g" "$PROJECT_FILE"

echo "✅ Project file updated"

# Build the plugin
echo ""
echo "🔨 Building plugin..."
dotnet build -c Release

if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    mv "$PROJECT_FILE.bak" "$PROJECT_FILE"
    exit 1
fi

# Package the plugin
echo ""
echo "📦 Packaging plugin..."
OUTPUT_DIR="release"
ZIP_NAME="Jellyfin-easier-life.zip"

rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

DLL_PATH="bin/Release/net8.0/Jellyfin-easier-life.dll"
MANIFEST_PATH="manifest.json"

if [ ! -f "$DLL_PATH" ]; then
    echo "❌ DLL not found at $DLL_PATH"
    exit 1
fi

if [ ! -f "$MANIFEST_PATH" ]; then
    echo "❌ manifest.json not found"
    exit 1
fi

cp "$DLL_PATH" "$OUTPUT_DIR/"
cp "$MANIFEST_PATH" "$OUTPUT_DIR/"

rm -f "$ZIP_NAME"
cd "$OUTPUT_DIR"
zip -r "../$ZIP_NAME" .
cd ..

# Calculate checksum
CHECKSUM=$(sha1sum "$ZIP_NAME" | awk '{print $1}')
CHECKSUM_FULL="sha1:$CHECKSUM"
TIMESTAMP=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

echo ""
echo "=========================================="
echo "✅ Plugin built and packaged!"
echo "=========================================="
echo ""
echo "ZIP file: $ZIP_NAME"
echo "SHA1 checksum: $CHECKSUM_FULL"
echo "Timestamp: $TIMESTAMP"
echo ""

# Check if GitHub CLI is available
if command -v gh &> /dev/null; then
    echo "🔐 GitHub CLI found. Creating release..."
    
    if gh auth status &> /dev/null; then
        # Create or update release
        if gh release view v1.0.0 --repo trolle6/Jellyfin-life-easier &> /dev/null; then
            echo "📤 Uploading to existing release..."
            gh release upload v1.0.0 "$ZIP_NAME" --repo trolle6/Jellyfin-life-easier --clobber
        else
            echo "📤 Creating new release..."
            gh release create v1.0.0 "$ZIP_NAME" \
                --title "v1.0.0 - Initial Release" \
                --notes "Initial release of Jellyfin Easier Life plugin.

## Features
- Converts soft library scans to full metadata replacement
- Universal settings support
- Season combination functionality

## Installation
Add this repository URL to Jellyfin:
\`\`\`
https://raw.githubusercontent.com/trolle6/Jellyfin-life-easier/main/repository.json
\`\`\`" \
                --repo trolle6/Jellyfin-life-easier
        fi
        
        echo ""
        echo "✅ Release created/updated on GitHub!"
        echo ""
        echo "📋 Next: Update repository.json with:"
        echo "   checksum: $CHECKSUM_FULL"
        echo "   timestamp: $TIMESTAMP"
        echo ""
        echo "Then commit and push:"
        echo "   git add repository.json"
        echo "   git commit -m 'Update repository.json for v1.0.0'"
        echo "   git push origin main"
    else
        echo "⚠️  GitHub CLI not authenticated. Run: gh auth login"
        echo ""
        echo "Or manually upload $ZIP_NAME to:"
        echo "   https://github.com/trolle6/Jellyfin-life-easier/releases"
    fi
else
    echo "⚠️  GitHub CLI not found. Please:"
    echo "   1. Upload $ZIP_NAME to: https://github.com/trolle6/Jellyfin-life-easier/releases"
    echo "   2. Update repository.json with checksum: $CHECKSUM_FULL"
    echo "   3. Update timestamp: $TIMESTAMP"
fi

# Restore backup
mv "$PROJECT_FILE.bak" "$PROJECT_FILE"

echo ""
echo "✅ Done!"



