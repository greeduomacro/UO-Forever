using System;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
	public class HighMagicAWSTemplate : BaseLootTemplate
	{
		private static List<Tuple<BaseLootSet,double>> m_LootSets;
		public override List<Tuple<BaseLootSet,double>> LootSets{ get{ return m_LootSets; } set{ m_LootSets = value; } }

		public override void InitializeTemplate()
		{
			AddLootSet( HighMagicWeaponLootSets.LootSet( MagicGrade.NonMagical ), 0.5 );
			AddLootSet( HighMagicArmorLootSets.LootSet( MagicGrade.NonMagical ), 0.5 );

			AddLootSet( HighMagicWeaponLootSets.LootSet( MagicGrade.NonetoLowest ), 0.5 );
			AddLootSet( HighMagicArmorLootSets.LootSet( MagicGrade.NonetoLowest ), 0.5 );

			AddLootSet( HighMagicWeaponLootSets.LootSet( MagicGrade.NonetoMedium ), 0.165 );
			AddLootSet( HighMagicArmorLootSets.LootSet( MagicGrade.NonetoMedium ), 0.165 );

			AddLootSet( HighMagicWeaponLootSets.LootSet( MagicGrade.NonetoHigh ), 0.025 );
			AddLootSet( HighMagicArmorLootSets.LootSet( MagicGrade.NonetoHigh ), 0.025 );
		}

		public HighMagicAWSTemplate() : base( 1750 )
		{
		}
	}
}