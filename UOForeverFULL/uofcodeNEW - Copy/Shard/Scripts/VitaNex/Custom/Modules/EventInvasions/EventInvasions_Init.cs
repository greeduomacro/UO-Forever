#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using VitaNex;
using VitaNex.IO;

#endregion

namespace Server.Engines.EventInvasions
{
    [CoreService("Event Invasions", "1.0.0", TaskPriority.High)]
    public static partial class EventInvasions
    {
        static EventInvasions()
        {
            CSOptions = new EventInvasionsOptions();

            Invasions = new BinaryDataStore<InvasionSerial, Invasion>(
                VitaNexCore.SavesDirectory + "/EventInvasions", "Invasions")
            {
                Async = true,
                OnSerialize = SerializeInvasions,
                OnDeserialize = DeserializeInvasions
            };

            PlayerProfiles = new BinaryDataStore<PlayerMobile, PlayerInvasionProfile>(
                VitaNexCore.SavesDirectory + "/EventInvasions", "PlayerProfiles")
            {
                Async = true,
                OnSerialize = SerializePlayerProfiles,
                OnDeserialize = DeserializePlayerProfiles
            };
        }

        private static void CSSave()
        {
            VitaNexCore.TryCatchGet(PlayerProfiles.Export, x => x.ToConsole());
            VitaNexCore.TryCatchGet(Invasions.Export, x => x.ToConsole());
        }

        private static void CSLoad()
        {
            VitaNexCore.TryCatchGet(PlayerProfiles.Import, x => x.ToConsole());
            VitaNexCore.TryCatchGet(Invasions.Import, x => x.ToConsole());
        }

        private static void CSInvoke()
        {
            CommandUtility.Register(
                "Invade",
                AccessLevel.Player,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }
                    new InvasionUI(e.Mobile as PlayerMobile).Send();
                });
            CommandUtility.Register(
                "InvadeScore",
                AccessLevel.Administrator,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    PlayerInvasionProfile p = EnsureProfile(e.Mobile as PlayerMobile);
                    if (p != null)
                    {
                        e.Mobile.SendMessage("Overall Score: " + p.OverallScore);
                        foreach (var kvp in p.SpecificInvasionScores)
                        {
                            var invasion = GetInvasion(kvp.Key);
                            if (invasion != null)
                            {
                                e.Mobile.SendMessage(54, invasion.RegionName + ": " + kvp.Value);
                            }
                        }
                    }
                });
            foreach (var invasion in Invasions.Values.Where(invasion => invasion.Status == InvasionStatus.Running))
            {
                invasion.init();
            }
        }

        private static bool SerializeInvasions(GenericWriter writer)
        {
            writer.SetVersion(0);

            writer.WriteBlockDictionary(
                Invasions,
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

        private static bool DeserializeInvasions(GenericReader reader)
        {
            reader.GetVersion();

            reader.ReadBlockDictionary(
                () =>
                {
                    var c = reader.ReadTypeCreate<Invasion>(reader);

                    InvasionSerial s = c != null ? c.UID : null;

                    return new KeyValuePair<InvasionSerial, Invasion>(s, c);
                },
                Invasions);

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

                    var p = new PlayerInvasionProfile(reader);

                    return new KeyValuePair<PlayerMobile, PlayerInvasionProfile>(e, p);
                },
                PlayerProfiles);

            return true;
        }
    }
}