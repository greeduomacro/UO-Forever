#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Commands;
#endregion

namespace Server.Engines.Harvest
{
	public class HarvestDefinition
	{
		public static void Initialize()
		{
			CommandSystem.Register("RefreshResources", AccessLevel.GameMaster, RefreshResources_Command);
		}

		public static void RefreshResources_Command(CommandEventArgs e)
		{
			String[] args = e.Arguments;

			if (args.Length < 2)
			{
				e.Mobile.SendMessage("Usage: [refreshresources x y");
				return;
			}

			try
			{
				Point3D point = new Point3D(int.Parse(args[0]), int.Parse(args[1]), 0);
				RefreshResource(Lumberjacking.System.Definition, Map.Felucca, point.X, point.Y);
				RefreshResource(Mining.System.OreAndStone, Map.Felucca, point.X, point.Y);
			}
			catch
			{
				e.Mobile.SendMessage("Usage: [refreshresources x y (z)");
			}
		}

		public int BankWidth { get; set; }
		public int BankHeight { get; set; }
		public int MinTotal { get; set; }
		public int MaxTotal { get; set; }
		public int[] Tiles { get; set; }
		public bool RangedTiles { get; set; }
		public TimeSpan MinRespawn { get; set; }
		public TimeSpan MaxRespawn { get; set; }
		public int MaxRange { get; set; }
		public int ConsumedPerHarvest { get; set; }
		public int ConsumedPerFeluccaHarvest { get; set; }
		public bool PlaceAtFeetIfFull { get; set; }
		public SkillName Skill { get; set; }
		public int[] EffectActions { get; set; }
		public int[] EffectCounts { get; set; }
		public int[] EffectSounds { get; set; }
		public TimeSpan EffectSoundDelay { get; set; }
		public TimeSpan EffectDelay { get; set; }
		public object NoResourcesMessage { get; set; }
		public object OutOfRangeMessage { get; set; }
		public object TimedOutOfRangeMessage { get; set; }
		public object DoubleHarvestMessage { get; set; }
		public object FailMessage { get; set; }
		public object PackFullMessage { get; set; }
		public object ToolBrokeMessage { get; set; }
		public HarvestResource[] Resources { get; set; }
		public HarvestVein[] Veins { get; set; }
		public BonusHarvestResource[] BonusResources { get; set; }
		public bool RaceBonus { get; set; }
		public bool RandomizeVeins { get; set; }

		private Dictionary<Map, Dictionary<Point2D, HarvestBank>> m_BanksByMap;

		public Dictionary<Map, Dictionary<Point2D, HarvestBank>> Banks { get { return m_BanksByMap; } set { m_BanksByMap = value; } }

		public void SendMessageTo(Mobile from, object message)
		{
			if (message is int)
			{
				from.SendLocalizedMessage((int)message);
			}
			else if (message is string)
			{
				from.SendMessage((string)message);
			}
		}

		public HarvestBank GetBank(Map map, int x, int y)
		{
			if (map == null || map == Map.Internal)
			{
				return null;
			}

			x /= BankWidth;
			y /= BankHeight;

			Dictionary<Point2D, HarvestBank> banks = null;
			m_BanksByMap.TryGetValue(map, out banks);

			if (banks == null)
			{
				m_BanksByMap[map] = banks = new Dictionary<Point2D, HarvestBank>();
			}

			var key = new Point2D(x, y);
			HarvestBank bank = null;
			banks.TryGetValue(key, out bank);

			if (bank == null)
			{
				banks[key] = bank = new HarvestBank(this, GetVeinAt(map, x, y));
			}

			return bank;
		}

		public static void RefreshResource(HarvestDefinition definition, Map map, int x, int y)
		{
			HarvestBank bank = definition.GetBank(map, x, y);
			bank.NextRespawn = DateTime.MinValue;
		}

		public HarvestVein GetVeinAt(Map map, int x, int y)
		{
			if (Veins.Length == 1)
			{
				return Veins[0];
			}

			double randomValue;

			if (RandomizeVeins)
			{
				randomValue = Utility.RandomDouble();
			}
			else
			{
				var random = new Random((x * 18) + (y * 12) + (map.MapID * 4));
				randomValue = random.NextDouble();
			}

			return GetVeinFrom(randomValue);
		}

		public HarvestVein GetVeinFrom(double randomValue)
		{
			HarvestVein[] veins = Veins;

			if (veins.Length == 1)
			{
				return veins[0];
			}

			randomValue *= 100;

			foreach (HarvestVein v in veins)
			{
				if (randomValue <= v.VeinChance)
				{
					return v;
				}

				randomValue -= v.VeinChance;
			}

			return null;
		}

		public BonusHarvestResource GetBonusResource(Expansion e)
		{
			if (BonusResources == null)
			{
				return null;
			}

			double randomValue = Utility.RandomDouble() * 100;

			foreach (BonusHarvestResource b in BonusResources.Where(b => e > b.ReqExpansion))
			{
				if (randomValue <= b.Chance)
				{
					return b;
				}

				randomValue -= b.Chance;
			}

			return null;
		}

		public HarvestDefinition()
		{
			m_BanksByMap = new Dictionary<Map, Dictionary<Point2D, HarvestBank>>();
		}

		public bool Validate(int tileID)
		{
			if (RangedTiles)
			{
				bool contains = false;

				for (int i = 0; !contains && i < Tiles.Length; i += 2)
				{
					contains = (tileID >= Tiles[i] && tileID <= Tiles[i + 1]);
				}

				return contains;
			}
			
			int dist = -1;

			for (int i = 0; dist < 0 && i < Tiles.Length; ++i)
			{
				dist = (Tiles[i] - tileID);
			}

			return (dist == 0);
		}
	}
}