/*
  This file is auto-generated and should not be edited directly.
  All changes should be made to the accompanying template.
*/

using System;
using System.Collections.Generic;
using TSQLLint.Lib.Standard.Rules;

namespace TSQLLint.Lib.Standard.Parser
{
    public static class RuleVisitorTypes
    {
        // a list of all classes that implement ISqlRule
        public static readonly List<Type> List = new List<Type>
        {
            typeof(ConditionalBeginEndRule),
            typeof(CrossDatabaseRule),
            typeof(DataCompressionOptionRule),
            typeof(DataTypeLengthRule),
            typeof(DisallowCursorRule),
            typeof(InformationSchemaRule),
            typeof(KeywordCapitalizationRule),
            typeof(LinkedServerRule),
            typeof(MultiTableAliasRule),
            typeof(NonSargableRule),
            typeof(ObjectPropertyRule),
            typeof(PrintStatementRule),
            typeof(SchemaQualifyRule),
            typeof(SelectStarRule),
            typeof(SemicolonTerminationRule),
            typeof(SetAnsiNullsRule),
            typeof(SetNoCountRule),
            typeof(SetQuotedIdentifierRule),
            typeof(SetTransactionIsolationLevelRule),
            typeof(SetVariableRule),
            typeof(UpperLowerRule),
        };
    }
}
