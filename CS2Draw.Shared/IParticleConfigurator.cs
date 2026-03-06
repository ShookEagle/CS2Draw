using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared;

/// <summary>
/// Passed to IShapeSetup.Configure() — clean wrapper around control point inputs.
/// Consumers never touch CParticleSystem directly.
/// </summary>
public interface IParticleConfigurator
{
  /// <summary>Set a control point by individual components.</summary>
  void SetCp(int index, float x, float y, float z);

  /// <summary>Set a control point from a Vector.</summary>
  void SetCp(int index, Vector value);
}