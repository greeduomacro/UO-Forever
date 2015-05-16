using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Factions;
using Server.Ethics;
using Server.Network;

namespace Server.Mobiles
{
	public class WarriorGuard : BaseGuard
	{
		private Timer m_AttackTimer, m_IdleTimer;

		private Mobile m_Focus;

		[Constructable]
		public WarriorGuard() : this( null, null )
		{
		}

		public WarriorGuard( Faction faction ) : this( null, faction )
		{
		}

		public WarriorGuard( Mobile target ) : this( target, null )
		{
		}

		public WarriorGuard( Mobile target, Faction faction ) : base( target, faction )
		{
			this.NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds( 0.5 );
			this.Focus = target;
		}

		public override void InitBody( Faction faction )
		{
			Utility.AssignRandomHair( this );

			if ( Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );

				if( Utility.RandomBool() )
					Utility.AssignRandomFacialHair( this, HairHue );
			}

			InitStats( 1000, 1000, 1000 );

			if ( faction != null )
				Title = String.Format( "the {0} guard", faction.Definition.FriendlyName.ToLower() );
			else
				Title = "the guard";

			SpeechHue = Utility.RandomDyedHue();

			Hue = Utility.RandomSkinHue();

			Container pack = new Backpack();
			pack.Movable = false;

			pack.DropItem( new Gold( 10, 25 ) );

			AddItem( pack );

			Skills[SkillName.Anatomy].Base = 120.0;
			Skills[SkillName.Tactics].Base = 120.0;
			Skills[SkillName.Swords].Base = 120.0;
			Skills[SkillName.MagicResist].Base = 120.0;
			Skills[SkillName.DetectHidden].Base = 100.0;
		}

		public override void InitOutfit( Faction faction )
		{
			if ( Female )
			{
				if ( faction != null )
				{
					if ( Utility.RandomBool() )
					{
						AddItem( InitItem( new FemalePlateChest(), faction.Definition.HuePrimary ) );
						AddItem( InitItem( new PlateArms(), faction.Definition.HuePrimary ) );
						AddItem( InitItem( new PlateGloves(), faction.Definition.HuePrimary ) );
					}
					else
					{
						switch( Utility.Random( 4 ) )
						{
							case 0: AddItem( InitItem( new FemaleLeatherChest(), faction.Definition.HuePrimary ) ); break;
							case 1: AddItem( InitItem( new FemaleStuddedChest(), faction.Definition.HuePrimary ) ); break;
							case 2: AddItem( InitItem( new LeatherBustierArms(), faction.Definition.HuePrimary ) ); break;
							case 3: AddItem( InitItem( new StuddedBustierArms(), faction.Definition.HuePrimary ) ); break;
						}

						AddItem( InitItem( new LeatherGloves(), faction.Definition.HuePrimary ) );
					}

					AddItem( InitItem( new PlateLegs(), faction.Definition.HuePrimary ) );
					AddItem( InitItem( new BodySash(), faction.Definition.HueSecondary ) );
					AddItem( InitItem( new LeatherGorget(), faction.Definition.HueSecondary ) );
				}
				else
				{
					switch( Utility.Random( 2 ) )
					{
						case 0: AddItem( InitItem( new LeatherSkirt() ) ); break;
						case 1: AddItem( InitItem( new LeatherShorts() ) ); break;
					}

					switch( Utility.Random( 5 ) )
					{
						case 0: AddItem( InitItem( new FemaleLeatherChest() ) ); break;
						case 1: AddItem( InitItem( new FemaleStuddedChest() ) ); break;
						case 2: AddItem( InitItem( new LeatherBustierArms() ) ); break;
						case 3: AddItem( InitItem( new StuddedBustierArms() ) ); break;
						case 4: AddItem( InitItem( new FemalePlateChest() ) ); break;
					}

					if ( Utility.Random( 5 ) == 1 ) //20% chance
						AddItem( InitItem( new LeatherGloves() ) );
				}
			}
			else
			{
				if ( faction != null )
				{
					AddItem( InitItem( new PlateChest(), faction.Definition.HuePrimary ) );
					AddItem( InitItem( new PlateArms(), faction.Definition.HuePrimary ) );
					AddItem( InitItem( new PlateGloves(), faction.Definition.HuePrimary ) );

					AddItem( InitItem( new PlateLegs(), faction.Definition.HuePrimary ) );
					AddItem( InitItem( new LeatherGorget(), faction.Definition.HueSecondary ) );

					AddItem( InitItem( new BodySash(), faction.Definition.HueSecondary ) );
				}
				else
				{
					AddItem( InitItem( new PlateChest() ) );
					AddItem( InitItem( new PlateArms() ) );
					AddItem( InitItem( new PlateLegs() ) );

					switch( Utility.Random( 3 ) )
					{
						case 0: AddItem( InitItem( new Doublet( Utility.RandomNondyedHue() ) ) ); break;
						case 1: AddItem( InitItem( new Tunic( Utility.RandomNondyedHue() ) ) ); break;
						case 2: AddItem( InitItem( new BodySash( Utility.RandomNondyedHue() ) ) ); break;
					}
				}
			}

			Halberd weapon = new Halberd();
			InitItem( weapon, faction != null ? faction.Definition.HueSecondary : 0 );

			weapon.Crafter = this;
			weapon.Quality = WeaponQuality.Exceptional;

			AddItem( weapon );
		}

		#region Begging
		public override bool CanBeBegged( Mobile from )
		{
			return true;
		}

