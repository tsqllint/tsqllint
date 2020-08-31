#!/bin/bash

set -e

WORKING_DIRECTORY=$(pwd)
SCRIPTDIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
TESTSCRIPTDIR="$(dirname "$SCRIPTDIR")"
REPODIR="$(dirname "$TESTSCRIPTDIR")"
COVERAGEDIR="$REPODIR/artifacts/coverage"
COVERAGEFILE="$(find $COVERAGEDIR -name coverage.opencover.xml)"
mkdir -p "$COVERAGEDIR/report"

command -v reportgenerator >/dev/null 2>&1 || { dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.6.5; }

reportgenerator \
    -reports:$COVERAGEFILE \
    -targetdir:$COVERAGEDIR/report

tar -zcvf $COVERAGEDIR/coverage-report.tgz   $COVERAGEDIR/report
