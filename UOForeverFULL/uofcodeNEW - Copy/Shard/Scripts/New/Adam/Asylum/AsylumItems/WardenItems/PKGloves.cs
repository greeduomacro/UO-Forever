using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1414, 0x1418 )]
    public class WardenGloves : PlateGloves
	{
		[Constructable]
        public WardenGloves()
            : base(0x1414)
		{
            Name = "plate gloves of the warden";
            Hue = 1157;
			Weight = 2.0;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Covered in congealing blood.", 1157);
        }

        public WardenGloves(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 1.0 )
				Weight = 5.0;
		}
	}
}