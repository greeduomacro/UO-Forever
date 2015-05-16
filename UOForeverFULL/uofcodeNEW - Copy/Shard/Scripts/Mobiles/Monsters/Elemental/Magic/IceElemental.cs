using System;
using Server;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ice elemental corpse" )]
	public class IceElemental : BaseCreature
	{
		public override string DefaultName{ get{ return "an ice elemental"; } }

		[Constructable]
		public IceElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 161;
			BaseSoundID = 268;

            Alignment = Alignment.Elemental;

			SetStr( 156, 185 );
			SetDex( 96, 115 );
			SetInt( 171, 192 );

			SetHits( 94, 111 );

			SetDamage( 10, 21 );

			
			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 10.5, 60.0 );
			SetSkill( SkillName.Magery, 10.5, 60.0 );
			SetSkill( SkillName.MagicResist, 30.1, 80.0 );
			SetSkill( SkillName.Tactics, 70.1, 100.0 );
			SetSkill( SkillName.Wrestling, 60.1, 100.0 );

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 40;

			PackItem( new BlackPearl() );
			PackReg( 3 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.Gems, 2 );

            if ( 0.06 > Utility.RandomDouble() )// 6 percent 
                { SkillScroll scroll = new SkillScroll(); scroll.Randomize(); PackItem(scroll); }
		}
		public override bool BleedImmune{ get{ return true; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public IceElemental( Serial serial ) : base( serial )
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

		public override bool HasAura{ get{ return true; } }
	}
}