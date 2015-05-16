#region References
using System;
using System.Linq;

using Server.Targeting;

using VitaNex.Targets;
#endregion

namespace Server.Items
{
	public class SashLayerDeed : Item
	{
		private static readonly Type[] _LayerableTypes = new[] {typeof(BodySash), typeof(JinBaori)};

		public static Layer TargetLayer = Layer.Earrings;

		private ItemSelectTarget<Item> _Target;

		public override bool DisplayLootType { get { return false; } }

		[Constructable]
		public SashLayerDeed()
			: base(0x14F0)
		{
			Name = "a sash layering deed";
			Weight = 1.0;
			Hue = 1266;
			LootType = LootType.Blessed;
		}

		public SashLayerDeed(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile m)
		{
			if (!this.CheckDoubleClick(m, true, false, 2, true))
			{
				return;
			}

			if (_Target != null)
			{
				_Target.Cancel(_Target.User, TargetCancelType.Overridden);
				_Target = null;
			}

			m.SendMessage("Which sash would you like to layer?");
			m.Target = _Target = new ItemSelectTarget<Item>(OnTarget, OnCancel);
		}

		protected void OnTarget(Mobile m, Item item)
		{
			if (!this.CheckDoubleClick(m, true, false, 2, true) || item == null || item.Deleted || _Target == null ||
				_Target.User != m)
			{
				return;
			}

			if (!_LayerableTypes.Any(t => item.TypeEquals(t)))
			{
				m.SendMessage(34, "You can not layer that item.");
				return;
			}

			if (item.Layer == TargetLayer)
			{
				m.SendMessage(34, "That sash is already layered.");
				return;
			}

			if (item.RootParent != m)
			{
				m.SendMessage(34, "That sash must be equipped or in your pack to layer it.");
				return;
			}

			if (item.IsEquipped())
			{
				var otherItem = m.FindItemOnLayer(TargetLayer);

				if (otherItem != null)
				{
					m.SendMessage(
						34,
						"You must unequip the {0} in your {1} slot before you can layer that sash.",
						otherItem.ResolveName(m),
						TargetLayer.ToString().ToLower());
					return;
				}
			}

			item.Layer = TargetLayer;

			if (String.IsNullOrWhiteSpace(item.Name))
			{
				item.Name = "a body sash [Layered]";
			}
			else if (!item.Name.EndsWith(" [Layered]"))
			{
				item.Name += " [Layered]";
			}

			m.SendMessage(85, "You successfully layer the sash!");

			Delete();
		}

		protected void OnCancel(Mobile m)
		{
			if (_Target != null && _Target.User == m)
			{
				_Target = null;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			LootType = LootType.Blessed;
		}
	}
}