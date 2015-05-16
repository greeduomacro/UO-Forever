using System;
using Server;
using Server.Prompts;

namespace Server.Multis
{
	public class NewRenameBoatPrompt : Prompt
	{
		private BaseShip m_Boat;

		public NewRenameBoatPrompt( BaseShip boat )
		{
			m_Boat = boat;
		}

		public override void OnResponse( Mobile from, string text )
		{
			m_Boat.EndRename( from, text );
		}
	}
}