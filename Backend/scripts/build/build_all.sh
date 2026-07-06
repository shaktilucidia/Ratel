#!/usr/bin/env bash

echo "Building docker images..."

echo "Backend: users"
cd ../../backend/ratel_backend_users
docker build -f dockerfile -t ratel_backend_users .
docker build -f dockerfile-migrate -t ratel_migrate_backend_users .

exit 0
