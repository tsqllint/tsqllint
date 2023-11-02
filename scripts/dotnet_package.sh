#!/bin/bash

################################################################################
# a script to package, publish and push nuget packages
################################################################################

# enable for bash debugging
#set -x

# fail script if a cmd fails
set -e

# fail script if piped command fails
set -o pipefail

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_ROOT"

source "$SCRIPT_DIR/utils.sh"

if [ "$RELEASE" == "false" ]; then
    info "not a release build exiting now"
    exit 0
else
    info "starting release"
fi

info "packing project"

dotnet pack \
    "$PROJECT_ROOT/source/TSQLLint.sln" \
    -p:VERSION="$VERSION" \
    --configuration Release \
    --output "$ARTIFACTS_DIR"

info "build and archive assemblies"

PLATFORMS=("win-x86" "win-x64" "win-arm64" "osx-x64" "osx-arm64" "linux-x64" "linux-musl-x64" "linux-musl-arm64" "linux-arm" "linux-arm64")
for PLATFORM in "${PLATFORMS[@]}"
do
    info "building assemblies for platform $PLATFORM"

    OUT_DIR="$ARTIFACTS_DIR/$PLATFORM"
    mkdir -p "$OUT_DIR"

    info "creating assemblies directory $OUT_DIR"

    dotnet publish \
        "$PROJECT_ROOT/source/TSQLLint/TSQLLint.csproj" \
        -c Release \
        -r "$PLATFORM" \
        /p:Version="$VERSION" \
        -o "$OUT_DIR"

    info "archiving assemblies for platform $PLATFORM"

    # change directory to reduce directory depth in archive file
    info "changing directory to $ARTIFACTS_DIR"
    cd "$ARTIFACTS_DIR"

    tar -zcf "$ARTIFACTS_DIR/$PLATFORM.tgz" "$PLATFORM"
    rm -rf "$PLATFORM"

    info "changing directory to $PROJECT_ROOT"
    cd "$PROJECT_ROOT"
done

[ -n "$NUGET_API_KEY" ] || { error "NUGET_API_KEY is required and not set, aborting"; }

info "pushing to Nuget"

dotnet nuget push \
    "$ARTIFACTS_DIR/TSQLLint.$VERSION.nupkg" \
    --api-key "$NUGET_API_KEY"  \
    --source https://api.nuget.org/v3/index.json

info "done"

exit 0
