using System;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
	public class AllRegsLootBagSet : BaseCommodityLootBagSet
	{
		private static Type[] AllRegTypes = new Type[]
			{
				typeof( BlackPearl ),			typeof( Bloodmoss ),			typeof( Garlic ),
				typeof( Ginseng ),				typeof( MandrakeRoot ),			typeof( Nightshade ),
				typeof( SulfurousAsh ),			typeof( SpidersSilk )
			};

		public AllRegsLootBagSet() : this( 60 )
		{
		}

		public AllRegsLootBagSet( int amount ) : this( amount, amount )
		{
		}

		public AllRegsLootBagSet( int mina, int maxa ) : this( AllRegTypes.Length, AllRegTypes.Length, false, mina, maxa )
		{
		}

		public AllRegsLootBagSet( int minc, int maxc, bool random, int mina, int maxa ) : base( minc, maxc, random, mina, maxa, AllRegTypes, BasicBags )
		{
		}
	}

	public class RecallRegsLootBagSet : BaseCommodityLootBagSet
	{
		private static Type[] RecallRegTypes = new Type[]
			{
				typeof( BlackPearl ),			typeof( Bloodmoss ),			typeof( MandrakeRoot ),
				typeof( SulfurousAsh )
			};

		public RecallRegsLootBagSet() : this( 30 )
		{
		}

		public RecallRegsLootBagSet( int amount ) : this( amount, amount )
		{
		}

		public RecallRegsLootBagSet( int mina, int maxa ) : base( RecallRegTypes.Length, RecallRegTypes.Length, false, mina, maxa, RecallRegTypes, BasicBags )
		{
		}
	}

	public class HealRegsLootBagSet : BaseCommodityLootBagSet
	{
		private static Type[] HealRegTypes = new Type[]
			{
				typeof( Garlic ),			typeof( Ginseng ),			typeof( MandrakeRoot ),
				typeof( SpidersSilk )
			};

		public HealRegsLootBagSet() : this( 30 )
		{
		}

		public HealRegsLootBagSet( int amount ) : this( amount, amount )
		{
		}

		public HealRegsLootBagSet( int mina, int maxa ) : base( HealRegTypes.Length, HealRegTypes.Length, false, mina, maxa, HealRegTypes, BasicBags )
		{
		}
	}
}