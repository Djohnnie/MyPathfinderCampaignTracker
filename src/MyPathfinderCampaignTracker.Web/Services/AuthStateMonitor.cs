namespace MyPathfinderCampaignTracker.Web.Services;

/// <summary>
/// Scoped service that signals when a 401 with a stale token is detected.
/// Components subscribe to <see cref="OnStaleToken"/> and navigate to sign-out
/// only when they are in interactive rendering mode (i.e., RendererInfo.IsInteractive).
/// </summary>
public class AuthStateMonitor
{
    private bool _staleTokenReported;

    public event Action? OnStaleToken;

    /// <summary>
    /// Called by <see cref="ApiClient"/> when a request returns 401 and a token was sent.
    /// Fires <see cref="OnStaleToken"/> at most once per circuit scope.
    /// </summary>
    public void ReportStaleToken()
    {
        if (_staleTokenReported) return;
        _staleTokenReported = true;
        OnStaleToken?.Invoke();
    }
}
