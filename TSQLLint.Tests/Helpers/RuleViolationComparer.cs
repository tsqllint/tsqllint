using System;
using System.Collections;
using System.Collections.Generic;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.Helpers
{
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

        public int Compare(RuleViolation x, RuleViolation y)
        {
            if (y != null && x != null && x.Line != y.Line)
            {
                return -1;
            }

            if (y != null && x != null && x.Column != y.Column)
            {
                return -1;
            }

            if (y != null && x != null && x.RuleName != y.RuleName)
            {
                return -1;
            }

            return 0;
        }
    }
}
