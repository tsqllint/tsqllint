using System;
using System.Collections.Generic;
using NUnit.Framework;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.Helpers
{
    public class RuleViolationCompareTests
    {
        public static readonly object[] LineComparison = 
        {
          new object[] 
          {
              new List<RuleViolation>
              {
                  new RuleViolation(ruleName: "some-rule", startLine: 99, startColumn: 0),
                  new RuleViolation(ruleName: "some-rule", startLine: 0, startColumn: 0)
              }
          },
          new object[] 
          {
              new List<RuleViolation>
              {
                  new RuleViolation(ruleName: "some-rule", startLine: 0, startColumn: 99),
                  new RuleViolation(ruleName: "some-rule", startLine: 0, startColumn: 0)
              }
          },
          new object[]
          {
              new List<RuleViolation>
              {
                  new RuleViolation(ruleName: "some-rule", startLine: 0, startColumn: 0),
                  new RuleViolation(ruleName: "foo", startLine: 0, startColumn: 0)
              }
          }
        };

        private readonly RuleViolationCompare _ruleViolationCompare = new RuleViolationCompare();

        [Test, TestCaseSource("LineComparison")]
        public void RuleTests(List<RuleViolation> ruleViolations)
        {
            Assert.AreEqual(-1, _ruleViolationCompare.Compare(ruleViolations[0], ruleViolations[1]));
        }

        [Test]
        public void RuleCompareShouldThrow()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => 
                _ruleViolationCompare.Compare(new object(), new object()));

            Assert.That(ex.Message, Is.EqualTo("cannot compare null object"));
        }
    }
}
