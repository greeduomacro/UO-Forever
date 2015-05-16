#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using VitaNex;
using VitaNex.IO;

#endregion

namespace Server.Engines.Portals
{
    [CoreService("Portals", "1.0.0", TaskPriority.High)]
    public static partial class Portals
    {
        static Portals()
        {
            CSOptions = new PortalsOptions();

            PortalList = new BinaryDataStore<PortalSerial, Portal>(
                VitaNexCore.SavesDirectory + "/Portals", "PortalsList")
            {
                Async = true,
                OnSerialize = SerializePortals,
                OnDeserialize = DeserializePortals
            };

            PlayerProfiles = new BinaryDataStore<PlayerMobile, PlayerPortalProfile>(
                VitaNexCore.SavesDirectory + "/Portals", "PlayerProfiles")
            {
                Async = true,
                OnSerialize = SerializePlayerProfiles,
                OnDeserialize = DeserializePlayerProfiles
            };
        }

        private static void CSSave()
        {
            VitaNexCore.TryCatchGet(PlayerProfiles.Export, x => x.ToConsole());
            VitaNexCore.TryCatchGet(PortalList.Export, x => x.ToConsole());
        }

        private static void CSLoad()
        {
            VitaNexCore.TryCatchGet(PlayerProfiles.Import, x => x.ToConsole());
            VitaNexCore.TryCatchGet(PortalList.Import, x => x.ToConsole());
        }

        private static void CSInvoke()
        {
            CommandUtility.Register(
                "StartPortals",
                AccessLevel.Developer,
                e => StartPortals());

            CommandUtility.Register(
                "StopPortals",
                AccessLevel.Developer,
                e => StopPortals());

            foreach (Portal portal in PortalList.Values.Where(portal => portal.Status == PortalStatus.Running))
            {
                portal.init();
            }
        }

        private static bool SerializePortals(GenericWriter writer)
        {
            writer.SetVersion(0);

            writer.WriteBlockDictionary(
                PortalList,
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

        private static bool DeserializePortals(GenericReader reader)
        {
            reader.GetVersion();

            reader.ReadBlockDictionary(
                () =>
                {
                    var c = reader.ReadTypeCreate<Portal>(reader);

                    PortalSerial s = c != null ? c.UID : null;

                    return new KeyValuePair<PortalSerial, Portal>(s, c);
                },
                PortalList);

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

                    var p = new PlayerPortalProfile(reader);

                    return new KeyValuePair<PlayerMobile, PlayerPortalProfile>(e, p);
                },
                PlayerProfiles);

            return true;
        }
    }
}