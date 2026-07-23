#!/usr/bin/env bash
set -euo pipefail

echo "Building..."

pushd ../../backend

    docker build -f docker/users/dockerfile --target runtime -t ratel_backend_users .
    docker build -f docker/users/dockerfile --target migrations -t ratel_migrate_backend_users .

popd


echo "Loading images..."

kind load docker-image ratel_backend_users:latest --name "$RATEL_CLUSTER"
kind load docker-image ratel_migrate_backend_users:latest --name "$RATEL_CLUSTER"


echo "Running migrations..."

kubectl --context "$RATEL_CONTEXT" delete job ratel-migrate-backend-users -n ratel-backend --ignore-not-found
kubectl --context "$RATEL_CONTEXT" apply -f ../k8s/local/backend/microservices/users/migrations


kubectl --context "$RATEL_CONTEXT" wait \
    --for=condition=complete \
    job/ratel-migrate-backend-users \
    -n ratel-backend \
    --timeout=120s

echo "Restarting deployment..."

kubectl --context "$RATEL_CONTEXT" apply -f ../k8s/local/backend/microservices/users/instance

kubectl --context "$RATEL_CONTEXT" rollout restart deployment ratel-backend-users -n ratel-backend
kubectl --context "$RATEL_CONTEXT" rollout status deployment ratel-backend-users -n ratel-backend

exit 0
