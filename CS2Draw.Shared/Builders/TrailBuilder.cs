using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared.Timers;

namespace CS2DrawShared.Builders;

public sealed class TrailBuilder(CBaseEntity anchor,
  Func<TrailBuilder, ILoopTimer> commit)
  : DrawBuilder<TrailBuilder>(anchor.AbsOrigin ?? Vector.Zero) {
  public CBaseEntity Anchor { get; } = anchor;
  public float Interval { get; private set; } = 2f;

  /// <summary>
  /// How often a new trail particle is spawned in seconds.
  /// Defaults to 2 seconds.
  /// </summary>
  public TrailBuilder WithInterval(float seconds) {
    Interval = seconds;
    return this;
  }

  /// <summary>
  /// Starts the trail loop. Returns an ILoopTimer to stop it later.
  /// Stopping allows existing particles to fade naturally.
  /// </summary>
  public ILoopTimer Start() => commit(this);

  // Trail uses a looping timer, so this is not supported.
  public override IDrawHandle Draw()
    => throw new NotSupportedException(
      "Trail uses a looping timer. Call .Start() instead of .Draw().");
}