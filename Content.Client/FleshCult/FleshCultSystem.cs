using Content.Client.Antag;
using Content.Shared.StatusIcon.Components;
using Content.Shared.FleshCult;

namespace Content.Client.FleshCult;

public sealed class FleshCultSystem : AntagStatusIconSystem<FleshCultistComponent>
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FleshCultistComponent, GetStatusIconsEvent>(GetIcon);
    }

    // TODO: Does the ability to see the icons of other cultists become available from changing mutations?
    private void GetIcon(Entity<FleshCultistComponent> fleshCult, ref GetStatusIconsEvent args)
    {
        GetStatusIcon(fleshCult.Comp.FactionPrototypeId, ref args);
    }
}
