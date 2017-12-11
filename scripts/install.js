#!/usr/bin/env node

const https = require('follow-redirects').https;
const os = require('os');
const fs = require('fs');
const decompress = require('decompress');
const decompressTargz = require('decompress-targz');

function download(url, dest) {
    return new Promise((resolve, reject) => {
        var file = fs.createWriteStream(dest);
        var request = https.get(url, function(response) {
            response.pipe(file);
            console.log(response.statusCode)
            file.on('finish', function() {
                file.close(resolve);
            });
        }).on('error', function(err) {
            fs.unlink(dest);
            reject(err);
        });
    });
};

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

var urlBase = "https://github.com/tsqllint/tsqllint/releases/download/v1.8.2";
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
    }).then((err) => {
        if(err){
            console.log(err);
            return;
        }

        console.log('Files decompressed');
    });
});
