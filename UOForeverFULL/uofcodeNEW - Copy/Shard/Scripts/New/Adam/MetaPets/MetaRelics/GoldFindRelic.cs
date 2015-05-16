#region References
using System;

using Server.Factions;
using Server.Mobiles;
using Server.Mobiles.MetaPet;
using Server.Mobiles.MetaPet.Skills;
using Server.Mobiles.MetaSkills;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using VitaNex.Targets;

#endregion

namespace Server.Items
{
    public class GoldFindRelic : BaseMetaRelic
	{
		[Constructable]
		public GoldFindRelic()
		{
			Name = "a golden relic";
		    Hue = 1196;
			Weight = 2.0;
			Stackable = false;
            MetaSkillType = MetaSkillType.GoldFind;
            MetaSkillName = "Wealth";
            NextLevelExperience = 1000;
            MaxLevel = 10;
            ApplicationChance = 0.2;
            CoolDown = TimeSpan.FromSeconds(30);
            SkillMulti = 0.015;
		}

        public GoldFindRelic(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
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
                    break;
            }
		}
	}
}