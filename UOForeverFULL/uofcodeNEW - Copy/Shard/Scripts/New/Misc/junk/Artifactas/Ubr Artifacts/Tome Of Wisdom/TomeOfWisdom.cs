using System;
using Server;

namespace Server.Items
{
	public class TomeOfWisdom : Spellbook
	{
	
		[Constructable]
		public TomeOfWisdom()
		{
			this.Content = ulong.MaxValue;
			Movable = true;
			Name = "Tome Of Wisdom";
			Hue = 1152;

			Attributes.BonusInt = 9;
  			Attributes.Luck = 25;
			SkillBonuses.SetValues( 0, SkillName.Magery, 10.0 );
			Slayer = SlayerName.Exorcism;
			Attributes.NightSight = 15;
			
			Attributes.RegenMana = 3;
			Attributes.SpellDamage = 12;
			Attributes.LowerManaCost = 25;
      		Attributes.LowerRegCost = 20;
		}
		
		public TomeOfWisdom( Serial serial ) : base( serial )
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

