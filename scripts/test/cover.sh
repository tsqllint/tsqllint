#!/bin/bash

TOOLPATH="$HOME/.dotnet/tools"
PATH="$PATH:$TOOLPATH"

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
command -v reportgenerator >/dev/null 2>&1 || { dotnet tool install dotnet-reportgenerator-globaltool --tool-path $TOOLPATH --version 4.6.5; }

reportgenerator \
    -reports:$COVERAGEFILE \
    -targetdir:$COVERAGEDIR/report

# change directory to reduce directory depth in archive file
cd $COVERAGEDIR
tar -zcvf $COVERAGEDIR/coverage-report.tgz report
cd $WORKING_DIRECTORY

if [[ -n "${TRAVIS}" ]]; then
  command -v csmacnz.Coveralls >/dev/null 2>&1 || { dotnet tool install coveralls.net --tool-path $TOOLPATH --version 2.0.0-beta.1; }
  # send coverage report to coveralls
  csmacnz.Coveralls --opencover -i "$COVERAGEFILE"
fi
