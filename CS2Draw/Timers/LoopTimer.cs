using CounterStrikeSharp.API.Core;
using CS2DrawShared.Timers;

namespace CS2Draw.Timers;

/// <summary>
/// Wraps CSS#'s AddTimer into a start/stop loop.
/// Each tick schedules the next tick — stopping breaks the chain.
/// </summary>
internal sealed class LoopTimer(float interval, Action callback,
  BasePlugin plugin) : ILoopTimer {
  public bool IsRunning { get; private set; }

  public void Start() {
    if (IsRunning) return;
    IsRunning = true;
    callback();
    schedule();
  }

  public void Stop() { IsRunning = false; }

  private void schedule() {
    plugin.AddTimer(interval, () => {
      if (!IsRunning) return;
      callback();
      schedule(); // reschedule next tick
    });
  }
}