namespace CS2DrawShared;

/// <summary>
/// Implement this to register a custom shape with CS2Draw.
/// CS2Draw owns the spawn sequence — you only set control points here.
/// </summary>
public interface IShapeSetup {
  /// <summary>
  /// Key that maps to an effect name in cs2draw.json.
  /// e.g. "my_star" -> "particles/myaddon/star.vpcf"
  /// </summary>
  string EffectKey { get; }

  /// <summary>
  /// Set your control points here. Called after the particle is created
  /// and teleported, before DispatchSpawn and Start.
  /// </summary>
  void Configure(IParticleConfigurator configurator, int particleCount);
}