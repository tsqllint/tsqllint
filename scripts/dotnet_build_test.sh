#!/bin/bash

################################################################################
# a script to build, test, and push coverage results to coveralls
################################################################################

# enable for bash debugging
#set -x

# fail script if a cmd fails
set -e

# fail script if piped command fails
set -o pipefail

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
PATH="$PATH:$HOME/.dotnet/tools"
cd "$PROJECT_ROOT"

source "$SCRIPT_DIR/utils.sh"

confirmRunningInDocker

info "restoring project"

dotnet restore \
    "$PROJECT_ROOT/source/TSQLLint.sln" \
    --verbosity m \
    --packages "$DOTNET_PACKAGE_DIR"

info "building project"

dotnet build \
    "$PROJECT_ROOT/source/TSQLLint.sln" \
    /p:Version="$VERSION" \
    --configuration Release \
    --no-restore

info "calculating test coverage"

command -v csmacnz.Coveralls >/dev/null 2>&1 || {
    info "installing coveralls tooling"
    dotnet tool install coveralls.net --global --version 3.0.0;
}

command -v reportgenerator >/dev/null 2>&1 || {
    info "installing report generator tooling"
    dotnet tool install dotnet-reportgenerator-globaltool --global --version 5.0.2;
}

TEST_TARGET_FRAMEWORK=$1

info "running test project on $TEST_TARGET_FRAMEWORK"

dotnet test \
    --no-restore \
    --collect:"XPlat Code Coverage" \
    --settings "$PROJECT_ROOT/source/coverlet.runsettings" \
    --results-directory "$COVERAGE_DIR" \
    -property:TargetFramework=$TEST_TARGET_FRAMEWORK \
    "$PROJECT_ROOT/source/TSQLLint.sln"

COVERAGE_FILE="$(find $COVERAGE_DIR -name coverage.opencover.xml)"
info "COVERAGE_FILE: $COVERAGE_FILE"

reportgenerator \
    -reports:$COVERAGE_FILE \
    -targetdir:$COVERAGE_DIR/report

# change directory to reduce directory depth in archive file
cd "$COVERAGE_DIR"
tar -zcf $COVERAGE_DIR/coverage-report.tgz report

#cd "$PROJECT_ROOT"

#if [[ -n "${COVERALLS_REPO_TOKEN}" ]]; then
#
#  info "pushing coverage results"
#
#  JOB_ID=${CIRCLE_WORKFLOW_JOB_ID:-"$HEAD_COMMIT"}
#
#  csmacnz.Coveralls --opencover -i "$COVERAGE_FILE" \
#      --repoToken $COVERALLS_REPO_TOKEN \
#      --commitId $HEAD_COMMIT \
#      --commitBranch $BRANCH_NAME \
#      --commitAuthor "$COMMIT_AUTHOR" \
#      --commitEmail "$COMMIT_AUTHOR_EMAIL" \
#      --commitMessage "$COMMIT_MESSAGE" \
#      --jobId $JOB_ID  \
#      --serviceName "circle-ci"  \
#      --useRelativePaths
#fi

info "done"

exit 0
