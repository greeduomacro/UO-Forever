using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1410, 0x1417 )]
    public class PKArms : PlateArms
	{

		[Constructable]
		public PKArms() : base( 0x1410 )
		{
		    Name = "plate arms of the pumpkin king";
		    Hue = 1358;
			Weight = 5.0;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Infused with the awesome might of the Pumpkin King.", 1358);
        }

        public PKArms(Serial serial)
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