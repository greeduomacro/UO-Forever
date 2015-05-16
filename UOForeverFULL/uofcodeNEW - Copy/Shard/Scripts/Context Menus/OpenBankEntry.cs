using System;
using Server.Items;
using Server.Factions;
using Server.Mobiles;

namespace Server.ContextMenus
{
	public class OpenBankEntry : ContextMenuEntry
	{
		private Banker m_Banker;
		private bool m_Criminal;

		public OpenBankEntry( Banker banker ) : this( banker, true )
		{
		}

		public OpenBankEntry( Banker banker, bool criminal ) : base( 6105, 12 )
		{
			m_Banker = banker;
			m_Criminal = criminal;
		}

		public override void OnClick()
		{
			if ( Owner.From.CheckAlive() )
			{
				Faction fact = Faction.Find( Owner.From );

				if ( Owner.From.Criminal && m_Criminal )
					m_Banker.Say( 500378 ); // Thou art a criminal and cannot access thy bank box.
				else if ( m_Banker.FactionAllegiance != null && fact != null && m_Banker.FactionAllegiance != fact )
					m_Banker.Say( "I will not do business with the enemy!" );
				else
					Owner.From.BankBox.Open();
			}
		}
	}
}