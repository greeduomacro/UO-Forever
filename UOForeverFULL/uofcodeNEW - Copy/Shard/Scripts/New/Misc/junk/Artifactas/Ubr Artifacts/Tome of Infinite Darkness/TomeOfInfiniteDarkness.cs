using System;
using Server;

namespace Server.Items
{
	public class TomeOfInfiniteDarkness : Spellbook
	{
				
		[Constructable]
		public TomeOfInfiniteDarkness()
		{
			this.Content = ulong.MaxValue;
			Movable = true;
			Name = "Tome Of Infinite Darkness";
			Hue = 2019;

			Attributes.BonusInt = 12;
			SkillBonuses.SetValues( 0, SkillName.Necromancy, 10.0 );
			Slayer = SlayerName.BloodDrinking;
			Attributes.NightSight = 20;
			
			Attributes.RegenHits = 5;
			Attributes.CastSpeed = 2;
			Attributes.LowerManaCost = 20;
     		Attributes.LowerRegCost = 25;
		}
		
		public TomeOfInfiniteDarkness( Serial serial ) : base( serial )
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

