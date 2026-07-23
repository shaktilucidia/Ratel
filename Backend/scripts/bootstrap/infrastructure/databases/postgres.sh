#!/usr/bin/env bash
set -euo pipefail

echo "Deploying PostgreSQL"

pwd
pushd ../k8s/local

    kubectl --context "$RATEL_CONTEXT" apply -f backend/infrastructure/databases/postgres

popd

kubectl --context "$RATEL_CONTEXT" rollout restart sts/ratel-postgres -n ratel-backend
kubectl --context "$RATEL_CONTEXT" rollout status sts/ratel-postgres -n ratel-backend

exit 0
