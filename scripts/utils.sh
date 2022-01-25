#!/bin/bash

# enable for bash debugging
#set -x

# fail script if a cmd fails
set -e

# fail script if piped command fails
set -o pipefail

NO_COLOR='\033[0m'

BLUE='\033[0;34m'
info () {
    MESSAGE=$1
    printf "${BLUE}INFO:${NO_COLOR} $MESSAGE\n"
}

RED='\033[0;31m'
error () {
    MESSAGE=$1
    printf "${RED}ERROR:${NO_COLOR} $MESSAGE\n"
    exit 1
}

function confirmRunningInDocker() {
    if [ ! -f /.dockerenv ]; then
        error "This script must be run from within a docker container. For local development use the ci_run_local.sh script.";
    else
        info "script is running in container"
    fi
}

info "initializing"

GIT_STATE="clean"
if [[ $(git diff --stat) != '' ]]; then
  GIT_STATE="dirty"
fi

BRANCH_NAME="$(git rev-parse --abbrev-ref HEAD)"

# replace underscores with -
BRANCH_NAME="${BRANCH_NAME//[_]/-}"

# replace slashes with -
BRANCH_NAME="${BRANCH_NAME//[\/]/-}"

TAG_COMMIT="$(git rev-list --abbrev-commit --tags --max-count=1)"
TAG="$(git describe --abbrev=0 --tags "${TAG_COMMIT}" 2>/dev/null || true)"

HEAD_COMMIT="$(git rev-parse --short HEAD)"
HEAD_COMMIT_DATE=$(git log -1 --format=%cd --date=format:'%Y%m%d')

ARTIFACTS_DIR="$PROJECT_ROOT/artifacts"
mkdir -p "$ARTIFACTS_DIR"

COVERAGE_DIR="$ARTIFACTS_DIR/coverage"
mkdir -p "$COVERAGE_DIR"

COMMIT_AUTHOR=$(git show -s --pretty=format:"%cn")
COMMIT_AUTHOR_EMAIL=$(git show -s --pretty=format:"%ce")
COMMIT_MESSAGE=$(git show -s --pretty=format:"%s")

DOTNET_PACKAGE_DIR="$PROJECT_ROOT/packages"

RELEASE="false"
if [ "$HEAD_COMMIT" == "$TAG_COMMIT" ] && [ "$GIT_STATE" == "clean" ]; then
	VERSION="$TAG"
    RELEASE="true"
else
	VERSION="$TAG"-"$BRANCH_NAME"-"$HEAD_COMMIT"-"$HEAD_COMMIT_DATE"-"$GIT_STATE"
fi

echo "###############################################################"
echo "# BRANCH_NAME:          ${BRANCH_NAME}                         "
echo "# GIT_STATE:            ${GIT_STATE}                           "
echo "# RELEASE:              ${RELEASE}                             "
echo "# TAG:                  ${TAG}                                 "
echo "# TAG_COMMIT:           ${TAG_COMMIT}                          "
echo "# HEAD_COMMIT:          ${HEAD_COMMIT}                         "
echo "# HEAD_COMMIT_DATE:     ${HEAD_COMMIT_DATE}                    "
echo "# VERSION:              ${VERSION}                             "
echo "# COMMIT_AUTHOR:        $COMMIT_AUTHOR                         "
echo "# COMMIT_AUTHOR_EMAIL:  $COMMIT_AUTHOR_EMAIL                   "
echo "# COMMIT_MESSAGE:       $COMMIT_MESSAGE                        "
echo "# PROJECT_ROOT:         $PROJECT_ROOT                          "
echo "# SCRIPT_DIR:           $SCRIPT_DIR                            "
echo "# ARTIFACTS_DIR:        $ARTIFACTS_DIR                         "
echo "# COVERAGE_DIR:         $COVERAGE_DIR                          "
echo "# DOTNET_PACKAGE_DIR:   $DOTNET_PACKAGE_DIR                    "
echo "###############################################################"

info "verifying variables"

[ -n "$BRANCH_NAME" ]          || { error "BRANCH_NAME is required and not set"; }
[ -n "$GIT_STATE" ]            || { error "GIT_STATE is required and not set"; }
[ -n "$RELEASE" ]              || { error "RELEASE is required and not set"; }
[ -n "$TAG" ]                  || { error "TAG is required and not set"; }
[ -n "$TAG_COMMIT" ]           || { error "TAG_COMMIT is required and not set"; }
[ -n "$HEAD_COMMIT" ]          || { error "HEAD_COMMIT is required and not set"; }
[ -n "$HEAD_COMMIT_DATE" ]     || { error "HEAD_COMMIT_DATE is required and not set"; }
[ -n "$VERSION" ]              || { error "VERSION is required and not set"; }
[ -n "$PROJECT_ROOT" ]         || { error "PROJECT_ROOT is required and not set"; }
[ -n "$SCRIPT_DIR" ]           || { error "SCRIPT_DIR is required and not set"; }
[ -n "$COVERAGE_DIR" ]         || { error "COVERAGE_DIR is required and not set"; }
[ -n "$DOTNET_PACKAGE_DIR" ]   || { error "DOTNET_PACKAGE_DIR is required and not set"; }

info "verifying version number"

if [[ $VERSION =~ ^[0-9]+\.[0-9]+ ]]; then
    _=${BASH_REMATCH[0]}
else
    error "Version number failed validation: '$VERSION'"
fi

info "exporting variables"

export BRANCH_NAME
export GIT_STATE
export RELEASE
export TAG
export TAG_COMMIT
export HEAD_COMMIT
export HEAD_COMMIT_DATE
export VERSION
export PROJECT_ROOT
export SCRIPT_DIR
export COVERAGE_DIR
export DOTNET_PACKAGE_DIR

info "verifying directories"

[ ! -d "$PROJECT_ROOT" ]       && error "PROJECT_ROOT does not exist $PROJECT_ROOT"
[ ! -d "$SCRIPT_DIR" ]         && error "SCRIPT_DIR does not exist: $SCRIPT_DIR"
[ ! -d "$ARTIFACTS_DIR" ]      && error "ARTIFACTS_DIR does not exist: $ARTIFACTS_DIR"
[ ! -d "$COVERAGE_DIR" ]       && error "COVERAGE_DIR does not exist: $COVERAGE_DIR"

# DOTNET_PACKAGE_DIR should not exist before dotnet restore

info "initialization complete"
