/*
  This file is auto-generated and should not be edited directly.
  All changes should be made to the accompanying template.
*/

using System;
using System.Collections.Generic;

namespace TSQLLint.Lib.Parser
{
    public static class RuleVisitorFriendlyNameTypeMap
    {
        // a list of all classes that implement ISqlRule
        public static readonly Dictionary<string, Type> List = new Dictionary<string, Type>
        {
            { "cross-database", typeof(TSQLLint.Lib.Rules.CrossDatabaseRule) },
            { "data-type-length", typeof(TSQLLint.Lib.Rules.DataTypeLengthRule) },
            { "linked-server", typeof(TSQLLint.Lib.Rules.LinkedServerRule) },
            { "non-sargable", typeof(TSQLLint.Lib.Rules.NonSargableRule) },
            { "print-statement", typeof(TSQLLint.Lib.Rules.PrintStatementRule) },
            { "conditional-begin-end", typeof(TSQLLint.Lib.Rules.ConditionalBeginEndRule) },
            { "disallow-cursors", typeof(TSQLLint.Lib.Rules.DisallowCursorRule) },
            { "keyword-capitalization", typeof(TSQLLint.Lib.Rules.KeywordCapitalizationRule) },
            { "multi-table-alias", typeof(TSQLLint.Lib.Rules.MultiTableAliasRule) },
            { "set-variable", typeof(TSQLLint.Lib.Rules.SetVariableRule) },
            { "upper-lower", typeof(TSQLLint.Lib.Rules.UpperLowerRule) },
            { "schema-qualify", typeof(TSQLLint.Lib.Rules.SchemaQualifyRule) },
            { "information-schema", typeof(TSQLLint.Lib.Rules.InformationSchemaRule) },
            { "object-property", typeof(TSQLLint.Lib.Rules.ObjectPropertyRule) },
            { "data-compression", typeof(TSQLLint.Lib.Rules.DataCompressionOptionRule) },
            { "select-star", typeof(TSQLLint.Lib.Rules.SelectStarRule) },
            { "set-quoted-identifier", typeof(TSQLLint.Lib.Rules.SetQuotedIdentifierRule) },
            { "set-ansi", typeof(TSQLLint.Lib.Rules.SetAnsiNullsRule) },
            { "set-nocount", typeof(TSQLLint.Lib.Rules.SetNoCountRule) },
            { "set-transaction-isolation-level", typeof(TSQLLint.Lib.Rules.SetTransactionIsolationLevelRule) },
            { "semicolon-termination", typeof(TSQLLint.Lib.Rules.SemicolonTerminationRule) },
        };
    }
}
