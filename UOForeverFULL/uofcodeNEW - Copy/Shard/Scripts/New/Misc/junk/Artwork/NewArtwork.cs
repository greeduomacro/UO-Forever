using System;
using Server;

namespace Server.Items
{
	public class SnSten1 : Item
	{
		[Constructable]
		public SnSten1() : base( 0x3FFD ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stone";
		}

		public SnSten1( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

}

	public class Sten1 : Item
	{
		[Constructable]
		public Sten1() : base( 0x3FFC ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stone";
		}

		public Sten1( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Sten2 : Item
	{
		[Constructable]
		public Sten2() : base( 0x3FFB ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stone";
		}

		public Sten2( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Sten3 : Item
	{
		[Constructable]
		public Sten3() : base( 0x3FFa ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stone";
		}

		public Sten3( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Sten4 : Item
	{
		[Constructable]
		public Sten4() : base( 0x3FF9 ) // It isn't flipable
		{
			Weight = 3.0;
Name = "a stone";
		}

		public Sten4( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	

	public class Sten5 : Item
	{
		[Constructable]
		public Sten5() : base( 0x3FF8 ) // It isn't flipable
		{
			Weight = 3.0;
Name = "a stone";
		}

		public Sten5( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Sten6 : Item
	{
		[Constructable]
		public Sten6() : base( 0x3FF7 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stone";
		}

		public Sten6( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Sten7 : Item
	{
		[Constructable]
		public Sten7() : base( 0x3FF6 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stone";
		}

		public Sten7( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Sten8 : Item
	{
		[Constructable]
		public Sten8() : base( 0x3FF5 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stone";
		}

		public Sten8( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Sten9 : Item
	{
		[Constructable]
		public Sten9() : base( 0x3FF4 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stone";
		}

		public Sten9( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}



	public class Sten10 : Item
	{
		[Constructable]
		public Sten10() : base( 0x3FF3 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stone";
		}

		public Sten10( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Gran11 : Item
	{
		[Constructable]
		public Gran11() : base( 0x3FF2 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a tree";
		}

		public Gran11( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Stubbe12 : Item
	{
		[Constructable]
		public Stubbe12() : base( 0x3FF1 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stump";
		}

		public Stubbe12( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Gran13 : Item
	{
		[Constructable]
		public Gran13() : base( 0x3FF0 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a tree";
		}

		public Gran13( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Stubbe14 : Item
	{
		[Constructable]
		public Stubbe14() : base( 0x3FEF ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a stump";
		}

		public Stubbe14( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Gran15 : Item
	{
		[Constructable]
		public Gran15() : base( 0x3FEE ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a tree";
		}

		public Gran15( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Tree16 : Item
	{
		[Constructable]
		public Tree16() : base( 0x3FED ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a tree";
		}

		public Tree16( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Gran17 : Item
	{
		[Constructable]
		public Gran17() : base( 0x3FEC ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a tree";
		}

		public Gran17( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Gran18 : Item
	{
		[Constructable]
		public Gran18() : base( 0x3FEB ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a tree";
		}

		public Gran18( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}


	public class Tree19 : Item
	{
		[Constructable]
		public Tree19() : base( 0x3FEA ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a tree";
		}

		public Tree19( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Bulletinboard20 : Item
	{
		[Constructable]
		public Bulletinboard20() : base( 0x3FE9 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a bulletin board";
		}

		public Bulletinboard20( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Gran21 : Item
	{
		[Constructable]
		public Gran21() : base( 0x3FE8 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a tree";
		}

		public Gran21( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Fruitbarrel22 : Item
	{
		[Constructable]
		public Fruitbarrel22() : base( 0X3FE7 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a barrel of fruit";
		}

		public Fruitbarrel22( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Fruitbarrel23 : Item
	{
		[Constructable]
		public Fruitbarrel23() : base( 0x3FE6 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a barrel of fruit";
		}

		public Fruitbarrel23( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Gravestone24 : Item
	{
		[Constructable]
		public Gravestone24() : base( 0x3FE5 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a gravestone";
		}

		public Gravestone24( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Gravestone25 : Item
	{
		[Constructable]
		public Gravestone25() : base( 0x3F3E ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a gravestone";
		}

		public Gravestone25( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Barrel26 : Item
	{
		[Constructable]
		public Barrel26() : base( 0x3F3D ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a barrel of fruit";
		}

		public Barrel26( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Waterthrough27 : Item
	{
		[Constructable]
		public Waterthrough27() : base( 0x3F3C ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a water through";
		}

		public Waterthrough27( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Waterthrough28 : Item
	{
		[Constructable]
		public Waterthrough28() : base( 0x3F3B ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a water through";
		}

		public Waterthrough28( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
		public class Waterthrough29 : Item
	{
		[Constructable]
		public Waterthrough29() : base( 0x3F3A ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a water through";
		}

		public Waterthrough29( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Waterthrough30 : Item
	{
		[Constructable]
		public Waterthrough30() : base( 0x3F39 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a water through";
		}

		public Waterthrough30( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Signpost31 : Item
	{
		[Constructable]
		public Signpost31() : base( 0x3F38 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a signpost";
		}

		public Signpost31( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Signpost32 : Item
	{
		[Constructable]
		public Signpost32() : base( 0x3F37 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a signpost";
		}

		public Signpost32( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Anvil33 : Item
	{
		[Constructable]
		public Anvil33() : base( 0x3F36 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "an anvil";
		}

		public Anvil33( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
	
		public class Foodtray34 : Item
	{
		[Constructable]
		public Foodtray34() : base( 0x3F35 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a foodtray";
		}

		public Foodtray34( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Foodtray35 : Item
	{
		[Constructable]
		public Foodtray35() : base( 0x3F34 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a foodtray";
		}

		public Foodtray35( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Foodtray36 : Item
	{
		[Constructable]
		public Foodtray36() : base( 0x3F33 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a foodtray";
		}

		public Foodtray36( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Foodtray37 : Item
	{
		[Constructable]
		public Foodtray37() : base( 0x3F32 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a foodtray";
		}

		public Foodtray37( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Foodtray38 : Item
	{
		[Constructable]
		public Foodtray38() : base( 0x3F31 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a foodtray";
		}

		public Foodtray38( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
		public class Foodtray39 : Item
	{
		[Constructable]
		public Foodtray39() : base( 0x3F30 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a foodtray";
		}

		public Foodtray39( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Foodtray40 : Item
	{
		[Constructable]
		public Foodtray40() : base( 0x3F2F ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a foodtray";
		}

		public Foodtray40( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Foodtray41 : Item
	{
		[Constructable]
		public Foodtray41() : base( 0x3F2E ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a foodtray";
		}

		public Foodtray41( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Foodtray42 : Item
	{
		[Constructable]
		public Foodtray42() : base( 0x3F2D ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a foodtray";
		}

		public Foodtray42( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Fruitbarrel43 : Item
	{
		[Constructable]
		public Fruitbarrel43() : base( 0x3F2C ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a barrel of fruit";
		}

		public Fruitbarrel43( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
	
	public class Fruitbarrel44 : Item
	{
		[Constructable]
		public Fruitbarrel44() : base( 0x3F2B ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a barrel of fruit";
		}

		public Fruitbarrel44( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Fruitbarrel45 : Item
	{
		[Constructable]
		public Fruitbarrel45() : base( 0x3F2A ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a barrel of fruit";
		}

		public Fruitbarrel45( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Spellbook46 : Item
	{
		[Constructable]
		public Spellbook46() : base( 0x3F29 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a spellbook";
		}

		public Spellbook46( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Linne47 : Item
	{
		[Constructable]
		public Linne47() : base( 0x3F28 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "linnen";
		}

		public Linne47( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Linne48 : Item
	{
		[Constructable]
		public Linne48() : base( 0x3F27 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "linnen";
		}

		public Linne48( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
	
	public class Linne49 : Item
	{
		[Constructable]
		public Linne49() : base( 0x3F26 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "linnen";
		}

		public Linne49( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Linne50 : Item
	{
		[Constructable]
		public Linne50() : base( 0x3F25 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "linnen";
		}

		public Linne50( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Map51 : Item
	{
		[Constructable]
		public Map51() : base( 0x3F24 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a map";
		}

		public Map51( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Map52 : Item
	{
		[Constructable]
		public Map52() : base( 0x3F23 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a map";
		}

		public Map52( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Map53 : Item
	{
		[Constructable]
		public Map53() : base( 0x3F22 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "a map";
		}

		public Map53( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
	
	
	public class Plant54 : Item
	{
		[Constructable]
		public Plant54() : base( 0x3F21 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant54( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant55 : Item
	{
		[Constructable]
		public Plant55() : base( 0x3F20 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant55( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant56 : Item
	{
		[Constructable]
		public Plant56() : base( 0x3F1F ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant56( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant57 : Item
	{
		[Constructable]
		public Plant57() : base( 0x3F1E ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant57( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant58 : Item
	{
		[Constructable]
		public Plant58() : base( 0x3F1D ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant58( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
	
	public class Plant59 : Item
	{
		[Constructable]
		public Plant59() : base( 0x3F1C ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant59( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant60 : Item
	{
		[Constructable]
		public Plant60() : base( 0x3F1B ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant60( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant61 : Item
	{
		[Constructable]
		public Plant61() : base( 0x3F1A ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant61( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant62 : Item
	{
		[Constructable]
		public Plant62() : base( 0x3F19 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant62( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant63 : Item
	{
		[Constructable]
		public Plant63() : base( 0x3F18 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant63( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
	
	public class Plant64 : Item
	{
		[Constructable]
		public Plant64() : base( 0x3F17 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant64( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant65 : Item
	{
		[Constructable]
		public Plant65() : base( 0x3F16 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant65( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant66 : Item
	{
		[Constructable]
		public Plant66() : base( 0x3F15 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant66( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant67 : Item
	{
		[Constructable]
		public Plant67() : base( 0x3F14 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant67( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant68 : Item
	{
		[Constructable]
		public Plant68() : base( 0x3F13 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant68( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
	
	public class Plant69 : Item
	{
		[Constructable]
		public Plant69() : base( 0x3F12 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant69( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant70 : Item
	{
		[Constructable]
		public Plant70() : base( 0x3F11 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant70( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant71 : Item
	{
		[Constructable]
		public Plant71() : base( 0x3F10 ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant71( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant72 : Item
	{
		[Constructable]
		public Plant72() : base( 0x3F0F ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant72( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class Plant73 : Item
	{
		[Constructable]
		public Plant73() : base( 0x3F0E ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant73( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
		public class Plant74 : Item
	{
		[Constructable]
		public Plant74() : base( 0x3F0D ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant74( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
		public class Plant75 : Item
	{
		[Constructable]
		public Plant75() : base( 0x3F0C ) // It isn't flipable
		{
			Weight = 3.0;
			Name = "potted plant";
		}

		public Plant75( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	
