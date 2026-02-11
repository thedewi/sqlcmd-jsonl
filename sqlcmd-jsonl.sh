#!/usr/bin/env bash
set -Eeuo pipefail
SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)

dotnet run --file "$SCRIPT_DIR/sqlcmd-jsonl.cs" -- "$@"
