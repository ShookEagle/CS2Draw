using System.Drawing;
using CounterStrikeSharp.API.Core;

namespace CS2DrawShared.Builders;

public sealed class CustomTrailBuilder(CBaseEntity anchor, IShapeSetup setup,
  Func<CustomTrailBuilder, ITrailHandle> commit) {
  public CBaseEntity Anchor { get; } = anchor;
  public IShapeSetup Setup { get; } = setup;
  public float Interval { get; private set; } = 2f;
  public Color? TintColor { get; private set; }
  public int TintCp { get; private set; } = 1;
  public int ParticleCount { get; private set; } = 16;

  // Runtime-mutable CP overrides — read by each tick
  internal float[]? CpOverride { get; private set; }
  internal int CpOverrideIndex { get; private set; }

  public CustomTrailBuilder WithInterval(float seconds) {
    Interval = seconds;
    return this;
  }

  public CustomTrailBuilder Color(Color color, int cp = 1) {
    TintColor = color;
    TintCp    = cp;
    return this;
  }

  public CustomTrailBuilder Particles(int count) {
    ParticleCount = count;
    return this;
  }

  public ITrailHandle Start() => commit(this);
}