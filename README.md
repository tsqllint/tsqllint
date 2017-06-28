# tsqllint

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

add rule severity property to [LintConfigRules](./TSQLLINT_LIB/Config/LintConfigRules.cs), be sure to decorate with the JsonProperty name that you use in the .tsqllintrc file

```csharp
internal class LintConfigRules
{
    [JsonProperty("select-star")]
    public RuleViolationSeverity SelectStar { get; set; }

    [JsonProperty("statement-semicolon-termination")]
    public RuleViolationSeverity StatementSemicolon { get; set; }

    [JsonProperty("set-transaction-isolation-level")]
    public RuleViolationSeverity SetTransactionIsolationLevel { get; set; }
}

```

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