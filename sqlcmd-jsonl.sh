#!/usr/bin/env bash
set -Eeuo pipefail
SCRIPT_FILE="${BASH_SOURCE[0]}"

dotnet run --file "${SCRIPT_FILE%.sh}.cs" -- "$@"
