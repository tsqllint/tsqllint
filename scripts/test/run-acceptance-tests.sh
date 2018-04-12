#!/bin/bash

git clone "https://github.com/tsqllint/tsqllint-acceptance-testing.git"
cd tsqllint-acceptance-testing
npm install
export TEST_SCRIPT_PATH="../tsqllint.js"
npm run test
