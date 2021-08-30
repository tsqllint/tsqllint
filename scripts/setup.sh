#!/bin/bash

# fail script if a cmd fails
set -e

# fail script if piped command fails
set -o pipefail

# echo a message
function echoMessage () {
    MESSAGE=$1
    printf "\n%s" "$MESSAGE"
}

# echos a message surrounded by a seperator
function echoBlockMessage () {
  MESSAGE=$1
  printf "\n#########################################################"
  printf "\n# %s" "$MESSAGE"
  printf "\n#########################################################"
  printf "\n"
  printf "\n"
}

if [ ! -f /.dockerenv ]; then
    echoMessage "This script must be run from within a Docker container."
    echoMessage "There is a script that will do this for you"
    exit
fi

cd app

GIT_STATE="clean"
if [[ $(git diff --stat) != '' ]]; then
  GIT_STATE="dirty"
fi

BRANCH_NAME="$(git rev-parse --abbrev-ref HEAD)"

# replace underscores with -
BRANCH_NAME="${BRANCH_NAME//[_]/-}"

# replace underscores with -
BRANCH_NAME="${BRANCH_NAME//[\/]/-}"

TAG_COMMIT="$(git rev-list --abbrev-commit --tags --max-count=1)"
TAG="$(git describe --abbrev=0 --tags "${TAG_COMMIT}" 2>/dev/null || true)"

HEAD_COMMIT="$(git rev-parse --short HEAD)"
HEAD_COMMIT_DATE=$(git log -1 --format=%cd --date=format:'%Y%m%d')

RELEASE="false"
if [ "$HEAD_COMMIT" == "$TAG_COMMIT" ] && [ "$GIT_STATE" == "clean" ]; then
	VERSION="$TAG"
    RELEASE="true"
else
	VERSION="$TAG"-"$BRANCH_NAME"-"$HEAD_COMMIT"-"$HEAD_COMMIT_DATE"-"$GIT_STATE"
fi

echo "#########################################################"
echo "# Branch Name: ${BRANCH_NAME}                            "
echo "# Git State:   ${GIT_STATE}                            "
echo "# Release:     ${RELEASE}                                "
echo "# Tag:         ${TAG}                                    "
echo "# Tag Commit:  ${TAG_COMMIT}                             "
echo "# HEAD Commit: ${HEAD_COMMIT}                            "
echo "# HEAD Date:   ${HEAD_COMMIT_DATE}                       "
echo "# Version:     ${VERSION}                                "
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
