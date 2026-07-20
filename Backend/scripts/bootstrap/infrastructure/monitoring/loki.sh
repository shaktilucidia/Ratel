#!/usr/bin/env bash
set -euo pipefail

echo "Deploying Loki"

pwd
pushd ../k8s/local

    kubectl apply -f backend/infrastructure/monitoring/loki

popd

kubectl rollout restart deployment/loki -n ratel-monitoring
kubectl rollout status deployment/loki -n ratel-monitoring

exit 0
