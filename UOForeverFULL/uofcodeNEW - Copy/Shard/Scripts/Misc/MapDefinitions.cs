#region References
using VitaNex.Network;
#endregion

namespace Server.Misc
{
	public class MapDefinitions
	{
		public static void Configure()
		{
			/* Here we configure all maps. Some notes:
			 *
			 * 1) The first 32 maps are reserved for core use.
			 * 2) Map 0x7F is reserved for core use.
			 * 3) Map 0xFF is reserved for core use.
			 * 4) Changing or removing any predefined maps may cause server instability.
			 */

			RegisterMap(0, 0, 0, 7168, 4096, Season.Desolation, Expansion.UOR, "Felucca", MapRules.FeluccaRules);
			RegisterMap(1, 1, 1, 7168, 4096, Season.Winter, Expansion.UOR, "Trammel", MapRules.FeluccaRules);
			RegisterMap(2, 2, 2, 2304, 1600, Season.Summer, Expansion.UOR, "Ilshenar", MapRules.FeluccaRules);
			RegisterMap(3, 3, 3, 2560, 2048, Season.Summer, Expansion.UOR, "Malas", MapRules.FeluccaRules);
			RegisterMap(4, 4, 4, 1448, 1448, Season.Summer, Expansion.UOR, "Tokuno", MapRules.FeluccaRules);
			RegisterMap(5, 5, 5, 1280, 4096, Season.Summer, Expansion.UOR, "Ter Mur", MapRules.FeluccaRules);

			RegisterMap(6, 0, 0, 7168, 4096, Season.Desolation, Expansion.T2A, "Lost Lands", MapRules.FeluccaRules); // Felucca clone

            RegisterMap(7, 0, 0, 7168, 4096, Season.Desolation, Expansion.UOTD, "Zombieland", MapRules.FeluccaRules); // Zombie Event

			RegisterMap(0x7F, 0x7F, 0x7F, Map.SectorSize, Map.SectorSize, Season.Desolation, Expansion.None, "Internal", MapRules.Internal);

			/* Example of registering a custom map:
			 * RegisterMap( 32, 0, 0, 6144, 4096, Season.Desolation, Expansion.None, "Iceland", MapRules.FeluccaRules );
			 *
			 * Defined:
			 * RegisterMap( <index>, <mapID>, <fileIndex>, <width>, <height>, <season>, <expansion>, <name>, <rules> );
			 *  - <index> : An unreserved unique index for this map
			 *  - <mapID> : An identification number used in client communications. For any visible maps, this value must be from 0-3
			 *  - <fileIndex> : A file identification number. For any visible maps, this value must be 0, 2, 3, or 4
			 *  - <width>, <height> : Size of the map (in tiles)
			 *  - <expansion> : The supported Expansion, this controls the features that are available to players in this facet.
			 *  - <name> : Reference name for the map, used in props gump, get/set commands, region loading, etc
			 *  - <rules> : Rules and restrictions associated with the map. See documentation for details
			*/

			TileMatrixPatch.Enabled = false; // OSI Client Patch 6.0.0.0

			MultiComponentList.PostHSFormat = true; // OSI Client Patch 7.0.9.0
		}

		public static void RegisterMap(
			int mapIndex, int mapID, int fileIndex, int width, int height, Season season, Expansion ex, string name, MapRules rules)
		{
			var newMap = new Map(mapID, mapIndex, fileIndex, width, height, season.GetID(), ex, name, rules);

			Map.Maps[mapIndex] = newMap;
			Map.AllMaps.Add(newMap);
		}
	}
}