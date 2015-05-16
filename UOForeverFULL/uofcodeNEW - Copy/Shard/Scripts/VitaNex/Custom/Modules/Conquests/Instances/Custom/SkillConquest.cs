#region References
using System;

using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
	public class SkillConquest : Conquest
	{
		public override string DefCategory { get { return "Skills"; } }

		public virtual Skill DefSkill { get { return null; } }
		public virtual bool DefSkillReset { get { return false; } }
        public virtual bool DefChildren { get { return false; } }

		[CommandProperty(Conquests.Access)]
		public SkillName Skill { get; set; }

        [CommandProperty(Conquests.Access)]
        public double SkillAmount { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool Children { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ChangeSkillReset { get; set; }

		public SkillConquest()
		{ }

        public SkillConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Children = DefChildren;
		    SkillAmount = 0;
			ChangeSkillReset = DefSkillReset;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as Skill);
		}

		protected virtual int GetProgress(ConquestState state, Skill skill)
		{
            if (skill == null)
			{
				return 0;
			}

            if (state.User == null)
                return 0;

            if ((Skill != (SkillName)skill.SkillID))
            {
                if (ChangeSkillReset)
                {
                    return -state.Progress;
                }

                return 0;
            }

		    if (SkillAmount != 0 && SkillAmount > state.User.Skills[Skill].Value)
		    {
                return 0;
		    }

			return 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				case 0:
					{
						writer.Write((int)Skill);
                        writer.Write(SkillAmount);
						writer.Write(Children);
						writer.Write(ChangeSkillReset);
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
				case 0:
					{
						Skill = (SkillName)reader.ReadInt();
						SkillAmount = version > 0 ? reader.ReadDouble() : reader.ReadInt();
						Children = reader.ReadBool();
						ChangeSkillReset = reader.ReadBool();
					}
					break;
			}
		}
	}
}