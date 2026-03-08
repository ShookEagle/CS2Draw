using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders;

public sealed class RectangleBuilder : ShapeBuilder<RectangleBuilder>
{
  internal float Width  { get; private set; }
  internal float Height { get; private set; }

  private readonly Func<RectangleBuilder, IDrawHandle> commit;

  internal RectangleBuilder(Vector origin, float width, float height, Func<RectangleBuilder, IDrawHandle> commit)
    : base(origin)
  {
    Width       = width;
    Height      = height;
    this.commit = commit;
  }

  public override IDrawHandle Draw() => commit(this);
}