using CS2Draw.Utils;
using CS2DrawShared;

namespace CS2Draw.Shapes;

/// <summary>
/// Built-in star shape setup. Sets width and height via CP5
/// Star has 10 sides (5 points with inner valleys) so valid CP count = (floor(n / 10) * 10) + 1
/// e.g. 20 particles → 21 CP value (2 per side + closing point)
/// </summary>
internal sealed class StarShapeSetup(float width, float height) : IShapeSetup {
  public string EffectKey => "star";

  public void Configure(IParticleConfigurator cp, int particleCount) {
    var count = ParticleMath.CalcCount(particleCount, sides: 10);
    cp.SetCp(2, count, 0, 0);
    cp.SetCp(5, width, height, 0);
  }
}