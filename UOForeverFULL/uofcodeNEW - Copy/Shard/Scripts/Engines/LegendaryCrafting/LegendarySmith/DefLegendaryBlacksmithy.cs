#region References

using System;
using System.Drawing;
using Server.Items;

#endregion

namespace Server.Engines.Craft
{
    public class DefLegendaryBlacksmithy : CraftSystem
    {
        public override SkillName MainSkill { get { return SkillName.Blacksmith; } }

        public override string GumpTitleString
        {
            get
            {
                return "Legendary Crafting".WrapUOHtmlTag("CENTER").WrapUOHtmlTag("BIG").WrapUOHtmlColor(Color.Orange);
            }
        }

        public static void Initialize()
        {
            m_CraftSystem = new DefLegendaryBlacksmithy();
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get { return m_CraftSystem ?? (m_CraftSystem = new DefLegendaryBlacksmithy()); }
        }

        public override CraftECA ECA { get { return CraftECA.ChanceMinusSixtyToFourtyFive; } }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        private DefLegendaryBlacksmithy()
            : base(1, 1, 1.25) // base( 1, 2, 1.7 )
        {
            /*

			base( MinCraftEffect, MaxCraftEffect, Delay )

			MinCraftEffect	: The minimum number of time the mobile will play the craft effect
			MaxCraftEffect	: The maximum number of time the mobile will play the craft effect
			Delay			: The delay between each craft effect

			Example: (3, 6, 1.7) would make the mobile do the PlayCraftEffect override
			function between 3 and 6 time, with a 1.7 second delay each time.

			*/
        }

        private static readonly Type typeofAnvil = typeof(AnvilAttribute);
        private static readonly Type typeofForge = typeof(ForgeAttribute);

        public static void CheckAnvilAndForge(Mobile from, int range, out bool anvil, out bool forge)
        {
            anvil = false;
            forge = false;

            Map map = from.Map;

            if (map == null)
            {
                return;
            }

            IPooledEnumerable eable = map.GetItemsInRange(from.Location, range);

            foreach (Item item in eable)
            {
                Type type = item.GetType();

                bool isAnvil = (type.IsDefined(typeofAnvil, false) || item.ItemID == 4015 || item.ItemID == 4016 ||
                                item.ItemID == 0x2DD5 || item.ItemID == 0x2DD6);
                bool isForge = (type.IsDefined(typeofForge, false) || item.ItemID == 4017 ||
                                (item.ItemID >= 6522 && item.ItemID <= 6569) || item.ItemID == 0x2DD8);
                bool isSoulForge = (item.ItemID >= 16995 && item.ItemID <= 17010);

                if (isAnvil || isForge || isSoulForge)
                {
                    if ((from.Z + 16) < item.Z || (item.Z + 16) < from.Z || !from.InLOS(item))
                    {
                        continue;
                    }

                    if (isSoulForge)
                    {
                        anvil = true;
                        forge = true;
                    }
                    else
                    {
                        anvil = anvil || isAnvil;
                        forge = forge || isForge;
                    }

                    if (anvil && forge)
                    {
                        break;
                    }
                }
            }

            eable.Free();

            for (int x = -range; (!anvil || !forge) && x <= range; ++x)
            {
                for (int y = -range; (!anvil || !forge) && y <= range; ++y)
                {
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(from.X + x, from.Y + y, true);

                    for (int i = 0; (!anvil || !forge) && i < tiles.Length; ++i)
                    {
                        int id = tiles[i].ID;

                        bool isAnvil = (id == 4015 || id == 4016 || id == 0x2DD5 || id == 0x2DD6);
                        bool isForge = (id == 4017 || (id >= 6522 && id <= 6569) || id == 0x2DD8);

                        if (isAnvil || isForge)
                        {
                            if (from.Z + 16 < tiles[i].Z || tiles[i].Z + 16 < from.Z ||
                                !from.InLOS(new Point3D(from.X + x, from.Y + y, tiles[i].Z + (tiles[i].Height / 2) + 1)))
                            {
                                continue;
                            }

                            anvil = anvil || isAnvil;
                            forge = forge || isForge;
                        }
                    }
                }
            }
        }

        public override int CanCraft(Mobile from, IBaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
            {
                return 1044038; // You have worn out your tool!
            }

            if (!BaseTool.CheckTool(tool, from))
            {
                return 1048146; // If you have a tool equipped, you must use that tool.
            }

            if (!BaseTool.CheckAccessible(tool, from))
            {
                return 1044263; // The tool must be on your person to use.
            }

            bool anvil, forge;
            CheckAnvilAndForge(from, 2, out anvil, out forge);

            if (anvil && forge)
            {
                return 0;
            }

            return 1044267; // You must be near an anvil and a forge to smith items.
        }

