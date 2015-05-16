using System;
using Server;

namespace Server.Items
{
	public class SeerRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Seer Robe"; } }

		[Constructable]
		public SeerRobe() : base( AccessLevel.Seer, 1267, 0x204F )
		{
		}

		public SeerRobe( Serial serial ) : base( serial )
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