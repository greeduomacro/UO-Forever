#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
#endregion

namespace VitaNex.Modules.UOFLegends
{
	public abstract class ItemLegendState<TItem> : LegendState<TItem>
		where TItem : Item
	{
		protected override void OnCompile(TItem o, IDictionary<string, SimpleType> data)
		{
			if (o == null || o.Deleted)
			{
				data.Clear();
				return;
			}

			data.Add("serial", o.Serial.Value);
			data.Add("typeof", o.GetType().Name);
			data.Add("label", o.LabelNumber);
			data.Add("name", o.ResolveName());

			data.Add("assoc", o.Association);

			data.Add("layer", (int)o.Layer);
			data.Add("layername", o.Layer.ToString());

			data.Add("itemid", o.ItemID);
			data.Add("hue", o.Hue);

			data.Add("amount", o.Amount);
			data.Add("weight", o.Weight);
			data.Add("totalweight", o.TotalWeight);

			data.Add("stackable", o.Stackable);
			data.Add("visible", o.Visible);
			data.Add("movable", o.Movable);
			data.Add("dyable", o.Dyable);
			data.Add("insured", o.Insured);
			data.Add("locked", o.IsLockedDown);
			data.Add("secure", o.IsSecure);
			data.Add("breakable", o.Breakable);
			data.Add("notransfer", o.Nontransferable);

			data.Add("direction", o.Direction.ToString());
			
			data.Add("loottype", o.LootType.ToString());
			data.Add("blessedfor", o.BlessedFor != null ? o.BlessedFor.Serial.Value : -1);

			data.Add("heldby", o.HeldBy != null ? o.HeldBy.Serial.Value : -1);
			data.Add("rootparent", o.RootParentEntity != null ? o.RootParentEntity.Serial.Value : -1);
			data.Add("parent", o.ParentEntity != null ? o.ParentEntity.Serial.Value : -1);
			data.Add("children", JoinData(o.Items.Where(c => c != null).Select(c => c.Serial.Value)));

			data.Add("location", JoinData(o.X, o.Y, o.Z));
			
			data.Add("expansion", o.Expansion.ToString());
			data.Add("map", o.Map != null ? o.Map.MapIndex : -1);
		}
	}

	public sealed class ItemLegendState : ItemLegendState<Item>
	{
		public override string TableName { get { return "items"; } }
	}
}