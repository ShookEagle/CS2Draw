using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders.Shapes;

public sealed class CircleBuilder(Vector origin, float radius,
  Func<CircleBuilder, IDrawHandle> commit)
  : DrawBuilder<CircleBuilder>(origin) {
  public float Radius { get; private set; } = radius;
  public int Segments { get; private set; } = 32;

  /// <summary>How many line segments approximate the circle. Higher = smoother.</summary>
  public CircleBuilder WithSegments(int segments) {
    Segments = segments;
    return this;
  }

  public override IDrawHandle Draw() => commit(this);
}