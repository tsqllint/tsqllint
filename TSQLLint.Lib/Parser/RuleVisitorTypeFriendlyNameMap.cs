/*
  This file is auto-generated and should not be edited directly.
  All changes should be made to the accompanying template.
*/

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
            { "cross-database", typeof(CrossDatabaseRule) },
            { "data-type-length", typeof(DataTypeLengthRule) },
            { "linked-server", typeof(LinkedServerRule) },
            { "non-sargable", typeof(NonSargableRule) },
            { "print-statement", typeof(PrintStatementRule) },
            { "conditional-begin-end", typeof(ConditionalBeginEndRule) },
            { "disallow-cursors", typeof(DisallowCursorRule) },
            { "keyword-capitalization", typeof(KeywordCapitalizationRule) },
            { "multi-table-alias", typeof(MultiTableAliasRule) },
            { "set-variable", typeof(SetVariableRule) },
            { "upper-lower", typeof(UpperLowerRule) },
            { "schema-qualify", typeof(SchemaQualifyRule) },
            { "information-schema", typeof(InformationSchemaRule) },
            { "object-property", typeof(ObjectPropertyRule) },
            { "data-compression", typeof(DataCompressionOptionRule) },
            { "select-star", typeof(SelectStarRule) },
            { "set-quoted-identifier", typeof(SetQuotedIdentifierRule) },
            { "set-ansi", typeof(SetAnsiNullsRule) },
            { "set-nocount", typeof(SetNoCountRule) },
            { "set-transaction-isolation-level", typeof(SetTransactionIsolationLevelRule) },
            { "semicolon-termination", typeof(SemicolonTerminationRule) },
            { "concat-strings", typeof(ConcatStringsRule) }
        };
    }
}
