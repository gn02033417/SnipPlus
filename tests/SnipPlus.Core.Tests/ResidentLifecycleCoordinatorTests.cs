using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class ResidentLifecycleCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public void DefaultSettingLoadsDisabledWithoutRegistration()
    {
        var takeover = new FakePrintScreenTakeover();
        var settings = new MemorySettingsStore();
        var lifecycle = new ResidentLifecycleCoordinator(takeover, settings);

        var result = lifecycle.Initialize();

        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(lifecycle.IsTakeoverEnabled);
        Assert.AreEqual(0, takeover.RegisterCalls);
        Assert.IsFalse(settings.Enabled);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void SavedEnabledSettingRegistersOnceOnInitialize()
    {
        var takeover = new FakePrintScreenTakeover();
        var settings = new MemorySettingsStore { Enabled = true };
        var lifecycle = new ResidentLifecycleCoordinator(takeover, settings);

        var result = lifecycle.Initialize();

        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(lifecycle.IsTakeoverEnabled);
        Assert.AreEqual(1, takeover.RegisterCalls);
        Assert.IsTrue(settings.Enabled);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void EnablingTwiceRegistersOnce()
    {
        var takeover = new FakePrintScreenTakeover();
        var lifecycle = new ResidentLifecycleCoordinator(takeover, new MemorySettingsStore());

        Assert.IsTrue(lifecycle.SetTakeoverEnabled(true).IsSuccess);
        Assert.IsTrue(lifecycle.SetTakeoverEnabled(true).IsSuccess);

        Assert.AreEqual(1, takeover.RegisterCalls);
        Assert.IsTrue(lifecycle.IsTakeoverEnabled);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void DisablingAfterEnableUnregistersOnce()
    {
        var takeover = new FakePrintScreenTakeover();
        var lifecycle = new ResidentLifecycleCoordinator(takeover, new MemorySettingsStore());
        lifecycle.SetTakeoverEnabled(true);

        var result = lifecycle.SetTakeoverEnabled(false);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(lifecycle.IsTakeoverEnabled);
        Assert.AreEqual(1, takeover.UnregisterCalls);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void RepeatedDisableIsSafe()
    {
        var takeover = new FakePrintScreenTakeover();
        var lifecycle = new ResidentLifecycleCoordinator(takeover, new MemorySettingsStore());

        Assert.IsTrue(lifecycle.SetTakeoverEnabled(false).IsSuccess);
        Assert.IsTrue(lifecycle.SetTakeoverEnabled(false).IsSuccess);

        Assert.AreEqual(0, takeover.UnregisterCalls);
        Assert.IsFalse(lifecycle.IsTakeoverEnabled);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void RegistrationFailureDoesNotLeaveEnabledState()
    {
        var takeover = new FakePrintScreenTakeover { RegistrationSucceeds = false };
        var settings = new MemorySettingsStore { Enabled = true };
        var lifecycle = new ResidentLifecycleCoordinator(takeover, settings);

        var result = lifecycle.Initialize();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsFalse(result.IsRegistered);
        Assert.IsFalse(lifecycle.IsTakeoverEnabled);
        Assert.IsFalse(settings.Enabled);
        Assert.AreEqual(PrintScreenTakeoverFailureCode.RegistrationFailed, result.FailureCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void ApplicationExitReleasesRegisteredTakeover()
    {
        var takeover = new FakePrintScreenTakeover();
        var settings = new MemorySettingsStore { Enabled = true };
        var lifecycle = new ResidentLifecycleCoordinator(takeover, settings);
        lifecycle.Initialize();

        var result = lifecycle.ExitApplication();

        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(lifecycle.IsTakeoverEnabled);
        Assert.AreEqual(1, takeover.UnregisterCalls);
        Assert.AreEqual(1, takeover.DisposeCalls);
        Assert.IsTrue(settings.Enabled);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void DisabledApplicationExitDoesNotProduceAnError()
    {
        var takeover = new FakePrintScreenTakeover();
        var lifecycle = new ResidentLifecycleCoordinator(takeover, new MemorySettingsStore());
        lifecycle.Initialize();

        var result = lifecycle.ExitApplication();

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(0, takeover.UnregisterCalls);
        Assert.AreEqual(1, takeover.DisposeCalls);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void DisposeAndExitAreIdempotent()
    {
        var takeover = new FakePrintScreenTakeover();
        var lifecycle = new ResidentLifecycleCoordinator(takeover, new MemorySettingsStore());
        lifecycle.SetTakeoverEnabled(true);

        lifecycle.Dispose();
        var secondExit = lifecycle.ExitApplication();
        lifecycle.Dispose();

        Assert.IsTrue(secondExit.IsSuccess);
        Assert.AreEqual(1, takeover.UnregisterCalls);
        Assert.AreEqual(1, takeover.DisposeCalls);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void PrintScreenIsForwardedOnlyToTheApplicationEventBoundary()
    {
        var takeover = new FakePrintScreenTakeover();
        var lifecycle = new ResidentLifecycleCoordinator(
            takeover,
            new MemorySettingsStore { Enabled = true });
        lifecycle.Initialize();
        var received = 0;
        lifecycle.PrintScreenReceived += (_, _) => received++;

        takeover.RaisePrintScreen();

        Assert.AreEqual(1, received);
    }

    private sealed class MemorySettingsStore : IPrintScreenTakeoverSettingsStore
    {
        public bool Enabled { get; set; }

        public bool LoadEnabled() => Enabled;

        public void SaveEnabled(bool enabled) => Enabled = enabled;
    }

    private sealed class FakePrintScreenTakeover : IPrintScreenTakeover
    {
        public bool RegistrationSucceeds { get; init; } = true;

        public bool IsRegistered { get; private set; }

        public int RegisterCalls { get; private set; }

        public int UnregisterCalls { get; private set; }

        public int DisposeCalls { get; private set; }

        public event EventHandler<PrintScreenReceivedEventArgs>? PrintScreenReceived;

        public PrintScreenTakeoverResult Register()
        {
            RegisterCalls++;
            if (!RegistrationSucceeds)
            {
                return PrintScreenTakeoverResult.Failed(
                    PrintScreenTakeoverFailureCode.RegistrationFailed,
                    false,
                    "Synthetic registration failure.");
            }

            IsRegistered = true;
            return PrintScreenTakeoverResult.Enabled(true);
        }

        public PrintScreenTakeoverResult Unregister()
        {
            if (!IsRegistered)
            {
                return PrintScreenTakeoverResult.Disabled(false);
            }

            UnregisterCalls++;
            IsRegistered = false;
            return PrintScreenTakeoverResult.Disabled(true);
        }

        public void RaisePrintScreen()
        {
            PrintScreenReceived?.Invoke(
                this,
                new PrintScreenReceivedEventArgs(Guid.NewGuid(), DateTimeOffset.UnixEpoch));
        }

        public void Dispose()
        {
            DisposeCalls++;
            IsRegistered = false;
        }
    }
}
