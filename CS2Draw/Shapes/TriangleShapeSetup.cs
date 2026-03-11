using CS2Draw.Utils;
using CS2DrawShared;

namespace CS2Draw.Shapes;

/// <summary>
/// Built-in triangle shape setup. Sets width and height via CP5
/// Triangle has 3 sides so valid CP count = (floor(n / 3) * 3) + 1
/// e.g. 15 particles → 16 CP value (5 per side + closing point)
/// </summary>
internal sealed class TriangleShapeSetup(float width, float height)
  : IShapeSetup {
  public string EffectKey => "triangle";

  public void Configure(IParticleConfigurator cp, int particleCount) {
    var count = ParticleMath.CalcCount(particleCount, sides: 3);
    cp.SetCp(2, count, 0, 0);
    cp.SetCp(5, width, height, 0);
  }
}