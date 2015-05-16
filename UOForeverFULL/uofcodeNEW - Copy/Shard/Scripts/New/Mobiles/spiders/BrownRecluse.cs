using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a spider corpse" )]
	public class BrownRecluse : BaseCreature
	{
		public override string DefaultName{ get{ return "a brown recluse"; } }

		[Constructable]
		public BrownRecluse() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 173;
			Hue = 2707;
			BaseSoundID = 0x388;

			SetStr( 300, 350 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 1000, 1200 );

			SetDamage( 15, 20 );

			SetSkill( SkillName.Poisoning, 120 );
			SetSkill( SkillName.MagicResist, 120 );
			SetSkill( SkillName.Tactics, 85.1, 90.0 );
			SetSkill( SkillName.Wrestling, 90.1, 95.0 );

			Fame = 600;
			Karma = -600;

			VirtualArmor = 60;
		}

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( from != null && from.Map != null )
			{
				int amt = 0;

				Mobile target = this;

				if ( willKill )
					amt = 3 + ((Utility.Random( 6 ) % 5) >> 2); // 20% = 1, 20% = 2, 60% = 0

				if ( Hits < 550 )
				{
					double rand = Utility.RandomDouble();

					if ( 0.10 > rand )
						target = from;

					if ( 0.20 > rand )
						amt++;
				}

				if ( amt > 0 )
				{
					SpillAcid( target, amt );

					if ( willKill )
						from.SendMessage( "Your body explodes into a pile of venom!" );
					else
						from.SendMessage( "The creature spits venom at you!" );
				}
			}

			base.OnDamage( amount, from, willKill );
		}

		private Timer m_SoundTimer;
		private bool m_HasTeleportedAway;

		public override void OnCombatantChange()
		{
			base.OnCombatantChange();

			if ( Hidden && Combatant != null )
				Combatant = null;
		}

		public void SendTrackingSound()
		{
			if ( Hidden )
			{
				Effects.PlaySound( this.Location, this.Map, 0x2C8 );
				Combatant = null;
			}
			else
			{
				Frozen = false;

				if ( m_SoundTimer != null )
					m_SoundTimer.Stop();

				m_SoundTimer = null;
			}
		}

		public override void OnThink()
		{
			if ( !m_HasTeleportedAway && Hits < (HitsMax / 2) )
			{
				Map map = this.Map;

				if ( map != null )
				{
					// try 10 times to find a teleport spot
					for ( int i = 0; i < 10; ++i )
					{
						int x = X + (Utility.RandomMinMax( 5, 10 ) * (Utility.RandomBool() ? 1 : -1));
						int y = Y + (Utility.RandomMinMax( 5, 10 ) * (Utility.RandomBool() ? 1 : -1));
						int z = Z;

						if ( map.CanFit( x, y, z, 16, false, false ) )
						{
							Point3D from = this.Location;
							Point3D to = new Point3D( x, y, z );

							if ( InLOS( to ) )
							{
								this.Location = to;
								this.ProcessDelta();
								this.Hidden = true;
								this.Combatant = null;

								Effects.SendLocationParticles( EffectItem.Create( from, map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
								Effects.SendLocationParticles( EffectItem.Create(   to, map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );

								Effects.PlaySound( to, map, 0x1FE );

								m_HasTeleportedAway = true;
								m_SoundTimer = Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( 2.5 ), new TimerCallback( SendTrackingSound ) );

								Frozen = true;

								break;
							}
						}
					}
				}
			}

			base.OnThink();
		}

		private bool m_HasUltraRich = 0.25 > Utility.RandomDouble();

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Potions, 2 );
			AddLoot( LootPack.Gems, 3 );
			AddLoot( LootPack.MedScrolls, 3 );

			AddLoot( LootPack.FilthyRich );
			if ( m_HasUltraRich )
				AddLoot( LootPack.UltraRich );
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath( c );

			if ( 0.01 > Utility.RandomDouble() ) //Cocoon
				c.AddItem( new Cocoon() );
		}

		public BrownRecluse( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( m_HasUltraRich );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			m_HasUltraRich = reader.ReadBool();

			int version = reader.ReadEncodedInt();
		}
	}
}
