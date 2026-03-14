param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$Owner,

    [ValidateNotNullOrEmpty()]
    [string]$Repo = "breakerandopendoor",

    [ValidateNotNullOrEmpty()]
    [string]$Tag = "v0.1.0"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
Set-Location $repoRoot

$isRepo = git rev-parse --is-inside-work-tree 2>$null
if (-not $isRepo) {
    throw "Ce dossier n'est pas un depot git."
}

$remoteUrl = "https://github.com/$Owner/$Repo.git"
$existingRemote = git remote get-url origin 2>$null
if ($LASTEXITCODE -eq 0 -and -not [string]::IsNullOrWhiteSpace($existingRemote)) {
    git remote set-url origin $remoteUrl
} else {
    git remote add origin $remoteUrl
}

Write-Host "==> Push main vers $remoteUrl"
git push -u origin main

$existingTag = git tag -l $Tag
if ([string]::IsNullOrWhiteSpace($existingTag)) {
    git tag $Tag
}

Write-Host "==> Push tag $Tag"
git push origin $Tag

Write-Host ""
Write-Host "Publication terminee." -ForegroundColor Green
Write-Host "Le workflow .github/workflows/release.yml publiera automatiquement la release GitHub sur le tag $Tag."
