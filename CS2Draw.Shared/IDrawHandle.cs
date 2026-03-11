using CounterStrikeSharp.API.Core;

namespace CS2DrawShared;

/// <summary>
/// Returned from every .Draw() call.
/// Safe to discard if you don't need early cancellation.
/// </summary>
public interface IDrawHandle {
  Guid Id { get; }
  bool IsAlive { get; }
  void Cancel();
  
  /// <summary>
  /// Update a control point on the live particle at any time after spawn.
  /// Use this to drive runtime changes — alpha, color, size — without
  /// needing to cancel and re-spawn.
  /// </summary>
  void SetCp(int cp, float x, float y, float z);
  CParticleSystem? Particle { get; }
}