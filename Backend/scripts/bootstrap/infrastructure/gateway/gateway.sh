#!/usr/bin/env bash
set -euo pipefail

echo "Deploying gateway"

pwd
pushd ../k8s

    kubectl --context "$RATEL_CONTEXT" apply -f backend/infrastructure/gateway

popd

kubectl --context "$RATEL_CONTEXT" wait \
  --for=condition=Programmed \
  gateway/ratel-gateway \
  -n ratel-backend \
  --timeout=300s

exit 0
