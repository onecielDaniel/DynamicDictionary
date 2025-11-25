# NuGet Package Build and Publish Guide

This guide explains how to build and publish the OneCiel.Core.Dynamics NuGet packages.

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

## Automated CI/CD with GitHub Actions

This repository includes a GitHub Actions workflow that automatically builds and publishes NuGet packages whenever code is merged into the `master` branch.

### How It Works

The workflow (`.github/workflows/nuget-publish.yml`) automatically:
1. ✅ Builds the solution in Release configuration
2. ✅ Creates NuGet packages for both projects
3. ✅ Publishes packages to NuGet.org
4. ✅ Saves packages as GitHub Actions artifacts

### Setup Instructions

#### 1. Create NuGet.org API Key

1. Go to https://www.nuget.org/account/apikeys
2. Click **"Create"**
3. Enter a name for your API key (e.g., "GitHub Actions")
4. Under **"Select Scopes"**, choose **"Push"**
5. Under **"Select Packages"**, choose:
   - **"Select specific packages"** and select your packages, OR
   - **"All packages"** (if you want to publish any package from this account)
6. Click **"Create"**
7. **Copy the API key** (you won't be able to see it again!)

#### 2. Add API Key to GitHub Secrets

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **"New repository secret"**
4. Name: `NUGET_API_KEY`
5. Value: Paste your NuGet API key
6. Click **"Add secret"**

#### 3. Test the Workflow

1. Make a change to your code
2. Commit and push to `master` branch (or merge a PR)
3. Go to **Actions** tab in GitHub
4. Watch the workflow run automatically!

### Workflow Triggers

The workflow runs automatically when:
- ✅ Code is pushed directly to `master` branch
- ✅ A Pull Request is merged into `master` branch

The workflow does **NOT** run on:
- ❌ Pull Requests that are opened but not merged
- ❌ Pushes to other branches

### Workflow Features

- **Automatic Versioning**: Uses version from `.csproj` files
- **Skip Duplicates**: Won't fail if package version already exists
- **Artifact Storage**: Packages are saved as artifacts for 30 days
- **Error Handling**: Continues even if one package fails to publish

### Monitoring Workflows

1. Go to the **Actions** tab in your GitHub repository
2. Click on a workflow run to see detailed logs
3. Check the **Artifacts** section to download generated packages

### Troubleshooting CI/CD

**Workflow not running?**
- Check that the workflow file is in `.github/workflows/` directory
- Verify the file is named `nuget-publish.yml`
- Ensure you're pushing to `master` branch

**Publish failing?**
- Verify `NUGET_API_KEY` secret is set correctly
- Check that the API key has "Push" permissions
- Ensure the API key hasn't expired
- Check workflow logs for detailed error messages

**Package version already exists?**
- Update the version in `.csproj` files before merging
- The workflow uses `--skip-duplicate` flag, so it won't fail, but the package won't be updated

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
dotnet pack OneCiel.Core.Dynamics\OneCiel.Core.Dynamics.csproj \
    --configuration Release \
    --no-build \
    --output packages

# Pack JSON Extension
dotnet pack OneCiel.Core.Dynamics.JsonExtension\OneCiel.Core.Dynamics.JsonExtension.csproj \
    --configuration Release \
    --no-build \
    --output packages
```

### Step 5: Verify Packages

```bash
# List created packages
ls packages/*.nupkg

# Inspect package contents (optional)
dotnet nuget verify packages/OneCiel.Core.Dynamics.*.nupkg
```

### Step 6: Publish to NuGet.org

```bash
# Publish Core Library
dotnet nuget push packages/OneCiel.Core.Dynamics.*.nupkg \
    --api-key YOUR_API_KEY \
    --source https://api.nuget.org/v3/index.json \
    --skip-duplicate

# Publish JSON Extension
dotnet nuget push packages/OneCiel.Core.Dynamics.JsonExtension.*.nupkg \
    --api-key YOUR_API_KEY \
    --source https://api.nuget.org/v3/index.json \
    --skip-duplicate
```

## Package Information

### OneCiel.Core.Dynamics

- **Package ID**: `OneCiel.Core.Dynamics`
- **Version**: 1.0.0
- **Target Framework**: .NET Standard 2.1
- **Dependencies**: None

### OneCiel.Core.Dynamics.JsonExtension

- **Package ID**: `OneCiel.Core.Dynamics.JsonExtension`
- **Version**: 1.0.0
- **Target Frameworks**: .NET 8.0, .NET 9.0
- **Dependencies**: 
  - `OneCiel.Core.Dynamics` (>= 1.0.0)
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

