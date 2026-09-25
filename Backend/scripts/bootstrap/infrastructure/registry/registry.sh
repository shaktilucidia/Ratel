#!/usr/bin/env bash
set -euo pipefail

docker compose -p ratel-cache -f compose.yaml up -d

exit 0
