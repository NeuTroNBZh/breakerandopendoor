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
    throw "This folder is not a git repository."
}

$remoteUrl = "https://github.com/$Owner/$Repo.git"
$hasOrigin = $false
$remoteNames = git remote
foreach ($name in $remoteNames) {
    if ($name -eq "origin") {
        $hasOrigin = $true
        break
    }
}

if ($hasOrigin) {
    git remote set-url origin $remoteUrl
} else {
    git remote add origin $remoteUrl
}

Write-Host "==> Push main to $remoteUrl"
git push -u origin main

$existingTag = git tag -l $Tag
if ([string]::IsNullOrWhiteSpace($existingTag)) {
    git tag $Tag
}

Write-Host "==> Push tag $Tag"
git push origin $Tag

Write-Host ""
Write-Host "Publication completed." -ForegroundColor Green
Write-Host "The .github/workflows/release.yml workflow will publish the GitHub release automatically for tag $Tag."
