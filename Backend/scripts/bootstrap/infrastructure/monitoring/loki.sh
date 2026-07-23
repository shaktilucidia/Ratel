#!/usr/bin/env bash
set -euo pipefail

echo "Deploying Loki"

pwd
pushd ../k8s/local

    kubectl --context "$RATEL_CONTEXT" apply -f backend/infrastructure/monitoring/loki

popd

kubectl --context "$RATEL_CONTEXT" rollout restart deployment/loki -n ratel-monitoring
kubectl --context "$RATEL_CONTEXT" rollout status deployment/loki -n ratel-monitoring

exit 0
