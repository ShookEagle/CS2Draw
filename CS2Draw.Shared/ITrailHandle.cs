using CounterStrikeSharp.API.Core;

namespace CS2DrawShared;

/// <summary>
/// Returned from CustomTrail().Start().
/// Represents a looping trail attached to a CBaseEntity.
/// </summary>
public interface ITrailHandle
{
  bool IsRunning { get; }
 
  /// <summary>Stop the loop. Existing particles fade naturally.</summary>
  void Stop();
 
  /// <summary>
  /// Update a control point applied to every particle spawned by this trail
  /// from the next tick onward.
  /// </summary>
  void SetCp(int cp, float x, float y, float z);
 
  /// <summary>
  /// Re-parent the trail to a different entity.
  /// Use this to detach from a moving pawn and attach to a static anchor,
  /// freezing the trail in place without removing existing particles.
  /// </summary>
  void SetParent(CBaseEntity anchor);
}