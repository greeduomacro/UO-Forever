using System;
using Server.Items;

namespace Server
{
	public class EtherealResidueLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 100; } }

		public EtherealResidueLootSet( int amt ) : this( amt, amt )
		{
		}

		public EtherealResidueLootSet( int min, int max ) : base( min, max, new Type[]{ typeof( EtherealResidue ) } )
		{
		}
	}
}