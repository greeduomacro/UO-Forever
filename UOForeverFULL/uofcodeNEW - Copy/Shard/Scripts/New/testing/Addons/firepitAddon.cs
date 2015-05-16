
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class firepitAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {6078, 1, -2, 2}, {6078, 2, -2, 2}, {6079, -1, -2, 2}// 1	2	3	
			, {6079, 0, -2, 2}, {6077, 1, -1, 2}, {2861, -1, -1, 1}// 4	5	6	
			, {6089, 3, 0, 2}, {6089, 2, 0, 2}, {2861, 1, -1, 1}// 7	8	10	
			, {6077, -1, -1, 2}, {6077, 0, -1, 2}, {6079, -2, -2, 2}// 11	12	13	
			, {7686, 1, 0, 2}, {6089, 1, 0, 2}, {6079, -2, -1, 2}// 14	15	16	
			, {6007, 2, 0, 0}, {6089, 0, 0, 2}, {6082, 3, 0, 2}// 18	19	20	
			, {4334, 2, 0, 2}, {6079, -2, 0, 2}, {2860, -1, 0, 2}// 21	22	23	
			, {2861, 3, -1, 1}, {6077, 3, -1, 2}, {2861, 0, -1, 1}// 24	25	26	
			, {2861, 2, -1, 1}, {6077, 3, -2, 2}, {6077, -1, 0, 2}// 27	28	29	
			, {6077, 2, -1, 2}, {3343, -1, -2, 2}, {6004, 1, 2, 3}// 30	31	32	
			, {6084, 0, 3, 2}, {4970, 1, 2, 0}, {7684, 2, 3, 2}// 33	34	35	
			, {3118, 1, 2, 2}, {6077, -1, 2, 2}, {6077, -1, 3, 2}// 36	37	39	
			, {7683, 3, 2, 2}, {6007, 2, 1, 7}, {7685, 0, 1, 2}// 40	42	43	
			, {3120, 1, 2, 2}, {4973, 2, 2, 4}, {6090, 3, 1, 2}// 44	45	46	
			, {7685, 0, 2, 2}, {3349, -2, 3, 3}, {7682, 3, 1, 2}// 47	48	49	
			, {14133, 1, 2, 7}, {14133, 2, 2, 10}, {6092, 0, 1, 2}// 50	51	52	
			, {3345, -2, 1, 2}, {7682, 1, 3, 2}, {3120, 2, 2, 7}// 54	55	56	
			, {6079, -2, 2, 2}, {3379, 1, 1, 14}, {6077, -1, 1, 2}// 57	58	59	
			, {6091, 2, 3, 2}, {3119, 1, 2, 2}, {2860, -1, 2, 2}// 60	61	62	
			, {6091, 1, 3, 2}, {2860, -1, 1, 2}, {2860, -1, 3, 2}// 63	64	65	
			, {4334, 0, 3, 2}, {3379, 3, 2, 2}, {6079, -2, 3, 2}// 66	68	69	
			, {6079, -2, 1, 2}, {6004, 2, 1, 2}, {4971, 2, 1, 2}// 70	72	74	
			, {6092, 0, 2, 2}, {6007, 1, 1, 2}, {6012, 1, 1, 2}// 75	76	77	
			, {3379, -2, 3, 2}, {6090, 3, 2, 2}, {3119, 1, 2, 2}// 78	80	81	
			, {6083, 3, 3, 2}, {3350, -2, 3, 2}// 82	83	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new firepitAddonDeed();
			}
		}

		[ Constructable ]
		public firepitAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 5164, -1, -1, 4, 38, 1, "", 1);// 9
			AddComplexComponent( (BaseAddon) this, 5168, -1, -1, 6, 38, 1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 3561, 2, 1, 4, 0, 1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 14742, 2, 2, 6, 0, 1, "", 1);// 41
			AddComplexComponent( (BaseAddon) this, 3555, 2, 2, 9, 0, 1, "", 1);// 53
			AddComplexComponent( (BaseAddon) this, 3555, 1, 2, 5, 0, 1, "", 1);// 67
			AddComplexComponent( (BaseAddon) this, 14732, 1, 2, 3, 0, 1, "", 1);// 71
			AddComplexComponent( (BaseAddon) this, 3561, 1, 2, 3, 0, 1, "", 1);// 73
			AddComplexComponent( (BaseAddon) this, 3561, 2, 2, 2, 0, 1, "", 1);// 79

		}

		public firepitAddon( Serial serial ) : base( serial )
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

	public class firepitAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new firepitAddon();
			}
		}

		[Constructable]
		public firepitAddonDeed()
		{
			Name = "firepit";
		}

		public firepitAddonDeed( Serial serial ) : base( serial )
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