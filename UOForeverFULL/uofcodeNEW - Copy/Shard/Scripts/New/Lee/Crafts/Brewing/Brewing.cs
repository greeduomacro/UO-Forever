#region References
using System;

using Server;
using Server.Engines.Craft;
using Server.Items;
#endregion

namespace VitaNex.SuperCrafts
{
	public class Brewing : SuperCraftSystem
	{
		public override SkillName MainSkill { get { return SkillName.Cooking; } }

		public override TextDefinition GumpTitle { get { return "<CENTER>BREWING MENU</CENTER>"; } }

		public override CraftECA ECA { get { return CraftECA.ChanceMinusSixtyToFourtyFive; } }

		public Brewing()
			: base(1, 1, 2.0)
		{
			MarkOption = true;
		}

		public override double GetChanceAtMin(CraftItem item)
		{
			return 0.25;
		}

		public override int CanCraft(Mobile from, IBaseTool tool, Type itemType)
		{
			if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
			{
				return 1044038; // You have worn out your tool!
			}

			if (!BaseTool.CheckAccessible(tool, from))
			{
				return 1044263; // The tool must be on your person to use.
			}

			return 0;
		}

		public override void PlayCraftEffect(Mobile from)
		{ }

		public override int PlayEndingEffect(
			Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
		{
			if (toolBroken)
			{
				from.SendLocalizedMessage(1044038); // You have worn out your tool
			}

			if (failed)
			{
				if (lostMaterial)
				{
					return 1044043; // You failed to create the item, and some of your materials are lost.
				}

				return 1044157; // You failed to create the item, but no materials were lost.
			}

			if (quality == 0)
			{
				return 502785; // You were barely able to make this item.  It's quality is below average.
			}

			if (makersMark && quality == 2)
			{
				return 1044156; // You create an exceptional quality item and affix your maker's mark.
			}

			if (quality == 2)
			{
				return 1044155; // You create an exceptional quality item.
			}

			return 1044154; // You create the item.
		}

		public override void InitCraftList()
		{
			InitIngredients();
			InitBrews();
			InitSpecialBrews();
		}

		private void InitIngredients()
		{
			AddCraft<SackBarley>(
				1044495,
				"Sack of Barley",
				0.0,
				100.0,
				new[] //
				{new ResourceInfo(typeof(BarleySheaf), "Barley Sheaf", 2)},
				c => c.NeedMill = true);

			AddCraft<SackSugar>(
				1044495,
				"Sack of Sugar",
				0.0,
				100.0,
				new[] //
				{new ResourceInfo(typeof(SugarcaneSheaf), "Sugarcane Sheaf", 2)},
				c => c.NeedMill = true);

			AddCraft<SackYeast>(
				1044495,
				"Sack of Yeast",
				0.0,
				100.0,
				new[] //
				{
					new ResourceInfo(typeof(BaseBeverage), 1046458, 1), new ResourceInfo(typeof(Apple), "Apple", 2),
					new ResourceInfo(typeof(Grapes), "Grapes", 2), new ResourceInfo(typeof(Peach), "Peach", 2)
				},
				c => c.NeedHeat = true);
		}

		private void InitBrews()
		{
			AddCraft<PitcherOfAle>(
				"Pitchers",
				"Pitcher of Ale",
				0.0,
				20.0,
				new[] //
				{
					new ResourceInfo(typeof(BaseBeverage), 1046458, 1), new ResourceInfo(typeof(SackYeast), "Yeast", 2),
					new ResourceInfo(typeof(Hops), "Hops", 1)
				});

			AddCraft<PitcherOfCider>(
				"Pitchers",
				"Pitcher of Cider",
				20.0,
				40.0,
				new[] //
				{
					new ResourceInfo(typeof(BaseBeverage), 1046458, 1), new ResourceInfo(typeof(SackYeast), "Yeast", 2),
					new ResourceInfo(typeof(Hops), "Hops", 1), new ResourceInfo(typeof(Apple), "Apple", 1)
				});

			AddCraft<PitcherOfMead>(
				"Pitchers",
				"Pitcher of Mead",
				40.0,
				60.0,
				new[] //
				{
					new ResourceInfo(typeof(BaseBeverage), 1046458, 1), new ResourceInfo(typeof(SackYeast), "Yeast", 2),
					new ResourceInfo(typeof(Hops), "Hops", 1), new ResourceInfo(typeof(JarSyrup), "Syrup", 1)
				});

			AddCraft<PitcherOfWine>(
				"Pitchers",
				"Pitcher of Wine",
				60.0,
				80.0,
				new[] //
				{
					new ResourceInfo(typeof(BaseBeverage), 1046458, 1), new ResourceInfo(typeof(SackYeast), "Yeast", 2),
					new ResourceInfo(typeof(SackSugar), "Sugar", 1), new ResourceInfo(typeof(Grapes), "Grapes", 3)
				});

			AddCraft<PitcherOfLiquor>(
				"Pitchers",
				"Pitcher of Liquor",
				80.0,
				100.0,
				new[] //
				{
					new ResourceInfo(typeof(BaseBeverage), 1046458, 1), new ResourceInfo(typeof(SackYeast), "Yeast", 2),
					new ResourceInfo(typeof(Grapes), "Grapes", 5)
				},
				c => c.NeedHeat = true); // TODO: NeedDistiller (create DistillerAddon)
		}

		private void InitSpecialBrews()
		{
			AddCraft<MoonglowMead>(
				"Special Brews",
				"Moonglow Mead",
				80.0,
				100.0,
				new[] //
				{
					new ResourceInfo(typeof(BaseBeverage), 1046458, 1), new ResourceInfo(typeof(SackYeast), "Yeast", 2),
					new ResourceInfo(typeof(JarHoney), "Honey", 2), new ResourceInfo(typeof(Nightshade), "Nightshade", 2)
				});
		}
	}
}