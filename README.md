# tsqllint

[![Build Status](https://ci.appveyor.com/api/projects/status/github/tsqllint/tsqllint?svg=true&branch=master)](https://ci.appveyor.com/project/nathan-boyd/tsqllint)
[![codecov](https://codecov.io/gh/tsqllint/tsqllint/branch/master/graph/badge.svg)](https://codecov.io/gh/tsqllint/tsqllint)

<!--
[![Code Climate](https://codeclimate.com/github/codeclimate/codeclimate/badges/gpa.svg)](https://codeclimate.com/github/tsqllint/tsqllint) 
-->

[![Gitter chat](https://badges.gitter.im/gitterHQ/gitter.png)](https://gitter.im/TSQLLint/Lobby)

## Usage 
### Configuration

sample .tsqllintrc file

```
{
  "rules": {
    "select-star": "error",
    "statement-semicolon-termination": "error",
    "set-transaction-isolation-level": "error"
  }
}
```

### Usage
```
TSQLLINT.exe -c .tsqllintrc -p test.sql
```

## Contributing to the project
### Adding a new rule


implement rule visitor in [Rules](./TSQLLINT_LIB/Rules)

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

add your new rule type to the RuleVisitors List

```csharp
    private readonly List<Type> RuleVisitors = new List<Type>()
    {
        typeof(DataCompressionOptionRule),
        typeof(DataTypeLengthRule),
        typeof(InformationSchemaRule),
        typeof(ObjectPropertyRule),
        typeof(SchemaQualifyRule),
        typeof(SelectStarRule),
        typeof(SemicolonRule),
        typeof(SetNoCountRule),
        typeof(SetTransactionIsolationLevelRule)
    };
```