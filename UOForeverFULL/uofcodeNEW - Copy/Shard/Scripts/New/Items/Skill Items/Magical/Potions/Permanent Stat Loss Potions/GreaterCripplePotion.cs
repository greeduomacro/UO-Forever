using System;
using Server;

namespace Server.Items
{
	public class GreaterCripplePotion : BaseCripplePotion
	{
		public override int Loss { get { return 5; } }
		public override string DefaultName{ get{ return "greater cripple potion"; } }

		[Constructable]
		public GreaterCripplePotion() : base( PotionEffect.CrippleGreater )
		{
		}

		public GreaterCripplePotion( Serial serial ) : base( serial )
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