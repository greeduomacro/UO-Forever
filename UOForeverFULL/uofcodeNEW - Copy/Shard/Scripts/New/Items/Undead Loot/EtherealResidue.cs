using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class EtherealResidue : BaseReagent
	{
		public override int DescriptionNumber { get { return 0; } }
		public override string DefaultName{ get{ return "ethereal residue"; } }

		private Packet m_FallbackWorldPacket;

		public Packet FallbackWorldPacket
		{
			get
			{
				if ( m_FallbackWorldPacket == null )
				{
					m_FallbackWorldPacket = new OldEtherealResidueWorldItem( this );
					m_FallbackWorldPacket.SetStatic();
				}

				return m_FallbackWorldPacket;
			}
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

		[Constructable]
		public EtherealResidue() : this( 1 )
		{
		}

		[Constructable]
		public EtherealResidue( int amount ) : base( 0x5745, amount )
		{
		}

		public EtherealResidue( Serial serial ) : base( serial )
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

	public sealed class OldEtherealResidueWorldItem : Packet
	{
		public OldEtherealResidueWorldItem( Item item ) : base( 0x1A )
		{
			this.EnsureCapacity( 20 );

			uint serial = (uint)item.Serial.Value;
			int itemID = 0x0F8F & 0x3FFF; //grave dust
			int amount = item.Amount;
			Point3D loc = item.Location;
			int x = loc.X;
			int y = loc.Y;
			int hue = item.Hue == 0 ? 1151 : item.Hue; //pretty close in hue
			int flags = item.GetPacketFlags();
			int direction = (int)item.Direction;

			if ( amount != 0 )
				serial |= 0x80000000;
			else
				serial &= 0x7FFFFFFF;

			m_Stream.Write( (uint) serial );

			if ( item is BaseMulti )
				m_Stream.Write( (short) (itemID | 0x4000) );
			else
				m_Stream.Write( (short) itemID );

			if ( amount != 0 )
				m_Stream.Write( (short) amount );

			x &= 0x7FFF;

			if ( direction != 0 )
				x |= 0x8000;

			m_Stream.Write( (short) x );

			y &= 0x3FFF;

			if ( hue != 0 )
				y |= 0x8000;

			if ( flags != 0 )
				y |= 0x4000;

			m_Stream.Write( (short) y );

			if ( direction != 0 )
				m_Stream.Write( (byte) direction );

			m_Stream.Write( (sbyte) loc.Z );

			if ( hue != 0 )
				m_Stream.Write( (ushort) hue );

			if ( flags != 0 )
				m_Stream.Write( (byte) flags );
		}
	}
}