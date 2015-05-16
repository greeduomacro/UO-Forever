using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;

namespace Server.Multis
{
	public abstract class BaseDockedGalleon : Item
	{
		private int m_MultiID;
		private Point3D m_Offset;
		private string m_ShipName;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MultiID{ get{ return m_MultiID; } set{ m_MultiID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Offset{ get{ return m_Offset; } set{ m_Offset = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public string ShipName{ get{ return m_ShipName; } set{ m_ShipName = value; InvalidateProperties(); } }

		public BaseDockedGalleon( int id, Point3D offset, BaseGalleon galleon ) : base( 0x14F4 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;

			m_MultiID = id;
			m_Offset = offset;

			m_ShipName = galleon.ShipName;
		}

		public BaseDockedGalleon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( m_MultiID );
			writer.Write( m_Offset );
			writer.Write( m_ShipName );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				case 0:
				{
					m_MultiID = reader.ReadInt();
					m_Offset = reader.ReadPoint3D();
					m_ShipName = reader.ReadString();

					if ( version == 0 )
						reader.ReadUInt();

					break;
				}
			}

			if ( LootType == LootType.Newbied )
				LootType = LootType.Blessed;

			if ( Weight == 0.0 )
				Weight = 1.0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendLocalizedMessage( 502482 ); // Where do you wish to place the ship?

				from.Target = new InternalTarget( this );
			}
		}

		public abstract BaseGalleon Galleon{ get; }

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( m_ShipName != null )
				list.Add( m_ShipName );
			else
				base.AddNameProperty( list );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_ShipName != null )
				LabelTo( from, m_ShipName );
			else
				base.OnSingleClick( from );
		}

		public void OnPlacement( Mobile from, Point3D p )
		{
			if ( Deleted )
			{
				return;
			}
			else if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				Map map = from.Map;

				if ( map == null )
					return;

				BaseGalleon galleon = Galleon;

				if ( galleon == null )
					return;

				p = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );

				if ( galleon.CanFit( p, map, galleon.ItemID ) && map != Map.Ilshenar && map != Map.Malas )
				{
					Delete();

					galleon.Owner = from;
					galleon.Anchored = true;
					galleon.ShipName = m_ShipName;

					uint keyValue = galleon.CreateKeys( from );

					for (int i = 0; i < galleon.Ropes.Count; ++i)
						galleon.Ropes[i].KeyValue = keyValue;

					galleon.MoveToWorld( p, map );
				}
				else
				{
					galleon.Delete();
					from.SendLocalizedMessage( 1043284 ); // A ship can not be created here.
				}
			}
		}

		private class InternalTarget : MultiTarget
		{
			private BaseDockedGalleon m_Model;

			public InternalTarget( BaseDockedGalleon model ) : base( model.MultiID, model.Offset )
			{
				m_Model = model;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D ip = o as IPoint3D;

				if ( ip != null )
				{
					if ( ip is Item )
						ip = ((Item)ip).GetWorldTop();

					Point3D p = new Point3D( ip );

					Region region = Region.Find( p, from.Map );

					if ( region.IsPartOf( typeof( DungeonRegion ) ) )
						from.SendLocalizedMessage( 502488 ); // You can not place a ship inside a dungeon.
					else if ( region.IsPartOf( typeof( HouseRegion ) ) || region.IsPartOf( typeof( ChampionSpawnRegion ) ) )
						from.SendLocalizedMessage( 1042549 ); // A boat may not be placed in this area.
					else
						m_Model.OnPlacement( from, p );
				}
			}
		}
	}
}