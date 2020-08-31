#!/bin/bash

WORKING_DIRECTORY=$(pwd)
SCRIPTDIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
BUILDSCRIPTDIR="$(dirname "$SCRIPTDIR")"
REPODIR="$(dirname "$BUILDSCRIPTDIR")"
COVERAGEDIR="$REPODIR/artifacts/coverage"
COVERAGEFILE="$(find $COVERAGEDIR -name coverage.opencover.xml)"

mkdir -p "$COVERAGEDIR/report"

dotnet test \
    --collect:"XPlat Code Coverage" \
    --settings $REPODIR/source/coverlet.runsettings \
    --results-directory $COVERAGEDIR \
    $REPODIR/source/TSQLLint.sln

command -v reportgenerator >/dev/null 2>&1 || { dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.6.5; }

reportgenerator \
    -reports:$COVERAGEFILE \
    -targetdir:$COVERAGEDIR/report

tar -zcvf $COVERAGEDIR/coverage-report.tgz   $COVERAGEDIR/report
