#!/usr/bin/env bash
set -euo pipefail

echo "Deploying Grafana"

pwd
pushd ../k8s/local

    kubectl apply -f backend/infrastructure/monitoring/grafana

popd

kubectl rollout restart deployment/grafana -n ratel-monitoring
kubectl rollout status deployment/grafana -n ratel-monitoring

exit 0
