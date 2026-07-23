#!/usr/bin/env bash

export RATEL_CLUSTER="${1:-ratel-dev}" # Default cluster name is ratel-dev
export RATEL_CONTEXT="kind-$RATEL_CLUSTER"
