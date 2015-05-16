using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class ElementalHunter : BaseHunter
	{
		[Constructable]
		public ElementalHunter() : base( AIType.AI_Mage )
		{
			Title = "the elemental hunter";
			int hue = 2308 + Utility.Random( 4 );
			AddItem( new ThighBoots( hue ) );
			AddItem( new SkullCap( hue ) );
			AddItem( new Surcoat( hue ) );
			AddItem( Identify( Rehued( new Spellbook(), hue ) ) );
			AddItem( Identify( Rehued( new LeatherGloves(), hue ) ) );
			AddItem( Identify( Rehued( new LeatherGorget(), hue ) ) );

			Torch torch = new Torch();
			torch.Movable = false;
			AddItem( torch );
			torch.Ignite();

			SetStr( 315, 345 );
			SetDex( 81, 95 );
			SetInt( 156, 187 );

			SetDamage( 35, 40 );

			SetSkill( SkillName.MagicResist, 110.0, 145.5 );
			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.Wrestling, 55.0, 87.5 );
			SetSkill( SkillName.Meditation, 100.0 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Rich );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public ElementalHunter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}