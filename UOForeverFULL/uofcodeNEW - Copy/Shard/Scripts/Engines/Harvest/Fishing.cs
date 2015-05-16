#region References
using System;
using System.Collections.Generic;
using Server.Engines.Conquests;
using Server.Engines.Quests;
using Server.Engines.Quests.Collector;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
#endregion

namespace Server.Engines.Harvest
{
	public class Fishing : HarvestSystem
	{
		private static Fishing m_System;

		public static Fishing System { get { return m_System ?? (m_System = new Fishing()); } }

		public HarvestDefinition Fish { get; private set; }

		private Fishing()
		{
			#region Fishing
			var fish = new HarvestDefinition
			{
				// Resource banks are every 8x8 tiles
				BankWidth = 8,
				BankHeight = 8,
				// Every bank holds from 5 to 15 fish
				MinTotal = 5,
				MaxTotal = 15,
				// A resource bank will respawn its content every 15 to 25 minutes
				MinRespawn = TimeSpan.FromMinutes(15.0),
				MaxRespawn = TimeSpan.FromMinutes(25.0),
				// Skill checking is done on the Fishing skill
				Skill = SkillName.Fishing,
				// Set the list of harvestable tiles
				Tiles = m_WaterTiles,
				// Players must be within N tiles to harvest
				RangedTiles = true,
				MaxRange = 3,
				// One fish per harvest action
				ConsumedPerHarvest = 1,
				ConsumedPerFeluccaHarvest = 1,
				// The fishing
				EffectActions = new[] {12},
				EffectSounds = new int[0],
				EffectCounts = new[] {1},
				EffectDelay = TimeSpan.Zero,
				EffectSoundDelay = TimeSpan.FromSeconds(8.0),
				NoResourcesMessage = 503172,
				FailMessage = 503171,
				TimedOutOfRangeMessage = 500976,
				OutOfRangeMessage = 500976,
				PackFullMessage = 503176,
				ToolBrokeMessage = 503174
			};

			var res = new[] {new HarvestResource(Expansion.None, 00.0, 00.0, 120.0, 1043297, typeof(Fish))};

			var veins = new[] {new HarvestVein(Expansion.None, 120.0, 0.0, res[0], null)};

			fish.Resources = res;
			fish.Veins = veins;

			fish.PlaceAtFeetIfFull = true;

			fish.BonusResources = new[]
			{
				new BonusHarvestResource(Expansion.ML, 0, 99.4, null, null), //set to same chance as mining ml gems
				new BonusHarvestResource(Expansion.ML, 80.0, 0.6, 1072597, typeof(WhitePearl))
			};

			Fish = fish;
			Definitions.Add(Fish);
			#endregion
		}

		public override void OnConcurrentHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{
			from.SendLocalizedMessage(500972); // You are already fishing.
		}

		private class MutateEntry
		{
			public readonly double m_ReqSkill;
			public readonly double m_MinSkill;
			public readonly double m_MaxSkill;
			public readonly bool m_DeepWater;
			public readonly Type[] m_Types;

			public MutateEntry(double reqSkill, double minSkill, double maxSkill, bool deepWater, params Type[] types)
			{
				m_ReqSkill = reqSkill;
				m_MinSkill = minSkill;
				m_MaxSkill = maxSkill;
				m_DeepWater = deepWater;
				m_Types = types;
			}
		}

		private static readonly MutateEntry[] m_MutateTable = new[]
		{
			new MutateEntry(80.0, 80.0, 4080.0, true, typeof(SpecialFishingNet)),
			new MutateEntry(80.0, 80.0, 4080.0, true, typeof(BigFish)),
			new MutateEntry(90.0, 80.0, 3780.0, true, typeof(TreasureMap)),
			new MutateEntry(100.0, 80.0, 3580.0, true, typeof(MessageInABottle)),
			new MutateEntry(110.0, 100.0, 10000.0, true, typeof(WondrousFish)),
			new MutateEntry(0.0, 125.0, -2375.0, false, typeof(PrizedFish), typeof(TrulyRareFish), typeof(PeculiarFish)),
			new MutateEntry(0.0, 105.0, -420.0, false, typeof(Boots), typeof(Shoes), typeof(Sandals), typeof(ThighBoots)),
			new MutateEntry(0.0, 200.0, -200.0, false, new Type[1] {null})
		};

