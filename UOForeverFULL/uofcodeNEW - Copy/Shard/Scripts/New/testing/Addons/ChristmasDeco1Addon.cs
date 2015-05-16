
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ChristmasDeco1Addon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {6089, -2, 4, 0}, {6078, -3, 1, 0}, {3677, 5, 1, 0}// 1	2	3	
			, {9003, 1, 2, 0}, {6090, -4, 2, 0}, {6082, -3, 3, 0}// 4	5	6	
			, {6087, 3, 4, 0}, {6078, 0, -1, 0}, {6085, -3, -2, 0}// 7	8	9	
			, {6078, 1, 2, 0}, {6078, 2, 3, 0}, {6092, 4, 1, 0}// 10	11	12	
			, {6090, -4, 0, 0}, {6091, -1, -2, 0}, {6092, 4, 2, 0}// 14	15	19	
			, {6086, 4, -1, 0}, {6078, 0, 3, 0}, {6088, -4, 3, 0}// 20	21	22	
			, {6091, 0, -2, 0}, {6078, 0, 1, 0}, {6078, 2, 1, 0}// 23	24	25	
			, {6078, -2, -1, 0}, {6078, -1, -1, 0}, {6078, 2, -1, 0}// 26	27	28	
			, {6091, -2, -2, 0}, {6078, -2, 2, 0}, {6078, -3, 2, 0}// 29	30	31	
			, {6078, 1, 1, 0}, {6078, 2, 2, 0}, {6078, 1, 0, 0}// 32	33	34	
			, {6085, -2, -2, 0}, {6085, -4, -1, 0}, {6078, -1, 1, 0}// 35	36	37	
			, {6078, 1, 3, 0}, {9079, -1, -3, 0}, {9079, 2, -3, 0}// 40	41	43	
			, {9054, 0, -3, 0}, {6090, -4, 1, 0}, {14186, 0, 1, 20}// 44	45	46	
			, {2602, 0, -2, 0}, {6078, -1, 0, 0}, {2602, 1, -2, 0}// 47	48	49	
			, {6089, -1, 4, 0}, {6078, -2, 3, 0}, {6078, -2, 0, 0}// 54	55	56	
			, {6091, 1, -2, 0}, {6078, 3, 2, 0}, {6089, 0, 4, 0}// 57	58	59	
			, {6078, 0, 0, 0}, {6092, 4, 0, 0}, {6078, 3, 0, 0}// 60	61	62	
			, {6083, -3, -1, 0}, {6091, 2, -2, 0}, {6084, 3, -1, 0}// 64	65	66	
			, {6078, 2, 0, 0}, {6078, 3, 1, 0}, {6088, -3, 4, 0}// 67	68	69	
			, {6086, 3, -2, 0}, {6078, -1, 3, 0}, {6078, 0, 2, 0}// 73	74	75	
			, {6081, 3, 3, 0}, {6078, -2, 1, 0}, {6089, 1, 4, 0}// 76	77	78	
			, {6078, -1, 2, 0}, {6078, -3, 0, 0}, {6089, 2, 4, 0}// 79	80	81	
			, {6087, 4, 3, 0}, {6077, 1, -1, 0}// 82	83	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new ChristmasDeco1AddonDeed();
			}
		}

		[ Constructable ]
		public ChristmasDeco1Addon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 9003, -1, 1, 0, 593, -1, "", 1);// 13
			AddComplexComponent( (BaseAddon) this, 3631, 1, 1, 3, 0, 2, "a snowy scene of Vesper", 1);// 16
			AddComplexComponent( (BaseAddon) this, 9056, 1, -3, 0, 0, 1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 9003, -1, 0, 0, 38, -1, "", 1);// 18
			AddComplexComponent( (BaseAddon) this, 9003, 0, 0, 0, 1359, -1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 9002, 0, 2, 0, 77, -1, "", 1);// 39
			AddComplexComponent( (BaseAddon) this, 9000, 3, 2, 0, 769, -1, "", 1);// 42
			AddComplexComponent( (BaseAddon) this, 9003, 1, 1, 0, 4, -1, "", 1);// 50
			AddComplexComponent( (BaseAddon) this, 9003, 0, 2, 0, 88, -1, "", 1);// 51
			AddComplexComponent( (BaseAddon) this, 9003, -1, 2, 0, 38, -1, "", 1);// 52
			AddComplexComponent( (BaseAddon) this, 9003, 1, 2, 6, 193, -1, "", 1);// 53
			AddComplexComponent( (BaseAddon) this, 9003, -1, 1, 6, 108, -1, "", 1);// 63
			AddComplexComponent( (BaseAddon) this, 5703, 1, 2, 9, 0, 1, "", 1);// 70
			AddComplexComponent( (BaseAddon) this, 5703, 0, 1, 2, 0, 1, "", 1);// 71
			AddComplexComponent( (BaseAddon) this, 5703, 1, 1, 0, 0, 1, "", 1);// 72

		}

		public ChristmasDeco1Addon( Serial serial ) : base( serial )
		{
		}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (name != null && name.Length > 0)
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType) lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
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

	public class ChristmasDeco1AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new ChristmasDeco1Addon();
			}
		}

		[Constructable]
		public ChristmasDeco1AddonDeed()
		{
			Name = "ChristmasDeco1";
		}

		public ChristmasDeco1AddonDeed( Serial serial ) : base( serial )
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