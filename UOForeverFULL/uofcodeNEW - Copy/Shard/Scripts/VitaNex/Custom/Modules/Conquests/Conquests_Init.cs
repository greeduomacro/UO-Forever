#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Server.Accounting;
using Server.Mobiles;
using VitaNex;
using VitaNex.IO;
using VitaNex.Reflection;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.Conquests
{
    [CoreModule("Conquests", "1.0.1")]
    public static partial class Conquests
    {
        static Conquests()
        {
            ConquestTypes = typeof(Conquest).GetConstructableChildren();
            ConquestCompleteGumpTypes = typeof(ConquestCompletedGump).GetConstructableChildren();

            ConquestRegistry = new BinaryDataStore<ConquestSerial, Conquest>(
                VitaNexCore.SavesDirectory + "Conquests", "Conquests")
            {
                OnSerialize = SerializeConquestRegistry,
                OnDeserialize = DeserializeConquestRegistry
            };

            Profiles = new BinaryDataStore<PlayerMobile, ConquestProfile>(VitaNexCore.SavesDirectory + "Conquests",
                "Profiles")
            {
                Async = true,
                OnSerialize = SerializeProfiles,
                OnDeserialize = DeserializeProfiles
            };
        }

        /*private static void CMConfig()
        {
            SetEvents();
        }

        private static void CMEnabled()
        {
            SetEvents();
        }

        private static void CMDisabled()
        {
            UnsetEvents();
        }*/

        private static void CMInvoke()
        {
            CommandUtility.Register(
                "Conquests",
                AccessLevel.Player,
                e =>
                {
                    if (!CMOptions.ModuleEnabled && e.Mobile.AccessLevel < Access)
                    {
                        e.Mobile.SendMessage(0x22, "Conquests are currently unavailable.");
                        return;
                    }

                    if (e.Mobile is PlayerMobile)
                    {
                        SendConquestsGump((PlayerMobile) e.Mobile);
                    }
                });

            CommandUtility.Register(
                "ConsolidateConquests",
                AccessLevel.Developer,
                e =>
                {
                    if (!CMOptions.ModuleEnabled && e.Mobile.AccessLevel < Access)
                    {
                        e.Mobile.SendMessage(0x22, "Conquests are currently unavailable.");
                        return;
                    }

                    if (e.Mobile is PlayerMobile)
                    {
                        ConsolidateConquests();
                    }
                });

            CommandUtility.Register(
                "CheckAccountsConquests",
                AccessLevel.Developer,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }
                    foreach (PlayerMobile player in Profiles.Keys)
                    {
                        IAccount account = player.Account;
                        foreach (PlayerMobile checkplayer in Profiles.Keys)
                        {
                            if (player != checkplayer)
                            {
                                IAccount checkaccount = checkplayer.Account;
                                if (account == checkaccount)
                                {
                                    e.Mobile.SendMessage(54, "THIS SHOULDN'T HAPPEN");
                                }
                            }
                        }
                    }
                });

            CommandUtility.Register(
                "PruneConquests",
                AccessLevel.Developer,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }
                    PruneConquestProfiles();
                });

            //Removed until we can get VNC lists to handle large amounts of entries
            /*
			CommandUtility.Register(
				"ConquestProfiles",
				Access,
				e =>
				{
					if (!CMOptions.ModuleEnabled && e.Mobile.AccessLevel < Access)
					{
						e.Mobile.SendMessage(0x22, "Conquests are currently unavailable.");
						return;
					}

					if (e.Mobile is PlayerMobile)
					{
						SendConquestProfilesGump((PlayerMobile)e.Mobile);
					}
				});
			*/

            CommandUtility.Register(
                "ConquestAdmin",
                Access,
                e =>
                {
                    if (e.Mobile is PlayerMobile)
                    {
                        SendConquestAdminGump((PlayerMobile) e.Mobile);
                    }
                });

            CommandUtility.Register(
                "IncreaseConquestState",
                AccessLevel.Developer,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    if (e.Arguments.Length < 2)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <PlayerName> <ConquestName> <Amount>");
                        return;
                    }

                    string name = e.Arguments[0];
                    string conquest = e.Arguments[1];
                    int value;

                    if (!Int32.TryParse(e.Arguments[2], out value))
                    {
                        e.Mobile.SendMessage(0x22, "Format: <PlayerName> <ConquestName> <Amount>");
                        return;
                    }

                    if (e.Arguments.Length > 3)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <PlayerName> <ConquestName> <Amount>");
                        return;
                    }

                    Mobile mobile =
                        World.Mobiles.Values.FirstOrDefault(x => x.RawName == name && x is PlayerMobile);
                    if (mobile is PlayerMobile)
                    {
                        ConquestProfile profile = EnsureProfile(mobile as PlayerMobile);
                        ConquestState state;
                        if (profile.TryGetState(ConquestRegistry.Values.FirstOrDefault(x => x.Name == conquest),
                            out state))
                        {
                            state.Progress += value;
                        }
                    }
                });
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
            VitaNexCore.TryCatch(Defragment, CMOptions.ToConsole);
            VitaNexCore.TryCatch(ConquestRewardInfo.Defragment, CMOptions.ToConsole);

            VitaNexCore.TryCatch(SaveConquestRegistry, CMOptions.ToConsole);
            VitaNexCore.TryCatch(SaveProfiles, CMOptions.ToConsole);
        }

        public static void Load()
        {
            VitaNexCore.TryCatch(LoadConquestRegistry, CMOptions.ToConsole);
            VitaNexCore.TryCatch(LoadProfiles, CMOptions.ToConsole);

            VitaNexCore.TryCatch(Defragment, CMOptions.ToConsole);
            VitaNexCore.TryCatch(ConquestRewardInfo.Defragment, CMOptions.ToConsole);
        }

        #region Write/Read Methods for Conquest

        public static void WriteConquest(GenericWriter writer, Conquest c)
        {
            if (c != null && !c.Deleted)
            {
                writer.Write(true);

                c.UID.Serialize(writer);
            }
            else
            {
                writer.Write(false);
            }
        }

        public static Conquest ReadConquest(GenericReader reader)
        {
            if (!reader.ReadBool())
            {
                return null;
            }

            var uid = new ConquestSerial(reader);

            Conquest c;

            return ConquestRegistry.TryGetValue(uid, out c) ? c : null;
        }

        #endregion

        private static void Defragment()
        {
            ConquestRegistry.RemoveValueRange(c => c == null || c.Deleted);

            foreach (ConquestProfile p in Profiles.Values)
            {
                p.Where(s => s != null && !s.ConquestExists).ForEach(s => p.Registry.Remove(s));
            }

            Profiles.RemoveValueRange(p => p == null || p.Count == 0 || p.Owner == null || p.Owner.Deleted);
        }

        #region Conquests Save/Load

        public static void SaveConquestRegistry()
        {
            DataStoreResult result = ConquestRegistry.Export();
            CMOptions.ToConsole("Result: {0}", result.ToString());

            switch (result)
            {
                case DataStoreResult.Null:
                case DataStoreResult.Busy:
                case DataStoreResult.Error:
                {
                    if (ConquestRegistry.HasErrors)
                    {
                        CMOptions.ToConsole("Conquest database has errors...");

                        ConquestRegistry.Errors.ForEach(CMOptions.ToConsole);
                    }
                }
                    break;
                case DataStoreResult.OK:
                    CMOptions.ToConsole("Conquest count: {0:#,0}", ConquestRegistry.Count);
                    break;
            }
        }

        public static void LoadConquestRegistry()
        {
            DataStoreResult result = ConquestRegistry.Import();
            CMOptions.ToConsole("Result: {0}", result.ToString());

            switch (result)
            {
                case DataStoreResult.Null:
                case DataStoreResult.Busy:
                case DataStoreResult.Error:
                {
                    if (ConquestRegistry.HasErrors)
                    {
                        CMOptions.ToConsole("Conquest database has errors...");

                        ConquestRegistry.Errors.ForEach(CMOptions.ToConsole);
                    }
                }
                    break;
                case DataStoreResult.OK:
                    CMOptions.ToConsole("Conquest count: {0:#,0}", ConquestRegistry.Count);
                    break;
            }
        }

        private static bool SerializeConquestRegistry(GenericWriter writer)
        {
            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                {
                    writer.WriteBlockDictionary(
                        ConquestRegistry,
                        (key, val) => writer.WriteType(
                            val,
                            t =>
                            {
                                if (t != null)
                                {
                                    val.Serialize(writer);
                                }
                            }));
                }
                    break;
            }

            return true;
        }

        private static bool DeserializeConquestRegistry(GenericReader reader)
        {
            int version = reader.GetVersion();

            switch (version)
            {
                case 0:
                {
                    reader.ReadBlockDictionary(
                        () =>
                        {
                            var c = reader.ReadTypeCreate<Conquest>(reader);

                            ConquestSerial s = c != null ? c.UID : null;

                            return new KeyValuePair<ConquestSerial, Conquest>(s, c);
                        },
                        ConquestRegistry);
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
                {
                    CMOptions.ToConsole("Profile count: {0:#,0}", Profiles.Count);
                    CMOptions.ToConsole("State count: {0:#,0}", Profiles.Values.Sum(p => p.Count));
                }
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
                {
                    CMOptions.ToConsole("Profile count: {0:#,0}", Profiles.Count);
                    CMOptions.ToConsole("State count: {0:#,0}", Profiles.Values.Sum(p => p.Count));
                }
                    break;
            }
        }

        private static bool SerializeProfiles(GenericWriter writer)
        {
            int version = writer.SetVersion(1);

            switch (version)
            {
                case 1:
                {
                    writer.WriteBlockArray(
                        Profiles.Values.ToArray(),
                        obj =>
                        {
                            if (obj != null)
                            {
                                obj.Serialize(writer);
                            }
                        });
                }
                    break;
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
                case 1:
                {
                    ConquestProfile[] list = reader.ReadBlockArray(() => new ConquestProfile(reader));

                    foreach (ConquestProfile p in list)
                    {
                        Profiles.AddOrReplace(p.Owner, p);
                    }
                }
                    break;
                case 0:
                {
                    reader.ReadBlockDictionary(
                        () =>
                        {
                            var p = new ConquestProfile(reader);
                            PlayerMobile s = p.Owner;

                            return new KeyValuePair<PlayerMobile, ConquestProfile>(s, p);
                        },
                        Profiles);
                }
                    break;
            }

            return true;
        }

        #endregion

        #region XML Export/Import

        public static void ExportXML(PlayerMobile user)
        {
            string path = VitaNexCore.DataDirectory + "Conquests";

            new InputDialogGump(user)
            {
                Title = "XML Export",
                Html = "Type a file name for the Conquests XML export.\nThe file will be saved to:\n" + path,
                Callback = (b, s) => ExportXML(path, String.IsNullOrWhiteSpace(s) ? "Registry" : s)
            }.Send();
        }

        public static void ExportXML(string path, string file)
        {
            using (var xml = new XmlDataStore<ConquestSerial, Conquest>(path, file))
            {
                xml.OnSerialize = doc =>
                {
                    XmlElement root = doc.CreateElement("Conquests");

                    foreach (Conquest conquest in xml.Values)
                    {
                        PropertyList<Conquest> pList = conquest.GetProperties(
                            BindingFlags.Instance | BindingFlags.Public,
                            p => p.CanWrite && p.Name != "Enabled" && p.Name != "InvokeReset" && p.Name != "InvokeClear");

                        XmlElement conquestNode = doc.CreateElement("Conquest");

                        conquestNode.SetAttribute("type", conquest.GetType().Name);

                        foreach (KeyValuePair<string, object> kv in pList.Where(kv => SimpleType.IsSimpleType(kv.Value))
                            )
                        {
                            string flag;
                            string value;

                            if (SimpleType.IsSimpleType(kv.Value))
                            {
                                SimpleType sType = SimpleType.FromObject(kv.Value);

                                flag = sType.Flag.ToString();
                                value = sType.Value.ToString();
                            }
                            else if (kv.Value is Type)
                            {
                                flag = "Type";
                                value = ((Type) kv.Value).Name;
                            }
                            else if (kv.Value is ITypeSelectProperty)
                            {
                                flag = "Type";
                                value = ((ITypeSelectProperty) kv.Value).TypeName ?? String.Empty;
                            }
                            else
                            {
                                continue;
                            }

                            XmlElement conquestPropNode = doc.CreateElement(kv.Key);

                            conquestPropNode.SetAttribute("type", flag);
                            conquestPropNode.SetAttribute("value", value);

                            conquestNode.AppendChild(conquestPropNode);
                        }

                        root.AppendChild(conquestNode);
                    }

                    doc.AppendChild(root);

                    return true;
                };

                ConquestRegistry.CopyTo(xml, true);

                xml.Export();

                foreach (Exception e in xml.Errors)
                {
                    e.ToConsole();
                }
            }
        }

        public static void ImportXML(PlayerMobile user)
        {
            string path = VitaNexCore.DataDirectory + "Conquests";

            new InputDialogGump(user)
            {
                Title = "XML Import",
                Html =
                    "Type a file name for the Conquests XML import.\nThe file will be loaded from:\n" + path +
                    "\n\nThe file will be deleted after a successful import.",
                Callback = (b, s) => ImportXML(path, String.IsNullOrWhiteSpace(s) ? "Registry" : s)
            }.Send();
        }

        public static void ImportXML(string path, string file)
        {
            using (var xml = new XmlDataStore<ConquestSerial, Conquest>(path, file))
            {
                xml.OnDeserialize = doc =>
                {
                    XmlElement root = doc.DocumentElement;

                    if (root == null)
                    {
                        return true;
                    }

                    foreach (XmlElement conquestNode in root.ChildNodes.OfType<XmlElement>())
                    {
                        string typeAttr = conquestNode.GetAttribute("type");
                        var type = new TypeSelectProperty<Conquest>(typeAttr);

                        if (!type.IsNotNull)
                        {
                            continue;
                        }

                        var conquest = type.CreateInstance<Conquest>();
                        PropertyList<Conquest> pList = conquest.GetProperties(
                            BindingFlags.Instance | BindingFlags.Public,
                            p => p.CanWrite && p.Name != "Enabled" && p.Name != "InvokeReset" && p.Name != "InvokeClear");

                        foreach (XmlElement conquestPropNode in conquestNode.ChildNodes.OfType<XmlElement>())
                        {
                            string pName = conquestPropNode.Name;
                            string dType = conquestPropNode.GetAttribute("type");
                            string data = conquestPropNode.GetAttribute("value");

                            switch (dType)
                            {
                                case "Type":
                                {
                                    Type t = null;

                                    if (!String.IsNullOrWhiteSpace(data))
                                    {
                                        t = Type.GetType(data, false, true) ??
                                            ScriptCompiler.FindTypeByName(data, true) ??
                                            ScriptCompiler.FindTypeByFullName(data);
                                    }

                                    pList.Set(pName, t);
                                }
                                    break;
                                default:
                                {
                                    DataType dataType;

                                    if (!Enum.TryParse(dType, out dataType))
                                    {
                                        continue;
                                    }

                                    SimpleType sType;

                                    if (SimpleType.TryParse(data, dataType, out sType) && sType.Flag != DataType.Null)
                                    {
                                        pList.Set(pName, sType.Value);
                                    }
                                }
                                    break;
                            }
                        }

                        pList.Serialize(conquest);

                        xml.AddOrReplace(conquest.UID, conquest);
                    }

                    return true;
                };

                if (xml.Import() == DataStoreResult.OK)
                {
                    xml.CopyTo(ConquestRegistry);

                    xml.Document.Delete();
                }

                foreach (Exception e in xml.Errors)
                {
                    e.ToConsole();
                }
            }
        }

        #endregion
    }
}