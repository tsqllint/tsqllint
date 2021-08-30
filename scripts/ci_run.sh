#!/bin/bash

source /app/scripts/setup.sh

echoBlockMessage "restoring project"

dotnet restore \
    ./source/TSQLLint.sln \
    --verbosity m

echoBlockMessage "building project"

dotnet build \
    ./source/TSQLLint.sln \
    /p:Version="$VERSION" \
    --configuration Release \
    --no-restore

echoBlockMessage "running test project"

dotnet test \
    --no-restore \
    ./source/TSQLLint.sln

echoBlockMessage "packing project"

dotnet pack \
    ./source/TSQLLint.sln \
    -p:VERSION="$VERSION" \
    --configuration Release \
    --output /artifacts

echoBlockMessage "build and archive assemblies"

ASSEMBLIES_DIR="/artifacts/assemblies"
PLATFORMS=( "win-x86" "win-x64" "osx-x64" "linux-x64")
for PLATFORM in "${PLATFORMS[@]}"
do
    echoBlockMessage "building assemblies for platform $PLATFORM"

    OUT_DIR="$ASSEMBLIES_DIR/$PLATFORM"
    mkdir -p "$OUT_DIR"

    dotnet publish \
        "./source/TSQLLint/TSQLLint.csproj" \
        -c Release \
        -r "$PLATFORM" \
        /p:Version="$VERSION" \
        -o "$OUT_DIR"

    echoBlockMessage "archiving assemblies for platform $PLATFORM"

    cd "$ASSEMBLIES_DIR"
    tar -zcvf "/artifacts/$PLATFORM.tgz" "$PLATFORM"

    cd "/app"
done

if [ "$RELEASE" == "false" ]; then
    echoMessage "done"
    exit 0
fi

echoBlockMessage "pushing to Nuget"

if [[ -z "$NUGET_API_KEY" ]]; then
  echoMessage "NUGET_API_KEY is not set in the environment."
  echoMessage "Artifacts will not be pushed to Nuget."
  exit 1
fi

dotnet nuget push \
    "/artifacts/TSQLLint.$VERSION.nupkg" \
    --api-key "$NUGET_API_KEY"  \
    --source https://api.nuget.org/v3/index.json

echoBlockMessage "creating github release"

gh auth login  --hostname "github.com" --with-token < "$GITHUB_TOKEN_FILE"

gh release create "$VERSION" -d /artifacts/*.tgz

echoBlockMessage "done"

exit 0
