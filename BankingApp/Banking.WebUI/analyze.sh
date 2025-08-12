#!/bin/sh

# Parse command line arguments
MODE="syncoverasync"  # Default mode
FILTER="System.Runtime.Serialization.SerializationException"

# Parse arguments
while [ $# -gt 0 ]; do
    case $1 in
        syncoverasync)
            MODE="syncoverasync"
            shift
            ;;
        exception)
            MODE="exception"
            shift
            ;;
        *)
            echo "Error: Unknown parameter $1"
            echo "Usage: $0 [syncoverasync] | [exception <filter>]"
            exit 1
            ;;
    esac
done

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

echo "Running in $MODE mode"

if [ "$MODE" = "syncoverasync" ]; then
    # Install dotnet-dump tool
    "$DOTNET_ROOT/dotnet" tool install --tool-path "$BASE_DIR" dotnet-dump

    echo ">>COMPLETED DOWNLOAD<<"

    # Collect memory dump of PID 1
    "$BASE_DIR/dotnet-dump" collect -p 1 -o "$BASE_DIR/dump"

    echo ">>STARTED ANALYSIS<<"

    # Run analysis
    "/diag/cli/DiagnosticAnalysis" analyze "$BASE_DIR/dump"
    echo ">>COMPLETED ANALYSIS<<"

elif [ "$MODE" = "exception" ]; then

    echo ">>COMPLETED DOWNLOAD<<"

    # Use procdump with the specified filter
    echo ">>STARTED ANALYSIS<<"
    
    # Install procdump if not available (assuming it's available in the container)
    # Note: You may need to adjust this based on your container setup
    procdump -n 1 -e -f "$FILTER" -o 1 "$BASE_DIR/exception_dump"
   
        # Run analysis
    "/diag/cli/DiagnosticAnalysis" analyze "$BASE_DIR/exception_dump_0"
    echo ">>COMPLETED ANALYSIS<<"
fi
