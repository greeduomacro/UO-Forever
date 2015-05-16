using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Gumps;

namespace Server.Gumps
{
	public class BaseGumpSection : IGumpType
	{
		private BaseGumpLayout m_RootParent;
		private BaseGumpSection m_Parent;
		private int m_XOffset;
		private int m_YOffset;
		private List<BaseGumpSection> m_ActiveSections;

		public BaseGumpLayout RootParent{ get{ return m_RootParent; } set{ m_RootParent = value; } }
		public BaseGumpSection Parent{ get{ return m_Parent; } set{ m_Parent = value; } }
		public int XOffset{ get{ return m_XOffset; } set{ m_XOffset = value; } }
		public int YOffset{ get{ return m_YOffset; } set{ m_YOffset = value; } }
		public List<BaseGumpSection> ActiveSections{ get{ return m_ActiveSections; } }

		public virtual string Name{ get{ return null; } }
		public Mobile From{ get{ return RootParent.From; } }

		//This is where we started before initialization
		private int m_ButtonPos;
		private int m_TextEntryPos;
		private int m_SwitchPos;

		//This is how many were initialized
		private int m_ButtonCount;
		private int m_TextEntryCount;
		private int m_SwitchCount;

		public int ButtonPos{ get{ return m_ButtonPos; } set{ m_ButtonPos = value; } }
		public int TextEntryPos{ get{ return m_TextEntryPos; } set{ m_TextEntryPos = value; } }
		public int SwitchPos{ get{ return m_SwitchPos; } set{ m_SwitchPos = value; } }

		public int ButtonCount{ get{ return m_ButtonCount; } set{ m_ButtonCount = value; } }
		public int TextEntryCount{ get{ return m_TextEntryCount; } set{ m_TextEntryCount = value; } }
		public int SwitchCount{ get{ return m_SwitchCount; } set{ m_SwitchCount = value; } }

		public BaseGumpSection( BaseGumpSection parent, int mod_x, int mod_y ) : this( parent != null ? parent.RootParent : null, mod_x, mod_y )
		{
			m_Parent = parent;
		}

		public BaseGumpSection( BaseGumpLayout root, int mod_x, int mod_y )
		{
			m_RootParent = root;
			InitializeSection( mod_x, mod_y );
		}

		public void ReinitializeSection()
		{
			InitializeSection( XOffset, YOffset );
		}

		//We created our object, do stuff to get it ready to construct
		public virtual void InitializeSection( int mod_x, int mod_y )
		{
			m_XOffset = mod_x;
			m_YOffset = mod_y;
			m_ActiveSections = new List<BaseGumpSection>( 2 );
		}

		public virtual void Construct( int buttonpos, int textentrypos, int switchpos )
		{
			m_ButtonPos = buttonpos;
			m_TextEntryPos = textentrypos;
			m_SwitchPos = switchpos;
			Background();
			Components();
		}

		public virtual void Background()
		{
		}

		public virtual void Components()
		{
			//By default we activate the sections, then construct them.
			ConstructActiveSections();
		}

		public RelayInfo LocalRelayInfo( RelayInfo info )
		{
			return new RelayInfo( GetButtonID( info.ButtonID ), LocalSwitches( new List<int>( info.Switches ) ).ToArray(), LocalTextEntries( new List<TextRelay>( info.TextEntries ) ).ToArray() );
		}

		//This isn't used.... but it could be
		public int GetButtonID( int buttonid )
		{
			return buttonid - m_ButtonPos;
		}

		//Localizes text entries to this section
		public virtual List<TextRelay> LocalTextEntries( List<TextRelay> entries )
		{
			//Add all the remaining valid entries to a local list
			//This includes subsection entries
			List<TextRelay> list = new List<TextRelay>( 2 );
			int max = m_TextEntryPos + m_TextEntryCount;

			for ( int i = 0; i < entries.Count; i++ )
				if ( entries[i].EntryID >= m_TextEntryPos && entries[i].EntryID <= max )
					list.Add( entries[i] );

			//Remove all switches from all active sections
			for ( int i = 0; i < m_ActiveSections.Count; i++ )
				m_ActiveSections[i].TrimTextEntries( list );

			return list;
		}

