using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;

namespace Server.Mobiles
{
	[CorpseName( "a spartan corpse" )]
	public class Spartan : BaseCreature
	{
		private Mobile m_Owner;
		private DateTime m_ExpireTime;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner
		{
			get{ return m_Owner; }
			set{ m_Owner = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime ExpireTime
		{
			get{ return m_ExpireTime; }
			set{ m_ExpireTime = value; }
		}

		[Constructable]
		public Spartan() : this( null )
		{
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public override void DisplayPaperdollTo(Mobile to)
		{
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			for ( int i = 0; i < list.Count; ++i )
			{
				if ( list[i] is ContextMenus.PaperdollEntry )
					list.RemoveAt( i-- );
			}
		}

		public Spartan( Mobile owner ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			m_Owner = owner;
			m_ExpireTime = DateTime.UtcNow + TimeSpan.FromMinutes( 1.0 );

			Name = "Spartan";
			Body = 400;

			SetStr( 2010, 3000 );
			SetDex( 710, 900 );
			SetInt( 2000, 2500 );

			SetHits( 1210, 1800 );

			SetDamage( 20, 30 );

			

			
			
			
			
			

			SetSkill( SkillName.Anatomy, 100, 100 );
			SetSkill( SkillName.EvalInt, 100, 100.0 );
			SetSkill( SkillName.Magery, 100, 100.0 );
			SetSkill( SkillName.Meditation, 100, 100.0 );
			SetSkill( SkillName.MagicResist, 140.1, 150.0 );
			SetSkill( SkillName.Tactics, 100, 100 );
			SetSkill( SkillName.Wrestling, 100, 100 );

			Spear weapon = new Spear();
			weapon.Hue = 0x835;
			weapon.Movable = false;
			AddItem( weapon );

			CloseHelm helm = new CloseHelm();
			helm.Hue = 0x835;
			AddItem( helm );

			StuddedArms arms = new StuddedArms();
			arms.Hue = 0x835;
			AddItem( arms );

			StuddedGloves gloves = new StuddedGloves();
			gloves.Hue = 0x835;
			AddItem( gloves );
			
			StuddedGorget gorget = new StuddedGorget();
			gorget.Hue = 0x835;
			AddItem( gorget );

			StuddedChest tunic = new StuddedChest();
			tunic.Hue = 0x835;
			AddItem( tunic );

			StuddedLegs legs = new StuddedLegs();
			legs.Hue = 0x835;
			AddItem( legs );

			AddItem( new Boots() );

			Fame = 0;
			Karma = 0;

			VirtualArmor = 50;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
			AddLoot( LootPack.Gems );
		}
		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 1; } }

		private DateTime m_NextAbilityTime;

		public override void OnThink()
		{
			if ( DateTime.UtcNow >= m_NextAbilityTime )
			{
				KingLeonidas toBuff = null;

				foreach ( Mobile m in this.GetMobilesInRange( 8 ) )
				{
					if ( m is KingLeonidas && IsFriend( m ) && m.Combatant != null && CanBeBeneficial( m ) && m.CanBeginAction( typeof( JukaMage ) ) && InLOS( m ) )
					{
						toBuff = (KingLeonidas)m;
						break;
					}
				}

				if ( toBuff != null )
				{
					if ( CanBeBeneficial( toBuff ) && toBuff.BeginAction( typeof( Spartan ) ) )
					{
						m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.RandomMinMax( 30, 60 ) );

						toBuff.Say( true, "I shall use my armies to destroy you!" );
						this.Say( true, "Fight well my King!" );

						DoBeneficial( toBuff );

						object[] state = new object[]{ toBuff, toBuff.HitsMaxSeed, toBuff.RawStr, toBuff.RawDex };


						int toScale = toBuff.HitsMaxSeed;

						if ( toScale > 0 )
						{
							toBuff.HitsMaxSeed += AOS.Scale( toScale, 600 );
							toBuff.Hits += AOS.Scale( toScale, 600 );
						}

						toScale = toBuff.RawStr;

						if ( toScale > 0 )
							toBuff.RawStr += AOS.Scale( toScale, 100 );

						toScale = toBuff.RawDex;

						if ( toScale > 0 )
						{
							toBuff.RawDex += AOS.Scale( toScale, 100 );
							toBuff.Stam += AOS.Scale( toScale, 100 );
						}

						toBuff.Hits = toBuff.Hits;
						toBuff.Stam = toBuff.Stam;

						toBuff.FixedParticles( 0x375A, 10, 15, 5017, EffectLayer.Waist );
						toBuff.PlaySound( 0x1EE );

						Timer.DelayCall( TimeSpan.FromSeconds( 20.0 ), new TimerStateCallback( Unbuff ), state );
						
						bool expired;
	
						expired = ( DateTime.UtcNow >= m_ExpireTime );

						if ( !expired && m_Owner != null )
						expired = m_Owner.Deleted || Map != m_Owner.Map || !InRange( m_Owner, 16 );

						if ( expired )
						{
							PlaySound( GetIdleSound() );
							Delete();
					}
				}
				else
				{
					m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.RandomMinMax( 2, 5 ) );
				}
			}

			base.OnThink();
		
	            }	
		}

		private void Unbuff( object state )
		{
			object[] states = (object[])state;

			KingLeonidas toDebuff = (KingLeonidas)states[0];

			toDebuff.EndAction( typeof( Spartan ) );

			if ( toDebuff.Deleted )
				return;

			toDebuff.HitsMaxSeed = (int)states[1];
			toDebuff.RawStr = (int)states[2];
			toDebuff.RawDex = (int)states[3];

			toDebuff.Hits = toDebuff.Hits;
			toDebuff.Stam = toDebuff.Stam;
                }

		public Spartan( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}