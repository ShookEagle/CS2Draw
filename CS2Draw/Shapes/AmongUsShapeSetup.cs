using CS2Draw.Utils;
using CS2DrawShared;

namespace CS2Draw.Shapes;

/// <summary>
/// Built-in Among Us crewmate shape setup. Sets width and height via CP5
/// Among Us shape uses multiple segments to approximate the character silhouette
/// Using 16 segments for body, visor, and backpack outline
/// </summary>
internal sealed class AmongUsShapeSetup(float width, float height)
  : IShapeSetup {
  public string EffectKey => "amongus";

  public void Configure(IParticleConfigurator cp, int particleCount) {
    var count = ParticleMath.CalcCount(particleCount, sides: 16);
    cp.SetCp(2, count, 0, 0);
    cp.SetCp(5, width, height, 0);
  }
}