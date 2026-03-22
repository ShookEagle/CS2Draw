using System.Collections.Concurrent;
using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Draw.Shapes;
using CS2Draw.Timers;
using CS2DrawShared;
using CS2DrawShared.Builders;
using CS2DrawShared.Builders.Shapes;
using CS2DrawShared.Timers;
using Microsoft.Extensions.Logging;

namespace CS2Draw;

public sealed class DrawService(CS2DrawConfig config, ITimerService timers,
  ILogger logger) : IDrawService {
  private readonly BeaconManager beacons = new();
  private readonly ConcurrentDictionary<Guid, DrawHandle> handles = new();
  private readonly Dictionary<string, IShapeSetup> customShapes = new();

  // Shapes 
  public CircleBuilder Circle(Vector origin, float radius)
    => new(origin, radius,
      b => spawnShape(b.Origin, new CircleShapeSetup(b.Radius), b));

  public TriangleBuilder Triangle(Vector origin, float width, float height)
    => new(origin, width, height,
      b => spawnShape(b.Origin, new TriangleShapeSetup(b.Width, b.Height), b));

  public DiamondBuilder Diamond(Vector origin, float width, float height)
    => new(origin, width, height,
      b => spawnShape(b.Origin, new DiamondShapeSetup(b.Width, b.Height), b));

  public RectangleBuilder Rectangle(Vector origin, float width, float height)
    => new(origin, width, height,
      b => spawnShape(b.Origin, new RectangleShapeSetup(b.Width, b.Height), b));

  public PentagonBuilder Pentagon(Vector origin, float width, float height)
    => new(origin, width, height,
      b => spawnShape(b.Origin, new PentagonShapeSetup(b.Width, b.Height), b));

  public HexagonBuilder Hexagon(Vector origin, float width, float height)
    => new(origin, width, height,
      b => spawnShape(b.Origin, new HexagonShapeSetup(b.Width, b.Height), b));

  public StarBuilder Star(Vector origin, float width, float height)
    => new(origin, width, height,
      b => spawnShape(b.Origin, new StarShapeSetup(b.Width, b.Height), b));

  public HeartBuilder Heart(Vector origin, float width, float height)
    => new(origin, width, height,
      b => spawnShape(b.Origin, new HeartShapeSetup(b.Width, b.Height), b));

  public JellyBeanBuilder JellyBean(Vector origin, float width, float height)
    => new(origin, width, height,
      b => spawnShape(b.Origin, new JellyBeanShapeSetup(b.Width, b.Height), b));

  public AmongUsBuilder AmongUs(Vector origin, float width, float height)
    => new(origin, width, height,
      b => spawnShape(b.Origin, new AmongUsShapeSetup(b.Width, b.Height), b));


  // Beams 
  public BeamBuilder Beam(Vector from, Vector to) => new(from, to, spawnBeam);

  // Trails 
  public TrailBuilder Trail(CBaseEntity anchor) => new(anchor, startTrail);
  
  public CustomTrailBuilder CustomTrail(CBaseEntity anchor, IShapeSetup setup)
    => new(anchor, setup, b => startCustomTrail(b));

  // Beacons
  public BeaconBuilder Beacon(CCSPlayerController player) {
    if (HasBeacon(player)) RemoveBeacon(player);
    return new BeaconBuilder(player, startBeacon);
  }

  public bool HasBeacon(CCSPlayerController player) => beacons.Has(player);

  public void RemoveBeacon(CCSPlayerController player)
    => beacons.Remove(player);

  public void RemoveAllBeacons() => beacons.RemoveAll();

  // Custom 

  public void RegisterShape(IShapeSetup setup)
    => customShapes[setup.EffectKey] = setup;

  public CustomBuilder Custom(Vector origin, IShapeSetup setup)
    => new(origin, setup, b => spawnShape(b.Origin, b.Setup, b));

  // Lifecycle 

  public void CancelAll() {
    beacons.RemoveAll();
    foreach (var handle in handles.Values.ToArray()) handle.Cancel();
  }

  // Spawners

  private IDrawHandle spawnShape<T>(Vector origin, IShapeSetup setup,
    DrawBuilder<T> builder) where T : DrawBuilder<T> {
    var effectName = config.Resolve(setup.EffectKey);
    if (effectName == null) {
      logger.LogWarning(
        "[CS2Draw] No effect found for key '{SetupEffectKey}'. Check cs2draw.json.",
        setup.EffectKey);
      return NullHandle.INSTANCE;
    }

    var particle =
      Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");
    if (particle == null) return NullHandle.INSTANCE;

    particle.EffectName = effectName;
    particle.Teleport(origin, QAngle.Zero, Vector.Zero);
    particle.LifeState = 5;

    var configurator = new ParticleConfigurator(particle);
    setup.Configure(configurator, builder.ParticleCount);

    applyTint(particle, builder.TintColor, builder.TintCp);

    particle.StartActive = true;
    particle.DispatchSpawn();
    particle.AcceptInput("Start");

    var handle = new DrawHandle(particle, h => handles.TryRemove(h.Id, out _));
    handles[handle.Id] = handle;

    if (builder is { IsInfinite: false, Lifetime: > 0f })
      timers.Delay(builder.Lifetime, () => handle.Cancel());

    return handle;
  }

  private IDrawHandle spawnBeam(BeamBuilder builder) {
    var effectName = config.Resolve("beam");
    if (effectName == null) {
      logger.LogError(
        "[CS2Draw] No effect found for key 'beam'. Check cs2draw.json.");
      return NullHandle.INSTANCE;
    }

    var beam =
      Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");
    if (beam == null) return NullHandle.INSTANCE;

    beam.EffectName = effectName;
    beam.Teleport(builder.From, QAngle.Zero, Vector.Zero);

    beam.AcceptInput("SetControlPoint", value: $"2: 2 0 0");
    beam.AcceptInput("SetControlPoint",
      value: $"5: {builder.From.X} {builder.From.Y} {builder.From.Z}");
    beam.AcceptInput("SetControlPoint",
      value: $"6: {builder.To.X} {builder.To.Y} {builder.To.Z}");

    applyTint(beam, builder.TintColor, builder.TintCp);

    beam.StartActive = true;
    beam.DispatchSpawn();
    beam.AcceptInput("Start");

    var handle = new DrawHandle(beam, h => handles.TryRemove(h.Id, out _));
    handles[handle.Id] = handle;

    if (builder is { IsInfinite: false, Lifetime: > 0f })
      timers.Delay(builder.Lifetime, () => handle.Cancel());

    return handle;
  }

  private ILoopTimer startBeacon(BeaconBuilder builder) {
    var effectName = config.Resolve("beacon");
    if (effectName == null) {
      logger.LogError(
        "[CS2Draw] No effect found for key 'beacon'. Check cs2draw.json.");
      return NullLoopTimer.INSTANCE;
    }

    timers.CreateLoop(2f, () => spawnBeaconTick(builder, effectName));
    beacons.Add(builder.Player, builder, timers, b => 
    {
      var effect = config.Resolve("beacon");
      if (effect != null) spawnBeaconTick(b, effect);
    });
    // Return a NullLoopTimer — the caller doesn't need to manage the timer directly.
    // The shared timer is owned by BeaconManager.
    return NullLoopTimer.INSTANCE;
  }

  private void
    spawnBeaconTick(BeaconBuilder builder, string effectName) {
    var pawn = builder.Player.PlayerPawn.Value;
    if (pawn?.AbsOrigin == null) return;

    var pos = new Vector(pawn.AbsOrigin.X, pawn.AbsOrigin.Y,
      pawn.AbsOrigin.Z + builder.ZOffset);

    var particle =
      Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");
    if (particle == null) return;

    particle.EffectName = effectName;
    particle.Teleport(pos, QAngle.Zero, Vector.Zero);
    particle.StartActive = true;

    // use override color if set, otherwise fall back to team color
    var color = builder.Override ?? getTeamColor(builder.Player);
    applyTint(particle, color, builder.TintCp);

    particle.DispatchSpawn();
    particle.AcceptInput("Start");
    particle.AcceptInput("SetParent", pawn, particle, "!activator");

    builder.Player.EmitSound(config.BeaconSoundEvent);
  }

  private ILoopTimer startTrail(TrailBuilder builder) {
    var effectName = config.Resolve("trail");
    if (effectName == null) {
      logger.LogError(
        "[CS2Draw] No effect found for key 'trail'. Check cs2draw.json.");
      return NullLoopTimer.INSTANCE;
    }

    var timer = timers.CreateLoop(builder.Interval,
      () => spawnTrailTick(builder, effectName));
    timer.Start();
    return timer;
  }

  private static void spawnTrailTick(TrailBuilder builder, string effectName) {
    if (builder.Anchor.AbsOrigin == null) return;

    var particle =
      Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");
    if (particle == null) return;

    particle.EffectName = effectName;
    particle.Teleport(builder.Anchor.AbsOrigin, QAngle.Zero, Vector.Zero);

    particle.AcceptInput("SetControlPoint", value: "2: 0 0 0");

    applyTint(particle, builder.TintColor, builder.TintCp);

    particle.StartActive = true;
    particle.DispatchSpawn();
    particle.AcceptInput("Start");
    particle.AcceptInput("SetParent", builder.Anchor, particle, "!activator");
  }
  
  private ITrailHandle startCustomTrail(CustomTrailBuilder builder)
  {
    var effectName = config.Resolve(builder.Setup.EffectKey);
    if (effectName == null)
    {
      Console.WriteLine($"[CS2Draw] No effect found for key '{builder.Setup.EffectKey}'. Check cs2draw.json.");
      return NullTrailHandle.INSTANCE;
    }
 
    // Handle is created first so the tick closure can close over it
    // and read the current anchor + CP overrides each tick
    CustomTrailHandle? handle = null;

    var customTrailHandle = handle;
    var timer = timers.CreateLoop(builder.Interval,
      () => spawnCustomTrailTick(builder, effectName, customTrailHandle!));
 
    handle = new CustomTrailHandle(timer, builder.Anchor);
    timer.Start();
 
    return handle;
  }
 
  private static void spawnCustomTrailTick(CustomTrailBuilder builder,
    string effectName, CustomTrailHandle handle)
  {
    var anchor = handle.CurrentAnchor;
    if (anchor.AbsOrigin == null) return;
 
    var particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");
    if (particle == null) return;
 
    particle.EffectName = effectName;
    particle.Teleport(anchor.AbsOrigin);
 
    var configurator = new ParticleConfigurator(particle);
    builder.Setup.Configure(configurator, builder.ParticleCount);
 
    // Apply tint from builder
    applyTint(particle, builder.TintColor, builder.TintCp);
 
    // Apply any runtime CP overrides set via handle.SetCP()
    foreach (var (cp, val) in handle.CPOverrides)
      particle.AcceptInput("SetControlPoint", value: $"{cp}: {val.x} {val.y} {val.z}");
 
    particle.StartActive = true;
    particle.DispatchSpawn();
    particle.AcceptInput("Start");
 
    // Parent to current anchor — this is what SetParent() changes
    particle.AcceptInput("SetParent", anchor, particle, "!activator");
  }

  private static void
    applyTint(CParticleSystem particle, Color? color, int cp) {
    if (!color.HasValue) return;
    particle.TintCP = cp;
    particle.Tint   = color.Value;
  }

  private static Color getTeamColor(CCSPlayerController player)
    => player.TeamNum switch {
      2 => Color.Red,  // T
      3 => Color.Blue, // CT
      _ => Color.White
    };
}