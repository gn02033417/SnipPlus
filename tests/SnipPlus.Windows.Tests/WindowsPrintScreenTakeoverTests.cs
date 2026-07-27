using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsPrintScreenTakeoverTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void InvalidWindowHandleReturnsUnderstandableRegistrationFailure()
    {
        using var takeover = new WindowsPrintScreenTakeover(nint.Zero);

        var result = takeover.Register();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsFalse(result.IsRegistered);
        Assert.AreEqual(PrintScreenTakeoverFailureCode.InvalidWindowHandle, result.FailureCode);
        StringAssert.Contains(result.UserMessage, "window handle");
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void UnregisterAndDisposeAreSafeBeforeRegistration()
    {
        var takeover = new WindowsPrintScreenTakeover(nint.Zero);

        var firstUnregister = takeover.Unregister();
        takeover.Dispose();
        takeover.Dispose();
        var secondUnregister = takeover.Unregister();

        Assert.IsTrue(firstUnregister.IsSuccess);
        Assert.IsTrue(secondUnregister.IsSuccess);
        Assert.IsFalse(secondUnregister.IsRegistered);
    }
}
