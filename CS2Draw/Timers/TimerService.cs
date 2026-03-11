using CounterStrikeSharp.API.Core;
using CS2DrawShared.Timers;

namespace CS2Draw.Timers;

internal sealed class TimerService(BasePlugin plugin) : ITimerService {
  public ILoopTimer CreateLoop(float interval, Action callback)
    => new LoopTimer(interval, callback, plugin);

  public void Delay(float delay, Action callback)
    => plugin.AddTimer(delay, callback);
}