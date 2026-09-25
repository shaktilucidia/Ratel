#!/usr/bin/env bash
set -euo pipefail

echo "Deploying Grafana"

pwd
pushd ../k8s

for manifest in backend/infrastructure/monitoring/grafana/*.yaml; do
    if [[ "$with_gateway" == false \
        && "${manifest##*/}" == "http-route.yaml" ]]; then
        continue
    fi

    kubectl --context "$RATEL_CONTEXT" apply -f "$manifest"
done

popd

kubectl --context "$RATEL_CONTEXT" rollout restart deployment/grafana -n ratel-monitoring
kubectl --context "$RATEL_CONTEXT" rollout status deployment/grafana -n ratel-monitoring

exit 0
