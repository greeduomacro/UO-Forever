#region References

using Server.Network;

#endregion

namespace Server.Misc
{
    public enum ProfanityAction
    {
        None, // no action taken
        Disallow, // speech is not displayed
        Criminal, // makes the player criminal, not killable by guards
        CriminalAction, // makes the player criminal, can be killed by guards
        Disconnect, // player is kicked
        Other // some other implementation
    }

    public class ProfanityProtection
    {
        private static bool Enabled = false;

        private static ProfanityAction Action = ProfanityAction.None;
            // change here what to do when profanity is detected

        public static void Initialize()
        {
            if (Enabled)
            {
                EventSink.Speech += EventSink_Speech;
            }
        }

        private static bool OnProfanityDetected(Mobile from)
        {
            switch (Action)
            {
                case ProfanityAction.None:
                    return true;
                case ProfanityAction.Disallow:
                    return false;
                case ProfanityAction.Criminal:
                    from.Criminal = true;
                    return true;
                case ProfanityAction.CriminalAction:
                    from.CriminalAction(false);
                    return true;
                case ProfanityAction.Disconnect:
                {
                    NetState ns = from.NetState;

                    if (ns != null)
                    {
                        ns.Dispose();
                    }

                    return false;
                }
                default:
                case ProfanityAction.Other: // TODO: Provide custom implementation if this is chosen
                {
                    return true;
                }
            }
        }

        private static void EventSink_Speech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from.AccessLevel > AccessLevel.Player)
            {
                return;
            }

            if (
                NameVerification.Validate(e.Speech, 0, int.MaxValue, true, true, false, int.MaxValue, m_Exceptions,
                    m_DisallowedWords, m_StartDisallowed, m_DisallowedAnywhere) != NameResultMessage.Allowed)
            {
                e.Blocked = !OnProfanityDetected(from);
            }
        }

        public static char[] Exceptions { get { return m_Exceptions; } }
        public static string[] StartDisallowed { get { return m_StartDisallowed; } }
        public static string[] DisallowedWords { get { return m_DisallowedWords; } }
        public static string[] DisallowedAnywhere { get { return m_DisallowedAnywhere; } }

        private static char[] m_Exceptions =
        {
            ' ', '-', '.', '\'', '"', ',', '_', '+', '=', '~', '`', '!', '^', '*', '\\', '/', ';', ':', '<', '>', '[',
            ']', '{', '}', '?', '|', '(', ')', '%', '$', '&', '#', '@'
        };

        private static string[] m_StartDisallowed = {};

        private static string[] m_DisallowedWords =
        {
            "wop"
        };

        private static string[] m_DisallowedAnywhere =
        {
            "wop"
        };
    }
}