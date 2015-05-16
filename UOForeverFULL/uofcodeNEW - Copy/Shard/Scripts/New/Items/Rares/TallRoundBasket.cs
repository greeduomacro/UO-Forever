using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.Plants;

public class TallRoundBasket : BaseContainer
	{
        public override int LabelNumber { get { return 1112297; } } //Tall Round Basket
        public bool RetainsColorFrom { get { return true; } }

		[Constructable]
        public TallRoundBasket(): this(1)
		{
			Weight = 1.0; 
		}

        [Constructable]
        public TallRoundBasket(int amount) : base(0x24D8)
		{
            Weight = 1.0;

            //Hue = 0;  			
			Stackable = true;
			Amount = amount; 
		}


		public TallRoundBasket( Serial serial ) : base( serial )
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