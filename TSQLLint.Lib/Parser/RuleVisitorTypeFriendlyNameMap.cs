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
            { "cross-database", typeof(TSQLLint.Lib.Standard.Rules.CrossDatabaseRule) },
            { "data-type-length", typeof(TSQLLint.Lib.Standard.Rules.DataTypeLengthRule) },
            { "linked-server", typeof(TSQLLint.Lib.Standard.Rules.LinkedServerRule) },
            { "non-sargable", typeof(TSQLLint.Lib.Standard.Rules.NonSargableRule) },
            { "print-statement", typeof(TSQLLint.Lib.Standard.Rules.PrintStatementRule) },
            { "conditional-begin-end", typeof(TSQLLint.Lib.Standard.Rules.ConditionalBeginEndRule) },
            { "disallow-cursors", typeof(TSQLLint.Lib.Standard.Rules.DisallowCursorRule) },
            { "keyword-capitalization", typeof(TSQLLint.Lib.Standard.Rules.KeywordCapitalizationRule) },
            { "multi-table-alias", typeof(TSQLLint.Lib.Standard.Rules.MultiTableAliasRule) },
            { "set-variable", typeof(TSQLLint.Lib.Standard.Rules.SetVariableRule) },
            { "upper-lower", typeof(TSQLLint.Lib.Standard.Rules.UpperLowerRule) },
            { "schema-qualify", typeof(TSQLLint.Lib.Standard.Rules.SchemaQualifyRule) },
            { "information-schema", typeof(TSQLLint.Lib.Standard.Rules.InformationSchemaRule) },
            { "object-property", typeof(TSQLLint.Lib.Standard.Rules.ObjectPropertyRule) },
            { "data-compression", typeof(TSQLLint.Lib.Standard.Rules.DataCompressionOptionRule) },
            { "select-star", typeof(TSQLLint.Lib.Standard.Rules.SelectStarRule) },
            { "set-quoted-identifier", typeof(TSQLLint.Lib.Standard.Rules.SetQuotedIdentifierRule) },
            { "set-ansi", typeof(TSQLLint.Lib.Standard.Rules.SetAnsiNullsRule) },
            { "set-nocount", typeof(TSQLLint.Lib.Standard.Rules.SetNoCountRule) },
            { "set-transaction-isolation-level", typeof(TSQLLint.Lib.Standard.Rules.SetTransactionIsolationLevelRule) },
            { "semicolon-termination", typeof(TSQLLint.Lib.Standard.Rules.SemicolonTerminationRule) },
        };
    }
}
