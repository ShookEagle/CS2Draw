using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CS2DrawShared;

namespace CS2Draw;

public class DrawHandle : IDrawHandle {
  private readonly Action<DrawHandle> onCancel;

  public Guid Id { get; } = Guid.NewGuid();
  public bool IsAlive { get; private set; } = true;

  internal DrawHandle(CParticleSystem particle, Action<DrawHandle> onCancel) {
    Particle = particle;
    this.onCancel = onCancel;
  }

  public void Cancel() {
    if (!IsAlive) return;
    IsAlive = false;

    Particle.AcceptInput("Stop");
    Particle.AcceptInput("DestroyImmediately");
    Server.RunOnTick(Server.TickCount + 16, () => {
      Particle.Remove();
    });

    onCancel(this);
  }
  
  public void SetCp(int cp, float x, float y, float z)
  {
    if (!IsAlive) return;
    Particle.AcceptInput("SetControlPoint", value: $"{cp}: {x} {y} {z}");
  }

  public CParticleSystem Particle { get; }

  // TODO: lifetime management — investigate CP based approach
  // Options being explored:
  //   - Possibly Look into EndCap effects to allow particles to have a smooth end or fade out
  //   - Setting LifeTime within a particle w/o affecting particle age effects
}