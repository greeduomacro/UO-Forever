using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class VialOfPoison : Item
	{
		private static readonly int m_FallbackItemID = 0x0F82;

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

		public override string DefaultName{ get{ return "a vial of lethal poison"; } }

		[Constructable]
		public VialOfPoison() : base( 0x5722 )
		{
		}

		public VialOfPoison( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelToExpansion(from);

			string name = Name;

			if ( String.IsNullOrEmpty( name ) )
				LabelTo( from, 1272, LabelNumber );
			else
				LabelTo( from, name, 1272 );
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