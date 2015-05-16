using System;
using Server;

namespace Server.Items
{
	public class TBSkullCandle : BaseLight
	{
		public override int LitItemID{ get { return 0x1855; } }
		public override int UnlitItemID{ get { return 0x1853; } }

		[Constructable]
        public TBSkullCandle()
            : base(0x1853)
		{
			if ( Burnout )
				Duration = TimeSpan.FromMinutes( 25 );
			else
				Duration = TimeSpan.Zero;

            Name = "faction incense";

			Burning = false;
			Light = LightType.Circle150;
			Weight = 2.0;
		    LootType = LootType.Blessed;
		    Hue = 2214;
		}

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[True Britannians]", 2214);
            base.OnSingleClick(from);
        }

        public TBSkullCandle(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    public class CoMskullCandle : BaseLight
    {
        public override int LitItemID { get { return 0x1855; } }
        public override int UnlitItemID { get { return 0x1853; } }

        [Constructable]
        public CoMskullCandle()
            : base(0x1853)
        {
            if (Burnout)
                Duration = TimeSpan.FromMinutes(25);
            else
                Duration = TimeSpan.Zero;

            Name = "faction incense";

            Burning = false;
            Light = LightType.Circle150;
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 1325;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[Council of Mages]", 1325);
            base.OnSingleClick(from);
        }

        public CoMskullCandle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class MinaxSkullCandle : BaseLight
    {
        public override int LitItemID { get { return 0x1855; } }
        public override int UnlitItemID { get { return 0x1853; } }

        [Constructable]
        public MinaxSkullCandle()
            : base(0x1853)
        {
            if (Burnout)
                Duration = TimeSpan.FromMinutes(25);
            else
                Duration = TimeSpan.Zero;

            Name = "faction incense";

            Burning = false;
            Light = LightType.Circle150;
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 1645;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[Minax]", 1645);
            base.OnSingleClick(from);
        }

        public MinaxSkullCandle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SLSkullCandle : BaseLight
    {
        public override int LitItemID { get { return 0x1855; } }
        public override int UnlitItemID { get { return 0x1853; } }

        [Constructable]
        public SLSkullCandle()
            : base(0x1853)
        {
            if (Burnout)
                Duration = TimeSpan.FromMinutes(25);
            else
                Duration = TimeSpan.Zero;

            Name = "faction incense";

            Burning = false;
            Light = LightType.Circle150;
            Weight = 2.0;
            LootType = LootType.Blessed;
            Hue = 2211;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "[Shadowlords]", 2211);
            base.OnSingleClick(from);
        }

        public SLSkullCandle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}