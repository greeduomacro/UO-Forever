using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class ThunderHide : BaseGMJewel
	{
		public override bool CastHide{ get{ return false; } }

		public override void HideEffects(Mobile from)
		{
			from.Hidden = !from.Hidden;
			from.BoltEffect( 0 );
		}

		[Constructable]
		public ThunderHide() : base(AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Hue = 1153;
			Name = "GM Thunder Ball";
		}
		public ThunderHide( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}