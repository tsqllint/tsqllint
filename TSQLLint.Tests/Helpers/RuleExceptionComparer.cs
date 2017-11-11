using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Lib.Parser;
using TSQLLint.Lib.Parser.Interfaces;

namespace TSQLLint.Tests.Helpers
{
    [ExcludeFromCodeCoverage]
    public class RuleExceptionComparer : IComparer, IComparer<IRuleException>
    {
        public int Compare(object x, object y)
        {
            if (!(x is IRuleException lhs) || !(y is IRuleException rhs))
            {
                throw new InvalidOperationException("cannot compare null object");
            }

            return Compare(lhs, rhs);
        }

        public int Compare(IRuleException left, IRuleException right)
        {
            if (right != null && left != null && left.StartLine != right.StartLine)
            {
                return -1;
            }

            if (right != null && left != null && left.EndLine != right.EndLine)
            {
                return -1;
            }

            if (right != null && left != null)
            {
                if (left is RuleException lhs && right is RuleException rhs && lhs.RuleType != rhs.RuleType)
                {
                    return -1;
                }
            }

            return 0;
        }
    }
}
