#!/bin/bash

rm -rf artifacts

VERSION="0.0.1"

dotnet pack ./source/TSQLLint/TSQLLint.csproj \
  /p:Version="0.0.1" \
  --output artifacts \
  --configuration Release

dotnet tool uninstall --global tsqllint

dotnet tool install --global --add-source ./artifacts --version 0.0.1 TSQLLint

echo ""
echo "###############################"
echo "tsqllint --help"
echo "###############################"
echo ""

tsqllint --help

echo ""
echo "###############################"
echo "tsqllint --print-config"
echo "###############################"
echo ""

tsqllint --print-config

echo ""
echo "###############################"
echo "tsqllint --list-plugins"
echo "###############################"
echo ""

tsqllint --list-plugins

echo ""
echo "###############################"
echo "tsqllint foo.sql"
echo "###############################"
echo ""

tsqllint foo.sql

echo ""
echo "###############################"
echo "###############################"
echo ""

dotnet nuget push \
    "./artifacts/TSQLLint.$VERSION.nupkg" \
    --api-key "$NUGET_API_KEY"  \
    --source https://api.nuget.org/v3/index.json
