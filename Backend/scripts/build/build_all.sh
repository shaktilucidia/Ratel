#!/usr/bin/env bash

echo "Building docker images..."

echo "Backend: users"
cd ../../backend/ratel_backend_users/ratel_backend_users
docker build -t ratel_backend_users .

exit 0
