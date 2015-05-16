using System;
using Server;
using Server.Network;

namespace Server.Gumps
{
	public abstract class BaseGumpListSection : BaseGumpSection
	{
		public abstract int LinesPerPage{ get; }
		public abstract int PrevNormalID{ get; }
		public abstract int PrevPressedID{ get; }
		public abstract int PrevDisabledID{ get; }

		public abstract int NextNormalID{ get; }
		public abstract int NextPressedID{ get; }
		public abstract int NextDisabledID{ get; }

		private int m_ListPage;
		public int ListPage{ get{ return m_ListPage; } set{ m_ListPage = value; } }

		public BaseGumpListSection( BaseGumpSection parent, int mod_x, int mod_y ) : base( parent, mod_x, mod_y )
		{
		}

		public BaseGumpListSection( BaseGumpLayout root, int mod_x, int mod_y ) : base( root, mod_x, mod_y )
		{
		}

		public virtual void AddPageButtons( int prevx, int prevy, int nextx, int nexty, int count )
		{
			if ( count > LinesPerPage )
			{
				//Previous Button
				AddListPrevButton( prevx, prevy, m_ListPage > 0, NewResponse( new GumpResponseHandler( ListPage_Response ), -1 ) );
				//Next Button
				AddListNextButton( nextx, nexty, m_ListPage < 1, NewResponse( new GumpResponseHandler( ListPage_Response ), 1 ) );
			}
		}

		public virtual void AddListPrevButton( int x, int y, bool valid, GumpResponse response )
		{
			if ( valid )
				AddButton( x, y, PrevNormalID, PrevPressedID, response );
			else
				AddImage( x, y, PrevDisabledID );
		}

		public virtual void AddListNextButton( int x, int y, bool valid, GumpResponse response )
		{
			if ( valid )
				AddButton( x, y, NextNormalID, NextPressedID, response );
			else
				AddImage( x, y, NextDisabledID );
		}

		public virtual void ListPage_Response( NetState netstate, RelayInfo info, RelayInfo relinfo, int localid )
		{
			//-1 = previous, 1 = next
			m_ListPage += localid;
		}

		public void ConstructList( int count )
		{
			ConstructList( 0, 0, count );
		}

		//call this when you want to place your list
		public virtual void ConstructList( int x, int y, int count )
		{
			for ( int i = 0, index = (m_ListPage * LinesPerPage); i < LinesPerPage && index >= 0 && index < count; ++i, ++index )
				ListLine( x, y, index, i );
		}

		//index = list index
		//local = number of that page
		public virtual void ListLine( int x, int y, int index, int local )
		{
		}
	}
}