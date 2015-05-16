using System;
using Server;

namespace Server.Items
{
	public class TrickOrTreatBag : Bag
	{
		private DateTime m_EndDate;

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime EndDate{ get{ return m_EndDate; } }

		public override string DefaultName{ get{ return "a trick or treat bag"; } }

		[Constructable]
		public TrickOrTreatBag( DateTime enddate )
		{
			m_EndDate = enddate;
			Hue = 1072;
			Dyable = false;
		}

		public TrickOrTreatBag( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version

			writer.Write( m_EndDate );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt(); // version

			m_EndDate = reader.ReadDateTime();
		}
	}
}