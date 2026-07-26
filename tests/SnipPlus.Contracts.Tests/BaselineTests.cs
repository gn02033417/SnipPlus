using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SnipPlus.Contracts.Tests;

[TestClass]
public sealed class BaselineTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void BaselineIsAvailable()
    {
        Assert.AreEqual("baseline", Contracts.Baseline.Status);
    }
}
