using System;
using Server;

namespace Server.Items
{
	public class EmissaryRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Robe of the Emissary"; } }

		[Constructable]
		public EmissaryRobe() : base( AccessLevel.Seer, 2575, 0x204F )
		{
		}

		public EmissaryRobe( Serial serial ) : base( serial )
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