using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Multis;
using Server.Targeting;

namespace Server.Engines.Apiculture
{
	public enum HiveHealth
	{
		Dying,
		Sickly,
		Healthy,
		Thriving
	}

	public enum HiveStatus
	{
		Empty   		= 0,
		Colonizing		= 1,
		Brooding    	= 3,
		Producing		= 5,

		Stage1			= 1,
		Stage2			= 2,
		Stage3			= 3,
		Stage4			= 4,
		Stage5			= 5
	}

	public enum ResourceStatus
	{
		None		= 0,  //red X
		VeryLow		= 1,  //red -
		Low			= 2,  //yellow -
		Normal		= 3,  //nothing
		High		= 4,  //green +
		VeryHigh	= 5,  //yellow +
		TooHigh		= 6   //red +
	}

	public enum HiveGrowthIndicator
	{
		None = 0,
		LowResources,
		NotHealthy,
		Grown,
		PopulationUp,
		PopulationDown
	}



	public class BeehiveHelper
	{
		public static readonly TimeSpan CheckDelay = TimeSpan.FromHours( 24.0 );
		//public static readonly TimeSpan CheckDelay = TimeSpan.FromSeconds( 1.0 );  //for testing

		public static void Configure()
		{
			EventSink.WorldSave += new WorldSaveEventHandler( EventSink_WorldSave );
		}

		private static void EventSink_WorldSave( WorldSaveEventArgs args)
		{
			HiveUpdateAll();
		}

		public static void HiveUpdateAll()
		{
			//loop through all the hives in the world and update them
			foreach ( Item item in World.Items.Values )
			{
				if( item is Beehive )
				{
					HiveUpdate( (Beehive)item );
				}
			}
		}

		public static void HiveUpdate(Beehive hive)
		{
			if ( !hive.IsCheckable )
				return;

			//make sure it is time for update
			if ( DateTime.UtcNow >= hive.NextCheck )
			{
				hive.NextCheck = DateTime.UtcNow + CheckDelay; //update check timer
				hive.LastGrowth = HiveGrowthIndicator.None; //reset growth indicator

				hive.HiveAge++;	//update age of the hive
				hive.FindFlowersInRange(); //update flowers
				hive.FindWaterInRange();   //update water

				//apply any potions
				hive.ApplyBenefitEffects();

				//apply negative effects
				if( !hive.ApplyMaladiesEffects() )  //Dead
					return;

				//update stage
				hive.Grow();

				//update maladies
				hive.UpdateMaladies();

				hive.BeehiveComponent.InvalidateProperties(); //to refresh beehive properties
			}
		}

		public static int[] m_HeatSources = new int[]
		{
			0x461, 0x48E, // Sandstone oven/fireplace
			0x92B, 0x96C, // Stone oven/fireplace
			0xDE3, 0xDE9, // Campfire
			0xFAC, 0xFAC, // Firepit
			0x184A, 0x184C, // Heating stand (left)
			0x184E, 0x1850, // Heating stand (right)
			0x398C, 0x399F  // Fire field
		};

		public static int[] m_WaterSources = new int[]
		{
			0xB41,	0xB44,
			0x00A8, 0x00AB,
			0x0136, 0x0137,
			0x5797, 0x579C,
			0x746E, 0x7485,
			0x7490, 0x74AB,
			0x74B5, 0x75D5
		};

		public static int[] m_FlowerSources = new int[]
		{
			0xC37,	0xC38,
			0xC45,	0xC54,
			0xC83,	0xC8E,
			0xCBE,	0xCC1,
			0xD29,	0xD29,
			0xD2B,	0xD2B,
			0xD2F,	0xD2F,
			0xD34,	0xD34,
			0xD36,	0xD36,
			0xD96,	0xD96, //Apple trees!
			0xD9A,	0xD9A,
			0xD9E,	0xD9E,
			0xDA2,	0xDA2,
			0xDA6,	0xDA6,
			0xDAA,	0xDAA
		};

		public static bool HasHeatSource( IEntity entity )
		{
			return Find( entity, true, 4, m_HeatSources ) > 0;
		}

		public static int FindFlowerSources( Beehive hive )
		{
			return Find( hive, false, hive.Range, m_FlowerSources );
		}

		public static int FindWaterSources( Beehive hive )
		{
			return Find( hive, false, hive.Range, m_WaterSources );
		}

		public static int Find( IEntity entity, bool zcheck, int range, int[] itemIDs )
		{
			Map map = entity.Map;

			if ( map == null )
				return 0;

			int sources = 0;

			int hrange = range / 2;

			IPooledEnumerable eable = map.GetItemsInRange( entity.Location, hrange );

			foreach ( Item item in eable )
			{
				if ( ( !zcheck || (item.Z + 16) > entity.Z && (entity.Z + 16) > item.Z ) && Find( item.ItemID, itemIDs ) )
				{
					sources++;
					//eable.Free();
					//return true;
				}
			}

			eable.Free();

			for ( int x = -hrange; x <= hrange; ++x )
			{
				for ( int y = -hrange; y <= hrange; ++y )
				{
					int vx = entity.X + x;
					int vy = entity.Y + y;

					StaticTile[] tiles = map.Tiles.GetStaticTiles( vx, vy, true );

					for ( int i = 0; i < tiles.Length; ++i )
					{
						int z = tiles[i].Z;
						int id = tiles[i].ID & 0x3FFF;

						if ( ( !zcheck || (z + 16) > entity.Z && (entity.Z + 16) > z ) && Find( id, itemIDs ) )
							sources++;
							//return true;
					}
				}
			}

			return sources;
		}

		public static bool Find( int itemID, int[] itemIDs )
		{
			bool contains = false;

			for ( int i = 0; !contains && i < itemIDs.Length; i += 2 )
				contains = ( itemID >= itemIDs[i] && itemID <= itemIDs[i + 1] );

			return contains;
		}
	}
}