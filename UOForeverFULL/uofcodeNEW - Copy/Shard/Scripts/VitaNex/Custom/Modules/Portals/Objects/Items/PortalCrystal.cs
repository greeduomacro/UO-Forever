using System;
using Server.Engines.XmlSpawner2;
using Server.Network;

namespace Server.Items
{
	public class PortalCrystal : Item
	{
		[Constructable]
        public PortalCrystal()
            : base(8740)
		{
		    Breakable = false;
		    DoesNotDecay = true;
		    Movable = false;
		    Name = "a crystal";
                XmlAttach.AttachTo(
                    this,
                    new XmlScript("crystal\\crystal.us")
                    {
                        Name = "crystal"
                    });
		}

        public PortalCrystal(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}