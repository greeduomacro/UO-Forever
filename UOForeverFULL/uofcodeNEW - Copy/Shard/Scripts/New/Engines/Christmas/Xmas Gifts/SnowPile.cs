using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class XmasSnowPile : BaseHolidayGift
	{
		[Constructable]
		public XmasSnowPile(int year)
		{
            ItemID = 0x913;

            HolidayName = "Christmas";
            HolidayYear = year;

			Name = "a pile of snow";
			Hue = 0x481;

			Weight = 1.0;
			LootType = LootType.Blessed;
		}

        public XmasSnowPile(Serial serial)
            : base(serial)
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042010 ); // You must have the object in your backpack to use it.
			}
            else if (from.CanBeginAction(typeof(XmasSnowPile)))
			{
				from.SendLocalizedMessage( 1005575 ); // You carefully pack the snow into a ball...
				from.Target = new SnowTarget( from, this );
			}
			else
			{
				from.SendLocalizedMessage( 1005574 ); // The snow is not ready to be packed yet.  Keep trying.
			}
		}

		private class InternalTimer : Timer
		{
			private Mobile m_From;

			public InternalTimer( Mobile from ) : base( TimeSpan.FromSeconds( 5.0 ) )
			{
				m_From = from;
			}

			protected override void OnTick()
			{
                m_From.EndAction(typeof(XmasSnowPile));
			}
		}

		private class SnowTarget : Target
		{
			private Mobile m_Thrower;
			private Item m_Snow;

			public SnowTarget( Mobile thrower, Item snow ) : base ( 10, false, TargetFlags.None )
			{
				m_Thrower = thrower;
				m_Snow = snow;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				if ( target == from )
				{
					from.SendLocalizedMessage( 1005576 ); // You can't throw this at yourself.
				}
				else if ( target is Mobile )
				{
					Mobile targ = (Mobile) target;
					Container pack = targ.Backpack;

                    if (from.BeginAction(typeof(XmasSnowPile)))
					{
						new InternalTimer( from ).Start();

						from.PlaySound( 0x145 );

						from.Animate( 9, 1, 1, true, false, 0 );

						targ.SendLocalizedMessage( 1010572 ); // You have just been hit by a snowball!
						from.SendLocalizedMessage( 1010573 ); // You throw the snowball and hit the target!

						Effects.SendMovingEffect( from, targ, 0x36E4, 7, 0, false, true, 0x480, 0 );
					}

					else
					{
						from.SendLocalizedMessage( 1005574 ); // The snow is not ready to be packed yet.  Keep trying.
					}
				}
			}
		}
	}
}