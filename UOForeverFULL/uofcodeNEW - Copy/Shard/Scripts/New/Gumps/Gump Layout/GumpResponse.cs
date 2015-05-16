using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Gumps
{
	public delegate void GumpResponseHandler( NetState netstate, RelayInfo info, RelayInfo relinfo, int localid );

	public class GumpResponse
	{
		private BaseGumpSection m_Section;
		private GumpResponseHandler m_Handler;
		private int m_LocalButtonID;

		public BaseGumpSection Section{ get{ return m_Section; } set{ m_Section = value; } }
		public GumpResponseHandler Handler{ get{ return m_Handler; } set{ m_Handler = value; } }
		public int LocalButtonID{ get{ return m_LocalButtonID; } set{ m_LocalButtonID = value; } }

		public GumpResponse( BaseGumpSection section, GumpResponseHandler handler ) : this ( section, handler, 0 )
		{
		}

		public GumpResponse( BaseGumpSection section, GumpResponseHandler handler, int localid )
		{
			m_Section = section;
			m_Handler = handler;
			m_LocalButtonID = localid;
		}
	}
}