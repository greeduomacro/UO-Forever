using System;
using Server;
using Server.Items;

namespace Server.Misc
{
	public abstract class BaseHalloweenGiftGiver : GiftGiver, IHalloween
	{
		
		public static void Initialize()
		{
			GiftGiving.Register( new HalloweenGiftGiver2011() );
		}
		

		public static bool IsHalloween()
		{
			return IsHalloween( DateTime.UtcNow );
		}

		public static bool IsHalloween( DateTime date )
		{
			foreach ( GiftGiver g in GiftGiving.Givers )
				if ( g is IHalloween && date >= g.Start && date <= g.Finish )
					return true;

			return false;
		}

		  public override DateTime Start{ get{ return new DateTime( 2014, 10, 25 ); } }
          public override DateTime Finish { get { return new DateTime(2014, 11, 7); } }

		public override void GiveGift( Mobile mob )
		{
			TrickOrTreatBag bag = new TrickOrTreatBag( Finish );

			switch ( GiveGift( mob, bag ) )
			{
				case GiftResult.Backpack:
					mob.SendMessage( 0x482, "Happy Halloween from the team!  A trick or treat bag has been placed in your backpack.  Put on a costume and go beg for treats!" );
					break;
				case GiftResult.BankBox:
					mob.SendMessage( 0x482, "Happy Halloween from the team!  A trick or treat bag has been placed in your bank box.  Put on a costume and go beg for treats!" );
					break;
			}
		}
	}
}