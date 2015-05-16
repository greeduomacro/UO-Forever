#region References

using System;
using System.Linq;
using Server.Engines.EventScores;
using Server.Gumps;
using Server.Mobiles;

#endregion

namespace VitaNex.SuperGumps.UI
{
    public class UserProfileUI : NoticeDialogGump
    {
        private PlayerEventScoreProfile UserProfile;

        public UserProfileUI(
            PlayerMobile user,
            PlayerEventScoreProfile profile,
            Gump parent = null,
            int? x = null,
            int? y = null,
            string title = null,
            string html = null,
            int icon = 7004,
            Action<GumpButton> onAccept = null,
            Action<GumpButton> onCancel = null)
            : base(user, parent, 400, 400, title, html, icon, onAccept, onCancel)
        {
            CanMove = true;
            Closable = true;
            Modal = false;
            UserProfile = profile;
        }

        protected override void CompileLayout(SuperGumpLayout layout)
        {
            layout.Add(
                "background",
                () =>
                {
                    AddBackground(27, 83, 424, 275, 9200);
                    AddImageTiled(426, 87, 21, 269, 10464);
                    AddImageTiled(29, 87, 21, 269, 10464);
                    AddImage(208, 44, 9000, 1359);
                    AddItem(227, 92, 9934, 1360);
                    AddItem(202, 92, 9935, 1360);

                    AddImage(419, 19, 10410);
                    AddImage(419, 162, 10411);
                    AddImage(420, 331, 10412);

                    AddLabel(81, 149, 137, @"Ultima Online Forever 1st Annual Battles Tournament");
                    AddImageTiled(64, 173, 345, 1, 5410);
                });

            layout.Add(
                "stats",
                () =>
                {
                    AddBackground(53, 179, 371, 176, 9270);
                    //AddAlphaRegion(65, 191, 345, 152);

                    AddLabel(66, 195, 2049, @"Your Stats");
                    AddImageTiled(64, 212, 76, 1, 5410);

                    AddLabel(68, 225, 1258, @"Character Name");
                    AddLabel(68, 250, 2049, UserProfile.DisplayCharacter.Name);

                    AddLabel(229, 225, 1258, @"Rank");
                    var Rank = EventScores.SortedProfiles().IndexOf(UserProfile) + 1;
                    if (Rank.ToString().Length == 1)
                    {
                        AddLabel(240, 250, 2049, Rank.ToString());
                    }
                    else if (Rank.ToString().Length == 2)
                    {
                        AddLabel(235, 250, 2049, Rank.ToString());
                    }
                    else
                    {
                        AddLabel(230, 250, 2049, Rank.ToString());
                    }

                    AddLabel(314, 225, 1258, @"Overall Points");

                    if (UserProfile.OverallScore.ToString().Length == 1)
                    {
                        AddLabel(352, 250, 2049, UserProfile.OverallScore.ToString());
                    }
                    else if (UserProfile.OverallScore.ToString().Length == 2)
                    {
                        AddLabel(349, 250, 2049, UserProfile.OverallScore.ToString());
                    }
                    else if (UserProfile.OverallScore.ToString().Length == 3)
                    {
                        AddLabel(347, 250, 2049, UserProfile.OverallScore.ToString());
                    }
                    else if (UserProfile.OverallScore.ToString().Length == 4)
                    {
                        AddLabel(344, 250, 2049, UserProfile.OverallScore.ToString());
                    }
                    else
                    {
                        AddLabel(341, 250, 2049, UserProfile.OverallScore.ToString());
                    }

                    AddLabel(68, 289, 1258, @"Battles Tournament Points");
                    if (UserProfile.SpendablePoints.ToString().Length == 1)
                    {
                        AddLabel(140, 314, 2049, UserProfile.SpendablePoints.ToString());
                    }
                    else if (UserProfile.SpendablePoints.ToString().Length == 2)
                    {
                        AddLabel(137, 314, 2049, UserProfile.SpendablePoints.ToString());
                    }
                    else if (UserProfile.SpendablePoints.ToString().Length == 3)
                    {
                        AddLabel(134, 314, 2049, UserProfile.SpendablePoints.ToString());
                    }
                    else if (UserProfile.SpendablePoints.ToString().Length == 4)
                    {
                        AddLabel(131, 314, 2049, UserProfile.SpendablePoints.ToString());
                    }
                    else
                    {
                        AddLabel(128, 314, 2049, UserProfile.SpendablePoints.ToString());
                    }

                    AddLabel(318, 289, 1258, @"Redeem Points");
                    AddButton(326, 314, 247, 248, b =>
                    {
                        var g = new BattlesRewardsUI(User, UserProfile, Refresh());
                        g.Send();
                    });

                });
        }
    }
}