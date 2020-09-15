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
  REPO_COMMIT_AUTHOR=$(git show -s --pretty=format:"%cn")
  REPO_COMMIT_AUTHOR_EMAIL=$(git show -s --pretty=format:"%ce")
  REPO_COMMIT_MESSAGE=$(git show -s --pretty=format:"%s")
  csmacnz.Coveralls --opencover -i "$COVERAGEFILE" \
      --repoToken $COVERALLS_REPO_TOKEN \
      --commitId $TRAVIS_COMMIT \
      --commitBranch $TRAVIS_BRANCH \
      --commitAuthor "$REPO_COMMIT_AUTHOR" \
      --commitEmail "$REPO_COMMIT_AUTHOR_EMAIL" \
      --commitMessage "$REPO_COMMIT_MESSAGE" \
      --jobId $TRAVIS_JOB_ID  \
      --serviceName "travis-ci"  \
      --useRelativePaths
fi
