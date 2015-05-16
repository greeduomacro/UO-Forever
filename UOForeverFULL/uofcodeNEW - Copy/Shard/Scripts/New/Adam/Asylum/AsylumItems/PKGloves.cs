using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1414, 0x1418 )]
    public class PKGloves : PlateGloves
	{
		[Constructable]
        public PKGloves()
            : base(0x1414)
		{
            Name = "plate gloves of the pumpkin king";
            Hue = 1358;
			Weight = 2.0;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Infused with the awesome might of the Pumpkin King.", 1358);
        }

        public PKGloves(Serial serial)
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