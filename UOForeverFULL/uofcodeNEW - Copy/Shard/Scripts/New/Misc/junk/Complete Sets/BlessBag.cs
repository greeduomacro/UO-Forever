//RunUO script: SphericalSolaris
//Copyright (c) 2004 by SphericalSolaris <sphericalsolaris@hotmail.com>
//This script is free software; 
//you can redistribute it and/or modify it under the terms of the GNU General Public License as 
//published by the Free Software Foundation version 2 of the License applies. This program is 
//distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY without even the implied 
//warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public 
//License for more details.
//Please do NOT remove or change this header.

using System; 
using Server.Network; 

namespace Server.Items 
{ 
   	public class BlessBag : Container 
   	{ 
      	public virtual int MaxHolding{ get{ return 5; } } 
      
	public override int DefaultGumpID{ get{ return 0x3C; } }
	public override int DefaultDropSound{ get{ return 0x48; } }
	public override Rectangle2D Bounds 
      	{ 
         get{ return new Rectangle2D( 44, 65, 142, 94 ); } 
      	} 

   [Constructable]
      public BlessBag() : base( 0x9B2 )
      { 
	 Name = "Bless Bag";
         Weight = 3.0; 
         LootType = LootType.Blessed;
	 Hue = 2238; 
      } 

      public BlessBag( Serial serial ) : base( serial ) 
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

      public override bool OnDragDrop( Mobile from, Item dropped ) 
      { 
        if ( !base.OnDragDrop( from, dropped ) )
		return false;
		dropped.LootType = LootType.Blessed;
		from.SendMessage("Thee item becomes Blessed");
         //LootType = LootType.Blessed;

		return true;
      } 

      public override bool OnDragDropInto( Mobile from, Item item, Point3D p ) 
      { 
         if ( !base.OnDragDropInto( from, item, p ) )
		return false;
		item.LootType = LootType.Blessed;
		from.SendMessage("Thee item becomes Blessed");
		return true;
      } 

      //public override void OnItemRemoved(Mobile from, Item item ) 
      //{ 
		//base.OnItemRemoved( item );
			//PublicOverheadMessage( Network.MessageType.Regular, 0x3B2, Utility.Random( 1042891, 8 ) );
			//PublicOverheadMessage( "Thank You!! Come Again." );
		//from.SendMessage("Thank You! Come Again.");
     // } 

   } 
}