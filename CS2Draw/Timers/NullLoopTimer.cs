using CS2DrawShared.Timers;

namespace CS2Draw.Timers;

/// <summary>
/// Returned when a looping spawn fails (e.g. missing effect key in config).
/// Prevents null reference exceptions on the caller side.
/// </summary>
internal sealed class NullLoopTimer : ILoopTimer {
  public static readonly NullLoopTimer INSTANCE = new();

  public bool IsRunning => false;
  public void Start() { }
  public void Stop() { }
}