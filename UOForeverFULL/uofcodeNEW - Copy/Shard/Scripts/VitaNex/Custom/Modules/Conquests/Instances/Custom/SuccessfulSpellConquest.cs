#region References
using System;

using Server.Items;
using Server.Spells;

using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
	public class SuccessfulSpellConquest : Conquest
	{
		public override string DefCategory { get { return "Casting"; } }

		public virtual Type DefSpell { get { return null; } }
		public virtual bool DefChangeSpellReset { get { return false; } }

		[CommandProperty(Conquests.Access)]
		public SpellTypeSelectProperty Spell { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ChangeSpellReset { get; set; }

		public SuccessfulSpellConquest()
		{ }

        public SuccessfulSpellConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Spell = DefSpell;

			ChangeSpellReset = DefChangeSpellReset;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as ISpell);
		}

        protected virtual int GetProgress(ConquestState state, ISpell spell)
		{
            if (state.User == null)
                return 0;

            if (Spell.IsNotNull && !spell.TypeEquals(Spell, false))
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
						ChangeSpellReset = reader.ReadBool();
					}
					break;
			}
		}
	}
}