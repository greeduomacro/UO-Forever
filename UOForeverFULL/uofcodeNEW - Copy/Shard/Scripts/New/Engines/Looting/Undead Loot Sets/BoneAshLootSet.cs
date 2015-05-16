using System;
using Server.Items;

namespace Server
{
	public class BoneAshLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 10; } }

		public BoneAshLootSet( int amt ) : this( amt, amt )
		{
		}

		public BoneAshLootSet( int min, int max ) : base( min, max, new Type[]{ typeof( BoneAsh ) } )
		{
		}
	}
}