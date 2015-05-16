using System;
using Server;
using Server.Items;

namespace Server.Commands
{
	public class FixTreasureCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register( "fixtreasure", AccessLevel.Owner, new CommandEventHandler( FixTreasure_OnCommand ) );
		}

		public static void FixTreasure_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			foreach ( Item item in World.Items.Values )
			{
				if ( item is BaseTreasureChest && !(item is PuzzleChest) )
					FixTreasure( (BaseTreasureChest)item );
			}
		}

		public static void FixTreasure( BaseTreasureChest chest )
		{
			if ( chest.Level == TreasureLevel.Level2 ) //Fix based on skill
			{
				if ( chest.MaxLockLevel == 250 )
					chest.Level = TreasureLevel.Level8;
				else if ( chest.MaxLockLevel == 200 )
					chest.Level = TreasureLevel.Level7;
				else if ( chest.MaxLockLevel == 150 )
					chest.Level = TreasureLevel.Level6;
				else if ( chest.MaxLockLevel == 135 )
					chest.Level = TreasureLevel.Level5;
				else if ( chest.MaxLockLevel == 125 )
					chest.Level = TreasureLevel.Level4;
				else if ( chest.MaxLockLevel == 105 )
					chest.Level = TreasureLevel.Level3;
				else if ( chest.MaxLockLevel == 85 )
					chest.Level = TreasureLevel.Level2;
				else if ( chest.MaxLockLevel == 65 )
					chest.Level = TreasureLevel.Level1;
			}
		}
	}
}