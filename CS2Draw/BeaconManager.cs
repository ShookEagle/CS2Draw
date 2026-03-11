using CounterStrikeSharp.API.Core;
using CS2DrawShared.Builders;
using CS2DrawShared.Timers;

namespace CS2Draw;

/// <summary>
/// Tracks active beacons per player. A single shared timer drives all beacons
/// so all players pulse in sync and we never have more than one timer running.
/// </summary>
internal sealed class BeaconManager {
  private readonly Dictionary<ulong, BeaconBuilder> active = new();
  private ILoopTimer? sharedTimer;

  /// <summary>
  /// Add or replace a beacon for a player and ensure the shared timer is running.
  /// </summary>
  public void Add(CCSPlayerController player, BeaconBuilder builder,
    ITimerService timers, Action<BeaconBuilder> onTick) {
    // replace existing entry if present — builder holds the new config
    active[player.SteamID] = builder;

    // start the shared timer only if it isn't already running
    if (sharedTimer is { IsRunning: true }) return;
    sharedTimer = timers.CreateLoop(2f, () => tick(onTick));
    sharedTimer.Start();
  }

  /// <summary>
  /// Remove a single player's beacon. Stops the shared timer if no beacons remain.
  /// </summary>
  public void Remove(CCSPlayerController player) {
    active.Remove(player.SteamID);
    stopTimerIfEmpty();
  }

  /// <summary>
  /// Remove all beacons and stop the shared timer immediately.
  /// </summary>
  public void RemoveAll() {
    active.Clear();
    sharedTimer?.Stop();
    sharedTimer = null;
  }

  public bool Has(CCSPlayerController player)
    => active.ContainsKey(player.SteamID);

  /// <summary>
  /// Called every 2s by the shared timer — fires the tick callback for every active beacon.
  /// </summary>
  private void tick(Action<BeaconBuilder> onTick) {
    // snapshot keys to avoid mutation during iteration
    foreach (var builder in active.Values.ToArray()) onTick(builder);
  }

  private void stopTimerIfEmpty() {
    if (active.Count > 0) return;
    sharedTimer?.Stop();
    sharedTimer = null;
  }
}