		public void TrimTextEntries( List<TextRelay> entries )
		{
			int max = m_TextEntryPos + m_TextEntryCount;

			for ( int i = 0; i < entries.Count; i++ )
				if ( entries[i].EntryID >= m_TextEntryPos && entries[i].EntryID <= max )
					entries.RemoveAt( i-- );
		}

		//Localizes switches to this section
		public virtual List<int> LocalSwitches( List<int> switches )
		{
			//Add all the remaining valid switches to a local list
			//This includes subsection switches
			List<int> list = new List<int>( 2 );
			int max = m_SwitchPos + m_SwitchCount;

			for ( int i = 0; i < switches.Count; i++ )
				if ( switches[i] >= m_SwitchPos && switches[i] <= max )
					list.Add( switches[i] );

			//Remove all switches from all active sections
			for ( int i = 0; i < m_ActiveSections.Count; i++ )
				m_ActiveSections[i].TrimSwitches( list );

			return list;
		}

		public void TrimSwitches( List<int> switches )
		{
			int max = m_SwitchPos + m_SwitchCount;

			for ( int i = 0; i < switches.Count; i++ )
				if ( switches[i] >= m_SwitchPos && switches[i] <= max )
					switches.RemoveAt( i-- );
		}

		//Capture any button which defines its response with this section
		public virtual void OnResponse( NetState sender, RelayInfo info, RelayInfo relinfo, int localid )
		{
		}

		public virtual void ConstructActiveSections()
		{
			for ( int i = 0; i < m_ActiveSections.Count; i++ )
				ConstructAddedSection( m_ActiveSections[i] );
		}

		//Construct and make active the section
		public void ConstructSection( BaseGumpSection section )
		{
			if ( section != null )
				ConstructAddedSection( AddActiveSection( section ) );
		}

		//Construct already active section
		public virtual void ConstructAddedSection( BaseGumpSection section )
		{
			section.ActiveSections.Clear();
			section.Construct( m_ButtonCount, m_TextEntryCount, m_SwitchCount );
		}

		public virtual BaseGumpSection AddActiveSection( BaseGumpSection section )
		{
			m_ActiveSections.Add( section );
			return section;
		}

		public GumpResponse NewResponse( GumpResponseHandler handler )
		{
			return NewResponse( handler, 0 );
		}

		public GumpResponse NewResponse( BaseGumpSection section, GumpResponseHandler handler )
		{
			return new GumpResponse( section, handler, 0 );
		}

		public GumpResponse NewResponse( GumpResponseHandler handler, int localid )
		{
			return new GumpResponse( this, handler, localid );
		}

		public virtual GumpResponse NewResponse( BaseGumpSection section, GumpResponseHandler handler, int localid )
		{
			return new GumpResponse( section, handler, localid );
		}

		//This shouldn't even be used, but whatever the hell... try it out.
		public virtual void AddPage( int page )
		{
			m_RootParent.AddPage( page );
		}

		public virtual void AddAlphaRegion( int x, int y, int width, int height )
		{
			m_RootParent.AddAlphaRegion( x + m_XOffset, y + m_YOffset, width, height );
		}

		public virtual void AddBackground( int x, int y, int width, int height, int gumpID )
		{
			m_RootParent.AddBackground( x + m_XOffset, y + m_YOffset, width, height, gumpID );
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
			m_RootParent.AddButton( x + m_XOffset, y + m_YOffset, normalID, pressedID, type, param, response );
			m_ButtonCount++;
		}

		public virtual void AddCheck( int x, int y, int inactiveID, int activeID, bool initialState )
		{
			m_RootParent.AddCheck( x + m_XOffset, y + m_YOffset, inactiveID, activeID, initialState );
			m_SwitchCount++;
		}

