using System;
using System.Collections.Generic;
using TSQLLint.Lib.Rules;

namespace TSQLLint.Lib.Parser
{
    public static class RuleVisitorFriendlyNameTypeMap
    {
        // a list of all classes that implement ISqlRule
        public static readonly Dictionary<string, Type> List = new Dictionary<string, Type>
        {
            { "conditional-begin-end", typeof(ConditionalBeginEndRule) },
            { "cross-database-transaction", typeof(CrossDatabaseTransactionRule) },
            { "data-compression", typeof(DataCompressionOptionRule) },
            { "data-type-length", typeof(DataTypeLengthRule) },
            { "disallow-cursors", typeof(DisallowCursorRule) },
            { "full-text", typeof(FullTextRule) },
            { "information-schema", typeof(InformationSchemaRule) },
            { "keyword-capitalization", typeof(KeywordCapitalizationRule) },
            { "linked-server", typeof(LinkedServerRule) },
            { "multi-table-alias", typeof(MultiTableAliasRule) },
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
            { "upper-lower", typeof(UpperLowerRule) },
            { "unicode-string", typeof(UnicodeStringRule) }
        };
    }
}
