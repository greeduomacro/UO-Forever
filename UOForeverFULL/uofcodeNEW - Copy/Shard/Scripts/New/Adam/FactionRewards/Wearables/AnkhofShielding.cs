using System;
using Server.Factions;
using Server.Network;

namespace Server.Items	
{
	public class AnkhofShielding : BaseNecklace
	{
        public override string DefaultName { get { return "Ankh of Rejuvenation"; } }

 		private static readonly int m_FallbackItemID = 0x1088;

		private Packet m_FallbackWorldPacket;

		public Packet FallbackWorldPacket
		{
			get
			{
				if ( m_FallbackWorldPacket == null )
				{
					m_FallbackWorldPacket = new WorldItem( this, m_FallbackItemID );
					m_FallbackWorldPacket.SetStatic();
				}

				return m_FallbackWorldPacket;
			}
		}

		[Constructable]
		public AnkhofShielding() : base( 0x3BB5 )
		{
		}

        public AnkhofShielding(Serial serial)
            : base(serial)
		{
		}

        public override bool OnEquip(Mobile from)
        {
            if (Faction.InSkillLoss(from))
            {
                Faction.SkillLossContext context;
                Faction.m_SkillLoss.TryGetValue(from, out context);
                if (context != null && DateTime.UtcNow >= (context.m_Timer.Next - TimeSpan.FromMinutes(5.0)))
                {
                    Faction.ClearSkillLoss(from);
                }
            }
            return base.OnEquip(from);
        }

		protected override Packet GetWorldPacketFor( NetState state )
		{
			if ( state.HighSeas )
				return this.WorldPacketHS;
			else if ( state.StygianAbyss )
				return this.WorldPacketSA;
			else
				return this.FallbackWorldPacket;
		}

		public override void ReleaseWorldPackets()
		{
			base.ReleaseWorldPackets();

			Packet.Release( ref m_FallbackWorldPacket );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

    public class AnkhofShieldingTB : AnkhofShielding
    {
        [Constructable]
        public AnkhofShieldingTB()
        {
            FactionItem.Imbue(this, TrueBritannians.Instance, false, TrueBritannians.Instance.Definition.HuePrimary);
            Hue = 2214;
            Weight = 0.1;
            LootType = LootType.Blessed;
        }

        public AnkhofShieldingTB(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "[True Britannians]", TrueBritannians.Instance.Definition.HuePrimary);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
    public class AnkhofShieldingMinax : AnkhofShielding
    {
        [Constructable]
        public AnkhofShieldingMinax()
        {

            Name = "Ankh of Shielding";
            FactionItem.Imbue(this, Minax.Instance, false, Minax.Instance.Definition.HuePrimary);
            Hue = 1645;
            Weight = 0.1;
            LootType = LootType.Blessed;
        }

        public AnkhofShieldingMinax(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "[Minax]", Minax.Instance.Definition.HuePrimary);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
    public class AnkhofShieldingCoM : AnkhofShielding
    {
        [Constructable]
        public AnkhofShieldingCoM()
        {
            FactionItem.Imbue(this, CouncilOfMages.Instance, false, CouncilOfMages.Instance.Definition.HuePrimary);
            Hue = 1325;
            Weight = 0.1;
            LootType = LootType.Blessed;
        }

        public AnkhofShieldingCoM(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "[Council of Mages]", CouncilOfMages.Instance.Definition.HuePrimary);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
    public class AnkhofShieldingSL : AnkhofShielding
    {
        [Constructable]
        public AnkhofShieldingSL()
        {
            Name = "Ankh of Shielding";
            FactionItem.Imbue(this, Shadowlords.Instance, false, Shadowlords.Instance.Definition.HuePrimary);
            Hue = 1109;
            Weight = 0.1;
            LootType = LootType.Blessed;
        }

        public AnkhofShieldingSL(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "[Shadowlords]", Shadowlords.Instance.Definition.HuePrimary);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}