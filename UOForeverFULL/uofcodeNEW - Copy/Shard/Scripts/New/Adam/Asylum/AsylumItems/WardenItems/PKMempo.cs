using System;

namespace Server.Items
{
    public class WardenMempo : PlateMempo
	{
		[Constructable]
        public WardenMempo()
            : base(0x2779)
		{
            Name = "plate mempo of the warden";
            Hue = 1157;
			Weight = 3.0;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Covered in congealing blood.", 1157);
        }

        public WardenMempo(Serial serial)
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