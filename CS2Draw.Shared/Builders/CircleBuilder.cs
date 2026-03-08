using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders;

public sealed class CircleBuilder : ShapeBuilder<CircleBuilder>
{
  public float Radius   { get; private set; }
  public int   Segments { get; private set; } = 32;

  private readonly Func<CircleBuilder, IDrawHandle> commit;

  public CircleBuilder(Vector origin, float radius, Func<CircleBuilder, IDrawHandle> commit)
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