using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class ClipboardRetryPolicyTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void RetryDelayIsBoundedAndExponential()
    {
        var first = ClipboardRetryPolicy.Decide(1, 5, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        var second = ClipboardRetryPolicy.Decide(2, 5, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        var fifth = ClipboardRetryPolicy.Decide(5, 5, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        Assert.IsTrue(first.ShouldRetry);
        Assert.AreEqual(TimeSpan.FromMilliseconds(25), first.Delay);
        Assert.AreEqual(TimeSpan.FromMilliseconds(50), second.Delay);
        Assert.IsFalse(fifth.ShouldRetry);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void RetryStopsAtBudgetBoundary()
    {
        var decision = ClipboardRetryPolicy.Decide(
            2,
            5,
            TimeSpan.FromMilliseconds(980),
            TimeSpan.FromSeconds(1));

        Assert.IsFalse(decision.ShouldRetry);
        Assert.AreEqual(TimeSpan.Zero, decision.Delay);
    }
}
