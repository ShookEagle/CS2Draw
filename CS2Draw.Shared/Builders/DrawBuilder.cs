using System.Drawing;
using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared;


namespace CS2DrawShared.Builders;

/// <summary>
/// Base fluent builder. Every shape builder inherits these.
/// T is the concrete builder so the chain never loses its type.
/// </summary>
public abstract class DrawBuilder<T>(Vector origin) where T : DrawBuilder<T> {
  public Vector Origin = origin;
  public int ParticleCount = 20;
  public float Lifetime;
  public bool IsInfinite = true;
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

  /// <summary>
  /// Keep this drawable alive until Cancel() is called explicitly.
  /// This is the default — only needed if you previously called WithLifetime()
  /// and want to revert.
  /// </summary>
  public T Infinite() {
    IsInfinite = true;
    Lifetime   = 0f;
    return (T)this;
  }

  /// <summary>
  /// Commits the draw call to the world.
  /// Returns a handle for early cancellation — safe to discard if not needed.
  /// </summary>
  public abstract IDrawHandle Draw();
}