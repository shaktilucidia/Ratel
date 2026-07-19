#!/usr/bin/env bash
set -euo pipefail

echo "Building docker images for E2E tests..."

echo "Backend: users"
cd ../../backend
docker build -f docker/users/dockerfile-e2e -t ratel_backend_users_e2e .

exit 0
