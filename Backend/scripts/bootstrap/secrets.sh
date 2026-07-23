#!/usr/bin/env bash
set -euo pipefail

cd ../k8s/local

echo "Loading secrets"

kubectl --context "$RATEL_CONTEXT" apply -f backend/secrets

exit 0
