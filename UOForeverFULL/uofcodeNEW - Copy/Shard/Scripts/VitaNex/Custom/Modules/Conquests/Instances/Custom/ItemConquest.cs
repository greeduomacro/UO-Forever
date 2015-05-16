#region References
using System;

using Server.Items;

using VitaNex;
using VitaNex.Text;

#endregion

namespace Server.Engines.Conquests
{
	public class ItemConquest : Conquest
	{
		public override string DefCategory { get { return "Items"; } }

		public virtual Type DefItem { get { return null; } }
		public virtual bool DefItemChildren { get { return true; } }
		public virtual bool DefItemChangeReset { get { return false; } }

		public virtual string DefKeywordReq { get { return null; } }
		public virtual bool DefKeywordChangeReset { get { return false; } }
		public virtual StringSearchFlags DefKeywordSearch { get { return StringSearchFlags.Contains; } }
		public virtual bool DefKeywordIgnoreCaps { get { return true; } }

		[CommandProperty(Conquests.Access)]
		public ItemTypeSelectProperty Item { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ItemChildren { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ItemChangeReset { get; set; }

        [CommandProperty(Conquests.Access)]
		public string KeywordReq { get; set; }

        [CommandProperty(Conquests.Access)]
		public bool KeywordChangeReset { get; set; }

        [CommandProperty(Conquests.Access)]
		public StringSearchFlags KeywordSearch { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool KeywordIgnoreCaps { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool CheckAmount { get; set; }

		public ItemConquest()
		{ }

		public ItemConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Item = DefItem;
			ItemChildren = DefItemChildren;
			ItemChangeReset = DefItemChangeReset;

            KeywordReq = DefKeywordReq;
            KeywordChangeReset = DefKeywordChangeReset;
            KeywordSearch = DefKeywordSearch;
            KeywordIgnoreCaps = DefKeywordIgnoreCaps;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			// Handle multi args (if any).
			if (args is object[])
			{
				var o = (object[])args;

				if (o.Length == 0)
				{
					return 0;
				}

				if (o.Length < 2)
				{
					return GetProgress(state, o[0] as Item, String.Empty);
				}

				return GetProgress(state, o[0] as Item, o[1] as string);
			}

			// Handle single item argument.
			return GetProgress(state, args as Item, String.Empty);
		}

        protected virtual int GetProgress(ConquestState state, Item item, string keyword)
		{
            if (item == null)
			{
				return 0;
			}

            if (state.User == null)
                return 0;

            if (Item.IsNotNull && !item.TypeEquals(Item, ItemChildren))
			{
				if (ItemChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

	        if (!String.IsNullOrEmpty(KeywordReq) && !KeywordSearch.Execute(keyword, KeywordReq, KeywordIgnoreCaps))
	        {
		        if (KeywordChangeReset)
		        {
			        return -state.Progress;
		        }

		        return 0;
	        }

	        if (item.Amount > 1 && CheckAmount)
			{
                return item.Amount;
			}

			return 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(2);

			switch (version)
			{
                case 2:
                    {
                        writer.WriteFlag(KeywordSearch);
                        writer.Write(KeywordIgnoreCaps);
                        writer.Write(KeywordReq);
                        writer.Write(KeywordChangeReset);
                    }
			        goto case 1;
				case 1:
					{
						writer.Write(CheckAmount);
					}
					goto case 0;
				case 0:
					{
						writer.WriteType(Item);
						writer.Write(ItemChildren);
						writer.Write(ItemChangeReset);
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
                    {
                        KeywordSearch = reader.ReadFlag<StringSearchFlags>();
                        KeywordIgnoreCaps = reader.ReadBool();
                        KeywordReq = reader.ReadString();
                        KeywordChangeReset = reader.ReadBool();
                    }
                    goto case 1;
				case 1:
					{
						CheckAmount = reader.ReadBool();
					}
					goto case 0;
				case 0:
					{
						Item = reader.ReadType();
						ItemChildren = reader.ReadBool();
						ItemChangeReset = reader.ReadBool();
					}
					break;
			}
		}
	}
}