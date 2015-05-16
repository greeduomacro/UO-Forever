//Created Deviously by The Jester. XD
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "a Greater Phoenix corpse" )]	
	public class GreaterPhoenix : BaseCreature
	{
		[Constructable]
		public GreaterPhoenix() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.05, 0.2 )
		{
			Name = "a Greater Phoenix";
			Body = 0x1E;
			Hue = 1161;

			SetStr( 605, 611 );
			SetDex( 391, 519 );
			SetInt( 669, 818 );

			SetHits( 1500, 2000 );

			SetDamage( 15, 25 );

			
			

			
			
			
			

			SetSkill( SkillName.Wrestling, 121.9, 130.6 );
			SetSkill( SkillName.Tactics, 114.9, 117.4 );
			SetSkill( SkillName.MagicResist, 147.7, 153.0 );
			SetSkill( SkillName.Poisoning, 122.8, 124.0 );
			SetSkill( SkillName.Magery, 121.8, 127.8 );
			SetSkill( SkillName.EvalInt, 103.6, 117.0 );
		}
				
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosUltraRich, 4 );
		}
		
		public override WeaponAbility GetWeaponAbility()
		{
			switch ( Utility.Random( 2 ) )
			{
				case 0: return WeaponAbility.ParalyzingBlow;
				case 1: return WeaponAbility.BleedAttack;
			}
		
			return null;
		}
		
		bool tick = false;
		
		public override void OnThink()
		{
			tick = !tick;
		
			if ( tick )
				return;
		
			List<Mobile> targets = new List<Mobile>();

			if ( Map != null )
				foreach ( Mobile m in GetMobilesInRange( 2 ) )
					if ( this != m && SpellHelper.ValidIndirectTarget( this, m ) && CanBeHarmful( m, false ) && ( !m.EraAOS || InLOS( m ) ) )
					{
						if ( m is BaseCreature && ((BaseCreature) m).Controlled )
							targets.Add( m );
						else if ( m.Player )
							targets.Add( m );
					}
					
			for ( int i = 0; i < targets.Count; ++i )
			{
				Mobile m = targets[ i ];

                m.Damage(5, this);
				
				if ( m.Player )
					m.SendLocalizedMessage( 1008112, Name ); // : The intense heat is damaging you!
			}			
		}
		
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Feathers{ get{ return 36; } }
//OFF		public override bool GivesMinorArtifact{ get{ return true; } }		
		
		public override int GetIdleSound() { return 0x2EF; }
		public override int GetAttackSound() { return 0x2EE; }
		public override int GetAngerSound() { return 0x2EF; }
		public override int GetHurtSound() { return 0x2F1; }
		public override int GetDeathSound()	{ return 0x2F2; }

		public GreaterPhoenix( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
}