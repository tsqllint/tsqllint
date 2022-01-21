#!/bin/bash

# fail script if a cmd fails
set -e

# fail script if piped command fails
set -o pipefail

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

NC='\033[0m'

BLUE='\033[0;34m'
info () {
    MESSAGE=$1
    printf "${BLUE}INFO:${NC} $MESSAGE\n"
}

RED='\033[0;31m'
error () {
    MESSAGE=$1
    printf "${RED}ERROR:${NC} $MESSAGE\n"
    exit 1
}

if [ ! -f /.dockerenv ]; then
    error "This script must be run from within a docker container. For local development use the ci_run_local.sh script.";
fi

info "SCRIPT_DIR:  $SCRIPT_DIR"
info "PROJECT_DIR: $PROJECT_ROOT"

cd $PROJECT_ROOT

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

echo "###############################################################"
echo "# Branch Name:       ${BRANCH_NAME}                            "
echo "# Git State:         ${GIT_STATE}                              "
echo "# Release:           ${RELEASE}                                "
echo "# Tag:               ${TAG}                                    "
echo "# Tag Commit:        ${TAG_COMMIT}                             "
echo "# HEAD Commit:       ${HEAD_COMMIT}                            "
echo "# HEAD Date:         ${HEAD_COMMIT_DATE}                       "
echo "# Version:           ${VERSION}                                "
echo "###############################################################"

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
    error "Version number failed validation: '$VERSION'"
fi

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

    info "creating assemblies directory $OUT_DIR"

    dotnet publish \
        "./source/TSQLLint/TSQLLint.csproj" \
        -c Release \
        -r "$PLATFORM" \
        /p:Version="$VERSION" \
        -o "$OUT_DIR"

    info "archiving assemblies for platform $PLATFORM"

    cd "$ASSEMBLIES_DIR"

    tar -zcvf "/artifacts/$PLATFORM.tgz" "$PLATFORM"

    info "changing directory to $PROJECT_ROOT"

    cd $PROJECT_ROOT
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
