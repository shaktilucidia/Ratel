#!/usr/bin/env bash

echo "Stage 0: Building images"

cd build
./build_all.sh

cd ../k8s/local

echo "Stage 1: Deleting old cluster"

kind delete cluster --name ratel-testing


echo "Stage 2: Creating cluster"

kind create cluster --config kind-config.yaml
kubectl apply -f namespace-backend.yaml
kubectl apply -f namespace-monitoring.yaml


echo "Stage 3: Loading images"

kind load docker-image ratel_backend_users:latest --name ratel-testing
kind load docker-image ratel_migrate_backend_users:latest --name ratel-testing


echo "Stage 4: Applying infrastructure deployment"

kubectl apply -f backend/infrastructure/database/postgres
kubectl apply -f backend/infrastructure/monitoring/grafana


echo "Stage 5: Applying migrations deployment"

kubectl apply -f backend/microservices/users/migrations


echo "Stage 6: Applying backend deployment"

kubectl apply -f backend/microservices/users/instance

exit 0
