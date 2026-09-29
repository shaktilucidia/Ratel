#!/usr/bin/env bash
set -euo pipefail

with_gateway=true

for arg in "$@"; do
    case "$arg" in
        --no-gateway)
            with_gateway=false
            ;;
        *)
            echo "Unknown argument: $arg" >&2
            exit 1
            ;;
    esac
done

echo "Building..."

pushd ../../backend

    docker build -f docker/creatures/dockerfile --target runtime -t ratel_backend_creatures .
    docker build -f docker/creatures/dockerfile --target migrations -t ratel_migrate_backend_creatures .

popd


echo "Loading images..."

kind load docker-image ratel_backend_creatures:latest --name "$RATEL_CLUSTER"
kind load docker-image ratel_migrate_backend_creatures:latest --name "$RATEL_CLUSTER"


echo "Running migrations..."

kubectl --context "$RATEL_CONTEXT" delete job ratel-migrate-backend-creatures -n ratel-backend --ignore-not-found
kubectl --context "$RATEL_CONTEXT" apply -f ../k8s/backend/microservices/creatures/migrations

kubectl --context "$RATEL_CONTEXT" wait \
    --for=condition=complete \
    job/ratel-migrate-backend-creatures \
    -n ratel-backend \
    --timeout=120s


echo "Initializing users and roles..."
kubectl --context "$RATEL_CONTEXT" delete job ratel-init-backend-creatures -n ratel-backend --ignore-not-found
kubectl --context "$RATEL_CONTEXT" apply -f ../k8s/backend/microservices/creatures/init

kubectl --context "$RATEL_CONTEXT" wait \
    --for=condition=complete \
    job/ratel-init-backend-creatures \
    -n ratel-backend \
    --timeout=120s


echo "Restarting deployment..."

kubectl --context "$RATEL_CONTEXT" apply \
    -f ../k8s/backend/microservices/creatures/instance/deployment.yaml \
    -f ../k8s/backend/microservices/creatures/instance/service.yaml

if [[ "$with_gateway" == true ]]; then
    kubectl --context "$RATEL_CONTEXT" apply \
        -f ../k8s/backend/microservices/creatures/instance/http-route.yaml
fi

kubectl --context "$RATEL_CONTEXT" rollout restart deployment ratel-backend-creatures -n ratel-backend
kubectl --context "$RATEL_CONTEXT" rollout status deployment ratel-backend-creatures -n ratel-backend

exit 0
