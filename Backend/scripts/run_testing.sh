#!/usr/bin/env bash

echo "Stage 0: Building images"

cd build
build_all.sh

cd ../k8s/local

echo "Stage 1: Deleting old cluster"

kind delete cluster --name ratel-testing


echo "Stage 2: Creating cluster"

kind create cluster --config kind-config.yaml


echo "Stage 3: Loading images"

kind load docker-image ratel_backend_users:latest --name ratel-testing


echo "Stage 4: Applying deployment"

kubectl apply -f deployment-backend-users.yaml

exit 0
