#!/usr/bin/env bash
set -euo pipefail

echo "Deploying monitoring"

./infrastructure/monitoring/grafana.sh
./infrastructure/monitoring/loki.sh
./infrastructure/monitoring/fluentbit.sh
./infrastructure/monitoring/tempo.sh
./infrastructure/monitoring/prometheus.sh

exit 0
