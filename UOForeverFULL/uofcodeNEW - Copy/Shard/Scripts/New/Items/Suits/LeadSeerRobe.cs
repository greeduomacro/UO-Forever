using System;
using Server;

namespace Server.Items
{
	public class LeadSeerRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Lead Seer Robe"; } }

		[Constructable]
		public LeadSeerRobe() : base( AccessLevel.Lead, 2003, 0x204F )
		{
		}

		public LeadSeerRobe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}