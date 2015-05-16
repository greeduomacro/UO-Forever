#region References
using System;

using Server.Engines.Craft;
using Server.Network;
#endregion

namespace Server.Items
{
	public enum ToolQuality
	{
		Low,
		Regular,
		Exceptional
	}

	public interface IBaseTool
	{
		void Delete();
		CraftSystem CraftSystem { get; }
		bool Deleted { get; }
		Mobile Crafter { get; set; }
		int UsesRemaining { get; set; }
		ToolQuality ToolQuality { get; set; }
	}

	public abstract class BaseTool : Item, IUsesRemaining, ICraftable, IBaseTool
	{
		public static bool CheckAccessible(IBaseTool tool, Mobile m)
		{
			return CheckAccessible(tool as Item, m);
		}

		public static bool CheckAccessible(Item tool, Mobile m)
		{
			return tool != null && !tool.Deleted && tool.RootParent == m;
		}

		public static bool CheckTool(IBaseTool tool, Mobile m)
		{
			return CheckTool(tool as Item, m);
		}

		public static bool CheckTool(Item tool, Mobile m)
		{
			if (tool == null || tool.Deleted)
			{
				return false;
			}

			Item check = m.FindItemOnLayer(Layer.OneHanded);

			if (check is BaseTool && (check != tool || (check is IIdentifiable && !((IIdentifiable)check).Identified)))
			{
				return false;
			}

			check = m.FindItemOnLayer(Layer.TwoHanded);

			if (check is BaseTool && (check != tool || (check is IIdentifiable && !((IIdentifiable)check).Identified)))
			{
				return false;
			}

			return true;
		}

		private Mobile _Crafter;
		private ToolQuality _Quality;
		private int _UsesRemaining;
		private bool _ShowUsesRemaining;

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Crafter
		{
			get { return _Crafter; }
			set
			{
				_Crafter = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public ToolQuality ToolQuality
		{
			get { return _Quality; }
			set
			{
				UnscaleUses();
				_Quality = value;
				InvalidateProperties();
				ScaleUses();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int UsesRemaining
		{
			get { return _UsesRemaining; }
			set
			{
				_UsesRemaining = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ShowUsesRemaining
		{
			get { return _ShowUsesRemaining; }
			set
			{
				_ShowUsesRemaining = value;
				InvalidateProperties();
			}
		}

		public abstract CraftSystem CraftSystem { get; }

		public BaseTool(int itemID)
			: this(Utility.RandomMinMax(25, 75), itemID)
		{ }

		public BaseTool(int uses, int itemID)
			: base(itemID)
		{
			_ShowUsesRemaining = true;
			_UsesRemaining = uses;
			_Quality = ToolQuality.Regular;
		}

		public BaseTool(Serial serial)
			: base(serial)
		{ }

		public void ScaleUses()
		{
			_UsesRemaining = (_UsesRemaining * GetUsesScalar()) / 100;
			InvalidateProperties();
		}

		public void UnscaleUses()
		{
			_UsesRemaining = (_UsesRemaining * 100) / GetUsesScalar();
		}

		public int GetUsesScalar()
		{
			return _Quality == ToolQuality.Exceptional ? 200 : 100;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			// Makers mark not displayed on OSI
			if (_Crafter != null)
			{
				list.Add(1050043, _Crafter.RawName); // crafted by ~1_NAME~
			}

			if (_Quality == ToolQuality.Exceptional)
			{
				list.Add(1060636); // exceptional
			}

			if (ShowUsesRemaining)
			{
				list.Add(1060584, _UsesRemaining.ToString("#,0")); // uses remaining: ~1_val~
			}
		}

		public virtual void DisplayDurabilityTo(Mobile m)
		{
			LabelToAffix(m, 1017323, AffixType.Append, ": " + _UsesRemaining.ToString("#,0")); // Durability
		}

		public override void OnSingleClick(Mobile m)
		{
			base.OnSingleClick(m);

			if (ShowUsesRemaining)
			{
				DisplayDurabilityTo(m);
			}
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!this.CheckDoubleClick(m, true, false, 2, true))
			{
				return;
			}

			CraftSystem system = CraftSystem;

			if (system == null)
			{
				return;
			}

			int num = system.CanCraft(m, this, null);

			// Blacksmithing shows the gump regardless of proximity of an anvil and forge after SE
			if (num <= 0 || (num == 1044267 && m.EraSE))
			{
				m.SendGump(new CraftGump(m, system, this, null));
				return;
			}
			
			m.SendLocalizedMessage(num);
		}

		public int OnCraft(
			int quality,
			bool makersMark,
			Mobile from,
			CraftSystem craftSystem,
			Type typeRes,
			IBaseTool tool,
			CraftItem craftItem,
			int resHue)
		{
			ToolQuality = (ToolQuality)quality;

			if (makersMark)
			{
				Crafter = from;
			}

			return quality;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(2);

			switch (version)
			{
				case 2:
					writer.Write(_ShowUsesRemaining);
					goto case 1;
				case 1:
					{
						writer.Write(_Crafter);

						if (version < 2)
						{
							writer.Write((int)_Quality);
						}
						else
						{
							writer.WriteFlag(_Quality);
						}
					}
					goto case 0;
				case 0:
					writer.Write(_UsesRemaining);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
					_ShowUsesRemaining = reader.ReadBool();
					goto case 1;
				case 1:
					{
						_Crafter = reader.ReadMobile();
						_Quality = version < 2 ? (ToolQuality)reader.ReadInt() : reader.ReadFlag<ToolQuality>();
					}
					goto case 0;
				case 0:
					_UsesRemaining = reader.ReadInt();
					break;
			}

			if (version < 2)
			{
				_ShowUsesRemaining = true;
			}
		}
	}
}