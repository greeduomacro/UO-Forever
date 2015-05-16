using System;
using Server;

namespace Server.Items
{
	public class LeadGMRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Lead GM Robe"; } }

		[Constructable]
		public LeadGMRobe() : base( AccessLevel.Lead, 1157, 0x204F )
		{
		}

		public LeadGMRobe( Serial serial ) : base( serial )
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