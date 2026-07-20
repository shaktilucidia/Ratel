#!/usr/bin/env bash
set -euo pipefail

echo "Deploying Tempo"

pwd
pushd ../k8s/local

    kubectl apply -f backend/infrastructure/monitoring/tempo

popd

kubectl rollout restart deployment/tempo -n ratel-monitoring
kubectl rollout status deployment/tempo -n ratel-monitoring

exit 0
