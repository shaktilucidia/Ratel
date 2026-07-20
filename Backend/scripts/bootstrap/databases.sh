#!/usr/bin/env bash
set -euo pipefail

echo "PostgreSQL"

./infrastructure/databases/postgres.sh

exit 0
