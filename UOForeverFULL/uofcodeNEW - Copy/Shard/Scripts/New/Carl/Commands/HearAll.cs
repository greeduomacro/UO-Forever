#region References

using System;
using System.Collections;
using Server.Commands;

#endregion

namespace Server.Misc
{
    public class HearAll
    {
        private static ArrayList m_HearAll = new ArrayList();

        public static void Initialize()
        {
            CommandSystem.Register("HearAll", AccessLevel.GameMaster, HearAll_OnCommand);

            EventSink.Speech += OnSpeech;
        }

        private static void OnSpeech(SpeechEventArgs args)
        {
            string msg;

            if (args.Mobile == null)
            {
                return;
            }

            if (args.Mobile.Region != null && !string.IsNullOrEmpty(args.Mobile.Region.Name))
            {
                msg = String.Format("{0} ({1}): {2}", args.Mobile.RawName, args.Mobile.Region.Name, args.Speech);
            }
            else
            {
                msg = String.Format("{0}: {1}", args.Mobile.RawName, args.Speech);
            }

            ArrayList rem = null;

            foreach (object t in m_HearAll)
            {
                if (t is Mobile)
                {
                    var m = (Mobile) t;

                    if (m.NetState == null)
                    {
                        if (rem == null)
                        {
                            rem = new ArrayList(1);
                        }

                        rem.Add(m);
                    }
                    else
                    {
                        if (m.InRange(args.Mobile.Location, 14))
                        {
                            continue;
                        }

                        m.SendMessage(msg);
                    }
                }
            }

            if (rem != null)
            {
                foreach (object t in rem)
                {
                    m_HearAll.Remove(t);
                }
            }
        }

        [Usage("HearAll")]
        [Description("Toggles listening to global player chat.")]
        private static void HearAll_OnCommand(CommandEventArgs args)
        {
            if (m_HearAll.Contains(args.Mobile))
            {
                m_HearAll.Remove(args.Mobile);
                args.Mobile.SendMessage("\'Hear all\' disabled.");
            }
            else
            {
                m_HearAll.Add(args.Mobile);
                args.Mobile.SendMessage("\'Hear all\' enabled. Type [HearAll again to disable it.");
            }
        }

        public static void RemoveMobile(Mobile m)
        {
            if (m_HearAll != null && m_HearAll.Contains(m))
            {
                m_HearAll.Remove(m);
            }
        }

        public static void ClearList()
        {
            if (m_HearAll != null)
            {
                m_HearAll.Clear();
            }
        }
    }
}