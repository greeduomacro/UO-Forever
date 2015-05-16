using System;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
	public class Fists : BaseMeleeWeapon
	{
		public static void Initialize()
		{
			Mobile.DefaultWeapon = new Fists();

			EventSink.DisarmRequest += new DisarmRequestEventHandler( EventSink_DisarmRequest );
			EventSink.StunRequest += new StunRequestEventHandler( EventSink_StunRequest );
		}

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.Disarm; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int OldStrengthReq{ get{ return 0; } }
		public override int NewMinDamage{ get{ return 1; } }
		public override int NewMaxDamage{ get{ return 8; } }
//		public override int DiceDamage{ get{ return Utility.Dice( 1, 8, 0 ); } } // 1d8 + 0 (1-8)
		public override int OldSpeed{ get{ return 30; } }

		public override int DefHitSound{ get{ return -1; } }
		public override int DefMissSound{ get{ return -1; } }

		public override SkillName DefSkill{ get{ return SkillName.Wrestling; } }
		public override WeaponType DefType{ get{ return WeaponType.Fists; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Wrestle; } }

		public Fists() : base( 0 )
		{
			Visible = false;
			Movable = false;
			Quality = WeaponQuality.Regular;
		}

		public Fists( Serial serial ) : base( serial )
		{
		}

		public override double GetDefendSkillValue( Mobile attacker, Mobile defender )
		{
			double wresValue = defender.Skills[SkillName.Wrestling].Value;

		    if (defender.IsT2A || attacker.IsT2A)
		        return wresValue;

			double anatValue = defender.Skills[SkillName.Anatomy].Value;
			double evalValue = defender.Skills[SkillName.EvalInt].Value;
            double incrValue = Math.Min((anatValue + evalValue) / 200.0 * SpecialMovesController._DefensiveWrestlingMaxValue, 120.0);

			if ( wresValue > incrValue )
				return wresValue;
			else
				return incrValue;
		}

		public override TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
			if ( attacker.StunReady && !attacker.IsT2A)
			{
				if ( attacker.CanBeginAction( typeof( Fists ) ) )
				{
					if ( attacker.Skills[SkillName.Anatomy].Value >= 80.0 && attacker.Skills[SkillName.Wrestling].Value >= 80.0 )
					{
						if ( attacker.Stam >= SpecialMovesController._StunStaminaRequired )
						{
                            attacker.Stam -= SpecialMovesController._StunStaminaRequired;

							if ( CheckMove( attacker, SkillName.Anatomy ) )
							{
								StartMoveDelay( attacker );

								attacker.StunReady = false;

								attacker.SendLocalizedMessage( 1004013 ); // You successfully stun your opponent!
								defender.SendLocalizedMessage( 1004014 ); // You have been stunned!

								defender.Freeze( TimeSpan.FromSeconds( SpecialMovesController._StunPunchSeconds) );
								if( defender.Spell != null )
									defender.Spell.OnCasterHurt();
							}
							else
							{
								attacker.SendLocalizedMessage( 1004010 ); // You failed in your attempt to stun.
								defender.SendLocalizedMessage( 1004011 ); // Your opponent tried to stun you and failed.
							}
						}
						else
						{
							attacker.SendLocalizedMessage( 1004009 ); // You are too fatigued to attempt anything.
						}
					}
					else
					{
						attacker.SendLocalizedMessage( 1004008 ); // You are not skilled enough to stun your opponent.
						attacker.StunReady = false;
					}
				}
			}
            else if (attacker.DisarmReady && !attacker.IsT2A)
			{
				if ( attacker.CanBeginAction( typeof( Fists ) ) )
				{
					if ( defender.Player || defender.Body.IsHuman )
					{
						if ( attacker.Skills[SkillName.ArmsLore].Value >= 80.0 && attacker.Skills[SkillName.Wrestling].Value >= 80.0 )
						{
							if ( attacker.Stam >= SpecialMovesController._DisarmStaminaRequired )
							{
								Item toDisarm = defender.FindItemOnLayer( Layer.OneHanded );

								if ( toDisarm == null || !toDisarm.Movable )
									toDisarm = defender.FindItemOnLayer( Layer.TwoHanded );

								Container pack = defender.Backpack;

								if ( pack == null || toDisarm == null || !toDisarm.Movable )
								{
									attacker.SendLocalizedMessage( 1004001 ); // You cannot disarm your opponent.
								}
								else if ( CheckMove( attacker, SkillName.ArmsLore ) )
								{
									StartMoveDelay( attacker );

                                    attacker.Stam -= SpecialMovesController._DisarmStaminaRequired;
									attacker.DisarmReady = false;

									attacker.SendLocalizedMessage( 1004006 ); // You successfully disarm your opponent!
									defender.SendLocalizedMessage( 1004007 ); // You have been disarmed!

									pack.DropItem( toDisarm );
								}
								else
								{
                                    attacker.Stam -= SpecialMovesController._DisarmStaminaRequired;

									attacker.SendLocalizedMessage( 1004004 ); // You failed in your attempt to disarm.
									defender.SendLocalizedMessage( 1004005 ); // Your opponent tried to disarm you but failed.
								}
							}
							else
							{
								attacker.SendLocalizedMessage( 1004003 ); // You are too fatigued to attempt anything.
							}
						}
						else
						{
							attacker.SendLocalizedMessage( 1004002 ); // You are not skilled enough to disarm your opponent.
							attacker.DisarmReady = false;
						}
					}
					else
					{
						attacker.SendLocalizedMessage( 1004001 ); // You cannot disarm your opponent.
					}
				}
			}

			return base.OnSwing( attacker, defender );
		}

		/*public override void OnMiss( Mobile attacker, Mobile defender )
		{
			base.PlaySwingAnimation( attacker );
		}*/

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			Delete();
		}

		/* Wrestling moves */

		private static bool CheckMove( Mobile m, SkillName other )
		{
			double wresValue = m.Skills[SkillName.Wrestling].Value;
			double scndValue = m.Skills[other].Value;

			/* 40% chance at 80, 80
			 * 50% chance at 100, 100
			 * 60% chance at 120, 120
			 */

			//double chance = (wresValue + scndValue) / 400.0;
            double chance = (wresValue + scndValue) / 200.0 * SpecialMovesController._SpecialMoveChanceAtGM_GM;

			return ( chance >= Utility.RandomDouble() );
		}

		private static bool HasFreeHands( Mobile m )
		{
			Item item = m.FindItemOnLayer( Layer.OneHanded );

			if ( item != null && !(item is Spellbook) && !(item.AllowEquippedCast(m) ))
				return false;

            Item twoHanded = m.FindItemOnLayer( Layer.TwoHanded );
			return twoHanded == null || twoHanded.AllowEquippedCast(m);
		}

		private static void EventSink_DisarmRequest( DisarmRequestEventArgs e )
		{
			Mobile m = e.Mobile;

			if ( !Engines.ConPVP.DuelContext.AllowSpecialAbility( m, "Disarm", true ) )
				return;

            if (m.IsT2A)
                return;

			double armsValue = m.Skills[SkillName.ArmsLore].Value;
			double wresValue = m.Skills[SkillName.Wrestling].Value;

			if ( !HasFreeHands( m ) )
			{
				m.SendLocalizedMessage( 1004029 ); // You must have your hands free to attempt to disarm your opponent.
				m.DisarmReady = false;
			}
			else if ( armsValue >= 80.0 && wresValue >= 80.0 )
			{
				m.DisruptiveAction();
				m.DisarmReady = !m.DisarmReady;
				m.SendLocalizedMessage( m.DisarmReady ? 1019013 : 1019014 );
			}
			else
			{
				m.SendLocalizedMessage( 1004002 ); // You are not skilled enough to disarm your opponent.
				m.DisarmReady = false;
			}
		}

		private static void EventSink_StunRequest( StunRequestEventArgs e )
		{
			Mobile m = e.Mobile;

			if ( !Engines.ConPVP.DuelContext.AllowSpecialAbility( m, "Stun", true ) )
				return;

		    if (m.IsT2A)
		        return;

			double anatValue = m.Skills[SkillName.Anatomy].Value;
			double wresValue = m.Skills[SkillName.Wrestling].Value;

			if ( !HasFreeHands( m ) )
			{
				m.SendLocalizedMessage( 1004031 ); // You must have your hands free to attempt to stun your opponent.
				m.StunReady = false;
			}
			else if ( m.Followers >= 4 )
			{
				m.SendMessage( "You have too many followers and cannot concentration on stunning your target." );
				m.StunReady = false;
			}
			else if ( anatValue >= 80.0 && wresValue >= 80.0 )
			{
				m.DisruptiveAction();
				m.StunReady = !m.StunReady;
				m.SendLocalizedMessage( m.StunReady ? 1019011 : 1019012 );
			}
			else
			{
				m.SendLocalizedMessage( 1004008 ); // You are not skilled enough to stun your opponent.
				m.StunReady = false;
			}
		}

		private class MoveDelayTimer : Timer
		{
			private Mobile m_Mobile;

			public MoveDelayTimer( Mobile m ) : base( TimeSpan.FromSeconds( 10.0 ) )
			{
				m_Mobile = m;

				Priority = TimerPriority.TwoFiftyMS;

				m_Mobile.BeginAction( typeof( Fists ) );
			}

			protected override void OnTick()
			{
				m_Mobile.EndAction( typeof( Fists ) );
			}
		}

		private static void StartMoveDelay( Mobile m )
		{
			new MoveDelayTimer( m ).Start();
		}
	}
}