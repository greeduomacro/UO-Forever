using Server.Engines.Craft;

namespace Server.Items
{
	public class SoulForgeAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {16996, 0, -1, 0}, {16998, 2, -1, 0}, {17004, 0, 1, 0}// 1	2	3	
			, {17007, -1, 2, 0}, {17009, 1, 2, 0}, {17000, 0, 0, 0}// 4	5	6	
			, {17006, 2, 1, 0}, {16995, -1, -1, 0}, {17003, -1, 1, 0}// 7	8	9	
			, {17001, 1, 0, 0}, {17008, 0, 2, 0}, {16997, 1, -1, 0}// 10	11	12	
			, {17010, 2, 2, 0}, {17002, 2, 0, 0}, {17005, 1, 1, 0}// 13	14	15	
			, {16999, -1, 0, 0}// 16	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new SoulForgeAddonDeed();
			}
		}

		[ Constructable ]
		public SoulForgeAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new SoulForgeComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);


		}

		public SoulForgeAddon( Serial serial ) : base( serial )
		{
		}

        public override void OnChop(Mobile from)
        {
            from.SendMessage(54, "You must page a GM to have a Soulforge moved.");
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    public class SoulForgeComponent : AddonComponent
    {
        [Constructable]
        public SoulForgeComponent(int itemID)
            : base(itemID)
        {
        }

        public SoulForgeComponent(Serial serial)
            : base(serial)
        {
        }


        public override void OnDoubleClick(Mobile from)
        {
            var item = from.FindItemOnLayer(Layer.OneHanded) as SmithHammerWeapon;
            if (item == null && from.Backpack != null)
            {
                item = from.Backpack.FindItemByType<SmithHammerWeapon>();
            }
            if (item != null)
            {
                from.SendGump(new CraftGump(from, DefLegendaryBlacksmithy.CraftSystem, item, null));
            }
            else
            {
                from.SendMessage(54, "You must have a smithing hammer to work the Soulforge!");
            }

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

	public class SoulForgeAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new SoulForgeAddon();
			}
		}

		[Constructable]
		public SoulForgeAddonDeed()
		{
			Name = "a soulforge";
		    LootType = LootType.Blessed;
		}

		public SoulForgeAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}