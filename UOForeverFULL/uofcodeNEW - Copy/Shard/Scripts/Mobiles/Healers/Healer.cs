namespace Server.Mobiles
{
	public class Healer : BaseHealer
	{
		public override bool CanTeach { get { return true; } }

		public override bool CheckTeach(SkillName skill, Mobile from)
		{
			if (!base.CheckTeach(skill, from))
			{
				return false;
			}

			return (skill == SkillName.Forensics) || (skill == SkillName.Healing) || (skill == SkillName.SpiritSpeak) ||
				   (skill == SkillName.Swords);
		}

		[Constructable]
		public Healer()
		{
			Title = "the healer";

			SetSkill(SkillName.Forensics, 80.0, 100.0);
			SetSkill(SkillName.SpiritSpeak, 80.0, 100.0);
			SetSkill(SkillName.Swords, 80.0, 100.0);
		}

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			NameHue = EraAOS ? -1 : 0x35;
		}

		public override bool IsActiveVendor { get { return true; } }
		//public override bool IsInvulnerable{ get{ return true; } }

		public override void InitSBInfo()
		{
			SBInfos.Add(new SBHealer());
		}

		public override bool CheckResurrect(Mobile m)
		{
			if (m.Criminal)
			{
				Say(501222); // Thou art a criminal.  I shall not resurrect thee.
				return false;
			}
			
			if (m.Kills >= MurderCount)
			{
				Say(501223); // Thou'rt not a decent and good person. I shall not resurrect thee.
				return false;
			}
			
			if (m.Karma < 0)
			{
				Say(501224); // Thou hast strayed from the path of virtue, but thou still deservest a second chance.
			}

			return true;
		}

		public Healer(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			if (EraAOS && NameHue == 0x35)
			{
				NameHue = -1;
			}
		}
	}
}