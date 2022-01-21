#!/bin/bash

# fail script if a cmd fails
set -e

# fail script if piped command fails
set -o pipefail

RED='\033[0;31m'
NC='\033[0m'

error () {
    MESSAGE=$1
    printf "${RED}ERROR:${NC} $MESSAGE\n"
    exit 1
}

if [ ! -f /.dockerenv ]; then
    error "This script must be run from within a docker container. For local development use the ci_run_local.sh script.";
fi

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
source "$SCRIPT_DIR/setup.sh"

info "restoring project"

dotnet restore \
    ./source/TSQLLint.sln \
    --verbosity m

info "building project"

dotnet build \
    ./source/TSQLLint.sln \
    /p:Version="$VERSION" \
    --configuration Release \
    --no-restore

info "running test project"

dotnet test \
    --no-restore \
    ./source/TSQLLint.sln

info "packing project"

dotnet pack \
    ./source/TSQLLint.sln \
    -p:VERSION="$VERSION" \
    --configuration Release \
    --output /artifacts

info "build and archive assemblies"

ASSEMBLIES_DIR="/artifacts/assemblies"
PLATFORMS=( "win-x86" "win-x64" "osx-x64" "linux-x64")
for PLATFORM in "${PLATFORMS[@]}"
do
    info "building assemblies for platform $PLATFORM"

    OUT_DIR="$ASSEMBLIES_DIR/$PLATFORM"
    mkdir -p "$OUT_DIR"

    dotnet publish \
        "./source/TSQLLint/TSQLLint.csproj" \
        -c Release \
        -r "$PLATFORM" \
        /p:Version="$VERSION" \
        -o "$OUT_DIR"

    info "archiving assemblies for platform $PLATFORM"

    cd "$ASSEMBLIES_DIR"
    tar -zcvf "/artifacts/$PLATFORM.tgz" "$PLATFORM"

    cd "/app"
done

if [ "$RELEASE" == "false" ]; then
    info "done"
    exit 0
fi

info "pushing to Nuget"

if [[ -z "$NUGET_API_KEY" ]]; then
  error "NUGET_API_KEY is not set in the environment. Artifacts will not be pushed to Nuget."
fi

dotnet nuget push \
    "/artifacts/TSQLLint.$VERSION.nupkg" \
    --api-key "$NUGET_API_KEY"  \
    --source https://api.nuget.org/v3/index.json

info "creating github release"

gh auth login  --hostname "github.com" --with-token < "$GITHUB_TOKEN_FILE"

gh release create "$VERSION" -d /artifacts/*.tgz

info "done"

exit 0
