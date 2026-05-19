# Re-download all GameHoldOn character walk animations from PixelLab.
$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

$pairs = @(
    @{ Id = "5f750a95-08dc-4d18-bf64-90557f6a14b2"; Slug = "chr_player_dev" },
    @{ Id = "cd3fa112-b0bd-4a45-a703-e27f7f3226fe"; Slug = "chr_enemy_hr" },
    @{ Id = "0d6bf6ab-f4d6-4192-b81c-95052f0e2e1a"; Slug = "chr_enemy_ceo" },
    @{ Id = "103eb4fb-4415-4c70-85f4-066fd9313d62"; Slug = "chr_enemy_cto" }
)

foreach ($p in $pairs) {
    & (Join-Path $scriptDir "download_pixellab_animations.ps1") -CharacterId $p.Id -Slug $p.Slug
}

Write-Host "All walk animations downloaded. Open Godot src/ to import."
