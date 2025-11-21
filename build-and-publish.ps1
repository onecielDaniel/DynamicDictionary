# OneCiel.System.Dynamics - NuGet Package Build and Publish Script
# This script builds and publishes both packages to NuGet.org

param(
    [Parameter(Mandatory=$false)]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipPublish = $false,
    
    [Parameter(Mandatory=$false)]
    [string]$NuGetApiKey = $env:NUGET_API_KEY
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "OneCiel.System.Dynamics Package Builder" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get the solution directory
$SolutionDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $SolutionDir

# Project paths
$CoreProject = Join-Path $SolutionDir "OneCiel.System.Dynamics\OneCiel.System.Dynamics.csproj"
$JsonExtensionProject = Join-Path $SolutionDir "OneCiel.System.Dynamics.JsonExtension\OneCiel.System.Dynamics.JsonExtension.csproj"

# Validate projects exist
if (-not (Test-Path $CoreProject)) {
    Write-Error "Core project not found: $CoreProject"
    exit 1
}

if (-not (Test-Path $JsonExtensionProject)) {
    Write-Error "JsonExtension project not found: $JsonExtensionProject"
    exit 1
}

# Step 1: Clean
Write-Host "Step 1: Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean --configuration $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "Clean failed"
    exit 1
}

# Step 2: Restore
Write-Host "Step 2: Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed"
    exit 1
}

# Step 3: Build
if (-not $SkipBuild) {
    Write-Host "Step 3: Building solution..." -ForegroundColor Yellow
    dotnet build --configuration $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed"
        exit 1
    }
    Write-Host "Build completed successfully!" -ForegroundColor Green
} else {
    Write-Host "Step 3: Skipping build (--SkipBuild specified)" -ForegroundColor Yellow
}

# Step 4: Pack
Write-Host "Step 4: Creating NuGet packages..." -ForegroundColor Yellow

# Pack Core Library
Write-Host "  Packing OneCiel.System.Dynamics..." -ForegroundColor Cyan
$CorePackage = dotnet pack $CoreProject --configuration $Configuration --no-build --output "$SolutionDir\packages" 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to pack OneCiel.System.Dynamics"
    Write-Host $CorePackage
    exit 1
}

# Pack JsonExtension
Write-Host "  Packing OneCiel.System.Dynamics.JsonExtension..." -ForegroundColor Cyan
$JsonPackage = dotnet pack $JsonExtensionProject --configuration $Configuration --no-build --output "$SolutionDir\packages" 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to pack OneCiel.System.Dynamics.JsonExtension"
    Write-Host $JsonPackage
    exit 1
}

Write-Host "Packaging completed successfully!" -ForegroundColor Green

# Find package files
$CorePackageFile = Get-ChildItem -Path "$SolutionDir\packages" -Filter "OneCiel.System.Dynamics.*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$JsonPackageFile = Get-ChildItem -Path "$SolutionDir\packages" -Filter "OneCiel.System.Dynamics.JsonExtension.*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if (-not $CorePackageFile) {
    Write-Error "Core package file not found"
    exit 1
}

if (-not $JsonPackageFile) {
    Write-Error "JsonExtension package file not found"
    exit 1
}

Write-Host ""
Write-Host "Package files created:" -ForegroundColor Green
Write-Host "  - $($CorePackageFile.FullName)" -ForegroundColor Cyan
Write-Host "  - $($JsonPackageFile.FullName)" -ForegroundColor Cyan
Write-Host ""

# Step 5: Publish
if (-not $SkipPublish) {
    if ([string]::IsNullOrWhiteSpace($NuGetApiKey)) {
        Write-Error "NuGet API Key is required for publishing. Please provide it via -NuGetApiKey parameter or NUGET_API_KEY environment variable."
        exit 1
    }
    
    Write-Host "Step 5: Publishing packages to NuGet.org..." -ForegroundColor Yellow
    
    # Publish Core Library
    Write-Host "  Publishing OneCiel.System.Dynamics..." -ForegroundColor Cyan
    dotnet nuget push $CorePackageFile.FullName --api-key $NuGetApiKey --source https://api.nuget.org/v3/index.json --skip-duplicate
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Failed to publish OneCiel.System.Dynamics (may already exist)"
    } else {
        Write-Host "  ✓ OneCiel.System.Dynamics published successfully!" -ForegroundColor Green
    }
    
    # Publish JsonExtension
    Write-Host "  Publishing OneCiel.System.Dynamics.JsonExtension..." -ForegroundColor Cyan
    dotnet nuget push $JsonPackageFile.FullName --api-key $NuGetApiKey --source https://api.nuget.org/v3/index.json --skip-duplicate
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Failed to publish OneCiel.System.Dynamics.JsonExtension (may already exist)"
    } else {
        Write-Host "  ✓ OneCiel.System.Dynamics.JsonExtension published successfully!" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "Publishing completed!" -ForegroundColor Green
} else {
    Write-Host "Step 5: Skipping publish (--SkipPublish specified)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To publish manually, run:" -ForegroundColor Yellow
    Write-Host "  dotnet nuget push `"$($CorePackageFile.FullName)`" --api-key $NuGetApiKey --source https://api.nuget.org/v3/index.json" -ForegroundColor Cyan
    Write-Host "  dotnet nuget push `"$($JsonPackageFile.FullName)`" --api-key $NuGetApiKey --source https://api.nuget.org/v3/index.json" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build and Publish Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

