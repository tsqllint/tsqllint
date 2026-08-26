using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using TSQLLint.Infrastructure.Plugins;

namespace TSQLLint.Tests.UnitTests.PluginHandler
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class VersionInfoWrapperTests
    {
        [Test]
        public void GetVersion_ReturnsFileVersionForAssembly()
        {
            var wrapper = new VersionInfoWrapper();

            // use an assembly that is guaranteed to be on disk with a file version
            var assembly = typeof(VersionInfoWrapper).Assembly;

            var version = wrapper.GetVersion(assembly);

            Assert.That(version, Is.Not.Null);
            Assert.That(version, Is.Not.Empty);
        }
    }
}
