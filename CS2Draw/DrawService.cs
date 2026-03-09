using System.Collections.Concurrent;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Draw.Shapes;
using CS2DrawShared;
using CS2DrawShared.Builders;

namespace CS2Draw;

public sealed class DrawService(CS2DrawConfig config) : IDrawService {
  private readonly ConcurrentDictionary<Guid, DrawHandle> handles = new();
  private readonly Dictionary<string, IShapeSetup> customShapes = new();

  #region IDrawService

  // IDrawService
  public CircleBuilder Circle(Vector origin, float radius)
    => new(origin, radius, b => spawn(b.Origin, new CircleShapeSetup(b.Radius), b));

  public RectangleBuilder Rectangle(Vector origin, float width, float height)
    => new(origin, width, height,
      b => spawn(b.Origin, new RectangleShapeSetup(b.Width, b.Height), b));

  public void RegisterShape(IShapeSetup setup) {
    customShapes[setup.EffectKey] = setup;
  }

  public CustomBuilder Custom(Vector origin, IShapeSetup setup)
    => new(origin, setup, b => spawn(b.Origin, b.Setup, b));

  public void CancelAll() {
    foreach (var handle in handles.Values.ToArray()) handle.Cancel();
  }

  #endregion

  // ── Spawn sequence ────────────────────────────────────────────────────────

  private IDrawHandle spawn<T>(Vector origin, IShapeSetup setup,
    ShapeBuilder<T> builder) where T : ShapeBuilder<T> {
    var effectName = config.Resolve(setup.EffectKey);
    if (effectName == null) {
      Console.WriteLine($"Could not find effect for {setup.EffectKey}");
      return NullHandle.INSTANCE;
    }

    // 1. Create and teleport
    var particle =
      Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;
    particle.EffectName = effectName;
    particle.Teleport(origin, QAngle.Zero, Vector.Zero);

    // 2. Shape sets its control points
    var configurator = new ParticleConfigurator(particle);
    setup.Configure(configurator, builder.ParticleCount);

    // 3. Apply tint if color was set
    if (builder.TintColor.HasValue) {
      particle.TintCP = builder.TintCp;
      particle.Tint   = builder.TintColor.Value;
    }

    // 4. Spawn and start
    particle.StartActive = true;
    particle.DispatchSpawn();
    particle.AcceptInput("Start");

    // 5. Track the handle
    var handle = new DrawHandle(particle, h => handles.TryRemove(h.Id, out _));
    handles[handle.Id] = handle;

    // TODO: wire up lifetime timer using builder.Lifetime / builder.IsInfinite

    return handle;
  }
}