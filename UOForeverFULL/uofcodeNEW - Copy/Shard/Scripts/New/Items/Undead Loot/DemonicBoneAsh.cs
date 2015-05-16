using System;
using Server;

namespace Server.Items
{
	public class DemonicBoneAsh : BaseReagent
	{
		public override int DescriptionNumber { get { return 0; } }
		public override string DefaultName{ get{ return "Demonic Bone Ash"; } }

		[Constructable]
		public DemonicBoneAsh() : this( 1 )
		{
		}

		[Constructable]
		public DemonicBoneAsh( int amount ) : base( 0x2DB1, amount )
		{
		}

		public DemonicBoneAsh( Serial serial ) : base( serial )
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