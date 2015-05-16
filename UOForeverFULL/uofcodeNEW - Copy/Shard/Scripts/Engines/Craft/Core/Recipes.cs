#region References
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;
#endregion

namespace Server.Engines.Craft
{
	public class Recipe
	{
		public static void Initialize()
		{
			CommandSystem.Register("LearnAllRecipes", AccessLevel.GameMaster, LearnAllRecipes_OnCommand);
			CommandSystem.Register("ForgetAllRecipes", AccessLevel.GameMaster, ForgetAllRecipes_OnCommand);
		}

		[Usage("LearnAllRecipes")]
		[Description("Teaches a player all available recipes.")]
		private static void LearnAllRecipes_OnCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;
			m.SendMessage("Target a player to teach them all of the recipies.");

			m.BeginTarget(
				-1,
				false,
				TargetFlags.None,
				(from, targeted) =>
				{
					if (targeted is PlayerMobile)
					{
						foreach (var kvp in Recipes)
						{
							((PlayerMobile)targeted).AcquireRecipe(kvp.Key);
						}

						m.SendMessage("You teach them all of the recipies.");
					}
					else
					{
						m.SendMessage("That is not a player!");
					}
				});
		}

		[Usage("ForgetAllRecipes")]
		[Description("Makes a player forget all the recipies they've learned.")]
		private static void ForgetAllRecipes_OnCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;
			m.SendMessage("Target a player to have them forget all of the recipies they've learned.");

			m.BeginTarget(
				-1,
				false,
				TargetFlags.None,
				(from, targeted) =>
				{
					if (targeted is PlayerMobile)
					{
						((PlayerMobile)targeted).ResetRecipes();

						m.SendMessage("They forget all their recipies.");
					}
					else
					{
						m.SendMessage("That is not a player!");
					}
				});
		}

		public static Dictionary<int, Recipe> Recipes { get; private set; }
		public static int LargestRecipeID { get; private set; }

		static Recipe()
		{
			Recipes = new Dictionary<int, Recipe>();
		}

		public int ID { get; private set; }

		public CraftSystem CraftSystem { get; set; }
		public CraftItem CraftItem { get; set; }

		private TextDefinition m_TD;

		public TextDefinition TextDefinition { get { return m_TD ?? (m_TD = new TextDefinition(CraftItem.NameNumber, CraftItem.NameString)); } }

		public Recipe(int id, CraftSystem system, CraftItem item)
		{
			ID = id;

			CraftSystem = system;
			CraftItem = item;

			if (Recipes.ContainsKey(id))
			{
				throw new Exception("Attempting to create recipe with pre-existing ID.");
			}

			Recipes.Add(id, this);

			LargestRecipeID = Math.Max(id, LargestRecipeID);
		}

        public static Recipe GetRandomRecipe()
        {
            Random rand = new Random();
            var recipe = Recipes.ElementAt(rand.Next(0, Recipes.Count)).Value;
            return recipe;
        }
	}
}