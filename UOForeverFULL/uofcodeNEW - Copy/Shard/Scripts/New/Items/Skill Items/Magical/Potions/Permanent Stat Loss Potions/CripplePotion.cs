using System;
using Server;

namespace Server.Items
{
	public class CripplePotion : BaseCripplePotion
	{
		public override int Loss { get { return 3; } }
		public override string DefaultName{ get{ return "cripple potion"; } }

		[Constructable]
		public CripplePotion() : base( PotionEffect.Cripple )
		{
		}

		public CripplePotion( Serial serial ) : base( serial )
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