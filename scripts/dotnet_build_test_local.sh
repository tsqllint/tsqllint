#!/bin/bash

################################################################################
# a script to run the dotnet build test script locally
################################################################################

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
    mcr.microsoft.com/dotnet/sdk:6.0 \
    /app/scripts/dotnet_build_test.sh
