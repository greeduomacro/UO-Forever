using System;
using Server;

namespace Server.Items
{
	public class GreaterAmnesiaPotion : BaseAmnesiaPotion
	{
		public override int Loss { get { return 5; } }
		public override string DefaultName{ get{ return "greater amnesia potion"; } }

		[Constructable]
		public GreaterAmnesiaPotion() : base( PotionEffect.AmnesiaGreater )
		{
		}

		public GreaterAmnesiaPotion( Serial serial ) : base( serial )
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