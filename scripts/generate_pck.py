"""Generate a valid Godot PCK v2 file (compatible with Godot 4.5.1).

Usage: python3 generate_pck.py <output_path>
"""

import struct
import sys


def generate_pck(output_path: str) -> None:
    with open(output_path, "wb") as f:
        f.write(b"GDPC")                      # Magic
        f.write(struct.pack("<I", 2))          # Pack format version
        f.write(struct.pack("<I", 4))          # Godot major version
        f.write(struct.pack("<I", 5))          # Godot minor version
        f.write(struct.pack("<I", 1))          # Godot patch version
        f.write(struct.pack("<I", 0))          # Flags
        f.write(struct.pack("<Q", 0))          # File base offset
        f.write(b"\x00" * 64)                  # Reserved
        f.write(struct.pack("<I", 0))          # File count


if __name__ == "__main__":
    if len(sys.argv) != 2:
        print(f"Usage: {sys.argv[0]} <output_path>", file=sys.stderr)
        sys.exit(1)
    generate_pck(sys.argv[1])
