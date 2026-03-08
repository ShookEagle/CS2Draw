using CS2DrawShared;

namespace CS2Draw;

/// <summary>
/// Returned when a spawn fails (e.g. missing effect key in config).
/// Prevents null reference exceptions on the caller side.
/// </summary>
internal sealed class NullHandle : IDrawHandle
{
  public static readonly NullHandle INSTANCE = new();

  public Guid Id      { get; } = Guid.Empty;
  public bool IsAlive => false;
  public void Cancel() { }
}