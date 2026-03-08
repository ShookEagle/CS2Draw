namespace CS2Draw.Utils;

/// <summary>
/// Particle count calculations for shapes drawn with CS2 particles.
/// Each shape has a different number of sides which determines the valid CP value.
/// 
/// Formula: floor(requestedCount / sides) * sides + 1
/// 
/// Example: 16 particles on a rectangle (4 sides)
///   16 / 4 = 4 per side → (4 * 4) + 1 = 17 → CP2 = 17
/// </summary>
internal static class ParticleMath {
  /// <summary>
  /// Calculate the correct particle count CP value for a given shape.
  /// Snaps the requested count down to the nearest valid multiple for that shape.
  /// </summary>
  /// <param name="requestedCount">The count passed in by the caller.</param>
  /// <param name="sides">Number of sides/segments the shape has.</param>
  public static int CalcCount(int requestedCount, int sides) {
    var perSide = Math.Max(1, requestedCount / sides);
    return perSide * sides + 1;
  }
}