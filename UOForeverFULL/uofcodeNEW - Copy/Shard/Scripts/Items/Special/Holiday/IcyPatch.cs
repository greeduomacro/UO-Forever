using System;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public class IcyPatch : Item
	{
		public override int LabelNumber { get { return 1095159; } } //An Icy Patch
		public override double DefaultWeight { get { return 5.0; } }

		/* On OSI, the IcyPatch with itemid 0x122a is "rarer", so we will give it 1:10 chance of creating it that way */

		[Constructable]
		public IcyPatch() : this ( Utility.RandomDouble() < 0.10 ? 0x122A : 0x122F )
		{
		}

		public IcyPatch( int itemid ) : base ( itemid )
		{
			Hue = 0x481;
		}

		public IcyPatch( Serial serial ) : base( serial )
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( m.Player && m.Alive && m.AccessLevel == AccessLevel.Player )
			{
				switch ( Utility.Random( 3 ) )
				{
					case 0: m.SendLocalizedMessage( 1095160 ); break; //You steadily walk over the slippery surface.
					case 1: BalanceSequence( m ); break;
					default: SplatterSequence( m ); break;
				}
			}

			return base.OnMoveOver( m );
		}

		public void BalanceSequence( Mobile m )
		{
			m.Frozen = true;
			Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( 1.25 ), new TimerStateCallback<Mobile>( EndFall_Callback ), m );

			m.SendLocalizedMessage( 1095161 ); //You skillfully manage to maintain your balance.
			if ( !m.Mounted )
				m.Animate( 17, 1, 1, false, true, 0 );

			m.PlaySound( m.Female ? 0x319 : 0x429 );
		}

		public void SplatterSequence( Mobile m )
		{
			m.Frozen = true;
			Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( 1.25 ), new TimerStateCallback<Mobile>( EndFall_Callback ), m );

			m.SendLocalizedMessage( 1095162 ); //You lose your footing and ungracefully splatter on the ground.

			if ( m.Mounted )
				m.Mount.Rider = null;

			Point3D p = new Point3D( m.Location );

			if ( SpellHelper.FindValidSpawnLocation( m.Map, ref p, true ) )
				m.Location = p;

			m.Animate( 21 + Utility.Random( 2 ), 1, 1, false, true, 0 );

			m.PlaySound( m.Female ? 0x317 : 0x426 );
		}

		private static void EndFall_Callback( Mobile from )
		{
			from.Frozen = false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}