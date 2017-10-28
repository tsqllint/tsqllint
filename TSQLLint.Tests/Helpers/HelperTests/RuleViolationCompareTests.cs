using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.Helpers.HelperTests
{
    public class RuleViolationCompareTests
    {
        private readonly RuleViolationComparer _ruleViolationComparer = new RuleViolationComparer();

        public static readonly object[] EquilaventRuleViolations =
        {
            new object[]
            {
                new List<RuleViolation>
                {
                    new RuleViolation("some-rule", 0, 1),
                    new RuleViolation("some-rule", 0, 1)
                }
            }
        };

        [Test, TestCaseSource(nameof(EquilaventRuleViolations))]
        public void CompareEquilaventRulesTest(List<RuleViolation> ruleViolations)
        {
            Assert.AreEqual(0, _ruleViolationComparer.Compare(ruleViolations[0], ruleViolations[1]));
        }
        
        public static readonly object[] NonEquilaventRuleViolations = 
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
                    new RuleViolation("some-rule", 0, 0),
                    new RuleViolation("some-rule", 0, 99)
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

        [Test, TestCaseSource(nameof(NonEquilaventRuleViolations))]
        public void CompareNonEquilaventRulesTest(List<RuleViolation> ruleViolations)
        {
            Assert.AreEqual(-1, _ruleViolationComparer.Compare(ruleViolations[0], ruleViolations[1]));
        }

        [Test]
        [ExcludeFromCodeCoverage]
        public void CompareRulesShouldThrow()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _ruleViolationComparer.Compare(new object(), new object());
            });

            Assert.That(ex.Message, Is.EqualTo("cannot compare null object"));
        }
    }
}
