using System;

namespace Server.Items
{
	public class WhiteClothDyeTub : DyeTub
	{
		public override int LabelNumber{ get{ return 1149984; } } // White Cloth Dye Tub

		[Constructable]
		public WhiteClothDyeTub()
		{
			Hue = DyedHue = 2498;
			Redyable = false;
		}

		public WhiteClothDyeTub( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}