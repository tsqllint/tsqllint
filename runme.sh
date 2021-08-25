#!/bin/bash

rm -rf artifacts

dotnet pack ./source/TSQLLint.sln \
 --output artifacts \
 --configuration Release

dotnet tool uninstall --global TSQLLint.Console

dotnet tool install --global --add-source ./artifacts TSQLLint.Console

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

echo "###############################"
echo "###############################"
echo ""
