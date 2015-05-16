using System;
using Server.Items;
using Server.Factions;
using Server.Mobiles;

namespace Server.ContextMenus
{
	public class CheckBalanceEntry : ContextMenuEntry
	{
		private Banker m_Banker;
		private bool m_Criminal;

		public CheckBalanceEntry( Banker banker ) : this( banker, true )
		{
		}

		public CheckBalanceEntry( Banker banker, bool criminal ) : base( 6124, 12 )
		{
			m_Banker = banker;
			m_Criminal = criminal;
		}

		public override void OnClick()
		{
			if (Owner.From.CheckAlive())
			{
				Faction fact = Faction.Find(Owner.From);

				if (Owner.From.Criminal && m_Criminal && m_Banker.Kills == 0)
					m_Banker.Say(500378); // Thou art a criminal and cannot access thy bank box.
				else if (m_Banker.FactionAllegiance != null && fact != null && m_Banker.FactionAllegiance != fact)
					m_Banker.Say("I will not do business with the enemy!");
				else
				{
					ulong balance = Banker.GetFullBalance(Owner.From, m_Banker.TypeOfCurrency);

					Owner.From.SendMessage("Thy current bank balance is {0:#,0} {1}.", balance, m_Banker.TypeOfCurrency.Name);
				}
			}
		}
	}
}