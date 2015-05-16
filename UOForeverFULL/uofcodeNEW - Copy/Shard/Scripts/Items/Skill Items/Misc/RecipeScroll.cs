#region References

using System.Linq;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Items
{
	public class RecipeScroll : Item
	{
		public override int LabelNumber { get { return 1074560; } } // recipe scroll

		private int m_RecipeID;

		[CommandProperty(AccessLevel.GameMaster)]
		public int RecipeID
		{
			get { return m_RecipeID; }
			set
			{
				m_RecipeID = value;
				InvalidateProperties();
			}
		}

		public Recipe Recipe { get { return Recipe.Recipes.ContainsKey(m_RecipeID) ? Recipe.Recipes[m_RecipeID] : null; } }

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			Recipe r = Recipe;

			if (r != null)
			{
				list.Add(1049644, r.TextDefinition.ToString()); // [~1_stuff~]
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			Recipe r = Recipe;

			if (r != null)
			{
				LabelTo(from, 1049644, r.TextDefinition.ToString()); // [~1_stuff~]
			}
		}

		public RecipeScroll(Recipe r)
			: this(r.ID)
		{ }

		[Constructable]
		public RecipeScroll(int recipeID)
			: base(0x2831)
		{
			m_RecipeID = recipeID;
		    Weight = 13;
		}

		public RecipeScroll(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile m)
		{
			if (!m.InRange(GetWorldLocation(), 2))
			{
				m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				return;
			}

			Recipe r = Recipe;

            if (r == null || !(m is PlayerMobile) || RootParentEntity != m)
			{
				return;
			}

			var pm = m as PlayerMobile;

			if (!pm.HasRecipe(r))
			{
				bool allRequiredSkills = true;
				double chance = r.CraftItem.GetSuccessChance(pm, null, r.CraftSystem, false, ref allRequiredSkills);

				if (allRequiredSkills && chance >= 0.0)
				{
					pm.SendLocalizedMessage(1073451, r.TextDefinition.ToString()); // You have learned a new recipe: ~1_RECIPE~
					pm.AcquireRecipe(r);
					Delete();
				}
				else
				{
					pm.SendLocalizedMessage(1044153); // You don't have the required skills to attempt this item.
				}
			}
			else
			{
				pm.SendLocalizedMessage(1073427); // You already know this recipe.
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(m_RecipeID);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					m_RecipeID = reader.ReadInt();
					break;
			}
		}
	}
}