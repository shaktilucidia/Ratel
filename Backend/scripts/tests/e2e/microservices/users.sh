#!/usr/bin/env bash
set -euo pipefail


echo "Users E2E - Stage 0: Building testing images"

pushd ../../../backend

    docker build -f docker/users/dockerfile-e2e -t ratel_backend_users_e2e .

popd


echo "Users E2E - Stage 1: Loading testing images"

kind load docker-image ratel_backend_users_e2e:latest --name ratel-testing


echo "Users E2E - Stage 2: Running tests"

kubectl apply -f ../../k8s/local/backend/microservices/users/tests/e2e

kubectl wait \
  --for=condition=complete \
  job/ratel-backend-users-e2e -n ratel-backend \
  --timeout=300s

kubectl logs job/ratel-backend-users-e2e -n ratel-backend


echo "Users E2E - Stage 3: Cleanup"

kubectl delete job ratel-backend-users-e2e -n ratel-backend


exit 0
