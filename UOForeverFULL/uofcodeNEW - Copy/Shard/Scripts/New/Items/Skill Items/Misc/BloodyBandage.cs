using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	public class BloodyBandage : Item
	{
		private static int[,] WaterTiles_Land = new int[3,2]
		{
			{ 168, 171, }, { 310, 311, }, { 16368, 16371 }
		};
		private static int[,] WaterTiles_Static = new int[18,2]
		{
			{ 2881, 2884 }, { 4088, 4089 }, { 4090, 4090 },
			{ 4104, 4104 }, { 5453, 5453 }, { 5456, 5462 },
			{ 5464, 5465 }, { 5937, 5978 }, { 6038, 6066 },
			{ 6595, 6636 }, { 8081, 8084 }, { 8093, 8094 },
			{ 8099, 8138 },	{ 9299, 9309 }, { 13422, 13445 },
			{ 13456, 13483 }, { 13493, 13525 }, { 13549, 13616 }
		};

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public BloodyBandage() : this( 1 )
		{
		}

		[Constructable]
		public BloodyBandage( int amt ) : base( 0xE20 )
		{
			this.Stackable = true;
			this.Weight = 0.1;
			this.Amount = amt;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Amount != 1 )
				list.Add( String.Format( "{0} bloody bandages", this.Amount ) );
			else
				list.Add( "a bloody bandage" );
		}

		public override void OnDoubleClick( Mobile m )
		{
			m.SendMessage( "What will you wash the bloody bandage{0} in?", Amount > 1 ? "s" : "" );
			m.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private BloodyBandage m_Bandage;

			public InternalTarget( BloodyBandage bandage ) : base( 3, true, TargetFlags.None )
			{
				m_Bandage = bandage;
			}

			protected override void OnTarget( Mobile m, object targ )
			{
				if ( m_Bandage.Deleted || m_Bandage.Amount < 1 )
					return;

				m.RevealingAction();

				int id;
				bool found = false;

				if( targ is LandTarget )
				{
					id = (targ as LandTarget).TileID;

					for( int i = 0; i < WaterTiles_Land.Length / 2; i++ )
					{
						if( id >= WaterTiles_Land[i,0] && id <= WaterTiles_Land[i,1] )
						{
							found = true;
							break;
						}
					}
				}
				else if ( targ is Item || targ is StaticTarget )
				{
					if( targ is Item )
						id = (targ as Item).ItemID;
					else
						id = (targ as StaticTarget).ItemID;

					for( int i = 0; i < WaterTiles_Static.Length / 2; i++ )
					{
						if( id >= WaterTiles_Static[i,0] && id <= WaterTiles_Static[i,1] )
						{
							found = true;
							break;
						}
					}
				}

				if ( found )
				{
					m.AddToBackpack( new Bandage( m_Bandage.Amount ) );
					m.SendMessage( "You wash {0} bloody bandage{1} and put the clean bandage{1} in your pack.", m_Bandage.Amount != 1 ? m_Bandage.Amount.ToString() : "the", m_Bandage.Amount != 1 ? "s" : "" );
					m_Bandage.Delete();
				}
				else
					m.SendMessage( "You can only wash bloody bandages in water." );
			}
		}

		public BloodyBandage( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}