		public virtual void AddGroup( int group )
		{
			m_RootParent.AddGroup( group );
		}

		public virtual void AddTooltip( int number )
		{
			m_RootParent.AddTooltip( number );
		}

		public virtual void AddHtml( int x, int y, int width, int height, string text, bool background, bool scrollbar )
		{
			m_RootParent.AddHtml( x + m_XOffset, y + m_YOffset, width, height, text, background, scrollbar );
		}

		public virtual void AddHtmlLocalized( int x, int y, int width, int height, int number, bool background, bool scrollbar )
		{
			m_RootParent.AddHtmlLocalized( x + m_XOffset, y + m_YOffset, width, height, number, background, scrollbar );
		}

		public virtual void AddHtmlLocalized( int x, int y, int width, int height, int number, int color, bool background, bool scrollbar )
		{
			m_RootParent.AddHtmlLocalized( x + m_XOffset, y + m_YOffset, width, height, number, color, background, scrollbar );
		}

		public virtual void AddHtmlLocalized( int x, int y, int width, int height, int number, string args, int color, bool background, bool scrollbar )
		{
			m_RootParent.AddHtmlLocalized( x + m_XOffset, y + m_YOffset, width, height, number, args, color, background, scrollbar );
		}

		public virtual void AddImage( int x, int y, int gumpID )
		{
			m_RootParent.AddImage( x + m_XOffset, y + m_YOffset, gumpID );
		}

		public virtual void AddImage( int x, int y, int gumpID, int hue )
		{
			m_RootParent.AddImage( x + m_XOffset, y + m_YOffset, gumpID, hue );
		}

		public virtual void AddImageTiled( int x, int y, int width, int height, int gumpID )
		{
			m_RootParent.AddImageTiled( x + m_XOffset, y + m_YOffset, width, height, gumpID );
		}

		public virtual void AddImageTiledButton( int x, int y, int normalID, int pressedID, GumpButtonType type, int param, int itemID, int hue, int width, int height, GumpResponse response )
		{
			m_RootParent.AddImageTiledButton( x + m_XOffset, y + m_YOffset, normalID, pressedID, type, param, itemID, hue, width, height, response );
			m_ButtonCount++;
		}
		public virtual void AddImageTiledButton( int x, int y, int normalID, int pressedID, GumpButtonType type, int param, int itemID, int hue, int width, int height, int localizedTooltip, GumpResponse response )
		{
			m_RootParent.AddImageTiledButton( x + m_XOffset, y + m_YOffset, normalID, pressedID, type, param, itemID, hue, width, height, localizedTooltip, response );
			m_ButtonCount++;
		}

		public virtual void AddItem( int x, int y, int itemID )
		{
			m_RootParent.AddItem( x + m_XOffset, y + m_YOffset, itemID );
		}

		public virtual void AddItem( int x, int y, int itemID, int hue )
		{
			m_RootParent.AddItem( x + m_XOffset, y + m_YOffset, itemID, hue );
		}

		public virtual void AddLabel( int x, int y, int hue, string text )
		{
			m_RootParent.AddLabel( x + m_XOffset, y + m_YOffset, hue, text );
		}

		public virtual void AddLabelCropped( int x, int y, int width, int height, int hue, string text )
		{
			m_RootParent.AddLabelCropped( x + m_XOffset, y + m_YOffset, width, height, hue, text );
		}

		public virtual void AddRadio( int x, int y, int inactiveID, int activeID, bool initialState )
		{
			m_RootParent.AddRadio( x + m_XOffset, y + m_YOffset, inactiveID, activeID, initialState );
			m_SwitchCount++;
		}

		public virtual void AddTextEntry( int x, int y, int width, int height, int hue, string initialText )
		{
			m_RootParent.AddTextEntry( x + m_XOffset, y + m_YOffset, width, height, hue, initialText );
			m_TextEntryCount++;
		}

        public static string GetTextRelayString( RelayInfo info, int id )
        {
			return BaseGumpLayout.GetTextRelayString( info, id );
        }
	}
}