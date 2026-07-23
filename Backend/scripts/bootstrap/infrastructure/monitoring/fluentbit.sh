#!/usr/bin/env bash
set -euo pipefail

echo "Deploying FluentBit"

pwd
pushd ../k8s/local

    kubectl --context "$RATEL_CONTEXT" apply -f backend/infrastructure/monitoring/fluentbit

popd

kubectl --context "$RATEL_CONTEXT" rollout restart daemonset/fluent-bit -n ratel-monitoring
kubectl --context "$RATEL_CONTEXT" rollout status daemonset/fluent-bit -n ratel-monitoring

exit 0
