#region References
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Engines.Craft
{
	public class CraftGroupCol : CollectionBase, IEnumerable<CraftGroup>
	{
		public int Add(CraftGroup craftGroup)
		{
			return List.Add(craftGroup);
		}

		public void Remove(int index)
		{
			if (index <= Count - 1 && index >= 0)
			{
				List.RemoveAt(index);
			}
		}

		public CraftGroup GetAt(int index)
		{
			return (CraftGroup)List[index];
		}

		public int SearchFor(TextDefinition groupName)
		{
			for (int i = 0; i < List.Count; i++)
			{
				var craftGroup = (CraftGroup)List[i];

				int nameNumber = craftGroup.NameNumber;
				string nameString = craftGroup.NameString;

				if ((nameNumber != 0 && nameNumber == groupName.Number) || (nameString != null && nameString == groupName.String))
				{
					return i;
				}
			}

			return -1;
		}

		IEnumerator<CraftGroup> IEnumerable<CraftGroup>.GetEnumerator()
		{
			return List.OfType<CraftGroup>().GetEnumerator();
		}
	}
}