using System;
using Server;

namespace Server.Items
{
	public class BoneAsh : BaseReagent
	{
		public override int DescriptionNumber { get { return 0; } }
		public override string DefaultName{ get{ return "Bone Ash"; } }

		[Constructable]
		public BoneAsh() : this( 1 )
		{
		}

		[Constructable]
		public BoneAsh( int amount ) : base( 0x2DB5, amount )
		{
		}

		public BoneAsh( Serial serial ) : base( serial )
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