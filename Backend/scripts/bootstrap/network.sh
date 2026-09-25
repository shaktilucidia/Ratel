#!/usr/bin/env bash
set -euo pipefail

docker network inspect kind > /dev/null 2>&1 || docker network create --driver bridge --ipv6 --subnet 172.18.0.0/16 --subnet fc00:f853:ccd:e793::/64 kind

exit 0
