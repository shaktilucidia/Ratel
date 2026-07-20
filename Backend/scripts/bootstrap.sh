#!/usr/bin/env bash
set -euo pipefail

pushd bootstrap
    ./bootstrap.sh
popd

pushd deploy
    ./deploy.sh
popd

exit 0
