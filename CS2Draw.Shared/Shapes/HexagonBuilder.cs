using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared.Builders;

namespace CS2DrawShared.Shapes;

public sealed class HexagonBuilder(Vector origin, float width, float height,
  Func<HexagonBuilder, IDrawHandle> commit)
  : DrawBuilder<HexagonBuilder>(origin) {
  public float Width { get; private set; } = width;
  public float Height { get; private set; } = height;

  public override IDrawHandle Draw() => commit(this);
}