#!/usr/bin/env bash
set -euo pipefail

pushd tests/e2e

    ./run.sh

popd

exit 0
