#!/usr/bin/env bash
set -euo pipefail

echo "Building docker images..."

echo "Backend: users"
cd ../../backend
docker build -f docker/users/dockerfile --target runtime -t ratel_backend_users .
docker build -f docker/users/dockerfile --target migrations -t ratel_migrate_backend_users .

exit 0
