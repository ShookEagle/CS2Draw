using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders.Shapes;

public sealed class AmongUsBuilder(Vector origin, float width, float height,
  Func<AmongUsBuilder, IDrawHandle> commit)
  : DrawBuilder<AmongUsBuilder>(origin) {
  public float Width { get; private set; } = width;
  public float Height { get; private set; } = height;

  public override IDrawHandle Draw() => commit(this);
}