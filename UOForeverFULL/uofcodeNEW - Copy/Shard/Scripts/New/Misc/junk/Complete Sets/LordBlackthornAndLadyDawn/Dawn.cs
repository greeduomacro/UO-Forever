using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
public class Dawn : BaseCreature
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
 public Dawn() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.1, 0.3 ) // mage or melee?
 {
  Name = "Lady Dawn";
  Body = 774;

  SetStr( 1005, 1225 );
  SetDex( 222, 300 );
  SetInt( 1505, 1750 );

  SetHits( 25000 );
  SetStam( 302, 400 );

  SetDamage( 30, 40 );

  

  
  
  
  
  

  SetSkill( SkillName.MagicResist, 100.0 );
  SetSkill( SkillName.Tactics, 100.0, 110.0 );
  SetSkill( SkillName.Swords, 130.0 );
  SetSkill( SkillName.Wrestling, 100.0, 110.0 );
  SetSkill( SkillName.Magery, 115.0 );
  SetSkill( SkillName.EvalInt, 115.0 );

  Fame = 59000;
  Karma = -1;

  VirtualArmor = 70;

  PackGem();
  PackGem();
  PackGem();
  PackGem();
  PackGold( 300, 800 );
  PackGold( 1000, 1500 );
  PackGold( 2000, 800 );
  PackGold( 10000, 20000 );
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
  PackScroll( 7, 8 );
  PackScroll( 7, 8 );
  PackScroll( 7, 8 );
                               }

 public override bool BardImmune{ get{ return true; } }
 public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

 public override bool ShowFameTitle{ get{ return false; } }
 public override bool ClickTitle{ get{ return false; } }


 public Dawn( Serial serial ) : base( serial )
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
