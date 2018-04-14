#!/usr/bin/env node

const https = require('follow-redirects').https
const os = require('os')
const fs = require('fs')
const decompress = require('decompress')
const decompressTargz = require('decompress-targz')
const ProgressBar = require('progress')

var runTime = ''
if (os.type() === 'Darwin') {
  runTime = 'osx-x64'
} else if (os.type() === 'Linux') {
  runTime = 'linux-x64'
} else if (os.type() === 'Windows_NT') {
  if (process.arch === 'ia32') {
    runTime = 'win-x86'
  } else if (process.arch === 'x64') {
    runTime = 'win-x64'
  }
} else {
  throw new Error(`Invalid Platform: ${os.type()}`)
}

function download (url, dest) {
  return new Promise((resolve, reject) => {
    var file = fs.createWriteStream(dest)
    var request = https.get(url, function (response) {
      response.pipe(file)
      file.on('finish', function () {
        file.close(resolve)
      })
    })
        .on('response', (res) => {
          if (res.statusCode != 200) {
            fs.unlink(dest)
            return reject(new Error(`There was a problem downloading ${url}`))
          }

          var len = parseInt(res.headers['content-length'], 10)

          var bar = new ProgressBar(`Downloading TSQLLint ${runTime} Runtime [:bar] :rate/bps :percent :etas`, {
            complete: '=',
            incomplete: ' ',
            width: 20,
            total: len
          })

          res.on('data', function (chunk) {
            bar.tick(chunk.length)
          })
        })
        .on('error', function (err) {
          fs.unlink(dest)
          reject(err)
        })
  })
};

var version = 'v1.10.1'
var urlBase = `https://github.com/tsqllint/tsqllint/releases/download/${version}`
download(`${urlBase}/${runTime}.tgz`, `${runTime}.tgz`, (err) => {
  if (err) {
    console.log(err)
  }
}).then((err) => {
  if (err) {
    console.log(err)
    return
  }

    decompress(`${runTime}.tgz`, './assemblies', {
    plugins: [
      decompressTargz()
    ]
  })
}).catch((err) => {
  console.log(err)
  process.exit(1)
})
