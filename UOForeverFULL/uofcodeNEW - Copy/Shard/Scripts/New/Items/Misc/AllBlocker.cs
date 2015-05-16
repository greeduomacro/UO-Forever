using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class AllBlocker : Item
	{
        public static void Initialize()
        {
            TileData.ItemTable[0x21A3].Flags = TileFlag.Wall | TileFlag.NoShoot | TileFlag.Impassable;
            TileData.ItemTable[0x21A3].Height = 20;
        }

		public override string DefaultName{ get{ return "Impassable! (No LOS)"; } }

		private Packet m_WorldGMPacket;
		private Packet m_WorldGMPacketSA;
		private Packet m_WorldGMPacketHS;

		public Packet WorldGMPacket
		{
			get
			{
				if ( m_WorldGMPacket == null )
				{
					m_WorldGMPacket = new WorldItem( this, 0x241A );
					m_WorldGMPacket.SetStatic();
				}

				return m_WorldGMPacket;
			}
		}

		public Packet WorldGMPacketSA
		{
			get
			{
				if ( m_WorldGMPacketSA == null )
				{
					m_WorldGMPacketSA = new WorldItemSA( this, 0x241A );
					m_WorldGMPacketSA.SetStatic();
				}

				return m_WorldGMPacketSA;
			}
		}

		public Packet WorldGMPacketHS
		{
			get
			{
				if ( m_WorldGMPacketHS == null )
				{
					m_WorldGMPacketHS = new WorldItemHS( this, 0x241A );
					m_WorldGMPacketHS.SetStatic();
				}

				return m_WorldGMPacketHS;
			}
		}

        [Constructable]
		public AllBlocker() : base( 0x21A3 )
		{
			Movable = false;
        }

		public AllBlocker( Serial serial ) : base( serial )
		{
		}

		public override void ReleaseWorldPackets()
		{
			base.ReleaseWorldPackets();


			Packet.Release( ref m_WorldGMPacket );
			Packet.Release( ref m_WorldGMPacketSA );
			Packet.Release( ref m_WorldGMPacketHS );
		}

		protected override Packet GetWorldPacketFor( NetState state )
		{
			Mobile mob = state.Mobile;

			if ( mob != null && mob.AccessLevel >= AccessLevel.GameMaster )
			{
				if ( state.HighSeas )
					return WorldGMPacketHS;
				else if ( state.StygianAbyss )
					return WorldGMPacketSA;
				else
					return WorldGMPacket;
			}

			return base.GetWorldPacketFor( state );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
    }
}