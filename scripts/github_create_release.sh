#!/bin/bash

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

info "creating github release"

gh config set prompt disabled
gh release create "$VERSION" --title "$VERSION" --prerelease --draft

PLATFORMS=( "win-x86" "win-x64" "osx-x64" "linux-x64")
for PLATFORM in "${PLATFORMS[@]}"
do
    ARTIFACT="${ARTIFACTS_DIR}/${PLATFORM}".tgz
    info "uploading artifact to release $ARTIFACT"
    gh release upload "$VERSION" "$ARTIFACT"
done

info "done"

exit 0
