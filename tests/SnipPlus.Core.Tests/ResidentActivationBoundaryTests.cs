using SnipPlus.Core;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class ResidentActivationBoundaryTests
{
    [TestMethod]
    public void ResidentReadyActivationShowsTheExistingMainWindow()
    {
        var showCount = 0;
        var boundary = new ResidentActivationBoundary(
            isApplicationExiting: () => false,
            isCaptureActive: () => false,
            showMainWindow: () => showCount++);

        var result = boundary.HandleActivation();

        Assert.AreEqual(ResidentActivationDisposition.MainWindowShown, result);
        Assert.AreEqual(1, showCount);
    }

    [TestMethod]
    public void CaptureActivationDoesNotShowOrInterruptTheExistingMainWindow()
    {
        var showCount = 0;
        var boundary = new ResidentActivationBoundary(
            isApplicationExiting: () => false,
            isCaptureActive: () => true,
            showMainWindow: () => showCount++);

        var result = boundary.HandleActivation();

        Assert.AreEqual(ResidentActivationDisposition.IgnoredDuringCapture, result);
        Assert.AreEqual(0, showCount);
    }

    [TestMethod]
    public void ApplicationExitActivationDoesNotShowTheExistingMainWindow()
    {
        var showCount = 0;
        var boundary = new ResidentActivationBoundary(
            isApplicationExiting: () => true,
            isCaptureActive: () => false,
            showMainWindow: () => showCount++);

        var result = boundary.HandleActivation();

        Assert.AreEqual(ResidentActivationDisposition.IgnoredDuringApplicationExit, result);
        Assert.AreEqual(0, showCount);
    }
}
