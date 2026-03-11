// ┌─────────────────────────────────────────────────────────────────────────┐
// │  Example 04 — Beacons                                                   │
// │  Shows: BeaconBuilder, team color, color override, HasBeacon,           │
// │         RemoveBeacon, automatic round end cleanup                       │
// └─────────────────────────────────────────────────────────────────────────┘

using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CS2DrawShared;

namespace CS2Draw.Examples;

public sealed class Beacons : BasePlugin {
  public override string ModuleName => "CS2Draw Example 04 — Beacons";
  public override string ModuleVersion => "1.0.0";
  public override string ModuleAuthor => "";

  public override string ModuleDescription
    => "Beacons on players — team color, overrides, and lifecycle management.";

  private static readonly PluginCapability<IDrawService> DRAW_CAPABILITY =
    new("cs2draw:service");

  private IDrawService? draw;

  public override void OnAllPluginsLoaded(bool hotReload) {
    draw = DRAW_CAPABILITY.Get();
    if (draw == null) {
      Console.WriteLine("[Example04] CS2Draw not loaded.");
      return;
    }

    AddCommand("css_ex4_beacon", "Start beacon (team color)", OnBeacon);
    AddCommand("css_ex4_beacon_color", "Start beacon (custom color)",
      OnBeaconColor);
    AddCommand("css_ex4_beacon_stop", "Stop your beacon", OnBeaconStop);
    AddCommand("css_ex4_beacon_check", "Check if you have a beacon",
      OnBeaconCheck);
    AddCommand("css_ex4_beacon_all", "Beacon every player on server",
      OnBeaconAll);
    AddCommand("css_ex4_stop_all", "Stop all beacons", OnStopAll);
  }

  // Starts a beacon using the player's team color.
  // Red = T, Blue = CT, White = FFA.
  // Replaces any existing beacon on this player automatically.
  private void OnBeacon(CCSPlayerController? player, CommandInfo info) {
    if (player == null) return;

    draw!.Beacon(player).WithOffset(8f).Start();

    player.PrintToChat("[Example04] Beacon started — team color.");
  }

  // Starts a beacon with a custom color override.
  // The offset is raised slightly so the beacon is more visible.
  private void OnBeaconColor(CCSPlayerController? player, CommandInfo info) {
    if (player == null) return;

    draw!.Beacon(player).WithOffset(16f).Color(Color.Magenta).Start();

    player.PrintToChat("[Example04] Beacon started — magenta override.");
  }

  // Stops the calling player's beacon via RemoveBeacon.
  // HasBeacon() is used to guard against stopping a non-existent beacon.
  private void OnBeaconStop(CCSPlayerController? player, CommandInfo info) {
    if (player == null) return;

    if (!draw!.HasBeacon(player)) {
      player.PrintToChat("[Example04] You don't have an active beacon.");
      return;
    }

    draw.RemoveBeacon(player);
    player.PrintToChat("[Example04] Your beacon has been stopped.");
  }

  // Checks whether the calling player currently has an active beacon.
  // Shows HasBeacon() usage.
  private void OnBeaconCheck(CCSPlayerController? player, CommandInfo info) {
    if (player == null) return;

    var status = draw!.HasBeacon(player) ? "ACTIVE" : "NONE";
    player.PrintToChat($"[Example04] Your beacon: {status}");
  }

  // Starts a beacon on every connected player.
  // Shows that beacons can be managed independently per player.
  private void OnBeaconAll(CCSPlayerController? player, CommandInfo info) {
    var players = Utilities.GetPlayers()
     .Where(p => p.IsValid && p.PlayerPawn.Value != null);

    foreach (var p in players) draw!.Beacon(p).WithOffset(8f).Start();

    player!.PrintToChat("[Example04] Beacons started on all players.");
  }

  // Stops all active beacons across all players at once.
  private void OnStopAll(CCSPlayerController? player, CommandInfo info) {
    draw?.RemoveAllBeacons();
    player!.PrintToChat("[Example04] All beacons stopped.");
  }

  // CS2Draw automatically cleans up beacons on the round end and disconnects.
  // This hook is here to show you can also hook into it yourself if needed.
  [GameEventHandler]
  public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info) {
    // _draw?.RemoveAllBeacons() is already called by CS2Draw itself.
    // This is just to show you can react to it in your own plugin too.
    Console.WriteLine(
      "[Example04] Round ended — CS2Draw cleaned up beacons automatically.");
    return HookResult.Continue;
  }
}