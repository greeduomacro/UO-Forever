// Wolf Family
// a RunUO ver 2.0 Script
// Written by David
// last edited 6/17/06

using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "a wolf corpse" )]
	public class MotherWolf : BaseCreature
	{
		private List<WolfPup> m_Pups;
		int pupCount = Utility.RandomMinMax( 2, 5 );

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 3; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }
		public override bool CanRegenHits{ get{ return true; } }
		public override string DefaultName{ get{ return "a wolf mother"; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RespawnPups
		{
			get{ return false; }
			set{ if( value ) SpawnBabies(); }
		}

		[Constructable]
        public MotherWolf() : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.1, 0.3)
		{
			Body = 25;
			BaseSoundID = 0xE5;

			SetStr( 91, 110 );
			SetDex( 76, 95 );
			SetInt( 31, 50 );

			SetHits( 42, 68 );
			SetMana( 0 );

			SetDamage( 11, 21 );

			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 30.6, 45.0 );
			SetSkill( SkillName.Tactics, 50.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 75.0 );

			Fame = 300;
			Karma = 0;

			VirtualArmor = 22;

			Tamable = false;

			m_Pups = new List<WolfPup>();
			Timer m_timer = new WolfFamilyTimer( this );
			m_timer.Start();
		}

		public override bool OnBeforeDeath()
		{
			foreach( WolfPup m in m_Pups )
			{
				if( m.Alive && m.ControlMaster != null )
					m.Kill();
			}

			return base.OnBeforeDeath();
		}

		public void SpawnBabies()
		{

			Defrag();
			int family = m_Pups.Count;

			if( family >= pupCount )
				return;

			//Say( "family {0}, should be {1}", family, pupCount );

			Map map = this.Map;

			if ( map == null )
				return;

			int hr = (int)((this.RangeHome + 1) / 2);

			for ( int i = family; i < pupCount; ++i )
			{
				WolfPup pup = new WolfPup();

				bool validLocation = false;
				Point3D loc = this.Location;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = X + Utility.Random( 5 ) - 1;
					int y = Y + Utility.Random( 5 ) - 1;
					int z = map.GetAverageZ( x, y );

					if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
						loc = new Point3D( x, y, Z );
					else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				pup.Mother = this;
				pup.Team = this.Team;
				pup.Home = this.Location;
				pup.HomeMap = this.Map;
				pup.RangeHome = ( hr > 4 ? 4 : hr );

				pup.MoveToWorld( loc, map );
				m_Pups.Add( pup );
			}
		}

		protected override void OnLocationChange( Point3D oldLocation )
		{
			try
			{
				foreach( WolfPup m in m_Pups )
				{
					if( m.Alive && m.ControlMaster == null )
					{
						m.Home = this.Location;
						m.HomeMap = this.Map;
					}
				}
			}
			catch{}

			base.OnLocationChange( oldLocation );
		}

		public void Defrag()
		{
			for ( int i = 0; i < m_Pups.Count; ++i )
			{
				try
				{
					object o = m_Pups[i];

					WolfPup pup = o as WolfPup;

					if ( pup == null || !pup.Alive )
					{
						m_Pups.RemoveAt( i );
						--i;
					}
					else if ( pup.Controlled || pup.IsStabled )
					{
						pup.Mother = null;
						m_Pups.RemoveAt( i );
						--i;
					}
				}
				catch{}
			}
		}

		public override void OnDelete()
		{
			Defrag();

			foreach( Mobile m in m_Pups )
			{
				if( m.Alive && ((WolfPup)m).ControlMaster == null )
					m.Delete();
			}

			base.OnDelete();
		}

		public MotherWolf(Serial serial) : base(serial)
		{
			m_Pups = new List<WolfPup>();
			Timer m_timer = new WolfFamilyTimer( this );
			m_timer.Start();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt((int) 0);
			writer.WriteMobileList( m_Pups, true );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
			m_Pups = reader.ReadStrongMobileList<WolfPup>();
		}
	}

	[CorpseName( "a young wolf corpse" )]
	public class WolfPup : BaseCreature
	{
		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }
		public override string DefaultName{ get{ return "a wolf pup"; } }

		private MotherWolf m_Mommy;

		[CommandProperty( AccessLevel.GameMaster )]
		public MotherWolf Mother
		{
			get{ return m_Mommy; }
			set{ m_Mommy = value; }
		}

		[Constructable]
		public WolfPup() : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
		{
			Name = "a wolf pup";
			Body = 0xD9;
			BaseSoundID = 0xE5;

			SetStr( 37, 47 );
			SetDex( 38, 53 );
			SetInt( 39, 47 );

			SetHits( 17, 42 );
			SetMana( 0 );

			SetDamage( 4, 7 );

			

			

			SetSkill( SkillName.MagicResist, 22.1, 47.0 );
			SetSkill( SkillName.Tactics, 19.2, 31.0 );
			SetSkill( SkillName.Wrestling, 19.2, 46.0 );

			Fame = 100;
			Karma = 100;

			VirtualArmor = 10;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 53.1;
		}

		public override void OnCombatantChange()
		{
			if( Combatant != null && Combatant.Alive && m_Mommy != null && m_Mommy.Combatant == null )
				m_Mommy.Combatant = Combatant;
		}

		public WolfPup(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
			writer.Write( m_Mommy );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
			m_Mommy = (MotherWolf)reader.ReadMobile();
		}
	}

	public class WolfFamilyTimer : Timer
	{
		private MotherWolf m_From;

		public WolfFamilyTimer( MotherWolf from  ) : base( TimeSpan.FromMinutes( 1 ), TimeSpan.FromMinutes( 20 ) )
		{
			Priority = TimerPriority.OneMinute;
			m_From = from;
		}

		protected override void OnTick()
		{
			if ( m_From != null && m_From.Alive )
				m_From.SpawnBabies();
			else
				Stop();
		}
	}
}