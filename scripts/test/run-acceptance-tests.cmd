git clone https://github.com/tsqllint/tsqllint-acceptance-testing.git
SET TEST_SCRIPT_PATH=..\tsqllint.js
npm --prefix ./tsqllint-acceptance-testing install ./tsqllint-acceptance-testing && npm --prefix ./tsqllint-acceptance-testing run test