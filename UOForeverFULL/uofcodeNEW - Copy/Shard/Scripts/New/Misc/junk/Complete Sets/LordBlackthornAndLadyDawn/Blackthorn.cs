using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
public class Blackthorn : BaseCreature
{

		public override WeaponAbility GetWeaponAbility()
		{
			switch ( Utility.Random( 3 ) )
			{
				default:
				case 0: return WeaponAbility.DoubleStrike;
				case 1: return WeaponAbility.WhirlwindAttack;
				case 2: return WeaponAbility.CrushingBlow;
			}
		}

 [Constructable]
 public Blackthorn() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.1, 0.3 ) // mage or melee?
 {
  Name = "Lord Blackthorn";
  Body = 769;

  SetStr( 1205, 1425 );
  SetDex( 272, 350 );
  SetInt( 2505, 2750 );

  SetHits( 55000 );
  SetStam( 402, 600 );

  SetDamage( 40, 50 );

  

  
  
  
  
  

  SetSkill( SkillName.MagicResist, 120.0 );
  SetSkill( SkillName.Tactics, 160.0, 150.0 );
  SetSkill( SkillName.Swords, 180.0 );
  SetSkill( SkillName.Wrestling, 90.0, 100.0 );
  SetSkill( SkillName.Magery, 200.0 );
  SetSkill( SkillName.EvalInt, 200.0 );

  Fame = 99000;
  Karma = -1;

  VirtualArmor = 80;

  PackGem();
  PackGem();
  PackGem();
  PackGem();
  PackGem();
  PackGem();
  PackGem();
  PackGem();
  PackGem();
  PackGem();
  PackGold( 1000, 1500 );
  PackGold( 2000, 3500 );
  PackGold( 5000, 15000 );
  PackGold( 10000, 30000 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackWeapon( 8, 9 );
  PackArmor( 8, 9 );
  PackArmor( 8, 9 );
  PackArmor( 8, 9 );
  PackArmor( 8, 9 );
  PackArmor( 8, 9 );
  PackArmor( 8, 9 );
  PackArmor( 8, 9 );
  PackArmor( 8, 9 );
  PackArmor( 8, 9 );
  PackArmor( 8, 9 );
  PackArmor( 8, 9 );
  PackScroll( 7, 8 );
  PackScroll( 7, 8 );
  PackScroll( 7, 8 );
  PackScroll( 7, 8 );
  PackScroll( 7, 8 );
  PackScroll( 7, 8 );
                               }

 public override bool AlwaysMurderer{ get{ return true; } }
 public override bool BardImmune{ get{ return true; } }
 public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

 public override bool ShowFameTitle{ get{ return false; } }
 public override bool ClickTitle{ get{ return false; } }

 public override void OnGaveMeleeAttack( Mobile defender )
 {
  base.OnGaveMeleeAttack( defender );

  if ( 0.1 >= Utility.RandomDouble() ) // 10% chance to drop or throw an unholy bone
   AddUnholyBone( defender, 0.25 );
 }

 public override void OnGotMeleeAttack( Mobile attacker )
 {
  base.OnGotMeleeAttack( attacker );

  if ( 0.1 >= Utility.RandomDouble() ) // 10% chance to drop or throw an unholy bone
   AddUnholyBone( attacker, 0.25 );
 }

 public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
 {
  base.AlterDamageScalarFrom( caster, ref scalar );

  if ( 0.1 >= Utility.RandomDouble() ) // 10% chance to throw an unholy bone
   AddUnholyBone( caster, 1.0 );
 }

 public void AddUnholyBone( Mobile target, double chanceToThrow )
 {
  if ( chanceToThrow >= Utility.RandomDouble() )
  {
   Direction = GetDirectionTo( target );
   MovingEffect( target, 0xF7E, 10, 1, true, false, 0x496, 0 );
   new DelayTimer( this, target ).Start();
  }
  else
  {
   new UnholyBone().MoveToWorld( Location, Map );
  }
 }

 private class DelayTimer : Timer
 {
  private Mobile m_Mobile;
  private Mobile m_Target;

  public DelayTimer( Mobile m, Mobile target ) : base( TimeSpan.FromSeconds( 1.0 ) )
  {
   m_Mobile = m;
   m_Target = target;
  }

  protected override void OnTick()
  {
   if ( m_Mobile.CanBeHarmful( m_Target ) )
   {
    m_Mobile.DoHarmful( m_Target );
    m_Target.Damage(Utility.RandomMinMax(10, 20), m_Mobile);
    new UnholyBone().MoveToWorld( m_Target.Location, m_Target.Map );
   }
  }
 }

 public Blackthorn( Serial serial ) : base( serial )
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