		public override bool SpecialHarvest(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
		{
			var player = from as PlayerMobile;

			if (player != null)
			{
				QuestSystem qs = player.Quest;

				if (qs is CollectorQuest)
				{
					QuestObjective obj = qs.FindObjective(typeof(FishPearlsObjective));

					if (obj != null && !obj.Completed)
					{
						if (Utility.RandomDouble() < 0.5)
						{
							player.SendLocalizedMessage(1055086, "", 0x59);
							// You pull a shellfish out of the water, and find a rainbow pearl inside of it.

							obj.CurProgress++;
						}
						else
						{
							player.SendLocalizedMessage(1055087, "", 0x2C);
							// You pull a shellfish out of the water, but it doesn't have a rainbow pearl.
						}

						return true;
					}
				}
			}

			return false;
		}

		public override Type MutateType(
			Type type, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
		{
			bool deepWater = SpecialFishingNet.FullValidation(map, loc.X, loc.Y);

			double skillBase = from.Skills[SkillName.Fishing].Base;
			double skillValue = from.Skills[SkillName.Fishing].Value;

			for (int i = 0; i < m_MutateTable.Length; ++i)
			{
				MutateEntry entry = m_MutateTable[i];

				if (!deepWater && entry.m_DeepWater)
				{
					continue;
				}

				if (skillBase >= entry.m_ReqSkill)
				{
					double chance = (skillValue - entry.m_MinSkill) / (entry.m_MaxSkill - entry.m_MinSkill);

					if (chance > Utility.RandomDouble())
					{
						return entry.m_Types[Utility.Random(entry.m_Types.Length)];
					}
				}
			}

			return type;
		}

		private static Map SafeMap(Map map)
		{
			if (map == null || map == Map.Internal)
			{
				return Map.Felucca;
			}

			return map;
		}

		public override bool CheckResources(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
		{
			Container pack = from.Backpack;

			if (pack != null)
			{
				List<SOS> messages = pack.FindItemsByType<SOS>();

				for (int i = 0; i < messages.Count; ++i)
				{
					SOS sos = messages[i];

					if (from.Map == sos.TargetMap && from.Skills[SkillName.Fishing].Value >= 90.0 &&
						from.InRange(sos.TargetLocation, (int)(from.Skills[SkillName.Fishing].Value - 75.0)))
					{
						return true;
					}
				}
			}

			return base.CheckResources(from, tool, def, map, loc, timed);
		}

		public override Item Construct(Type type, Mobile from)
		{
			if (type == typeof(TreasureMap))
			{
				int level;
				//if ( from is PlayerMobile && ((PlayerMobile)from).Young && from.Map == Map.Trammel && TreasureMap.IsInHavenIsland( from ) )
				//	level = 0;
				//else
				level = 1;

				return new TreasureMap(level, Map.Felucca);
			}
			else if (type == typeof(MessageInABottle))
			{
				return new MessageInABottle( /*from.Map == Map.Felucca ?*/ Map.Felucca /* : Map.Trammel*/);
			}

			Container pack = from.Backpack;

			if (pack != null)
			{
				List<SOS> messages = pack.FindItemsByType<SOS>();

				for (int i = 0; i < messages.Count; ++i)
				{
					SOS sos = messages[i];

					if (from.Map == sos.TargetMap &&
						from.InRange(sos.TargetLocation, (int)(from.Skills[SkillName.Fishing].Value - 75.0)))
					{
						Item preLoot = null;

						switch (Utility.Random(8))
						{
							case 0: // Body parts
								{
									var list = new[]
									{
										0x1CDD, 0x1CE5, // arm
										0x1CE0, 0x1CE8, // torso
										0x1CE1, 0x1CE9, // head
										0x1CE2, 0x1CEC // leg
									};

									preLoot = new ShipwreckedItem(Utility.RandomList(list));
									break;
								}
							case 1: // Bone parts
								{
									var list = new[]
									{
										0x1AE0, 0x1AE1, 0x1AE2, 0x1AE3, 0x1AE4, // skulls
										0x1B09, 0x1B0A, 0x1B0B, 0x1B0C, 0x1B0D, 0x1B0E, 0x1B0F, 0x1B10, // bone piles
										0x1B15, 0x1B16 // pelvis bones
									};

									preLoot = new ShipwreckedItem(Utility.RandomList(list));
									break;
								}
							case 2: // Paintings and portraits
								{
									preLoot = new ShipwreckedItem(Utility.Random(0xE9F, 10));
									break;
								}
							case 3: // Pillows
								{
									preLoot = new ShipwreckedItem(Utility.Random(0x13A4, 11));
									break;
								}
							case 4: // Shells
								{
									preLoot = new ShipwreckedItem(Utility.Random(0xFC4, 9));
									break;
								}
							case 5: //Hats
								{
									if (Utility.RandomBool())
									{
										preLoot = new SkullCap();
									}
									else
									{
										preLoot = new TricorneHat();
									}

									break;
								}
							case 6: // Misc
								{
									var list = new[]
									{
										0x1EB5, // unfinished barrel
										0xA2A, // stool
										0xC1F, // broken clock
										0x1047, 0x1048, // globe
										0x1EB1, 0x1EB2, 0x1EB3, 0x1EB4 // barrel staves
									};

									if (Utility.Random(list.Length + 1) == 0)
									{
										preLoot = new Candelabra();
									}
									else
									{
										preLoot = new ShipwreckedItem(Utility.RandomList(list));
									}

									break;
								}
						}

						if (preLoot != null)
						{
							if (preLoot is IShipwreckedItem)
							{
								((IShipwreckedItem)preLoot).IsShipwreckedItem = true;
							}

							return preLoot;
						}

						LockableContainer chest = null;

						switch (Utility.Random(3))
						{
							case 0:
								chest = new MetalGoldenChest();
								break;
							case 1:
								chest = new MetalChest();
								break;
							default:
							case 2:
								chest = new WoodenChest();
								break;
						}
						chest.Breakable = false;
						chest.Locked = false;

						if (sos.IsAncient)
						{
							int hue = 1150;

							if (0.20 > Utility.RandomDouble())
							{
								switch (Utility.Random((chest is WoodenChest) ? 6 : 14))
								{
									case 0:
										hue = 1193;
										break;
									case 1:
										hue = 1281;
										break;
									case 2:
										hue = 1190;
										break;
									case 3:
										hue = 1165;
										break;
									case 4:
										hue = 1160;
										break;
									case 5:
										hue = 1126;
										break;
									case 6:
										hue = CraftResources.GetInfo(CraftResource.Valorite).Hue;
										break;
									case 7:
										hue = CraftResources.GetInfo(CraftResource.Verite).Hue;
										break;
									case 8:
										hue = CraftResources.GetInfo(CraftResource.Agapite).Hue;
										break;
									case 9:
										hue = CraftResources.GetInfo(CraftResource.Gold).Hue;
										break;
									case 10:
										hue = CraftResources.GetInfo(CraftResource.Bronze).Hue;
										break;
									case 11:
										hue = CraftResources.GetInfo(CraftResource.Copper).Hue;
										break;
									case 12:
										hue = CraftResources.GetInfo(CraftResource.ShadowIron).Hue;
										break;
									case 13:
										hue = CraftResources.GetInfo(CraftResource.DullCopper).Hue;
										break;
								}
							}

							chest.Hue = hue;
						}
						else if ((chest is MetalChest || chest is MetalGoldenChest) && (0.5 * sos.Level) >= Utility.RandomDouble())
						{
							int randhue = Utility.Random(120);
							var resource = CraftResource.None;

							if (randhue >= 118)
							{
								resource = CraftResource.Valorite;
							}
							else if (randhue >= 115)
							{
								resource = CraftResource.Verite;
							}
							else if (randhue >= 110)
							{
								resource = CraftResource.Agapite;
							}
							else if (randhue >= 100)
							{
								resource = CraftResource.Gold;
							}
							else if (randhue >= 90)
							{
								resource = CraftResource.Bronze;
							}
							else if (randhue >= 70)
							{
								resource = CraftResource.Copper;
							}
							else if (randhue >= 40)
							{
								resource = CraftResource.ShadowIron;
							}
							else
							{
								resource = CraftResource.DullCopper;
							}

							chest.Hue = CraftResources.GetInfo(resource).Hue;
						}

						int soslevel = sos.Level;

						TreasureMapChest.Fill(chest, soslevel, from.Expansion);

						if (!sos.IsAncient)
						{
							chest.Locked = false;
						}

						double chance = Utility.RandomDouble();
						BaseCreature mibmonster1;
						BaseCreature mibmonster2;
						mibmonster1 = new DeepWaterElemental();
						if (chance <= 0.02 && from.Skills[SkillName.Fishing].Base >= 115)
						{
							mibmonster2 = new Osiredon();
							int choice = Utility.Random(5);
							switch (choice)
							{
								case 4:
									mibmonster2.PackItem(new SmallFishingNetDeed());
									break;
								case 3:
									mibmonster2.PackItem(new LargeFishingNetDeed());
									break;
								case 2:
									mibmonster2.PackItem(new Shell());
									break;
								case 1:
									mibmonster2.PackItem(new Anchor());
									break;
								case 0:
									mibmonster2.PackItem(new Hook());
									break;
							}
						}
						else if (soslevel < 5)
						{
							mibmonster2 = new WaterElemental();
						}
						else
						{
							mibmonster2 = new DeepWaterElemental();
						}
						int x = from.X, y = from.Y;

						Map map = from.Map;

						mibmonster1.MoveToWorld(new Point3D(x, y, -5), map);
						mibmonster2.MoveToWorld(new Point3D(x, y, -5), map);

						mibmonster1.Home = mibmonster1.Location;
						mibmonster1.HomeMap = mibmonster1.Map;
						mibmonster1.RangeHome = 10;

						mibmonster2.Home = mibmonster2.Location;
						mibmonster2.HomeMap = mibmonster2.Map;
						mibmonster2.RangeHome = 10;

						if (sos.IsAncient)
						{
							chest.DropItem(new FabledFishingNet());
						}
						else
						{
							chest.DropItem(new SpecialFishingNet());
						}

						chest.Movable = true;
						chest.Name = "treasure chest";
						chest.IsShipwreckedItem = true;

						if (sos.Level > 0)
						{
							chest.TrapType = TrapType.ExplosionTrap;
							chest.TrapPower = soslevel * Utility.RandomMinMax(9, 19);
							chest.TrapLevel = 0;
						}
						else
						{
							chest.TrapType = TrapType.None;
							chest.TrapPower = 1;
							chest.TrapLevel = 1;
						}

						sos.Delete();

						return chest;
					}
				}
			}

			return base.Construct(type, from);
		}

		public override bool Give(Mobile m, Item item, bool placeAtFeet)
		{
			if (item is TreasureMap || item is MessageInABottle || item is SpecialFishingNet)
			{
				BaseCreature serp;
				if (0.25 > Utility.RandomDouble())
				{
					serp = new DeepSeaSerpent();
				}
				else
				{
					serp = new SeaSerpent();
				}

				int x = m.X, y = m.Y;

				Map map = m.Map;

				for (int i = 0; map != null && i < 20; ++i)
				{
					int tx = m.X - 10 + Utility.Random(21);
					int ty = m.Y - 10 + Utility.Random(21);

					LandTile t = map.Tiles.GetLandTile(tx, ty);

					if (t.Z == -5 && ((t.ID >= 0xA8 && t.ID <= 0xAB) || (t.ID >= 0x136 && t.ID <= 0x137)) &&
						!SpellHelper.CheckMulti(new Point3D(tx, ty, -5), map))
					{
						x = tx;
						y = ty;
						break;
					}
				}

				serp.MoveToWorld(new Point3D(x, y, -5), map);

				serp.Home = serp.Location;
				serp.HomeMap = serp.Map;
				serp.RangeHome = 10;

				serp.PackItem(item);

				m.SendLocalizedMessage(503170); // Uh oh! That doesn't look like a fish!

				return true; // we don't want to give the item to the player, it's on the serpent
			}

			if (item is BigFish || item is WoodenChest || item is MetalGoldenChest)
			{
				placeAtFeet = true;
			}

			return base.Give(m, item, placeAtFeet);
		}

		public override void SendSuccessTo(Mobile from, Item item, HarvestResource resource)
		{
			if (item is BigFish)
			{
				from.SendLocalizedMessage(1042635); // Your fishing pole bends as you pull a big fish from the depths!

				((BigFish)item).Fisher = from;
			}
			else if (item is WoodenChest || item is MetalGoldenChest || item is MetalChest)
			{
				from.SendLocalizedMessage(503175); // You pull up a heavy chest from the depths of the ocean!
			}
			else
			{
				//int number = 0;
				string name;

				/*if (item is WondrousFish)
                {
                    number = 1008125;
                    name = "a terrible monster";
                }
                else*/
				if (item is BaseMagicFish)
				{
					//number = 1008124;
					name = "a mess of small fish";
				}
				else if (item is Fish)
				{
					//number = 1008124;
					name = item.ItemData.Name;
				}
				else if (item is BaseShoes)
				{
					//number = 1008124;
					name = item.ItemData.Name;
				}
				else if (item is TreasureMap)
				{
					//number = 1008125;
					name = "a sodden piece of parchment";
				}
				else if (item is MessageInABottle)
				{
					//number = 1008125;
					name = "a bottle, with a message in it";
				}
				else if (item is SpecialFishingNet)
				{
					//number = 1008125;
					name = "a special fishing net"; // TODO: this is just a guess--what should it really be named?
				}
				else
				{
					//number = 1043297;

					if ((item.ItemData.Flags & TileFlag.ArticleA) != 0)
					{
						name = "a " + item.ItemData.Name;
					}
					else if ((item.ItemData.Flags & TileFlag.ArticleAn) != 0)
					{
						name = "an " + item.ItemData.Name;
					}
					else
					{
						name = item.ItemData.Name;
					}
				}

				NetState ns = from.NetState;

				if (ns == null)
				{
					return;
				}

				from.SendMessage("You pull " + name + " out of the sea.");
			}

            Conquests.Conquests.CheckProgress<CraftingConquest>(
                from as PlayerMobile, item, 0, false, CraftResource.None, null);
		}

		public override void OnHarvestStarted(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{
			base.OnHarvestStarted(from, tool, def, toHarvest);

			int tileID;
			Map map;
			Point3D loc;

			if (GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
			{
				Timer.DelayCall(
					TimeSpan.FromSeconds(1.5),
					delegate
					{
						if (from.EraML)
						{
							from.RevealingAction();
						}

						Effects.SendLocationEffect(loc, map, 0x352D, 16, 4);
						Effects.PlaySound(loc, map, 0x364);
					});
			}
		}

		public override void OnHarvestFinished(
			Mobile from,
			Item tool,
			HarvestDefinition def,
			HarvestVein vein,
			HarvestBank bank,
			HarvestResource resource,
			object harvested)
		{
			base.OnHarvestFinished(from, tool, def, vein, bank, resource, harvested);

			if (from.EraML)
			{
				from.RevealingAction();
			}
		}

		public override object GetLock(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
		{
			return this;
		}

		public override bool BeginHarvesting(Mobile from, Item tool)
		{
			if (!base.BeginHarvesting(from, tool))
			{
				return false;
			}

			from.SendLocalizedMessage(500974); // What water do you want to fish in?
			return true;
		}

		public override bool CheckHarvest(Mobile from, Item tool)
		{
			if (!base.CheckHarvest(from, tool))
			{
				return false;
			}

			if (from.Mounted)
			{
				from.SendLocalizedMessage(500971); // You can't fish while riding!
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

			if (from.Mounted)
			{
				from.SendLocalizedMessage(500971); // You can't fish while riding!
				return false;
			}

			return true;
		}

		private static readonly int[] m_WaterTiles = new[]
		{0x00A8, 0x00AB, 0x0136, 0x0137, 0x5797, 0x579C, 0x746E, 0x7485, 0x7490, 0x74AB, 0x74B5, 0x75D5};
	}
}