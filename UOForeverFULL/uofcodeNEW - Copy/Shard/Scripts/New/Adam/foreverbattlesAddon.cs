
////////////////////////////////////////
//                                     //
//     //
// Addon Generator  //
//          //
//             //
//                                     //
////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class foreverbattlesAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {105, -5, -3, 24}, {105, -6, -3, 24}, {1872, -7, -1, 0}// 1	2	3	
			, {1872, -6, -1, 0}, {1872, -5, -1, 0}, {1872, -7, -2, 2}// 4	5	6	
			, {1872, -6, -2, 2}, {1872, -5, -2, 2}, {1881, -8, -3, 2}// 7	8	12	
			, {1875, -7, -3, 2}, {1875, -6, -3, 2}, {1875, -5, -3, 2}// 13	14	15	
			, {1876, -8, -1, 2}, {1876, -8, -2, 2}, {90, -8, -3, 4}// 16	17	18	
			, {105, -7, -3, 24}, {108, -8, -3, 24}, {106, -8, -1, 24}// 19	20	21	
			, {106, -8, -2, 24}, {111, -7, -3, 8}, {1872, -7, 0, 0}// 22	23	24	
			, {1872, -7, 1, 0}, {1872, -7, 2, 0}, {1872, -7, 3, 2}// 25	26	27	
			, {1872, -6, 0, 0}, {1872, -6, 1, 0}, {1872, -6, 2, 0}// 28	29	30	
			, {1872, -6, 3, 2}, {1872, -5, 0, 0}, {1872, -5, 1, 0}// 31	32	33	
			, {1872, -5, 2, 0}, {1872, -5, 3, 2}, {1873, -7, 4, 2}// 34	35	36	
			, {1873, -6, 4, 2}, {1873, -5, 4, 2}, {1883, -8, 4, 2}// 37	38	48	
			, {1876, -8, 3, 2}, {1876, -8, 2, 2}, {1876, -8, 1, 2}// 49	50	51	
			, {1876, -8, 0, 2}, {110, -8, 4, 4}, {105, -7, 4, 24}// 52	53	54	
			, {106, -8, 3, 24}, {106, -8, 4, 24}, {106, -8, 2, 24}// 55	56	57	
			, {106, -8, 1, 24}, {106, -8, 0, 24}, {111, -7, 4, 8}// 58	59	60	
			, {105, -6, 4, 24}, {105, -5, 4, 24}, {105, -4, -3, 24}// 61	62	63	
			, {1872, -4, -1, 0}, {1872, -4, -2, 2}, {1875, -4, -3, 2}// 64	65	67	
			, {1874, -3, -1, 2}, {1874, -3, -2, 2}, {1884, -3, -3, 2}// 68	69	70	
			, {2760, -2, -1, 2}, {2760, -1, -1, 2}, {2760, 0, -1, 2}// 71	72	73	
			, {112, -3, -3, 8}, {2760, 1, -1, 2}, {2760, 2, -1, 2}// 74	75	76	
			, {2760, 3, -1, 2}, {2760, 4, -1, 2}, {2760, 5, -1, 2}// 77	78	79	
			, {2760, 6, -1, 2}, {2760, 7, -1, 2}, {2760, 8, -1, 2}// 80	81	82	
			, {108, -3, -3, 24}, {105, -3, -3, 24}, {2760, 3, -2, 2}// 83	84	85	
			, {2760, 2, -2, 2}, {2760, 1, -2, 2}, {2760, 0, -2, 2}// 86	87	88	
			, {2760, -1, -2, 2}, {2760, -2, -2, 2}, {2760, 4, -2, 2}// 89	90	91	
			, {2760, 5, -2, 2}, {2760, 6, -2, 2}, {2760, 7, -2, 2}// 92	93	94	
			, {2760, 8, -2, 2}, {105, -2, -3, 2}, {105, -1, -3, 2}// 95	96	97	
			, {105, 0, -3, 2}, {105, 1, -3, 2}, {105, 2, -3, 2}// 98	99	100	
			, {105, 3, -3, 2}, {105, 4, -3, 2}, {105, 5, -3, 2}// 101	102	103	
			, {105, 6, -3, 2}, {105, 7, -3, 2}, {105, 8, -3, 2}// 104	105	106	
			, {1872, -4, 0, 0}, {1872, -4, 1, 0}, {1872, -4, 2, 0}// 107	108	109	
			, {1872, -4, 3, 2}, {1873, -4, 4, 2}, {1882, -3, 4, 2}// 110	111	115	
			, {1874, -3, 3, 2}, {1874, -3, 2, 2}, {1874, -3, 1, 2}// 116	117	118	
			, {1874, -3, 0, 2}, {2760, -2, 0, 2}, {2760, -2, 1, 2}// 119	120	121	
			, {2760, -2, 2, 2}, {2760, -1, 0, 2}, {2760, -1, 1, 2}// 122	123	124	
			, {2760, -1, 2, 2}, {2760, 0, 0, 2}, {2760, 0, 1, 2}// 125	126	127	
			, {2760, 0, 2, 2}, {2760, 1, 0, 2}, {2760, 1, 1, 2}// 128	129	130	
			, {2760, 1, 2, 2}, {2760, 5, 3, 2}, {2760, 2, 0, 2}// 131	132	133	
			, {2760, 2, 1, 2}, {2760, 2, 2, 2}, {2760, 5, 4, 2}// 134	135	136	
			, {2760, 3, 0, 2}, {2760, 3, 1, 2}, {2760, 3, 2, 2}// 137	138	139	
			, {2760, 6, 3, 2}, {2760, 4, 0, 2}, {2760, 4, 1, 2}// 140	141	142	
			, {2760, 4, 2, 2}, {2760, 6, 4, 2}, {2760, 5, 0, 2}// 143	144	145	
			, {2760, 5, 1, 2}, {2760, 5, 2, 2}, {2760, 7, 3, 2}// 146	147	148	
			, {2760, 6, 0, 2}, {2760, 6, 1, 2}, {2760, 6, 2, 2}// 149	150	151	
			, {2760, 7, 4, 2}, {2760, 7, 0, 2}, {2760, 7, 1, 2}// 152	153	154	
			, {2760, 7, 2, 2}, {2760, 8, 3, 2}, {2760, 8, 0, 2}// 155	156	157	
			, {2760, 8, 1, 2}, {2760, 8, 2, 2}, {2760, 8, 4, 2}// 158	159	160	
			, {2760, 1, 4, 2}, {2760, 1, 3, 2}, {2760, 0, 4, 2}// 161	162	163	
			, {2760, 0, 3, 2}, {2760, -1, 4, 2}, {2760, -1, 3, 2}// 164	165	166	
			, {2760, 2, 3, 2}, {2760, 2, 4, 2}, {2760, 3, 3, 2}// 167	168	169	
			, {2760, 3, 4, 2}, {2760, 4, 3, 2}, {2760, 4, 4, 2}// 170	171	172	
			, {112, -3, 4, 4}, {2760, -2, 4, 2}, {2760, -2, 3, 2}// 173	174	175	
			, {105, -2, 4, 2}, {105, -1, 4, 2}, {105, 0, 4, 2}// 176	177	178	
			, {105, 1, 4, 2}, {105, 2, 4, 2}, {105, 3, 4, 2}// 179	180	181	
			, {105, 4, 4, 2}, {105, 5, 4, 2}, {105, 6, 4, 2}// 182	183	184	
			, {105, 7, 4, 2}, {105, 8, 4, 2}, {105, -4, 4, 24}// 185	186	187	
			, {105, -3, 4, 24}// 188	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new foreverbattlesAddonDeed();
			}
		}

		[ Constructable ]
		public foreverbattlesAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 3633, -6, -1, 5, 0, 0, "", 1);// 9
			AddComplexComponent( (BaseAddon) this, 3633, -7, -1, 5, 0, 0, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 3633, -5, -1, 5, 0, 0, "", 1);// 11
			AddComplexComponent( (BaseAddon) this, 3633, -7, 2, 5, 0, 0, "", 1);// 39
			AddComplexComponent( (BaseAddon) this, 3633, -7, 1, 5, 0, 0, "", 1);// 40
			AddComplexComponent( (BaseAddon) this, 3633, -7, 0, 5, 0, 0, "", 1);// 41
			AddComplexComponent( (BaseAddon) this, 3633, -6, 0, 5, 0, 0, "", 1);// 42
			AddComplexComponent( (BaseAddon) this, 3633, -6, 1, 5, 0, 0, "", 1);// 43
			AddComplexComponent( (BaseAddon) this, 3633, -6, 2, 5, 0, 0, "", 1);// 44
			AddComplexComponent( (BaseAddon) this, 3633, -5, 2, 5, 0, 0, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 3633, -5, 1, 5, 0, 0, "", 1);// 46
			AddComplexComponent( (BaseAddon) this, 3633, -5, 0, 5, 0, 0, "", 1);// 47
			AddComplexComponent( (BaseAddon) this, 3633, -4, -1, 5, 0, 0, "", 1);// 66
			AddComplexComponent( (BaseAddon) this, 3633, -4, 0, 5, 0, 0, "", 1);// 112
			AddComplexComponent( (BaseAddon) this, 3633, -4, 1, 5, 0, 0, "", 1);// 113
			AddComplexComponent( (BaseAddon) this, 3633, -4, 2, 5, 0, 0, "", 1);// 114

		}

		public foreverbattlesAddon( Serial serial ) : base( serial )
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

	public class foreverbattlesAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new foreverbattlesAddon();
			}
		}

		[Constructable]
		public foreverbattlesAddonDeed()
		{
			Name = "foreverbattles";
		}

		public foreverbattlesAddonDeed( Serial serial ) : base( serial )
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