        public override void PlayCraftEffect(Mobile from)
        {
            // no animation, instant sound
            //if ( from.Body.Type == BodyType.Human && !from.Mounted )
            //	from.Animate( 9, 5, 1, true, false, 0 );
            //new InternalTimer( from ).Start();

            from.PlaySound(0x2A);
        }

        // Delay to synchronize the sound with the hit on the anvil
        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;

            public InternalTimer(Mobile from)
                : base(TimeSpan.FromSeconds(0.7))
            {
                m_From = from;
            }

            protected override void OnTick()
            {
                m_From.PlaySound(0x2A);
            }
        }

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
            /*
			Synthax for a SIMPLE craft item
			AddCraft( ObjectType, Group, MinSkill, MaxSkill, ResourceType, Amount, Message )

			ObjectType		: The type of the object you want to add to the build list.
			Group			: The group in wich the object will be showed in the craft menu.
			MinSkill		: The minimum of skill value
			MaxSkill		: The maximum of skill value
			ResourceType	: The type of the resource the mobile need to create the item
			Amount			: The amount of the ResourceType it need to create the item
			Message			: String or Int for Localized.  The message that will be sent to the mobile, if the specified resource is missing.

			Synthax for a COMPLEXE craft item.  A complexe item is an item that need either more than
			only one skill, or more than only one resource.

			Coming soon....
			*/

            int index = -1;

            #region Components

            index = AddCraft(
                typeof(StackofIngots), "Components", "Stack of Ingots", 105.0, 120.0, typeof(IronIngot), "iron ingot",
                1000, 1044037);
            index = AddCraft(
                typeof(StackofLogs), "Components", "Stack of Logs", 105.0, 120.0, typeof(Log), "log", 1000,
                "You do not have sufficient lumber to make that.");
            AddSkill(index, SkillName.Carpentry, 105.0, 120.0);

            #endregion

            #region Decorations

            index = AddCraft(
                typeof(SuitOfSilverArmorDeed),
                "Decorations",
                "Suit of Silver Armor",
                110.0,
                125.0,
                typeof(AncientArmorNew),
                "a piece of ancient armor",
                10,
                "You do not have sufficient materials to make that.");
            AddRes(index, typeof(StackofIngots), "Stack of Ingots", 20, 1044253);
            AddSkill(index, SkillName.Tinkering, 105.0, 120.0);
            AddRecipe(index, 1);

            index = AddCraft(
                typeof(SuitOfGoldArmorDeed),
                "Decorations",
                "Suit of Gold Armor",
                110.0,
                125.0,
                typeof(AncientArmorNew),
                "a piece of ancient armor",
                10,
                1044037);
            AddRes(index, typeof(StackofIngots), "Stack of Ingots", 20, 1044253);
            AddSkill(index, SkillName.Tinkering, 105.0, 120.0);
            AddRecipe(index, 2);

            index = AddCraft(
                typeof(SingingCrystalBall),
                "Decorations",
                "Singing Crystal Ball",
                110.0,
                125.0,
                typeof(CrackedCrystalBall),
                "a cracked crystal ball",
                1,
                1044037);
            AddRes(index, typeof(Sand), "Sand", 100, 1044253);
            AddRes(index, typeof(SeersPowder), "Seer's Powder", 50, 1044253);
            AddSkill(index, SkillName.Tinkering, 105.0, 120.0);
            AddRecipe(index, 3);

            #endregion

            #region UsableItems

            index = AddCraft(
                typeof(NewFireableCannonDeed),
                "Usable Items",
                "Cannon",
                115.0,
                125.0,
                typeof(CannonFuse),
                "a cannon fuse",
                1,
                "You do not have sufficient materials to make that.");
            AddRes(index, typeof(StackofLogs), "Stack of Logs", 10, 1044253);
            AddRes(index, typeof(MuzzlePacker), "Muzzle Packer", 1, 1044253);
            AddRes(index, typeof(StackofIngots), "Stack of Ingots", 10, 1044253);
            AddRecipe(index, 4);

            index = AddCraft(
                typeof(ArchwayAddonDeed),
                "Usable Items",
                "a dimensional archway",
                115.0,
                125.0,
                typeof(EnchantedMarble),
                "Enchanted Marble",
                3,
                "You do not have sufficient materials to make that.");
            AddRecipe(index, 5);

            #endregion

            MarkOption = true;
            CanEnhance = false;
        }
    }
}