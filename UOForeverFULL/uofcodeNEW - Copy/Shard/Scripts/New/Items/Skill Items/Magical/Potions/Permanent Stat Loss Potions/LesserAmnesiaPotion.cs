using System;
using Server;

namespace Server.Items
{
	public class LesserAmnesiaPotion : BaseAmnesiaPotion
	{
		public override int Loss { get { return 1; } }
		public override string DefaultName{ get{ return "lesser amnesia potion"; } }

		[Constructable]
		public LesserAmnesiaPotion() : base( PotionEffect.AmnesiaLesser )
		{
		}

		public LesserAmnesiaPotion( Serial serial ) : base( serial )
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