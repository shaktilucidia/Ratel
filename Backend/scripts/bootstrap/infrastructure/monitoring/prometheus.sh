#!/usr/bin/env bash
set -euo pipefail

echo "Deploying Prometheus"

pwd
pushd ../k8s/local

    kubectl apply -f backend/infrastructure/monitoring/prometheus

popd

kubectl rollout restart deployment/prometheus -n ratel-monitoring
kubectl rollout status deployment/prometheus -n ratel-monitoring

exit 0
