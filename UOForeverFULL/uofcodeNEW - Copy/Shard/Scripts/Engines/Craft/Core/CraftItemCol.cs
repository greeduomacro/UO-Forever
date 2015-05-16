#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Engines.Craft
{
	public class CraftItemCol : CollectionBase, IEnumerable<CraftItem>
	{
		public int Add(CraftItem craftItem)
		{
			return List.Add(craftItem);
		}

		public void Remove(int index)
		{
			if (index >= 0 && index < Count)
			{
				List.RemoveAt(index);
			}
		}

		public CraftItem GetAt(int index)
		{
			return (CraftItem)List[index];
		}

		public CraftItem SearchForSubclass(Type type)
		{
			return
				List.OfType<CraftItem>()
					.FirstOrDefault(craftItem => craftItem.ItemType == type || type.IsSubclassOf(craftItem.ItemType));
		}

		public CraftItem SearchFor(Type type)
		{
			return List.OfType<CraftItem>().FirstOrDefault(craftItem => craftItem.ItemType == type);
		}

		IEnumerator<CraftItem> IEnumerable<CraftItem>.GetEnumerator()
		{
			return List.OfType<CraftItem>().GetEnumerator();
		}
	}
}