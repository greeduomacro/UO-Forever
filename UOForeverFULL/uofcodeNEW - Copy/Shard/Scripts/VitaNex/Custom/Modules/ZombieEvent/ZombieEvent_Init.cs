#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using VitaNex;
using VitaNex.IO;

#endregion

namespace Server.Engines.ZombieEvent
{
    [CoreService("Zombie Event", "1.0.0", TaskPriority.High)]
    public static partial class ZombieEvent
    {
        static ZombieEvent()
        {
            CSOptions = new ZombieEventOptions();

            ZombieEvents = new BinaryDataStore<ZombieInstanceSerial, ZombieInstance>(
                VitaNexCore.SavesDirectory + "/Zombies", "ZombieInstances")
            {
                Async = true,
                OnSerialize = SerializeZombieEvent,
                OnDeserialize = DeserializeZombieEvent
            };

            PlayerProfiles = new BinaryDataStore<PlayerMobile, PlayerZombieProfile>(
                VitaNexCore.SavesDirectory + "/Zombies", "PlayerProfiles")
            {
                Async = true,
                OnSerialize = SerializePlayerProfiles,
                OnDeserialize = DeserializePlayerProfiles
            };
        }

        private static void CSSave()
        {
            VitaNexCore.TryCatchGet(PlayerProfiles.Export, x => x.ToConsole());
            VitaNexCore.TryCatchGet(ZombieEvents.Export, x => x.ToConsole());
        }

        private static void CSLoad()
        {
            VitaNexCore.TryCatchGet(PlayerProfiles.Import, x => x.ToConsole());
            VitaNexCore.TryCatchGet(ZombieEvents.Import, x => x.ToConsole());
        }

        private static void CSInvoke()
        {
            CommandUtility.Register(
                "Zombieland",
                AccessLevel.Player,
                e =>
                {
                    PlayerMobile player = null;
                    ZombieAvatar avatar = null;
                    if (e.Mobile is PlayerMobile)
                    {
                        player = e.Mobile as PlayerMobile;
                    }
                    else if (e.Mobile is ZombieAvatar)
                    {
                        avatar = e.Mobile as ZombieAvatar;
                        if (avatar.Owner != null)
                        {
                            player = ((ZombieAvatar) e.Mobile).Owner;
                        }
                    }
                    if (player != null)
                    {
                        PlayerZombieProfile profile = EnsureProfile(player);
                        ZombieInstance instance = GetInstance();
                        if (instance != null || player.AccessLevel >= AccessLevel.GameMaster)
                        {
                            new ZombieEventUI(player, e.Mobile as ZombieAvatar, instance, profile).Send();
                        }
                        else
                        {
                            player.SendMessage(54, "Zombieland is currently inactive.");
                        }
                    }
                });

            CommandUtility.Register(
                "ZombieRewards",
                AccessLevel.Player,
                e =>
                {
                    PlayerMobile player = null;
                    if (e.Mobile is PlayerMobile)
                    {
                        player = e.Mobile as PlayerMobile;
                    }
                    else if (e.Mobile is ZombieAvatar)
                    {
                        e.Mobile.SendMessage(54, "You can only claim prizes in your player form!");
                        return;
                    }
                    if (player != null)
                    {
                        PlayerZombieProfile profile = EnsureProfile(player);
                        new ZombieEventRewardsUI(player, profile).Send();
                    }
                });

            CommandUtility.Register(
                "RecalculatePoints",
                AccessLevel.Developer,
                e =>
                {
                    foreach (PlayerZombieProfile playerZombieProfile in PlayerProfiles.Values)
                    {
                        playerZombieProfile.RecalculatePoints();
                    }
                });

            CommandUtility.Register(
                "FixInstance",
                AccessLevel.Developer,
                e =>
                {
                    PlayerZombieProfile profile = PlayerProfiles.Values.FirstOrDefault(x => x.OverallScore >= 1);
                    var instance = new ZombieInstance(profile.ZombieKills.Keys.FirstOrDefault());
                    ZombieEvents.Add(instance.Uid, instance);
                    instance.init();
                });

            CommandUtility.Register(
                "ZombieRespawn",
                AccessLevel.Developer,
                e =>
                {
                    ZombieInstance instance = GetInstance();
                    instance.RespawnEvent();
                });

            CommandUtility.Register(
                "ZombiePause",
                AccessLevel.Developer,
                e =>
                {
                    ZombieInstance instance = GetInstance();
                    if (instance != null)
                    {
                        PauseEvent();
                    }
                });

            CommandUtility.Register(
                "ZombieUnpause",
                AccessLevel.Developer,
                e =>
                {
                    ZombieInstance instance = GetPausedInstance();
                    if (instance != null)
                    {
                        UnpauseEvent();
                    }
                });

            CommandUtility.Register(
                "ResetZombieScore",
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
                        PlayerZombieProfile profile = EnsureProfile(mobile as PlayerMobile);
                        profile.SpendablePoints = 0;
                        profile.OverallScore = 0;
                        mobile.SendMessage(54,
                            "Your spendable zombie points and zombie score have been wiped by " + e.Mobile.RawName + ".");
                    }
                });

