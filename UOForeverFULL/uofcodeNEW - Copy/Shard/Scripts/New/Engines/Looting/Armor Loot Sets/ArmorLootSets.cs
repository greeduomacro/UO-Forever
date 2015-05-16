using System;
using Server.Items;

namespace Server
{
	public class LowArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( LeatherGloves ),		typeof( LeatherArms ),		typeof( LeatherLegs ),
				typeof( LeatherGorget ),		typeof( LeatherCap ),		typeof( LeatherChest ),
				typeof( FemaleLeatherChest ),	typeof( LeatherShorts ),		typeof( LeatherSkirt ),
				typeof( LeatherBustierArms ),	typeof( Buckler ),			typeof( WoodenShield ),
				typeof( MetalShield )
			};

		public LowArmorLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class MediumArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( LeatherGloves ),		typeof( LeatherArms ),		typeof( LeatherLegs ),
				typeof( LeatherGorget ),		typeof( LeatherCap ),		typeof( LeatherChest ),
				typeof( FemaleLeatherChest ),	typeof( LeatherShorts ),		typeof( LeatherSkirt ),
				typeof( LeatherBustierArms ),	typeof( Buckler ),			typeof( WoodenShield ),
				typeof( MetalShield ),		typeof( StuddedGloves ),		typeof( StuddedArms ),
				typeof( StuddedLegs ),		typeof( StuddedGorget ),		typeof( StuddedCap ),
				typeof( StuddedChest ),		typeof( FemaleStuddedChest ), typeof( StuddedBustierArms ),
				typeof( RingmailGloves ),	typeof( RingmailArms ),		typeof( RingmailLegs ),
				typeof( RingmailChest ),		typeof( BronzeShield ),		typeof( WoodenKiteShield )
			};

		public MediumArmorLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class HighArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( LeatherGloves ),		typeof( LeatherArms ),		typeof( LeatherLegs ),
				typeof( LeatherGorget ),		typeof( LeatherCap ),		typeof( LeatherChest ),
				typeof( FemaleLeatherChest ),	typeof( LeatherShorts ),		typeof( LeatherSkirt ),
				typeof( LeatherBustierArms ),	typeof( Buckler ),			typeof( WoodenShield ),
				typeof( MetalShield ),		typeof( StuddedGloves ),		typeof( StuddedArms ),
				typeof( StuddedLegs ),		typeof( StuddedGorget ),		typeof( StuddedCap ),
				typeof( StuddedChest ),		typeof( FemaleStuddedChest ), typeof( StuddedBustierArms ),
				typeof( RingmailGloves ),	typeof( RingmailArms ),		typeof( RingmailLegs ),
				typeof( RingmailChest ),		typeof( BronzeShield ),		typeof( WoodenKiteShield ),
				typeof( ChainCoif ),		typeof( ChainLegs ),		typeof( ChainChest ),
				typeof( PlateGloves ),		typeof( PlateArms ),		typeof( PlateLegs ),
				typeof( PlateGorget ),		typeof( PlateHelm ),		typeof( Helmet ),
				typeof( Bascinet ),			typeof( NorseHelm ),		typeof( CloseHelm ),
				typeof( MetalKiteShield ),	typeof( HeaterShield ),		typeof( PlateChest )
			};

		public HighArmorLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class LeatherArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( LeatherGloves ),		typeof( LeatherArms ),		typeof( LeatherLegs ),
				typeof( LeatherGorget ),		typeof( LeatherCap ),		typeof( LeatherChest ),
				typeof( FemaleLeatherChest ),	typeof( LeatherShorts ),		typeof( LeatherSkirt ),
				typeof( LeatherBustierArms )
			};

		public LeatherArmorLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class StuddedArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( StuddedGloves ),		typeof( StuddedArms ),
				typeof( StuddedLegs ),		typeof( StuddedGorget ),		typeof( StuddedCap ),
				typeof( StuddedChest ),		typeof( FemaleStuddedChest ),	typeof( StuddedBustierArms )
			};

		public StuddedArmorLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class RangerArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( RangerGloves ),		typeof( RangerArms ),
				typeof( RangerLegs ),		typeof( RangerGorget ),		typeof( RangerCap ),
				typeof( RangerChest ),		typeof( FemaleRangerChest ),	typeof( RangerBustierArms )
			};

		public RangerArmorLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class RingmailArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( RingmailGloves ),	typeof( RingmailArms ),		typeof( RingmailLegs ),
				typeof( RingmailChest )
			};

		public RingmailArmorLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class ChainmailArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( ChainChest ),	typeof( ChainCoif ),		typeof( ChainLegs )
			};

		public ChainmailArmorLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class PlateArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( PlateGloves ),		typeof( PlateArms ),		typeof( PlateLegs ),
				typeof( PlateGorget ),		typeof( PlateHelm ),		typeof( Helmet ),
				typeof( Bascinet ),			typeof( NorseHelm ),		typeof( CloseHelm ),
				typeof( PlateChest )
			};

		public PlateArmorLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class ShieldLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( Buckler ),			typeof( WoodenShield ),		typeof( MetalShield ),
				typeof( BronzeShield ),		typeof( WoodenKiteShield ),	typeof( MetalKiteShield ),
				typeof( HeaterShield )
			};

		public ShieldLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class BoneArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( BoneGloves ),		typeof( BoneArms ),		typeof( BoneChest ),
				typeof( BoneLegs ),			typeof( BoneHelm )
			};

		public BoneArmorLootSet( int min, int max ) : base( min, max, ArmorTypes )
		{
		}
	}

	public class PhoenixArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( PhoenixGloves ),		typeof( PhoenixArms ),		typeof( PhoenixLegs ),
				typeof( PhoenixGorget ),		typeof( PhoenixChest ),		typeof( PhoenixHelm )
			};

		public PhoenixArmorLootSet() : base( 0, 0, ArmorTypes )
		{
		}
	}

	public class DaemonArmorLootSet : BaseArmorLootSet
	{
		private static readonly Type[] ArmorTypes = new Type[]
			{
				typeof( DaemonGloves ),		typeof( DaemonArms ),		typeof( DaemonLegs ),
				typeof( DaemonChest ),		typeof( DaemonHelm )
			};

		public DaemonArmorLootSet() : base( 0, 0, ArmorTypes )
		{
		}
	}

	public class LowMagicArmorLootSets : MagicLootSets<LowArmorLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class MediumMagicArmorLootSets : MagicLootSets<MediumArmorLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class HighMagicArmorLootSets : MagicLootSets<HighArmorLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class MagicLeatherArmorLootSets : MagicLootSets<LeatherArmorLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class MagicStuddedArmorLootSets : MagicLootSets<StuddedArmorLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class MagicRingArmorLootSets : MagicLootSets<RingmailArmorLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class MagicChainArmorLootSets : MagicLootSets<ChainmailArmorLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class MagicPlateArmorLootSets : MagicLootSets<PlateArmorLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class MagicShieldArmorLootSets : MagicLootSets<ShieldLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class MagicBoneArmorLootSets : MagicLootSets<BoneArmorLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}
}