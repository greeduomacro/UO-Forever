#region References
using System;
using System.Diagnostics;
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
	public class BaseMetaRelic : Item
	{
        [CommandProperty(AccessLevel.GameMaster)]
        public MetaSkillType MetaSkillType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string MetaSkillName { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int NextLevelExperience { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLevel { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double ApplicationChance { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan CoolDown { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double SkillMulti { get; set; }

        [Constructable]
        public BaseMetaRelic()
			: base(10916)
		{
			Name = "a relic";
            Hue = 1368;
			Weight = 2.0;
			Stackable = false;
		}

        public BaseMetaRelic(Serial serial)
			: base(serial)
		{ }

	    public override void OnDoubleClick(Mobile from)
	    {
	        if (IsChildOf(from.Backpack) && from is PlayerMobile)
	        {
                    from.SendMessage(54, "Perhaps if you had a Meta Stone you could harness the raw energy of this relic..");
	        }
	        else
	        {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
	        }
	    }

        public virtual void GetMeta(Mobile User, Mobile target)
        {
            var metapet = target as BaseMetaPet;

            if (metapet != null && metapet.Controlled && metapet.ControlMaster == User)
            {
                if (metapet.MaxAbilities < 0 || metapet.CurrentAbilities >= metapet.MaxAbilities)
                {
                    User.SendMessage(54, "Your Meta-Pet does not have any free slots to add this ability to.");
                    return;
                }
                if (metapet.Metaskills != null && !metapet.Metaskills.ContainsKey(MetaSkillType))
                {
                    metapet.CurrentAbilities++;
                    metapet.Metaskills.Add(MetaSkillType, new BaseMetaSkill(MetaSkillType, MetaSkillName, NextLevelExperience, MaxLevel, ApplicationChance, CoolDown, SkillMulti));
                    User.SendMessage(54, "You have successfully applied this relic to your pet.");
                    Consume();
                }
                else
                {
                    User.SendMessage(54, "Your Meta-Pet already has this skill!");                   
                }
            }
            else
            {
                User.SendMessage(54, "This can only be used on a Meta-Pet that you own!");
            }
        }

        #region GetRandomRelic
        public static BaseMetaRelic GetRandomRelic()
	    {
	        BaseMetaRelic relic = null;

            Array values = Enum.GetValues(typeof(MetaSkillType));
            Random random = new Random();
            MetaSkillType RandomSkill = (MetaSkillType)values.GetValue(random.Next(values.Length));

	        switch (RandomSkill)
	        {
	            case MetaSkillType.Bleed:
	            {
	                relic = new BloodyRelic();
	                break;
	            }
                case MetaSkillType.Molten:
                {
                    relic = new MoltenRelic();
                    break;
                }
                case MetaSkillType.VenemousBlood:
                {
                    relic = new NoxiousRelic();
                    break;
                }
                case MetaSkillType.Quicksilver:
                {
                    relic = new QuicksilverRelic();
                    break;
                }
                case MetaSkillType.GoldFind:
                {
                    relic = new GoldFindRelic();
                    break;
                }
	        }
	        return relic;
	    }
        #endregion

        public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

            int version = writer.SetVersion(0);

            switch (version)
            {
                case 0:
                {
                    writer.Write((int)MetaSkillType);
                    writer.Write(MetaSkillName);
                    writer.Write(NextLevelExperience);
                    writer.Write(MaxLevel);
                    writer.Write(ApplicationChance);
                    writer.Write(CoolDown);
                    writer.Write(SkillMulti);                 
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
                    MetaSkillType = (MetaSkillType) reader.ReadInt();
                    MetaSkillName = reader.ReadString();
                    NextLevelExperience = reader.ReadInt();
                    MaxLevel = reader.ReadInt();
                    ApplicationChance = reader.ReadDouble();
                    CoolDown = reader.ReadTimeSpan();
                    SkillMulti = reader.ReadDouble();
                }
                    break;
            }
		}
	}
}