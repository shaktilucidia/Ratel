#!/usr/bin/env bash

echo "Building docker images for E2E tests..."

echo "Backend: users"
cd ../../backend/ratel_backend_users
docker build -f dockerfile-e2e -t ratel_backend_users_e2e .

exit 0
