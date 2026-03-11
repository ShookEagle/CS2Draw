using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared.Builders;

namespace CS2DrawShared.Shapes;

public sealed class RectangleBuilder(Vector origin, float width, float height,
  Func<RectangleBuilder, IDrawHandle> commit)
  : DrawBuilder<RectangleBuilder>(origin) {
  public float Width { get; private set; } = width;
  public float Height { get; private set; } = height;

  public override IDrawHandle Draw() => commit(this);
}