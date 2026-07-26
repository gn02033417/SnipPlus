using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsGraphicsCapturePlatformTests
{
    [TestMethod]
    [TestCategory("Platform")]
    [TestCategory("Capture")]
    [TestCategory("Interactive")]
    public void WindowsGraphicsCaptureSupportIsObservable()
    {
        Assert.IsTrue(
            WindowsGraphicsCaptureAdapter.IsSupported,
            "Windows.Graphics.Capture is unavailable on this Windows baseline.");
    }
}
