using System;
using Server;

namespace Server.Items
{
	[Flipable]
	public class LargeGlowingBeetle3 : BaseLight
	{
		public override int LitItemID
		{
			get
			{
				if ( ItemID == 0x2CFF )
					return 0x2D00;
				else
					return 0x2CFE;
			}
		}

		public override int UnlitItemID
		{
			get
			{
				if ( ItemID == 0x2D00 )
					return 0x2CFF;
				else
					return 0x2CFD;
			}
		}

		[Constructable]
		public LargeGlowingBeetle3() : base( 0x2CFF )
		{
			Movable = true;
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = false;
			Light = LightType.WestBig;
			Weight = 1.0;
		}

		public LargeGlowingBeetle3( Serial serial ) : base( serial )
		{
		}

		public void Flip()
		{
			if ( Light == LightType.WestBig )
				Light = LightType.NorthBig;
			else if ( Light == LightType.NorthBig )
				Light = LightType.WestBig;

			switch ( ItemID )
			{
				case 0x2CFF: ItemID = 0x2CFD; break;
				case 0x2D00: ItemID = 0x2CFE; break;

				case 0x2CFD: ItemID = 0x2CFF; break;
				case 0x2CFE: ItemID = 0x2D00; break;
			}
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