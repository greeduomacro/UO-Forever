// **********
// RunUO Shard - ClientVerification.cs
// **********

#region References

using System;
using System.Diagnostics;
using System.IO;
using Server.Games;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Misc
{
    public class ClientVerification
    {
        private enum OldClientResponse
        {
            Ignore,
            Warn,
            Annoy,
            LenientKick,
            Kick
        }

        private static bool m_DetectClientRequirement = false;
        private static OldClientResponse m_OldClientResponse = OldClientResponse.Kick;

        private static bool m_AllowRegular = true;

        private static TimeSpan m_AgeLeniency = TimeSpan.FromDays(10);
        private static TimeSpan m_GameTimeLeniency = TimeSpan.FromHours(25);

        private static TimeSpan m_KickDelay = TimeSpan.FromSeconds(60.0);

        static ClientVerification()
        {
            AllowGod = false;
        }

        public static ClientVersion Required { get; set; }
        public static ClientVersion Recommended { get; set; }

        public static bool AllowRegular
        {
            get { return m_AllowRegular; }
            set { m_AllowRegular = value; }
        }

        public static bool AllowUOTD
        {
            get { return PseudoSeerStone.AllowThirdDawnClient; }
        }

        public static bool AllowGod { get; set; }

        public static TimeSpan KickDelay
        {
            get { return m_KickDelay; }
            set { m_KickDelay = value; }
        }

        public static void Initialize()
        {
            EventSink.ClientVersionReceived += EventSink_ClientVersionReceived;

            Required = new ClientVersion("5.0.0");
            Recommended = new ClientVersion("7.0.34.6");

            if (m_DetectClientRequirement)
            {
                string path = Core.FindDataFile("client.exe");

                if (File.Exists(path))
                {
                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);

                    if (info.FileMajorPart != 0 || info.FileMinorPart != 0 || info.FileBuildPart != 0 ||
                        info.FilePrivatePart != 0)
                    {
                        Required = new ClientVersion(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart,
                            info.FilePrivatePart);
                    }
                }
            }

            if (Required != null)
            {
                Utility.PushColor(ConsoleColor.White);
                Console.WriteLine("Client Restriction: Required client {0}. Action taken: {1}", Required,
                    m_OldClientResponse);
                Console.WriteLine("Client Restriction: Recommended client {0}.", Recommended);
                Utility.PopColor();
            }
        }

        private static void EventSink_ClientVersionReceived(ClientVersionReceivedArgs e)
        {
            string kickMessage = null;
            NetState state = e.State;
            ClientVersion version = e.Version;

            if (state.Mobile == null || state.Mobile.AccessLevel > AccessLevel.Player)
                return;

            if (Required != null && version < Required &&
                (m_OldClientResponse == OldClientResponse.Kick ||
                 (m_OldClientResponse == OldClientResponse.LenientKick &&
                  (DateTime.UtcNow - state.Mobile.CreationTime) > m_AgeLeniency && state.Mobile is PlayerMobile &&
                  ((PlayerMobile) state.Mobile).GameTime > m_GameTimeLeniency)))
            {
                kickMessage = String.Format("This server requires your client version be at least {0}.", Required);
            }
            else if (!AllowGod || !AllowRegular || !AllowUOTD)
            {
                if (!AllowGod && version.Type == ClientType.God)
                    kickMessage = "This server does not allow god clients to connect.";
                else if (!AllowRegular && version.Type == ClientType.Regular)
                    kickMessage = "This server does not allow regular clients to connect.";
                else if (!AllowUOTD && state.IsUOTDClient)
                    kickMessage = "This server does not allow UO:TD clients to connect.";

                if (!AllowGod && !AllowRegular && !AllowUOTD)
                {
                    kickMessage = "This server does not allow any clients to connect.";
                }
                else if (AllowGod && !AllowRegular && !AllowUOTD && version.Type != ClientType.God)
                {
                    kickMessage = "This server requires you to use the god client.";
                }
                else if (kickMessage != null)
                {
                    if (AllowRegular && AllowUOTD)
                        kickMessage += " You can use regular or UO:TD clients.";
                    else if (AllowRegular)
                        kickMessage += " You can use regular clients.";
                    else if (AllowUOTD)
                        kickMessage += " You can use UO:TD clients.";
                }
            }

            if (kickMessage != null)
            {
                state.Mobile.SendAsciiMessage(0x22, 0, kickMessage);
                state.Mobile.SendAsciiMessage(0x22, 0,
                    String.Format("You will be disconnected in {0} seconds.", KickDelay.TotalSeconds));

                Timer.DelayCall(KickDelay, delegate
                {
                    if (state.Socket != null)
                    {
                        Console.WriteLine("Client: {0}: Disconnecting, bad version", state);
                        state.Dispose();
                    }
                });
            }
            else if (Required != null && version < Required)
            {
                switch (m_OldClientResponse)
                {
                    case OldClientResponse.Warn:
                    {
                        state.Mobile.SendAsciiMessage(0x22, 0, "Your client is out of date. Please update your client.");
                        state.Mobile.SendAsciiMessage(0x22, 0,
                            String.Format("This server recommends that your client version be at least {0}.",
                                Recommended));
                        break;
                    }
                    case OldClientResponse.LenientKick:
                    case OldClientResponse.Annoy:
                    {
                        SendAnnoyGump(state.Mobile);
                        break;
                    }
                }
            }
        }

        private static void SendAnnoyGump(Mobile m)
        {
            if (m.NetState != null && m.NetState.Version < Required)
            {
                Gump g = new WarningGump(1060637, 30720,
                    String.Format(
                        "Your client is out of date. Please update your client.<br>This server recommends that your client version be at least {0}.<br> <br>You are currently using version {1}.<br> <br>To patch, run UOPatch.exe inside your Ultima Online folder.",
                        Recommended, m.NetState.Version), 0xFFC000, 480, 360,
                    delegate
                    {
                        m.SendMessage("You will be reminded of this again.");

                        if (m_OldClientResponse == OldClientResponse.LenientKick)
                            m.SendMessage(
                                "Old clients will be kicked after {0} days of character age and {1} hours of play time",
                                m_AgeLeniency, m_GameTimeLeniency);

                        Timer.DelayCall(TimeSpan.FromMinutes(Utility.Random(5, 15)), () => SendAnnoyGump(m));
                    }, null, false);

                g.Dragable = false;
                g.Closable = false;
                g.Resizable = false;

                m.SendGump(g);
            }
        }
    }
}