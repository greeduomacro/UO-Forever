using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1411, 0x141a )]
    public class PKLegs : PlateLegs
	{
		[Constructable]
        public PKLegs()
            : base(0x1411)
		{
            Name = "plate legs of the pumpkin king";
            Hue = 1358;
			Weight = 7.0;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Infused with the awesome might of the Pumpkin King.", 1358);
        }

        public PKLegs(Serial serial)
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