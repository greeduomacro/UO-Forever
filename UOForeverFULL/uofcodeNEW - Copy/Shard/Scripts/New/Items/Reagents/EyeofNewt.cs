using System;
using Server;

namespace Server.Items
{
	public class EyeofNewt : BaseReagent
	{
		[Constructable]
		public EyeofNewt() : this( 1 )
		{
		}

		[Constructable]
		public EyeofNewt( int amount ) : base( 0xF87, amount )
		{
		}

		public EyeofNewt( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}