using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1411, 0x141a )]
    public class WardenLegs : PlateLegs
	{
		[Constructable]
        public WardenLegs()
            : base(0x1411)
		{
            Name = "plate legs of the warden";
            Hue = 1157;
			Weight = 7.0;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Covered in congealing blood.", 1157);
        }

        public WardenLegs(Serial serial)
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