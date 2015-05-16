using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class MouldyCheese : Item
	{
		[Constructable]
		public MouldyCheese() : base( 0x97D )
		{
			Hue = 768;
			Weight = 2.0;
			Name = "mouldy cheese";
		}

		public MouldyCheese( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				if ( !from.CanBeginAction( typeof( Spells.Fifth.IncognitoSpell ) ) )
					from.SendMessage( "You decide against eating it while in incognito." );
				else if ( !from.CanBeginAction( typeof( Spells.Seventh.PolymorphSpell ) ) )
					from.SendMessage( "You can not be polymorphed at this time." );
				//else if ( TransformationSpellHelper.UnderTransformation( Caster, typeof( WraithFormSpell ) ) )
				//	from.SendMessage( "You decide against eating it while polymorphed." );
				else if ( from.IsBodyMod || from.FindItemOnLayer( Layer.Helm ) is OrcishKinMask )
					from.SendMessage( "You decide against eating it while in your disguise." );
				else
				{
					if ( from is PlayerMobile )
					{
						PlayerMobile pm = from as PlayerMobile;
						if ( pm.SavagePaintExpiration > TimeSpan.Zero )
							from.SendMessage( "You already have the markings of another tribe." );
						else
						{
							from.BodyMod = ( from.Female ? 186 : 185 );
							from.HueMod = 0;

							pm.SavagePaintExpiration = TimeSpan.FromDays( 7.0 );

							//from.SendLocalizedMessage( 1042537 ); // You now bear the markings of the savage tribe.  Your body paint will last about a week or you can remove it with an oil cloth.
							from.SendMessage( "You now bear the markings of the undead pirates.  This will last about a week." );
							from.PlaySound( Utility.Random( 0x3A, 3 ) );
							if ( from.Body.IsHuman && !from.Mounted )
								from.Animate( 34, 5, 1, true, false, 0 );
							from.ApplyPoison( from, Poison.Greater );
							Consume();
						}
					}
				}
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}