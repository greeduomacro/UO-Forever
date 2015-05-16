using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.Plants;

public class WinnowingBasket : BaseContainer
	{
        public override int LabelNumber { get { return 1112355; } } // Basket
        public bool RetainsColorFrom { get { return true; } }

		[Constructable]
        public WinnowingBasket(): this(1)
		{
			Weight = 1.0; 
		}

        [Constructable]
        public WinnowingBasket(int amount) : base(0x1882)
		{
            Weight = 1.0;

            //Hue = 0;  			
			Stackable = true;
			Amount = amount; 
		}


		public WinnowingBasket( Serial serial ) : base( serial )
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