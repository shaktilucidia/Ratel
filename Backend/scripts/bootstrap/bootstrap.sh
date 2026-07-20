#!/usr/bin/env bash
set -euo pipefail

echo "Stage 0: Cluster"

./cluster.sh


echo "Stage 1: Secrets"

./secrets.sh


echo "Stage 2: Databases"

./databases.sh


echo "Stage 3: Monitoring"

./monitoring.sh

exit 0
