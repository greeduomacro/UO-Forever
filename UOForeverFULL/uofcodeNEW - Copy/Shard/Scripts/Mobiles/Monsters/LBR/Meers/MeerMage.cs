using System;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "a meer's corpse" )]
	public class MeerMage : BaseCreature
	{
		public override string DefaultName{ get{ return "a meer mage"; } }

		[Constructable]
		public MeerMage() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Body = 770;

			SetStr( 171, 200 );
			SetDex( 126, 145 );
			SetInt( 276, 305 );

			SetHits( 103, 120 );

			SetDamage( 24, 26 );

			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.Magery, 70.1, 80.0 );
			SetSkill( SkillName.Meditation, 85.1, 95.0 );
			SetSkill( SkillName.MagicResist, 80.1, 100.0 );
			SetSkill( SkillName.Tactics, 70.1, 90.0 );
			SetSkill( SkillName.Wrestling, 60.1, 80.0 );

			Fame = 8000;
			Karma = 8000;

			VirtualArmor = 16;

			m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.RandomMinMax( 2, 5 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.MedScrolls, 2 );
			// TODO: Daemon bone ...
		}

		public override bool AutoDispel{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }

		public override bool InitialInnocent{ get{ return true; } }

		public override int GetHurtSound()
		{
			return 0x14D;
		}

		public override int GetDeathSound()
		{
			return 0x314;
		}

		public override int GetAttackSound()
		{
			return 0x75;
		}

		private DateTime m_NextAbilityTime;

		public override void OnThink()
		{
			if ( DateTime.UtcNow >= m_NextAbilityTime )
			{
				Mobile combatant = this.Combatant;

				if ( combatant != null && combatant.Map == this.Map && combatant.InRange( this, 12 ) && IsEnemy( combatant ) && !UnderEffect( combatant ) )
				{
					m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds( Utility.RandomMinMax( 20, 30 ) );

					if( combatant is BaseCreature )
					{
						BaseCreature bc = (BaseCreature)combatant;

						if( bc.Controlled && bc.ControlMaster != null && !bc.ControlMaster.Deleted && bc.ControlMaster.Alive )
						{
							if ( bc.ControlMaster.Map == this.Map && bc.ControlMaster.InRange( this, 12 ) && !UnderEffect( bc.ControlMaster ) )
							{
								Combatant = combatant = bc.ControlMaster;
							}
						}
					}

					if( Utility.RandomDouble() < .1 )
					{
						int[][] coord =
						{
							new int[]{-4,-6}, new int[]{4,-6}, new int[]{0,-8}, new int[]{-5,5}, new int[]{5,5}
						};

						BaseCreature rabid;

						for( int i=0; i<5; i++ )
						{
							int x = combatant.X + coord[i][0];
							int y = combatant.Y + coord[i][1];

							Point3D loc = new Point3D( x, y, combatant.Map.GetAverageZ( x, y ) );

							if ( !combatant.Map.CanSpawnMobile( loc ) )
								continue;

							switch ( i )
							{
								case 0: rabid = new EnragedRabbit( this ); break;
								case 1: rabid = new EnragedHind( this ); break;
								case 2: rabid = new EnragedHart( this ); break;
								case 3: rabid = new EnragedBlackBear( this ); break;
								default: rabid = new EnragedEagle( this ); break;
							}

							rabid.FocusMob = combatant;
							rabid.MoveToWorld( loc, combatant.Map );
						}
						this.Say( 1071932 ); //Creatures of the forest, I call to thee!  Aid me in the fight against all that is evil!
					}
					else if ( combatant.Player )
					{
						this.Say( true, "I call a plague of insects to sting your flesh!" );
						m_Table[combatant] = Timer.DelayCall<EffectInfo>( TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds( 7.0 ), new TimerStateCallback<EffectInfo>( DoEffect ), new EffectInfo( combatant ) );
					}
				}
			}

			base.OnThink();
		}

		private static Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

		public static bool UnderEffect( Mobile m )
		{
			Timer t;
			m_Table.TryGetValue( m, out t );
			return t != null;
		}

		public static void StopEffect( Mobile m, bool message )
		{
			Timer t;
			m_Table.TryGetValue( m, out t );

			if ( t != null )
			{
				if ( message )
					m.PublicOverheadMessage( Network.MessageType.Emote, m.SpeechHue, true, "* The open flame begins to scatter the swarm of insects *" );

				t.Stop();
				m_Table.Remove( m );
			}
		}

		private class EffectInfo
		{
			public Mobile From;
			public int Count;

			public EffectInfo( Mobile from )
			{
				From = from;
			}
		}

		private void DoEffect( EffectInfo effect )
		{
			Mobile m = effect.From;
			int count = effect.Count++;

			if ( !m.Alive )
				StopEffect( m, false );
			else
			{
				Torch torch = m.FindItemOnLayer( Layer.TwoHanded ) as Torch;

				if ( torch != null && torch.Burning )
					StopEffect( m, true );
				else
				{
					if ( (count % 4) == 0 )
					{
						m.LocalOverheadMessage( Network.MessageType.Emote, m.SpeechHue, true, "* The swarm of insects bites and stings your flesh! *" );
						m.NonlocalOverheadMessage( Network.MessageType.Emote, m.SpeechHue, true, String.Format( "* {0} is stung by a swarm of insects *", m.Name ) );
					}

					m.FixedParticles( 0x91C, 10, 180, 9539, EffectLayer.Waist );
					m.PlaySound( 0x00E );
					m.PlaySound( 0x1BC );

					m.Damage( Utility.RandomMinMax( 20, 30 ) , this);

					if ( !m.Alive )
						StopEffect( m, false );
				}
			}
		}

		public MeerMage( Serial serial ) : base( serial )
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