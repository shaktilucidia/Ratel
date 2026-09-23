#!/usr/bin/env bash
set -euo pipefail

cluster="ratel-dev"
bootstrap_args=()

for arg in "$@"; do
    case "$arg" in
        --no-monitoring)
            bootstrap_args+=("$arg")
            ;;
        -*)
            echo "Unknown option: $arg" >&2
            exit 1
            ;;
        *)
            cluster="$arg"
            ;;
    esac
done

source "$(dirname "$0")/config_env.sh" "$cluster"

pushd bootstrap
    ./bootstrap.sh "${bootstrap_args[@]}"
popd

pushd deploy
    ./deploy.sh
popd

exit 0
