#!/usr/bin/env bash
set -euo pipefail

cd ../k8s/local

echo "Loading secrets"

kubectl apply -f backend/secrets

exit 0
