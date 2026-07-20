#!/usr/bin/env bash
set -euo pipefail

cd ../k8s/local

echo "Deleting old cluster"

kind delete cluster --name ratel-testing


echo "Stage 1: Creating cluster"

kind create cluster --config kind-config.yaml


echo "Creating namespaces"

kubectl apply -f namespace-backend.yaml
kubectl apply -f namespace-monitoring.yaml


exit 0
