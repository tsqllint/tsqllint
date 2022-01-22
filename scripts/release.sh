#!/bin/bash

# enable for bash debugging
#set -x

# fail script if a cmd fails
set -e

# fail script if piped command fails
set -o pipefail

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd $PROJECT_ROOT

source "$SCRIPT_DIR/utils.sh"

if [ "$RELEASE" == "false" ]; then
    info "not a release build exiting now"
    exit 0
else
    info "starting release"
fi

[ -n "$NUGET_API_KEY" ]     || { echo "NUGET_API_KEY is required and not set, aborting..." >&2; exit 1; }
[ -n "$GITHUB_TOKEN_FILE" ] || { echo "GITHUB_TOKEN_FILE is required and not set, aborting..." >&2; exit 1; }

info "pushing to Nuget"

dotnet nuget push \
    "/artifacts/TSQLLint.$VERSION.nupkg" \
    --api-key "$NUGET_API_KEY"  \
    --source https://api.nuget.org/v3/index.json

info "creating github release"

gh auth login  --hostname "github.com" --with-token < "$GITHUB_TOKEN_FILE"
gh release create "$VERSION" -d /artifacts/*.tgz

info "done"

exit 0
