using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;

namespace Server.Factions
{
	public class FactionHorseVendor : BaseFactionVendor
	{
		public FactionHorseVendor( Town town, Faction faction ) : base( town, faction, "the Horse Breeder" )
		{
			SetSkill( SkillName.AnimalLore, 64.0, 100.0 );
			SetSkill( SkillName.AnimalTaming, 90.0, 100.0 );
			SetSkill( SkillName.Veterinary, 65.0, 88.0 );
		}

		public override void InitSBInfo()
		{
		}

		public override VendorShoeType ShoeType
		{
			get{ return Female ? VendorShoeType.ThighBoots : VendorShoeType.Boots; }
		}

		public override int GetShoeHue()
		{
			return 0;
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( Utility.RandomBool() ? (Item)new QuarterStaff() : (Item)new ShepherdsCrook() );
		}

		public FactionHorseVendor( Serial serial ) : base( serial )
		{
		}

		public override void VendorBuy( Mobile buyer )
		{
			if ( this.Faction == null || Faction.Find( buyer, true ) != this.Faction )
				PrivateOverheadMessage( MessageType.Regular, 0x3B2, 1042201, buyer.NetState ); // You are not in my faction, I cannot sell you a horse!
			else if ( FactionGump.Exists( buyer ) )
				buyer.SendLocalizedMessage( 1042160 ); // You already have a faction menu open.
			else if ( buyer is PlayerMobile )
				buyer.SendGump( new HorseBreederGump( (PlayerMobile) buyer, this.Faction ) );
		}

		public override void VendorSell( Mobile from )
		{
		}

		public override bool OnBuyItems( Mobile buyer, List<BuyItemResponse> list )
		{
			return false;
		}

		public override bool OnSellItems( Mobile seller, List<SellItemResponse> list )
		{
			return false;
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