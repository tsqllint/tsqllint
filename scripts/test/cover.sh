#!/bin/bash

PATH="$PATH:/root/.dotnet/tools"

WORKING_DIRECTORY=$(pwd)
SCRIPTDIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
BUILDSCRIPTDIR="$(dirname "$SCRIPTDIR")"
REPODIR="$(dirname "$BUILDSCRIPTDIR")"
COVERAGEDIR="$REPODIR/artifacts/coverage"

dotnet test \
    --collect:"XPlat Code Coverage" \
    --settings $REPODIR/source/coverlet.runsettings \
    --results-directory $COVERAGEDIR \
    $REPODIR/source/TSQLLint.sln

COVERAGEFILE="$(find $COVERAGEDIR -name coverage.opencover.xml)"

# if necessary install reportgenerator
command -v reportgenerator >/dev/null 2>&1 || { dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.6.5; }

reportgenerator \
    -reports:$COVERAGEFILE \
    -targetdir:$COVERAGEDIR/report

# change directory to reduce directory depth in archive file
cd $COVERAGEDIR
tar -zcvf $COVERAGEDIR/coverage-report.tgz report
cd $WORKING_DIRECTORY

if [[ ! -z "${TRAVIS}" ]]; then
  command -v csmacnz.Coveralls >/dev/null 2>&1 || { dotnet tool install --global coveralls.net --version 2.0.0-beta.1; }
  # send coverage report to coveralls
  csmacnz.Coveralls --opencover -i "$COVERAGEFILE"
fi
