#!/usr/bin/env bash
set -euo pipefail

source "$(dirname "$0")/config_env.sh" "ratel-e2e"

./bootstrap.sh "ratel-e2e"

pushd tests/e2e

    ./run.sh

popd


kind delete cluster --name "$RATEL_CLUSTER"

exit 0
