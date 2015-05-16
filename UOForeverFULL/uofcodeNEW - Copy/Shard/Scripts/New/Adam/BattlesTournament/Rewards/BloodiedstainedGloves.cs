using System;
using Server.Mobiles;
using Server.Spells.Fifth;
using VitaNex.FX;

namespace Server.Items
{
	[Flipable]
	public class BloodstainedGloves : BaseArmor
	{
		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 40; } }

		public override int OldStrReq{ get{ return 10; } }

		public override int ArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }
		public override int LabelNumber{ get{ return GetLeatherLabel( 1049160 ); } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

        private Timer m_Timer;
        public Timer Timer { get { return m_Timer; } }

		[Constructable]
		public BloodstainedGloves() : base( 0x13C6 )
		{
			Weight = 1.0;
			Dyable = false;
		    Identified = true;
		    Hue = 1157;
            Slayer = SlayerName.Repond;
            LootType = LootType.Blessed;
		    Name = "Bloodstained Gloves";
		}

        public BloodstainedGloves(Serial serial)
            : base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "These gruesome gloves are awash in the blood of the fallen.", 137);
        }

	    public override void OnAdded(object parent)
	    {
            if (parent is Mobile)
            {
                m_Timer = new InternalTimer(parent as Mobile);
                m_Timer.Start();
            }
	        base.OnAdded(parent);
	    }

        public override void OnRemoved(object parent)
        {
            if (m_Timer != null)
			{
                m_Timer.Stop();
			}
            base.OnAdded(parent);
        }

        public static void CreateBlood(Point3D loc, Map map, int hue, bool delayed, Mobile m)
        {
            int bloodID = Utility.RandomMinMax(4650, 4655);
            new Blood(bloodID, hue, delayed).MoveToWorld(new Point3D(BloodOffset(loc.X), BloodOffset(loc.Y), loc.Z), map);
        }

        public static int BloodOffset(int coord)
        {
            return coord + Utility.RandomMinMax(-1, 1);
        }

	    public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write(0); // version

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
				}
			        break;
			}
		}

        private class InternalTimer : Timer
        {
            private Mobile m_Owner;

            public InternalTimer(Mobile owner)
                :   base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(20.0))
            {
                m_Owner = owner;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                CreateBlood(m_Owner.Location, m_Owner.Map, m_Owner.BloodHue, false, m_Owner);
            }
        }
	}
}
