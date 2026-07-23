#!/usr/bin/env bash
set -euo pipefail

echo "Deploying Grafana"

pwd
pushd ../k8s/local

    kubectl --context "$RATEL_CONTEXT"  apply -f backend/infrastructure/monitoring/grafana

popd

kubectl --context "$RATEL_CONTEXT" rollout restart deployment/grafana -n ratel-monitoring
kubectl --context "$RATEL_CONTEXT" rollout status deployment/grafana -n ratel-monitoring

exit 0
