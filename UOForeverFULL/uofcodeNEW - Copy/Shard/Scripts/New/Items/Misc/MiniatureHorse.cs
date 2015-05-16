using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class MiniatureHorse : Item
	{
		public override string DefaultName{ get{ return "a miniature horse"; } }

		[Constructable]
		public MiniatureHorse() : base( 0x2120 )
		{
		}

		public MiniatureHorse( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				BaseCreature pet = new Horse();

				pet.SetControlMaster( from );
				pet.Direction = Direction.Down;

				pet.MoveToWorld( from.Location, from.Map );

				pet.ControlTarget = from;
				pet.ControlOrder = OrderType.Follow;

				pet.Animate( 5, 7, 1, true, false, 0 );
				//pet.PlaySound( pet.GetAttackSound() );

				Consume();
			}
			else
				from.SendLocalizedMessage( 1042010 );
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
}