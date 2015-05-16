using System;
using Server;
using Server.Gumps;
using Server.Items;

namespace Server.Mobiles
{
	public class ChildDistSpawner : Item, IChildDistSpawner
	{
		private MasterDistSpawner m_Master;
		private int m_HomeRange;
		private int m_WalkingRange;
		private WayPoint m_WayPoint;

		public override string DefaultName{ get{ return "Distribution Spawner (Child)"; } }

		[CommandProperty( AccessLevel.Administrator )]
		public MasterDistSpawner Master{ get{ return m_Master; } set{ LinktoMaster( value ); } }

		[CommandProperty( AccessLevel.Administrator )]
		public WayPoint WayPoint{ get{ return m_WayPoint; } set{ m_WayPoint = value; } }

		[CommandProperty( AccessLevel.Administrator )]
		public int HomeRange{ get { return m_HomeRange; } set { m_HomeRange = value; } }

		[CommandProperty( AccessLevel.Administrator )]
		public int WalkingRange{ get { return m_WalkingRange; } set { m_WalkingRange = value; } }

		[Constructable]
		public ChildDistSpawner() : base( 0x1f13 )
		{
			Movable = false;
			Visible = false;
			WalkingRange = -1;
			HomeRange = 4;
		}

		public void LinktoMaster( MasterDistSpawner master )
		{
			if ( m_Master != null )
			{
				if ( master != null ) // link to diff master
				{
					if ( master != m_Master )
					{
						m_Master.RemoveChild( this );
						m_Master = master;
						m_Master.AddChild( this );
					}
				}
				else // unlink from master
				{
					m_Master.RemoveChild( this );
					m_Master = null;
				}
			}
			else if ( master != null )
			{
				m_Master = master;
				m_Master.AddChild( this );
			}
		}

		public virtual Point3D GetSpawnPosition( ISpawnable spawned )
		{
			Map map = Map;

			if ( map == null || map == Map.Internal )
				return Location;

			bool waterMob, waterOnlyMob;

			if ( spawned is Mobile )
			{
				Mobile mob = (Mobile)spawned;

				waterMob = mob.CanSwim;
				waterOnlyMob = ( mob.CanSwim && mob.CantWalk );
			}
			else
			{
				waterMob = false;
				waterOnlyMob = false;
			}

			// Try 10 times to find a Spawnable location.
			for ( int i = 0; i < 10; i++ )
			{
				int x = Location.X + (Utility.Random( (m_HomeRange * 2) + 1 ) - m_HomeRange);
				int y = Location.Y + (Utility.Random( (m_HomeRange * 2) + 1 ) - m_HomeRange);
				int z = Map.GetAverageZ( x, y );

				int mapZ = map.GetAverageZ( x, y );

				if ( waterMob )
				{
					if ( Server.Mobiles.Spawner.IsValidWater( map, x, y, this.Z ) )
						return new Point3D( x, y, this.Z );
					else if ( Server.Mobiles.Spawner.IsValidWater( map, x, y, mapZ ) )
						return new Point3D( x, y, mapZ );
				}

				if ( !waterOnlyMob )
				{
					if ( map.CanSpawnMobile( x, y, this.Z ) )
						return new Point3D( x, y, this.Z );
					else if ( map.CanSpawnMobile( x, y, mapZ ) )
						return new Point3D( x, y, mapZ );
				}
			}

			return this.Location;
		}

		public virtual int GetWalkingRange()
		{
			return m_WalkingRange;
		}

		public virtual WayPoint GetWayPoint()
		{
			return m_WayPoint;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Master != null && from.AccessLevel >= AccessLevel.Administrator )
			{
				from.SendGump( new SpawnerGump( m_Master ) );
				from.SendGump( new PropertiesGump( from, m_Master ) );
			}
		}

		public ChildDistSpawner( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.WriteItem<MasterDistSpawner>( m_Master );

			writer.Write( m_HomeRange );
			writer.Write( m_WalkingRange );
			writer.WriteItem<WayPoint>( m_WayPoint );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			Master = reader.ReadItem<MasterDistSpawner>();

			m_HomeRange = reader.ReadInt();
			m_WalkingRange = reader.ReadInt();

			m_WayPoint = reader.ReadItem<WayPoint>();
		}
	}
}