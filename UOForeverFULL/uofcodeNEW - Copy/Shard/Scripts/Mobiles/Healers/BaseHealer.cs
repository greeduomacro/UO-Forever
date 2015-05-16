using System;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Games;

namespace Server.Mobiles
{
	public abstract class BaseHealer : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public override bool IsActiveVendor{ get{ return false; } }
		public override bool IsInvulnerable{ get{ return false; } }

		public override void InitSBInfo()
		{
		}

		public BaseHealer() : base( null )
		{
			if ( !IsInvulnerable )
			{
				AI = AIType.AI_Mage;
				ActiveSpeed = 0.2;
				PassiveSpeed = 0.8;
				RangePerception = BaseCreature.DefaultRangePerception;
				FightMode = FightMode.Aggressor;
			}

			SpeechHue = 0;

			SetStr( 34, 40 );
			SetDex( 52, 60 );
			SetInt( 84, 90 );

			SetDamage( 10, 23 );			

			SetSkill( SkillName.Anatomy, 75.0, 97.5 );
			SetSkill( SkillName.EvalInt, 82.0, 100.0 );
			SetSkill( SkillName.Healing, 75.0, 97.5 );
			SetSkill( SkillName.Magery, 82.0, 100.0 );
			SetSkill( SkillName.MagicResist, 82.0, 100.0 );
			SetSkill( SkillName.Tactics, 82.0, 100.0 );

			Fame = 100;
			Karma = 100;

		//	PackItem( new Bandage( Utility.RandomMinMax( 5, 10 ) ) );
		//	PackItem( new HealPotion() );
		//	PackItem( new CurePotion() );
		}

		public override VendorShoeType ShoeType{ get{ return VendorShoeType.Sandals; } }

		public virtual int GetRobeColor()
		{
			return Utility.RandomYellowHue();
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Robe( GetRobeColor() ) );
		}

		public virtual bool HealsYoungPlayers{ get{ return true; } }

		public virtual bool CheckResurrect( Mobile m )
		{
			return true;
		}

		private DateTime m_NextResurrect;
		private static TimeSpan ResurrectDelay = TimeSpan.FromSeconds( 2.0 );

		public virtual void OfferResurrection( Mobile m )
		{
			Direction = GetDirectionTo( m );

			m.PlaySound( 0x1F2 );
			m.FixedEffect( 0x376A, 10, 16 );

			m.CloseGump( typeof( ResurrectGump ) );
			//PlayerMobile pres = m as PlayerMobile;

			/*if (pres.MurderBounty > 0)
				m.SendGump( new ResurrectGump( m, this, pres.MurderBounty ) );  //removed for bounty system removal
			else*/
				m.SendGump( new ResurrectGump( m, this) );
		}

		public virtual void OfferHeal( PlayerMobile m )
		{
			Direction = GetDirectionTo( m );

			if ( m.CheckYoungHealTime() )
			{
				Say( 501229 ); // You look like you need some healing my child.

				m.PlaySound( 0x1F2 );
				m.FixedEffect( 0x376A, 9, 32 );

				m.Hits = m.HitsMax;
			}
			else
			{
				Say( 501228 ); // I can do no more for you at this time.
			}
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( !m.Frozen && DateTime.UtcNow >= m_NextResurrect && InRange( m, 4 ) && !InRange( oldLocation, 4 ) && InLOS( m ) )
			{
				if ( !m.Alive )
				{
					m_NextResurrect = DateTime.UtcNow + ResurrectDelay;

					if ( m.Map == null || !m.Map.CanFit( m.Location, 16, false, false ) )
					{
						m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
					}
					else if ( CheckResurrect( m ) )
					{
                        if (!CreaturePossession.IsInHouseOrBoat(m.Location, m.Map))
                        {
                            OfferResurrection(m);
                        }
					}
				}
				else if ( this.HealsYoungPlayers && m.Hits < m.HitsMax && m is PlayerMobile && ((PlayerMobile)m).Young )
				{
					OfferHeal( (PlayerMobile) m );
				}
			}
		}

		public BaseHealer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			if ( !IsInvulnerable )
			{
				AI = AIType.AI_Mage;
				ActiveSpeed = 0.2;
				PassiveSpeed = 0.8;
				RangePerception = BaseCreature.DefaultRangePerception;
				FightMode = FightMode.Aggressor;
			}
		}
	}
}