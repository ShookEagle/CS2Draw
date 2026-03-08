using CounterStrikeSharp.API.Core;
using CS2DrawShared;

namespace CS2Draw;

public class DrawHandle : IDrawHandle {
  private readonly CParticleSystem particle;
  private readonly Action<DrawHandle> onCancel;

  public Guid Id      { get; } = Guid.NewGuid();
  public bool IsAlive { get; private set; } = true;

  internal DrawHandle(CParticleSystem particle, Action<DrawHandle> onCancel)
  {
    this.particle = particle;
    this.onCancel = onCancel;
  }

  public void Cancel()
  {
    if (!IsAlive) return;
    IsAlive = false;

    // Stop and remove the particle from the world
    particle.AcceptInput("Stop");
    particle.Remove();

    onCancel(this);
  }

  // TODO: lifetime management — investigate non-CP based approach
  // Options being explored:
  //   - Server-side timer that calls Cancel() after n seconds
  //   - Hooking particle death events if exposed by CSS#
}