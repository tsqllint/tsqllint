#!/usr/bin/env node

const https = require('follow-redirects').https;
const os = require('os');
const fs = require('fs');
const decompress = require('decompress');
const decompressTargz = require('decompress-targz');
const ProgressBar = require('progress');

var runTime = "";
if (os.type() === 'Darwin') {
    runTime = "osx-x64";
}
else if (os.type() === 'Windows_NT') {
    if (process.arch == 'x32') {
        runTime = "win-x64";
    } else if (process.arch == 'x64') {
        runTime = 'win-x86';
    }
}
else {
    throw new Error(`Invalid Platform: ${os.type()}`)
}


function download(url, dest) {
    return new Promise((resolve, reject) => {
        var file = fs.createWriteStream(dest);
        var request = https.get(url, function(response) {
            if(response.statusCode != 200){
                console.log(`There was a problem downloading ${url}`);
                return;
            }
            response.pipe(file);
            file.on('finish', function() {
                file.close(resolve);
            });
        })
        .on('response', (res) => {
            var len = parseInt(res.headers['content-length'], 10);

            var bar = new ProgressBar(`Downloading TSQLLint ${runTime} Runtime [:bar] :rate/bps :percent :etas`, {
                complete: '=',
                incomplete: ' ',
                width: 20,
                total: len
            });

            res.on('data', function (chunk) {
                bar.tick(chunk.length);
            });
        })
        .on('error', function(err) {
            fs.unlink(dest);
            reject(err);
        });
    });
};

var urlBase = 'https://github.com/tsqllint/tsqllint/releases/download/v1.8.2';
download(`${urlBase}/${runTime}.tar.gz`, `${runTime}.tar.gz`, (err) => {
    if(err){
        console.log(err);
        return;
    }
}).then((err) => {
    if(err){
        console.log(err);
        return;
    }

    decompress(`${runTime}.tar.gz`, './', {
        plugins: [
            decompressTargz()
        ]
    });
});
