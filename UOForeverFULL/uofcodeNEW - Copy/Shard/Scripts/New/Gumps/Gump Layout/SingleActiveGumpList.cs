using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;

namespace Server.Gumps
{
	public delegate void SetActiveCategory( BaseGumpSection section );

	public abstract class BaseCategoryListSection : BaseGumpListSection
	{
		public abstract int LineWidthSpace{ get; }
		public abstract int LineHeightSpace{ get; }

		private List<BaseGumpSection> m_Categories;
		private int m_DefaultActiveCategory;
		//We use this instead of ActiveSections, because it should be persistent
		private BaseGumpSection m_ActiveCategory;

		public List<BaseGumpSection> Categories{ get{ return m_Categories; } set{ m_Categories = value; } }
		public int DefaultActiveCategory{ get{ return m_DefaultActiveCategory; } set{ m_DefaultActiveCategory = value; } }
		public BaseGumpSection ActiveCategory{ get{ return m_ActiveCategory; } set{ m_ActiveCategory = value; } }

		public BaseCategoryListSection( BaseGumpSection parent, int mod_x, int mod_y ) : base( parent, mod_x, mod_y )
		{
		}

		public BaseCategoryListSection( BaseGumpLayout root, int mod_x, int mod_y ) : base( root, mod_x, mod_y )
		{
		}

		public BaseCategoryListSection( BaseGumpSection parent, int mod_x, int mod_y, int activecat ) : this( parent, mod_x, mod_y )
		{
			m_DefaultActiveCategory = activecat;
		}

		public BaseCategoryListSection( BaseGumpLayout root, int mod_x, int mod_y, int activecat ) : this( root, mod_x, mod_y )
		{
			m_DefaultActiveCategory = activecat;
		}

		public override void Construct( int buttonpos, int textentrypos, int switchpos )
		{
			//If we do not have an active category, lets get a default
			if ( m_ActiveCategory == null )
				SetActiveCategory( GetDefaultSection() );

			base.Construct( buttonpos, textentrypos, switchpos );
		}

		public override void Components()
		{
			//if we have an active category, lets add it to our active sections for construction
			if ( m_ActiveCategory != null )
				AddActiveSection( m_ActiveCategory );

			base.Components();
		}

		public override void InitializeSection( int mod_x, int mod_y )
		{
			base.InitializeSection( mod_x, mod_y );
			m_Categories = new List<BaseGumpSection>( 2 );
		}

		public virtual BaseGumpSection GetDefaultSection()
		{
			if ( m_DefaultActiveCategory >= 0 && m_Categories.Count > 0 )
			{
				BaseGumpSection section = m_Categories[m_DefaultActiveCategory];
				if ( section != null )
					return section;
			}

			return null;
		}

		public virtual void SetActiveCategory( BaseGumpSection section )
		{
			m_ActiveCategory = section;
		}

		public void SetActiveCategory( int index )
		{
			if ( m_Categories.Count == 0 )
				return;
			if ( index >= m_Categories.Count || index < 0 )
				index = 0;

			SetActiveCategory( m_Categories[index] );
		}

		public void ClearActiveCategory()
		{
			SetActiveCategory( null );
		}

		public virtual void AddCategory( BaseGumpSection section )
		{
			m_Categories.Add( section );
		}

		public virtual void AddPageButtons( int prevx, int prevy, int nextx, int nexty )
		{
			AddPageButtons( prevx, prevy, nextx, nexty, m_Categories.Count );
		}

		public virtual void ConstructList( int x, int y )
		{
			ConstructList( x, y, m_Categories.Count );
		}

		public override void ListLine( int x, int y, int index, int local )
		{
			if ( m_Categories[index] != null )
				DisplayCategory( x + (local * LineWidthSpace), y + (local * LineHeightSpace), m_Categories[index], NewResponse( new GumpResponseHandler( Category_Response ), index ) );
		}

		public abstract void DisplayCategory( int x, int y, BaseGumpSection section, GumpResponse response );

		public virtual void Category_Response( NetState netstate, RelayInfo info, RelayInfo relinfo, int localid )
		{
			SetActiveCategory( localid );
		}
	}
}