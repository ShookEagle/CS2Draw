using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders.Shapes;

public sealed class TriangleBuilder(Vector origin, float width, float height,
  Func<TriangleBuilder, IDrawHandle> commit)
  : DrawBuilder<TriangleBuilder>(origin) {
  public float Width { get; private set; } = width;
  public float Height { get; private set; } = height;

  public override IDrawHandle Draw() => commit(this);
}