using Server;
using Server.Items;
using Server.Multis;
using Server.Network;
using System;

namespace Server.Items
{
	[FlipableAttribute( 0xe43, 0xe42 )]
	public class WoodenTreasureChest : BaseTreasureChest
	{
		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
				return "a locked wooden chest";

				return "a wooden chest";
			}
		}

		[Constructable]
		public WoodenTreasureChest() : base( 0xE43 )
		{
		}

		public WoodenTreasureChest( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0xe41, 0xe40 )]
	public class MetalGoldenTreasureChest : BaseTreasureChest
	{
		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
				return "a locked metal chest";

				return "a metal chest";
			}
		}

		[Constructable]
		public MetalGoldenTreasureChest() : base( 0xE41 )
		{
		}

		public MetalGoldenTreasureChest( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x9ab, 0xe7c )]
	public class MetalTreasureChest : BaseTreasureChest
	{
		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
				return "a locked metal chest";

				return "a metal chest";
			}
		}

		[Constructable]
		public MetalTreasureChest() : base( 0x9AB )
		{
		}

		public MetalTreasureChest( Serial serial ) : base( serial )
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

	[Flipable( 0xA97, 0xA99, 0xA98, 0xA9A, 0xA9B, 0xA9C )]
	public class TreasureBookcase : BaseTreasureChest
	{
		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
				return "a locked bookcase";

				return "a bookcase";
			}
		}

		[Constructable]
		public TreasureBookcase() : base( 0xA97 )
		{
		}

		public TreasureBookcase( Serial serial ) : base( serial )
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

	[Flipable( 0xE3D, 0xE3C )]
	public class LargeTreasureCrate : BaseTreasureChest
	{
		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
				return "a locked crate";

				return "a crate";
			}
		}

		[Constructable]
		public LargeTreasureCrate() : base( 0xE3D )
		{
		}

		public LargeTreasureCrate( Serial serial ) : base( serial )
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

	[Flipable( 0x9A9, 0xE7E )]
	public class SmallTreasureCrate : BaseTreasureChest
	{
		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
				return "a locked crate";

				return "a crate";
			}
		}

		[Constructable]
		public SmallTreasureCrate() : base( 0x9A9 )
		{
		}

		public SmallTreasureCrate( Serial serial ) : base( serial )
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

	[Flipable( 0x9AA, 0xE7D )]
	public class WoodenTreasureBox : BaseTreasureChest
	{
		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
				return "a locked wooden box";

				return "a wooden box";
			}
		}

		[Constructable]
		public WoodenTreasureBox() : base( 0x9AA )
		{
		}

		public WoodenTreasureBox( Serial serial ) : base( serial )
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

	[Flipable( 0x9A8, 0xE80 )]
	public class MetalTreasureBox : BaseTreasureChest
	{
		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
				return "a locked metal box";

				return "a metal box";
			}
		}

		[Constructable]
		public MetalTreasureBox() : base( 0x9A8 )
		{
		}

		public MetalTreasureBox( Serial serial ) : base( serial )
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

	public class TreasureBarrel : BaseTreasureChest
	{
		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
				return "a sealed barrel";

				return "a barrel";
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override bool Locked
		{
			get { return base.Locked; }
			set
			{
				base.Locked = value;
				if ( value )
					ItemID = 0xFAE;
				else
					ItemID = 0x0E77;
			}
		}

		[Constructable]
		public TreasureBarrel() : base( 0xFAE )
		{
		}

		public TreasureBarrel( Serial serial ) : base( serial )
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