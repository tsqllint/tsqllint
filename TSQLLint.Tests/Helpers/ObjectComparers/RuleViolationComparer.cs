using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.Helpers.ObjectComparers
{
    [ExcludeFromCodeCoverage]
    public class RuleViolationComparer : IComparer, IComparer<RuleViolation>
    {
        public int Compare(object x, object y)
        {
            if (!(x is RuleViolation lhs) || !(y is RuleViolation rhs))
            {
                throw new InvalidOperationException("cannot compare null object");
            }

            return Compare(lhs, rhs);
        }

        public int Compare(RuleViolation left, RuleViolation right)
        {
            if (right != null && left != null && left.Line != right.Line)
            {
                return -1;
            }

            if (right != null && left != null && left.Column != right.Column)
            {
                return -1;
            }

            if (right != null && left != null && left.RuleName != right.RuleName)
            {
                return -1;
            }

            return 0;
        }
    }
}
