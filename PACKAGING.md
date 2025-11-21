# NuGet Package Build and Publish Guide

This guide explains how to build and publish the OneCiel.System.Dynamics NuGet packages.

## Prerequisites

- .NET SDK 8.0 or later
- NuGet account with API key
- PowerShell (Windows) or Bash (Linux/macOS)

## Quick Start

### Using Build Scripts (Recommended)

**Windows (PowerShell)**:
```powershell
# Build and publish
.\build-and-publish.ps1

# Build only (skip publish)
.\build-and-publish.ps1 -SkipPublish

# Build only (skip build step)
.\build-and-publish.ps1 -SkipBuild
```

**Linux/macOS (Bash)**:
```bash
# Make script executable (first time only)
chmod +x build-and-publish.sh

# Build and publish
./build-and-publish.sh

# Build only (skip publish)
./build-and-publish.sh Release false true

# Build only (skip build step)
./build-and-publish.sh Release true false
```

### Using Environment Variables

Set the NuGet API key as an environment variable:

**Windows (PowerShell)**:
```powershell
$env:NUGET_API_KEY = "your-api-key-here"
.\build-and-publish.ps1
```

**Linux/macOS (Bash)**:
```bash
export NUGET_API_KEY="your-api-key-here"
./build-and-publish.sh
```

## Manual Build Process

### Step 1: Clean Previous Builds

```bash
dotnet clean --configuration Release
```

### Step 2: Restore Dependencies

```bash
dotnet restore
```

### Step 3: Build Solution

```bash
dotnet build --configuration Release --no-restore
```

### Step 4: Create NuGet Packages

```bash
# Create packages directory
mkdir -p packages

# Pack Core Library
dotnet pack OneCiel.System.Dynamics\OneCiel.System.Dynamics.csproj \
    --configuration Release \
    --no-build \
    --output packages

# Pack JSON Extension
dotnet pack OneCiel.System.Dynamics.JsonExtension\OneCiel.System.Dynamics.JsonExtension.csproj \
    --configuration Release \
    --no-build \
    --output packages
```

### Step 5: Verify Packages

```bash
# List created packages
ls packages/*.nupkg

# Inspect package contents (optional)
dotnet nuget verify packages/OneCiel.System.Dynamics.*.nupkg
```

### Step 6: Publish to NuGet.org

```bash
# Publish Core Library
dotnet nuget push packages/OneCiel.System.Dynamics.*.nupkg \
    --api-key YOUR_API_KEY \
    --source https://api.nuget.org/v3/index.json \
    --skip-duplicate

# Publish JSON Extension
dotnet nuget push packages/OneCiel.System.Dynamics.JsonExtension.*.nupkg \
    --api-key YOUR_API_KEY \
    --source https://api.nuget.org/v3/index.json \
    --skip-duplicate
```

## Package Information

### OneCiel.System.Dynamics

- **Package ID**: `OneCiel.System.Dynamics`
- **Version**: 1.0.0
- **Target Framework**: .NET Standard 2.1
- **Dependencies**: None

### OneCiel.System.Dynamics.JsonExtension

- **Package ID**: `OneCiel.System.Dynamics.JsonExtension`
- **Version**: 1.0.0
- **Target Frameworks**: .NET 8.0, .NET 9.0
- **Dependencies**: 
  - `OneCiel.System.Dynamics` (>= 1.0.0)
  - `System.Text.Json` (included in .NET 8.0+)

## NuGet Configuration

### Using .nuget.config

1. Copy `.nuget.config.example` to `.nuget.config`
2. Add your API key to the configuration file
3. **Important**: `.nuget.config` is in `.gitignore` - do not commit API keys

### Using Environment Variables

Set `NUGET_API_KEY` environment variable before running build scripts.

### Using Build Script Parameters

Pass API key directly to build scripts:

**PowerShell**:
```powershell
.\build-and-publish.ps1 -NuGetApiKey "your-api-key"
```

**Bash**:
```bash
./build-and-publish.sh Release false false "your-api-key"
```

## Version Management

To update package versions, edit the `.csproj` files:

```xml
<PropertyGroup>
  <Version>1.0.1</Version>  <!-- Update version here -->
</PropertyGroup>
```

Then rebuild and republish.

## Troubleshooting

### Package Already Exists

If you get an error that the package already exists:
- Update the version number in `.csproj` files
- Or use `--skip-duplicate` flag (packages won't be overwritten)

### API Key Issues

- Verify your API key is correct
- Check that the API key has publish permissions
- Ensure the API key hasn't expired

### Build Failures

- Ensure all dependencies are restored: `dotnet restore`
- Check that all projects build successfully: `dotnet build`
- Verify target frameworks are compatible

### Publish Failures

- Check internet connection
- Verify NuGet.org is accessible
- Ensure API key has correct permissions
- Check package size limits (NuGet has size restrictions)

## Security Best Practices

1. **Never commit API keys** to version control
2. Use environment variables or secure configuration files
3. Rotate API keys regularly
4. Use separate API keys for different environments
5. Limit API key permissions to minimum required

## Additional Resources

- [NuGet Package Creation](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package-dotnet-cli)
- [NuGet Publishing](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
- [NuGet API Keys](https://docs.microsoft.com/en-us/nuget/nuget-org/individual-accounts#api-keys)
