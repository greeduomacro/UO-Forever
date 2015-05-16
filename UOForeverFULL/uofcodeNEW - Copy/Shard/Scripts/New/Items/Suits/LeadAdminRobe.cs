using System;
using Server;

namespace Server.Items
{
	public class LeadAdminRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Lead Admin Robe"; } }

		[Constructable]
		public LeadAdminRobe() : base( AccessLevel.Administrator, 1153, 0x204F )
		{
		}

		public LeadAdminRobe( Serial serial ) : base( serial )
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