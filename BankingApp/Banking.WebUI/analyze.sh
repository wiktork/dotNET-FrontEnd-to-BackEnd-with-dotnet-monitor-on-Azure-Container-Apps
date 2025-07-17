#!/bin/sh

# Choose writable base directory
if [ -w "/app" ]; then
    BASE_DIR="/app/diag-tools"
else
    echo "No write permission on /app, using /tmp instead"
    BASE_DIR="/tmp/diag-tools"
fi

mkdir -p "$BASE_DIR"

# Install .NET CLI locally
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --install-dir "$BASE_DIR/.dotnet"

export DOTNET_ROOT="$BASE_DIR/.dotnet"
export PATH="$DOTNET_ROOT:$BASE_DIR:$PATH"

# Install dotnet-dump tool
"$DOTNET_ROOT/dotnet" tool install --tool-path "$BASE_DIR" dotnet-dump

echo ">>COMPLETED DOWNLOAD<<"

# Collect memory dump of PID 1
"$BASE_DIR/dotnet-dump" collect -p 1 -o "$BASE_DIR/dump"

echo ">>STARTED ANALYSIS<<"

# Run analysis
"/diag/cli/DiagnosticAnalysis" analyze "$BASE_DIR/dump"
echo ">>COMPLETED ANALYSIS<<"
