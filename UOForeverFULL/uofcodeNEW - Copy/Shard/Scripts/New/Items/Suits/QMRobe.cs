using System;
using Server;

namespace Server.Items
{
	public class QMRobe : BaseSuit
	{
		public override string DefaultName{ get{ return "QM Robe"; } }

		[Constructable]
		public QMRobe() : base( AccessLevel.Lead, 1151, 0x204F )
		{
		}

		public QMRobe( Serial serial ) : base( serial )
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