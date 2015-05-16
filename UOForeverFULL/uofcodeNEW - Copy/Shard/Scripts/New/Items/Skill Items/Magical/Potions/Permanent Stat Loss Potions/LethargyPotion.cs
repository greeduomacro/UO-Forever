using System;
using Server;

namespace Server.Items
{
	public class LethargyPotion : BaseLethargyPotion
	{
		public override int Loss { get { return 3; } }
		public override string DefaultName{ get{ return "lethargy potion"; } }

		[Constructable]
		public LethargyPotion() : base( PotionEffect.Lethargy )
		{
		}

		public LethargyPotion( Serial serial ) : base( serial )
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