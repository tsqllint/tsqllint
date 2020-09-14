#!/bin/bash

set -e

WORKING_DIRECTORY=$(pwd)
SCRIPTDIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
BUILDSCRIPTDIR="$(dirname "$SCRIPTDIR")"
REPODIR="$(dirname "$BUILDSCRIPTDIR")"
ASSEMBLIESDIR=$REPODIR/artifacts/assemblies
mkdir -p $ASSEMBLIESDIR

VERSION="0.0.0.1"
CHECKOUT=$(git symbolic-ref -q HEAD || git name-rev --name-only --no-undefined --always HEAD)
if grep -q '^tags/' <<< "$CHECKOUT"; then
    VERSION=$(echo "$CHECKOUT" | grep -oE "[0-9]+[.][0-9]+[.][0-9]+")
fi

PLATFORMS=( "win-x86" "win-x64" "osx-x64" "linux-x64")
for PLATFORM in "${PLATFORMS[@]}"
do
    dotnet publish \
        $REPODIR/source/TSQLLint.Console/TSQLLint.Console.csproj \
        -c Release \
        -f netcoreapp3.1 \
        -r "$platform" \
        /p:Version="$VERSION" \
        -o "$ASSEMBLIESDIR/$PLATFORM"

    cd "$ASSEMBLIESDIR"
    tar -zcvf "$ASSEMBLIESDIR/$PLATFORM.tgz" "$PLATFORM"
    cd "$WORKING_DIRECTORY"
done
