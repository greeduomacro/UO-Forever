#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Commands;
using Server.Mobiles;

#endregion

namespace Server.Misc
{
    public enum NameResultMessage
    {
        Allowed,
        InvalidCharacter,
        TooFewCharacters,
        TooManyCharacters,
        AlreadyExists,
        NotAllowed
    }

    public class NameVerification
    {
        public static readonly char[] SpaceDashPeriodQuote =
        {
            ' ', '-', '.', '\''
        };

        public static readonly char[] Empty = new char[0];

        public static void Initialize()
        {
            //EventSink.ValidatePlayerName += new ValidatePlayerNameEventHandler( EventSink_ValidatePlayerName );
            CommandSystem.Register("ValidateName", AccessLevel.Administrator,
                ValidateName_OnCommand);
            CommandSystem.Register("ValidatePlayerName", AccessLevel.Administrator,
                ValidatePlayerName_OnCommand);
        }

        [Usage("ValidateName")]
        [Description("Checks the result of NameValidation on the specified name.")]
        public static void ValidateName_OnCommand(CommandEventArgs e)
        {
            if (Validate(e.ArgString, 2, 16, true, false, true, 1, SpaceDashPeriodQuote) != NameResultMessage.Allowed)
            {
                e.Mobile.SendMessage(0x59, "That non-player name is considered invalid.");
            }
            else
            {
                e.Mobile.SendMessage(0x22, "That non-player name is considered valid.");
            }
        }

        [Usage("ValidatePlayerName")]
        [Description("Checks the result of NameValidation on the specified player name.")]
        public static void ValidatePlayerName_OnCommand(CommandEventArgs e)
        {
            if (ValidatePlayerName(e.ArgString, 2, 16, true, false, true, 1, SpaceDashPeriodQuote) !=
                NameResultMessage.Allowed)
            {
                e.Mobile.SendMessage(0x59, "That player name is considered invalid.");
            }
            else
            {
                e.Mobile.SendMessage(0x22, "That player name is considered valid.");
            }
        }

/*
		public static bool EventSink_ValidatePlayerName( ValidatePlayerNameEventArgs e )
		{
			NetState state = e.State;
			string name = e.Name;
			string lowername = name.ToLower();

			NameResultMessage result = NameVerification.ValidatePlayerName( lowername, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote );

			switch ( result )
			{
				default:
				case NameResultMessage.NotAllowed: SendErrorOnCharacterCreation( state, String.Format( "The name {0} is not allowed.", name ) ); return false;
				case NameResultMessage.InvalidCharacter: SendErrorOnCharacterCreation( state, String.Format( "The name {0} contains invalid characters.", name ) ); return false;
				case NameResultMessage.TooFewCharacters: case NameResultMessage.TooManyCharacters: SendErrorOnCharacterCreation( state, "The name must be between 2-16 characters." ); return false;
				case NameResultMessage.AlreadyExists: SendErrorOnCharacterCreation( state, String.Format( "A player with the name {0} already exists.", name ) ); return false;
				case NameResultMessage.Allowed: return true;
			}
		}

		public static void SendErrorOnCharacterCreation( NetState state, string message )
		{
			Console.WriteLine( "Login: {0}: Character creation failed. {1}", state, message );

			if ( Core.AOS )
				state.Send( SupportedFeatures.Instantiate( state ) );

			state.Send( new CharacterList( state.Account, state.CityInfo ) );

			state.Send( new AsciiMessage( Serial.MinusOne, -1, MessageType.Regular, 38, 0, "System", message ) );
		}
*/

        public static NameResultMessage Validate(string name, int minLength, int maxLength, bool allowLetters,
            bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions)
        {
            return Validate(name, minLength, maxLength, allowLetters, allowDigits, noExceptionsAtStart, maxExceptions,
                exceptions, m_DisallowedWords, m_StartDisallowed, m_DisallowedAnywhere);
        }

        public static NameResultMessage Validate(string name, int minLength, int maxLength, bool allowLetters,
            bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions, string[] disallowedWords,
            string[] startDisallowed, string[] disallowedAnywhere)
        {
            if (name == null || name.Length < minLength)
            {
                return NameResultMessage.TooFewCharacters;
            }

            if (name.Length > maxLength)
            {
                return NameResultMessage.TooManyCharacters;
            }

            string kw;

            if (AntiAdverts.Detect(name, out kw))
            {
                return NameResultMessage.NotAllowed;
            }

            int exceptCount = 0;

            name = name.ToLower();

            if (!allowLetters || !allowDigits ||
                (exceptions.Length > 0 && (noExceptionsAtStart || maxExceptions < int.MaxValue)))
            {
                for (int i = 0; i < name.Length; ++i)
                {
                    char c = name[i];

                    if (c >= 'a' && c <= 'z')
                    {
                        if (!allowLetters)
                        {
                            return NameResultMessage.InvalidCharacter;
                        }

                        exceptCount = 0;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        if (!allowDigits)
                        {
                            return NameResultMessage.InvalidCharacter;
                        }

                        exceptCount = 0;
                    }
                    else
                    {
                        bool except = false;

                        for (int j = 0; !except && j < exceptions.Length; ++j)
                        {
                            if (c == exceptions[j])
                            {
                                except = true;
                            }
                        }

                        if (!except || (i == 0 && noExceptionsAtStart))
                        {
                            return NameResultMessage.InvalidCharacter;
                        }

                        if (exceptCount++ == maxExceptions)
                        {
                            return NameResultMessage.InvalidCharacter;
                        }
                    }
                }
            }

            foreach (string t in disallowedWords)
            {
                int indexOf = name.IndexOf(t, StringComparison.Ordinal);

                if (indexOf == -1)
                {
                    continue;
                }

                bool badPrefix = (indexOf == 0);

                for (int j = 0; !badPrefix && j < exceptions.Length; ++j)
                {
                    badPrefix = (name[indexOf - 1] == exceptions[j]);
                }

                if (!badPrefix)
                {
                    continue;
                }

                bool badSuffix = ((indexOf + t.Length) >= name.Length);

                for (int j = 0; !badSuffix && j < exceptions.Length; ++j)
                {
                    badSuffix = (name[indexOf + t.Length] == exceptions[j]);
                }

                if (badSuffix)
                {
                    return NameResultMessage.NotAllowed;
                }
            }

            if (startDisallowed.Any(t => name.StartsWith(t)))
            {
                return NameResultMessage.NotAllowed;
            }

            if (disallowedAnywhere.Any(t => name.IndexOf(t, StringComparison.Ordinal) > -1))
            {
                return NameResultMessage.NotAllowed;
            }

            return NameResultMessage.Allowed;
        }

