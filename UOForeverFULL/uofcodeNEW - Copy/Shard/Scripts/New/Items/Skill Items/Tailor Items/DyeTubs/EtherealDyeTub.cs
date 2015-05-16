using System;
using Server.Mobiles;

namespace Server.Items
{
	public class EtherealDyeTub : BaseDyeTub
	{
		public override TextDefinition TargetMessage{ get{ return new TextDefinition( "Select the item to dye." ); } }
		public override TextDefinition FailMessage{ get{ return new TextDefinition( "You can only dye ethereal statuettes with this tub." ); } }
		public override int LabelNumber{ get{ return 0; } } // ethereal statuette dye tub

		public override CustomHuePicker CustomHuePicker{ get{ return CustomHuePicker.SpecialDyeTub; } }

		public override string DefaultName{ get{ return "Ethereal Statuette Dye Tub"; } }

		[Constructable]
		public EtherealDyeTub() : this( 0 )
		{
		}

		[Constructable]
		public EtherealDyeTub( int hue ) : this( hue, true )
		{
		}

		[Constructable]
		public EtherealDyeTub( int hue, bool redyable ) : this( hue, true, -1 )
		{
		}

		[Constructable]
		public EtherealDyeTub( int hue, bool redyable, int uses ) : base( hue, redyable, uses )
		{
			LootType = LootType.Blessed;
		}

		public override bool Dye( Mobile from, Item item )
		{
			if ( !item.Movable )
			{
				from.SendLocalizedMessage( 1049779 ); // You cannot dye statuettes that are locked down.
				return false;
			}

			return base.Dye( from, item );
		}

		public EtherealDyeTub( Serial serial ) : base( serial )
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