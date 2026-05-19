# Download PixelLab character ZIP (with walk animations) and extract frames for Godot.
# Usage: .\tools\download_pixellab_animations.ps1 -CharacterId <uuid> -Slug chr_player_dev

param(
    [Parameter(Mandatory = $true)][string]$CharacterId,
    [Parameter(Mandatory = $true)][string]$Slug
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$outRoot = Join-Path $root "src\assets\art\animations\$Slug"
$zipPath = Join-Path $env:TEMP "pixellab-$CharacterId.zip"
$extract = Join-Path $env:TEMP "pixellab-$CharacterId"

$downloadUrl = "https://api.pixellab.ai/mcp/characters/$CharacterId/download"
Write-Host "Downloading $Slug ..."
curl.exe --ssl-no-revoke -L -f -o $zipPath $downloadUrl

if (Test-Path $extract) { Remove-Item -Recurse -Force $extract }
Expand-Archive -Path $zipPath -DestinationPath $extract -Force

$dirs = @("south", "east", "north", "west")
$frameDirs = Get-ChildItem -Path $extract -Recurse -Directory |
    Where-Object {
        $_.Name -in $dirs -and
        (Get-ChildItem $_.FullName -Filter "frame_*.png" -ErrorAction SilentlyContinue).Count -ge 1
    }

if ($frameDirs.Count -eq 0) {
    Write-Error "No animation direction folders found. Inspect: $extract"
}

if (Test-Path $outRoot) { Remove-Item -Recurse -Force $outRoot }
New-Item -ItemType Directory -Force -Path $outRoot | Out-Null

foreach ($dir in $frameDirs) {
    $direction = $dir.Name
    $destDir = Join-Path $outRoot "walk\$direction"
    New-Item -ItemType Directory -Force -Path $destDir | Out-Null

    Get-ChildItem $dir.FullName -Filter "frame_*.png" | Sort-Object Name | ForEach-Object {
        Copy-Item -Force $_.FullName (Join-Path $destDir $_.Name)
    }
    Write-Host "  walk/$direction -> $((Get-ChildItem $destDir).Count) frames"
}

Write-Host "Done: $outRoot"
