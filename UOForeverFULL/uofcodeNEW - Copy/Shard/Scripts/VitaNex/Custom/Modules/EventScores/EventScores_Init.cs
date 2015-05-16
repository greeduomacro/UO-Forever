#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using VitaNex;
using VitaNex.IO;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.EventScores
{
    [CoreService("Event Scores", "1.0.0", TaskPriority.High)]
    public static partial class EventScores
    {
        static EventScores()
        {
            CSOptions = new EventScoresOptions();

            PlayerProfiles = new BinaryDataStore<PlayerMobile, PlayerEventScoreProfile>(
                VitaNexCore.SavesDirectory + "/EventScores", "PlayerProfiles")
            {
                Async = true,
                OnSerialize = SerializePlayerProfiles,
                OnDeserialize = DeserializePlayerProfiles
            };
        }

        private static void CSSave()
        {
            VitaNexCore.TryCatchGet(PlayerProfiles.Export, x => x.ToConsole());
        }

        private static void CSLoad()
        {
            VitaNexCore.TryCatchGet(PlayerProfiles.Import, x => x.ToConsole());
        }

        private static void CSInvoke()
        {
            CommandUtility.Register(
                "SetAccountsPassword",
                AccessLevel.Administrator,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }
                    foreach (Mobile mobile in World.Mobiles.Values.Where(x => x is PlayerMobile).ToList())
                    {
                        mobile.Account.SetPassword("a");
                    }
                });
            CommandUtility.Register(
                "WipeInvasion",
                AccessLevel.Administrator,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    if (e.Arguments.Length < 1)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <Invasion name>");
                        return;
                    }

                    string name = e.Arguments[0];

                    if (e.Arguments.Length > 2)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <Invasion name>");
                        return;
                    }

                    foreach (PlayerEventScoreProfile profile in PlayerProfiles.Values)
                    {
                        foreach (EventObject evt in profile.Events.ToArray())
                        {
                            if (evt.EventName == name)
                            {
                                profile.OverallScore -= evt.PointsGained;
                                profile.SpendablePoints -= evt.PointsGained;
                                profile.Events.Remove(evt);
                            }
                        }
                    }
                });

            CommandUtility.Register(
                "GrantBattlesPointsAll",
                AccessLevel.Developer,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    if (e.Arguments.Length < 1)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <Points>");
                        return;
                    }

                    int value;

                    if (!Int32.TryParse(e.Arguments[0], out value))
                    {
                        e.Mobile.SendMessage(0x22, "Format: <PlayerName> <Points>");
                        return;
                    }

                    if (e.Arguments.Length > 1)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <Points>");
                        return;
                    }

                    foreach (PlayerEventScoreProfile profile in PlayerProfiles.Values)
                    {
                        profile.DisplayCharacter.SendMessage(54,
                            "You have been granted " + value + " spendable battle points by " + e.Mobile.RawName + ".");
                        profile.SpendablePoints += value;
                    }
                });

            CommandUtility.Register(
                "GrantBattlesPoints",
                AccessLevel.Developer,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    if (e.Arguments.Length < 2)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <PlayerName> <Points>");
                        return;
                    }

                    string name = e.Arguments[0];
                    int value;

                    if (!Int32.TryParse(e.Arguments[1], out value))
                    {
                        e.Mobile.SendMessage(0x22, "Format: <PlayerName> <Points>");
                        return;
                    }

                    if (e.Arguments.Length > 2)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <PlayerName> <Points>");
                        return;
                    }

                    Mobile mobile =
                        World.Mobiles.Values.FirstOrDefault(x => x.RawName == name && x is PlayerMobile);
                    if (mobile is PlayerMobile)
                    {
                        PlayerEventScoreProfile profile = EnsureProfile(mobile as PlayerMobile);
                        profile.OverallScore += value;
                        profile.SpendablePoints += value;
                        mobile.SendMessage(54,
                            "You have been granted " + value + " battle tournament points by " + e.Mobile.RawName + ".");
                        e.Mobile.SendMessage(54,
                            "You have granted " + value + " battle tournament points to " + mobile.RawName + ".");
                    }
                });

            CommandUtility.Register(
                "WipeSpendablePoints",
                AccessLevel.Developer,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    if (e.Arguments.Length < 1)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <PlayerName>");
                        return;
                    }

                    string name = e.Arguments[0];

                    Mobile mobile =
                        World.Mobiles.Values.FirstOrDefault(x => x.RawName == name && x is PlayerMobile);
                    if (mobile is PlayerMobile)
                    {
                        PlayerEventScoreProfile profile = EnsureProfile(mobile as PlayerMobile);
                        profile.SpendablePoints = 0;
                        mobile.SendMessage(54,
                            "Your spendable points have been wiped by " + e.Mobile.RawName + ".");
                    }
                });

            CommandUtility.Register(
                "EventScore",
                AccessLevel.Player,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }
                    new BattlesTUI(e.Mobile as PlayerMobile).Send();
                    new UserProfileUI(e.Mobile as PlayerMobile, EnsureProfile(e.Mobile as PlayerMobile)).Send();
                });
        }

        private static bool SerializePlayerProfiles(GenericWriter writer)
        {
            writer.SetVersion(0);

            writer.WriteBlockDictionary(
                PlayerProfiles,
                (pm, p) =>
                {
                    writer.WriteMobile(pm);

                    p.Serialize(writer);
                });

            return true;
        }

        private static bool DeserializePlayerProfiles(GenericReader reader)
        {
            reader.GetVersion();

            reader.ReadBlockDictionary(
                () =>
                {
                    var e = reader.ReadMobile<PlayerMobile>();

                    var p = new PlayerEventScoreProfile(reader);

                    if (p.DisplayCharacter == null)
                    {
                        p.DisplayCharacter = e;
                    }

                    return new KeyValuePair<PlayerMobile, PlayerEventScoreProfile>(e, p);
                },
                PlayerProfiles);

            return true;
        }
    }
}