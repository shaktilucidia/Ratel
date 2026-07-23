#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/config_env.sh" "$@"

pushd bootstrap
    ./bootstrap.sh
popd

pushd deploy
    ./deploy.sh
popd

exit 0
