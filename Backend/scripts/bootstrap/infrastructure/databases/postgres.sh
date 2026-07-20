#!/usr/bin/env bash
set -euo pipefail

echo "Deploying PostgreSQL"

pwd
pushd ../k8s/local

    kubectl apply -f backend/infrastructure/databases/postgres

popd

kubectl rollout restart sts/ratel-postgres -n ratel-backend
kubectl rollout status sts/ratel-postgres -n ratel-backend

exit 0
