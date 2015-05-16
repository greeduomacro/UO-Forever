using System;
using Server.Items;

namespace Server
{
	public class LesserPotionLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 25; } }

		private static Type[] m_PotionTypes = new Type[]
			{
				typeof( AgilityPotion ),		typeof( StrengthPotion ),		typeof( RefreshPotion ),
				typeof( LesserCurePotion ),		typeof( LesserHealPotion ),		typeof( LesserPoisonPotion ),
				typeof( NightSightPotion ),		typeof( LesserExplosionPotion )
			};

		public LesserPotionLootSet( int min, int max ) : base( min, max, m_PotionTypes )
		{
		}
	}

	public class RegularPotionLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 25; } }

		private static Type[] m_PotionTypes = new Type[]
			{
				typeof( AgilityPotion ),	typeof( StrengthPotion ),	typeof( RefreshPotion ),
				typeof( CurePotion ),		typeof( HealPotion ),		typeof( PoisonPotion ),
				typeof( NightSightPotion ),	typeof( ExplosionPotion )
			};

		public RegularPotionLootSet( int min, int max ) : base( min, max, m_PotionTypes )
		{
		}
	}

	public class LesserOrRegularPotionLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 25; } }

		private static Type[] m_PotionTypes = new Type[]
			{
				typeof( AgilityPotion ),	typeof( StrengthPotion ),	typeof( RefreshPotion ),
				typeof( CurePotion ),		typeof( HealPotion ),		typeof( PoisonPotion ),
				typeof( NightSightPotion ),	typeof( ExplosionPotion ),	typeof( LesserCurePotion ),
				typeof( LesserHealPotion ),	typeof( LesserPoisonPotion ), typeof( LesserExplosionPotion )
			};

		public LesserOrRegularPotionLootSet( int min, int max ) : base( min, max, m_PotionTypes )
		{
		}
	}

	public class GreaterPotionLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 50; } }

		private static Type[] m_PotionTypes = new Type[]
			{
				typeof( GreaterAgilityPotion ),	typeof( GreaterStrengthPotion ),	typeof( TotalRefreshPotion ),
				typeof( GreaterCurePotion ),	typeof( GreaterHealPotion ),		typeof( GreaterPoisonPotion ),
				typeof( NightSightPotion ),		typeof( GreaterExplosionPotion )
			};

		public GreaterPotionLootSet( int min, int max ) : base( min, max, m_PotionTypes )
		{
		}
	}

	public class AnyPotionLootSet : BaseRandomItemLootSet
	{
		public override int BaseValue{ get{ return 40; } }

		private static Type[] m_PotionTypes = new Type[]
			{
				typeof( AgilityPotion ),	typeof( StrengthPotion ),	typeof( RefreshPotion ),
				typeof( CurePotion ),		typeof( HealPotion ),		typeof( PoisonPotion ),
				typeof( NightSightPotion ),	typeof( ExplosionPotion ),	typeof( LesserCurePotion ),
				typeof( LesserHealPotion ),	typeof( LesserPoisonPotion ), typeof( LesserExplosionPotion ),
				typeof( GreaterAgilityPotion ),	typeof( GreaterStrengthPotion ),	typeof( TotalRefreshPotion ),
				typeof( GreaterCurePotion ),	typeof( GreaterHealPotion ),		typeof( GreaterPoisonPotion ),
				typeof( GreaterExplosionPotion )
			};

		public AnyPotionLootSet( int min, int max ) : base( min, max, m_PotionTypes )
		{
		}
	}
}