            CommandUtility.Register(
                "RewardtheCure",
                AccessLevel.Developer,
                e =>
                {
                    ZombieInstance instance = GetPausedInstance();
                    if (instance != null)
                    {
                        if (instance.CureWinner != null)
                        {
                            if (instance.CureWinner.BankBox != null)
                            {
                                var thecure = new TheCure();
                                instance.CureWinner.BankBox.DropItem(thecure);
                            }
                            var invalid = new List<PlayerMobile>();

                            foreach (PlayerMobile mobile in instance.CureCompleters.ToList())
                            {
                                PlayerZombieProfile profile = EnsureProfile(mobile);
                                if (invalid.Contains(mobile) || instance.CureWinner == mobile)
                                {
                                    instance.CureCompleters.Remove(mobile);
                                    profile.OverallScore -= 800;
                                    profile.SpendablePoints -= 800;
                                }
                                else
                                {
                                    invalid.Add(mobile);
                                }
                            }
                            if (instance.CureCompleters != null && instance.CureCompleters.Count > 0)
                            {
                                foreach (PlayerMobile mobile in instance.CureCompleters)
                                {
                                    var thecure = new TheCure();
                                    if (mobile != null && mobile.BankBox != null)
                                    {
                                        mobile.BankBox.DropItem(thecure);
                                    }
                                }
                            }
                        }
                    }
                });

            CommandUtility.Register(
                "GrantZombiePointsAll",
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

                    foreach (PlayerZombieProfile profile in PlayerProfiles.Values)
                    {
                        profile.Owner.SendMessage(54,
                            "You have been granted " + value + " spendable zombie points by " + e.Mobile.RawName + ".");
                        profile.SpendablePoints += value;
                    }
                });

            CommandUtility.RegisterAlias("Zombieland", "z");

            CommandUtility.Register(
                "GrantZombiePoints",
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
                        PlayerZombieProfile profile = EnsureProfile(mobile as PlayerMobile);
                        profile.SpendablePoints += value;
                        mobile.SendMessage(54,
                            "You have been granted " + value + " zombie points by " + e.Mobile.RawName + ".");
                        e.Mobile.SendMessage(54,
                            "You have granted " + value + " zombie points to " + mobile.RawName + ".");
                    }
                });

            foreach (
                ZombieInstance instance in
                    ZombieEvents.Values.Where(instance => instance.Status == ZombieEventStatus.Running))
            {
                instance.init();
            }
        }

        private static bool SerializeZombieEvent(GenericWriter writer)
        {
            writer.SetVersion(0);

            writer.WriteBlockDictionary(
                ZombieEvents,
                (key, val) => writer.WriteType(
                    val,
                    t =>
                    {
                        if (t != null)
                        {
                            val.Serialize(writer);
                        }
                    }));

            return true;
        }

        private static bool DeserializeZombieEvent(GenericReader reader)
        {
            reader.GetVersion();

            reader.ReadBlockDictionary(
                () =>
                {
                    var c = reader.ReadTypeCreate<ZombieInstance>(reader);

                    ZombieInstanceSerial s = c != null ? c.Uid : null;

                    return new KeyValuePair<ZombieInstanceSerial, ZombieInstance>(s, c);
                },
                ZombieEvents);

            return true;
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

                    var p = new PlayerZombieProfile(reader);

                    return new KeyValuePair<PlayerMobile, PlayerZombieProfile>(e, p);
                },
                PlayerProfiles);

            return true;
        }
    }
}