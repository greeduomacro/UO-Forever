using System;
using System.Collections.Generic;
using System.Linq;
using Server.Accounting;
using Server.Factions;
using Server.Mobiles;
using Server.Network;
using VitaNex.SuperGumps.UI;

namespace Server.Items
{
	public class VendorObelisk : Item
	{		
	    [Constructable]
		public VendorObelisk() : base(0x1184)
		{
		    Name = "a vendor obelisk";
			Movable = false;
		}

        public VendorObelisk(Serial serial)
            : base(serial)
		{
		}


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
	}
}