using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders;

public sealed class CustomBuilder : ShapeBuilder<CustomBuilder>
{
  public IShapeSetup Setup { get; }

  private readonly Func<CustomBuilder, IDrawHandle> commit;

  public CustomBuilder(Vector origin, IShapeSetup setup, Func<CustomBuilder, IDrawHandle> commit)
    : base(origin)
  {
    Setup       = setup;
    this.commit = commit;
  }

  public override IDrawHandle Draw() => commit(this);
}