using System; 
using Server; 
using Server.Items;
using Server.Gumps; 
using Server.Network; 
using Server.Menus; 
using Server.Mobiles;
using Server.Menus.Questions;

namespace Server.Items
{ 
	public class BankStone : Item 
	{ 

		[Constructable] 
		public BankStone() : base( 3796 ) 
		{ 
			Movable = false; 
			Hue = 0x485; 
			Name = "Bank Stone"; 
		} 

		public override void OnDoubleClick( Mobile from ) 
		{ 
			//from.Handled = true;

			BankBox box = from.BankBox;

			if ( box != null )
				box.Open();

     	 	} 
 
		public BankStone( Serial serial ) : base( serial ) 
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