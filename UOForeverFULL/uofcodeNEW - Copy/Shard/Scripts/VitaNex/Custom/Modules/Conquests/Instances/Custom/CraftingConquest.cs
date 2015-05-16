#region References
using System;

using Server.Engines.Craft;
using Server.Items;

using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
	public class CraftingConquest : Conquest
	{
		public override string DefCategory { get { return "Crafting"; } }

		public virtual Type DefItemType { get { return null; } }
		public virtual bool DefItemChildren { get { return true; } }
		public virtual bool DefItemChangeReset { get { return false; } }

		public virtual bool DefExceptional { get { return false; } }
		public virtual bool DefSlayer { get { return false; } }

		public virtual bool DefIsCloth { get { return false; } }

		public virtual CraftResource DefResource { get { return CraftResource.None; } }
		public virtual Type DefCraftSystem { get { return null; } }

		[CommandProperty(Conquests.Access)]
		public ItemTypeSelectProperty ItemType { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ItemChildren { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ItemChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool IsExceptional { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool IsSlayer { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool IsCloth { get; set; }

		[CommandProperty(Conquests.Access)]
		public CraftResource Resource { get; set; }

		[CommandProperty(Conquests.Access)]
		public TypeSelectProperty<CraftSystem> CraftSystem { get; set; }

		public CraftingConquest()
		{ }

		public CraftingConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			ItemType = DefItemType;
			ItemChildren = DefItemChildren;
			ItemChangeReset = DefItemChangeReset;

			IsExceptional = DefExceptional;
			IsSlayer = DefSlayer;
			IsCloth = DefIsCloth;

			Resource = DefResource;

			CraftSystem = DefCraftSystem;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			// Handle multi args.
			if (args is object[])
			{
				var o = (object[])args;

				try
				{
					var item = (Item)o[0];
					var quality = (int)o[1];
					var slayer = (bool)o[2];
					var res = (CraftResource)o[3];
					var craft = (CraftSystem)o[4];

					return GetProgress(state, item, quality, slayer, res, craft);
				}
				catch
				{
					return 0;
				}
			}

			return 0;
		}

		protected virtual int GetProgress(
			ConquestState state, Item item, int quality, bool slayer, CraftResource res, CraftSystem craft)
		{
            if (state.User == null)
                return 0;

			if (item == null || craft == null)
			{
				return 0;
			}

			if (ItemType.IsNotNull && !item.TypeEquals(ItemType, ItemChildren))
			{
				if (ItemChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			if (IsExceptional && quality == 0)
			{
				return 0;
			}

			if (IsSlayer && !slayer)
			{
				return 0;
			}

			if (IsCloth && !(item is BaseClothing))
			{
				return 0;
			}

			if (Resource != CraftResource.None && res != Resource)
			{
				return 0;
			}

			if (CraftSystem.IsNotNull && !craft.TypeEquals(CraftSystem, false))
			{
				return 0;
			}

            // This function is only called after a successful completion so amount will
            // not be zero except for the case of a potion that was created and auto-consolidated
            // into a keg by the oncraft handler in BasePotion
            if(item.Amount <= 0)
            {
                return 1;
            }

            return item.Amount;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(2);

			switch (version)
			{
				case 2:
					writer.Write(IsCloth);
					goto case 1;
				case 1:
					{
						writer.WriteType(CraftSystem);
						writer.Write((int)Resource);
						writer.Write(IsSlayer);
					}
					goto case 0;
				case 0:
					{
						writer.WriteType(ItemType);
						writer.Write(ItemChildren);
						writer.Write(ItemChangeReset);
						writer.Write(IsExceptional);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 2:
					IsCloth = reader.ReadBool();
					goto case 1;
				case 1:
					{
						CraftSystem = reader.ReadType();
						Resource = (CraftResource)reader.ReadInt();
						IsSlayer = reader.ReadBool();
					}
					goto case 0;
				case 0:
					{
						ItemType = reader.ReadType();
						ItemChildren = reader.ReadBool();
						ItemChangeReset = reader.ReadBool();
						IsExceptional = reader.ReadBool();
					}
					break;
			}
		}
	}
}