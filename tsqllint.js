#!/usr/bin/env node
const { spawn } = require('child_process');
const os = require('os');

var args = process.argv.slice(2);

if (os.type() === 'Linux' || os.type() === 'Darwin') {
  args.unshift(`${__dirname}/lib/netcoreapp2.0/TSQLLint.Console.dll`)
  var child = spawn('dotnet', args);
}
else if (os.type() === 'Windows_NT') {
  var child = spawn(`${__dirname}/lib/net47/TSQLLint.Console.exe`, args);
}
else {
  throw new Error(`Invalid Platform: ${os.type()}`)
}

child.stdout.on('data', function(data) {
  process.stdout.write(data);
});

child.stderr.on('data', function(data) {
  process.stdout.write(data);
});
