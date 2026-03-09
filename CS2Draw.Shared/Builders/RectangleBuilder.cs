using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders;

public sealed class RectangleBuilder(Vector origin, float width, float height,
  Func<RectangleBuilder, IDrawHandle> commit)
  : DrawBuilder<RectangleBuilder>(origin) {
  public float Width { get; private set; } = width;
  public float Height { get; private set; } = height;

  public override IDrawHandle Draw() => commit(this);
}