using System;
using Server.Items;

namespace Server
{
	public class LowWeaponLootSet : BaseWeaponLootSet
	{
		private static readonly Type[] WeaponTypes = new Type[]
			{
				typeof( Club ),			typeof( Mace ),			typeof( SkinningKnife ),
				typeof( Dagger ),		typeof( QuarterStaff ),		typeof( Cleaver ),
				typeof( ButcherKnife ),	typeof( ShepherdsCrook ),	typeof( Bow ),
				typeof( Hatchet ),		typeof( Pickaxe ),			typeof( Pitchfork ),
				typeof( SmithHammerWeapon )
			};

		public LowWeaponLootSet( int min, int max ) : base( min, max, WeaponTypes )
		{
		}
	}

	public class MediumWeaponLootSet : BaseWeaponLootSet
	{
		private static readonly Type[] WeaponTypes = new Type[]
			{
				typeof( Club ),			typeof( Mace ),			typeof( SkinningKnife ),
				typeof( Dagger ),		typeof( QuarterStaff ),		typeof( Cleaver ),
				typeof( ButcherKnife ),	typeof( ShepherdsCrook ),	typeof( Bow ),
				typeof( Hatchet ),		typeof( Pickaxe ),			typeof( ExecutionersAxe ),
				typeof( WarAxe ),		typeof( WarMace ),			typeof( Maul ),
				typeof( Bardiche ),		typeof( Crossbow ),			typeof( Pitchfork ),
				typeof( WarFork ),		typeof( Spear ),			typeof( GnarledStaff ),
				typeof( Broadsword ),	typeof( Cutlass ),			typeof( Katana ),
				typeof( Kryss ),		typeof( Scimitar )
			};

		public MediumWeaponLootSet( int min, int max ) : base( min, max, WeaponTypes )
		{
		}
	}

	public class HighWeaponLootSet : BaseWeaponLootSet
	{
		private static readonly Type[] WeaponTypes = new Type[]
			{
				typeof( Club ),			typeof( Mace ),			typeof( SkinningKnife ),
				typeof( Dagger ),		typeof( QuarterStaff ),		typeof( Cleaver ),
				typeof( ButcherKnife ),	typeof( ShepherdsCrook ),	typeof( Bow ),
				typeof( Hatchet ),		typeof( Pickaxe ),			typeof( ExecutionersAxe ),
				typeof( WarAxe ),		typeof( WarMace ),			typeof( Maul ),
				typeof( Bardiche ),		typeof( Crossbow ),			typeof( Pitchfork ),
				typeof( WarFork ),		typeof( Spear ),			typeof( GnarledStaff ),
				typeof( Broadsword ),	typeof( Cutlass ),			typeof( Katana ),
				typeof( Kryss ),		typeof( Scimitar ),			typeof( Longsword ),
				typeof( ThinLongsword ),	typeof( VikingSword ),		typeof( ShortSpear ),
				typeof( BlackStaff ),	typeof( HeavyCrossbow ),		typeof( Halberd ),
				typeof( WarHammer ),	typeof( HammerPick ),		typeof( TwoHandedAxe ),
				typeof( BattleAxe ),	typeof( DoubleAxe ),		typeof( LargeBattleAxe )
			};

		public HighWeaponLootSet( int min, int max ) : base( min, max, WeaponTypes )
		{
		}
	}

	public class LowMagicWeaponLootSets : MagicLootSets<LowWeaponLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class MediumMagicWeaponLootSets : MagicLootSets<MediumWeaponLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}

	public class HighMagicWeaponLootSets : MagicLootSets<HighWeaponLootSet>
	{
		public static void Configure()
		{
			GenerateLootSets();
		}
	}
}