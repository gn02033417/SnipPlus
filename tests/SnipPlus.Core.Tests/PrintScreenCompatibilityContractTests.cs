using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class PrintScreenCompatibilityContractTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void CompatibilityNoticeAndSettingsCommandAreDefined()
    {
        StringAssert.Contains(PrintScreenTakeoverCompatibility.Notice, "PrintScreen");
        StringAssert.Contains(PrintScreenTakeoverCompatibility.Notice, "Windows");
        StringAssert.Contains(
            PrintScreenTakeoverCompatibility.Notice,
            "使用 Print Screen 鍵開啟螢幕擷取");
        StringAssert.StartsWith(
            PrintScreenTakeoverCompatibility.OpenKeyboardSettingsLabel,
            "開啟");
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void RegisteredStatusExplainsTheWindowsCompatibilityBoundary()
    {
        StringAssert.Contains(PrintScreenTakeoverCompatibility.RegisteredStatus, "registered");
        StringAssert.Contains(PrintScreenTakeoverCompatibility.RegisteredStatus, "may conflict");
    }
}
