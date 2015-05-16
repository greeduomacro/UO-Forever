using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class SlaveMiner : BaseCreature
	{
		private Direction m_Direction;
		private DateTime m_NextDigTime;
		private DateTime m_NextSpeakTime;
		private DateTime m_NextHurtSpeak;

		[CommandProperty( AccessLevel.GameMaster )]
		public Direction SpawnDirection{ get{ return m_Direction; } }

		public static TimeSpan DigDelay{ get{ return TimeSpan.FromSeconds( 60.0 + ( 10.0 * Utility.RandomDouble() ) ); } }
		public static TimeSpan SpeakDelay{ get{ return TimeSpan.FromSeconds( 45.0 + ( 15.0 * Utility.RandomDouble() ) ); } }
		public static TimeSpan HurtSpeakDelay{ get{ return TimeSpan.FromSeconds( 5.0 + ( 3.0 * Utility.RandomDouble() ) ); } }

		[Constructable]
		public SlaveMiner() : this( Direction.North )
		{
		}

		[Constructable]
		public SlaveMiner( Direction direction ) : base( AIType.AI_Melee, FightMode.Aggressor, 3, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();
			Body = 400;
			Name = NameList.RandomName( "male" );

			SetStr( 300 );
			SetDex( 150 );
			SetInt( 50 );

			SetHits( 90 );
			SetStam( 400 );

			SetDamage( 8, 13 );

			SetSkill( SkillName.Mining, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );
			SetSkill( SkillName.Swords, 100.0 );

			AddItem( new LongPants( Utility.RandomGreyHue() ) );
			AddItem( new Server.Items.Pickaxe() );

			CantWalk = false;
			Title = "the slave";
			m_Direction = direction;
			m_NextDigTime = DateTime.UtcNow;
			m_NextSpeakTime = DateTime.UtcNow + SpeakDelay;
			m_NextHurtSpeak = DateTime.UtcNow;

			Karma = 5000;
			Fame = 0;

			VirtualArmor = 50;

			if ( 0.25 > Utility.RandomDouble() )
				PackItem( new IronIngot( Utility.RandomMinMax( 2, 10 ) ) );
			else
				PackItem( new IronOre( Utility.RandomMinMax( 2, 10 ) ) );

			PackItem( new Shovel() );
			if ( 0.50 > Utility.RandomDouble() )
				PackItem( new Shovel() );
			if ( 0.25 > Utility.RandomDouble() )
				PackItem( new Shovel() );
		}

		public SlaveMiner( Serial serial ) : base( serial )
		{
		}

		private static readonly string[] m_RandomHelpText = new string[]
			{
				"I cannot leave!",
				"I be hungry..",
				"I do not want to mine!",
				"Help me!",
				"Save me!",
				"Shh, they mayeth hear you!",
				"Someone free us!",
				"My feet hurt."
			};

		private static readonly string[] m_RandomHurtText = new string[]
			{
				"You cannot hurt me {0}!",
				"{0} wilt die by my axe!",
				"Enslave me also?",
				"Die!",
				"HELP!!!",
				"AHHHHH!"
			};

		private static readonly string[] m_RandomDeathText = new string[]
			{
				"I die free!",
				"God, guide me to the light!",
				"By my axe, you enslave me no more!",
				"AHHHHHHHH!",
				"I die without honor, or dignity.",
				"Kill me, it cannot be much worse than this!"
			};

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			base.OnDamage( amount, from, willKill );

			if ( from != null && !from.Deleted && !willKill && DateTime.UtcNow >= m_NextHurtSpeak )
			{
				Say( String.Format( m_RandomHurtText[Utility.Random( m_RandomHurtText.Length )], from.Name ) );
				m_NextHurtSpeak = DateTime.UtcNow + SpeakDelay;
			}
		}

		public override bool OnBeforeDeath()
		{
			Say( m_RandomDeathText[Utility.Random( m_RandomDeathText.Length )] );
			return base.OnBeforeDeath();
		}

		public override void OnThink()
		{
			base.OnThink();

			if ( !Controlled && !Summoned && CantWalk && Map != null && Map != Map.Internal ) //Lets dig!
			{
				Mobile target = Combatant;

				if ( target == null || !target.Alive || target.Map != this.Map || !target.InRange( this, RangePerception ) || !InLOS( target ) )
				{
					Combatant = null;
					FocusMob = null;
					Warmode = false;

					if ( DateTime.UtcNow >= m_NextDigTime )
					{
						new SlaveHarvestTimer( this ).Start();
						m_NextDigTime = DateTime.UtcNow + DigDelay;
					}

					if ( DateTime.UtcNow >= m_NextSpeakTime )
					{
						Say( m_RandomHelpText[Utility.Random( m_RandomHelpText.Length )] );
						m_NextSpeakTime = DateTime.UtcNow + SpeakDelay;
					}
				}
				else
				{
					m_NextDigTime = DateTime.UtcNow + DigDelay;
					m_NextSpeakTime = DateTime.UtcNow + SpeakDelay;
				}
			}
		}

		public override bool CanRummageCorpses{ get{ return true; } }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (byte)m_Direction );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Direction = (Direction)reader.ReadByte();
		}

		private class SlaveHarvestTimer : Timer
		{
			private SlaveMiner m_From;
			private int m_Index, m_Count;

			public SlaveHarvestTimer( SlaveMiner from ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.6 ) )
			{
				m_From = from;
				m_Count = 3;
				m_Index = 0;
			}

			protected override void OnTick()
			{
				if ( m_From == null || m_From.Deleted )
					Stop();
				else
				{
					Mobile target = m_From.Combatant;
					if ( target != null && target.Alive && m_From.CanBeHarmful( target ) && target.Map != m_From.Map && target.InRange( m_From, m_From.RangePerception ) && m_From.InLOS( target ) )
						Stop();
					else
					{
						DoHarvest();
						if ( ++m_Index == m_Count )
							Stop();
					}
				}
			}

			public void DoHarvest()
			{
				m_From.Direction = m_From.SpawnDirection;
				m_From.Animate( 11, 5, 1, true, false, 0 );
				Timer.DelayCall( TimeSpan.FromSeconds( 0.9 ), new TimerCallback( DoHarvestSound ) );
			}

			public void DoHarvestSound()
			{
				m_From.PlaySound( Utility.Random( 0x125, 2 ) );
			}
		}
	}
}