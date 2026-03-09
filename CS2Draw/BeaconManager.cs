using CounterStrikeSharp.API.Core;
using CS2DrawShared.Timers;

namespace CS2Draw;

/// <summary>
/// Tracks active beacons per player.
/// Ensures one beacon per player and handles cleanup on disconnect or round end.
/// Keyed by SteamID so the reference stays valid even if the player object changes.
/// </summary>
internal sealed class BeaconManager
{
  private readonly Dictionary<ulong, ILoopTimer> active = new();

  /// <summary>
  /// Add or replace a beacon for a player.
  /// If the player already has an active beacon it is stopped first.
  /// </summary>
  public void Add(CCSPlayerController player, ILoopTimer timer)
  {
    var steamId = player.SteamID;

    if (active.TryGetValue(steamId, out var existing))
    {
      existing.Stop();
      Console.WriteLine($"[CS2Draw] Replaced existing beacon for {player.PlayerName}.");
    }

    active[steamId] = timer;
  }

  /// <summary>Stop and remove the beacon for a specific player.</summary>
  public void Remove(CCSPlayerController player)
  {
    var steamId = player.SteamID;

    if (!active.TryGetValue(steamId, out var timer)) return;

    timer.Stop();
    active.Remove(steamId);
  }

  /// <summary>Returns true if the player currently has an active beacon.</summary>
  public bool Has(CCSPlayerController player)
    => active.TryGetValue(player.SteamID, out var timer) && timer.IsRunning;

  /// <summary>Stop and remove all active beacons. Called on round end.</summary>
  public void RemoveAll()
  {
    foreach (var timer in active.Values)
      timer.Stop();

    active.Clear();
  }
}