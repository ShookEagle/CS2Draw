# CS2Draw
CS2Draw is a centralized world-space shape rendering service for CS2 plugins. CS2Draw handles the particles, lifetime, and cleanup. Particles Must be created using an addon. For help feel free to reach out to me on discord or chekc the examples.

# CS2Draw.Shared
The shared contracts library for CS2Draw. This is the **only** project your plugin needs to reference.

## Setup
In your plugin, declare the capability reference using the same key CS2Draw registers under:

```csharp
using CS2Draw.Contracts;
using CounterStrikeSharp.API.Core.Capabilities;

public sealed class ConsumerPlugin : BasePlugin
{
    private static readonly PluginCapability<IDrawService> DrawCapability = new("cs2draw:service");

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        var draw = DrawCapability.Get();
        if (draw == null)
        {
            Console.WriteLine("[MyPlugin] CS2Draw not loaded.");
            return;
        }

        // you're ready to draw
    }
}
```

> CS2Draw must be loaded before your plugin calls `.Get()`. Use `OnAllPluginsLoaded` not `Load`.

---

## Drawing Built-in Shapes

Every draw call follows the same pattern:

```
service.Shape(origin, ...geometry params...)
    .ChainOptions()
    .Draw();
```

`.Draw()` returns an `IDrawHandle`. It is safe to discard if you don't need early cancellation.

---

### Circle

```csharp
draw.Circle(origin, radius);
```

| Parameter | Type     | Description                        |
|-----------|----------|------------------------------------|
| origin    | `Vector` | World position of the circle center |
| radius    | `float`  | Radius in world units              |

**Shape-specific options**

| Method                  | Default | Description                              |
|-------------------------|---------|------------------------------------------|
| `.WithSegments(int n)`  | `32`    | Line segments approximating the circle. Higher = smoother. |

**Examples**

```csharp
// minimal
draw.Circle(origin, 100f).Draw();

// fully configured
draw.Circle(origin, 100f)
    .WithSegments(64)
    .Color(Color.Red)
    .WithLifetime(10f)
    .Particles(30)
    .Draw();

// permanent until cancelled
var handle = draw.Circle(origin, 100f)
    .Color(Color.White)
    .Infinite()
    .Draw();

handle.Cancel(); // remove it early
```

---

### Rectangle

```csharp
draw.Rectangle(origin, width, height);
```

| Parameter | Type     | Description                              |
|-----------|----------|------------------------------------------|
| origin    | `Vector` | World position of the rectangle center   |
| width     | `float`  | Width in world units                     |
| height    | `float`  | Height in world units                    |

**Examples**

```csharp
// minimal
draw.Rectangle(origin, 200f, 100f).Draw();

// fully configured
draw.Rectangle(origin, 200f, 100f)
    .Color(Color.Blue)
    .WithLifetime(5f)
    .Draw();
```

---

## Base Options

All shapes share these options. They can be chained in any order.

| Method                          | Default | Description                                                                                       |
|---------------------------------|---------|---------------------------------------------------------------------------------------------------|
| `.Color(Color, int cp = 0)`     | null    | Tint color and the control point to apply it to. CP defaults to `1`.                              |
| `.Particles(int count)`         | `20`    | Number of particles to spawn for this shape. Often has contraints per shape to get a clean shape. |
| `.WithLifetime(float seconds)`  | `5f`    | How long the shape stays visible.                                                                 |
| `.Infinite()`                   | —       | Keep the shape alive until `.Cancel()` is called on the handle.                                   |

---

## Handles

Every `.Draw()` call returns an `IDrawHandle`. You can discard it for fire-and-forget, or hold it for control.

```csharp
public interface IDrawHandle
{
    Guid Id      { get; }
    bool IsAlive { get; }
    void Cancel();
}
```

**Examples**

```csharp
// fire and forget
draw.Circle(origin, 50f).Color(Color.Green).Draw();

// hold and cancel
var handle = draw.Rectangle(origin, 200f, 100f)
    .Infinite()
    .Draw();

// later...
if (handle.IsAlive)
    handle.Cancel();
```

---

## Bulk Cancellation

Cancel all active drawings at once:

```csharp
draw.CancelAll();
```

Useful for round end cleanup, player disconnect, or any event where all visuals should be cleared.

---

## Custom Shapes

If you need a shape CS2Draw doesn't provide, you can register your own. CS2Draw still owns the spawn sequence — you only define the control points.

### Step 1 — Add your effect name to `cs2draw.json`

```json
{
  "custom": {
    "my_star": "particles/myaddon/star.vpcf"
  }
}
```

### Step 2 — Implement `IShapeSetup`

```csharp
using CS2Draw.Contracts;

public sealed class StarSetup : IShapeSetup
{
    private readonly int   _points;
    private readonly float _outerRadius;
    private readonly float _innerRadius;

    public string EffectKey => "my_star";

    public StarSetup(int points, float outerRadius, float innerRadius)
    {
        _points      = points;
        _outerRadius = outerRadius;
        _innerRadius = innerRadius;
    }

    public void Configure(IParticleConfigurator cp)
    {
        // set whatever control points your particle file expects
        cp.SetCP(4, _points, 0, 0);
        cp.SetCP(5, _outerRadius, 0, 0);
        cp.SetCP(6, _innerRadius, 0, 0);
    }
}
```

### Step 3 — Register and draw

```csharp
public override void Load(bool hotReload)
{
    var draw = DrawCapability.Get();

    // register once
    draw.RegisterShape(new StarSetup(5, 100f, 50f));

    // draw it — gets the full builder chain
    draw.Custom(origin, new StarSetup(5, 100f, 50f))
        .Color(Color.Yellow)
        .WithLifetime(8f)
        .Draw();
}
```

> `Configure()` is called after the particle is created and teleported, before `DispatchSpawn` and `Start`. You only set control points — CS2Draw handles everything else.

---

## Color and Control Points

CS2 particles apply tint via a specific control point (`TintCP`) on the `CParticleSystem`. By default CS2Draw applies color to CP `1`. If your particle file expects the tint on a different CP, pass it explicitly:

```csharp
draw.Circle(origin, 100f)
    .Color(Color.Red, controlPoint: 3)
    .Draw();
```

If you don't call `.Color()` at all, no tint is applied and the particle renders with its baked-in color. If particles are alwasy rendering with a particular color  its likely that you've not got your colors setup right. Check the example or feel free to reach out to me on discord.

---