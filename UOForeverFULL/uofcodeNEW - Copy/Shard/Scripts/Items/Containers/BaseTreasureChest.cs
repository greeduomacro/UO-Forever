using Server;
using Server.Items;
using Server.Network;
using System;
using System.Collections;

namespace Server.Items
{
	public enum TreasureLevel : byte
	{
		Level1,
		Level2,
		Level3,
		Level4,
		Level5,
		Level6,
		Level7,
		Level8
	};

	public class BaseTreasureChest : LockableContainer
	{
		private TreasureLevel m_TreasureLevel;
		private short m_MaxSpawnTime = 60;
		private short m_MinSpawnTime = 45;
		private TreasureResetTimer m_ResetTimer;
		private bool m_IsTrapable;
		private int m_TrapChance = 0; //As a percent of 100

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsTrapable{ get{ return m_IsTrapable; } set{ m_IsTrapable = value; SetTrap(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int TrapChance{ get{ return m_TrapChance; } set{ m_TrapChance = Math.Max( Math.Min( value, 100 ), 0 ); SetTrap(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public TreasureLevel Level
		{
			get
			{
				return m_TreasureLevel;
			}
			set
			{
				m_TreasureLevel = value;
				ClearContents();
				SetLockLevel();
				SetTrap();
			//	GenerateTreasure();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public short MaxSpawnTime
		{
			get
			{
				return m_MaxSpawnTime;
			}
			set
			{
				m_MaxSpawnTime = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public short MinSpawnTime
		{
			get
			{
				return m_MinSpawnTime;
			}
			set
			{
				m_MinSpawnTime = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override bool Locked
		{
			get { return base.Locked; }
			set
			{
				if ( base.Locked != value )
				{
					base.Locked = value;

					if ( !value )
						StartResetTimer();
				}
			}
		}

		public override bool IsDecoContainer{ get{ return false; } }
		public override bool TrapOnOpen{ get{ return true; } }

		public BaseTreasureChest( int itemID ) : this( itemID, TreasureLevel.Level2 )
		{
		}

		public BaseTreasureChest( int itemID, TreasureLevel level ) : base( itemID )
		{
			m_TreasureLevel = level;
			Locked = true;
			Movable = false;

			SetLockLevel();
			SetTrap();
    	//	GenerateTreasure();
		}

		public BaseTreasureChest( Serial serial ) : base( serial )
		{
		}

		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
					return "a locked treasure chest";
				return "a treasure chest";
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );

			//Version 1
			writer.Write( m_IsTrapable );
			writer.Write( m_TrapChance );

			//Version 0
			writer.Write( (byte) m_TreasureLevel );
			writer.Write( m_MinSpawnTime );
			writer.Write( m_MaxSpawnTime );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_IsTrapable = reader.ReadBool();
					m_TrapChance = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_TreasureLevel = (TreasureLevel)reader.ReadByte();
					m_MinSpawnTime = reader.ReadShort();
					m_MaxSpawnTime = reader.ReadShort();
					break;
				}
			}

			StartResetTimer();
		}

		public override void LockPick( Mobile from )
		{
			base.LockPick( from );

			//GenerateTreasure( from );
		}

		protected virtual void SetLockLevel()
		{
			switch( m_TreasureLevel )
			{
				case TreasureLevel.Level1:
					this.RequiredSkill = 25;
					break;

				case TreasureLevel.Level2:
					this.RequiredSkill = 45;
					break;

				case TreasureLevel.Level3:
					this.RequiredSkill = 65;
					break;

				case TreasureLevel.Level4:
					this.RequiredSkill = 85;
					break;

				case TreasureLevel.Level5:
					this.RequiredSkill = 95;
					break;
				default:
					this.RequiredSkill = 100;
					break;
			}

			Locked = true;
			LockLevel = RequiredSkill - 10;
			MaxLockLevel = RequiredSkill + 40;

			if ( m_TreasureLevel > TreasureLevel.Level5 )
				MaxLockLevel = RequiredSkill + (((int)m_TreasureLevel-4)*50);
		}

		public virtual double GetDifficultyScalar( Mobile from )
		{
			double skill = from.Skills[SkillName.Lockpicking].Value;
			if ( skill > MaxLockLevel || ( skill >= 100.0 && m_TreasureLevel <= TreasureLevel.Level4 ) )
				return 0;

			double halfway = MaxLockLevel - ( ( MaxLockLevel - RequiredSkill ) / 2.0 );

			if ( skill < halfway )
				return 1.0;
			else
				return 0.5 - ( 0.25 * ( skill - halfway ) / ( MaxLockLevel - halfway ) );
		}

		protected virtual void SetTrap()
		{
			if ( m_IsTrapable && m_TrapChance > Utility.Random( 100 ) )
			{
				int level = (int)m_TreasureLevel;

				if ( level > Utility.Random( 8 ) )
					TrapType = Utility.RandomBool() ? TrapType.PoisonTrap : TrapType.ExplosionTrap;
				else
					TrapType = Utility.RandomBool() ? TrapType.ExplosionTrap : TrapType.MagicTrap;

				if ( TrapType == TrapType.PoisonTrap )
					TrapLevel = ((level+1)/2)+1;
				else
					TrapLevel = 0;

				TrapPower = (level+1) * Utility.RandomMinMax( 5, 10 );

				TrapOnLockpick = true;
			}
			else
			{
				TrapType = TrapType.None;
				TrapPower = 0;
				TrapLevel = 0;
				TrapOnLockpick = false;
			}
		}

		private void StartResetTimer()
		{
			if( m_ResetTimer == null )
				m_ResetTimer = new TreasureResetTimer( this );
			else
				m_ResetTimer.Delay = TimeSpan.FromMinutes( Utility.Random( m_MinSpawnTime, m_MaxSpawnTime ));

			m_ResetTimer.Start();
		}

		protected virtual void GenerateTreasure( Mobile from )
		{
			int MinGold = 1;
			int MaxGold = 2;
			bool trapped = TrapOnLockpick;

			switch( m_TreasureLevel )
			{
				case TreasureLevel.Level1:
					MinGold = 25;
					MaxGold = 75;
					break;
				case TreasureLevel.Level2:
					MinGold = 50;
					MaxGold = 100;
					break;
				case TreasureLevel.Level3:
					MinGold = 80;
					MaxGold = 125;
					break;
				case TreasureLevel.Level4:
					MinGold = 150;
					MaxGold = 275;
					break;
				case TreasureLevel.Level5:
					MinGold = 325;
					MaxGold = 405;
					break;
				case TreasureLevel.Level6:
					MinGold = 575;
					MaxGold = 725;
					break;
				case TreasureLevel.Level7:
					MinGold = 975;
					MaxGold = 1790;
					break;
				default:
				case TreasureLevel.Level8:
					MinGold = 1475;
					MaxGold = 2635;
					break;
			}

			switch ( m_TreasureLevel )
			{
				default:
				case TreasureLevel.Level8:
					AddLoot( from, LootPack.FilthyRich, 2 );
					if ( trapped )
						AddLoot( from, LootPack.UltraRich );
					goto case TreasureLevel.Level7;
				case TreasureLevel.Level7:
					AddLoot( from, LootPack.FilthyRich );
					if ( trapped )
						AddLoot( from, LootPack.FilthyRich );
					goto case TreasureLevel.Level6;
				case TreasureLevel.Level6:
					AddLoot( from, LootPack.Rich, 2 );
					if ( trapped )
						AddLoot( from, LootPack.Rich );
					goto case TreasureLevel.Level5;
				case TreasureLevel.Level5:
					AddLoot( from, LootPack.Gems );
					AddLoot( from, LootPack.Average, 2 );
					if ( trapped )
						AddLoot( from, LootPack.Rich );
					goto case TreasureLevel.Level4;
				case TreasureLevel.Level4:
					AddLoot( from, LootPack.Gems );
					if ( trapped )
						AddLoot( from, LootPack.Average );
					goto case TreasureLevel.Level3;
				case TreasureLevel.Level3:
					AddLoot( from, LootPack.Gems );
					if ( trapped )
						AddLoot( from, LootPack.Average );
					goto case TreasureLevel.Level2;
				case TreasureLevel.Level2:
					AddLoot( from, LootPack.Gems );
					if ( trapped )
						AddLoot( from, LootPack.Meager );
					goto case TreasureLevel.Level1;
				case TreasureLevel.Level1:
					AddLoot( from, LootPack.Gems );
					if ( trapped )
						AddLoot( from, LootPack.Meager );
				break;
			}

			double scalar = GetDifficultyScalar( from );

			if ( scalar > 0 )
				DropItem( new Gold( (int)( MinGold * scalar ), (int)( MaxGold * scalar ) ) );
		}

		public virtual void AddLoot( Mobile from, LootPack pack, int amount )
		{
			for ( int i = 0; i < amount; ++i )
				AddLoot( from, pack );
		}

		public virtual void AddLoot( Mobile from, LootPack pack )
		{
			pack.Generate( null, this, false);
		}

		public void ClearContents()
		{
			for ( int i = Items.Count - 1; i >= 0; --i )
				Items[i].Delete();
		}

		public void Reset()
		{
			if( m_ResetTimer != null )
			{
				if( m_ResetTimer.Running )
					m_ResetTimer.Stop();
			}

			Locked = true;
			SetTrap();
			ClearContents();
		//	GenerateTreasure();
		}

		private class TreasureResetTimer : Timer
		{
			private BaseTreasureChest m_Chest;

			public TreasureResetTimer( BaseTreasureChest chest ) : base ( TimeSpan.FromMinutes( Utility.Random( chest.MinSpawnTime, chest.MaxSpawnTime ) ) )
			{
				m_Chest = chest;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m_Chest.Reset();
			}
		}
	}
}