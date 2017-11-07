/*
  This file is auto-generated and should not be edited directly.
  All changes should be made to the accompanying template.
*/

using System;
using System.Collections.Generic;

namespace TSQLLint.Lib.Parser
{
    public static class RuleVisitorTypes
    {
        // a list of all classes that implement ISqlRule
        public static readonly List<Type> TypeList = new List<Type>
        {
            typeof(TSQLLint.Lib.Rules.ConditionalBeginEndRule),
            typeof(TSQLLint.Lib.Rules.CrossDatabaseRule),
            typeof(TSQLLint.Lib.Rules.DataCompressionOptionRule),
            typeof(TSQLLint.Lib.Rules.DataTypeLengthRule),
            typeof(TSQLLint.Lib.Rules.DisallowCursorRule),
            typeof(TSQLLint.Lib.Rules.InformationSchemaRule),
            typeof(TSQLLint.Lib.Rules.KeywordCapitalizationRule),
            typeof(TSQLLint.Lib.Rules.LinkedServerRule),
            typeof(TSQLLint.Lib.Rules.MultiTableAliasRule),
            typeof(TSQLLint.Lib.Rules.NonSargableRule),
            typeof(TSQLLint.Lib.Rules.ObjectPropertyRule),
            typeof(TSQLLint.Lib.Rules.PrintStatementRule),
            typeof(TSQLLint.Lib.Rules.SchemaQualifyRule),
            typeof(TSQLLint.Lib.Rules.SelectStarRule),
            typeof(TSQLLint.Lib.Rules.SemicolonTerminationRule),
            typeof(TSQLLint.Lib.Rules.SetAnsiNullsRule),
            typeof(TSQLLint.Lib.Rules.SetNoCountRule),
            typeof(TSQLLint.Lib.Rules.SetQuotedIdentifierRule),
            typeof(TSQLLint.Lib.Rules.SetTransactionIsolationLevelRule),
            typeof(TSQLLint.Lib.Rules.SetVariableRule),
            typeof(TSQLLint.Lib.Rules.UpperLowerRule),
        };
    }
}
