#region References
using System;

using Server.Items;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server.Engines.Harvest
{
	public class Lumberjacking : HarvestSystem
	{
		private static Lumberjacking m_System;

		public static Lumberjacking System { get { return m_System ?? (m_System = new Lumberjacking()); } }

		private readonly HarvestDefinition m_Definition;

		public HarvestDefinition Definition { get { return m_Definition; } }

		private Lumberjacking()
		{
			#region Lumberjacking
			var lumber = new HarvestDefinition
			{
				// Resource banks are every 4x3 tiles
				BankWidth = 2,
				BankHeight = 2,
				// Every bank holds from 20 to 45 logs
				MinTotal = 22,
				MaxTotal = 50,
				// A resource bank will respawn its content every 20 to 30 minutes
				MinRespawn = TimeSpan.FromMinutes(25.0),
				MaxRespawn = TimeSpan.FromMinutes(40.0),
				// Skill checking is done on the Lumberjacking skill
				Skill = SkillName.Lumberjacking,
				// Set the list of harvestable tiles
				Tiles = m_TreeTiles,
				// Players must be within 2 tiles to harvest
				MaxRange = 2,
				// Ten logs per harvest action
				ConsumedPerHarvest = 10,
				ConsumedPerFeluccaHarvest = 20,
				// The chopping effect
				EffectActions = new[] {13},
				EffectSounds = new[] {0x13E},
				EffectCounts = new[] {1, 2, 2, 2, 3},
				EffectDelay = TimeSpan.FromSeconds(1.6),
				EffectSoundDelay = TimeSpan.FromSeconds(0.9),
				NoResourcesMessage = 500493,
				FailMessage = 500495,
				OutOfRangeMessage = 500446,
				PackFullMessage = 500497,
				ToolBrokeMessage = 500499
			};			

            var res = new[]
			{
                // Made plain logs go to 105 so it would be possible to gain by chopping past 100
				new HarvestResource(Expansion.None, 00.0, 00.0, 105.0, 1072540, typeof(Log)),
				new HarvestResource(Expansion.None, 101.0, 80.0, 105.0, 1072541, typeof(OakLog)),
				new HarvestResource(Expansion.None, 105.0, 85.0, 120.0, 1072542, typeof(AshLog)),
				new HarvestResource(Expansion.None, 106.0, 90.0, 135.0, 1072543, typeof(YewLog)),
				new HarvestResource(Expansion.None, 110.0, 95.0, 140.0, 1072544, typeof(HeartwoodLog)),
				new HarvestResource(Expansion.None, 115.0, 100.0, 140.0, 1072545, typeof(BloodwoodLog)),
				new HarvestResource(Expansion.None, 116.0, 100.0, 140.0, 1072546, typeof(FrostwoodLog))
			};

            var veins = new[]
			{
				new HarvestVein(Expansion.None, 49.0, 0.0, res[0], null), // Ordinary Logs
				new HarvestVein(Expansion.None, 29.2, 0.5, res[1], res[0]), // Oak
				new HarvestVein(Expansion.None, 10.0, 0.5, res[2], res[0]), // Ash
				new HarvestVein(Expansion.None, 05.0, 0.5, res[3], res[0]), // Yew
				new HarvestVein(Expansion.None, 03.0, 0.5, res[4], res[0]), // Heartwood
				new HarvestVein(Expansion.None, 02.0, 0.5, res[5], res[0]), // Bloodwood
				new HarvestVein(Expansion.None, 01.8, 0.5, res[6], res[0]), // Frostwood
			};

            lumber.BonusResources = new[]
			{
				new BonusHarvestResource(Expansion.ML, 0, 83.9, null, null), //Nothing
				new BonusHarvestResource(Expansion.ML, 100, 10.0, 1072548, typeof(BarkFragment)),
				new BonusHarvestResource(Expansion.ML, 100, 03.0, 1072550, typeof(LuminescentFungi)),
				new BonusHarvestResource(Expansion.ML, 100, 02.0, 1072547, typeof(SwitchItem)),
				new BonusHarvestResource(Expansion.ML, 100, 01.0, 1072549, typeof(ParasiticPlant)),
				new BonusHarvestResource(Expansion.ML, 100, 00.1, 1072551, typeof(BrilliantAmber)),
				new BonusHarvestResource(
					Expansion.None, 120, 00.01, "You successfully cut away a piece of the tree's Heartwood.", typeof(HeartwoodLog))
			};

			lumber.Resources = res;
			lumber.Veins = veins;

			lumber.PlaceAtFeetIfFull = true;

			lumber.RaceBonus = false;

			//lumber.RandomizeVeins = false;
            // Lets not have recalling to specific trees like mining.
            lumber.RandomizeVeins = true;

			m_Definition = lumber;
			Definitions.Add(lumber);
			#endregion
		}

		public override bool CheckHarvest(Mobile from, Item tool)
		{
			if (!base.CheckHarvest(from, tool))
			{
				return false;
			}

			if (tool.Parent != from)
			{
				from.SendLocalizedMessage(500487); // The axe must be equipped for any serious wood chopping.
				return false;
			}

			return true;
		}

		public override bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{
			if (!base.CheckHarvest(from, tool, def, toHarvest))
			{
				return false;
			}

			if (tool.Parent != from)
			{
				from.SendLocalizedMessage(500487); // The axe must be equipped for any serious wood chopping.
				return false;
			}

			return true;
		}

		public override void OnBadHarvestTarget(Mobile from, Item tool, object toHarvest)
		{
			if (toHarvest is Mobile)
			{
				((Mobile)toHarvest).PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500450, from.NetState);
				// You can only skin dead creatures.
			}
			else if (toHarvest is Item)
			{
				((Item)toHarvest).LabelTo(from, 500464); // Use this on corpses to carve away meat and hide
			}
			else if (toHarvest is StaticTarget || toHarvest is LandTarget)
			{
				from.SendLocalizedMessage(500489); // You can't use an axe on that.
			}
			else
			{
				from.SendLocalizedMessage(1005213); // You can't do that
			}
		}

		public override void OnHarvestStarted(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{
			base.OnHarvestStarted(from, tool, def, toHarvest);

            if (0.01 > Utility.RandomDouble() && from.Skills.Lumberjacking.Value > 100.0)
            {
                from.SendMessage("You have dislodged a serpent from the tree.");
                var serpent = new Mobiles.GiantSerpent();
                try
                {                    
                    serpent.AddToBackpack(new FrostwoodLog(Utility.Random(1, 9)));
                    serpent.OnBeforeSpawn(from.Location, from.Map);
                    serpent.MoveToWorld(from.Location, from.Map);
                    serpent.Combatant = from;
                }
                catch 
                {   }
            }
     

			if (from.EraML)
			{
				from.RevealingAction();
			}
		}

		public static void Configure()
		{
			EventSink.ServerStarted += () => Array.Sort(m_TreeTiles);
		}

		#region Tile lists
		private static readonly int[] m_TreeTiles = new[]
		{
			0x4CCA, 0x4CCB, 0x4CCC, 0x4CCD, 0x4CD0, 0x4CD3, 0x4CD6, 0x4CD8, 0x4CDA, 0x4CDD, 0x4CE0, 0x4CE3, 0x4CE6, 0x4CF8,
			0x4CFB, 0x4CFE, 0x4D01, 0x4D41, 0x4D42, 0x4D43, 0x4D44, 0x4D57, 0x4D58, 0x4D59, 0x4D5A, 0x4D5B, 0x4D6E, 0x4D6F,
			0x4D70, 0x4D71, 0x4D72, 0x4D84, 0x4D85, 0x4D86, 0x52B5, 0x52B6, 0x52B7, 0x52B8, 0x52B9, 0x52BA, 0x52BB, 0x52BC,
			0x52BD, 0x4CCE, 0x4CCF, 0x4CD1, 0x4CD2, 0x4CD4, 0x4CD5, 0x4CD7, 0x4CD9, 0x4CDB, 0x4CDC, 0x4CDE, 0x4CDF, 0x4CE1,
			0x4CE2, 0x4CE4, 0x4CE5, 0x4CE7, 0x4CE8, 0x4CF9, 0x4CFA, 0x4CFC, 0x4CFD, 0x4CFF, 0x4D00, 0x4D02, 0x4D03, 0x4D45,
			0x4D46, 0x4D47, 0x4D48, 0x4D49, 0x4D4A, 0x4D4B, 0x4D4C, 0x4D4D, 0x4D4E, 0x4D4F, 0x4D50, 0x4D51, 0x4D52, 0x4D53,
			0x4D5C, 0x4D5D, 0x4D5E, 0x4D5F, 0x4D60, 0x4D61, 0x4D62, 0x4D63, 0x4D64, 0x4D65, 0x4D66, 0x4D67, 0x4D68, 0x4D69,
			0x4D73, 0x4D74, 0x4D75, 0x4D76, 0x4D77, 0x4D78, 0x4D79, 0x4D7A, 0x4D7B, 0x4D7C, 0x4D7D, 0x4D7E, 0x4D7F, 0x4D87,
			0x4D88, 0x4D89, 0x4D8A, 0x4D8B, 0x4D8C, 0x4D8D, 0x4D8E, 0x4D8F, 0x4D90, 0x4D95, 0x4D96, 0x4D97, 0x4D99, 0x4D9A,
			0x4D9B, 0x4D9D, 0x4D9E, 0x4D9F, 0x4DA1, 0x4DA2, 0x4DA3, 0x4DA5, 0x4DA6, 0x4DA7, 0x4DA9, 0x4DAA, 0x4DAB, 0x52BE,
			0x52BF, 0x52C0, 0x52C1, 0x52C2, 0x52C3, 0x52C4, 0x52C5, 0x52C6, 0x52C7
		};
		#endregion
	}
}