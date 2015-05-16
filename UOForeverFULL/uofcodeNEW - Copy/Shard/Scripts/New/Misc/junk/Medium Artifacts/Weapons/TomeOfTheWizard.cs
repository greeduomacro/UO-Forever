using System;
using Server;

namespace Server.Items
{
	public class TomeOfTheWizard : Spellbook
	{
		[Constructable]
		public TomeOfTheWizard()
		{
			this.Content = ulong.MaxValue;
			Movable = true;
			Name = "Tome Of The Wizard";
			Hue = 1512;
			LootType = LootType.Regular;
			Attributes.CastSpeed = 2;
			SkillBonuses.SetValues( 0, SkillName.Necromancy, 25.0 );
			Attributes.LowerManaCost = 25;
		}
		
		public TomeOfTheWizard( Serial serial ) : base( serial )
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

