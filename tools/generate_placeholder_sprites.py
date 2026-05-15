#!/usr/bin/env python3
"""Generate flat placeholder PNGs for GameHoldOn combat visuals (MVP)."""

from pathlib import Path

try:
    from PIL import Image, ImageDraw
except ImportError:
    raise SystemExit("Install Pillow: pip install pillow")

ROOT = Path(__file__).resolve().parents[1] / "src" / "assets" / "art"
OUTLINE = (26, 26, 34, 255)


def hex_rgb(h: str) -> tuple[int, int, int]:
    h = h.lstrip("#")
    return tuple(int(h[i : i + 2], 16) for i in (0, 2, 4))


def draw_outline_ellipse(d: ImageDraw.ImageDraw, xy, fill, width=2):
    d.ellipse(xy, fill=fill)
    d.ellipse(xy, outline=OUTLINE, width=width)


def player_dev(path: Path) -> None:
    img = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    g = hex_rgb("#5AD38E")
    gd = hex_rgb("#3FA86C")
    # shadow
    d.ellipse((20, 52, 44, 60), fill=(0, 0, 0, 76))
    # body
    d.rounded_rectangle((22, 30, 42, 52), radius=6, fill=gd, outline=OUTLINE, width=2)
    # hood head
    d.ellipse((18, 10, 46, 38), fill=g, outline=OUTLINE, width=2)
    # headphones band
    d.arc((16, 14, 48, 36), 200, 340, fill=OUTLINE, width=2)
    # mug
    d.rectangle((40, 40, 48, 50), fill=hex_rgb("#E8ECF0"), outline=OUTLINE, width=1)
    img.save(path)


def enemy_hr(path: Path) -> None:
    img = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    red = hex_rgb("#C94C4C")
    d.ellipse((20, 52, 44, 60), fill=(0, 0, 0, 76))
    d.polygon([(14, 34), (50, 34), (46, 54), (18, 54)], fill=red, outline=OUTLINE)
    d.ellipse((20, 12, 44, 36), fill=hex_rgb("#E8A0A0"), outline=OUTLINE, width=2)
    d.rectangle((8, 28, 18, 44), fill=hex_rgb("#F5F5F5"), outline=OUTLINE, width=2)
    img.save(path)


def enemy_ceo(path: Path) -> None:
    img = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    purple = hex_rgb("#7B5EBF")
    d.ellipse((20, 52, 44, 60), fill=(0, 0, 0, 76))
    d.polygon([(10, 32), (54, 32), (50, 54), (14, 54)], fill=purple, outline=OUTLINE)
    d.ellipse((20, 10, 44, 34), fill=hex_rgb("#B8A8E8"), outline=OUTLINE, width=2)
    d.polygon([(30, 22), (34, 22), (32, 36)], fill=hex_rgb("#FFD700"), outline=OUTLINE)
    img.save(path)


def enemy_cto(path: Path) -> None:
    img = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    orange = hex_rgb("#E07A2F")
    d.ellipse((20, 52, 44, 60), fill=(0, 0, 0, 76))
    d.rounded_rectangle((18, 30, 46, 54), radius=4, fill=orange, outline=OUTLINE, width=2)
    d.ellipse((20, 12, 44, 34), fill=hex_rgb("#F0C090"), outline=OUTLINE, width=2)
    d.ellipse((24, 20, 30, 26), outline=OUTLINE, width=2)
    d.ellipse((34, 20, 40, 26), outline=OUTLINE, width=2)
    d.rectangle((42, 36, 52, 48), fill=hex_rgb("#505868"), outline=OUTLINE, width=1)
    d.point((47, 42), fill=hex_rgb("#5ADCF0"))
    img.save(path)


def projectile(path: Path) -> None:
    img = Image.new("RGBA", (16, 16), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    y = hex_rgb("#FFE566")
    d.rounded_rectangle((2, 4, 14, 12), radius=2, fill=y, outline=OUTLINE, width=1)
    d.text((4, 3), "{}", fill=OUTLINE)
    img.save(path)


def floor_tile(path: Path) -> None:
    img = Image.new("RGBA", (128, 128), hex_rgb("#3D4A5C") + (255,))
    d = ImageDraw.Draw(img)
    light = hex_rgb("#465563")
    for y in range(0, 128, 64):
        for x in range(0, 128, 64):
            if (x // 64 + y // 64) % 2 == 0:
                d.rectangle((x, y, x + 63, y + 63), fill=light)
    for i in range(0, 129, 32):
        d.line([(i, 0), (i, 127)], fill=hex_rgb("#2E3848"), width=1)
        d.line([(0, i), (127, i)], fill=hex_rgb("#2E3848"), width=1)
    img.save(path)


def main() -> None:
    sprites = ROOT / "sprites"
    tiles = ROOT / "tiles"
    sprites.mkdir(parents=True, exist_ok=True)
    tiles.mkdir(parents=True, exist_ok=True)
    player_dev(sprites / "chr_player_dev.png")
    enemy_hr(sprites / "chr_enemy_hr.png")
    enemy_ceo(sprites / "chr_enemy_ceo.png")
    enemy_cto(sprites / "chr_enemy_cto.png")
    projectile(sprites / "prj_commit_default.png")
    floor_tile(tiles / "env_floor_office.png")
    print("Wrote 6 placeholder PNGs under src/assets/art/")


if __name__ == "__main__":
    main()
