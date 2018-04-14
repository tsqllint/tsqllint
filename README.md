[![Build Status](https://travis-ci.org/tsqllint/tsqllint.svg?branch=master)](https://travis-ci.org/tsqllint/tsqllint)
[![Build Status](https://ci.appveyor.com/api/projects/status/github/tsqllint/tsqllint?svg=true&branch=master)](https://ci.appveyor.com/project/nathan-boyd/tsqllint)
[![codecov](https://codecov.io/gh/tsqllint/tsqllint/branch/master/graph/badge.svg)](https://codecov.io/gh/tsqllint/tsqllint)  

[![npm version](https://badge.fury.io/js/tsqllint.svg)](https://badge.fury.io/js/tsqllint)
[![npm](https://img.shields.io/npm/dt/tsqllint.svg)](https://www.npmjs.com/package/tsqllint)
[![Gitter chat](https://img.shields.io/gitter/room/badges/shields.svg)](https://gitter.im/TSQLLint/Lobby)

# TSQLLint

TSQLLint is a tool for describing, identifying, and reporting on undesirable patterns in TSQL scripts.

## Installation

The recommended method of installing tsqllint is to install the tool globally using NPM.

This binary can be installed though [the `npm` registry](https://www.npmjs.com/). First, install [Node.js version 4 or higher](https://nodejs.org/en/download/), and then installation is done using the [`npm install` command](https://docs.npmjs.com/getting-started/installing-npm-packages-locally):

``` bash
npm install tsqllint -g
```

## Usage

``` bash
# lint a single file
tsqllint test.sql

# lint all files in a directory
tsqllint c:\database_scripts

# lint a list of files and directories, paths with whitespace must be enclosed in quotes
tsqllint file_one.sql file_two.sql "c:\database scripts"

# lint using wild cards
tsqllint c:\database_scripts\file*.sql

# print path to .tsqllintrc config file
tsqllint --print-config

# display usage info
tsqllint --help

# list the plugins loaded
tsqllint --list-plugins
```

## Integrating TSQLLint with other Tools

### Visual Studio Code Extension

TSQLLint is built into a [Visual Studio Code Extension](https://marketplace.visualstudio.com/items?itemName=tsqllint.tsqllint). This extension can be installed from within the VS Code Extension Interface or from the VS Code Extension marketplace.

![Usage Example](documentation/usage-animation.gif?raw=true)

### SQL Server Management Studio

TSQLLint uses a common message format that allows for integration into off the shelf tools. SQL Server Management Studio can use TSQLLint using SSMS's External Tools feature.

![SSMS Integration Image](documentation/SSMSIntegrationScreenshot.PNG)

## Configuration

TSQLLint utilizes a configuration file called `.tsqllintrc`. This file can be generated and edited by users to create their own configurations. To generate this file use the `-i` or `--init` flags. If no `.tsqllintrc` is found the tool with use a default configuration loaded in memory.

```bash
# generate a default .tsqllintrc file using the init flag (optional if just using a default configuration)
tsqllint --init
```

## Creating custom configurations

You may configure TSQLLint by editing its config file `.tsqllintrc` you can find its location with the `--print-confg` or `-p` option.

TSQLLint will load its config file in the following order or precedence:

1. The value passed with the `-c` command line argument, if one is passed
2. An Environment Variable named `TSQLLINTRC`
3. A file named `.tsqllintrc` in the same local directory in which you are executing TSQLLint
4. A file named `.tsqllintrc` in the user's home directory

## Rule configuration

TSQLLint rules may be set to "off", "warning", or "error". Rules that are violated and are set to "error" will result in TSQLLint returning a non-zero exit code. Rules that are violated, but configured to "warning" will result in a zero exit code, but a warning message will be displayed in the shell output. Rules that are set to "off" will be completely ignored. Rules that are not present in the `.tsqllintrc` configuration are set to "off"

```json
{
    "rules": {
        "concat-strings": "error",
        "conditional-begin-end": "error",
        "cross-database-transaction": "error",
        "data-compression": "error",
        "data-type-length": "error",
        "disallow-cursors": "error",
        "full-text": "error",
        "information-schema": "error",
        "keyword-capitalization": "error",
        "linked-server": "error",
        "multi-table-alias": "error",
        "non-sargable": "error",
        "object-property": "error",
        "print-statement": "error",
        "schema-qualify": "error",
        "select-star": "error",
        "semicolon-termination": "error",
        "set-ansi": "error",
        "set-nocount": "error",
        "set-quoted-identifier": "error",
        "set-transaction-isolation-level": "error",
        "set-variable": "error",
        "upper-lower": "error"
    }
}
```

## SQL Compatibility Level

TSQLLint provides a configurable "compatibility-level" that aligns with [SQL Server's Compatibility Level](http://docs.microsoft.com/en-us/sql/relational-databases/databases/view-or-change-the-compatibility-level-of-a-database). The value defaults to 120 but may be changed with the following edit to the `.tsqllintrc`. TSQLLint supports the following compatibility levels  80, 90, 100, 110, 120, and 130.

```json
{
    "rules": {
        "upper-lower": "error"
    },
    "compatability-level": 90
}
```

## Disabling Rules with Inline Comments

To temporarily disable rule warnings in a script, use comments in the following format:

```sql
/* tsqllint-disable */

SELECT * FROM FOO;

/* tsqllint-enable */
```

You can also disable or enable warnings for specific rules:

```sql
/* tsqllint-disable select-star */

SELECT * FROM FOO;

/* tsqllint-enable select-star */
```

To disable warnings for the entire script, place a /* tsqllint-disable */ comment at the top of the file:

```sql
/* tsqllint-disable */

SELECT * FROM FOO;
```

To disable specific rule warnings for the entire script place a comment similar to the following at the top of the file:

```sql
/* tsqllint-disable select-star */

SELECT * FROM FOO;
```

## Plugins

You can extend the base functionality of TSQLLint by creating a custom plugin. TSQLLint plugins are .Net assemblies that implement the IPlugin interface from TSQLLint.Common.

Once you complete your plugin, update your .tsqllintrc file to point to your assembly.

```json
{
    "rules": {
        "upper-lower": "error"
    },
    "plugins": {
        "my-first-plugin": "c:/users/someone/my-plugins/my-first-plugin.dll",
        "my-second-plugin": "c:/users/someone/my-plugins/my-second-plugin.dll/"
    }
}
```

This sample plugin notifies users that spaces should be used rather than tabs.

``` csharp
using System;
using TSQLLint.Common;

namespace TSQLLint.Tests.UnitTests.PluginHandler
{
    public class SamplePlugin : IPlugin
    {
        public void PerformAction(IPluginContext context, IReporter reporter)
        {
            string line;
            var lineNumber = 0;

            while ((line = context.FileContents.ReadLine()) != null)
            {
                lineNumber++;
                var column = line.IndexOf("\t", StringComparison.Ordinal);
                reporter.ReportViolation(new SampleRuleViolation(
                    context.FilePath,
                    "prefer-tabs",
                    "Should use spaces rather than tabs",
                    lineNumber,
                    column,
                    RuleViolationSeverity.Warning));
            }
        }
    }

    class SampleRuleViolation : IRuleViolation
    {
        public int Column { get; private set; }
        public string FileName { get; private set; }
        public int Line { get; private set; }
        public string RuleName { get; private set; }
        public RuleViolationSeverity Severity { get; private set; }
        public string Text { get; private set; }

        public TestRuleViolation(string fileName, string ruleName, string text, int lineNumber, int column, RuleViolationSeverity ruleViolationSeverity)
        {
            FileName = fileName;
            RuleName = ruleName;
            Text = text;
            Line = lineNumber;
            Column = column;
            Severity = ruleViolationSeverity;
        }
    }
}
```
