#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "Launching OpenMathLibrary Vulkan demo..."
cargo demo

read -p "Press enter to exit..."

