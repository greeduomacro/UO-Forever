using System;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Server.Mobiles
{
	public class FamousPirates : BaseCreature
	{
		[Constructable]
		public FamousPirates(AIType iAI, FightMode iFightMode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed) : base(iAI, iFightMode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed)
		{
			this.Body = 400;
			this.Hue = Utility.RandomSkinHue();

			this.Skills[SkillName.DetectHidden].Base = 100;
			this.Skills[SkillName.MagicResist].Base = 100;
			this.Skills[SkillName.Magery].Base = 100;
            this.Skills[SkillName.Healing].Base = 100;

			int Hue = 0 ;

			Utility.AssignRandomHair( this, Hue );

			LeatherGloves glv = new LeatherGloves();
			glv.Hue = Hue;
			glv.LootType = LootType.Regular;
			AddItem(glv);

			Container pack = new Backpack();

			pack.Movable = false;

			AddItem( pack );

			Kills = 5;
		}

        public FamousPirates(Serial serial) : base(serial) { }
    
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
		
		public Spellbook FullSpellbook()
		{
			Spellbook book = new Spellbook();
			book.Movable = false;
			book.LootType = LootType.Blessed;
			book.Content =0xFFFFFFFFFFFFFFFF;
			return book;
		}
	}
}

