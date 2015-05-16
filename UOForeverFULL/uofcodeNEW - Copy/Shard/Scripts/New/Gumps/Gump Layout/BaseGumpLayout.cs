using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;

namespace Server.Gumps
{

	public interface IGumpType
	{
		void AddPage( int page );
		void AddAlphaRegion( int x, int y, int width, int height );
		void AddBackground( int x, int y, int width, int height, int gumpID );
		void AddButton( int x, int y, int normalID, int pressedID, GumpButtonType type, int param, GumpResponse response );
		void AddCheck( int x, int y, int inactiveID, int activeID, bool initialState );
		void AddGroup( int group );
		void AddTooltip( int number );
		void AddHtml( int x, int y, int width, int height, string text, bool background, bool scrollbar );
		void AddHtmlLocalized( int x, int y, int width, int height, int number, bool background, bool scrollbar );
		void AddHtmlLocalized( int x, int y, int width, int height, int number, int color, bool background, bool scrollbar );
		void AddHtmlLocalized( int x, int y, int width, int height, int number, string args, int color, bool background, bool scrollbar );
		void AddImage( int x, int y, int gumpID );
		void AddImage( int x, int y, int gumpID, int hue );
		void AddImageTiled( int x, int y, int width, int height, int gumpID );
		void AddImageTiledButton( int x, int y, int normalID, int pressedID, GumpButtonType type, int param, int itemID, int hue, int width, int height, GumpResponse response );
		void AddImageTiledButton( int x, int y, int normalID, int pressedID, GumpButtonType type, int param, int itemID, int hue, int width, int height, int localizedTooltip, GumpResponse response );
		void AddItem( int x, int y, int itemID );
		void AddItem( int x, int y, int itemID, int hue );
		void AddLabel( int x, int y, int hue, string text );
		void AddLabelCropped( int x, int y, int width, int height, int hue, string text );
		void AddRadio( int x, int y, int inactiveID, int activeID, bool initialState );
		void AddTextEntry( int x, int y, int width, int height, int hue, string initialText );
	}

	public delegate void OnGumpSend( Mobile from, BaseGumpLayout layout );

	public class BaseGumpLayout : Gump, IGumpType
	{
		private List<GumpResponse> m_Responses;
		private Mobile m_From;
		public OnGumpSend GumpSendHandler;

		private int m_ButtonCount;
		private int m_TextEntryCount;
		private int m_SwitchCount;

		public List<GumpResponse> Responses{ get{ return m_Responses; } }
		public Mobile From{ get{ return m_From; } set{ m_From = value; } }

		public int ButtonCount{ get{ return m_ButtonCount; } set{ m_ButtonCount = value; } }
		public int TextEntryCount{ get{ return m_TextEntryCount; } set{ m_TextEntryCount = value; } }
		public int SwitchCount{ get{ return m_SwitchCount; } set{ m_SwitchCount = value; } }

		public virtual string Name{ get{ return null; } }

		public BaseGumpLayout( Mobile from ) : this( from, 0, 0 )
		{
		}

		public BaseGumpLayout( Mobile from, int x, int y ) : base( x, y )
		{
			m_From = from;
			InitializeGump();
		}

		//We have created our layout, now we must initialize it
		//when overriding, call the base, add your active sections, then initialize the active sections
		public virtual void InitializeGump()
		{
			m_Responses = new List<GumpResponse>( 2 );
		}

		//Components consisting of our gump background
		public virtual void Background()
		{
			AddPage( 0 );
		}

		//This is where you construct sections
		public virtual void Components()
		{
		}

		public virtual void SendGump( bool close )
		{
			SendGump( m_From, close );
		}

		public virtual void SendGump( Mobile from, bool close )
		{
			if ( close )
				from.CloseGump( GetType() );

			ClearGump();
			Background();
			Components();

			if ( GumpSendHandler != null )
				GumpSendHandler( from, this );

			from.SendGump( this );
		}

		public virtual void ClearGump()
		{
			Entries.Clear();
			m_Responses.Clear();
			m_ButtonCount = m_TextEntryCount = m_SwitchCount = 0;
		}

		public virtual void OnGumpCancelled( NetState sender, RelayInfo info )
		{
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			int buttonid = info.ButtonID-1;

			if ( buttonid < 0 || buttonid > m_Responses.Count )
				OnGumpCancelled( sender, info );
			else
			{
				GumpResponse response = m_Responses[buttonid];
				RelayInfo localinfo = response.Section.LocalRelayInfo( info );
				if ( response.Handler != null )
					response.Handler( sender, info, localinfo, response.LocalButtonID );
				response.Section.OnResponse( sender, info, localinfo, response.LocalButtonID );
			}
		}

		public virtual void ConstructSection( BaseGumpSection section )
		{
			section.ActiveSections.Clear();
			section.Construct( m_ButtonCount, m_TextEntryCount, m_SwitchCount );
		}

        public static string GetTextRelayString( RelayInfo info, int id )
        {
            TextRelay t = info.GetTextEntry( id );
            return ( t == null ? String.Empty : t.Text.Trim() );
        }

		public virtual void AddResponse( GumpResponse response )
		{
			m_Responses.Add( response );
		}

		public virtual void AddRadio( int x, int y, int inactiveID, int activeID, bool initialState )
		{
			base.AddRadio( x, y, inactiveID, activeID, initialState, m_SwitchCount++ );
		}

		public virtual void AddTextEntry( int x, int y, int width, int height, int hue, string initialText )
		{
			base.AddTextEntry( x, y, width, height, hue, m_TextEntryCount++, initialText );
		}

		public virtual void AddButton( int x, int y, int normalID, int pressedID, GumpResponse response )
		{
			AddButton( x, y, normalID, pressedID, GumpButtonType.Reply, 0, response );
		}

		public virtual void AddButton( int x, int y, int normalID, int pressedID, int param, GumpResponse response )
		{
			AddButton( x, y, normalID, pressedID, GumpButtonType.Page, param, response );
		}

		public virtual void AddButton( int x, int y, int normalID, int pressedID, GumpButtonType type, int param, GumpResponse response )
		{
			AddButton( x, y, normalID, pressedID, ++m_ButtonCount, type, param );
			AddResponse( response );
		}

		public virtual void AddCheck( int x, int y, int inactiveID, int activeID, bool initialState )
		{
			base.AddCheck( x, y, inactiveID, activeID, initialState, m_SwitchCount++ );
		}

		public virtual void AddImageTiledButton( int x, int y, int normalID, int pressedID, GumpButtonType type, int param, int itemID, int hue, int width, int height, GumpResponse response )
		{
			AddImageTiledButton( x, y, normalID, pressedID, ++m_ButtonCount, type, param, itemID, hue, width, height );
			AddResponse( response );
		}
		public virtual void AddImageTiledButton( int x, int y, int normalID, int pressedID, GumpButtonType type, int param, int itemID, int hue, int width, int height, int localizedTooltip, GumpResponse response )
		{
			AddImageTiledButton( x, y, normalID, pressedID, ++m_ButtonCount, type, param, itemID, hue, width, height, localizedTooltip );
			AddResponse( response );
		}
	}
}