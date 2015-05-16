#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Commands;
using Server.Engines.CustomTitles;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;

#endregion

namespace Server.Items
{
    [Flags]
    public enum SkillScrollFlags
    {
        None = 0x00,
        Newbie = 0x01
    }

    public class SkillScroll : Item
    {
        public static List<Item> ScrollsToConvert = new List<Item>();
        public static List<SkillName> ScrollsToConvertSkillNames = new List<SkillName>();
        // have to do it this way so as to ensure all parents have been deserialized
        public static void Initialize()
        {
            for (int i = 0; i < ScrollsToConvert.Count; i++)
            {
                ConvertSkillScrollOperation(ScrollsToConvert[i], ScrollsToConvertSkillNames[i]);
            }
        }

        public static void ConvertSkillScroll(Item scroll, SkillName skillname)
        {
            ScrollsToConvert.Add(scroll);
            ScrollsToConvertSkillNames.Add(skillname);
        }

        public static void ConvertSkillScrollOperation(Item scroll, SkillName skillname)
        {
            try
            {
                var convertedScroll = new SkillScroll(1);
                convertedScroll.SkillName = skillname;
                if (scroll.Parent != null)
                {
                    if (scroll.Parent is Container)
                    {
                        var scrollParent = (Container) scroll.Parent;
                        scrollParent.AddItem(convertedScroll);
                        convertedScroll.X = scroll.X;
                        convertedScroll.Y = scroll.Y;
                        convertedScroll.Z = scroll.Z;
                        if (scroll.IsLockedDown)
                        {
                            BaseHouse house = BaseHouse.FindHouseAt(scroll.RootParentEntity as Item);
                            if (house == null)
                            {
                                LoggingCustom.Log("LOG_ScrollFail.txt",
                                    scroll + "\t" + skillname + "\t" + scroll.Location + "\tHOUSE WAS NULL!");
                            }
                            else
                            {
                                house.Release(house.Owner, scroll);
                                house.LockDown(house.Owner, convertedScroll);
                                LoggingCustom.Log("LOG_ScrollFail.txt",
                                    scroll + "\t" + skillname + "\t" + scroll.Location + "\tLOCKED DOWN IN CONTAINER");
                            }
                        }
                        if (scroll.RootParentEntity is PlayerVendor)
                        {
                            var pv = scroll.RootParentEntity as PlayerVendor;
                            if (pv.Owner != null)
                            {
                                pv.Owner.BankBox.AddItem(convertedScroll);
                                LoggingCustom.Log("LOG_ScrollVendor.txt",
                                    scroll + "\t" + skillname + "\t" + scroll.Location + "\t" + scroll.RootParentEntity);
                            }
                        }
                        scroll.Delete();
                    }
                    else
                    {
                        LoggingCustom.Log("LOG_ScrollFail.txt", scroll + "\t" + skillname + "\t" + scroll.Location);
                    }
                    if (scroll.RootParentEntity != null)
                    {
                        LoggingCustom.Log("LOG_ScrollConvert.txt",
                            skillname + "\towner: " + scroll.RootParentEntity + "\t" + scroll.RootParentEntity.Location);
                    }
                }
                else if (scroll.Map == Map.Felucca)
                {
                    LoggingCustom.Log("LOG_ScrollConvert.txt", skillname + "\towner: null\t" + scroll.Location);
                    convertedScroll.MoveToWorld(scroll.Location, scroll.Map);
                    if (scroll.IsLockedDown)
                    {
                        BaseHouse house = BaseHouse.FindHouseAt(scroll);
                        house.Release(house.Owner, scroll);
                        house.LockDown(house.Owner, convertedScroll);
                        LoggingCustom.Log("LOG_ScrollFail.txt",
                            scroll + "\t" + skillname + "\t" + scroll.Location + "\tlockdown on house floor");
                    }
                    scroll.Delete();
                }
                else
                {
                    LoggingCustom.Log("LOG_ScrollFail.txt", scroll + "\t" + skillname + "\t" + scroll.Location);
                }
            }
            catch (Exception e)
            {
                LoggingCustom.Log("LOG_ScrollFail.txt", e.Message + "\n" + e.StackTrace);
            }
        }

