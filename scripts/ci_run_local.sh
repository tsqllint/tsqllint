#!/bin/bash

# enable for bash debugging
#set -x

# fail script if a cmd fails
set -e

# fail script if piped command fails
set -o pipefail

docker run \
    -v "$(pwd)":/app \
    -v "$(pwd)/artifacts":/artifacts \
    -v "$(pwd)/packages":/packages \
    --rm \
    mcr.microsoft.com/dotnet/sdk:5.0 \
    /app/scripts/ci_run.sh
