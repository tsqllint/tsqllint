#!/bin/bash

# this script publishes tsqllint assemblies for supported platforms

set -e

WORKING_DIRECTORY=$(pwd)
SCRIPTDIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
BUILDSCRIPTDIR="$(dirname "$SCRIPTDIR")"
REPODIR="$(dirname "$BUILDSCRIPTDIR")"
ASSEMBLIESDIR=$REPODIR/artifacts/assemblies
mkdir -p "$ASSEMBLIESDIR"

GIT_STATE="clean"
if [[ $(git diff --stat) != '' ]]; then
  GIT_STATE="dirty"
fi

TAG_COMMIT="$(git rev-list --abbrev-commit --tags --max-count=1)"
TAG="$(git describe --abbrev=0 --tags "${TAG_COMMIT}" 2>/dev/null || true)"

HEAD_COMMIT="$(git rev-parse --short HEAD)"
HEAD_COMMIT_DATE=$(git log -1 --format=%cd --date=format:'%Y%m%d')

BRANCH_NAME="$(git rev-parse --abbrev-ref HEAD)"

# replace / ( like in "pull/7" ) with -
BRANCH_NAME="${BRANCH_NAME//[\/]/-}"

RELEASE="false"
if [ "$HEAD_COMMIT" == "$TAG_COMMIT" ] && [ "$GIT_STATE" == "clean" ]; then
	VERSION="$TAG"
    RELEASE="true"
else
	VERSION="$TAG"-"$BRANCH_NAME"-"$HEAD_COMMIT"-"$HEAD_COMMIT_DATE"-"$GIT_STATE"
fi

# replace _ with -
VERSION="${VERSION//[_]/-}"

echo "#########################################################"
echo "# Branch Name: ${BRANCH_NAME}                            "
echo "# Git State:   ${GIT_STATE}                            "
echo "# Release:     ${RELEASE}                                "
echo "# Tag:         ${TAG}                                    "
echo "# Tag Commit:  ${TAG_COMMIT}                             "
echo "# HEAD Commit: ${HEAD_COMMIT}                            "
echo "# HEAD Date:   ${HEAD_COMMIT_DATE}                       "
echo "# Version:     ${VERSION}                               "
echo "#########################################################"

[ -n "$BRANCH_NAME" ]      || { echo "BRANCH_NAME is required and not set, aborting..." >&2; exit 1; }
[ -n "$GIT_STATE" ]        || { echo "GIT_STATE is required and not set, aborting..." >&2; exit 1; }
[ -n "$RELEASE" ]          || { echo "RELEASE is required and not set, aborting..." >&2; exit 1; }
[ -n "$TAG" ]              || { echo "TAG is required and not set, aborting..." >&2; exit 1; }
[ -n "$TAG_COMMIT" ]       || { echo "TAG_COMMIT is required and not set, aborting..." >&2; exit 1; }
[ -n "$HEAD_COMMIT" ]      || { echo "HEAD_COMMIT is required and not set, aborting..." >&2; exit 1; }
[ -n "$HEAD_COMMIT_DATE" ] || { echo "HEAD_COMMIT_DATE is required and not set, aborting..." >&2; exit 1; }
[ -n "$VERSION" ]          || { echo "VERSION is required and not set, aborting..." >&2; exit 1; }

if [[ $VERSION =~ ^[0-9]+\.[0-9]+ ]]; then
    _=${BASH_REMATCH[0]}
else
    echo "Version number failed validation: '$VERSION'"
    exit 1
fi

PLATFORMS=( "win-x86" "win-x64" "osx-x64" "linux-x64")
for PLATFORM in "${PLATFORMS[@]}"
do
    dotnet publish \
        "$REPODIR/source/TSQLLint/TSQLLint.csproj" \
        -c Release \
        -r "$PLATFORM" \
        /p:Version="$VERSION" \
        -o "$ASSEMBLIESDIR/$PLATFORM"

    # change directory to reduce directory depth in archive file
    cd "$ASSEMBLIESDIR"
    tar -zcvf "$ASSEMBLIESDIR/$PLATFORM.tgz" "$PLATFORM"
    cd "$WORKING_DIRECTORY"
done
