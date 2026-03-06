using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared.Builders;

namespace CS2DrawShared;

/// <summary>
/// Resolved via PluginCapability. The only thing consumer plugins need.
/// </summary>
public interface IDrawService
{
  // Built-in shapes 
  CircleBuilder    Circle(Vector origin, float radius);
  RectangleBuilder Rectangle(Vector origin, float width, float height);

  // Custom shapes

  /// <summary>
  /// Register a custom shape. The effect key must exist in cs2draw.json.
  /// Call this once from your plugin's OnAllPluginsLoaded.
  /// </summary>
  void RegisterShape(IShapeSetup setup);

  /// <summary>Draw a previously registered custom shape by its effect key.</summary>
  CustomBuilder Custom(Vector origin, IShapeSetup setup);

  // ── Lifecycle ─────────────────────────────────────────────────────────────
  void CancelAll();
}