#region References
using System;

using Server.Engines.Harvest;
using Server.Items;

using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
	public class HarvestConquest : Conquest
	{
		public override string DefCategory { get { return "Harvesting"; } }

		public virtual Type DefItemType { get { return null; } }
		public virtual bool DefItemChildren { get { return true; } }
		public virtual bool DefItemChangeReset { get { return false; } }

		public virtual CraftResource DefResource { get { return CraftResource.None; } }

		public virtual Type DefHarvestSystem { get { return null; } }

		[CommandProperty(Conquests.Access)]
		public ItemTypeSelectProperty ItemType { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ItemChildren { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ItemChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public CraftResource Resource { get; set; }
		
		[CommandProperty(Conquests.Access)]
		public TypeSelectProperty<HarvestSystem> HarvestSystem { get; set; }

		public HarvestConquest()
		{ }

		public HarvestConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			ItemType = DefItemType;
			ItemChildren = DefItemChildren;
			ItemChangeReset = DefItemChangeReset;

			Resource = DefResource;

			HarvestSystem = DefHarvestSystem;
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
					var res = (CraftResource)o[1];
					var harvest = (HarvestSystem)o[2];
					
					return GetProgress(state, item, res, harvest);
				}
				catch
				{
					return 0;
				}
			}

			return 0;
		}

		protected virtual int GetProgress(ConquestState state, Item item, CraftResource res, HarvestSystem harvest)
		{
			if (item == null || harvest == null)
			{
				return 0;
			}

            if (state.User == null)
                return 0;

			if (ItemType.IsNotNull && !item.TypeEquals(ItemType, ItemChildren))
			{
				if (ItemChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			if (Resource != CraftResource.None && res != Resource)
			{
				return 0;
			}

			if (HarvestSystem.IsNotNull && !harvest.TypeEquals(HarvestSystem, false))
			{
				return 0;
			}

			return item.Amount;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.WriteType(ItemType);
						writer.Write(ItemChildren);
						writer.Write(ItemChangeReset);

						writer.WriteFlag(Resource);

						writer.WriteType(HarvestSystem);
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
				case 0:
					{
						ItemType = reader.ReadType();
						ItemChildren = reader.ReadBool();
						ItemChangeReset = reader.ReadBool();

						Resource = reader.ReadFlag<CraftResource>();

						HarvestSystem = reader.ReadType();
					}
					break;
			}
		}
	}
}