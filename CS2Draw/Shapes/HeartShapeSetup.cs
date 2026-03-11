using CS2Draw.Utils;
using CS2DrawShared;

namespace CS2Draw.Shapes;

/// <summary>
/// Built-in heart shape setup. Sets width and height via CP5
/// Heart uses curved path segments, approximated with multiple points
/// Using 21 segments for smooth curves
/// </summary>
internal sealed class HeartShapeSetup(float width, float height) : IShapeSetup {
  public string EffectKey => "heart";

  public void Configure(IParticleConfigurator cp, int particleCount) {
    var count = ParticleMath.CalcCount(particleCount, sides: 21);
    cp.SetCp(2, count, 0, 0);
    cp.SetCp(5, width, height, 0);
  }
}