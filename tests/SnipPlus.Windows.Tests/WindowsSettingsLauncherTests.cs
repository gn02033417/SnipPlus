using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsSettingsLauncherTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public async Task KeyboardSettingsUsesTheOfficialSettingsUri()
    {
        Uri? launchedUri = null;
        var launcher = new WindowsSettingsLauncher(uri =>
        {
            launchedUri = uri;
            return Task.FromResult(true);
        });

        var result = await launcher.OpenKeyboardSettingsAsync();

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(SettingsLaunchFailureCode.None, result.FailureCode);
        Assert.AreEqual(WindowsSettingsLauncher.KeyboardSettingsUri, launchedUri?.ToString());
    }

    [TestMethod]
    [TestCategory("Contract")]
    public async Task FailedSettingsLaunchReturnsTypedFailure()
    {
        var launcher = new WindowsSettingsLauncher(_ => Task.FromResult(false));

        var result = await launcher.OpenKeyboardSettingsAsync();

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(SettingsLaunchFailureCode.LaunchFailed, result.FailureCode);
        StringAssert.Contains(result.UserMessage, "keyboard settings");
    }

    [TestMethod]
    [TestCategory("Contract")]
    public async Task SettingsLaunchExceptionReturnsTypedFailure()
    {
        var launcher = new WindowsSettingsLauncher(_ =>
            throw new InvalidOperationException("synthetic launcher failure"));

        var result = await launcher.OpenKeyboardSettingsAsync();

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(SettingsLaunchFailureCode.LaunchFailed, result.FailureCode);
        StringAssert.Contains(result.UserMessage, nameof(InvalidOperationException));
    }
}
