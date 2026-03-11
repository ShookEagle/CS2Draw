// ┌─────────────────────────────────────────────────────────────────────────┐
// │  Example 03 — Two Point Beams                                           │
// │  Shows: BeamBuilder, drawing between two world positions                │
// └─────────────────────────────────────────────────────────────────────────┘

using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared;

namespace CS2Draw.Examples;

public sealed class Example03Beams : BasePlugin {
  public override string ModuleName => "CS2Draw Example 03 — Beams";
  public override string ModuleVersion => "1.0.0";
  public override string ModuleAuthor => "";

  public override string ModuleDescription
    => "Draws a beam between two world positions.";

  private static readonly PluginCapability<IDrawService> DRAW_CAPABILITY =
    new("cs2draw:service");

  private IDrawService? draw;
  private IDrawHandle? persistentBeam;

  public override void OnAllPluginsLoaded(bool hotReload) {
    draw = DRAW_CAPABILITY.Get();
    if (draw == null) {
      Console.WriteLine("[Example03] CS2Draw not loaded.");
      return;
    }

    AddCommand("css_ex3_beam", "Draw a beam forward 300 units", OnBeam);
    AddCommand("css_ex3_beam_up", "Draw a vertical beam above you", OnBeamUp);
    AddCommand("css_ex3_beam_inf", "Draw a permanent beam", OnBeamInfinite);
    AddCommand("css_ex3_beam_cancel", "Cancel the permanent beam",
      OnBeamCancel);
  }

  // Draws a yellow beam 300 units in front of the player.
  // Fire and forget — expires after 10 seconds.
  private void OnBeam(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetOrigin(player, out var pos)) return;

    var endPos = new Vector(pos.X + 300f, pos.Y, pos.Z);

    draw!.Beam(pos, endPos).Color(Color.Yellow).WithLifetime(10f).Draw();

    player!.PrintToChat("[Example03] Beam drawn — 10s.");
  }

  // Draws an orange vertical beam 200 units above the player.
  // Shows that beams work in any direction — not just horizontal.
  private void OnBeamUp(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetOrigin(player, out var pos)) return;

    var topPos = new Vector(pos.X, pos.Y, pos.Z + 200f);

    draw!.Beam(pos, topPos).Color(Color.Orange).WithLifetime(10f).Draw();

    player!.PrintToChat("[Example03] Vertical beam drawn — 10s.");
  }

  // Draws a permanent beam and stores the handle.
  private void OnBeamInfinite(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetOrigin(player, out var pos)) return;

    if (persistentBeam?.IsAlive == true) {
      player!.PrintToChat(
        "[Example03] Already have a persistent beam. Use css_ex3_beam_cancel first.");
      return;
    }

    var endPos = new Vector(pos.X + 400f, pos.Y, pos.Z);

    persistentBeam = draw!.Beam(pos, endPos)
     .Color(Color.Cyan)
     .Infinite()
     .Draw();

    player!.PrintToChat(
      $"[Example03] Permanent beam drawn. ID: {persistentBeam.Id}");
  }

  // Cancels the persistent beam early via its handle.
  private void OnBeamCancel(CCSPlayerController? player, CommandInfo info) {
    if (persistentBeam is not { IsAlive: true }) {
      player!.PrintToChat("[Example03] No active persistent beam.");
      return;
    }

    persistentBeam.Cancel();
    player!.PrintToChat("[Example03] Beam cancelled.");
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