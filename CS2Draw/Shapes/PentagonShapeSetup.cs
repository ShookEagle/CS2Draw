using CS2Draw.Utils;
using CS2DrawShared;

namespace CS2Draw.Shapes;

/// <summary>
/// Built-in pentagon shape setup. Sets width and height via CP5
/// Pentagon has 5 sides so valid CP count = (floor(n / 5) * 5) + 1
/// e.g. 20 particles → 21 CP value (4 per side + closing point)
/// </summary>
internal sealed class PentagonShapeSetup(float width, float height)
  : IShapeSetup {
  public string EffectKey => "pentagon";

  public void Configure(IParticleConfigurator cp, int particleCount) {
    var count = ParticleMath.CalcCount(particleCount, sides: 5);
    cp.SetCp(2, count, 0, 0);
    cp.SetCp(5, width, height, 0);
  }
}