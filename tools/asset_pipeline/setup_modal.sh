#!/usr/bin/env bash
set -euo pipefail

if ! command -v modal >/dev/null 2>&1; then
  echo "Modal CLI is not installed. Install it first, then run this script." >&2
  exit 1
fi

echo "Creating Modal volumes if they do not already exist..."
modal volume create empire-card-model-cache || true
modal volume create empire-card-assets || true

if [[ -n "${HF_TOKEN:-}" ]]; then
  echo "Creating/updating Modal secret huggingface from environment..."
  modal secret create huggingface HF_TOKEN="${HF_TOKEN}" || true
else
  echo "HF_TOKEN is not set; skipping secret creation."
  echo "Run this later with: HF_TOKEN='<token>' ./tools/asset_pipeline/setup_modal.sh"
fi

echo "Setup complete."
