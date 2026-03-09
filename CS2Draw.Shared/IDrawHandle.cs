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
  CParticleSystem? Particle { get; }
}