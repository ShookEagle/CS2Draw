using CounterStrikeSharp.API.Core;
using CS2DrawShared;
using CS2DrawShared.Timers;

namespace CS2Draw;

/// <summary>
/// Manages a looping custom trail — spawns a particle via IShapeSetup
/// every interval, parented to the current anchor.
/// SetParent() swaps the anchor mid-loop without stopping emission.
/// </summary>
internal sealed class CustomTrailHandle : ITrailHandle
{
  private readonly ILoopTimer timer;
 
  // Mutable anchor — swapped by SetParent()
  private CBaseEntity anchor;
 
  // Mutable CP state — applied to each newly spawned particle
  private readonly Dictionary<int, (float x, float y, float z)> cpOverrides = new();
 
  public bool IsRunning => timer.IsRunning;
 
  internal CustomTrailHandle(ILoopTimer timer, CBaseEntity anchor)
  {
    this.timer  = timer;
    this.anchor = anchor;
  }
 
  public void Stop() => timer.Stop();
 
  /// <summary>
  /// Store a CP override — applied to every particle spawned from next tick onward.
  /// </summary>
  public void SetCp(int cp, float x, float y, float z)
    => cpOverrides[cp] = (x, y, z);
 
  /// <summary>
  /// Swap the anchor. The loop continues but new particles are parented
  /// to the new entity. Existing particles are unaffected.
  /// </summary>
  public void SetParent(CBaseEntity anchor)
    => this.anchor = anchor;
 
  /// <summary>
  /// Called by DrawService each tick to get the current anchor and CP state.
  /// </summary>
  internal CBaseEntity CurrentAnchor => anchor;
 
  internal IReadOnlyDictionary<int, (float x, float y, float z)> CPOverrides
    => cpOverrides;
}