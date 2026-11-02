# sqlcmd-jsonl

Execute SQL queries, yielding JSON Lines.

## Installation

Download a [release](../../releases) (single file executable binary).

Alternatively, run from `.cs` source (requires .NET 10):

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
