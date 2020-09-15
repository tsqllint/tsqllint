#!/bin/bash

PATH=$PATH:$HOME/.dotnet/tools

WORKING_DIRECTORY=$(pwd)
SCRIPTDIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
BUILDSCRIPTDIR="$(dirname "$SCRIPTDIR")"
REPODIR="$(dirname "$BUILDSCRIPTDIR")"
COVERAGEDIR="$REPODIR/artifacts/coverage"

command -v csmacnz.Coveralls >/dev/null 2>&1 || {
    dotnet tool install coveralls.net --global --version 2.0.0-beta0002;
}

command -v reportgenerator >/dev/null 2>&1 || {
    dotnet tool install dotnet-reportgenerator-globaltool --global --version 4.6.5;
}

dotnet test \
    --collect:"XPlat Code Coverage" \
    --settings $REPODIR/source/coverlet.runsettings \
    --results-directory $COVERAGEDIR \
    $REPODIR/source/TSQLLint.sln

COVERAGEFILE="$(find $COVERAGEDIR -name coverage.opencover.xml)"

reportgenerator \
    -reports:$COVERAGEFILE \
    -targetdir:$COVERAGEDIR/report

# change directory to reduce directory depth in archive file
cd $COVERAGEDIR
tar -zcvf $COVERAGEDIR/coverage-report.tgz report
cd $WORKING_DIRECTORY

if [[ -n "${COVERALLS_REPO_TOKEN}" ]]; then
  csmacnz.Coveralls --opencover -i "$COVERAGEFILE"
fi
