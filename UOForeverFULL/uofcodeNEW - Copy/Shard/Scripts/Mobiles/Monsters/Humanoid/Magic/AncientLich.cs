using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ancient lich corpse" )]
	public class AncientLich : BaseCreature
	{
		[Constructable]
		public AncientLich() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "ancient lich" );
			Body = 78;
			BaseSoundID = 412;

			SetStr( 216, 305 );
			SetDex( 96, 115 );
			SetInt( 966, 1045 );

			SetHits( 560, 595 );

			SetDamage( 15, 27 );






            Alignment = Alignment.Undead;
			
			
			

			SetSkill( SkillName.EvalInt, 120.1, 130.0 );
			SetSkill( SkillName.Magery, 120.1, 130.0 );
			SetSkill( SkillName.Meditation, 100.1, 101.0 );
			SetSkill( SkillName.Poisoning, 100.1, 101.0 );
			SetSkill( SkillName.MagicResist, 175.2, 200.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 75.1, 100.0 );
			//SetSkill( SkillName.Necromancy, 120.1, 130.0 );
			SetSkill( SkillName.SpiritSpeak, 120.1, 130.0 );

			Fame = 23000;
			Karma = -23000;

			VirtualArmor = 60;
			PackNecroReg( 30, 275 );
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override int GetIdleSound()
		{
			return 0x19D;
		}

		public override int GetAngerSound()
		{
			return 0x175;
		}

		public override int GetDeathSound()
		{
			return 0x108;
		}

		public override int GetAttackSound()
		{
			return 0xE2;
		}

		public override int GetHurtSound()
		{
			return 0x28B;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.MedScrolls, 2 );

			  if (0.009 > Utility.RandomDouble())// 2 percent - multipy number x 100 to get percent

                switch (Utility.Random(2))
                { // rolls and number it gives it a case. if the number is , say , 3 it will pack that item
                    case 0: PackItem(new BlackDyeTub()); break;
                    case 1: PackItem(new Sandals(1175)); break;
                }
		}
    
		// public override bool Unprovokable{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		private DateTime m_NextAttack;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.UtcNow >= m_NextAttack && AIObject.Action != ActionType.Combat && AIObject.Action != ActionType.Flee && !Paralyzed && Utility.Random( 10 ) == 0 )
			{
				SummonUndead( combatant );
				m_NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 300.0 + (300.0 * Utility.RandomDouble()) );
			}
		}

		public override void OnThink()
		{
			base.OnThink();

			if ( DateTime.UtcNow >= m_NextAttack && AIObject.Action != ActionType.Combat && AIObject.Action != ActionType.Flee && !Paralyzed )
				EndPolymorph();
		}

		public void SummonUndead( Mobile target )
		{
			Point3D[] locs = new Point3D[4];

			locs[0] = Location;

			for ( int i = 1; i < 4; i++ )
			{
				bool validLocation = false;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = X + Utility.Random( 4 ) - 1;
					int y = Y + Utility.Random( 4 ) - 1;
					int z = this.Map.GetAverageZ( x, y );

					if ( validLocation = this.Map.CanFit( x, y, this.Z, 16, false, false ) )
						locs[i] = new Point3D( x, y, Z );
					else if ( validLocation = this.Map.CanFit( x, y, z, 16, false, false ) )
						locs[i] = new Point3D( x, y, z );
				}
			}

			bool movelich = false;

			for ( int i = 0;i < 4; i++ )
			{
				BaseCreature summon = null;

				if ( !movelich && ( Utility.Random( 4 ) == 0 || i == 3 ) )
				{
					summon = this;
					BodyMod = Utility.RandomList( 50, 56, 57, 3, 26, 148, 147, 153, 154, 24, 35, 36 );
					HueMod = 0;
					movelich = true;
				}
				else
				{
					switch ( Utility.Random( 12 ) )
					{
						default:
						case 0: summon = new Skeleton(); break;
						case 1: summon = new Zombie(); break;
						case 2: summon = new Wraith(); break;
						case 3: summon = new Spectre(); break;
						case 4: summon = new Ghoul(); break;
						case 5: summon = new Mummy(); break;
						case 6: summon = new Bogle(); break;
						case 7: summon = new BoneKnight(); break;
						case 8: summon = new SkeletalKnight(); break;
						case 9: summon = new Lich(); break;
						case 10: summon = new Lizardman(); break;
						case 11: summon = new SkeletalMage(); break;
					}

					summon.Team = this.Team;
					summon.FightMode = FightMode.Closest;
				}

				summon.MoveToWorld( locs[i], Map );
				Effects.SendLocationEffect( summon.Location, summon.Map, 0x3728, 10, 10, 0, 0 );
				summon.PlaySound( 0x48F );
				summon.PlaySound( summon.GetAttackSound() );
				summon.Combatant = target;
			}
		}

		public void EndPolymorph()
		{
			BodyMod = 0;
			HueMod = -1;
		}

		public AncientLich( Serial serial ) : base( serial )
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