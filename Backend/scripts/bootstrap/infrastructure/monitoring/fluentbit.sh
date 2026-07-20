#!/usr/bin/env bash
set -euo pipefail

echo "Deploying FluentBit"

pwd
pushd ../k8s/local

    kubectl apply -f backend/infrastructure/monitoring/fluentbit

popd

kubectl rollout restart daemonset/fluent-bit -n ratel-monitoring
kubectl rollout status daemonset/fluent-bit -n ratel-monitoring

exit 0
