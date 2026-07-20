#!/usr/bin/env bash
set -euo pipefail

echo "Building..."

pushd ../../backend

    docker build -f docker/users/dockerfile --target runtime -t ratel_backend_users .
    docker build -f docker/users/dockerfile --target migrations -t ratel_migrate_backend_users .

popd


echo "Loading images..."

kind load docker-image ratel_backend_users:latest --name ratel-testing
kind load docker-image ratel_migrate_backend_users:latest --name ratel-testing


echo "Running migrations..."

kubectl delete job ratel-migrate-backend-users -n ratel-backend --ignore-not-found
kubectl apply -f ../k8s/local/backend/microservices/users/migrations


kubectl wait \
    --for=condition=complete \
    job/ratel-migrate-backend-users \
    -n ratel-backend \
    --timeout=120s

echo "Restarting deployment..."

kubectl apply -f ../k8s/local/backend/microservices/users/instance

kubectl rollout restart deployment ratel-backend-users -n ratel-backend
kubectl rollout status deployment ratel-backend-users -n ratel-backend

exit 0
