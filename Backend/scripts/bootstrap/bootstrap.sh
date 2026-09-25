#!/usr/bin/env bash
set -euo pipefail

with_monitoring=true
with_gateway=true
gateway_args=()

for arg in "$@"; do
    case "$arg" in
        --no-monitoring)
            with_monitoring=false
            ;;
        --no-gateway)
            with_gateway=false
            gateway_args+=("$arg")
            ;;
        *)
            echo "Unknown argument: $arg" >&2
            exit 1
            ;;
    esac
done

echo "Stage 0: Kind network"
./network.sh


echo "Stage 1: Registries"

pushd infrastructure/registries
    ./registries.sh
popd


echo "Stage 2: Cluster"

./cluster.sh "${gateway_args[@]}"


echo "Stage 3: Secrets"

./secrets.sh


echo "Stage 4: Databases"

./databases.sh


echo "Stage 5: Monitoring"

if [[ "$with_monitoring" == true ]]; then
    ./monitoring.sh "${gateway_args[@]}"
else
    echo "Skipped"
fi


echo "Stage 6: Gateway"

echo "Stage 6: Gateway"

if [[ "$with_gateway" == true ]]; then
    ./gateway.sh
else
    echo "Skipped"
fi

exit 0
