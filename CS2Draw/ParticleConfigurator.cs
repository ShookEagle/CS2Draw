using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared;

namespace CS2Draw;

/// <summary>
/// Concrete IParticleConfigurator backed by CParticleSystem.AcceptInput.
/// Consumers never see this — they only see IParticleConfigurator.
/// </summary>
internal sealed class ParticleConfigurator(CParticleSystem particle)
  : IParticleConfigurator {
  public void SetCp(int index, float x, float y, float z) {
    particle.AcceptInput("SetControlPoint", value: $"{index}: {x} {y} {z}");
  }

  public void SetCp(int index, Vector value) {
    SetCp(index, value.X, value.Y, value.Z);
  }
}