# Creating Particles for CS2Draw

CS2Draw renders shapes using custom `.vpcf` particle files built in the **CS2 Particle Editor Tool (PET)**. This guide covers the basics of building a compatible particle effect.

> For a real-world example of what's possible, see [Letaryat's CS2-CustomTrailAndTracers](https://github.com/Letaryat/CS2-CustomTrailAndTracers) — the project that inspired CS2Draw.

---

## Requirements

- CS2 installed via Steam
- 
- The **CS2 Particle Editor** — launch via Tools in your Steam library
- A mounted addon to save `.vpcf` files into

---

## How CS2Draw Uses Particles

CS2Draw creates a `CParticleSystem` entity, sets your `.vpcf` as the effect, then passes geometry data via **Control Points (CPs)** before spawning. Your particle file must be built to read those CPs and use them to draw the shape.

The general flow is:

```
CP2  → particle count / density
CP4  → shape-specific param (e.g. point count for a star)
CP5  → primary geometry (e.g. radius for circle, width for rectangle)
CP6  → secondary geometry (e.g. height for rectangle)
CP1  → tint color (TintCP default)
```

These are conventions used by the built-in shapes — your custom shapes can use any CPs your particle expects.

---

## Tint / Color Setup

For `.Color()` to work on your particle, you need to set up a **tint control point** in the PET:

1. In your particle's **Initializers**, add `Color Random` or `Color Lit Per Particle`
2. Set the **Color Input** source to `Control Point` and point it at CP `1` (or whichever CP you pass to `.Color()`)
3. This tells the particle to read its color from that CP at spawn time

If your shape always renders one color regardless of what you pass to `.Color()`, this is the likely cause.

---

## Lifetime

Particle lifetime is currently managed at the particle file level — set the lifetime in the PET directly on your emitter. The `.WithLifetime()` builder option is a TODO in CS2Draw and not yet wired to the particle system.

---

## Registering Your Particle

Once your `.vpcf` is built and mounted, add it to `cs2draw.json`:

```json
{
  "custom": {
    "my_shape": "particles/myaddon/my_shape.vpcf"
  }
}
```

Then implement `IShapeSetup` and set your CPs to match what your particle file expects. See the [README](./README.md#custom-shapes) for the full walkthrough.

---

## Help

If you're stuck on particle setup feel free to reach out on Discord or open an issue on [GitHub](https://github.com/ShookEagle/CS2Draw).