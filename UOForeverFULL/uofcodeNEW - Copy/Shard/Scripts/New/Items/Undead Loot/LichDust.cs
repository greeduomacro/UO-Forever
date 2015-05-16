using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class LichDust : BaseReagent
	{
		public override int DescriptionNumber { get { return 0; } }
		public override string DefaultName{ get{ return "lich dust"; } }

		private static readonly int m_FallbackItemID = 0x0F8F;

		private Packet m_FallbackWorldPacket;

		public Packet FallbackWorldPacket
		{
			get
			{
				if ( m_FallbackWorldPacket == null )
				{
					m_FallbackWorldPacket = new WorldItem( this, m_FallbackItemID );
					m_FallbackWorldPacket.SetStatic();
				}

				return m_FallbackWorldPacket;
			}
		}

		[Constructable]
		public LichDust() : this( 1 )
		{
		}

		[Constructable]
		public LichDust( int amount ) : base( 0x5745, amount )
		{
			Hue = 1072;
		}

		public LichDust( Serial serial ) : base( serial )
		{
		}

		protected override Packet GetWorldPacketFor( NetState state )
		{
			if ( state.HighSeas )
				return this.WorldPacketHS;
			else if ( state.StygianAbyss )
				return this.WorldPacketSA;
			else
				return this.FallbackWorldPacket;
		}

		public override void ReleaseWorldPackets()
		{
			base.ReleaseWorldPackets();

			Packet.Release( ref m_FallbackWorldPacket );
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