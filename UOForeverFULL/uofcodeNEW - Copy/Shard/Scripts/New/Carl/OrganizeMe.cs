#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles;

#endregion

namespace Server.Commands
{
    public class OrganizeMeCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("OrganizeMe", AccessLevel.Player, OrganizeMe_OnCommand);
        }

        //This command will not move spellbooks, runebooks, blessed items.
        [Usage("OrganizeMe")]
        [Description("Organize the items in your backpack into pouches.")]
        private static void OrganizeMe_OnCommand(CommandEventArgs arg)
        {

            Console.WriteLine("---------------- OrganizeMe -------------------");

            OrganizePouch weaponPouch = null;
            OrganizePouch armorPouch = null;
            OrganizePouch clothingPouch = null;
            OrganizePouch jewelPouch = null;
            OrganizePouch potionPouch = null;
            OrganizePouch currencyPouch = null;
            OrganizePouch resourcePouch = null;
            OrganizePouch toolPouch = null;
            OrganizePouch regsPouch = null;
            OrganizePouch miscPouch = null;

            Mobile from = arg.Mobile;
            var bp = from.Backpack as Backpack;

            if (@from == null || bp == null)
            {
                return;
            }

            if (bp.TotalWeight >= bp.MaxWeight && from.AccessLevel < AccessLevel.GameMaster)
            {
                if (from is PlayerMobile && from.NetState != null)
                {
                    from.SendMessage("You have too much weight in your pack to use the organizer.");
                }
                return;
            }

            if (bp.TotalItems >= (bp.MaxItems - 10)  && from.AccessLevel < AccessLevel.GameMaster)
            {
                if (from is PlayerMobile && from.NetState != null)
                {
                    from.SendMessage("You do not have enough room in your pack to use the organizer.");
                }
                return;
            }

            var backpackitems = new List<Item>(bp.Items);
            var subcontaineritems = new List<Item>();

            foreach (var item in backpackitems.OfType<BaseContainer>())
            {
                var lockable = item as LockableContainer;
                if (lockable != null)
                {
                    if (lockable.CheckLocked(from))
                    {
                        continue;
                    }
                }

                var trapped = item as TrapableContainer;
                if (trapped != null)
                {
                    if (trapped.TrapType != TrapType.None)
                    {
                        continue;
                    }
                }

                // Skip the pouches that are already created
                if (item is OrganizePouch)
                {    
                    if (item.Name == "Weapons")
                    {
                        weaponPouch = item as OrganizePouch;
                    }
                    if (item.Name == "Armor")
                    {
                        armorPouch = item as OrganizePouch;
                    }
                    if (item.Name == "Clothing")
                    {
                        clothingPouch = item as OrganizePouch;
                    }
                    if (item.Name == "Jewelry")
                    {
                        jewelPouch = item as OrganizePouch;
                    }
                    if (item.Name == "Potions")
                    {
                        potionPouch = item as OrganizePouch;
                    }
                    if (item.Name == "Currency")
                    {
                        currencyPouch = item as OrganizePouch;
                    }
                    if (item.Name == "Resources")
                    {
                        resourcePouch = item as OrganizePouch;
                    }
                    if (item.Name == "Tools")
                    {
                        toolPouch = item as OrganizePouch;
                    }
                    if (item.Name == "Reagents")
                    {
                        regsPouch = item as OrganizePouch;
                    }
                    if (item.Name == "Misc")
                    {
                        miscPouch = item as OrganizePouch;
                    }

                    // Skip all the items in the pouches since they should already be organized
                    continue;
                }

                // Add all the subcontainer items, but dont go all the way to comeplete depth
                subcontaineritems.AddRange(item.Items);
            }

            backpackitems.AddRange(subcontaineritems);

            if (weaponPouch == null)
            {
                weaponPouch = new OrganizePouch { Name = "Weapons", Hue = Utility.RandomMetalHue() };
            }
            if (armorPouch == null)
            {
                armorPouch = new OrganizePouch { Name = "Armor", Hue = Utility.RandomMetalHue() };
            }
            if (clothingPouch == null)
            {
                clothingPouch = new OrganizePouch { Name = "Clothing", Hue = Utility.RandomBrightHue() };
            }
            if (jewelPouch == null)
            {
                jewelPouch = new OrganizePouch { Name = "Jewelry", Hue = Utility.RandomPinkHue() };
            }
            if (potionPouch == null)
            {
                potionPouch = new OrganizePouch {Name = "Potions", Hue = Utility.RandomOrangeHue()};
            }
            if (currencyPouch == null)
            {
                currencyPouch = new OrganizePouch {Name = "Currency", Hue = Utility.RandomYellowHue()};
            }
            if (resourcePouch == null)
            {
                resourcePouch = new OrganizePouch {Name = "Resources", Hue = Utility.RandomNondyedHue()};
            }
            if (toolPouch == null)
            {
                toolPouch = new OrganizePouch {Name = "Tools", Hue = Utility.RandomMetalHue()};
            }
            if (regsPouch == null)
            {
                regsPouch = new OrganizePouch {Name = "Reagents", Hue = Utility.RandomGreenHue()};
            }
            if (miscPouch == null)
            {
                miscPouch = new OrganizePouch { Name = "Misc" };
            }
            var pouches = new List<OrganizePouch>
            {
                weaponPouch,
                armorPouch,
                clothingPouch,
                jewelPouch,
                potionPouch,
                currencyPouch,
                resourcePouch,
                toolPouch,
                regsPouch,
                miscPouch
            };

            foreach (
                Item item in
                    backpackitems.Where(
                        item =>
                            item.LootType != LootType.Blessed && 
                            !(item is Runebook) &&
                            !(item is Spellbook) && 
                            item.Movable && 
                            item.LootType != LootType.Newbied))
            {
                // Lets not add the pouches to themselves
                if (item is OrganizePouch)
                {
                    continue;
                }

                if (item is BaseWeapon)
                {
                    weaponPouch.TryDropItem(from, item, false);
                }
                else if (item is BaseArmor)
                {
                    armorPouch.TryDropItem(from, item, false);
                }
                else if (item is BaseClothing)
                {
                    clothingPouch.TryDropItem(from, item, false);
                }
                else if (item is BaseJewel)
                {
                    jewelPouch.TryDropItem(from, item, false);
                }
                else if (item is BasePotion)
                {
                    potionPouch.TryDropItem(from, item, false);
                }
                else if (item is Gold || item is Silver)
                {
                    currencyPouch.TryDropItem(from, item, false);
                }
                else if (item is BaseIngot || item is BaseOre || item is Feather || item is BaseBoard || item is Log ||
                         item is BaseLeather ||
                         item is Sand || item is BaseGranite)
                {
                    resourcePouch.TryDropItem(from, item, false);
                }
                else if (item is BaseTool)
                {
                    toolPouch.TryDropItem(from, item, false);
                }
                else if (item is BaseReagent)
                {
                    regsPouch.TryDropItem(from, item, false);
                }
                else
                {
                    miscPouch.TryDropItem(from, item, false);
                }
            }

            var x = 45;

            foreach (var pouch in pouches)
            {
                if (pouch.TotalItems <= 0)
                { 
                    continue;
                }
                
                // AddToBackpack doesnt do anything if the item is already in the backpack
                // calls DropItem internally
                
                if (!from.Backpack.Items.Contains(pouch))
                {
                    from.AddToBackpack(pouch);
                }

                pouch.X = x;
                pouch.Y = 65;

                x += 10;
            }
        }
    }
}