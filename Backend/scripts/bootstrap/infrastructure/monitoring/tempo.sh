#!/usr/bin/env bash
set -euo pipefail

echo "Deploying Tempo"

pwd
pushd ../k8s/local

    kubectl --context "$RATEL_CONTEXT" apply -f backend/infrastructure/monitoring/tempo

popd

kubectl --context "$RATEL_CONTEXT" rollout restart deployment/tempo -n ratel-monitoring
kubectl --context "$RATEL_CONTEXT" rollout status deployment/tempo -n ratel-monitoring

exit 0
