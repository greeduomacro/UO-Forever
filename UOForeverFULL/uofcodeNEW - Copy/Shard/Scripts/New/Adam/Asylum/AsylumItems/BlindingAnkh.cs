using System;
using Server.Factions;
using Server.Network;

namespace Server.Items	
{
	public class BlindingAnkh : BaseNecklace
	{
        public override string DefaultName { get { return "Ankh of Blinding"; } }

 		private static readonly int m_FallbackItemID = 0x1088;

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
		public BlindingAnkh() : base( 0x3BB5 )
		{
		    Hue = 1358;
		}

        public BlindingAnkh(Serial serial)
            : base(serial)
		{
		}

        public override bool OnEquip(Mobile from)
        {
            from.SendMessage(61, "You are now protected from the destructive gaze of the Asylum Guardian.");
            Effects.SendIndividualFlashEffect(from, (FlashType)2);
            return base.OnEquip(from);
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