#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Server.Items;
using Server.Mobiles;
using VitaNex;
using VitaNex.Crypto;
using VitaNex.IO;
using VitaNex.Modules.MOTD;

#endregion

namespace Server.Engines.CentralGump
{
    [CoreService("CentralGump", "1.0.0", TaskPriority.High)]
    public static partial class CentralGump
    {
        static CentralGump()
        {
            CSOptions = new CentralGumpOptions();

            PlayerProfiles = new BinaryDataStore<PlayerMobile, CentralGumpProfile>(
                VitaNexCore.SavesDirectory + "/CentralGump", "PlayerProfiles")
            {
                Async = true,
                OnSerialize = SerializePlayerProfiles,
                OnDeserialize = DeserializePlayerProfiles
            };

            Adventurers = new List<PlayerMobile>();

            Messages = new XmlDataStore<string, Message>(
                VitaNexCore.SavesDirectory + "/CentralGump", "NewsMessages")
            {
                OnSerialize = SerializeMessages,
                OnDeserialize = DeserializeMessages
            };
        }

        private static void CSConfig()
        {
            EventSink.VirtueGumpRequest += OnVirtueGumpRequest;
        }

        private static void CSEnabled()
        {
            EventSink.VirtueGumpRequest += OnVirtueGumpRequest;
        }

        private static void CSDisabled()
        {
            EventSink.VirtueGumpRequest -= OnVirtueGumpRequest;
        }

        private static void CSSave()
        {
            VitaNexCore.TryCatchGet(PlayerProfiles.Export, x => x.ToConsole());
            VitaNexCore.TryCatchGet(Messages.Export, x => x.ToConsole());
        }

        private static void CSLoad()
        {
            VitaNexCore.TryCatchGet(PlayerProfiles.Import, x => x.ToConsole());
            VitaNexCore.TryCatchGet(Messages.Import, x => x.ToConsole());
        }

        private static void CSInvoke()
        {
            CommandUtility.Register(
                "UOF",
                AccessLevel.Player,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }
                    new CentralGumpUI(e.Mobile as PlayerMobile, EnsureProfile(e.Mobile as PlayerMobile),
                        CentralGumpType.News).Send();
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

                    var p = new CentralGumpProfile(reader);

                    return new KeyValuePair<PlayerMobile, CentralGumpProfile>(e, p);
                },
                PlayerProfiles);

            return true;
        }

        private static bool SerializeMessages(XmlDocument doc)
        {
            VitaNexCore.TryCatch(
                () =>
                {
                    if (Messages == null || Messages.Count == 0)
                    {
                        return;
                    }

                    XmlElement root = doc.CreateElement("newsmessages");
                    XmlElement msgRoot = null, msgSub = null;

                    foreach (Message message in Messages.Values)
                    {
                        VitaNexCore.TryCatch(
                            () =>
                            {
                                msgRoot = doc.CreateElement("entry");
                                msgRoot.SetAttribute("timestamp", message.Date.Stamp.ToString(CultureInfo.InvariantCulture));
                                msgRoot.SetAttribute("title", message.Title);
                                msgRoot.SetAttribute("author", message.Author);
                                msgRoot.SetAttribute("published", message.Published.ToString());
                                msgRoot.SetAttribute("uid", message.UniqueID);

                                msgSub = doc.CreateElement("content");
                                msgSub.InnerText = message.Content;
                                msgRoot.AppendChild(msgSub);

                                root.AppendChild(msgRoot);
                                msgRoot = null;
                                msgSub = null;
                            },
                            CSOptions.ToConsole);
                    }

                    doc.AppendChild(root);
                },
                CSOptions.ToConsole);

            return true;
        }

        private static bool DeserializeMessages(XmlDocument doc)
        {
            VitaNexCore.TryCatch(
                () =>
                {
                    if (doc.FirstChild == null || doc.FirstChild.Name != "newsmessages")
                    {
                        return;
                    }

                    XmlElement root = doc["newsmessages"];

                    if (root == null)
                    {
                        return;
                    }

                    Message message;
                    TimeStamp date;
                    bool published;
                    string author, title, content, uid;

                    foreach (XmlElement node in root)
                    {
                        VitaNexCore.TryCatch(
                            () =>
                            {
                                date = node.HasAttribute("timestamp")
                                           ? (TimeStamp)Double.Parse(node.GetAttribute("timestamp"))
                                           : TimeStamp.UtcNow;
                                published = !node.HasAttribute("published") || Boolean.Parse(node.GetAttribute("published"));
                                author = node.HasAttribute("author") ? node.GetAttribute("author") : "Anonymous";
                                title = node.HasAttribute("title") ? node.GetAttribute("title") : "Update";
                                content = node["content"] != null ? node["content"].InnerText.Replace(@"\r\n", "[br]") : String.Empty;

                                uid = node.HasAttribute("uid")
                                          ? node.GetAttribute("uid")
                                          : CryptoGenerator.GenString(CryptoHashType.MD5, String.Format("{0}", date.Stamp)).Replace("-", "");

                                message = new Message(uid, date, title, content, author);

                                if (Messages.ContainsKey(uid))
                                {
                                    Messages[uid] = message;
                                }
                                else
                                {
                                    Messages.Add(uid, message);
                                }
                            },
                            CSOptions.ToConsole);
                    }
                },
                CSOptions.ToConsole);

            return true;
        }
    }
}