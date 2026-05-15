# Re-download PixelLab south rotations into src/assets/art/
# Requires: curl with --ssl-no-revoke on some corporate Windows networks

$ErrorActionPreference = "Stop"
$base = "https://backblaze.pixellab.ai/file/pixellab-characters/aded2ff3-b5fa-42a7-a720-2c4450839f9f"
$root = Split-Path -Parent $PSScriptRoot
$sprites = Join-Path $root "src\assets\art\sprites"
$tiles = Join-Path $root "src\assets\art\tiles"
New-Item -ItemType Directory -Force -Path $sprites, $tiles | Out-Null

$downloads = @(
    @{ Out = "chr_player_dev.png"; Url = "$base/5f750a95-08dc-4d18-bf64-90557f6a14b2/rotations/south.png" },
    @{ Out = "chr_enemy_hr.png"; Url = "$base/cd3fa112-b0bd-4a45-a703-e27f7f3226fe/rotations/south.png" },
    @{ Out = "chr_enemy_ceo.png"; Url = "$base/0d6bf6ab-f4d6-4192-b81c-95052f0e2e1a/rotations/south.png" },
    @{ Out = "chr_enemy_cto.png"; Url = "$base/103eb4fb-4415-4c70-85f4-066fd9313d62/rotations/south.png" },
    @{ Out = "prj_commit_default.png"; Url = "https://api.pixellab.ai/mcp/map-objects/58aecabf-8b45-411e-a2a1-3ab79635b5a8/download" }
)

foreach ($d in $downloads) {
    $path = Join-Path $sprites $d.Out
    Write-Host "Downloading $($d.Out)..."
    curl.exe --ssl-no-revoke -L -f -o $path $d.Url
}

Write-Host "Done. Open Godot src/ and let imports refresh."
