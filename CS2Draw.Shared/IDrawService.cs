using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared.Builders;

namespace CS2DrawShared;

/// <summary>
/// Resolved via PluginCapability. The only thing consumer plugins need.
/// </summary>
public interface IDrawService {
  // Shapes
  CircleBuilder Circle(Vector origin, float radius);
  RectangleBuilder Rectangle(Vector origin, float width, float height);

  // Beams
  BeamBuilder Beam(Vector from, Vector to);

  // Trails
  TrailBuilder Trail(CBaseEntity anchor);
  
  // Beacons 
  /// <summary>
  /// Start a beacon on a player. Replaces any existing beacon on that player.
  /// Uses team color by default — override with .Color() on the builder.
  /// </summary>
  BeaconBuilder Beacon(CCSPlayerController player);

  /// <summary>Returns true if the player currently has an active beacon.</summary>
  bool HasBeacon(CCSPlayerController player);

  /// <summary>Stop and remove the beacon for a specific player.</summary>
  void RemoveBeacon(CCSPlayerController player);

  /// <summary>Stop and remove all active beacons. Call on round end.</summary>
  void RemoveAllBeacons();

  // Custom shapes

  /// <summary>
  /// Register a custom shape. The effect key must exist in cs2draw.json.
  /// Call this once from your plugin's OnAllPluginsLoaded.
  /// </summary>
  void RegisterShape(IShapeSetup setup);

  /// <summary>Draw a previously registered custom shape by its effect key.</summary>
  CustomBuilder Custom(Vector origin, IShapeSetup setup);

  // Lifecycle 
  void CancelAll();
}