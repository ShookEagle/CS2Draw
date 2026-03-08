using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders;

public sealed class CustomBuilder(Vector origin, IShapeSetup setup,
  Func<CustomBuilder, IDrawHandle> commit)
  : ShapeBuilder<CustomBuilder>(origin) {
  public IShapeSetup Setup { get; } = setup;

  public override IDrawHandle Draw() => commit(this);
}