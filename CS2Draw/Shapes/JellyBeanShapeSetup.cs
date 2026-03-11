using CS2Draw.Utils;
using CS2DrawShared;

namespace CS2Draw.Shapes;

/// <summary>
/// Built-in jelly bean shape setup. Sets width and height via CP5
/// Jelly bean uses curved path segments for rounded pill shape
/// Using 22 segments for smooth rounded edges
/// </summary>
internal sealed class JellyBeanShapeSetup(float width, float height)
  : IShapeSetup {
  public string EffectKey => "jellybean";

  public void Configure(IParticleConfigurator cp, int particleCount) {
    var count = ParticleMath.CalcCount(particleCount, sides: 22);
    cp.SetCp(2, count, 0, 0);
    cp.SetCp(5, width, height, 0);
  }
}