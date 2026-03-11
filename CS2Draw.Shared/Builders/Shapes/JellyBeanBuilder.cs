using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders.Shapes;

public sealed class JellyBeanBuilder(Vector origin, float width, float height,
  Func<JellyBeanBuilder, IDrawHandle> commit)
  : DrawBuilder<JellyBeanBuilder>(origin) {
  public float Width { get; private set; } = width;
  public float Height { get; private set; } = height;

  public override IDrawHandle Draw() => commit(this);
}