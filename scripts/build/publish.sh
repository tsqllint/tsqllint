#!/bin/bash

set -e

WORKING_DIRECTORY=$(pwd)
SCRIPTDIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
BUILDSCRIPTDIR="$(dirname "$SCRIPTDIR")"
REPODIR="$(dirname "$BUILDSCRIPTDIR")"
ASSEMBLIESDIR=$REPODIR/artifacts/assemblies
mkdir -p $ASSEMBLIESDIR

dotnet publish $REPODIR/source/TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp3.1 -r win-x86   -o $ASSEMBLIESDIR/win-x86
dotnet publish $REPODIR/source/TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp3.1 -r win-x64   -o $ASSEMBLIESDIR/win-x64
dotnet publish $REPODIR/source/TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp3.1 -r osx-x64   -o $ASSEMBLIESDIR/osx-x64
dotnet publish $REPODIR/source/TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp3.1 -r linux-x64 -o $ASSEMBLIESDIR/linux-x64

tar -zcvf $ASSEMBLIESDIR/osx-x64.tgz   $ASSEMBLIESDIR/osx-x64
tar -zcvf $ASSEMBLIESDIR/win-x86.tgz   $ASSEMBLIESDIR/win-x86
tar -zcvf $ASSEMBLIESDIR/win-x64.tgz   $ASSEMBLIESDIR/win-x64
tar -zcvf $ASSEMBLIESDIR/linux-x64.tgz $ASSEMBLIESDIR/linux-x64
