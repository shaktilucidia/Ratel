#!/usr/bin/env bash
set -euo pipefail


echo "Users E2E - Stage 0: Building testing images"

pushd ../../../backend

    docker build -f docker/users/dockerfile-e2e -t ratel_backend_users_e2e .

popd


echo "Users E2E - Stage 1: Loading testing images"

kind load docker-image ratel_backend_users_e2e:latest --name "$RATEL_CLUSTER"


echo "Users E2E - Stage 2: Waiting for backend"

kubectl --context "$RATEL_CONTEXT" wait \
  --for=condition=Ready \
  pod \
  -n ratel-backend \
  -l app=ratel-backend-users


echo "Users E2E - Stage 3: Running tests"

kubectl --context "$RATEL_CONTEXT" apply -f ../../k8s/local/backend/microservices/users/tests/e2e

kubectl --context "$RATEL_CONTEXT" wait \
  --for=condition=complete \
  job/ratel-backend-users-e2e \
  -n ratel-backend \
  --timeout=300s

kubectl --context "$RATEL_CONTEXT" logs job/ratel-backend-users-e2e -n ratel-backend


echo "Users E2E - Stage 4: Cleanup"

kubectl --context "$RATEL_CONTEXT" delete job ratel-backend-users-e2e -n ratel-backend


exit 0
