# CS2Draw

A centralized world-space shape rendering service for CS2 plugins using `CParticleSystem`

> Built for use on [EdgeGamers](https://www.edgegamers.com/forums/news/) servers and shared as a demonstration of what's
> possible with CS2 particles. If this gains popularity, I'll look into expanding this to be a fully-fledged API.
> See [PARTICLES.md](./PARTICLES.md) for help creating compatible particle effects in the CS2 Particle Editor.

> Huge shout to [Letaryat](https://github.com/Letaryat) for spearheading this, see full credits at the bottom.

---

## Requirements

- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)—Plugin framework
- [MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager)—Required to mount the addon containing the `.vpcf`
  particle files

> Particles **will not render** without a mounted addon. See [PARTICLES.md](./PARTICLES.md) for help setting that up.

An addon containing all built-in CS2Draw particle files is available on
the [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3682043071). The raw `.vpcf` files are also
included in this repo under `particles/` if you want to build your own or use them as reference.

---

## Setup


### 1. Install the NuGet package

Add `CS2DrawShared` to your plugin's `.csproj`:

```xml
<PackageReference Include="CS2DrawShared" Version="1.0.0" />
```

Or via the .NET CLI:

```bash
dotnet add package CS2DrawShared --version 1.0.0
```

The package is available on [NuGet](https://www.nuget.org/packages/CS2DrawShared).

### 2. Resolve the capability

```csharp
using CS2DrawShared;
using CounterStrikeSharp.API.Core.Capabilities;

public sealed class MyPlugin : BasePlugin
{
    private static readonly PluginCapability DrawCapability = new("cs2draw:service");

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        var draw = DrawCapability.Get();
        if (draw == null) return;

        // ready to draw
    }
}
```

> CS2Draw must be loaded before your plugin. Always resolve in `OnAllPluginsLoaded`, not `Load`.

---

## Built-in Shapes

Every draw call follows the same pattern:

```csharp
draw.Shape(origin, ...params...).Options().Draw();
```

`.Draw()` returns an `IDrawHandle` — safe to discard if you don't need interaction with the particle after creation.

### Circle

```csharp
draw.Circle(origin, radius)
    .WithSegments(64)   // default 32 — higher = smoother
    .Color(Color.Red)
    .WithLifetime(10f)
    .Draw();
```

### Rectangle

```csharp
draw.Rectangle(origin, width, height)
    .Color(Color.Blue)
    .WithLifetime(5f)
    .Draw();
```

### Beam

```csharp
draw.Beam(startPos, endPos)
    .Color(Color.Yellow)
    .Draw();
```

---

## Beacons

Beacons loop on a player and default to team color (Red = T, Blue = CT, White = FFA). Returns an `ILoopTimer` — call
`.Stop()` to end it.

```csharp
// team color
var beacon = draw.Beacon(player).Start();

// color override
var beacon = draw.Beacon(player)
    .Color(Color.Purple)
    .WithOffset(16f)    // Z offset from ground, default 8
    .Start();

beacon.Stop();
```

CS2Draw automatically cleans up beacons on round end and player disconnect. You can also manage them manually:

```csharp
draw.HasBeacon(player);       // check
draw.RemoveBeacon(player);    // stop one
draw.RemoveAllBeacons();      // stop all
```

---

## Trails

Trails attach to any `CBaseEntity` and spawn a new particle on a repeating interval. Returns an `ILoopTimer`.

```csharp
var trail = draw.Trail(player.PlayerPawn.Value!)
    .Color(Color.Cyan)
    .WithInterval(2f)   // default 2s
    .Start();

trail.Stop(); // existing particles fade naturally
```

---

## Base Options

All drawables share these — chains in any order.

| Method                         | Default | Description                                                   |
|--------------------------------|---------|---------------------------------------------------------------|
| `.Color(Color, int cp = 1)`    | none    | Tint color and control point to apply it to                   |
| `.Particles(int count)`        | `20`    | Particle count — has per-shape constraints for a clean result |
| `.WithLifetime(float seconds)` | `5f`    | How long the shape stays visible                              |
| `.Infinite()`                  | —       | Permanent until `.Cancel()` is called                         |

---

## Handles and Cancellation

```csharp
// fire and forget
draw.Circle(origin, 50f).Color(Color.Green).Draw();

// hold for early cancel
var handle = draw.Circle(origin, 100f).Infinite().Draw();
if (handle.IsAlive) handle.Cancel();

// cancel everything
draw.CancelAll();
```

---

## Custom Shapes

### 1. Add your effect to `cs2draw.json`

```json
{
  "custom": {
    "my_star": "particles/path_to_star/star.vpcf"
  }
}
```

### 2. Implement `IShapeSetup`

```csharp
public sealed class StarSetup : IShapeSetup
{
    public string EffectKey => "my_star";

    public void Configure(IParticleConfigurator cp, int particleCount)
    {
        cp.SetCP(4, particleCount, 0, 0);
        cp.SetCP(5, outerRadius,   0, 0);
        cp.SetCP(6, innerRadius,   0, 0);
    }
}
```

### 3. Register and draw

```csharp
draw.RegisterShape(new StarSetup());

draw.Custom(origin, new StarSetup())
    .Color(Color.Yellow)
    .WithLifetime(8f)
    .Draw();
```

> `Configure()` is called after the particle is created and teleported, before `DispatchSpawn` and `Start`. CS2Draw
> handles everything else.

---

## Color and Control Points

Tint is applied via a control point on `CParticleSystem`. Default CP is `1`. Override if your particle expects it
elsewhere:

```csharp
draw.Circle(origin, 100f).Color(Color.Red, controlPoint: 3).Draw();
```

If your shapes always render the wrong color, _like always white_, your particle's tint CP is likely misconfigured.
See [PARTICLES.md](./PARTICLES.md) for guidance or reach out on Discord.

---

## Credits

**[Letaryat](https://github.com/Letaryat)** — His work
on [CS2-CustomTrailAndTracers](https://github.com/Letaryat/CS2-CustomTrailAndTracers) was the original inspiration for
CS2Draw and demonstrated what's truly possible with CS2's particle system. This project wouldn't exist without it.

---

## Questions?

Feel free to reach out on Discord or open an issue on [GitHub](https://github.com/ShookEagle/CS2Draw/issues).

If you need an example of a plugin using CS2Draw, check out [Jailbreak](https://github.com/edgegamers/Jailbreak).