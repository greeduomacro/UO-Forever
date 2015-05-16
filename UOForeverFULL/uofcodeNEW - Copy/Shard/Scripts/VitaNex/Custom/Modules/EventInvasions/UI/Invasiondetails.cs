#region References

using System;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.EventInvasions
{
    public sealed class InvasionDetailsUI : DialogGump
    {
        public Invasion Invasion { get; set; }

        public InvasionDetailsUI(PlayerMobile user, Gump parent = null, Invasion invasion = null)
            : base(user, parent, 0, 0)
        {
            CanMove = true;
            Modal = false;

            ForceRecompile = true;

            Invasion = invasion;
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            //base.CompileLayout(layout);

            //Name
            layout.Add(
                "background",
                () =>
                {
                    AddBackground(0, 40, 775, 492, 2600);
                    AddImage(269, 18, 1419);
                    AddImage(346, 0, 1417);
                    AddImage(355, 9, 9012);
                    AddBackground(35, 100, 703, 373, 9270);
                    AddBackground(49, 112, 676, 348, 9200);
                });
            //Notes
            layout.Add(
                "InvasionTime",
                () =>
                {
                    AddLabel(58, 116, 0, Invasion.InvasionName + " Details");
                    AddLabel(59, 156, 0, @"Region Invaded");
                    AddLabel(198, 156, 0, @"Date Started");
                    AddLabel(198, 184, 0, Invasion.DateStarted.ToShortDateString());
                    AddLabel(333, 156, 0, @"Date Finished");
                    AddLabel(333, 184, 0, @"N/A");
                    AddLabel(602, 156, 0, @"# of Participants");
                    AddLabel(460, 156, 0, @"Invasion Status");
                    AddLabel(59, 184, 0, Invasion.RegionName);
                    AddLabel(460, 184, Invasion.Status.AsHue(), Invasion.Status.ToString());
                    AddLabel(602, 184, 0, Invasion.ParticipantsScores.Count.ToString());

                    if (Invasion.Status == InvasionStatus.Running)
                    {
                        AddLabel(59, 245, 0, @"Active Invaders");
                        AddLabel(60, 273, 0, Invasion.Invaders.Count.ToString());
                        AddLabel(198, 245, 0, @"Time Left");
                        AddLabel(198, 273, 0,
                            Invasion.CurrentLevel.TimeLimit != TimeSpan.Zero
                                ? Invasion.GetTimeLeft()
                                : "N/A");
                        AddLabel(333, 245, 0, @"Total Kills");
                        AddLabel(335, 273, 0,
                            Invasion.CurrentLevel.KillAmount != 0
                                ? Invasion.CurrentLevelKills + "/" + Invasion.CurrentLevel.KillAmount
                                : "N/A");
                        AddLabel(460, 245, 0, @"Boss Spawned");
                        AddLabel(460, 273, 0, Invasion.CurrentLevel == Invasion.Levels.LastOrDefault() ? "Yes" : "No");
                    }

                    if (Invasion.Status == InvasionStatus.Running || Invasion.Status == InvasionStatus.Finished)
                    {
                        AddLabel(602, 245, 0, @"View Scoreboard");
                        AddButton(602, 271, 247, 248,
                            b => Send(new InvasionScoresOverviewGump(User, Invasion, Refresh(true))));
                    }

                    if (User.AccessLevel >= EventInvasions.Access)
                    {
                        AddLabel(559, 435, 0, @"Edit Invasion?");
                        AddButton(653, 434, 247, 248, b => Send(new EditAdminUI(User, Hide(true), Invasion)));

                        if (Invasion.Status == InvasionStatus.Running)
                        {
                            if (Invasion.Levels.Last() != Invasion.CurrentLevel && Invasion.Levels.Count > 1)
                            {
                                AddLabel(66, 435, 0, @"Skip Level?");
                                AddButton(145, 434, 247, 248, b =>
                                {
                                    Invasion.ForceIncrementLevel(); Refresh(true);
                                });
                            }
                            AddLabel(213, 435, 0, @"Restart Level?");
                            AddButton(315, 434, 247, 248, b => { Invasion.RestartLevel(); Refresh(true); });
                            AddLabel(390, 435, 0, @"Force Respawn?");
                            AddButton(490, 434, 247, 248, b => { Invasion.ForceSpawnInvaders(); Refresh(true); });
                        }
                    }
                });
        }

        protected override void OnCancel(GumpButton button)
        {}
    }
}