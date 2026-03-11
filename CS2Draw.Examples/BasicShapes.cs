// ┌─────────────────────────────────────────────────────────────────────────┐
// │  Example 01 — Basic Shapes                                              │
// │  Shows: Circle and Rectangle, minimal API, fire and forget              │
// └─────────────────────────────────────────────────────────────────────────┘
using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared;

namespace CS2Draw.Examples;

public class BasicShapes : BasePlugin {
  public override string ModuleName => "CS2Draw Example 01 — Basic Shapes";
  public override string ModuleVersion => "1.0.0";

  public override string ModuleDescription
    => "Draws a circle and a rectangle at the player's position.";

  private static readonly PluginCapability<IDrawService> DRAW_CAPABILITY =
    new("cs2draw:service");

  private IDrawService? draw;

  public override void OnAllPluginsLoaded(bool hotReload) {
    draw = DRAW_CAPABILITY.Get();
    if (draw == null) {
      Console.WriteLine("[Example01] CS2Draw not loaded.");
      return;
    }

    AddCommand("css_ex1_circle", "Draw a circle", OnCircle);
    AddCommand("css_ex1_rect", "Draw a rectangle", OnRect);
  }

  // Draws a red circle at the player's feet.
  // No handle stored — fire and forget.
  // WithLifetime(10f) means CS2Draw cancels it automatically after 10 seconds.
  private void OnCircle(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetOrigin(player, out var pos)) return;

    draw!.Circle(pos, 100f)
     .Particles(16)
     .Color(Color.Red)
     .WithLifetime(10f)
     .Draw();

    player!.PrintToChat("[BasicShapes] Circle drawn — disappears in 10s.");
  }

  // Draws a blue rectangle at the player's feet.
  // Width 200, height 100 world units.
  private void OnRect(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetOrigin(player, out var pos)) return;

    draw!.Rectangle(pos, 200f, 100f)
     .Particles(16)
     .Color(Color.Blue)
     .WithLifetime(10f)
     .Draw();

    player!.PrintToChat("[BasicShapes] Rectangle drawn — disappears in 10s.");
  }

  private static bool
    tryGetOrigin(CCSPlayerController? player, out Vector pos) {
    pos = Vector.Zero;
    if (player == null || !player.IsValid
      || player.Pawn.Value?.AbsOrigin == null)
      return false;
    pos = player.Pawn.Value.AbsOrigin;
    return true;
  }
}