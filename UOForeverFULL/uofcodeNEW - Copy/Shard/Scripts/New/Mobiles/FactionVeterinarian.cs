// **********
// RunUO Shard - FactionVeterinarian.cs
// **********

#region References

using System;
using System.Collections.Generic;
using Server.Factions;
using Server.Gumps;

#endregion

namespace Server.Mobiles
{
    public class FactionVeterinarian : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();

        protected override List<SBInfo> SBInfos
        {
            get
            {
                if (m_SBInfos != null) return m_SBInfos;
                return null;
            }
        }

        private int m_Price;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        [Constructable]
        public FactionVeterinarian()
            : this(5000)
        {
        }

        [Constructable]
        public FactionVeterinarian(int price)
            : base("the vet")
        {
            m_Price = price;
            SetSkill(SkillName.AnimalLore, 100.0, 100.0);
            SetSkill(SkillName.Veterinary, 100.0, 100.0);
        }


        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBVeterinarian());
        }


        public virtual void OfferResurrection(BaseCreature p, Mobile master)
        {
            Direction = GetDirectionTo(p);

            master = p.ControlMaster;

            p.PlaySound(0x214);
            p.FixedEffect(0x376A, 10, 16);

            master.CloseGump(typeof (FactionPetResurrectGump));
            master.SendGump(new FactionPetResurrectGump(master, p, m_Price));
        }

        public virtual bool CheckResurrect(BaseCreature p)
        {
            return (!p.Body.IsHuman && p.Controlled && p.ControlMaster != null);
        }

        private DateTime m_NextResurrect;
        private static TimeSpan ResurrectDelay = TimeSpan.FromSeconds(2.0);


        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is BaseCreature)
            {
                BaseCreature p = m as BaseCreature;
                PlayerMobile master;
                if (p.ControlMaster != null)
                    master = p.ControlMaster as PlayerMobile;
                else
                    return;
                if (!m.Frozen && DateTime.UtcNow >= m_NextResurrect && InRange(m, 4) && !InRange(oldLocation, 4) &&
                    InLOS(m))
                {
                    if (master != null && p.IsDeadPet && master.Alive && Faction.Find(master, true) != null)
                    {
                        m_NextResurrect = DateTime.UtcNow + ResurrectDelay;

                        if (m.Map == null || !m.Map.CanFit(m.Location, 16, false, false))
                        {
                            master.SendMessage("Your Pet cannot be ressurected there!");
                            // Thou can not be resurrected there!
                        }
                        else if (CheckResurrect(p))
                        {
                            OfferResurrection(p, master);
                        }
                    }
                }
            }
        }


        public FactionVeterinarian(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Price);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    m_Price = reader.ReadInt();
                    break;
                }
            }
        }
    }
}