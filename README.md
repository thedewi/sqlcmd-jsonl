# sqlcmd-jsonl

Execute SQL queries, yielding JSON Lines

## Installation

A self-contained single-file binary is [available](../../releases).

Manage installation via mise in **Command Prompt**:

```cmd
winget install --interactive --exact --id Microsoft.Pave
winget install --interactive --exact --id jdx.mise
pave add "%LOCALAPPDATA%/mise/shims"

mise use --global github:thedewi/sqlcmd-jsonl
```

## Usage

```cmd
sqlcmd-jsonl --query "select 1 as one"
```

## Run from single source file

Also runs from a single .cs file in .NET 10+:

```bash
# Download
curl --silent --show-error --globoff --fail \
    https://raw.githubusercontent.com/thedewi/sqlcmd-jsonl/refs/heads/main/sqlcmd-jsonl.cs \
    >sqlcmd-jsonl.cs
curl --silent --show-error --globoff --fail \
    https://raw.githubusercontent.com/thedewi/sqlcmd-jsonl/refs/heads/main/sqlcmd-jsonl.sh \
    >sqlcmd-jsonl.sh
chmod a+x sqlcmd-jsonl.sh

# Run
./sqlcmd-jsonl.sh --query 'select 1 as one'
```
