#!/bin/bash

version=$1

csprojFiles=( "./TSQLLint.Console/TSQLLint.Console.csproj" "./TSQLLint.Core/TSQLLint.Core.csproj" "./TSQLLint.Infrastructure/TSQLLint.Infrastructure.csproj" "./TSQLLint.Tests/TSQLLint.Tests.csproj")
for i in "${csprojFiles[@]}"
do
	sed -E -i .bak "s/\<Version\>([0-9]+\.){2}[0-9]+\<\/Version\>/\<Version\>$version\<\/Version\>/g" $i
done

sed -E -i .bak "s/\'v([0-9]+\.){2}[0-9]+\'/'v$version'/g" ./scripts/install.js
sed -E -i .bak "s/\"version\": \"([0-9]+\.){2}[0-9]+\"/\"version\": \"$version\"/g" ./package.json

find . -name "*.js.bak" -type f -delete
find . -name "*.json.bak" -type f -delete
find . -name "*.csproj.bak" -type f -delete
