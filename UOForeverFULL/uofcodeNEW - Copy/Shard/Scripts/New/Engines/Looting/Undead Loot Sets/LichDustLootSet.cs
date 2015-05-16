using System;
using Server.Items;

namespace Server
{
	public class LichDustLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 25; } }

		public LichDustLootSet( int amt ) : this( amt, amt )
		{
		}

		public LichDustLootSet( int min, int max ) : base( min, max, new Type[]{ typeof( LichDust ) } )
		{
		}
	}
}