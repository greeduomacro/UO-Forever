using System;
using Server;
using Server.Items;

namespace Server.Misc
{
	public class HalloweenGiftGiver2012 : BaseHalloweenGiftGiver, IHalloween
	{
		public static void Configure()
		{
			GiftGiving.Register( new HalloweenGiftGiver2011() );
		}

		public override DateTime Start{ get{ return new DateTime( 2012, 10, 25 ); } }
		public override DateTime Finish{ get{ return new DateTime( 2012, 11, 7 ); } }
	}
}