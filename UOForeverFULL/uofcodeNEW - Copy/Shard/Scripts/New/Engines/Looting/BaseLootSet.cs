using System;
using System.Collections.Generic;

namespace Server
{
	public abstract class BaseLootSet
	{
		public abstract int BaseValue{ get; }

		public abstract Tuple<Item[],int> GenerateLootItem( Mobile creature );

		public BaseLootSet()
		{
		}
	}
}