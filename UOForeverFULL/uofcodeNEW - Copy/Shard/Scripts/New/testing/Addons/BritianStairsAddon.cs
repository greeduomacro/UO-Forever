
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BritianStairsAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1852, -4, -2, 10}, {1852, -4, 0, 10}, {1853, -4, -3, 10}// 1	2	3	
			, {1852, -4, -1, 10}, {1301, 5, -1, 10}, {1848, 1, 0, 10}// 4	5	6	
			, {1848, 0, 0, 10}, {1848, 3, 0, 5}, {1850, 4, -2, 10}// 7	8	9	
			, {1850, 3, -1, 15}, {1855, 3, -2, 15}, {1848, 4, 0, 5}// 10	11	12	
			, {1848, 2, 0, 10}, {1848, 3, 0, 10}, {1848, 2, 0, 5}// 13	14	15	
			, {1848, 1, 0, 5}, {1852, -3, 0, 15}, {1850, 4, 0, 10}// 16	17	18	
			, {1850, 3, 0, 15}, {1848, 2, 0, 15}, {1848, 1, 0, 15}// 19	20	21	
			, {1848, 0, 0, 15}, {1848, -1, 0, 15}, {1848, -2, 0, 15}// 22	23	24	
			, {1854, 2, 3, 20}, {1848, 2, -1, 10}, {1851, -3, -3, 10}// 25	26	27	
			, {1848, -3, -2, 10}, {1851, 2, -2, 15}, {1848, -3, 0, 10}// 28	29	30	
			, {1851, -1, -1, 20}, {1848, 1, 1, 20}, {1848, 1, 0, 20}// 31	32	33	
			, {1848, 0, 1, 20}, {1848, 0, 0, 20}, {1848, -1, 0, 20}// 34	35	36	
			, {1855, 2, -1, 20}, {1850, 3, -1, 0}, {1851, 1, -2, 15}// 37	38	39	
			, {1851, 1, -3, 10}, {1851, 2, -3, 10}, {1851, 3, -3, 10}// 40	41	42	
			, {1851, -2, -2, 15}, {1851, -1, -2, 15}, {1851, 1, -1, 20}// 43	44	45	
			, {1848, -3, 0, 5}, {1851, 0, -3, 10}, {1852, -2, 1, 20}// 46	47	48	
			, {1853, -3, -2, 15}, {1849, -1, 3, 20}, {1850, 4, -1, 10}// 49	50	51	
			, {1849, 0, 3, 20}, {1849, 1, 3, 20}, {1848, -1, -2, 10}// 52	53	54	
			, {1850, 2, 1, 20}, {1848, 2, -2, 10}, {1848, 3, -2, 10}// 55	57	58	
			, {1848, -3, -1, 10}, {1853, -2, -1, 20}, {1855, 4, -3, 10}// 59	60	61	
			, {1851, -2, -3, 10}, {1851, -1, -3, 10}, {1852, -2, 2, 20}// 62	63	64	
			, {1851, 0, -2, 15}, {1852, -2, 0, 20}, {1848, 0, -2, 10}// 65	66	67	
			, {1848, 1, -2, 10}, {1848, 1, -1, 15}, {1848, 2, -1, 15}// 68	70	71	
			, {1848, -1, 1, 20}, {1851, 0, -1, 20}, {1850, 2, 0, 20}// 72	74	75	
			, {1856, -2, 3, 20}, {1852, -3, -1, 15}, {1848, 3, -1, 10}// 76	77	78	
			, {1848, -2, -2, 10}, {1848, -1, 2, 20}, {1848, 0, 2, 20}// 79	80	81	
			, {1848, 1, 2, 20}, {1850, 2, 2, 20}, {1848, -2, -1, 15}// 82	83	84	
			, {1848, -1, -1, 15}, {1848, 0, -1, 15}, {1848, -2, 0, 10}// 85	86	87	
			, {1848, -1, 0, 10}, {1848, 0, 0, 5}, {1848, -1, 0, 5}// 88	89	90	
			, {1848, -2, 0, 5}, {1848, -1, -1, 10}, {1848, 0, -1, 10}// 91	92	93	
			, {1848, -2, -1, 10}, {1848, 1, -1, 10}// 94	95	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new BritianStairsAddonDeed();
			}
		}

		[ Constructable ]
		public BritianStairsAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 2572, 2, 2, 15, 0, 1, "", 1);// 56
			AddComplexComponent( (BaseAddon) this, 6571, 3, 2, 27, 0, 1, "", 1);// 69
			AddComplexComponent( (BaseAddon) this, 6571, -3, 2, 27, 0, 1, "", 1);// 73

		}

		public BritianStairsAddon( Serial serial ) : base( serial )
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

	public class BritianStairsAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new BritianStairsAddon();
			}
		}

		[Constructable]
		public BritianStairsAddonDeed()
		{
			Name = "BritianStairs";
		}

		public BritianStairsAddonDeed( Serial serial ) : base( serial )
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