using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.FleshCult;

[RegisterComponent, NetworkedComponent]
public sealed partial class FleshCultistComponent : Component
{
    [DataField]
    public ProtoId<StatusIconPrototype> FactionPrototypeId = "FleshCultistFaction";
}
