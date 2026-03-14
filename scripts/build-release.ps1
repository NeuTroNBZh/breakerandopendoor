param(
    [ValidateNotNullOrEmpty()]
    [string]$Configuration = "Release"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$projectPath = Join-Path $repoRoot "src\RetakePlugin\RetakePluginHost.csproj"

$publishDir = Join-Path $repoRoot "artifacts\publish\breakerandopendoor"
$bundleRoot = Join-Path $repoRoot "artifacts\release\breakerandopendoor"
$pluginDir = Join-Path $bundleRoot "addons\counterstrikesharp\plugins\breakerandopendoor"
$configDir = Join-Path $bundleRoot "addons\counterstrikesharp\configs\plugins\breakerandopendoor"

$configSource = Join-Path $repoRoot "addons\counterstrikesharp\configs\plugins\breakerandopendoor\breakerandopendoor.json"
$configTarget = Join-Path $configDir "breakerandopendoor.json"

$zipPath = Join-Path $repoRoot "artifacts\release\breakerandopendoor.zip"

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
    throw "dotnet SDK introuvable (PATH et chemins usuels). Installe .NET 8 SDK puis relance scripts/build-release.ps1."
}

Write-Host "==> Dotnet detecte: $dotnetExe"

Write-Host "==> Nettoyage des anciens artefacts"
Remove-Item -Path $publishDir -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path $bundleRoot -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path $zipPath -Force -ErrorAction SilentlyContinue

Write-Host "==> Publication du plugin"
& $dotnetExe publish $projectPath -c $Configuration -o $publishDir
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish a echoue (code $LASTEXITCODE)."
}

Write-Host "==> Construction du bundle serveur"
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
    throw "Fichier de config introuvable: $configSource"
}
Copy-Item -Path $configSource -Destination $configTarget -Force

Write-Host "==> Creation de l'archive release"
Compress-Archive -Path (Join-Path $bundleRoot "*") -DestinationPath $zipPath -Force

Write-Host ""
Write-Host "Bundle pret:" -ForegroundColor Green
Write-Host " - Dossier: $bundleRoot"
Write-Host " - Archive: $zipPath"
Write-Host ""
Write-Host "Deploy: copier le contenu de $bundleRoot dans game\csgo\"
