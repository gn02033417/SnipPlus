namespace SnipPlus.Windows;

public readonly record struct ClipboardRetryDecision(bool ShouldRetry, TimeSpan Delay);

public static class ClipboardRetryPolicy
{
    public static ClipboardRetryDecision Decide(
        int attemptsCompleted,
        int maximumAttempts,
        TimeSpan elapsed,
        TimeSpan retryBudget)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(attemptsCompleted, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumAttempts, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(retryBudget, TimeSpan.Zero);

        var remaining = retryBudget - elapsed;
        if (attemptsCompleted >= maximumAttempts || remaining <= TimeSpan.Zero)
        {
            return new ClipboardRetryDecision(false, TimeSpan.Zero);
        }

        var delayMilliseconds = Math.Min(400, 25 * (1 << Math.Min(attemptsCompleted - 1, 4)));
        var delay = TimeSpan.FromMilliseconds(delayMilliseconds);
        return delay < remaining
            ? new ClipboardRetryDecision(true, delay)
            : new ClipboardRetryDecision(false, TimeSpan.Zero);
    }
}
