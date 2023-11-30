using Content.Shared.Roles;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules.Components;

[RegisterComponent]
public sealed partial class FleshCultRuleComponent : Component
{
    public const string RuleId = "FleshCult";

    [DataField]
    public Dictionary<string, EntityUid> FleshCultists = new();

    /// <summary>
    /// This includes the cultist player himself. Count of ready players to start the mode.
    /// </summary>
    [DataField]
    public int MinPlayers = 8;

    /// <summary>
    /// This includes the cultist player himself. Something similar to the number of traitors.
    /// </summary>
    [DataField]
    public int PlayersPerCultist = 10;

    // TODO: I don’t even know what quantity will be OK.
    [DataField]
    public int MaxFleshCultists = 10;

    [DataField]
    public ProtoId<AntagPrototype> FleshCultistPrototypeId = "FleshCultist";

    // TODO: Replace it.
    [DataField]
    public SoundSpecifier FleshCultistStartSound = new SoundPathSpecifier("/Audio/Ambience/Antag/zombie_start.ogg");
}
