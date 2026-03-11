// ┌─────────────────────────────────────────────────────────────────────────┐
// │  Example 05 — Trails                                                    │
// │  Shows: TrailBuilder, ILoopTimer, attaching to any CBaseEntity,         │
// │         start/stop, interval control, natural particle fade on stop     │
// └─────────────────────────────────────────────────────────────────────────┘

using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CS2DrawShared;
using CS2DrawShared.Timers;

namespace CS2Draw.Examples;

public sealed class Trails : BasePlugin {
  public override string ModuleName => "CS2Draw Example 05 — Trails";
  public override string ModuleVersion => "1.0.0";
  public override string ModuleAuthor => "";

  public override string ModuleDescription
    => "Trails attached to player pawns — start, stop, and interval control.";

  private static readonly PluginCapability<IDrawService> DRAW_CAPABILITY =
    new("cs2draw:service");

  private IDrawService? draw;

  // Per-player trail timers keyed by SteamID
  private readonly Dictionary<ulong, ILoopTimer> trails = new();

  public override void OnAllPluginsLoaded(bool hotReload) {
    draw = DRAW_CAPABILITY.Get();
    if (draw == null) {
      Console.WriteLine("[Example05] CS2Draw not loaded.");
      return;
    }

    AddCommand("css_ex5_trail", "Start a trail on yourself", OnTrail);
    AddCommand("css_ex5_trail_fast", "Start a fast trail (0.5s interval)",
      OnTrailFast);
    AddCommand("css_ex5_trail_slow", "Start a slow trail (4s interval)",
      OnTrailSlow);
    AddCommand("css_ex5_trail_stop", "Stop your trail", OnTrailStop);
    AddCommand("css_ex5_trail_check", "Check if your trail is running",
      OnTrailCheck);
  }

  // Starts a cyan trail on the calling player's pawn.
  // Trail attaches to the pawn entity — it will follow the player as they move.
  // Spawns a new particle every 2 seconds (default interval).
  private void OnTrail(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetPawn(player, out var pawn)) return;

    stopExistingTrail(player!);

    var timer = draw!.Trail(pawn!).Color(Color.Cyan).WithInterval(2f).Start();

    trails[player!.SteamID] = timer;
    player.PrintToChat("[Example05] Trail started — 2s interval.");
  }

  // Fast trail — spawns particles every 0.5 s for a denser effect.
  private void OnTrailFast(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetPawn(player, out var pawn)) return;

    stopExistingTrail(player!);

    var timer = draw!.Trail(pawn!)
     .Color(Color.HotPink)
     .WithInterval(0.5f)
     .Start();

    trails[player!.SteamID] = timer;
    player.PrintToChat("[Example05] Fast trail started — 0.5s interval.");
  }

  // Slow trail — spawns particles every 4s. Particles fade before the next one spawns.
  private void OnTrailSlow(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetPawn(player, out var pawn)) return;

    stopExistingTrail(player!);

    var timer = draw!.Trail(pawn!)
     .Color(Color.LimeGreen)
     .WithInterval(4f)
     .Start();

    trails[player!.SteamID] = timer;
    player.PrintToChat("[Example05] Slow trail started — 4s interval.");
  }

  // Stops the calling player's trail.
  // Existing particles are NOT removed — they fade out naturally.
  // This is intentional — the trail dissolves rather than cutting off abruptly.
  private void OnTrailStop(CCSPlayerController? player, CommandInfo info) {
    if (player == null) return;

    if (!trails.TryGetValue(player.SteamID, out var timer)
      || !timer.IsRunning) {
      player.PrintToChat("[Example05] You don't have an active trail.");
      return;
    }

    timer.Stop();
    trails.Remove(player.SteamID);
    player.PrintToChat(
      "[Example05] Trail stopped. Existing particles will fade naturally.");
  }

  // Checks whether the calling player's trail is currently running.
  private void OnTrailCheck(CCSPlayerController? player, CommandInfo info) {
    if (player == null) return;

    var running = trails.TryGetValue(player.SteamID, out var timer)
      && timer.IsRunning;
    player.PrintToChat(
      $"[Example05] Your trail: {(running ? "RUNNING" : "NONE")}");
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  private void stopExistingTrail(CCSPlayerController player) {
    if (!trails.TryGetValue(player.SteamID, out var existing)) return;
    existing.Stop();
    trails.Remove(player.SteamID);
  }

  private static bool tryGetPawn(CCSPlayerController? player,
    out CBaseEntity? pawn) {
    pawn = null;
    if (player == null || !player.IsValid || player.PlayerPawn.Value == null)
      return false;
    pawn = player.PlayerPawn.Value;
    return true;
  }
}