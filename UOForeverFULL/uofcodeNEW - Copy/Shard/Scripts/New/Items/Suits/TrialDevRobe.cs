using System;
using Server;

namespace Server.Items
{
	public class TrialDevRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Trial Developer Robe"; } }

		[Constructable]
		public TrialDevRobe() : base( AccessLevel.Developer, 0x966, 0x204F )
		{
		}

		public TrialDevRobe( Serial serial ) : base( serial )
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