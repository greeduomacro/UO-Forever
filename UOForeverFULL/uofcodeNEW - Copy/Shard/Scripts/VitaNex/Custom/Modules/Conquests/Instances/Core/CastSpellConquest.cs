#region References
using System;

using Server.Items;
using Server.Mobiles;
using Server.Spells;

using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
    public class CastSpellConquestContainer
    {
        private readonly Mobile m_Mobile;
        private readonly Item m_Spellbook;
        private readonly int m_SpellID;

        public Mobile Mobile { get { return m_Mobile; } }
        public Item Spellbook { get { return m_Spellbook; } }
        public int SpellID { get { return m_SpellID; } }

        public CastSpellConquestContainer(Mobile m, int spellID, Item book)
        {
            m_Mobile = m;
            m_Spellbook = book;
            m_SpellID = spellID;
        }
    }

	public class CastSpellConquest : Conquest
	{
		public override string DefCategory { get { return "Casting"; } }

		public virtual Type DefSpell { get { return null; } }
		public virtual bool DefCheckBook { get { return false; } }
		public virtual bool DefCheckRegs { get { return false; } }
		public virtual bool DefChangeSpellReset { get { return false; } }

		[CommandProperty(Conquests.Access)]
		public SpellTypeSelectProperty Spell { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool CheckBook { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool CheckRegs { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ChangeSpellReset { get; set; }

		public CastSpellConquest()
		{ }

		public CastSpellConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Spell = DefSpell;

			CheckBook = DefCheckBook;
			CheckRegs = DefCheckRegs;

			ChangeSpellReset = DefChangeSpellReset;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as CastSpellRequestEventArgs);
		}

        protected virtual int GetProgress(ConquestState state, CastSpellConquestContainer args)
		{
            if (state.User == null)
                return 0;

			if (args == null || args.SpellID < 0 || args.Mobile is PlayerMobile && args.Mobile.Account != state.User.Account)
			{
				return 0;
			}

			if (CheckBook)
			{
				var book = args.Spellbook as Spellbook;

				if (book == null || !book.HasSpell(args.SpellID))
				{
					book = Spellbook.Find(args.Mobile, args.SpellID);
				}

				if (book == null)
				{
					return 0;
				}
			}

			if (CheckRegs)
			{
				SpellInfo info = SpellRegistry.GetSpellInfo(args.SpellID);

				if (info != null && !args.Mobile.HasItems(info.Reagents, info.Amounts))
				{
					return 0;
				}
			}

			if (Spell.IsNotNull && SpellRegistry.GetRegistryNumber(Spell) != args.SpellID)
			{
				if (ChangeSpellReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			return 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.WriteType(Spell);

						writer.Write(CheckBook);
						writer.Write(CheckRegs);

						writer.Write(ChangeSpellReset);
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
						Spell = reader.ReadType();

						CheckBook = reader.ReadBool();
						CheckRegs = reader.ReadBool();

						ChangeSpellReset = reader.ReadBool();
					}
					break;
			}
		}
	}
}