        private int m_SkillBonus;
        private SkillScrollFlags m_Flags;

        private SkillName m_SkillName = SkillName.Begging;

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName SkillName
        {
            get { return m_SkillName; }
            set
            {
                m_SkillName = value;
                UpdateHue();
            }
        }

        public SkillScrollFlags Flags { get { return m_Flags; } set { m_Flags = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SkillBonus
        {
            get { return m_SkillBonus; }
            set
            {
                m_SkillBonus = value;
                InvalidateProperties();
            }
        }

        public bool GetFlag(SkillScrollFlags flag)
        {
            return (m_Flags & flag) != 0;
        }

        public void Randomize()
        {
            SkillName = RandomLootScrolls[Utility.Random(RandomLootScrolls.Length)];
            if (SkillName == SkillName.AnimalTaming && Utility.RandomDouble() >= 0.5)
            {
                SkillName = SkillName.Provocation;
            }
            if (SkillName == SkillName.Poisoning && Utility.RandomDouble() >= 0.5)
            {
                SkillName = SkillName.MagicResist;
            }
            if (SkillName == SkillName.Lockpicking && Utility.RandomDouble() >= 0.5)
            {
                SkillName = SkillName.Stealing;
            }
            if (SkillName == SkillName.Discordance && Utility.RandomDouble() >= 0.5)
            {
                SkillName = SkillName.Provocation;
            }
        }

        public static SkillName[] RandomLootScrolls = new SkillName[]
        {
            SkillName.Alchemy,
            SkillName.Anatomy,
            SkillName.Archery,
            SkillName.AnimalTaming,
            SkillName.Blacksmith,
            SkillName.Carpentry,
            SkillName.Discordance,
            SkillName.EvalInt,
            SkillName.Fencing,
            SkillName.Fishing,
            SkillName.Fletching,
            SkillName.Healing,
            SkillName.Hiding,
            SkillName.Inscribe,
            SkillName.Lockpicking,
            SkillName.Lumberjacking,
            SkillName.Macing,
            SkillName.Magery,
            SkillName.MagicResist,
            SkillName.Mining,
            SkillName.Parry,
            SkillName.Peacemaking,
            SkillName.Poisoning,
            SkillName.Stealing,
            SkillName.Stealth,
            SkillName.Swords,
            SkillName.Tactics,
            SkillName.Tailoring,
            SkillName.Tinkering,
            SkillName.Wrestling,
        };

        public void SetFlag(SkillScrollFlags flag, bool value)
        {
            if (value)
            {
                m_Flags |= flag;
            }
            else
            {
                m_Flags &= ~flag;
            }
        }

        [Constructable]
        public SkillScroll(int bonus)
            : base(8800)
        {
            m_SkillBonus = bonus;
            Stackable = true;
            Weight = 1;
        }

        [Constructable]
        public SkillScroll() : this(1)
        {}

        public SkillScroll(Serial serial) : base(serial)
        {}

        public override void OnAfterDuped(Item newItem)
        {
            if (newItem is SkillScroll)
            {
                ((SkillScroll) newItem).SkillName = SkillName;
            }
        }

        public override string DefaultName { get { return "a Skill Scroll"; } }
        public override int LabelNumber { get { return 0; } }
        public override bool DisplayLootType { get { return false; } }

        public virtual void UpdateHue()
        {
            if (m_SkillName == SkillName.Alchemy || m_SkillName == SkillName.ArmsLore ||
                m_SkillName == SkillName.Blacksmith || m_SkillName == SkillName.Carpentry ||
                m_SkillName == SkillName.Cartography
                || m_SkillName == SkillName.Lockpicking || m_SkillName == SkillName.Cooking
                || m_SkillName == SkillName.Fishing || m_SkillName == SkillName.Inscribe ||
                m_SkillName == SkillName.ItemID || m_SkillName == SkillName.Lumberjacking ||
                m_SkillName == SkillName.Mining
                || m_SkillName == SkillName.Tailoring || m_SkillName == SkillName.Tinkering ||
                m_SkillName == SkillName.Tracking)
            {
                Hue = 1355;
            }
            else if (m_SkillName == SkillName.Anatomy || m_SkillName == SkillName.Healing ||
                     m_SkillName == SkillName.EvalInt || m_SkillName == SkillName.Fencing
                     || m_SkillName == SkillName.Magery || m_SkillName == SkillName.Macing ||
                     m_SkillName == SkillName.MagicResist || m_SkillName == SkillName.Meditation
                     || m_SkillName == SkillName.Parry || m_SkillName == SkillName.Swords ||
                     m_SkillName == SkillName.Tactics || m_SkillName == SkillName.Wrestling)
            {
                Hue = 37;
            }
            else if (m_SkillName == SkillName.AnimalLore || m_SkillName == SkillName.Veterinary ||
                     m_SkillName == SkillName.Herding || m_SkillName == SkillName.Begging ||
                     m_SkillName == SkillName.Camping || m_SkillName == SkillName.DetectHidden
                     || m_SkillName == SkillName.Forensics || m_SkillName == SkillName.SpiritSpeak ||
                     m_SkillName == SkillName.TasteID)
            {
                Hue = 1368;
            }
            else if (m_SkillName == SkillName.Hiding || m_SkillName == SkillName.Poisoning ||
                     m_SkillName == SkillName.Snooping || m_SkillName == SkillName.Stealing ||
                     m_SkillName == SkillName.Stealth
                     || m_SkillName == SkillName.RemoveTrap)
            {
                Hue = 1904;
            }
            else if (m_SkillName == SkillName.Discordance || m_SkillName == SkillName.Musicianship ||
                     m_SkillName == SkillName.Peacemaking || m_SkillName == SkillName.Provocation)
            {
                Hue = 1376;
            }
            else if (m_SkillName == SkillName.AnimalTaming)
            {
                Hue = 1161;
            }
        }

        public virtual string GetLabelName()
        {
            if (Amount > 1)
            {
                return "+{0} " + SkillName.ToString() + " Skill Scrolls : " + Amount;
            }

            return "a +{0} " + SkillName.ToString() + " Skill Scroll";
        }

        public override bool CanStackWith(Item dropped)
        {
            if (dropped is SkillScroll && ((SkillScroll) dropped).SkillName == SkillName &&
                ((SkillScroll) dropped).SkillBonus == SkillBonus)
            {
                return base.CanStackWith(dropped);
            }
            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "bonus\t{0}", m_SkillBonus);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Deleted || !from.CanSee(this))
            {
                return;
            }

            LabelToExpansion(from);

            LabelTo(from, String.Format(GetLabelName(), m_SkillBonus));
        }

