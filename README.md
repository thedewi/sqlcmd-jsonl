# sqlcmd-jsonl

Execute SQL queries, yielding JSONLines results.

## Installation

Currently distributed a file-based `.cs` app. Requires .NET 10.

In bash:

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
