using CounterStrikeSharp.API.Core;
using CS2DrawShared;

namespace CS2Draw;

/// <summary>
/// Returned when a CustomTrail spawn fails.
/// All operations are no-ops.
/// </summary>
public sealed class NullTrailHandle : ITrailHandle
{
  public static readonly NullTrailHandle INSTANCE = new();
 
  public bool IsRunning => false;
  public void Stop()                                    { }
  public void SetCp(int cp, float x, float y, float z) { }
  public void SetParent(CBaseEntity anchor)             { }
}