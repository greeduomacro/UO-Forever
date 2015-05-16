
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class fireandlightningAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {3821, -3, -3, 20}, {3874, -1, -2, 8}, {3821, 6, -3, 26}// 1	2	3	
			, {6021, -5, -5, 8}, {3821, -1, -2, 26}, {3821, -2, -2, 36}// 4	5	6	
			, {3821, -2, -2, 26}, {3821, 1, -5, 20}, {6020, 3, -6, 8}// 7	9	10	
			, {3821, -5, -3, 20}, {6020, -1, -6, 8}, {3821, -4, -3, 20}// 11	12	13	
			, {3864, 6, -3, 8}, {3287, 6, -3, 6}, {3874, 4, -2, 8}// 14	15	16	
			, {3821, 7, -3, 26}, {3821, 1, -4, 20}, {3821, 3, -2, 26}// 17	18	20	
			, {3870, 0, -5, 2}, {3821, 0, -2, 26}, {3821, 4, -2, 26}// 21	22	23	
			, {3870, -1, -2, 8}, {3287, -4, -3, 0}, {6020, 5, -6, 8}// 24	26	27	
			, {3821, 5, -3, 36}, {3821, -1, -2, 26}, {14068, -3, -2, 6}// 28	30	31	
			, {3821, -4, -3, 20}, {3821, -5, -3, 30}, {14138, 6, -2, 6}// 32	33	35	
			, {3287, 0, -5, 0}, {3870, 4, -2, 8}, {3821, 5, -3, 26}// 36	37	38	
			, {3821, 5, -2, 26}, {3821, 5, -1, 26}, {6020, 7, -6, 8}// 39	40	41	
			, {6020, 0, -6, 8}, {3821, -1, -5, 20}, {3821, -4, -2, 20}// 42	43	44	
			, {3287, 4, -2, 6}, {3286, 0, -5, 0}, {3870, -4, -3, 2}// 45	46	47	
			, {3286, 4, -2, 6}, {3821, 0, -1, 26}, {3821, 4, -2, 26}// 48	49	50	
			, {3286, -4, -3, 0}, {3821, -4, -2, 20}, {3821, 0, -4, 20}// 52	53	54	
			, {3821, 1, -4, 20}, {3821, 6, -3, 26}, {3821, 7, -3, 26}// 55	56	57	
			, {6020, 6, -6, 8}, {3862, -1, -2, 8}, {3287, -1, -2, 6}// 58	59	60	
			, {14138, 0, -3, 6}, {6020, 2, -6, 8}, {3821, 7, -2, 26}// 61	62	63	
			, {3864, -1, -2, 8}, {3286, -1, -2, 6}, {3821, 4, -2, 26}// 64	65	67	
			, {3821, 3, -2, 36}, {6021, -5, -3, 8}, {3821, 0, -1, 26}// 68	69	70	
			, {3821, 6, -2, 26}, {3821, 0, -4, 20}, {6020, -4, -6, 8}// 71	73	74	
			, {6020, -3, -6, 8}, {6021, -5, -2, 8}, {3862, 4, -2, 8}// 75	76	77	
			, {3821, -1, -2, 26}, {3821, -1, -1, 26}, {3874, 0, -5, 2}// 78	79	80	
			, {3821, -3, -2, 20}, {3821, -3, -3, 20}, {3864, 0, -5, 2}// 81	82	83	
			, {3821, 5, -2, 26}, {14133, 0, -2, 6}, {3821, -1, -1, 26}// 84	85	86	
			, {3821, 4, -1, 26}, {3821, 5, -1, 26}, {3821, 7, -2, 26}// 87	88	89	
			, {14138, -4, -2, 0}, {6020, 4, -6, 8}, {3821, 0, -5, 20}// 90	91	92	
			, {3821, 0, -2, 26}, {3821, 5, -2, 26}, {3821, -3, -2, 20}// 94	95	96	
			, {3821, -4, -2, 20}, {3862, -4, -3, 2}, {3864, -4, -3, 2}// 98	99	100	
			, {6020, 1, -6, 8}, {3821, 0, -4, 20}, {3821, 0, -5, 20}// 101	102	103	
			, {3821, -1, -5, 30}, {3874, -4, -3, 2}, {3862, 0, -5, 2}// 104	105	106	
			, {3821, 1, -5, 20}, {3821, 0, -5, 20}, {6023, -5, -6, 8}// 107	108	109	
			, {6021, -5, -1, 8}, {3821, -4, -3, 20}, {3821, -3, -3, 20}// 110	111	112	
			, {3821, 1, -5, 20}, {14138, -2, -1, 6}, {14138, 4, -2, 8}// 113	114	115	
			, {14133, 5, -2, 6}, {3821, 0, -2, 26}, {3821, -1, -1, 26}// 116	117	118	
			, {3874, 6, -3, 8}, {3870, 6, -3, 8}, {3821, 6, -2, 26}// 119	120	121	
			, {3862, 6, -3, 8}, {3286, 6, -3, 6}, {3821, 7, -3, 26}// 122	123	124	
			, {3821, 6, -3, 26}, {3821, 6, -2, 26}, {3864, 4, -2, 8}// 125	126	127	
			, {3821, 4, -1, 26}, {3821, 4, -1, 26}, {3821, 2, 4, 26}// 128	129	130	
			, {3316, -2, 1, 6}, {6019, -6, 5, 8}, {3821, 1, 5, 58}// 131	132	133	
			, {13649, 4, 6, 9}, {3388, 1, 0, 6}, {3821, 1, 4, 26}// 134	135	136	
			, {3821, 1, 5, 26}, {3821, 0, 4, 26}, {3821, 1, 4, 26}// 137	138	139	
			, {3286, 1, 4, 6}, {13649, -1, 6, 9}, {3874, 1, 4, 8}// 140	141	142	
			, {3862, 1, 4, 8}, {3821, 2, 5, 58}, {14068, 0, 5, 6}// 143	144	145	
			, {3287, 1, 4, 6}, {3821, 2, 4, 26}, {6021, -5, 3, 8}// 146	147	148	
			, {3821, 0, 5, 68}, {13650, 7, 6, 9}, {3387, 3, 1, 21}// 149	150	152	
			, {3821, 1, 5, 26}, {4949, 3, 2, 6}, {14133, 1, 2, 6}// 153	154	155	
			, {3286, -1, 6, 8}, {6021, -5, 2, 8}, {4950, 3, 1, 6}// 156	157	158	
			, {3821, -1, 6, 28}, {13649, 0, 6, 9}, {6021, -5, 4, 8}// 159	160	161	
			, {6021, -5, 0, 8}, {4943, 2, 1, 6}, {3821, 2, 6, 58}// 164	165	166	
			, {3864, -1, 6, 10}, {3821, 0, 5, 58}, {13649, 6, 6, 9}// 167	168	169	
			, {3287, -1, 6, 8}, {13378, 4, 2, 6}, {13649, 2, 6, 9}// 170	171	172	
			, {3862, -1, 6, 10}, {3821, 2, 5, 26}, {13649, 5, 6, 9}// 173	174	177	
			, {3870, -1, 6, 10}, {3874, -1, 6, 10}, {3821, 0, 6, 28}// 178	179	180	
			, {3821, 1, 5, 58}, {3821, 1, 6, 58}, {3821, 2, 5, 58}// 182	183	184	
			, {3821, -2, 6, 28}, {3821, -1, 6, 28}, {14138, -2, 6, 8}// 185	186	187	
			, {3821, 1, 5, 26}, {4944, 3, 1, 6}, {6021, -5, 5, 8}// 188	189	190	
			, {6022, -5, 6, 8}, {3821, 2, 4, 26}, {3315, -2, 0, 6}// 192	193	194	
			, {3864, 1, 4, 8}, {3821, 0, 6, 28}, {3870, 1, 4, 8}// 195	196	197	
			, {3821, -2, 6, 38}, {3821, -1, 6, 28}, {3821, 0, 6, 28}// 198	199	200	
			, {3821, 2, 5, 26}, {3821, 1, 4, 26}, {3317, 4, 3, 6}// 201	202	203	
			, {4945, 4, 0, 6}, {6021, -5, 1, 8}, {6010, 4, 1, 6}// 204	205	206	
			, {13649, 3, 6, 9}, {13649, 1, 6, 9}, {3318, 5, 3, 6}// 207	208	209	
			, {14138, 3, 1, 16}, {3319, 6, 3, 6}, {3287, 1, 5, 38}// 210	211	212	
			, {3286, 1, 5, 38}, {3874, 1, 5, 40}, {3864, 1, 5, 40}// 213	214	215	
			, {3862, 1, 5, 40}, {3821, 2, 6, 58}, {3821, 1, 6, 58}// 216	217	218	
			, {3821, 1, 5, 58}, {3821, 1, 6, 58}, {3821, 2, 5, 58}// 219	220	221	
			, {3870, 1, 5, 40}, {14138, 2, 5, 6}, {14138, -2, 3, 6}// 222	223	224	
			, {10980, 3, 3, 6}, {3821, 0, 4, 36}// 225	226	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new fireandlightningAddonDeed();
			}
		}

		[ Constructable ]
		public fireandlightningAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 14089, 6, -3, 6, 0, 1, "", 1);// 8
			AddComplexComponent( (BaseAddon) this, 14732, 4, -1, 4, 0, 1, "", 1);// 19
			AddComplexComponent( (BaseAddon) this, 14742, 4, -2, 16, 0, 1, "", 1);// 25
			AddComplexComponent( (BaseAddon) this, 14089, 4, -2, 6, 0, 1, "", 1);// 29
			AddComplexComponent( (BaseAddon) this, 14732, -3, -3, 6, 0, 1, "", 1);// 34
			AddComplexComponent( (BaseAddon) this, 3555, 4, -1, 6, 0, 1, "", 1);// 51
			AddComplexComponent( (BaseAddon) this, 3555, 4, -3, 3, 0, 1, "", 1);// 66
			AddComplexComponent( (BaseAddon) this, 14089, -1, -3, 6, 0, 1, "", 1);// 72
			AddComplexComponent( (BaseAddon) this, 14742, -1, -2, 12, 0, 1, "", 1);// 93
			AddComplexComponent( (BaseAddon) this, 3555, -1, -2, 6, 0, 1, "", 1);// 97
			AddComplexComponent( (BaseAddon) this, 14742, 2, 4, 3, 0, 1, "", 1);// 151
			AddComplexComponent( (BaseAddon) this, 3555, 1, 5, 6, 0, 1, "", 1);// 162
			AddComplexComponent( (BaseAddon) this, 3023, 3, 2, 11, 0, -1, "SilverMoon was bored ;)", 1);// 163
			AddComplexComponent( (BaseAddon) this, 14732, 1, 5, 26, 0, 1, "", 1);// 175
			AddComplexComponent( (BaseAddon) this, 7955, 4, 2, 6, 0, -1, "Spawner", 1);// 176
			AddComplexComponent( (BaseAddon) this, 14089, -1, 5, 6, 0, 1, "", 1);// 181
			AddComplexComponent( (BaseAddon) this, 14089, -1, 3, 6, 0, 1, "", 1);// 191

		}

		public fireandlightningAddon( Serial serial ) : base( serial )
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

	public class fireandlightningAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new fireandlightningAddon();
			}
		}

		[Constructable]
		public fireandlightningAddonDeed()
		{
			Name = "fireandlightning";
		}

		public fireandlightningAddonDeed( Serial serial ) : base( serial )
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