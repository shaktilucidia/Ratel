#!/usr/bin/env bash
set -euo pipefail

echo "Deploying Prometheus"

pwd
pushd ../k8s/local

    kubectl --context "$RATEL_CONTEXT" apply -f backend/infrastructure/monitoring/prometheus

popd

kubectl --context "$RATEL_CONTEXT" rollout restart deployment/prometheus -n ratel-monitoring
kubectl --context "$RATEL_CONTEXT" rollout status deployment/prometheus -n ratel-monitoring

exit 0
