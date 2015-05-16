using System;
using Server;

namespace Server.Items
{
	public class CounselorRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "Counselor Robe"; } }

		[Constructable]
		public CounselorRobe() : base( AccessLevel.Counselor, 1156, 0x204F )
		{
		}

		public CounselorRobe( Serial serial ) : base( serial )
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