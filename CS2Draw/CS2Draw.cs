using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CS2Draw.Timers;
using CS2DrawShared;

namespace CS2Draw;

// ReSharper disable once InconsistentNaming
public class CS2Draw : BasePlugin, IPluginConfig<CS2DrawConfig> {
  public override string ModuleName => "CS2Draw";
  public override string ModuleVersion => "1.0.0";
  public override string ModuleAuthor => "ShookEagle";

  public override string ModuleDescription
    => "Centralized world-space shape rendering via particles.";

  public static readonly PluginCapability<IDrawService> CAPABILITY =
    new("cs2draw:service");

  public CS2DrawConfig Config { get; set; } = new();
  private DrawService? service;

  public override void Load(bool hotReload) {
    var timer = new TimerService(this);
    service = new DrawService(Config, timer, Logger);

    Capabilities.RegisterPluginCapability(CAPABILITY, () => service);
    RegisterListener<Listeners.OnServerPrecacheResources>(manifest => {
      foreach (var resource in Config.Shapes.Values) {
        manifest.AddResource(resource);
      }

      foreach (var resource in Config.Custom.Values) {
        manifest.AddResource(resource);
      }

      manifest.AddResource("soundevents/soundevents_beacon.vsndevts");
    });
  }

  public override void Unload(bool hotReload) { service?.CancelAll(); }

  public void OnConfigParsed(CS2DrawConfig config) {
    config.EnsureDefaults();
    Config = config;
  }

  [GameEventHandler]
  public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info) {
    service?.RemoveAllBeacons();
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event,
    GameEventInfo info) {
    if (@event.Userid != null) service?.RemoveBeacon(@event.Userid);

    return HookResult.Continue;
  }
}