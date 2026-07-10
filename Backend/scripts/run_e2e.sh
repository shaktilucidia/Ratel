#!/usr/bin/env bash

echo "Stage 0: Spinning up testing cluster"

./run_testing.sh


echo "Stage 1: Building testing images"

cd build
./build_e2e.sh

cd ../k8s/local

echo "Stage 2: Loading testing images"

kind load docker-image ratel_backend_users_e2e:latest --name ratel-testing

echo "Stage 3: Waiting for cluster spin-up"

kubectl rollout status deployment/ratel-backend-users --timeout=300s

sleep 60 # TODO: Dirty, will be fixed when readiness endpoint will be implemented

echo "Stage 4: Running tests"

kubectl apply -f e2e-backend-users.yaml

kubectl wait \
  --for=condition=complete \
  job/ratel-backend-users-e2e \
  --timeout=300s

kubectl logs job/ratel-backend-users-e2e

echo "Stage 5: Cleanup"

kubectl delete job ratel-backend-users-e2e
kind delete cluster --name ratel-testing

exit 0
