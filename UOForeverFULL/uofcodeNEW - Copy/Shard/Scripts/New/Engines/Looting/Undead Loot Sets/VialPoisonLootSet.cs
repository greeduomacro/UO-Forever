using System;
using Server.Items;

namespace Server
{
	public class VialPoisonLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 25; } }

		public VialPoisonLootSet( int amt ) : this( amt, amt )
		{
		}

		public VialPoisonLootSet( int min, int max ) : base( min, max, new Type[]{ typeof( VialOfPoison ) } )
		{
		}
	}
}