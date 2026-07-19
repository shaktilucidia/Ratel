#!/usr/bin/env bash
set -euo pipefail

echo "Stage 0: Building testing images"

cd build/
./build_e2e.sh
cd ../

echo "Stage 1: Loading testing images"

kind load docker-image ratel_backend_users_e2e:latest --name ratel-testing

echo "Stage 3: Waiting for cluster spin-up"

kubectl rollout status deployment/ratel-backend-users -n ratel-backend --timeout=300s

# Waiting for backend
kubectl wait \
  --for=condition=Ready \
  pod \
  -l app=ratel-backend-users -n ratel-backend

echo "Stage 4: Running tests"
cd k8s/local/backend/microservices

kubectl apply -f users/tests

kubectl wait \
  --for=condition=complete \
  job/ratel-backend-users-e2e -n ratel-backend \
  --timeout=300s

kubectl logs job/ratel-backend-users-e2e -n ratel-backend

kubectl delete job ratel-backend-users-e2e -n ratel-backend

exit 0
