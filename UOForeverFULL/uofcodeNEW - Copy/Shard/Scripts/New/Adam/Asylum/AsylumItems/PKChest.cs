using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1415, 0x1416 )]
	public class PKChest : PlateChest
	{
		[Constructable]
		public PKChest() : base( 0x1415 )
		{
            Name = "plate chest of the pumpkin king";
            Hue = 1358;
			Weight = 10.0;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Infused with the awesome might of the Pumpkin King.", 1358);
        }

        public PKChest(Serial serial)
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