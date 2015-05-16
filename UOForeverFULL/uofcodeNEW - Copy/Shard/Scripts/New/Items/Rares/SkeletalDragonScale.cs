using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class SkeletalDragonTarget : Target
	{
		private SkeletalDragonScale m_Scale;

		public SkeletalDragonTarget( SkeletalDragonScale scale ) : base( 1, false, TargetFlags.None )
		{
			m_Scale = scale;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( m_Scale.Deleted )
				return;

			if ( !m_Scale.IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else if ( target is Dragon || target is WhiteWyrm )
			{
				BaseCreature pet = (BaseCreature)target;
				if ( pet.ControlMaster != from )
					from.SendMessage( "This scale will not work on that" );
				else if ( pet.Body == 104 )
					from.SendMessage( "That is already undead!" );
				else
				{
					pet.Body = 104;
					pet.BloodHue = -1;
					pet.BaseSoundID = 0x488;
					pet.PlaySound( pet.GetIdleSound() );

					if ( pet is WhiteWyrm )
						pet.Hue = 1072;
					else if ( pet.Hue > 0 && pet.Hue < 0x4001 )
						pet.Hue = 0;

					if ( String.IsNullOrEmpty( pet.Name ) || pet.Name == pet.DefaultName )
					{
						if ( pet is Dragon )
							pet.Name = "an undead dragon";
						else if ( pet is WhiteWyrm )
							pet.Name = "an undead wyrm";
					}

					m_Scale.Consume();

					from.SendMessage( "Your pet's flesh has rotted and fallen off!" );
				}
			}
			else
				from.SendMessage( "This scale will not work on that!" );
		}
	}

	public class SkeletalDragonScale : Item
	{
		public override string DefaultName{ get{ return "scale of a skeletal dragon"; } }

		[Constructable]
		public SkeletalDragonScale() : base( 0x26B4 )
		{
			Weight = 1.0;
			Hue = 2109;
			LootType = LootType.Cursed;
		}

		public SkeletalDragonScale( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
				from.Target = new SkeletalDragonTarget( this );
		}
	}
}