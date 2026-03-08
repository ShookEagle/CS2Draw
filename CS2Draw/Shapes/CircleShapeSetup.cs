using CS2DrawShared;

namespace CS2Draw.Shapes;

/// <summary>
/// Built-in circle shape setup. Sets radius via CP5.
/// Circle uses a different draw method in the CS2 PET so particle
/// count math does not apply — count is passed through as-is.
/// </summary>
internal sealed class CircleShapeSetup(float radius, int particles)
  : IShapeSetup {
  public string EffectKey => "circle";

  public void Configure(IParticleConfigurator cp, int particleCount) {
    cp.SetCp(2, particleCount, 0, 0);
    cp.SetCp(5, radius, 0, 0);
  }
}