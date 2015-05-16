using System;
using Server.Items;

namespace Server
{
	public class FireHornLootSet : BasicLootSet
	{
		public override int BaseValue{ get{ return 75; } }

		public FireHornLootSet() : base()
		{
		}

		public override Item GenerateItem()
		{
			return new FireHorn();
		}
	}
}