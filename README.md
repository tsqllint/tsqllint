[![npm version](https://badge.fury.io/js/tsqllint.svg)](https://badge.fury.io/js/tsqllint)
[![Build Status](https://ci.appveyor.com/api/projects/status/github/tsqllint/tsqllint?svg=true&branch=master)](https://ci.appveyor.com/project/nathan-boyd/tsqllint)
[![codecov](https://codecov.io/gh/tsqllint/tsqllint/branch/master/graph/badge.svg)](https://codecov.io/gh/tsqllint/tsqllint)  

[![Gitter chat](https://img.shields.io/gitter/room/badges/shields.svg)](https://gitter.im/TSQLLint/Lobby)

# tsqllint

tsqllint is a tool for describing, identifying, and reporting on undesirable patterns in TSQL scripts

## Installation

The recommended method of installing tsqllint is to install the tool globally using NPM.

```
$ npm install tsqllint -g
```

## Configuration

```
# generate a default .tsqllintrc file using the init flag
$ tsqllint --init
```

## Usage
```
# lint a single file
$ tsqllint test.sql

# lint a list of files and directories (must be seperated by comma and enclosed in quotes)
$ tsqllint "test_one.sql, test_two.sql, c:\database_scripts"

# lint all files in a directory
$ tsqllint "c:\database_scripts"

# display usage hints
$ tsqllint --help

# print path to .tsqllintrc config file
$ tsqllint --print

# display usage info
$ tsqllint --help
```

## Creating custom configurations

Configure tsqllint by editing its config file, which is called .tsqllintrc, you can find its location with the "--print-confg" or "-p" option.  

Rules may be set to off, warning, or error.

```
{
    "rules": {
        "conditional-begin-end": "error",
        "data-compression": "error",
        "data-type-length": "error",
        "disallow-cursors": "error",
        "information-schema": "error",
        "keyword-capitalization": "error",
        "multi-table-alias": "error",
        "object-property": "error",
        "print-statement": "error",
        "schema-qualify": "error",
        "select-star": "error",
        "semicolon-termination": "error",
        "set-ansi": "error",
        "set-nocount": "error",
        "set-quoted-identifier": "error",
        "set-transaction-isolation-level": "error",
        "upper-lower": "error"
    }
}
```