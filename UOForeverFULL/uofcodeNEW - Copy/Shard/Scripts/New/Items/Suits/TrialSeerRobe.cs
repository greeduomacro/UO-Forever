using System;
using Server;

namespace Server.Items
{
	public class TrialSeerRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Trial Seer Robe"; } }

		[Constructable]
		public TrialSeerRobe() : base( AccessLevel.Seer, 0x1D3, 0x204F )
		{
		}

		public TrialSeerRobe( Serial serial ) : base( serial )
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