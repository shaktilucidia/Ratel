#!/usr/bin/env bash
set -euo pipefail

cd ../k8s/local

echo "Deleting old cluster"

kind delete cluster --name "$RATEL_CLUSTER"


echo "Stage 1: Creating cluster"

kind create cluster --config "$RATEL_CONTEXT.yaml"


echo "Creating namespaces"

kubectl --context "$RATEL_CONTEXT" apply -f namespace-backend.yaml
kubectl --context "$RATEL_CONTEXT" apply -f namespace-monitoring.yaml


exit 0
