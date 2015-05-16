using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public interface IChildDistSpawner
	{
		Point3D GetSpawnPosition( ISpawnable spawn );
		WayPoint GetWayPoint();
		int GetWalkingRange();
		Point3D Location{ get; set; }
		Map Map{ get; set; }
		bool Deleted{ get; }
		object RootParent{ get; }
	}

	public class MasterDistSpawner : Spawner, IChildDistSpawner
	{
		private List<IChildDistSpawner> m_Children;
		public List<IChildDistSpawner> Children{ get{ return m_Children; } }

		[CommandProperty( AccessLevel.Administrator )]
		public bool IsChild{ get{ return m_Children.Contains( this ); } set{ SetIsChild( value ); } }

		public override string DefaultName{ get{ return "Distribution Spawner (Master)"; } }

		public MasterDistSpawner( int amount, int minDelay, int maxDelay, int team, int homeRange, string spawnedNames ) : base( amount, minDelay, maxDelay, team, homeRange, spawnedNames )
		{
			m_Children = new List<IChildDistSpawner>();
			m_Children.Add( this );
		}

		public MasterDistSpawner( string spawnedName ) : base( spawnedName )
		{
			m_Children = new List<IChildDistSpawner>();
			m_Children.Add( this );
		}

		[Constructable]
		public MasterDistSpawner() : base()
		{
			m_Children = new List<IChildDistSpawner>();
			m_Children.Add( this );
		}

		public void SetIsChild( bool value )
		{
			int index = m_Children.IndexOf( this );

			if ( value )
			{
				if ( index < 0 )
					m_Children.Add( this );
			}
			else if ( index > -1 )
				m_Children.RemoveAt( index );
		}

		public override Point3D GetSpawnPosition( ISpawnable spawned, Map map )
		{
			//Eliminate children on different facets than the chosen map.

			List<IChildDistSpawner> children = new List<IChildDistSpawner>( m_Children );

			for ( int i = children.Count-1; i >= 0; i-- )
				if ( children[i].Map != map || children[i].RootParent != null ) //Not in a container or not on the same map!
					children.RemoveAt( i );

			if ( children.Count > 0 )
				return children[Utility.Random( children.Count )].GetSpawnPosition( spawned );
			else
				return GetSpawnPosition( spawned );
		}

		public virtual Point3D GetSpawnPosition( ISpawnable spawn )
		{
			return base.GetSpawnPosition( spawn, Map );
		}

		public override Map GetSpawnMap()
		{
			DefragChildren();

			if ( m_Children.Count > 0 )
				return m_Children[Utility.Random( m_Children.Count )].Map;

			return this.Map;
		}

		public void DefragChildren()
		{
			for ( int i = m_Children.Count-1; i >= 0; i-- )
				if ( m_Children[i] == null || m_Children[i].Deleted )
					m_Children.RemoveAt( i );
		}

		public void AddChild( ChildDistSpawner child )
		{
			if ( !m_Children.Contains( child ) )
				m_Children.Add( child );
		}

		public void RemoveChild( ChildDistSpawner child )
		{
			if ( m_Children.Contains( child ) )
				m_Children.Remove( child );
		}

		public MasterDistSpawner( Serial serial ) : base( serial )
		{
			m_Children = new List<IChildDistSpawner>();
			//m_Children.Add( this );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 1 ); // version

			writer.Write( IsChild );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 1: if ( reader.ReadBool() ) IsChild = true; break;
			}
			
		}
	}
}