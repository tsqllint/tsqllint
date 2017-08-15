var fs = require('fs')
fs.createReadStream('scripts/install/tsqllint').pipe(fs.createWriteStream(`${process.env.APPDATA}/npm/tsqllint`))