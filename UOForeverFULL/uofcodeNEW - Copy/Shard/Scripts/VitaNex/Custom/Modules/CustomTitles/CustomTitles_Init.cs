#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Commands;
using Server.Mobiles;
using Server.Mobiles.MetaPet.Skills;
using Server.Mobiles.MetaSkills;
using Server.Network;
using VitaNex;
using VitaNex.IO;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.CustomTitles
{
    [CoreModule("Custom Titles", "1.0.1", true, TaskPriority.High)]
    public static partial class CustomTitles
    {
        static CustomTitles()
        {
            Rarities = ((TitleRarity) 0).GetValues<TitleRarity>();

            TitleRegistry = new BinaryDataStore<TitleObjectSerial, Title>(VitaNexCore.SavesDirectory + "/CustomTitles",
                "Titles")
            {
                Async = true,
                OnSerialize = SerializeTitleRegistry,
                OnDeserialize = DeserializeTitleRegistry
            };

            HueRegistry = new BinaryDataStore<TitleObjectSerial, TitleHue>(
                VitaNexCore.SavesDirectory + "/CustomTitles", "Hues")
            {
                Async = true,
                OnSerialize = SerializeHueRegistry,
                OnDeserialize = DeserializeHueRegistry
            };

            Profiles = new BinaryDataStore<PlayerMobile, TitleProfile>(VitaNexCore.SavesDirectory + "/CustomTitles",
                "Profiles")
            {
                Async = true,
                OnSerialize = SerializeProfiles,
                OnDeserialize = DeserializeProfiles
            };
        }

        private static void CMConfig()
        {
            LookReqParent = PacketHandlers.GetHandler(0x09);

            PacketHandlers.Register(0x09, 5, true, OnSingleClick);
            PacketHandlers.Register6017(0x09, 5, true, OnSingleClick);
        }

        private static void CMInvoke()
        {
            CommandUtility.Register(
                "AddCustomTitle",
                AccessLevel.Administrator,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    if (!CMOptions.ModuleEnabled)
                    {
                        e.Mobile.SendMessage(0x22, "The Custom Titles module is currently disabled.");
                        return;
                    }

                    if (e.Arguments.Length < 2)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <maleTitle> <femaleTitle> [rarity]");
                        return;
                    }

                    string maleValue = e.Arguments[0];
                    string femaleValue = e.Arguments[1];

                    var rarity = TitleRarity.Common;

                    if (e.Arguments.Length > 2 && !Enum.TryParse(e.Arguments[2], true, out rarity))
                    {
                        e.Mobile.SendMessage(0x22, "Format: <maleTitle> <femaleTitle> [rarity]");
                        return;
                    }

                    var display = TitleDisplay.BeforeName;

                    if (e.Arguments.Length > 3 && !Enum.TryParse(e.Arguments[3], true, out display))
                    {
                        e.Mobile.SendMessage(0x22, "Format: <maleTitle> <femaleTitle> [rarity] [display]");
                        return;
                    }

                    string result;

                    Title title = CreateTitle(maleValue, femaleValue, rarity, display, out result);

                    e.Mobile.SendMessage(title == null ? 0x22 : 0x33, result);
                });

            CommandUtility.Register(
                "AddCustomTitleHue",
                AccessLevel.Administrator,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    if (!CMOptions.ModuleEnabled)
                    {
                        e.Mobile.SendMessage(0x22, "The Custom Titles module is currently disabled.");
                        return;
                    }

                    if (e.Arguments.Length == 0)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <hue> [rarity]");
                        return;
                    }

                    int value;

                    if (!Int32.TryParse(e.Arguments[0], out value))
                    {
                        e.Mobile.SendMessage(0x22, "Format: <hue> [rarity]");
                        return;
                    }

                    var rarity = TitleRarity.Common;

                    if (e.Arguments.Length > 1 && !Enum.TryParse(e.Arguments[1], true, out rarity))
                    {
                        e.Mobile.SendMessage(0x22, "Format: <hue> [rarity]");
                        return;
                    }

                    string result;

                    TitleHue hue = CreateHue(value, rarity, out result);

                    e.Mobile.SendMessage(hue == null ? 0x22 : 0x33, result);
                });

            CommandUtility.Register(
                "WipeCustomTitles",
                AccessLevel.Administrator,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    if (!CMOptions.ModuleEnabled)
                    {
                        e.Mobile.SendMessage(0x22, "The Custom Titles module is currently disabled.");
                        return;
                    }

                    new ConfirmDialogGump((PlayerMobile) e.Mobile)
                    {
                        Title = "Confirm Action: Wipe",
                        Html =
                            "This action will wipe all titles, title hues and title profiles.\nThis action can not be reversed!\n\nClick OK to confirm.",
                        AcceptHandler = b =>
                        {
                            Profiles.Values.ForEach(p => VitaNexCore.TryCatch(p.Clear));
                            Profiles.Clear();
                            e.Mobile.SendMessage("All title profiles have been cleared.");

                            HueRegistry.Values.ForEach(h => VitaNexCore.TryCatch(h.Clear));
                            HueRegistry.Clear();
                            e.Mobile.SendMessage("All title hues have been cleared.");

                            TitleRegistry.Values.ForEach(t => VitaNexCore.TryCatch(t.Clear));
                            TitleRegistry.Clear();
                            e.Mobile.SendMessage("All titles have been cleared.");
                        }
                    }.Send();
                });

            CommandUtility.Register(
                "GrantCustomTitles",
                AccessLevel.Administrator,
                e =>
                {
                    if (!CMOptions.ModuleEnabled)
                    {
                        e.Mobile.SendMessage(0x22, "The Custom Titles module is currently disabled.");
                        return;
                    }

                    GrantTitlesTarget(e.Mobile as PlayerMobile);
                });

            CommandUtility.Register(
                "RevokeCustomTitles",
                AccessLevel.Administrator,
                e =>
                {
                    if (!CMOptions.ModuleEnabled)
                    {
                        e.Mobile.SendMessage(0x22, "The Custom Titles module is currently disabled.");
                        return;
                    }

                    RevokeTitlesTarget(e.Mobile as PlayerMobile);
                });

            CommandUtility.Register(
                "pg",
                AccessLevel.Player,
                e =>
                {
                    if (e.Mobile is PlayerMobile && e.Mobile.Map != Map.ZombieLand)
                    {
                        var player = e.Mobile as PlayerMobile;
                        if (player.RawName == "Savo-" || player.RawName == "Savo" || player.RawName == "Tsavo" ||
                            player.RawName == "a evil savo" || player.RawName == "a pirate savo" || player.RawName == "Tsavo-")
                        {
                            player.Frozen = true;
                            if (!player.Mounted)
                                player.Animate(239, 7, 1, true, false, 0);
                            player.PublicOverheadMessage(MessageType.Spell, player.SpeechHue, true, "Kal Ort Por", false);
                            Timer.DelayCall(TimeSpan.FromSeconds(0.75 + (0.25 * 3)), () =>
                            {
                                player.Frozen = false;
                                player.Hidden = true;
                                BaseCreature.TeleportPets(player, new Point3D(2977, 2893, -4), Map.Felucca, false);
                                player.MoveToWorld(new Point3D(2977, 2893, -4), Map.Felucca);
                            });
                        }
                    }
                });

            CommandUtility.Register(
                "he",
                AccessLevel.Player,
                e =>
                {
                    if (e.Mobile is PlayerMobile && e.Mobile.Map != Map.ZombieLand)
                    {
                        var player = e.Mobile as PlayerMobile;
                        if (player.RawName == "Savo-" || player.RawName == "Savo" || player.RawName == "Tsavo" ||
                            player.RawName == "a evil savo" || player.RawName == "a pirate savo" || player.RawName == "Tsavo-")
                        {
                            player.Frozen = true;
                            if (!player.Mounted)
                                player.Animate(239, 7, 1, true, false, 0);
                            player.PublicOverheadMessage(MessageType.Spell, player.SpeechHue, true, "Kal Ort Por", false);
                            Timer.DelayCall(TimeSpan.FromSeconds(0.75 + (0.25 * 3)), () =>
                            {
                                player.Frozen = false;
                                player.Hidden = true;
                                BaseCreature.TeleportPets(player, new Point3D(2318, 3755, 0), Map.Felucca, false);
                                player.MoveToWorld(new Point3D(2318, 3755, 0), Map.Felucca);
                            });
                        }
                    }
                });

            CommandUtility.Register(
                "CustomTitles",
                AccessLevel.Player,
                e =>
                {
                    if (!CMOptions.ModuleEnabled)
                    {
                        e.Mobile.SendMessage(0x22, "The Custom Titles module is currently disabled.");
                        return;
                    }

                    SendTitlesGump(e.Mobile as PlayerMobile);
                });

            CommandUtility.Register(
                "ConvertMetaDragons",
                AccessLevel.Developer,
                e =>
                {
                    foreach (Mobile mob in World.Mobiles.Values.Where(x => x is EvolutionDragon).ToArray())
                    {
                        var dragon = mob as EvolutionDragon;
                        if (dragon != null)
                        {
                            var newmeta = new MetaDragon();
                            newmeta.Location = dragon.Location;
                            newmeta.Map = dragon.Map;
                            newmeta.Loyalty = 100;
                            newmeta.ControlMaster = dragon.ControlMaster;
                            newmeta.Controlled = true;
                            newmeta.ControlTarget = null;
                            newmeta.ControlOrder = OrderType.Come;
                            newmeta.IsBonded = true;
                            newmeta.Hue = dragon.Hue;
                            newmeta.RawStr = dragon.RawStr;
                            newmeta.RawDex = dragon.RawDex;
                            newmeta.RawInt = dragon.RawInt;
                            newmeta.Name = dragon.Name;

                            newmeta.Stage = dragon.Stage;
                            newmeta.MaxStage = 7;
                            newmeta.EvolutionPoints = dragon.EvolutionPoints;

                            newmeta.Metaskills = new Dictionary<MetaSkillType, BaseMetaSkill>();

                            dragon.Delete();
                        }
                    }
                });

            CommandUtility.Register(
                "GetAllItems",
                AccessLevel.Developer,
                e =>
                {
                    var itemsdict = new Dictionary<int, int>();
                    foreach (Item item in World.Items.Values.Where(x => x.Movable || x.IsLockedDown).ToArray())
                    {
                        if (itemsdict.ContainsKey(item.ItemID))
                        {
                            itemsdict[item.ItemID]++;
                        }
                        else
                        {
                            itemsdict.Add(item.ItemID, 1);
                        }
                    }

                    foreach (KeyValuePair<int, int> kvp in itemsdict.OrderBy(i => i.Value))
                    {
                        var sb = new StringBuilder();

                        sb.Append("ItemID: " + kvp.Key + "---------------Count: " + kvp.Value + "\n");
                        LoggingCustom.Log("ItemsLog/" + IOUtility.GetSafeFileName("Itemslog") + ".log", sb.ToString());
                    }
                });

            CommandUtility.RegisterAlias("CustomTitles", "Titles");

            CommandUtility.Register("FlameSpiral", AccessLevel.GameMaster, e => BeginTarget(e.Mobile));
            CommandUtility.Register("FlashEffect", AccessLevel.GameMaster, e => DoFlash(e.Mobile));
            CommandUtility.Register("UpgradeAccounts", AccessLevel.Developer, e => UpgradeAccounts());
            CommandUtility.Register("LockedDownFix", AccessLevel.Developer, e => LockedDownFix());
        }

        private static void CMSave()
        {
            Save();
        }

        private static void CMLoad()
        {
            Load();
        }

        public static void Save()
        {
            VitaNexCore.TryCatch(SaveTitleRegistry, CMOptions.ToConsole);
            VitaNexCore.TryCatch(SaveHueRegistry, CMOptions.ToConsole);

            // Profiles should always be last.
            VitaNexCore.TryCatch(SaveProfiles, CMOptions.ToConsole);
        }

        public static void Load()
        {
            VitaNexCore.TryCatch(LoadTitleRegistry, CMOptions.ToConsole);
            VitaNexCore.TryCatch(LoadHueRegistry, CMOptions.ToConsole);

            // Profiles should always be last.
            VitaNexCore.TryCatch(LoadProfiles, CMOptions.ToConsole);
        }

        #region Write/Read Methods for Title and TitleHue

        public static void WriteTitle(GenericWriter writer, Title title)
        {
            if (title != null)
            {
                writer.Write(true);

                title.UID.Serialize(writer);
            }
            else
            {
                writer.Write(false);
            }
        }

        public static Title ReadTitle(GenericReader reader)
        {
            if (!reader.ReadBool())
            {
                return null;
            }

            var uid = new TitleObjectSerial(reader);

            Title title;

            if (TitleRegistry.TryGetValue(uid, out title))
            {
                return title;
            }

            return null;
        }

        public static void WriteTitleHue(GenericWriter writer, TitleHue title)
        {
            if (title != null)
            {
                writer.Write(true);

                title.UID.Serialize(writer);
            }
            else
            {
                writer.Write(false);
            }
        }

        public static TitleHue ReadTitleHue(GenericReader reader)
        {
            if (!reader.ReadBool())
            {
                return null;
            }

            var uid = new TitleObjectSerial(reader);

            TitleHue hue;

            return HueRegistry.TryGetValue(uid, out hue) ? hue : null;
        }

        #endregion

        #region Titles Save/Load

        public static void SaveTitleRegistry()
        {
            DataStoreResult result = TitleRegistry.Export();
            CMOptions.ToConsole("Result: {0}", result.ToString());

            switch (result)
            {
                case DataStoreResult.Null:
                case DataStoreResult.Busy:
                case DataStoreResult.Error:
                {
                    if (TitleRegistry.HasErrors)
                    {
                        CMOptions.ToConsole("Titles database has errors...");

                        TitleRegistry.Errors.ForEach(CMOptions.ToConsole);
                    }
                }
                    break;
                case DataStoreResult.OK:
                    CMOptions.ToConsole("Titles count: {0:#,0}", TitleRegistry.Count);
                    break;
            }
        }

        public static void LoadTitleRegistry()
        {
            DataStoreResult result = TitleRegistry.Import();
            CMOptions.ToConsole("Result: {0}", result.ToString());

            switch (result)
            {
                case DataStoreResult.Null:
                case DataStoreResult.Busy:
                case DataStoreResult.Error:
                {
                    if (TitleRegistry.HasErrors)
                    {
                        CMOptions.ToConsole("Titles database has errors...");

                        TitleRegistry.Errors.ForEach(CMOptions.ToConsole);
                    }
                }
                    break;
                case DataStoreResult.OK:
                    CMOptions.ToConsole("Titles count: {0:#,0}", TitleRegistry.Count);
                    break;
            }
        }

        private static bool SerializeTitleRegistry(GenericWriter writer)
        {
            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                    writer.WriteBlockDictionary(TitleRegistry, (s, t) => t.Serialize(writer));
                    break;
            }

            return true;
        }

        private static bool DeserializeTitleRegistry(GenericReader reader)
        {
            int version = reader.GetVersion();

            switch (version)
            {
                case 0:
                {
                    reader.ReadBlockDictionary(
                        () =>
                        {
                            var t = new Title(reader);
                            TitleObjectSerial s = t.UID;

                            return new KeyValuePair<TitleObjectSerial, Title>(s, t);
                        },
                        TitleRegistry);
                }
                    break;
            }

            return true;
        }

        #endregion

        #region Hues Save/Load

        public static void SaveHueRegistry()
        {
            DataStoreResult result = HueRegistry.Export();
            CMOptions.ToConsole("Result: {0}", result.ToString());

            switch (result)
            {
                case DataStoreResult.Null:
                case DataStoreResult.Busy:
                case DataStoreResult.Error:
                {
                    if (HueRegistry.HasErrors)
                    {
                        CMOptions.ToConsole("Hues database has errors...");

                        HueRegistry.Errors.ForEach(CMOptions.ToConsole);
                    }
                }
                    break;
                case DataStoreResult.OK:
                    CMOptions.ToConsole("Hue count: {0:#,0}", HueRegistry.Count);
                    break;
            }
        }

        public static void LoadHueRegistry()
        {
            DataStoreResult result = HueRegistry.Import();
            CMOptions.ToConsole("Result: {0}", result.ToString());

            switch (result)
            {
                case DataStoreResult.Null:
                case DataStoreResult.Busy:
                case DataStoreResult.Error:
                {
                    if (HueRegistry.HasErrors)
                    {
                        CMOptions.ToConsole("Hues database has errors...");

                        HueRegistry.Errors.ForEach(CMOptions.ToConsole);
                    }
                }
                    break;
                case DataStoreResult.OK:
                    CMOptions.ToConsole("Hues count: {0:#,0}", HueRegistry.Count);
                    break;
            }
        }

        private static bool SerializeHueRegistry(GenericWriter writer)
        {
            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                    writer.WriteBlockDictionary(HueRegistry, (key, val) => val.Serialize(writer));
                    break;
            }

            return true;
        }

        private static bool DeserializeHueRegistry(GenericReader reader)
        {
            int version = reader.GetVersion();

            switch (version)
            {
                case 0:
                {
                    reader.ReadBlockDictionary(
                        () =>
                        {
                            var h = new TitleHue(reader);
                            TitleObjectSerial s = h.UID;

                            return new KeyValuePair<TitleObjectSerial, TitleHue>(s, h);
                        },
                        HueRegistry);
                }
                    break;
            }

            return true;
        }

        #endregion

        #region Profiles Save/Load

        public static void SaveProfiles()
        {
            DataStoreResult result = Profiles.Export();
            CMOptions.ToConsole("Result: {0}", result.ToString());

            switch (result)
            {
                case DataStoreResult.Null:
                case DataStoreResult.Busy:
                case DataStoreResult.Error:
                {
                    if (Profiles.HasErrors)
                    {
                        CMOptions.ToConsole("Profiles database has errors...");

                        Profiles.Errors.ForEach(CMOptions.ToConsole);
                    }
                }
                    break;
                case DataStoreResult.OK:
                    CMOptions.ToConsole("Profile count: {0:#,0}", Profiles.Count);
                    break;
            }
        }

        public static void LoadProfiles()
        {
            DataStoreResult result = Profiles.Import();
            CMOptions.ToConsole("Result: {0}", result.ToString());

            switch (result)
            {
                case DataStoreResult.Null:
                case DataStoreResult.Busy:
                case DataStoreResult.Error:
                {
                    if (Profiles.HasErrors)
                    {
                        CMOptions.ToConsole("Profiles database has errors...");

                        Profiles.Errors.ForEach(CMOptions.ToConsole);
                    }
                }
                    break;
                case DataStoreResult.OK:
                    CMOptions.ToConsole("Profile count: {0:#,0}", Profiles.Count);
                    break;
            }
        }

        private static bool SerializeProfiles(GenericWriter writer)
        {
            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                    writer.WriteBlockDictionary(Profiles, (key, val) => val.Serialize(writer));
                    break;
            }

            return true;
        }

        private static bool DeserializeProfiles(GenericReader reader)
        {
            int version = reader.GetVersion();

            switch (version)
            {
                case 0:
                {
                    reader.ReadBlockDictionary(
                        () =>
                        {
                            var p = new TitleProfile(reader);
                            PlayerMobile s = p.Owner;

                            return new KeyValuePair<PlayerMobile, TitleProfile>(s, p);
                        },
                        Profiles);
                }
                    break;
            }

            return true;
        }

        #endregion
    }
}