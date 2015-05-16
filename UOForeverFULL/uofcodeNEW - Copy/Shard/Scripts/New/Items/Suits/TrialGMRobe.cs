using System;
using Server;

namespace Server.Items
{
	public class TrialGMRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Trial GM Robe"; } }

		[Constructable]
		public TrialGMRobe() : base( AccessLevel.GameMaster, 37, 0x204F )
		{
		}

		public TrialGMRobe( Serial serial ) : base( serial )
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