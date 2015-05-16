


using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x2A75, 0x2A76 )] 
	public class BlueMountedPixie : Item
	{
		[Constructable]
		public BlueMountedPixie() : base( 0x2A75 )
		{
			Name = "Mounted Pixie";
		}

		public BlueMountedPixie( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x2A71, 0x2A72 )] 
	public class GreenMountedPixie : Item
	{
		[Constructable]
		public GreenMountedPixie() : base( 0x2A71 )
		{
			Name = "Mounted Pixie";
		}

		public GreenMountedPixie( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x2A77, 0x2A78 )] 
	public class GreenMountedPixie2 : Item
	{
		[Constructable]
		public GreenMountedPixie2() : base( 0x2A77 )
		{
			Name = "Mounted Pixie";
		}

		public GreenMountedPixie2( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x2A73, 0x2A74 )] 
	public class OrangeMountedPixie : Item
	{
		[Constructable]
		public OrangeMountedPixie() : base( 0x2A73 )
		{
			Name = "Mounted Pixie";
		}

		public OrangeMountedPixie( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x2A79, 0x2A7A )] 
	public class PinkMountedPixie : Item
	{
		[Constructable]
		public PinkMountedPixie() : base( 0x2A79 )
		{
			Name = "Mounted Pixie";
		}

		public PinkMountedPixie( Serial serial ) : base( serial )
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

}