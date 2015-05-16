using System;
using Server;
using Server.Ethics;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a holy corpse" )]
	public class HolySteed : BaseMount
	{
		public override bool IsDispellable { get{ return true; } }
		public override bool IsBondable { get { return false; } }

		//public override bool HasBreath { get { return false; } }
		//public override bool CanBreath { get { return false; } }

		public override string DefaultName{ get{ return "a holy steed"; } }

        public override int InternalItemItemID { get { return 0x3EA0; } }

		[Constructable]
		public HolySteed()
            : base(0xC8, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
		{
            BaseSoundID = 0xA8;

		    Hue = 2955;
            SetStr(400, 550);
            SetDex(180);
            SetInt(51, 55);

            SetHits(240);
            SetMana(0);

            SetDamage(2, 4);

            SetSkill(SkillName.MagicResist, 25.1, 30.0);
            SetSkill(SkillName.Tactics, 29.3, 44.0);
            SetSkill(SkillName.Wrestling, 29.3, 44.0);

            Fame = 300;
            Karma = 300;

            Tamable = false;
            ControlSlots = 1;
		}

		public override FoodType FavoriteFood { get { return FoodType.FruitsAndVeggies | FoodType.GrainsAndHay; } }

		public HolySteed( Serial serial )
			: base( serial )
		{
		}

		public override string ApplyNameSuffix( string suffix )
		{
			if ( suffix.Length == 0 )
				suffix = Ethic.Hero.Definition.NPCAdjunct.String;
			else
				suffix = String.Concat( suffix, " ", Ethic.Hero.Definition.NPCAdjunct.String );

			return base.ApplyNameSuffix( suffix );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Ethic.Find( from ) != Ethic.Hero )
				from.SendMessage( "You may not ride this steed." );
			else
				base.OnDoubleClick( from );
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