using CS2Draw.Utils;
using CS2DrawShared;

namespace CS2Draw.Shapes;

/// <summary>
/// Built-in rectangle shape setup. Sets width and height via CP5
/// Rectangle has 4 sides so valid CP count = (floor(n / 4) * 4) + 1
/// e.g. 16 particles → 17 CP value (4 per side + closing point)
/// </summary>
internal sealed class RectangleShapeSetup(float width, float height)
  : IShapeSetup {
  public string EffectKey => "rectangle";

  public void Configure(IParticleConfigurator cp, int particleCount) {
    var count = ParticleMath.CalcCount(particleCount, sides: 4);
    cp.SetCp(2, count, 0, 0);
    cp.SetCp(5, width, height, 0);
  }
}