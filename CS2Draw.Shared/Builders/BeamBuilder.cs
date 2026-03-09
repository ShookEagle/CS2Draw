using CounterStrikeSharp.API.Modules.Utils;

namespace CS2DrawShared.Builders;

public sealed class BeamBuilder(Vector from, Vector to,
  Func<BeamBuilder, IDrawHandle> commit) : DrawBuilder<BeamBuilder>(from) {
  public Vector From { get; } = from;
  public Vector To { get; } = to;

  public override IDrawHandle Draw() => commit(this);
}