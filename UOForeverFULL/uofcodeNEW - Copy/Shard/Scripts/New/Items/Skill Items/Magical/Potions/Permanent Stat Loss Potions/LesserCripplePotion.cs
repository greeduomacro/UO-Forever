using System;
using Server;

namespace Server.Items
{
	public class LesserCripplePotion : BaseCripplePotion
	{
		public override int Loss { get { return 1; } }
		public override string DefaultName{ get{ return "lesser cripple potion"; } }

		[Constructable]
		public LesserCripplePotion() : base( PotionEffect.CrippleLesser )
		{
		}

		public LesserCripplePotion( Serial serial ) : base( serial )
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