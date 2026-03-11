// ┌─────────────────────────────────────────────────────────────────────────┐
// │  Example 02 — Shape Lifecycle                                           │
// │  Shows: IDrawHandle, Infinite(), early Cancel(), IsAlive, CancelAll()  │
// └─────────────────────────────────────────────────────────────────────────┘

using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared;

namespace CS2Draw.Examples;

public sealed class ShapeLifecycle : BasePlugin {
  public override string ModuleName => "CS2Draw Example 02 — Shape Lifecycle";
  public override string ModuleVersion => "1.0.0";
  public override string ModuleAuthor => "";

  public override string ModuleDescription
    => "Demonstrates IDrawHandle — infinite shapes, early cancel, and bulk cancel.";

  private static readonly PluginCapability<IDrawService> DRAW_CAPABILITY =
    new("cs2draw:service");

  private IDrawService? draw;

  // Stored handles — these persist until explicitly canceled
  private IDrawHandle? circleHandle;
  private IDrawHandle? rectHandle;

  public override void OnAllPluginsLoaded(bool hotReload) {
    draw = DRAW_CAPABILITY.Get();
    if (draw == null) {
      Console.WriteLine("[Example02] CS2Draw not loaded.");
      return;
    }

    AddCommand("css_ex2_spawn", "Spawn two infinite shapes", OnSpawn);
    AddCommand("css_ex2_cancel", "Cancel the circle only", OnCancelCircle);
    AddCommand("css_ex2_status", "Check handle status", OnStatus);
    AddCommand("css_ex2_cancelall", "Cancel everything", OnCancelAll);
  }

  // Spawns a circle and a rectangle, both infinite.
  // Handles are stored so we can cancel them later.
  private void OnSpawn(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetOrigin(player, out var pos)) return;

    // Infinite() is the default — shown here explicitly for clarity
    circleHandle = draw!.Circle(pos, 100f).Color(Color.Green).Infinite().Draw();

    rectHandle = draw.Rectangle(pos, 200f, 100f)
     .Color(Color.Yellow)
     .Infinite()
     .Draw();

    player!.PrintToChat($"[Example02] Spawned. Circle ID: {circleHandle.Id}");
  }

  // Cancels only the circle — rectangle stays alive.
  // Demonstrates per-handle control.
  private void OnCancelCircle(CCSPlayerController? player, CommandInfo info) {
    if (circleHandle is not { IsAlive: true }) {
      player!.PrintToChat("[Example02] No active circle to cancel.");
      return;
    }

    circleHandle.Cancel();
    player!.PrintToChat("[Example02] Circle cancelled. Rectangle still alive.");
  }

  // Prints the live status of both handles.
  // Shows IsAlive reflecting the real state after cancel.
  private void OnStatus(CCSPlayerController? player, CommandInfo info) {
    var circle = circleHandle == null ? "none" :
      circleHandle.IsAlive ? "alive" : "dead";
    var rect = rectHandle == null ? "none" :
      rectHandle.IsAlive ? "alive" : "dead";
    player!.PrintToChat($"[Example02] Circle: {circle} | Rectangle: {rect}");
  }

  // Cancels everything at once.
  private void OnCancelAll(CCSPlayerController? player, CommandInfo info) {
    draw?.CancelAll();
    player!.PrintToChat("[Example02] All shapes cancelled.");
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