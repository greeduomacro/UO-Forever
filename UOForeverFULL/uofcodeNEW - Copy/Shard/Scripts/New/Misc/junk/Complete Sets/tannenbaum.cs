using System;

namespace Server.Items
{
	public class tannenbaum : GreaterExplosionPotion
	{
		[Constructable]
		public tannenbaum() 
		{
			ItemID = 3287;
		      	Weight = 1.0;
			Name = "Christmas Tree - A tannen-bomb. (tannenbaum)";
		}

		public tannenbaum( Serial serial ) : base( serial )
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