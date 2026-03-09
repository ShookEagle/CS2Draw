using System.Drawing;
using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared;


namespace CS2DrawShared.Builders;

/// <summary>
/// Base fluent builder. Every shape builder inherits these.
/// T is the concrete builder so the chain never loses its type.
/// </summary>
public abstract class ShapeBuilder<T>(Vector origin) where T : ShapeBuilder<T> {
  public Vector Origin = origin;
  public int ParticleCount = 20;
  protected float Lifetime = 5f;
  protected bool IsInfinite = false;
  public Color? TintColor;
  public int TintCp = 1;

  /// <summary>How many particles to spawn for this shape.</summary>
  public T Particles(int count) {
    ParticleCount = count;
    return (T)this;
  }

  /// <summary>
  /// Set the tint color and which control point to apply it to.
  /// Control point defaults to 0 — only change if your particle expects it elsewhere.
  /// </summary>
  public T Color(Color color, int controlPoint = 1) {
    TintColor = color;
    TintCp    = controlPoint;
    return (T)this;
  }

  /// <summary>How long the shape stays visible in seconds.</summary>
  public T WithLifetime(float seconds) {
    Lifetime   = seconds;
    IsInfinite = false;
    return (T)this;
  }

  /// <summary>Keep the shape alive until explicitly cancelled.</summary>
  public T Infinite() {
    IsInfinite = true;
    return (T)this;
  }

  /// <summary>
  /// Commits the draw call to the world.
  /// Returns a handle for early cancellation — safe to discard if not needed.
  /// </summary>
  public abstract IDrawHandle Draw();
}