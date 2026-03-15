param(
    [ValidateNotNullOrEmpty()]
    [string]$Configuration = "Release"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$projectPath = Join-Path $repoRoot "src\BreakerAndOpenDoor\BreakerAndOpenDoor.csproj"

$publishDir = Join-Path $repoRoot "artifacts\publish\breakerandopendoor"
$bundleRoot = Join-Path $repoRoot "artifacts\release\breakerandopendoor"
$legacyBundleRoot = Join-Path $repoRoot "artifacts\release\RetakePluginHost"
$pluginDir = Join-Path $bundleRoot "addons\counterstrikesharp\plugins\breakerandopendoor"
$configDir = Join-Path $bundleRoot "addons\counterstrikesharp\configs\plugins\breakerandopendoor"

$configSource = Join-Path $repoRoot "addons\counterstrikesharp\configs\plugins\breakerandopendoor\breakerandopendoor.json"
$configTarget = Join-Path $configDir "breakerandopendoor.json"

$zipPath = Join-Path $repoRoot "artifacts\release\breakerandopendoor.zip"
$legacyZipPath = Join-Path $repoRoot "artifacts\release\RetakePluginHost.zip"

$dotnetCmd = Get-Command dotnet -ErrorAction SilentlyContinue
$dotnetExe = if ($dotnetCmd) {
    $dotnetCmd.Source
}
elseif (Test-Path "$env:ProgramFiles\dotnet\dotnet.exe") {
    "$env:ProgramFiles\dotnet\dotnet.exe"
}
elseif (Test-Path "$env:USERPROFILE\.dotnet\dotnet.exe") {
    "$env:USERPROFILE\.dotnet\dotnet.exe"
}
else {
    $null
}

if ([string]::IsNullOrWhiteSpace($dotnetExe)) {
    throw "dotnet SDK not found (PATH and common locations). Install .NET 8 SDK, then run scripts/build-release.ps1 again."
}

Write-Host "==> Detected dotnet: $dotnetExe"

Write-Host "==> Cleaning previous artifacts"
Remove-Item -Path $publishDir -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path $bundleRoot -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path $zipPath -Force -ErrorAction SilentlyContinue
Remove-Item -Path $legacyBundleRoot -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path $legacyZipPath -Force -ErrorAction SilentlyContinue

Write-Host "==> Publishing plugin"
& $dotnetExe publish $projectPath -c $Configuration -o $publishDir
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed (code $LASTEXITCODE)."
}

Write-Host "==> Building server bundle"
New-Item -ItemType Directory -Path $pluginDir -Force | Out-Null
New-Item -ItemType Directory -Path $configDir -Force | Out-Null

$artifactNames = @(
    "breakerandopendoor.dll",
    "breakerandopendoor.deps.json",
    "breakerandopendoor.pdb"
)

foreach ($name in $artifactNames) {
    $source = Join-Path $publishDir $name
    if (Test-Path $source) {
        Copy-Item -Path $source -Destination (Join-Path $pluginDir $name) -Force
    }
}

if (-not (Test-Path $configSource)) {
    throw "Config file not found: $configSource"
}
Copy-Item -Path $configSource -Destination $configTarget -Force

Write-Host "==> Creating release archive"
Compress-Archive -Path (Join-Path $bundleRoot "*") -DestinationPath $zipPath -Force

Write-Host ""
Write-Host "Bundle ready:" -ForegroundColor Green
Write-Host " - Folder: $bundleRoot"
Write-Host " - Archive: $zipPath"
Write-Host ""
Write-Host "Deploy: copy the contents of $bundleRoot into game\csgo\"