        public static NameResultMessage ValidatePlayerName(string name, int minLength, int maxLength, bool allowLetters,
            bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions)
        {
            return ValidatePlayerName(name, minLength, maxLength, allowLetters, allowDigits, noExceptionsAtStart,
                maxExceptions, exceptions, m_DisallowedWords, m_StartDisallowed, m_DisallowedAnywhere);
        }

        public static NameResultMessage ValidatePlayerName(string name, int minLength, int maxLength, bool allowLetters,
            bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions, string[] disallowedWords,
            string[] startDisallowed, string[] disallowedAnywhere)
        {
            NameResultMessage result = Validate(name, minLength, maxLength, allowLetters, allowDigits,
                noExceptionsAtStart, maxExceptions, exceptions, disallowedWords, startDisallowed, disallowedAnywhere);

            string lowername = name.Trim().ToLower();

            if (result == NameResultMessage.Allowed)
            {
                var mobs = new List<Mobile>(World.Mobiles.Values);
                foreach (Mobile m in mobs)
                {
                    if (
                        m is PlayerMobile
                            /*&& AccessLevelToggler.GetRawAccessLevel( (PlayerMobile)m ) == AccessLevel.Player*/&&
                        !String.IsNullOrEmpty(m.RawName) && m.RawName.Trim().ToLower() == lowername)
                    {
                        result = NameResultMessage.AlreadyExists;
                    }
                }
            }

            return result;
        }

        public static string[] StartDisallowed { get { return m_StartDisallowed; } }
        public static string[] DisallowedWords { get { return m_DisallowedWords; } }
        public static string[] DisallowedAnywhere { get { return m_DisallowedAnywhere; } }

        private static string[] m_StartDisallowed =
        {
            "seer ",
            "counselor ",
            "gm ",
            "admin ",
            "lady ",
            "cnt ",
            "lord ",
            "staff ",
            "lead ",
            "trial ",
            "dev ",
            "owner ",
            "founder ",
            "gamemaster ",
            "generic player"
        };

        private static string[] m_DisallowedWords =
        {
            "wop",
            "tit",
            "spic",
            "cum",
            "ass",
            "clit",
            "klit",
            "dick",
            "anal",
            "suck",
            "sucks",
            "osi",
            "tit",
            "nigger",
            "negro"
        };

        private static string[] m_DisallowedAnywhere =
        {
            "staff",
            "minkio",
            "tjalfe",
            "jigaboo",
            "jiggaboo",
            "chigaboo",
            "xlordx",
            "xlx",
            "wop",
            "kyke",
            "kike",
            "spic",
            "prick",
            "piss",
            "lezbo",
            "lesbo",
            "felatio",
            "dyke",
            "dildo",
            "chinc",
            "chink",
            "cunnilingus",
            "cock",
            "clitoris",
            "hitler",
            "penis",
            "nigga",
            "nigger",
            "kunt",
            "jiz",
            "jism",
            "jerk",
            "jackoff",
            "jack off",
            "jack-off",
            "jack.off",
            "jack\'off",
            "goddamn",
            "god damn",
            "god-damn",
            "god.damn",
            "god\'damn",
            "fag",
            "blowjob",
            "blow job",
            "blow-job",
            "blow.job",
            "blow\'job",
            "handjob",
            "hand job",
            "hand-job",
            "hand.job",
            "hand\'job",
            "rimjob",
            "rim job",
            "rim-job",
            "rim.job",
            "rim\'job",
            "bitch",
            "asshole",
            "ass hole",
            "ass-hole",
            "ass.hole",
            "ass\'hole",
            "pussy",
            "snatch",
            "cunt",
            "twat",
            "shit",
            "fuck",
            "blackthorne",
            "blackthorn",
            "gamemaster",
            "squelched",
            "kurwa",
            "vittu",
            "uogamers",
            "porn",
            "pron",
            "dfi",
            "casiopia",
            "zenvera",
            "rapist",
            "vagina",
            "perilous",
            "negro",
            "Negro",
            "beaner",
            "cracker", 
            "niglet"
        };
    }
}