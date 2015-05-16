using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
	public class SnowglobeGump : Gump
	{
      	private Mobile m_Mobile;

		public SnowglobeGump( Mobile from ) : base( 20, 30 )
		{
        		m_Mobile = from;

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddImage(0, 0, 5010, 1150);
			this.AddImage(72, 69, 5608);
			this.AddItem(86, 19, 14199, 1150);
			this.AddItem(65, 25, 14196, 1150);
			this.AddItem(35, 26, 14199, 1150);

		}
		

	}
}