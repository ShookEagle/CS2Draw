using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared.Timers;

namespace CS2DrawShared.Builders;

public class BeaconBuilder(CCSPlayerController player,
  Func<BeaconBuilder, ILoopTimer> commit)
  : DrawBuilder<BeaconBuilder>(
    player.PlayerPawn.Value!.AbsOrigin ?? Vector.Zero) {
  public CCSPlayerController Player { get; } = player;
  public float ZOffset { get; private set; } = 8f;
  public Color? Override { get; private set; }
  private CParticleSystem? previousParticle;
  public CParticleSystem? PreviousParticle {
    get => previousParticle;
    set {
      previousParticle?.Remove();
      previousParticle = value;
    }
  }

  /// <summary>
  /// Z offset to lift the beacon off the ground.
  /// Defaults to 8 units.
  /// </summary>
  public BeaconBuilder WithOffset(float z) {
    ZOffset = z;
    return this;
  }

  /// <summary>
  /// Override the team color with a specific color.
  /// By default the beacon uses team color: Red = T, Blue = CT, White = FFA.
  /// </summary>
  public new BeaconBuilder Color(Color color, int controlPoint = 1) {
    Override = color;
    return base.Color(color, controlPoint);
  }

  /// <summary>
  /// Starts the beacon loop. Returns an ILoopTimer to stop it later.
  /// </summary>
  public ILoopTimer Start() => commit(this);

  // Beacon uses a looping timer, so this is not supported.
  public override IDrawHandle Draw()
    => throw new NotSupportedException(
      "Beacon uses a looping timer. Call .Start() instead of .Draw().");
}