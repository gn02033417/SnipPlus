namespace SnipPlus.Core;

public enum ResidentActivationDisposition
{
    MainWindowShown,
    IgnoredDuringCapture,
    IgnoredDuringApplicationExit
}

public sealed class ResidentActivationBoundary
{
    private readonly Func<bool> _isApplicationExiting;
    private readonly Func<bool> _isCaptureActive;
    private readonly Action _showMainWindow;

    public ResidentActivationBoundary(
        Func<bool> isApplicationExiting,
        Func<bool> isCaptureActive,
        Action showMainWindow)
    {
        _isApplicationExiting = isApplicationExiting
            ?? throw new ArgumentNullException(nameof(isApplicationExiting));
        _isCaptureActive = isCaptureActive
            ?? throw new ArgumentNullException(nameof(isCaptureActive));
        _showMainWindow = showMainWindow
            ?? throw new ArgumentNullException(nameof(showMainWindow));
    }

    public ResidentActivationDisposition HandleActivation()
    {
        if (_isApplicationExiting())
        {
            return ResidentActivationDisposition.IgnoredDuringApplicationExit;
        }

        if (_isCaptureActive())
        {
            return ResidentActivationDisposition.IgnoredDuringCapture;
        }

        _showMainWindow();
        return ResidentActivationDisposition.MainWindowShown;
    }
}
