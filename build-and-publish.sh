#!/bin/bash
# OneCiel.Core.Dynamics - NuGet Package Build and Publish Script
# This script builds and publishes both packages to NuGet.org

set -e

# Configuration
CONFIGURATION="${1:-Release}"
SKIP_BUILD="${2:-false}"
SKIP_PUBLISH="${3:-false}"
NUGET_API_KEY="${4:-${NUGET_API_KEY}}"

# Colors
CYAN='\033[0;36m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}OneCiel.Core.Dynamics Package Builder${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

# Get the script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

# Project paths
CORE_PROJECT="$SCRIPT_DIR/OneCiel.Core.Dynamics/OneCiel.Core.Dynamics.csproj"
JSON_EXTENSION_PROJECT="$SCRIPT_DIR/OneCiel.Core.Dynamics.JsonExtension/OneCiel.Core.Dynamics.JsonExtension.csproj"
PACKAGES_DIR="$SCRIPT_DIR/packages"

# Validate projects exist
if [ ! -f "$CORE_PROJECT" ]; then
    echo "Error: Core project not found: $CORE_PROJECT"
    exit 1
fi

if [ ! -f "$JSON_EXTENSION_PROJECT" ]; then
    echo "Error: JsonExtension project not found: $JSON_EXTENSION_PROJECT"
    exit 1
fi

# Create packages directory
mkdir -p "$PACKAGES_DIR"

# Step 1: Clean
echo -e "${YELLOW}Step 1: Cleaning previous builds...${NC}"
dotnet clean --configuration "$CONFIGURATION"

# Step 2: Restore
echo -e "${YELLOW}Step 2: Restoring NuGet packages...${NC}"
dotnet restore

# Step 3: Build
if [ "$SKIP_BUILD" != "true" ]; then
    echo -e "${YELLOW}Step 3: Building solution...${NC}"
    dotnet build --configuration "$CONFIGURATION" --no-restore
    echo -e "${GREEN}Build completed successfully!${NC}"
else
    echo -e "${YELLOW}Step 3: Skipping build (SKIP_BUILD=true)${NC}"
fi

# Step 4: Pack
echo -e "${YELLOW}Step 4: Creating NuGet packages...${NC}"

# Pack Core Library
echo -e "${CYAN}  Packing OneCiel.Core.Dynamics...${NC}"
dotnet pack "$CORE_PROJECT" --configuration "$CONFIGURATION" --no-build --output "$PACKAGES_DIR"

# Pack JsonExtension
echo -e "${CYAN}  Packing OneCiel.Core.Dynamics.JsonExtension...${NC}"
dotnet pack "$JSON_EXTENSION_PROJECT" --configuration "$CONFIGURATION" --no-build --output "$PACKAGES_DIR"

echo -e "${GREEN}Packaging completed successfully!${NC}"

# Find package files
CORE_PACKAGE=$(ls -t "$PACKAGES_DIR"/OneCiel.Core.Dynamics.*.nupkg 2>/dev/null | head -1)
JSON_PACKAGE=$(ls -t "$PACKAGES_DIR"/OneCiel.Core.Dynamics.JsonExtension.*.nupkg 2>/dev/null | head -1)

if [ -z "$CORE_PACKAGE" ]; then
    echo "Error: Core package file not found"
    exit 1
fi

if [ -z "$JSON_PACKAGE" ]; then
    echo "Error: JsonExtension package file not found"
    exit 1
fi

echo ""
echo -e "${GREEN}Package files created:${NC}"
echo -e "${CYAN}  - $CORE_PACKAGE${NC}"
echo -e "${CYAN}  - $JSON_PACKAGE${NC}"
echo ""

# Step 5: Publish
if [ "$SKIP_PUBLISH" != "true" ]; then
    if [ -z "$NUGET_API_KEY" ]; then
        echo "Error: NuGet API Key is required for publishing. Please provide it via NUGET_API_KEY environment variable or as the 4th argument."
        exit 1
    fi
    
    echo -e "${YELLOW}Step 5: Publishing packages to NuGet.org...${NC}"
    
    # Publish Core Library
    echo -e "${CYAN}  Publishing OneCiel.Core.Dynamics...${NC}"
    if dotnet nuget push "$CORE_PACKAGE" --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json --skip-duplicate; then
        echo -e "${GREEN}  ✓ OneCiel.Core.Dynamics published successfully!${NC}"
    else
        echo -e "${YELLOW}  ⚠ Failed to publish OneCiel.Core.Dynamics (may already exist)${NC}"
    fi
    
    # Publish JsonExtension
    echo -e "${CYAN}  Publishing OneCiel.Core.Dynamics.JsonExtension...${NC}"
    if dotnet nuget push "$JSON_PACKAGE" --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json --skip-duplicate; then
        echo -e "${GREEN}  ✓ OneCiel.Core.Dynamics.JsonExtension published successfully!${NC}"
    else
        echo -e "${YELLOW}  ⚠ Failed to publish OneCiel.Core.Dynamics.JsonExtension (may already exist)${NC}"
    fi
    
    echo ""
    echo -e "${GREEN}Publishing completed!${NC}"
else
    echo -e "${YELLOW}Step 5: Skipping publish (SKIP_PUBLISH=true)${NC}"
    echo ""
    echo -e "${YELLOW}To publish manually, run:${NC}"
    echo -e "${CYAN}  dotnet nuget push \"$CORE_PACKAGE\" --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json${NC}"
    echo -e "${CYAN}  dotnet nuget push \"$JSON_PACKAGE\" --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json${NC}"
fi

echo ""
echo -e "${CYAN}========================================${NC}"
echo -e "${GREEN}Build and Publish Complete!${NC}"
echo -e "${CYAN}========================================${NC}"


