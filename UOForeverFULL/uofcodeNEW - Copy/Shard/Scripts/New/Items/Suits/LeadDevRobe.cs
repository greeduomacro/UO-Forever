using System;
using Server;

namespace Server.Items
{
	public class LeadDevRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Lead Developer Robe"; } }

		[Constructable]
		public LeadDevRobe() : base( AccessLevel.Developer, 1281, 0x204F )
		{
		}

		public LeadDevRobe( Serial serial ) : base( serial )
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