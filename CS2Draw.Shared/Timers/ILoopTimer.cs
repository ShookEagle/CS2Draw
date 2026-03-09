namespace CS2DrawShared.Timers;

/// <summary>
/// A repeating timer that can be started and stopped.
/// Returned by trail and beacon draw calls so the caller can stop the loop.
/// </summary>
public interface ILoopTimer {
  bool IsRunning { get; }
  void Start();
  void Stop();
}