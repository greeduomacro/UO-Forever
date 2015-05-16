using System;
using Server;

namespace Server.Items
{
	public class LeadCounselorRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Lead Counselor Robe"; } }

		[Constructable]
		public LeadCounselorRobe() : base( AccessLevel.GameMaster, 1365, 0x204F )
		{
		}

		public LeadCounselorRobe( Serial serial ) : base( serial )
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