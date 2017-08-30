var fs = require('fs')
const execSync = require('child_process').execSync;

// calls powershell script to install missing dependencies
execSync(`call ${__dirname}/install-dependencies.cmd`);

// copy patched version of tsqllint script
fs.createReadStream(`${__dirname}/tsqllint`).pipe(fs.createWriteStream(`${process.env.npm_config_prefix}/tsqllint`))
