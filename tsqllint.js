#!/usr/bin/env node
const { spawn } = require('child_process');
const os = require('os');

var args = process.argv.slice(2);

if (os.type() === 'Linux' || os.type() === 'Darwin') {
  args.unshift(`${__dirname}/osx-x64/TSQLLint.Console.dll`)
  var child = spawn('dotnet', args);
}
else if (os.type() === 'Windows_NT') {
  if (os.type() === 'Windows_NT') {
    if (process.arch == 'x32') {
      var child = spawn(`${__dirname}/win-x86/TSQLLint.Console.exe`, args);
    } else if (process.arch == 'x64') {
      var child = spawn(`${__dirname}/win-x64/TSQLLint.Console.exe`, args);
    }
  } 
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
