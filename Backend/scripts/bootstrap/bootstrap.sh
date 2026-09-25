#!/usr/bin/env bash
set -euo pipefail

with_monitoring=true

for arg in "$@"; do
    case "$arg" in
        --no-monitoring)
            with_monitoring=false
            ;;
        *)
            echo "Unknown argument: $arg" >&2
            exit 1
            ;;
    esac
done

echo "Stage 0: Kind network"
./network.sh


echo "Stage 1: Registry"

pushd infrastructure/registry
    ./registry.sh
popd


echo "Stage 2: Cluster"

./cluster.sh


echo "Stage 3: Secrets"

./secrets.sh


echo "Stage 4: Databases"

./databases.sh


echo "Stage 5: Monitoring"

if [[ "$with_monitoring" == true ]]; then
    ./monitoring.sh
else
    echo "Skipped"
fi


echo "Stage 6: Gateway"

./gateway.sh

exit 0
