// ┌─────────────────────────────────────────────────────────────────────────┐
// │  Example 06 — Custom Shape                                              │
// │  Shows: IShapeSetup, IParticleConfigurator, RegisterShape(),            │
// │         DrawCustom(), full extensibility pipeline                       │
// └─────────────────────────────────────────────────────────────────────────┘

using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CS2DrawShared;

namespace CS2Draw.Examples;

// ── Custom shape definition ───────────────────────────────────────────────
//
// A triangle — 3 sides, so valid particle count = (floor(n / 3) * 3) + 1
// e.g. 16 particles → floor(16/3) = 5 → (5 * 3) + 1 = 16 → CP2 = 16
//
// EffectKey must match an entry in cs2draw.json under "custom":
// {
//   "custom": {
//     "triangle": "particles/cs2draw_examples/triangle.vpcf"
//   }
// }

public sealed class TriangleSetup(float size) : IShapeSetup {
  public string EffectKey => "triangle";

  public void Configure(IParticleConfigurator cp, int particleCount) {
    // Snap particle count to a valid multiple of 3 + 1
    var perSide = Math.Max(1, particleCount / 3);
    var count   = perSide * 3 + 1;

    cp.SetCp(2, count, 0, 0); // particle count
    cp.SetCp(5, size, 0, 0);  // side length
  }
}

// ── Plugin ────────────────────────────────────────────────────────────────

public sealed class CustomShape : BasePlugin {
  public override string ModuleName => "CS2Draw Example 06 — Custom Shape";
  public override string ModuleVersion => "1.0.0";
  public override string ModuleAuthor => "";

  public override string ModuleDescription
    => "Registers and draws a custom triangle shape using IShapeSetup.";

  private static readonly PluginCapability<IDrawService> DRAW_CAPABILITY =
    new("cs2draw:service");

  private IDrawService? draw;

  public override void OnAllPluginsLoaded(bool hotReload) {
    draw = DRAW_CAPABILITY.Get();
    if (draw == null) {
      Console.WriteLine("[Example06] CS2Draw not loaded.");
      return;
    }

    // Register the custom shape once on the load.
    // CS2Draw now knows how to handle TriangleSetup instances.
    draw.RegisterShape(
      new TriangleSetup(
        0f)); // size doesn't matter here — just registering the key

    AddCommand("css_ex6_triangle", "Draw a triangle", OnTriangle);
    AddCommand("css_ex6_triangle_big", "Draw a large triangle", OnTriangleBig);
  }

  // Draws a small green triangle at the player's position.
  private void OnTriangle(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetOrigin(player, out var pos)) return;

    // A new TriangleSetup is passed each time with the desired size.
    // CS2Draw calls Configure() on it during the spawn sequence.
    draw!.Custom(pos, new TriangleSetup(100f))
     .Particles(16)
     .Color(Color.LimeGreen)
     .WithLifetime(10f)
     .Draw();

    player!.PrintToChat("[Example06] Triangle drawn — 100 unit size, 10s.");
  }

  // Draws a large purple triangle. Same shape, different params.
  // Shows that custom shapes are just data — swap the setup params per draw.
  private void OnTriangleBig(CCSPlayerController? player, CommandInfo info) {
    if (!tryGetOrigin(player, out var pos)) return;

    draw!.Custom(pos, new TriangleSetup(300f))
     .Particles(32)
     .Color(Color.Purple)
     .WithLifetime(15f)
     .Draw();

    player!.PrintToChat(
      "[Example06] Large triangle drawn — 300 unit size, 15s.");
  }

  private static bool
    tryGetOrigin(CCSPlayerController? player, out Vector pos) {
    pos = Vector.Zero;
    if (player == null || !player.IsValid
      || player.Pawn.Value?.AbsOrigin == null)
      return false;
    pos = player.Pawn.Value.AbsOrigin;
    return true;
  }
}