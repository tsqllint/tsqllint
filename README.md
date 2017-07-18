# tsqllint

[![npm version](https://badge.fury.io/js/tsqllint.svg)](https://badge.fury.io/js/tsqllint)
[![Build Status](https://ci.appveyor.com/api/projects/status/github/tsqllint/tsqllint?svg=true&branch=master)](https://ci.appveyor.com/project/nathan-boyd/tsqllint)
[![codecov](https://codecov.io/gh/tsqllint/tsqllint/branch/master/graph/badge.svg)](https://codecov.io/gh/tsqllint/tsqllint)  

[![Gitter chat](https://img.shields.io/gitter/room/badges/shields.svg)](https://gitter.im/TSQLLint/Lobby)

### Installation 

```
npm install tsqllint -g
```

### Usage
```
# generate .tsqllintrc config file
tsqllint --init

# lint a single file
tsqllint --path test.sql

# lint a list of files (must be seperated by comma)
tsqllint --path "test_one.sql, test_two.sql"

# lint all .sql filed in a directory
tsqllint --path "c:\database_scripts"

# display usage hints
tsqllint --help
```

### Configuration

This tool can be configured by editing the .tsqllintrc file. Rules may be set to off, warning, or error.

sample .tsqllintrc file

```

{
  "rules": {
    "select-star": "off",
    "statement-semicolon-termination": "warning",
    "set-transaction-isolation-level": "error"
  }
}
```

## Contributing to the project

### Adding a new rule

#### Write tests
Write some failing tests in [Rules](./TSQLLINT_TEST/rules-tests.cs)  

#### Write your rule
Implement your new rule visitor in [Rules](./TSQLLINT_LIB/Rules) sample rule below.

```csharp
namespace TSQLLINT_LIB.Rules 
{
    public class SelectStarRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "select-star"; } }
        public string RULE_TEXT { get { return "Specify column names in SELECT"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;
    
        public SelectStarRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }
    
        public override void Visit(SelectStarExpression node)
        {
            ErrorCallback(RULE_NAME, RULE_TEXT, node);
        }
    }
}
```

#### Add rule to RuleVisitorBuilder
Add your new rule type to the RuleVisitors List in [RuleVisitorBuilder.cs](./TSQLLINT_LIB/Parser/RuleVisitorBuilder.cs), reflection is expensive.

```csharp
private readonly List<Type> RuleVisitors = new List<Type>()
{
    typeof(ConditionalBeginEndRule),
    typeof(DataCompressionOptionRule),
    typeof(DataTypeLengthRule),
    typeof(DisallowCursorRule),
    typeof(InformationSchemaRule),
    typeof(ObjectPropertyRule),
    typeof(PrintStatementRule),
    typeof(SchemaQualifyRule),
    typeof(SelectStarRule),
    typeof(SemicolonTerminationRule),
    typeof(SetAnsiNullsRule),
    typeof(SetNoCountRule),
    typeof(SetQuotedIdentifierRule),
    typeof(SetTransactionIsolationLevelRule),
    typeof(UpperLowerRule)
};
```