#region References
using System;

using VitaNex;
using VitaNex.Text;
#endregion

namespace Server.Engines.Conquests
{
    public abstract class ConquestStateContainer
    {
        public ConquestState State { get; private set; }

        public ConquestStateContainer(ConquestState s)
        {
            State = s;
        }
    }

    public abstract class RecursiveConquest<TArgs> : Conquest where TArgs : ConquestStateContainer
	{
		public override string DefCategory { get { return "Conquests"; } }

		public override int DefProgressMax { get { return base.DefProgressMax * 10; } }

		public virtual Type DefConquestType { get { return null; } }
		public virtual bool DefConquestChildren { get { return true; } }
		public virtual bool DefConquestChangeReset { get { return false; } }

		public virtual string DefNameReq { get { return null; } }
		public virtual bool DefNameChangeReset { get { return false; } }
		public virtual StringSearchFlags DefNameSearch { get { return StringSearchFlags.Contains; } }
		public virtual bool DefNameIgnoreCaps { get { return true; } }

		public virtual string DefCategoryReq { get { return null; } }
		public virtual bool DefCategoryChangeReset { get { return false; } }
		public virtual StringSearchFlags DefCategorySearch { get { return StringSearchFlags.Contains; } }
		public virtual bool DefCategoryIgnoreCaps { get { return true; } }

		[CommandProperty(Conquests.Access)]
		public TypeSelectProperty<Conquest> ConquestType { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ConquestChildren { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ConquestChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public string NameReq { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool NameChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public StringSearchFlags NameSearch { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool NameIgnoreCaps { get; set; }

		[CommandProperty(Conquests.Access)]
		public string CategoryReq { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool CategoryChangeReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public StringSearchFlags CategorySearch { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool CategoryIgnoreCaps { get; set; }

		public RecursiveConquest()
		{ }

		public RecursiveConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			ConquestType = DefConquestType;
			ConquestChildren = DefConquestChildren;
			ConquestChangeReset = DefConquestChangeReset;

			NameReq = DefNameReq;
			NameChangeReset = DefNameChangeReset;
			NameSearch = DefNameSearch;
			NameIgnoreCaps = DefNameIgnoreCaps;

			CategoryReq = DefCategoryReq;
			CategoryChangeReset = DefCategoryChangeReset;
			CategorySearch = DefCategorySearch;
			CategoryIgnoreCaps = DefCategoryIgnoreCaps;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as TArgs);
		}

		protected virtual int GetProgress(ConquestState state, TArgs args)
		{
			if (args == null || args.State == null || args.State.Conquest == null || args.State.Conquest == this)
			{
				return 0;
			}

            if (state.User == null)
                return 0;

			if (ConquestType.IsNotNull && !args.State.Conquest.TypeEquals(ConquestType, ConquestChildren))
			{
				if (ConquestChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			if (!String.IsNullOrWhiteSpace(NameReq) && !NameSearch.Execute(args.State.Conquest.Name, NameReq, NameIgnoreCaps))
			{
				if (NameChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			if (!String.IsNullOrWhiteSpace(CategoryReq) &&
				!CategorySearch.Execute(args.State.Conquest.Category, CategoryReq, CategoryIgnoreCaps))
			{
				if (CategoryChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			return 1;
		}

		protected virtual bool Include(ConquestState s)
		{
			if (s == null || s.Conquest == this)
			{
				return false;
			}

			if (ConquestType.IsNotNull && !s.Conquest.TypeEquals(ConquestType, ConquestChildren))
			{
				return false;
			}

			if (!String.IsNullOrWhiteSpace(NameReq) && !NameSearch.Execute(s.Conquest.Name, NameReq, NameIgnoreCaps))
			{
				return false;
			}

			if (!String.IsNullOrWhiteSpace(CategoryReq) &&
				!CategorySearch.Execute(s.Conquest.Category, CategoryReq, CategoryIgnoreCaps))
			{
				return false;
			}

			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					{
						writer.WriteFlag(NameSearch);
						writer.Write(NameIgnoreCaps);

						writer.WriteFlag(CategorySearch);
						writer.Write(CategoryIgnoreCaps);
					}
					goto case 0;
				case 0:
					{
						writer.WriteType(ConquestType);
						writer.Write(ConquestChildren);
						writer.Write(ConquestChangeReset);

						writer.Write(NameReq);
						writer.Write(CategoryReq);

						writer.Write(NameChangeReset);
						writer.Write(CategoryChangeReset);
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
				case 1:
					{
						NameSearch = reader.ReadFlag<StringSearchFlags>();
						NameIgnoreCaps = reader.ReadBool();

						CategorySearch = reader.ReadFlag<StringSearchFlags>();
						CategoryIgnoreCaps = reader.ReadBool();
					}
					goto case 0;
				case 0:
					{
						ConquestType = reader.ReadType();
						ConquestChildren = reader.ReadBool();
						ConquestChangeReset = reader.ReadBool();

						NameReq = reader.ReadString();
						CategoryReq = reader.ReadString();

						NameChangeReset = reader.ReadBool();
						CategoryChangeReset = reader.ReadBool();
					}
					break;
			}

			if (version > 0)
			{
				return;
			}

			NameSearch = DefNameSearch;
			NameIgnoreCaps = DefNameIgnoreCaps;

			CategorySearch = DefCategorySearch;
			CategoryIgnoreCaps = DefCategoryIgnoreCaps;
		}
	}
}