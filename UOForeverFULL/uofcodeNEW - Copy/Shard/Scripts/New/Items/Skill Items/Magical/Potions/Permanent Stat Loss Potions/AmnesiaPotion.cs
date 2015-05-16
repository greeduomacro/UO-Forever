using System;
using Server;

namespace Server.Items
{
	public class AmnesiaPotion : BaseAmnesiaPotion
	{
		public override int Loss { get { return 3; } }
		public override string DefaultName{ get{ return "amnesia potion"; } }

		[Constructable]
		public AmnesiaPotion() : base( PotionEffect.Amnesia )
		{
		}

		public AmnesiaPotion( Serial serial ) : base( serial )
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