        public virtual void SendGump(Mobile from)
        {
            if (from is PlayerMobile)
            {
                from.SendGump(new SkillScrollUI(from as PlayerMobile, null, this));
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (SkillBonus <= 0)
            {
                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    from.SendGump(new PropertiesGump(from, this));
                }
                SendLocalizedMessageTo(from, 1042544); // This item is out of charges.
            }
            else if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (!from.HasGump(typeof(SkillScroll)))
            {
                SendGump(from);
            }
        }

        public void GetPet(Mobile User, Mobile target)
        {
            var pet = target as BaseCreature;
            if (pet != null && pet.Controlled && pet.ControlMaster == User)
            {
                ApplySkilltoPet(pet, pet.ControlMaster as PlayerMobile);
            }
            else
            {
                User.SendMessage(54, "This can only be used on a pet that you own!");
            }
        }

        public void ApplySkilltoPlayer(PlayerMobile pm)
        {
            Skill skill = pm.Skills[SkillName];

            if (skill == null)
            {
                return;
            }

            double count = pm.Skills.Total / 10;
            double cap = pm.SkillsCap / 10;
            double decreaseamount;
            int bonus = SkillBonus;

            List<Skill> decreased = GetDecreasableSkills(pm, count, cap,
                out decreaseamount);

            if (decreased.Count > 0 && decreaseamount <= 0)
            {
                pm.SendMessage("You have exceeded the skill cap and do not have a skill set to be decreased.");
            }
            else if ((skill.Base + bonus) > skill.Cap)
            {
                pm.SendMessage("Your skill is too high to raise it further.");
            }
            else if (skill.Lock != SkillLock.Up)
            {
                pm.SendMessage("You must set the skill to be increased in order to raise it further.");
            }
            else
            {
                if ((cap - count + decreaseamount) >= bonus)
                {
                    pm.SendMessage(54, "Your " + skill.SkillName + " has increased by " + SkillBonus + ".");
                    DecreaseSkills(pm, decreased, count, cap, decreaseamount);
                    IncreaseSkill(pm, skill);

                    Consume();
                }
                else
                {
                    pm.SendMessage(
                        "You have exceeded the skill cap and do not have enough skill set to be decreased.");
                }
            }
        }

