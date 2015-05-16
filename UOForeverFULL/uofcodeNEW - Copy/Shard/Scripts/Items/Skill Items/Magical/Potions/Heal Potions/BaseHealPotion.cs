using System;
using System.Collections.Generic;
using Server;
using Server.Engines.Conquests;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Server.Items
{
	public abstract class BaseHealPotion : BasePotion
	{
		public abstract int MinHeal { get; }
		public abstract int MaxHeal { get; }
		public abstract double Delay { get; }

		public BaseHealPotion( PotionEffect effect ) : base( 0xF0C, effect )
		{
		}

		public BaseHealPotion( Serial serial ) : base( serial )
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

		public void DoHeal( Mobile from )
		{
			int min = Scale( from, MinHeal );
			int max = Scale( from, MaxHeal );

			int toHeal = Utility.RandomMinMax( min, max );

			int healmessage = Math.Min( from.HitsMax - from.Hits, toHeal );

			from.Heal( toHeal );

			if ( healmessage > 0 )
				from.PrivateOverheadMessage( MessageType.Regular, 0x42, false, healmessage.ToString(), from.NetState );
		}

		public override bool Drink( Mobile from )
		{
			if ( from.Hits < from.HitsMax )
			{
				if ( from.Poisoned && !from.IsT2A|| MortalStrike.IsWounded( from ) )
				{
					from.LocalOverheadMessage(MessageType.Regular, 0x22, 1005000); // You can not heal yourself in your current state.
					return false;
				}
				else
				{
					if ( from.BeginAction( typeof( BaseHealPotion ) ) )
					{
                        CustomRegion region1 = from.Region as CustomRegion;

						DoHeal( from );

						BasePotion.PlayDrinkEffect( from );

                        if (!Engines.ConPVP.DuelContext.IsFreeConsume(from) && (region1 == null || !region1.PlayingGame(from)))
                            this.Consume();
						
						Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( Delay ), new TimerStateCallback<Mobile>( ReleaseHealLock ), from );
					}
					else
					{
						from.LocalOverheadMessage( MessageType.Regular, 0x22, 500235 ); // You must wait 10 seconds before using another healing potion.
						return false;
					}
				}
			}
			else
			{
				from.SendLocalizedMessage(1049547); // You decide against drinking this potion, as you are already at full health.
				return false;
			}

			return true;
		}

		private static void ReleaseHealLock( Mobile m )
		{
			m.EndAction( typeof( BaseHealPotion ) );
		}
	}
}