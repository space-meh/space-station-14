using Content.Shared.Body.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Body.Organ;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedBodySystem))]
public sealed partial class OrganComponent : Component
{
    /// <summary>
    /// Relevant body this organ is attached to.
    /// </summary>
    [DataField("body"), AutoNetworkedField]
    public EntityUid? Body;

    /// <summary>
    /// The abilities that the bearer receives from this organ
    /// </summary>
    [DataField]
    public List<ProtoId<EntityPrototype>> OrganActions = new();

    /// <summary>
    ///  links to the created action entities.
    /// </summary>
    [DataField]
    public List<EntityUid> OrganSpawnedActions = new();
}
