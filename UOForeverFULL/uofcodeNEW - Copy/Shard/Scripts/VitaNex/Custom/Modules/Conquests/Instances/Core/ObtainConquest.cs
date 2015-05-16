#region References
using System;
using System.Linq;

using Server.Mobiles;

using VitaNex;
using VitaNex.Text;
#endregion

namespace Server.Engines.Conquests
{
	public class ObtainConquest : Conquest
	{
		public override string DefCategory { get { return "Collection"; } }

		public virtual Type DefObject { get { return null; } }
		public virtual bool DefObjectChildren { get { return true; } }
		public virtual bool DefObjectChangeReset { get { return false; } }

		public virtual string DefObjectNameReq { get { return null; } }
		public virtual bool DefObjectNameChangeReset { get { return false; } }
		public virtual StringSearchFlags DefObjectNameSearch { get { return StringSearchFlags.Contains; } }
		public virtual bool DefObjectNameIgnoreCaps { get { return true; } }

		public virtual int DefObjectHueReq { get { return -1; } }
		public virtual bool DefObjectHueChangeReset { get { return false; } }

		[CommandProperty(Conquests.Access)]
		public EntityTypeSelectProperty Object { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ObjectChildren { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ObjectChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public string ObjectNameReq { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ObjectNameChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public StringSearchFlags ObjectNameSearch { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ObjectNameIgnoreCaps { get; set; }
		
		[Hue, CommandProperty(Conquests.Access)]
		public int ObjectHueReq { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ObjectHueChangeReset { get; set; }

		public ObtainConquest()
		{ }

		public ObtainConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Object = DefObject;
			ObjectChildren = DefObjectChildren;
			ObjectChangeReset = DefObjectChangeReset;

			ObjectNameReq = DefObjectNameReq;
			ObjectNameChangeReset = DefObjectNameChangeReset;
			ObjectNameSearch = DefObjectNameSearch;
			ObjectNameIgnoreCaps = DefObjectNameIgnoreCaps;

			ObjectHueReq = DefObjectHueReq;
			ObjectHueChangeReset = DefObjectHueChangeReset;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as IEntity);
		}

		protected virtual int GetProgress(ConquestState state, IEntity obj)
		{
            if (state.User == null)
                return 0;

			if (obj == null)
			{
				return 0;
			}

			if (Object.IsNotNull && !obj.TypeEquals(Object, ObjectChildren))
			{
				if (ObjectChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			double amount;

			if (obj is Item)
			{
				var item = (Item)obj;

				if (!String.IsNullOrWhiteSpace(ObjectNameReq) &&
					!ObjectNameSearch.Execute(item.ResolveName(), ObjectNameReq, ObjectNameIgnoreCaps))
				{
					if (ObjectNameChangeReset)
					{
						return -state.Progress;
					}

					return 0;
				}

				if (ObjectHueReq > -1 && ObjectHueReq != item.Hue)
				{
					if (ObjectHueChangeReset)
					{
						return -state.Progress;
					}

					return 0;
				}

				// Start with the otained item itself.
				amount = ((item.RootParent == state.User || item.HeldBy == state.User) ? item.Amount : 0);

				// add all the amounts of the same item type from banks.
				amount +=
					state.User.BankBoxes.Sum(
						bank =>
						bank.FindItemsByType(Object, true)
							.Where(i => i != null && i != item && (i.RootParent == state.User || i.HeldBy == state.User))
							.Not(i => Object.IsNotNull && !i.TypeEquals(Object, ObjectChildren))
							.Not(
								i =>
								!String.IsNullOrWhiteSpace(ObjectNameReq) &&
								!ObjectNameSearch.Execute(i.ResolveName(), ObjectNameReq, ObjectNameIgnoreCaps))
							.Not(i => ObjectHueReq > -1 && ObjectHueReq != i.Hue)
							.Sum(i => (double)i.Amount));

				// add all the amounts of the same item type from pack.
				amount +=
					state.User.Backpacks.Sum(
						pack =>
						pack.FindItemsByType(Object, true)
							.Where(i => i != null && i != item && (i.RootParent == state.User || i.HeldBy == state.User))
							.Not(i => Object.IsNotNull && !i.TypeEquals(Object, ObjectChildren))
							.Not(
								i =>
								!String.IsNullOrWhiteSpace(ObjectNameReq) &&
								!ObjectNameSearch.Execute(i.ResolveName(), ObjectNameReq, ObjectNameIgnoreCaps))
							.Not(i => ObjectHueReq > -1 && ObjectHueReq != i.Hue)
							.Sum(i => (double)i.Amount));
			}
			else if (obj is BaseCreature)
			{
				var pet = (BaseCreature)obj;

				if (!String.IsNullOrWhiteSpace(ObjectNameReq) &&
					!ObjectNameSearch.Execute(pet.Name, ObjectNameReq, ObjectNameIgnoreCaps))
				{
					if (ObjectNameChangeReset)
					{
						return -state.Progress;
					}

					return 0;
				}

				if (ObjectHueReq > -1 && ObjectHueReq != pet.Hue)
				{
					if (ObjectHueChangeReset)
					{
						return -state.Progress;
					}

					return 0;
				}

				amount = (pet.GetMaster() == state.User || state.User.AllFollowers.Contains(pet)) ? 1 : 0;

				amount +=
					state.User.AllFollowers.Where(p => p != null && p != pet)
						 .Not(p => Object.IsNotNull && !p.TypeEquals(Object, ObjectChildren))
						 .Not(
							 p =>
							 !String.IsNullOrWhiteSpace(ObjectNameReq) &&
							 !ObjectNameSearch.Execute(p.Name, ObjectNameReq, ObjectNameIgnoreCaps))
						 .Not(p => ObjectHueReq > -1 && ObjectHueReq != p.Hue)
						 .Count();
			}
			else
			{
				amount = 1.0;
			}

			// progress = progress - (amount + bankAmount + packAmount)
			return Math.Max(0, (int)Math.Min(Int32.MaxValue, amount) - state.Progress);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.WriteType(Object);
						writer.Write(ObjectChildren);
						writer.Write(ObjectChangeReset);

						writer.WriteFlag(ObjectNameSearch);
						writer.Write(ObjectNameIgnoreCaps);
						writer.Write(ObjectNameReq);
						writer.Write(ObjectNameChangeReset);

						writer.Write(ObjectHueReq);
						writer.Write(ObjectHueChangeReset);
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
						Object = reader.ReadType();
						ObjectChildren = reader.ReadBool();
						ObjectChangeReset = reader.ReadBool();

						ObjectNameSearch = reader.ReadFlag<StringSearchFlags>();
						ObjectNameIgnoreCaps = reader.ReadBool();
						ObjectNameReq = reader.ReadString();
						ObjectNameChangeReset = reader.ReadBool();

						ObjectHueReq = reader.ReadInt();
						ObjectHueChangeReset = reader.ReadBool();
					}
					break;
			}
		}
	}
}