		public override void OnBegged( Mobile beggar )
		{
			beggar.SendLocalizedMessage( 500404 ); // They seem unwilling to give you any money.
			beggar.NextSkillTime = DateTime.UtcNow + TimeSpan.FromSeconds( 10.0 );
			PublicOverheadMessage( MessageType.Regular, SpeechHue, 500407 ); // I have not enough money to give thee any!
		}
		#endregion

		public WarriorGuard( Serial serial ) : base( serial )
		{
		}

		public override bool OnBeforeDeath()
		{
			if ( m_Focus != null && m_Focus.Alive )
				new AvengeTimer( m_Focus ).Start(); // If a guard dies, three more guards will spawn

			return base.OnBeforeDeath();
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override Mobile Focus
		{
			get
			{
				return m_Focus;
			}
			set
			{
				if ( Deleted )
					return;

				Mobile oldFocus = m_Focus;

				if ( oldFocus != value )
				{
					m_Focus = value;

					if ( value != null )
						this.AggressiveAction( value );

					Combatant = value;

					if ( oldFocus != null && !oldFocus.Alive )
						Say( "Thou hast suffered thy punishment, scoundrel." );

					if ( value != null )
						Say( 500131 ); // Thou wilt regret thine actions, swine!

					if ( m_AttackTimer != null )
					{
						m_AttackTimer.Stop();
						m_AttackTimer = null;
					}

					if ( m_IdleTimer != null )
					{
						m_IdleTimer.Stop();
						m_IdleTimer = null;
					}

					if ( m_Focus != null )
					{
						m_AttackTimer = new AttackTimer( this );
						m_AttackTimer.Start();
						((AttackTimer)m_AttackTimer).DoOnTick();
					}
					else
					{
						m_IdleTimer = new IdleTimer( this );
						m_IdleTimer.Start();
					}
				}
				else if ( m_Focus == null && m_IdleTimer == null )
				{
					m_IdleTimer = new IdleTimer( this );
					m_IdleTimer.Start();
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Focus );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Focus = reader.ReadMobile();

					if ( m_Focus != null )
					{
						m_AttackTimer = new AttackTimer( this );
						m_AttackTimer.Start();
					}
					else
					{
						m_IdleTimer = new IdleTimer( this );
						m_IdleTimer.Start();
					}

					break;
				}
			}
		}

		public override void OnAfterDelete()
		{
			if ( m_AttackTimer != null )
			{
				m_AttackTimer.Stop();
				m_AttackTimer = null;
			}

			if ( m_IdleTimer != null )
			{
				m_IdleTimer.Stop();
				m_IdleTimer = null;
			}

			base.OnAfterDelete();
		}

		private class AvengeTimer : Timer
		{
			private Mobile m_Focus;

			public AvengeTimer( Mobile focus ) : base( TimeSpan.FromSeconds( 2.5 ), TimeSpan.FromSeconds( 1.0 ), 3 )
			{
				m_Focus = focus;
			}

			protected override void OnTick()
			{
				BaseGuard.Spawn( m_Focus, m_Focus, 1, true );
			}
		}

		private class AttackTimer : Timer
		{
			private WarriorGuard m_Owner;

			public AttackTimer( WarriorGuard owner ) : base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 0.1 ) )
			{
				m_Owner = owner;
			}

			public void DoOnTick()
			{
				OnTick();
			}

			protected override void OnTick()
			{
				if ( !m_Owner.Deleted )
				{
					m_Owner.Criminal = false;
					m_Owner.Karma = 0;
					m_Owner.Fame = 0;
					m_Owner.Kills = 0;
					m_Owner.Stam = m_Owner.StamMax;

					Mobile target = m_Owner.Focus;

					if ( target != null )
					{
						if ( !target.Deleted && target.Alive && m_Owner.CanBeHarmful( target ) ) //Dead, deleted, or invul
						{
							if ( m_Owner.Weapon is Fists )
								m_Owner.Kill();
							else
							{
								if ( m_Owner.Combatant != target )
									m_Owner.Combatant = target;

								TeleportTo( target );
								target.BoltEffect( 0 );

								if ( target is BaseCreature )
									((BaseCreature)target).NoKillAwards = true;

								target.Damage( target.HitsMax, m_Owner );
								target.LastKiller = m_Owner;
								target.Kill(); // just in case, maybe Damage is overridden on some shard

								if ( target.Player )
								{
									if ( Faction.Find( target ) != null )
										Faction.ApplySkillLoss( target );

								}
								else if ( target.Corpse != null )
									target.Corpse.Delete();
							}
						}

						m_Owner.Focus = null;
					}
				}

				Stop();
			}

			private void TeleportTo( Mobile target )
			{
				Point3D from = m_Owner.Location;
				Point3D to = target.Location;

				m_Owner.Location = to;

				Effects.SendLocationParticles( EffectItem.Create( from, m_Owner.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				Effects.SendLocationParticles( EffectItem.Create(   to, m_Owner.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );

				m_Owner.PlaySound( 0x1FE );
			}
		}

		private class IdleTimer : Timer
		{
			private WarriorGuard m_Owner;
			private int m_Stage;

			public IdleTimer( WarriorGuard owner ) : base( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.5 ) )
			{
				m_Owner = owner;
			}

			protected override void OnTick()
			{
				if ( m_Owner.Deleted )
					Stop();
				else
				{
					if ( (m_Stage++ % 4) == 0 || !m_Owner.Move( m_Owner.Direction ) )
						m_Owner.Direction = (Direction)Utility.Random( 8 );

					if ( m_Stage > 16 )
					{
						Effects.SendLocationParticles( EffectItem.Create( m_Owner.Location, m_Owner.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
						m_Owner.PlaySound( 0x1FE );

						m_Owner.Delete();
					}
				}
			}
		}
	}
}