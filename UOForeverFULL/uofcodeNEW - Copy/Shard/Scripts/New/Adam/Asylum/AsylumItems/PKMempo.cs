using System;

namespace Server.Items
{
    public class PKMempo : PlateMempo
	{
		[Constructable]
        public PKMempo()
            : base(0x2779)
		{
            Name = "plate mempo of the pumpkin king";
            Hue = 1358;
			Weight = 3.0;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Infused with the awesome might of the Pumpkin King.", 1358);
        }

        public PKMempo(Serial serial)
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