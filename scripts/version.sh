#!/bin/bash

version=$1

csprojFiles=( "./TSQLLint.Console/TSQLLint.Console.csproj" "./TSQLLint.Lib/TSQLLint.Lib.csproj" "./TSQLLint.Tests/TSQLLint.Tests.csproj")
for i in "${csprojFiles[@]}"
do
    sed -i .bak "s/\<Version\>[[:digit:]]\.[[:digit:]]\.[[:digit:]]\<\/Version\>/\<Version\>$version\<\/Version\>/g" $i
done

sed -i .bak "s/v[[:digit:]]\.[[:digit:]]\.[[:digit:]]/v$version/g" ./scripts/install.js
sed -i .bak "s/\"version\": \"[[:digit:]]\.[[:digit:]]\.[[:digit:]]\"/\"version\": \"$version\"/g" ./package.json

find . -name "*.js.bak" -type f -delete
find . -name "*.json.bak" -type f -delete
find . -name "*.csproj.bak" -type f -delete
