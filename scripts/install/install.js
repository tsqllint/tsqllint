var fs = require('fs')
const execSync = require('child_process').execSync;

// calls powershell script to install missing dependencies
execSync(`call ${process.env.APPDATA}/npm/node_modules/tsqllint/scripts/install/install-dependencies.cmd`);

// copy patched version of tsqllint script
fs.createReadStream('./scripts/install/tsqllint').pipe(fs.createWriteStream(`${process.env.APPDATA}/npm/tsqllint`))