#!/usr/bin/env node

const spawn = require('child_process').spawn
const os = require('os')

var args = process.argv.slice(2)

if (os.type() === 'Darwin') {
  var child = spawn(`${__dirname}/assemblies/osx-x64/TSQLLint.Console`, args)
} else if (os.type() === 'Linux') {
  var child = spawn(`${__dirname}/assemblies/linux-x64/TSQLLint.Console`, args)
} else if (os.type() === 'Windows_NT') {
  if (os.type() === 'Windows_NT') {
    if (process.arch === 'ia32') {
      var child = spawn(`${__dirname}/assemblies/win-x86/TSQLLint.Console.exe`, args)
    } else if (process.arch === 'x64') {
      var child = spawn(`${__dirname}/assemblies/win-x64/TSQLLint.Console.exe`, args)
    } else {
      throw new Error(`Invalid Platform: ${os.type()}, ${process.arch}`)
    }
  }
} else {
  throw new Error(`Invalid Platform: ${os.type()}, ${process.arch}`)
}

child.stdout.on('data', function (data) {
  process.stdout.write(data)
})

child.stderr.on('data', function (data) {
  process.stdout.write(data)
})

child.on('exit', function (code) {
  process.exit(code);
})
