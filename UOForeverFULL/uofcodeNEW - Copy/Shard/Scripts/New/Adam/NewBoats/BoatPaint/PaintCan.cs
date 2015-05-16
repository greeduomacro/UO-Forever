using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class PaintCan : Item
    {
        private bool m_Redyable;
        private int m_DyedHue;
        private string Brand = "UO Broceliande's";
        public override int LabelNumber { get { return 1016211; } }
        private int m_Uses;

        public virtual CustomHuePicker CustomHuePicker { get { return null; } }

        public virtual bool AllowRepaint { get { return false; } }
        public virtual bool AllowHouse { get { return false; } }
        public virtual bool UsesCharges { get { return true; } }

        public virtual bool AllowWood { get { return false; } } //Wood walls and doors
        public virtual bool AllowStone { get { return false; } } //Stone walls and doors
        public virtual bool AllowMarble { get { return false; } } //Marble walls and doors
        public virtual bool AllowPlaster { get { return false; } } //Plaster and clay walls and doors
        public virtual bool AllowSandstone { get { return false; } } //Sandstone walls and doors
        public virtual bool AllowOther { get { return false; } } //Hide, Paper, Bamboo or Rattan walls and doors

        public override bool DisplayWeight { get { return false; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)m_Uses);
            writer.Write((bool)m_Redyable);
            writer.Write((int)m_DyedHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Uses = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Redyable = reader.ReadBool();
                        m_DyedHue = reader.ReadInt();

                        break;
                    }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(Brand);
            list.Add(Name);
            if (UsesCharges)
            {
                if (m_Uses > 0) list.Add("Uses left: {0}", m_Uses);
                else list.Add("** Empty **");
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Uses { get { return m_Uses; } set { m_Uses = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Redyable { get { return m_Redyable; } set { m_Redyable = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DyedHue { get { return m_DyedHue; } set { if (m_Redyable) { m_DyedHue = value; Hue = value; } } }

        [Constructable]
        public PaintCan()
            : this(1)
        {
        }

        [Constructable]
        public PaintCan(int uses)
            : base(0xFAB)
        {
            m_Uses = uses;
            Weight = 6.0;
            m_Redyable = true;
        }

        public PaintCan(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage("Target what you want to paint.");
                from.Target = new InternalTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        private class InternalTarget : Target
        {
            private PaintCan m_Can;

            public InternalTarget(PaintCan can)
                : base(3, false, TargetFlags.None)
            {
                m_Can = can;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                bool GM = from.AccessLevel >= AccessLevel.GameMaster;

                IPoint3D p3d;
                int id = 0;
                if (m_Can.UsesCharges && m_Can.Uses < 1)
                {
                    from.SendMessage("Your paint can is empty.");
                    return;
                }
				//if (from == BaseSmoothMulti.Owner)
				//{
						if (targeted is BaseSmoothMulti)
						{
							BaseSmoothMulti target = targeted as BaseSmoothMulti;
							id = target.ItemID;
							target.Hue = m_Can.DyedHue;
							from.PlaySound(0x23E);
							if (m_Can.UsesCharges)
								m_Can.Uses--;
							//from.SendMessage("TEMP: Successfully painted over BaseSmoothMulti.");

						}
						else
							if (targeted is MainMast)
							{
								MainMast target = targeted as MainMast;
								id = target.ItemID;
								target.Hue = m_Can.DyedHue;
								from.PlaySound(0x23E);
								if (m_Can.UsesCharges)
									m_Can.Uses--;
							}
							else
								from.SendMessage("Unable to paint that using this type of paint.");
						
                 //}
                 //else
                 //   from.SendMessage("You must be standing in, and targeting a boat you own.");
                
            }
        }
    }
}