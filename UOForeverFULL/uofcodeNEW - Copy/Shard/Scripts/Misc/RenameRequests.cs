using System;
using System.Linq;

using Server;

using VitaNex.Text;

namespace Server.Misc
{
	public class RenameRequests
	{
		public static void Initialize()
		{
			EventSink.RenameRequest += new RenameRequestEventHandler(EventSink_RenameRequest);
		}

		private static void EventSink_RenameRequest(RenameRequestEventArgs e)
		{
			Mobile m = e.From;
			Mobile targ = e.Target;
			string name = e.Name;

			if (!m.CanSee(targ) || !m.InRange(targ, 12) || !targ.CanBeRenamedBy(m))
			{
				return;
			}

			name = name.Trim();

			if (m.AccessLevel < AccessLevel.GameMaster)
			{
				if (ProfanityProtection.DisallowedAnywhere.Any(badWord => StringSearchFlags.Contains.Execute(name, badWord, true)))
				{
					m.SendMessage("That name isn't very polite.");
					return;
				}

				var result = NameVerification.ValidatePlayerName(
					name, 1, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote);

				if (result != NameResultMessage.Allowed && result != NameResultMessage.AlreadyExists)
				{
					m.SendMessage("That name isn't acceptable.");
					return;
				}
			}

			targ.Name = name;

			// Pet ~1_OLDPETNAME~ renamed to ~2_NEWPETNAME~.
			m.SendLocalizedMessage(1072623, String.Format("{0}\t{1}", targ.Name, name));
		}
	}
}