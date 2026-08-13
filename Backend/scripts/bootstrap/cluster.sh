#!/usr/bin/env bash
set -euo pipefail

cd ../k8s/local

echo "Stage 0: Deleting old cluster"

kind delete cluster --name "$RATEL_CLUSTER"


echo "Stage 1: Creating cluster"

kind create cluster --config "$RATEL_CONTEXT.yaml"


echo "Creating namespaces"

kubectl --context "$RATEL_CONTEXT" apply -f namespace-backend.yaml
kubectl --context "$RATEL_CONTEXT" apply -f namespace-monitoring.yaml


echo "Stage 2: Installing MetalLB"

kubectl --context "$RATEL_CONTEXT" apply \
  -f https://raw.githubusercontent.com/metallb/metallb/v0.16.1/config/manifests/metallb-native.yaml

kubectl --context "$RATEL_CONTEXT" wait \
  --for=condition=Available \
  deployment/controller \
  -n metallb-system \
  --timeout=300s

kubectl --context "$RATEL_CONTEXT" rollout status \
  daemonset/speaker \
  -n metallb-system \
  --timeout=300s

kubectl --context "$RATEL_CONTEXT" apply \
  -f backend/infrastructure/metallb


echo "Stage 3: Installing Envoy"

kubectl --context "$RATEL_CONTEXT" apply \
  --server-side \
  -f https://github.com/envoyproxy/gateway/releases/download/v1.8.3/install.yaml

kubectl --context "$RATEL_CONTEXT" rollout status \
  deployment/envoy-gateway \
  -n envoy-gateway-system \
  --timeout=300s


exit 0
