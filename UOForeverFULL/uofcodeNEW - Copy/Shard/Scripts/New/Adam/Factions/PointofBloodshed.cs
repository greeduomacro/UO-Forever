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
	public class ForgiveObelisk : FactionObelisk
	{		
	    [Constructable]
		public ForgiveObelisk() : base(0x1184)
		{
		    Name = "point of bloodshed";
	        ObeliskType = ObeliskType.Bloodshed;
		}

        public ForgiveObelisk(Serial serial)
            : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
	}
}