#!/bin/bash

################################################################################
# a script to create github releases from built artifacts
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

[ -n "$GITHUB_TOKEN_FILE" ] || { error "GITHUB_TOKEN_FILE is required and not set"; }

info "logging into github cli"

info "ls artifacts"
ls -lah $ARTIFACTS_DIR

echo "$GITHUB_TOKEN_FILE" > .githubtoken
gh auth login --with-token < .githubtoken
rm .githubtoken

info "configuring gh cli"
gh config set prompt disabled

info "creating github release"
gh release create "$VERSION" --title "$VERSION" --prerelease --draft

PLATFORMS=("win-x86" "win-x64" "win-arm64" "osx-x64" "osx-arm64" "linux-x64" "linux-musl-x64" "linux-musl-arm64" "linux-arm" "linux-arm64")
for PLATFORM in "${PLATFORMS[@]}"
do
    ARTIFACT="${ARTIFACTS_DIR}/${PLATFORM}".tgz
    info "uploading artifact '$ARTIFACT' to release $VERSION"
    gh release upload "$VERSION" "$ARTIFACT"
done

info "done"

exit 0
