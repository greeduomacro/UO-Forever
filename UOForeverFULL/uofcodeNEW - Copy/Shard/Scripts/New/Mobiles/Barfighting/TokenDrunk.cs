using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
	public class TokenDrunk : BaseCreature
	{
		[Constructable]
		public TokenDrunk() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "male" );
			Title = "the drunk";
			Hue = Utility.RandomSkinHue();
			BodyValue = 400;
			Blessed = true;

			SetStr( 100 );
			SetDex( 100 );
			SetInt( 100 );

			SetHits( 100 );

			AddItem( new Shirt() );
			AddItem( new Shoes() );
			AddItem( new LongPants() );

			m_NextStealTime = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.RandomMinMax( 10, 15 ) );
			m_NextBabbleTime = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.RandomMinMax( 10, 40 ) );
		}

		public override bool DisallowAllMoves{ get{ return true; } }

		private DateTime m_NextStealTime;
		private DateTime m_NextBabbleTime;

		public override void OnThink()
		{
			if ( DateTime.UtcNow >= m_NextStealTime )
			{
				m_NextStealTime = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.RandomMinMax( 5, 10 ) );
				ArrayList list = new ArrayList();

				foreach ( Mobile m in this.GetMobilesInRange( 1 ) )
				{
					if ( m == this || CanBeHarmful( m ) )
						continue;
					if ( m.Player && m.AccessLevel == AccessLevel.Player )
						list.Add( m );
				}

				foreach ( Mobile m in list )
				{
					m.PlaySound(Utility.RandomList( 62, 63, 65, 910, 911, 912 ) );

					m.SendMessage( "You got too close to the drunk and he hits you with an empty whiskey bottle" );

					m.Damage( 15, this );

					if ( m.BankBox.ConsumeTotal(typeof( Gold ), 10, true) )
					{
						m.SendMessage( "and steals 10 gold from your bank account!" );
					}
					else
					{
						m.Damage( 10, this );
						m.SendMessage( "and after discovering you have no gold, hits you again!" );
					}
				}
			}

			if ( DateTime.UtcNow >= m_NextBabbleTime )
			{
				m_NextBabbleTime = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.RandomMinMax( 10, 40 ) );

				string[] babble = {"Thinking you can go a round with me lad?",
 						"Your all a bunch of tossers and shapeshaggers!",
 						"Thinking you can go a round with me wee lad?",
						"Hope you brought a clean pair of underwear, you little girl!",
						"I think you'd be a match for my sister!",
						"Why Aye Man!",
						"You're my boy blue!",
						"Gimmi a mug of your worst grog you lousy landlord!",
						"So I said to 'er....mmm well I can't remember now, but it was damn rude!",
						"Look 'ere lass, if you don't come 'ere and gimmi a kiss I'm gonna bust ya nose!",
						"So ya see if you take this bottle of whiskey...which is now empty...'ere be a sport and by me a full one!",
						"Now either I'm very drunk or your one sexy dog!" };
				Say(babble[Utility.Random(12)] );
			}
			base.OnThink();
		}

		public TokenDrunk( Serial serial ) : base( serial )
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
	}
}