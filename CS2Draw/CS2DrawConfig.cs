using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace CS2Draw;

// ReSharper disable once InconsistentNaming
public class CS2DrawConfig : BasePluginConfig {
  public static readonly IReadOnlyDictionary<string, string> DEFAULT_SHAPES =
    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
      ["circle"]     = "particles/shapes/shape_circle.vpcf",
      ["triangle"]   = "particles/shapes/shape_triangle.vpcf",
      ["square"]     = "particles/shapes/shape_square.vpcf",
      ["diamond"]    = "particles/shapes/shape_diamond.vpcf",
      ["pentagon"]   = "particles/shapes/shape_pentagon.vpcf",
      ["hexagon"]    = "particles/shapes/shape_hexagon.vpcf",
      ["star"]       = "particles/shapes/shape_star.vpcf",
      ["heart"]      = "particles/shapes/shape_heart.vpcf",
      ["among_us"]   = "particles/shapes/shape_among_us.vpcf",
      ["jelly_bean"] = "particles/shapes/shape_jelly_bean.vpcf",
      ["beam"]       = "particles/lines/two_point_beam.vpcf",
      ["beacon"]     = "particles/beacons/beacon.vpcf",
      ["trail"]      = "particles/trails/entity_trail.vpcf"
    };

  /// <summary>Effect names for built-in shapes.</summary>
  [JsonPropertyName("shapes")]
  public Dictionary<string, string> Shapes { get; set; } = new(DEFAULT_SHAPES,
    StringComparer.OrdinalIgnoreCase);

  /// <summary>Effect names for consumer-registered custom shapes.</summary>
  [JsonPropertyName("custom")]
  public Dictionary<string, string> Custom { get; set; } =
    new(StringComparer.OrdinalIgnoreCase);
  
  [JsonPropertyName("beacon_sound_event_file")]
  public string BeaconSoundEventFile { get; set; } = "soundevents/soundevents_beacon.vsndevts";
  
  [JsonPropertyName("beacon_sound_event")]
  public string BeaconSoundEvent { get; set; } = "generic.beacon.blip";

  public void EnsureDefaults() {
    foreach (var (key, value) in DEFAULT_SHAPES) { Shapes.TryAdd(key, value); }
  }

  public string? Resolve(string shapeName) {
    if (string.IsNullOrWhiteSpace(shapeName)) return null;

    if (Custom.TryGetValue(shapeName, out var customEffect))
      return customEffect;

    return Shapes.TryGetValue(shapeName, out var builtInEffect) ?
      builtInEffect :
      null;
  }
}