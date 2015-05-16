using System;
using System.Collections.Generic;
using Server.Items;
using Server.Regions;

namespace Server
{
	public class LootSystem
	{
		public static readonly bool ExtraLootAsGold = true;

		private static Dictionary<Type, LootCollection> m_Table = new Dictionary<Type, LootCollection>();
		public static Dictionary<Type, LootCollection> Table{ get{ return m_Table; } }

		private static MagicGrade[] m_MagicGrades = Enum.GetValues( typeof(MagicGrade) ) as MagicGrade[];
		public static MagicGrade[] MagicGrades{ get{ return m_MagicGrades; } }
		
		public static void Register( Type type, LootCollection collection )
		{
			Register( type, null, collection );
		}

		public static void Register( Type type, string regionName, LootCollection collection )
		{
			BaseRegion reg = Find( regionName );
			if ( reg != null )
				reg.LootTable.Add( type, collection );
			else
				m_Table.Add( type, collection );
		}

		public static void AddLootSet( Type type, BaseLootSet set, double chance )
		{
			AddLootSet( type, null, set, chance );
		}

		public static void AddLootSet( Type type, string regionName, BaseLootSet set, double chance )
		{
			LootCollection collection;

			BaseRegion reg = Find( regionName );
			if ( reg != null )
				reg.LootTable.TryGetValue( type, out collection );
			else
				m_Table.TryGetValue( type, out collection );

			if ( collection != null )
				collection.AddLoot( set, chance );
			else
				throw new Exception( String.Format( "AddLoot before RegisterCollection for {0}", type.ToString() ) );
		}

		public static BaseRegion Find( string regionName )
		{
			if ( String.IsNullOrEmpty( regionName ) )
				return null;
			
			foreach ( Region reg in Region.Regions )
				if ( reg is BaseRegion && reg.Name == regionName )
					return (BaseRegion)reg;

			return null;
		}

		public static LootCollection GetCollection( Type type )
		{
			LootCollection coll;

			m_Table.TryGetValue( type, out coll );

			return coll;
		}

		public static LootCollection GetRegionCollection( Type type, Region region )
		{
			if ( region is BaseRegion )
			{
				LootCollection coll;

				((BaseRegion)region).LootTable.TryGetValue( type, out coll );

				return coll;
			}

			return null;
		}

		public static void GetMagicMinMax( MagicGrade grade, out int min, out int max )
		{
			switch ( grade )
			{
				default: min = 0; max = 0; break;
				case MagicGrade.NonMagical: min = 0; max = 0; break;
				case MagicGrade.NonetoLowest: min = 0; max = 60; break;
				case MagicGrade.NonetoLower: min = 10; max = 70; break;
				case MagicGrade.NonetoMedium: min = 20; max = 80; break;
				case MagicGrade.NonetoHigh: min = 30; max = 90; break;
				case MagicGrade.NonetoHighest30: min = 30; max = 100; break;
				case MagicGrade.NonetoHighest40: min = 40; max = 100; break;
				case MagicGrade.NonetoHighest45: min = 45; max = 100; break;
				case MagicGrade.NonetoHighest49: min = 49; max = 100; break;

				case MagicGrade.LowesttoHighest: min = 50; max = 100; break;
				case MagicGrade.LowertoHighest60: min = 60; max = 100; break;
				case MagicGrade.LowertoHighest65: min = 65; max = 100; break;
				case MagicGrade.LowtoHighest70: min = 70; max = 100; break;
				case MagicGrade.LowtoHighest74: min = 74; max = 100; break;
				case MagicGrade.MediumtoHighest: min = 75; max = 100; break;
				case MagicGrade.HightoHighest: min = 89; max = 100; break;
			}
		}
	}
}