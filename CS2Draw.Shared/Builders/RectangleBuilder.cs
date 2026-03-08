using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders;

public sealed class RectangleBuilder : ShapeBuilder<RectangleBuilder>
{
  public float Width  { get; private set; }
  public float Height { get; private set; }

  private readonly Func<RectangleBuilder, IDrawHandle> commit;

  public RectangleBuilder(Vector origin, float width, float height, Func<RectangleBuilder, IDrawHandle> commit)
    : base(origin)
  {
    Width       = width;
    Height      = height;
    this.commit = commit;
  }

  public override IDrawHandle Draw() => commit(this);
}