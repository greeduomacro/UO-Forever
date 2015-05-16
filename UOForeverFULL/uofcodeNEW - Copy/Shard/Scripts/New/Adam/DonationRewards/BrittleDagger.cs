using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	[FlipableAttribute( 0xF52, 0xF51 )]
	public class BrittleDaggerDonation : Dagger
	{
        public override bool AllowEquippedCast(Mobile from)
        {
            return true;
        }

		[Constructable]
		public BrittleDaggerDonation()
		{
			Weight = 1.0;
            LootType = LootType.Blessed;
		    Name = "a rusted dagger";
		    Hue = 1172;
		    Identified = true;
		}

        public BrittleDaggerDonation(Serial serial)
            : base(serial)
		{
		}

	    public override bool CanSwing(Mobile attacker, Mobile defender)
	    {
	        attacker.SendMessage(137, "Attacking anything with this dagger would break it!");
	        return false;
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