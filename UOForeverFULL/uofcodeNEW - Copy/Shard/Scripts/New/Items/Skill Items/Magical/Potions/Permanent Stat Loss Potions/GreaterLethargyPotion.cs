using System;
using Server;

namespace Server.Items
{
	public class GreaterLethargyPotion : BaseLethargyPotion
	{
		public override int Loss { get { return 5; } }
		public override string DefaultName{ get{ return "greater lethargy potion"; } }

		[Constructable]
		public GreaterLethargyPotion() : base( PotionEffect.LethargyGreater )
		{
		}

		public GreaterLethargyPotion( Serial serial ) : base( serial )
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