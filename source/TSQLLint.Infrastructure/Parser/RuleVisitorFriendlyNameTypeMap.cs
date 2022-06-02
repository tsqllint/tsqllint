using System;
using System.Collections.Generic;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Rules;

namespace TSQLLint.Infrastructure.Parser
{
    public static class RuleVisitorFriendlyNameTypeMap
    {
        public static Dictionary<string, ISqlLintRule> Rules => List.ToDictionary(
            x => x.Key, x => (ISqlLintRule)Activator.CreateInstance(x.Value, (Action<string, string, int, int>)null));

        // a list of all classes that implement ISqlRule
        public static readonly Dictionary<string, Type> List = new Dictionary<string, Type>
        {
            { "case-sensitive-variables", typeof(CaseSensitiveVariablesRule) },
            { "conditional-begin-end", typeof(ConditionalBeginEndRule) },
            { "count-star", typeof(CountStarRule) },
            { "cross-database-transaction", typeof(CrossDatabaseTransactionRule) },
            { "data-compression", typeof(DataCompressionOptionRule) },
            { "data-type-length", typeof(DataTypeLengthRule) },
            { "delete-where", typeof(DeleteWhereRule) },
            { "disallow-cursors", typeof(DisallowCursorRule) },
            { "duplicate-empty-line", typeof(DuplicateEmptyLineRule) },
            { "duplicate-go", typeof(DuplicateGoRule) },
            { "full-text", typeof(FullTextRule) },
            { "information-schema", typeof(InformationSchemaRule) },
            { "keyword-capitalization", typeof(KeywordCapitalizationRule) },
            { "linked-server", typeof(LinkedServerRule) },
            { "multi-table-alias", typeof(MultiTableAliasRule) },
            { "named-constraint", typeof(NamedContraintRule) },
            { "non-sargable", typeof(NonSargableRule) },
            { "object-property", typeof(ObjectPropertyRule) },
            { "print-statement", typeof(PrintStatementRule) },
            { "schema-qualify", typeof(SchemaQualifyRule) },
            { "select-star", typeof(SelectStarRule) },
            { "semicolon-termination", typeof(SemicolonTerminationRule) },
            { "set-ansi", typeof(SetAnsiNullsRule) },
            { "set-nocount", typeof(SetNoCountRule) },
            { "set-quoted-identifier", typeof(SetQuotedIdentifierRule) },
            { "set-transaction-isolation-level", typeof(SetTransactionIsolationLevelRule) },
            { "set-variable", typeof(SetVariableRule) },
            { "unicode-string", typeof(UnicodeStringRule) },
            { "update-where", typeof(UpdateWhereRule) },
            { "upper-lower", typeof(UpperLowerRule) }
        };
    }
}
