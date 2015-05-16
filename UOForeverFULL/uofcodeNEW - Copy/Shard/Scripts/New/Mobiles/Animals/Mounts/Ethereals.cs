using System;
using Server;

namespace Server.Mobiles
{
	public class EtherealFrenziedOstard : EtherealMount
	{
		public override int LabelNumber { get { return 1041299; } } // Ethereal Ostard Statuette

		[Constructable]
		public EtherealFrenziedOstard() : base( 0x2136, 0x3EA4 )
		{
		}

		public EtherealFrenziedOstard( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class EtherealForestOstard : EtherealMount
	{
		public override int LabelNumber { get { return 1041299; } } // Ethereal Ostard Statuette

		[Constructable]
		public EtherealForestOstard() : base( 0x2137, 0x3EA5 )
		{
		}

		public EtherealForestOstard( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class EtherealPolarBear : EtherealMount
	{
		public override string DefaultName{ get{ return "an ethereal polar bear"; } }

		[Constructable]
		public EtherealPolarBear() : base(8417, 16069)
		{
		}

		public EtherealPolarBear(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class EtherealSkeletalSteed : EtherealMount
	{
		public override int LabelNumber{ get{ return 1041298; } } // Ethereal skeletal steed

		[Constructable]
		public EtherealSkeletalSteed() : base( 0x2617, 0x3EBB )
		{
        }

		public EtherealSkeletalSteed( Serial serial ) : base( serial )
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

	public class EtherealLongManeHorse : EtherealMount
	{
		public override string DefaultName{ get{ return "an ethereal long mane horse"; } }

		[Constructable]
		public EtherealLongManeHorse() : base( 0x20DD, 0x3EA9 )
		{
        }

		public EtherealLongManeHorse( Serial serial ) : base( serial )
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