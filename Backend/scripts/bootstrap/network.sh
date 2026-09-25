#!/usr/bin/env bash
set -euo pipefail

docker network inspect kind > /dev/null 2>&1 || docker network create --driver bridge --ipv6 --subnet fd62:7261:7465:6c00::/64 kind

exit 0