        public void ApplySkilltoPet(BaseCreature pet, PlayerMobile owner)
        {
            Skill skill = pet.Skills[SkillName];

            if (skill == null)
            {
                return;
            }

            if (skill.BaseFixedPoint == 0)
            {
                owner.SendMessage("You cannot use a skill scroll on a pet that hasn't already had some training in " + SkillName);
                return;
            }

            double count = pet.Skills.Total / 10;
            double cap = pet.SkillsCap / 10;
            int bonus = SkillBonus;

            if ((skill.Base + bonus) > skill.Cap)
            {
                owner.SendMessage("Your pets skill is too high to raise it further.");
            }
            else
            {
                owner.SendMessage(54, "Your pets " + skill.SkillName + " has increased by " + SkillBonus + ".");
                IncreaseSkill(pet, skill);

                Consume();
            }
        }

        public virtual List<Skill> GetDecreasableSkills(Mobile from, double count, double cap,
            out double decreaseamount)
        {
            Skills skills = from.Skills;
            decreaseamount = 0.0;

            var decreased = new List<Skill>();
            double bonus = m_SkillBonus;

            if ((count + bonus) > cap)
            {
                foreach (Skill t in skills.Where(t => t.Lock == SkillLock.Down && t.Base > 0.0))
                {
                    decreased.Add(t);
                    decreaseamount += t.Base;
                }
            }

            return decreased;
        }

        public virtual void DecreaseSkills(Mobile from, List<Skill> decreased, double count, double cap,
            double decreaseamount)
        {
            double freepool = cap - count;
            double bonus = m_SkillBonus;

            if (freepool < bonus)
            {
                bonus -= freepool;

                foreach (Skill s in decreased)
                {
                    if (s.Base >= bonus)
                    {
                        s.Base -= bonus;
                        bonus = 0;
                    }
                    else
                    {
                        bonus -= s.Base;
                        s.Base = 0;
                    }

                    if (bonus == 0)
                    {
                        break;
                    }
                }
            }
        }

        public virtual void IncreaseSkill(Mobile from, Skill skill)
        {
            skill.Base += m_SkillBonus;
            SendConfirmMessage(from, skill);
        }

        public virtual void SendConfirmMessage(Mobile from, Skill skill)
        {
            from.SendMessage("Your skill in {0} has been raised by {1}", skill.Name, m_SkillBonus);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int) 0); // version

            writer.WriteEncodedInt((int) m_Flags);

            writer.Write(m_SkillBonus);
            writer.Write((byte) m_SkillName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                {
                    if (SkillBonus == 0)
                        SkillBonus = 1;
                    m_Flags = (SkillScrollFlags) reader.ReadEncodedInt();
                    m_SkillBonus = reader.ReadInt();
                    m_SkillName = (SkillName) reader.ReadByte();
                    break;
                }
            }
        }
    }
}

