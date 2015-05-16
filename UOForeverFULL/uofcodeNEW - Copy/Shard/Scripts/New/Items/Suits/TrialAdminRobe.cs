using System;
using Server;

namespace Server.Items
{
	public class TrialAdminRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Trial Admin Robe"; } }

		[Constructable]
		public TrialAdminRobe() : base( AccessLevel.Lead, 1072, 0x204F )
		{
		}

		public TrialAdminRobe( Serial serial ) : base( serial )
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