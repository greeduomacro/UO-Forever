using System;
using System.Collections.Generic;

namespace Server
{
	public abstract class BasicLootSet : BaseLootSet
	{
		public BasicLootSet() : base()
		{
		}

		public abstract Item GenerateItem();

		public override Tuple<Item[],int> GenerateLootItem( Mobile creature )
		{
			return new Tuple<Item[], int>( new Item[]{ GenerateItem() }, BaseValue );
		}
	}
}