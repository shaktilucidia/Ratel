#!/usr/bin/env bash
set -euo pipefail

docker compose -p ratel-dockerhub-cache -f dockerhub/compose.yaml up -d

docker compose -p ratel-quay-cache -f quay/compose.yaml up -d

exit 0
