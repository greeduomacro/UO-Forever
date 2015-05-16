using System;
using Server.Mobiles;

namespace Server.Items
{
	public class SpellbookDyeTub : BaseDyeTub
	{
		public override TextDefinition TargetMessage{ get{ return new TextDefinition( "Select the spellbook to dye." ); } }
		public override TextDefinition FailMessage{ get{ return new TextDefinition( "You can only dye spellbooks with this tub." ); } }
		public override int LabelNumber{ get{ return 0; } } // spellbook dye tub

		public override CustomHuePicker CustomHuePicker{ get{ return CustomHuePicker.SpecialDyeTub; } }

		public override string DefaultName{ get{ return "Spellbook Dye Tub"; } }

		[Constructable]
		public SpellbookDyeTub() : this( 0 )
		{
		}

		[Constructable]
		public SpellbookDyeTub( int hue ) : this( hue, true )
		{
		}

		[Constructable]
		public SpellbookDyeTub( int hue, bool redyable ) : this( hue, true, -1 )
		{
		}

		[Constructable]
		public SpellbookDyeTub( int hue, bool redyable, int uses ) : base( hue, redyable, uses )
		{
		}

		public override bool Dye( Mobile from, Item item )
		{
			if ( !item.Movable )
				from.SendMessage( "You cannot dye spellbooks which are locked down." );
			else if ( item.Parent is Mobile )
				from.SendMessage( "You cannot dye spellbooks that are being worn." );
			else if ( item.Dye( from, this ) )
			{
				from.PlaySound( 0x23E );
				return true;
			}
			else
				TextDefinition.SendMessageTo( from, FailMessage );

			return false;
		}

		public SpellbookDyeTub( Serial serial ) : base( serial )
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