/*namespace Server.Gumps
{
    public class SkillScrollGump : Gump
    {
        private const int FieldsPerPage = 1;

        private Skill m_Skill;
        private readonly SkillScroll m_SkillScroll;
        private readonly Mobile m_Target;

        public SkillScrollGump(Mobile from, Mobile target, SkillScroll ball) : base(20, 20)
        {
            m_SkillScroll = ball;
            m_Target = target;

            AddPage(0);
            AddBackground(0, 0, 260, 351, 5054);

            AddImageTiled(10, 10, 240, 23, 0x52);
            AddImageTiled(11, 11, 238, 21, 0xBBC);

            AddLabel(45, 11, 0, "Forever Skill Scroll");

            AddPage(1);

            int page = 1;
            int index = 0;

            Skills skills = m_Target.Skills;
            var allowedskills = new SkillName[] {m_SkillScroll.SkillName};

            for (int i = 0; i < allowedskills.Length; ++i)
            {
                if (index >= FieldsPerPage)
                {
                    AddButton(231, 13, 0x15E1, 0x15E5, 0, GumpButtonType.Page, page + 1);

                    ++page;
                    index = 0;

                    AddPage(page);

                    AddButton(213, 13, 0x15E3, 0x15E7, 0, GumpButtonType.Page, page - 1);
                }

                Skill skill = skills[allowedskills[i]];

                if (skill.Base == 0 &&
                    (skill.Base + m_SkillScroll.SkillBonus) <= skill.Cap && skill.Lock == SkillLock.Up)
                {
                    AddImageTiled(10, 32 + (index * 22), 240, 23, 0x52);
                    AddImageTiled(11, 33 + (index * 22), 238, 21, 0xBBC);

                    AddLabelCropped(13, 33 + (index * 22), 150, 21, 0, skill.Name);
                    AddImageTiled(180, 34 + (index * 22), 50, 19, 0x52);
                    AddImageTiled(181, 35 + (index * 22), 48, 17, 0xBBC);
                    AddLabelCropped(182, 35 + (index * 22), 234, 21, 0, skill.Base.ToString("F1"));

                    AddButton(231, 35 + (index * 22), 0x15E1, 0x15E5, i + 1, GumpButtonType.Reply, 0);

                    ++index;
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (from == null || m_SkillScroll.Deleted)
            {
                return;
            }

            if (!m_SkillScroll.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (info.ButtonID == 1)
            {
                SkillName skillname = m_SkillScroll.SkillName;
                m_Skill = m_Target.Skills[skillname];

                if (m_Skill == null)
                {
                    return;
                }

                double count = m_Target.Skills.Total / 10;
                double cap = m_Target.SkillsCap / 10;
                double decreaseamount;
                int bonus = m_SkillScroll.SkillBonus;

                List<Skill> decreased = m_SkillScroll.GetDecreasableSkills(from, m_Target, count, cap,
                    out decreaseamount);

                if (decreased.Count > 0 && decreaseamount <= 0)
                {
                    from.SendMessage("You have exceeded the skill cap and do not have a skill set to be decreased.");
                }
                else if ((m_Skill.Base + bonus) > m_Skill.Cap)
                {
                    from.SendMessage("Your skill is too high to raise it further.");
                }
                else if (m_Skill.Lock != SkillLock.Up)
                {
                    from.SendMessage("You must set the skill to be increased in order to raise it further.");
                }
                else
                {
                    if ((cap - count + decreaseamount) >= bonus)
                    {
                        m_SkillScroll.DecreaseSkills(from, m_Target, decreased, count, cap, decreaseamount);
                        m_SkillScroll.IncreaseSkill(from, m_Target, m_Skill);

                        m_SkillScroll.Consume();
                        m_SkillScroll.SkillBonus = 1;
                    }
                    else
                    {
                        from.SendMessage(
                            "You have exceeded the skill cap and do not have enough skill set to be decreased.");
                    }
                }
            }
        }
    }
}*/