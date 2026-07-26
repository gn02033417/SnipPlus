using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class BaselineTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public void BaselineIsAvailable()
    {
        Assert.AreEqual("baseline", Core.Baseline.Status);
    }
}
