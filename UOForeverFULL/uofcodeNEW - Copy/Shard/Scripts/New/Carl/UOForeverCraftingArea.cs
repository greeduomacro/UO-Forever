using System;
using Server;

namespace Server.Items
{
	public class UOForeverCraftingArea : BaseBook
	{
		public static readonly BookContent Content = new BookContent
		(
			"UO Forever Crafting Area", "Trystan",

			new BookPageInfo
			(
				"Players that choose to",
				"smith in Britain West at",
				"the main blacksmith will",
				"be granted with a number",
				"of bonuses that you can",
				"not get anywhere else.",
				"If you are crafting in",
				"this area you will"
			),
			new BookPageInfo
			(
				"receive the following",
				"benefits: 1) A chance to",
				"craft slayer weapons. At",
				"100 skill you have a",
				"chance to craft slayers",
				"for the following trade",
				"skills: blacksmiths,",
				"archery, carpentry. At"
			),
			new BookPageInfo
			(
				"120 of those skills the",
				"chance to craft a slayer",
				"weapon increases to",
				"2.5%. 2) 20% less",
				"resources will be used",
				"for everything crafted",
				"in this region. 3) 10%",
				"increased chance to"
			),
			new BookPageInfo
			(
				"craft exceptional",
				"quality equipment."
			)
		);

		public override BookContent DefaultContent{ get{ return Content; } }

		[Constructable]
		public UOForeverCraftingArea() : base( 0x1C13, false )
		{
			Hue = 723;
		}

		public UOForeverCraftingArea( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
} 
