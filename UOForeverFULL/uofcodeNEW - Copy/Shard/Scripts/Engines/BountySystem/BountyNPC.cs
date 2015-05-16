/*using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Guilds;

namespace Server.Mobiles
{
	public class BountyNPC : BaseShieldGuard
	{
		public override int Keyword{ get{ return 0x21; } } // *order shield*
		public override BaseShield Shield{ get{ return new OrderShield(); } }
		public override int SignupNumber{ get{ return 1007141; } } // Sign up with a guild of order if thou art interested.
		public override GuildType Type{ get{ return GuildType.Order; } }

		public override bool BardImmune{ get{ return true; } }

		[Constructable]
		public BountyNPC()
		{
			CantWalk = true;
			Blessed = true;
			Title = "the Bounty Keeper";
		}

		public BountyNPC( Serial serial ) : base( serial )
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
}*/