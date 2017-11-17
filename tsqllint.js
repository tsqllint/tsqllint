#!/usr/bin/env node
var exec = require('child_process').exec;
var os = require('os');

var args = process.argv.slice(2);

if (os.type() === 'Linux' || os.type() === 'Darwin') {
  var cmd = `dotnet ${__dirname}/lib/netcoreapp2.0/TSQLLint.Console.dll ${args.join(' ')}`
}
else if (os.type() === 'Windows_NT') {
   var cmd = `${__dirname}/lib/net452/TSQLLint.Console.exe ${args.join(' ')}`
}
else {
  throw new Error(`Invalid Platform: ${os.type()}`)
}

var child = exec(cmd);

child.stdout.on('data', function(data) {
  process.stdout.write(data);
});

child.stderr.on('data', function(data) {
  process.stdout.write(data);
});
