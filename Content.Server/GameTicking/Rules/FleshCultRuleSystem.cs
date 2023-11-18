using Content.Server.Antag;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Revolutionary.Components;
using Content.Server.Roles;
using Content.Shared.Chat;
using Content.Shared.FleshCult;
using Content.Shared.Mind;
using Content.Shared.Revolutionary.Components;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using System.Linq;

namespace Content.Server.GameTicking.Rules;

public sealed class FleshCultRuleSystem : GameRuleSystem<FleshCultRuleComponent>
{
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly AntagSelectionSystem _antagSelection = default!;
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly SharedJobSystem _jobs = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundStartAttemptEvent>(OnStartAttempt);
        SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(OnPlayerJobAssigned);
    }

    public void MakeFleshCultist(EntityUid mindId, MindComponent? mind = null)
    {
        if (!Resolve(mindId, ref mind))
            return;

        var rule = EntityQuery<FleshCultRuleComponent>().FirstOrDefault();
        if (rule == null)
        {
            GameTicker.StartGameRule(FleshCultRuleComponent.RuleId, out var ruleEnt);
            rule = Comp<FleshCultRuleComponent>(ruleEnt);
        }

        // TODO: Ignore NOT people, or make the target a person?
        if (!HasComp<HeadRevolutionaryComponent>(mind.OwnedEntity))
        {
            if (mind.OwnedEntity != null)
            {
                var player = new List<EntityUid>
                {
                    mind.OwnedEntity.Value
                };

                GiveFleshCultist(player, rule);
            }

            if (mind.Session != null)
            {
                var message = Loc.GetString("flesh-cult-role-greeting");
                var wrappedMessage = Loc.GetString("chat-manager-server-wrap-message", ("message", message));

                _chatManager.ChatMessageToOne(ChatChannel.Server, message, wrappedMessage, default, false, mind.Session.ConnectedClient, Color.FromHex("#ae424a"));
            }
        }
    }

    private void OnStartAttempt(RoundStartAttemptEvent ev)
    {
        var query = AllEntityQuery<FleshCultRuleComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var comp, out var gameRule))
        {
            _antagSelection.AttemptStartGameRule(ev, uid, comp.MinPlayers, gameRule);
        }
    }

    private void OnPlayerJobAssigned(RulePlayerJobsAssignedEvent ev)
    {
        var query = QueryActiveRules();
        while (query.MoveNext(out _, out var comp, out _))
        {
            // TODO: While he will take any race, this needs to be redone in the future.
            _antagSelection.EligiblePlayers(comp.FleshCultistPrototypeId, comp.MaxFleshCultists, comp.PlayersPerCultist, comp.FleshCultistStartSound, "flesh-cult-role-greeting", "#ae424a", out var chosen);

            if (chosen.Count > 0)
            {
                GiveFleshCultist(chosen, comp);
                continue;
            }

            _chatManager.SendAdminAnnouncement(Loc.GetString("flesh-cult-no-cultist"));
        }
    }

    private void GiveFleshCultist(List<EntityUid> chosen, FleshCultRuleComponent comp)
    {
        foreach (var fleshCultist in chosen)
        {
            GiveFleshCultist(fleshCultist, comp);
        }
    }

    private void GiveFleshCultist(EntityUid chosen, FleshCultRuleComponent comp)
    {
        // TODO: We should also remember his organs here.
        var inCharacterName = MetaData(chosen).EntityName;
        if (_mind.TryGetMind(chosen, out var mindId, out var mind))
        {
            if (!_role.MindHasRole<FleshCultistRoleComponent>(mindId))
            {
                _role.MindAddRole(mindId, new FleshCultistRoleComponent { PrototypeId = comp.FleshCultistPrototypeId });
            }

            if (mind.Session != null)
            {
                comp.FleshCultists.Add(inCharacterName, mindId);
            }
        }

        EnsureComp<FleshCultistComponent>(chosen);
    }
}
