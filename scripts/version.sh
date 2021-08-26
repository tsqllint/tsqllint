#!/bin/bash

version=$1

sed -E -i .bak "s/\'v([0-9]+\.){2}[0-9]+\'/'v$version'/g" ./scripts/install.js
sed -E -i .bak "s/\"version\": \"([0-9]+\.){2}[0-9]+\"/\"version\": \"$version\"/g" ./package.json

find . -name "*.js.bak" -type f -delete
find . -name "*.json.bak" -type f -delete
