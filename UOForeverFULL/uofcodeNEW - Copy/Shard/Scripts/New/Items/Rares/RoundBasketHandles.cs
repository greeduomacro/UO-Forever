using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.Plants;

public class RoundBasketHandles : BaseContainer
	{
        //public override int LabelNumber { get { return 1112293; } } // Basket
        public bool RetainsColorFrom { get { return true; } }

		[Constructable]
        public RoundBasketHandles(): this(1)
		{
			Weight = 1.0; 
		}

        [Constructable]
        public RoundBasketHandles(int amount) : base(0x9AC)
		{
            Weight = 1.0;

            Name = "Round basket w/Handles";  			
			Stackable = true;
			Amount = amount; 
		}


		public RoundBasketHandles( Serial serial ) : base( serial )
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