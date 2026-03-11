using CS2Draw.Utils;
using CS2DrawShared;

namespace CS2Draw.Shapes;

/// <summary>
/// Built-in hexagon shape setup. Sets width and height via CP5
/// Hexagon has 6 sides so valid CP count = (floor(n / 6) * 6) + 1
/// e.g. 18 particles → 19 CP value (3 per side + closing point)
/// </summary>
internal sealed class HexagonShapeSetup(float width, float height)
  : IShapeSetup {
  public string EffectKey => "hexagon";

  public void Configure(IParticleConfigurator cp, int particleCount) {
    var count = ParticleMath.CalcCount(particleCount, sides: 6);
    cp.SetCp(2, count, 0, 0);
    cp.SetCp(5, width, height, 0);
  }
}