using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class BaselineTests
{
    [TestMethod]
    [TestCategory("Rendering")]
    public void BaselineIsAvailable()
    {
        Assert.AreEqual("baseline", Windows.Baseline.Status);
    }
}
