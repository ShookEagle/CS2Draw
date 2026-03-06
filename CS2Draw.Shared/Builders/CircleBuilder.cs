using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders;

public sealed class CircleBuilder : DrawBuilder<CircleBuilder>
{
  internal float Radius   { get; private set; }
  internal int   Segments { get; private set; } = 32;

  private readonly Func<CircleBuilder, IDrawHandle> commit;

  internal CircleBuilder(Vector origin, float radius, Func<CircleBuilder, IDrawHandle> commit)
    : base(origin)
  {
    Radius      = radius;
    this.commit = commit;
  }

  /// <summary>How many line segments approximate the circle. Higher = smoother.</summary>
  public CircleBuilder WithSegments(int segments)
  {
    Segments = segments;
    return this;
  }

  public override IDrawHandle Draw() => commit(this);
}