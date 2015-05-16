using System;
using Server.Items;

namespace Server
{
	public class DemonicBoneAshLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 50; } }

		public DemonicBoneAshLootSet( int amt ) : this( amt, amt )
		{
		}

		public DemonicBoneAshLootSet( int min, int max ) : base( min, max, new Type[]{ typeof( DemonicBoneAsh ) } )
		{
		}
	}
}