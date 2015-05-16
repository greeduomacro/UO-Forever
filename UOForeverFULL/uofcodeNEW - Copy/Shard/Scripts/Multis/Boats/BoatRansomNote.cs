using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Gumps;
using Server.Items;

namespace Server.Multis
{
    public class BoatRansomNote : Item
	{
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat Boat { get; set; }

        [Constructable]
		public BoatRansomNote() : base( 0x2258 )
		{
			Weight = 1.0;

            LootType = LootType.Blessed;
            Name = "a boat ransom note";
            // the rest is done via uberscript
		}

        public BoatRansomNote(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
            writer.Write(Boat);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
            Boat = reader.ReadItem() as BaseBoat;
		}
	}
}