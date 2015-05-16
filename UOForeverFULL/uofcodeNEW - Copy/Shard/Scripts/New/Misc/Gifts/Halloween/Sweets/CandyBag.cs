#region Header Information
//Please feel free to redistribute/Edit this script as you wish.
//And enjoy the sweets!!! :D
//Scripter: Typhoonbot.
#endregion

using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class CandyBag : Bag
	{
		[Constructable]
        public CandyBag() : this (1)
		{
            Name = "Bag of Halloween Candy";
            Hue = 243;
        }
		[Constructable]
        public CandyBag( int amount )
        {
            DropItem(new ChocolateBrownie());
            DropItem(new ChocolateBrownie());
	    DropItem(new ChocolateBrownie());
	    DropItem(new ChocolateBrownie());
            DropItem(new ChocolateBar());
            DropItem(new ChocolateBar());
	    DropItem(new ChocolateBar());
	    DropItem(new ChocolateBar());	
            DropItem(new HardBoiledSweet());
            DropItem(new HardBoiledSweet());
	    DropItem(new HardBoiledSweet());
	    DropItem(new HardBoiledSweet());
            DropItem(new ChocolateCandy());
            DropItem(new ChocolateCandy());
            DropItem(new ChocolateCandy());
	    DropItem(new ChocolateCandy());
	    DropItem(new ChocolateCandy());
	    DropItem(new ChocolateCandy());
            DropItem(new WineGums());
            DropItem(new WineGums());
            DropItem(new WineGums());
	    DropItem(new WineGums());
            DropItem(new LollyPop());
	    DropItem(new LollyPop());
	    DropItem(new LollyPop());
	    DropItem(new LollyPop());
	    DropItem(new LollyPop());
            DropItem(new LollyPop());
            DropItem(new Marshmallow());
            DropItem(new Marshmallow());
	    DropItem(new Marshmallow());
	    DropItem(new Marshmallow());
            DropItem(new ChocolateHeart());
	    DropItem(new ChocolateHeart());
	    DropItem(new ChocolateHeart());
	    DropItem(new ChocolateHeart());
		}

        public CandyBag(Serial serial)
            : base(serial)
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
