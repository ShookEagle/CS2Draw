namespace CS2DrawShared.Timers;

/// <summary>
/// Abstraction over CSS#'s AddTimer so DrawService doesn't need
/// a direct reference to the plugin instance.
/// </summary>
public interface ITimerService {
  /// <summary>
  /// Create a looping timer that fires every <paramref name="interval"/> seconds.
  /// Does not start automatically — call Start() on the returned ILoopTimer.
  /// </summary>
  ILoopTimer CreateLoop(float interval, Action callback);
  
  /// <summary>
  /// Fire a one-shot callback after <paramref name="delay"/> seconds.
  /// Used internally for lifetime expiry.
  /// </summary>
  void Delay(float delay, Action callback);
}