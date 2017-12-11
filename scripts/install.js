#!/usr/bin/env node

const https = require('follow-redirects').https;
const os = require('os');
const fs = require('fs');

var download = function(url, dest, cb) {
    var file = fs.createWriteStream(dest);
    var request = https.get(url, function(response) {
        response.pipe(file);
        console.log(response.statusCode)
        file.on('finish', function() {
            file.close(cb);
        });
    }).on('error', function(err) {
        fs.unlink(dest);
        if (cb) cb(err.message);
    });
};

if (os.type() === 'Darwin') {
    download("https://github.com/tsqllint/tsqllint/releases/download/v1.8.2/tsqllint-1.8.2.tgz", "./lib/bar.tgz", (err) => {
        if(err){
            Console.log(err);
        }
        return;
    });
}
else if (os.type() === 'Windows_NT') {

}
else {
    throw new Error(`Invalid Platform: ${os.type()}`)
}

