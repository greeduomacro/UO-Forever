#region References
using System.Collections.Generic;
using System.Linq;

using Server.Targeting;
#endregion

namespace Server.Items
{
	public class ArcaneGem : Item
	{
		public override string DefaultName { get { return "arcane gem"; } }

		[Constructable]
		public ArcaneGem()
			: base(0x1EA7)
		{
			Stackable = false;
			Weight = 1.0;
		}

		public ArcaneGem(Serial serial)
			: base(serial)
		{ }

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (EraML)
			{
				Stackable = true;
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
			else
			{
				from.BeginTarget(2, false, TargetFlags.None, OnTarget);
				from.SendMessage("What do you wish to use the gem on?");
			}
		}

		public int GetChargesFor(Mobile m)
		{
			var v = (int)(m.Skills[SkillName.Tailoring].Value / 5);

			if (v < 16)
			{
				return 16;
			}

			if (v > 24)
			{
				return 24;
			}

			return v;
		}

		public const int DefaultArcaneHue = 2117;

		public void OnTarget(Mobile from, object obj)
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}

			if (obj is IArcaneEquip && obj is Item)
			{
				var item = (Item)obj;
				var resource = CraftResource.None;

				if (item is BaseClothing)
				{
					resource = ((BaseClothing)item).Resource;
				}
				else if (item is BaseArmor)
				{
					resource = ((BaseArmor)item).Resource;
				}
				else if (item is BaseWeapon) // Sanity, weapons cannot recieve gems...
				{
					resource = ((BaseWeapon)item).Resource;
				}

				var eq = (IArcaneEquip)obj;

				if (!item.IsChildOf(from.Backpack))
				{
					from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
					return;
				}

				if (item.LootType == LootType.Blessed)
				{
					from.SendMessage("You can only use this on exceptionally crafted robes, thigh boots, cloaks, or leather gloves.");
					return;
				}

				if (resource != CraftResource.None && resource != CraftResource.RegularLeather)
				{
					from.SendLocalizedMessage(1049690); // Arcane gems can not be used on that type of leather.
					return;
				}

				int charges = GetChargesFor(from);

				if (eq.IsArcane)
				{
					if (eq.CurArcaneCharges >= eq.MaxArcaneCharges)
					{
						from.SendMessage("That item is already fully charged.");
					}
					else
					{
						if (eq.CurArcaneCharges <= 0)
						{
							item.Hue = DefaultArcaneHue;
						}

						if ((eq.CurArcaneCharges + charges) > eq.MaxArcaneCharges)
						{
							eq.CurArcaneCharges = eq.MaxArcaneCharges;
						}
						else
						{
							eq.CurArcaneCharges += charges;
						}

						from.SendMessage("You recharge the item.");
						if (Amount <= 1)
						{
							Delete();
						}
						else
						{
							Amount--;
						}
					}
				}
				else if (from.Skills[SkillName.Tailoring].Value >= 80.0)
				{
					bool isExceptional = false;

					if (item is BaseClothing)
					{
						isExceptional = (((BaseClothing)item).Quality == ClothingQuality.Exceptional);
					}
					else if (item is BaseArmor)
					{
						isExceptional = (((BaseArmor)item).Quality == ArmorQuality.Exceptional);
					}
					else if (item is BaseWeapon)
					{
						isExceptional = (((BaseWeapon)item).Quality == WeaponQuality.Exceptional);
					}

					if (isExceptional)
					{
						if (item is BaseClothing)
						{
							((BaseClothing)item).Quality = ClothingQuality.Regular;
							((BaseClothing)item).Crafter = from;
						}
						else if (item is BaseArmor)
						{
							((BaseArmor)item).Quality = ArmorQuality.Regular;
							((BaseArmor)item).Crafter = from;
						}
						else //if (item is BaseWeapon) // Sanity, weapons cannot recieve gems...
						{
							((BaseWeapon)item).Quality = WeaponQuality.Regular;
							((BaseWeapon)item).Crafter = from;
						}

						eq.CurArcaneCharges = eq.MaxArcaneCharges = charges;

						item.Hue = DefaultArcaneHue;

						from.SendMessage("You enhance the item with your gem.");

						if (Amount <= 1)
						{
							Delete();
						}
						else
						{
							Amount--;
						}
					}
					else
					{
						from.SendMessage("Only exceptional items can be enhanced with the gem.");
					}
				}
				else
				{
					from.SendMessage("You do not have enough skill in tailoring to enhance the item.");
				}
			}
			else
			{
				from.SendMessage("You can only use this on exceptionally crafted robes, thigh boots, cloaks, or leather gloves.");
			}
		}

		public static bool ConsumeCharges(Mobile from, int amount)
		{
			List<Item> items = from.Items;
			int avail = items.OfType<IArcaneEquip>().Where(eq => eq.IsArcane).Sum(eq => eq.CurArcaneCharges);

			if (avail < amount)
			{
				return false;
			}

			foreach (IArcaneEquip eq in items.OfType<IArcaneEquip>().Where(eq => eq.IsArcane))
			{
				if (eq.CurArcaneCharges > amount)
				{
					eq.CurArcaneCharges -= amount;
					break;
				}

				amount -= eq.CurArcaneCharges;
				eq.CurArcaneCharges = 0;
			}

			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}