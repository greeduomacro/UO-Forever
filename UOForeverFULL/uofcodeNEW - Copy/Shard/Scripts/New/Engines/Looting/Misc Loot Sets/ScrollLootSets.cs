using System;
using Server.Items;

namespace Server
{
	public class BaseScrollLootSet : BaseRandomItemLootSet
	{
		private int m_MinCircle;
		public int MinCircle{ get{ return m_MinCircle; } }

		private int m_MaxCircle;
		public int MaxCircle{ get{ return m_MaxCircle; } }

		private bool m_CanBeBlank;
		public bool CanBeBlank{ get{ return m_CanBeBlank; } }

		public override int BaseValue{ get{ return 25; } }

		private static Type[] m_RegularScrollTypes = new Type[]
			{
				typeof( ReactiveArmorScroll ),	typeof( ClumsyScroll ),			typeof( CreateFoodScroll ),		typeof( FeeblemindScroll ),
				typeof( HealScroll ),			typeof( MagicArrowScroll ),		typeof( NightSightScroll ),		typeof( WeakenScroll ),
				typeof( AgilityScroll ),		typeof( CunningScroll ),		typeof( CureScroll ),			typeof( HarmScroll ),
				typeof( MagicTrapScroll ),		typeof( MagicUnTrapScroll ),	typeof( ProtectionScroll ),		typeof( StrengthScroll ),
				typeof( BlessScroll ),			typeof( FireballScroll ),		typeof( MagicLockScroll ),		typeof( PoisonScroll ),
				typeof( TelekinesisScroll ),	typeof( TeleportScroll ),		typeof( UnlockScroll ),			typeof( WallOfStoneScroll ),
				typeof( ArchCureScroll ),		typeof( ArchProtectionScroll ),	typeof( CurseScroll ),			typeof( FireFieldScroll ),
				typeof( GreaterHealScroll ),	typeof( LightningScroll ),		typeof( ManaDrainScroll ),		typeof( RecallScroll ),
				typeof( BladeSpiritsScroll ),	typeof( DispelFieldScroll ),	typeof( IncognitoScroll ),		typeof( MagicReflectScroll ),
				typeof( MindBlastScroll ),		typeof( ParalyzeScroll ),		typeof( PoisonFieldScroll ),	typeof( SummonCreatureScroll ),
				typeof( DispelScroll ),			typeof( EnergyBoltScroll ),		typeof( ExplosionScroll ),		typeof( InvisibilityScroll ),
				typeof( MarkScroll ),			typeof( MassCurseScroll ),		typeof( ParalyzeFieldScroll ),	typeof( RevealScroll ),
				typeof( ChainLightningScroll ), typeof( EnergyFieldScroll ),	typeof( FlamestrikeScroll ),	typeof( GateTravelScroll ),
				typeof( ManaVampireScroll ),	typeof( MassDispelScroll ),		typeof( MeteorSwarmScroll ),	typeof( PolymorphScroll ),
				typeof( EarthquakeScroll ),		typeof( EnergyVortexScroll ),	typeof( ResurrectionScroll ),	typeof( SummonAirElementalScroll ),
				typeof( SummonDaemonScroll ),	typeof( SummonEarthElementalScroll ),	typeof( SummonFireElementalScroll ),	typeof( SummonWaterElementalScroll )
			};

		public override Type GetRandomType()
		{
			int min = (m_MinCircle * 8);
			int total = ((m_MaxCircle * 8) + 7) - min;

			int rnd = Utility.Random( total + ( m_CanBeBlank ? 1 : 0 ) );

			if ( rnd >= total ) //blank
				return typeof( BlankScroll );

			return m_RegularScrollTypes[min + rnd];
		}

		public BaseScrollLootSet( int minCircle, int maxCircle ) : this( 1, 1, minCircle, maxCircle, true )
		{
		}

		public BaseScrollLootSet( int amt, int minCircle, int maxCircle ) : this( amt, amt, minCircle, maxCircle, true )
		{
		}

		public BaseScrollLootSet( int min, int max, int minCircle, int maxCircle ) : this( min, max, minCircle, maxCircle, true )
		{
		}

		public BaseScrollLootSet( int min, int max, int minCircle, int maxCircle, bool blank ) : base( min, max, null )
		{
			m_MinCircle = minCircle-1;
			m_MaxCircle = maxCircle-1;
			m_CanBeBlank = blank;
		}
	}

	public class LowScrollLootSet : BaseScrollLootSet
	{
		public LowScrollLootSet( int amt ) : this( amt, amt )
		{
		}

		public LowScrollLootSet( int min, int max ) : base( min, max, 1, 4, true )
		{
		}
	}

	public class MedScrollLootSet : BaseScrollLootSet
	{
		public MedScrollLootSet( int amt ) : this( amt, amt )
		{
		}

		public MedScrollLootSet( int min, int max ) : base( min, max, 3, 6, true )
		{
		}
	}

	public class HighScrollLootSet : BaseScrollLootSet
	{
		public HighScrollLootSet( int amt ) : this( amt, amt )
		{
		}

		public HighScrollLootSet( int min, int max ) : base( min, max, 5, 8, true )
		{
		}
	}
}