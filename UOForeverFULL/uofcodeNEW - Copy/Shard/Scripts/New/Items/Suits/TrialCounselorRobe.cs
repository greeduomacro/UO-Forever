using System;
using Server;

namespace Server.Items
{
	public class TrialCounselorRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Trial Counselor Robe"; } }

		[Constructable]
		public TrialCounselorRobe() : base( AccessLevel.Counselor, 0x3, 0x204F )
		{
		}

		public TrialCounselorRobe( Serial serial ) : base( serial )
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