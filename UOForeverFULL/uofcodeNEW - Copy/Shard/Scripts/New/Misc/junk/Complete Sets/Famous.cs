//Animal Crackers Made This xD
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class Famous : HoodedShroudOfShadows
  {


		 
      
      [Constructable]
		public Famous()
		{
          Name = "Famous";
          Hue = 1277;
		}

		public Famous( Serial serial ) : base( serial )
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
