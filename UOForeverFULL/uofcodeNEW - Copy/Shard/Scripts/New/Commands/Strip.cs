using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Commands.Generic
{
	public class StripCommand : BaseCommand
	{
		public static void Initialize()
		{
			TargetCommands.Register( new StripCommand() );
		}

		public StripCommand()
		{
			AccessLevel = AccessLevel.GameMaster;
			Supports = CommandSupport.AllMobiles;
			Commands = new string[]{ "Strip" };
			ObjectTypes = ObjectTypes.Mobiles;
			Usage = "Strip";
			Description = "Removes all equipables from a mobile and places it into their backpack";
		}

		public override void Execute( CommandEventArgs e, object obj )
		{
			Mobile from = e.Mobile;
			if ( obj is Mobile )
			{
				Mobile mob = (Mobile)obj;
				if ( mob.AccessLevel > from.AccessLevel )
					return;

				for ( int i = mob.Items.Count - 1; i >= 0; i-- )
				{
					Item item = mob.Items[i];
					Layer layer = item.Layer;
					if ( item.Movable && layer != Layer.Backpack && layer != Layer.Bank && layer != Layer.FacialHair &&
						layer != Layer.Hair && layer != Layer.Mount && layer != Layer.ShopBuy && layer != Layer.ShopResale && layer != Layer.ShopSell )
						mob.AddToBackpack( item );
				}
			}
		}
	}
}