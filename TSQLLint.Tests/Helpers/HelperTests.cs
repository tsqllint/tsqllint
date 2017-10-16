using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.Helpers
{
    public class RuleViolationCompareTests
    {
        private readonly RuleViolationComparer _ruleViolationComparer = new RuleViolationComparer();

        public static readonly object[] LineComparison = 
        {
          new object[] 
              {
                  new List<RuleViolation>
                  {
                      new RuleViolation("some-rule", 99, 0),
                      new RuleViolation("some-rule", 0, 0)
                  }
          },
          new object[] 
          {
              new List<RuleViolation>
              {
                  new RuleViolation("some-rule", 0, 99),
                  new RuleViolation("some-rule", 0, 0)
              }
          },
          new object[] 
          {
              new List<RuleViolation>
              {
                  new RuleViolation("some-rule", 0, 0),
                  new RuleViolation("foo", 0, 0)
              }
          }
        };

        [Test, TestCaseSource(nameof(LineComparison))]
        public void RuleTests(List<RuleViolation> ruleViolations)
        {
            Assert.AreEqual(-1, _ruleViolationComparer.Compare(ruleViolations[0], ruleViolations[1]));
        }

        [Test]
        public void RuleCompareShouldThrow()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var result = _ruleViolationComparer.Compare(new object(), new object());
                Assert.IsNull(result);
            });

            Assert.That(ex.Message, Is.EqualTo("cannot compare null object"));
        }
    }
}
