
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class silvershouseAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {4168, -4, -2, 28}, {3088, 0, -4, 5}, {4112, 0, 0, 6}// 1	3	5	
			, {4174, -7, -4, 5}, {3840, -4, 1, 34}, {4099, -1, 0, 6}// 6	7	10	
			, {4174, -7, 0, 5}, {7700, -7, -1, 11}, {3840, -5, -2, 13}// 13	14	15	
			, {4235, -7, -2, 28}, {3894, 2, 0, 7}, {3894, 0, -1, 27}// 16	18	19	
			, {4022, 1, 1, 5}, {2865, -7, 1, 28}, {2760, -5, -4, 5}// 21	22	23	
			, {2760, -5, -3, 5}, {5066, 2, 0, 8}, {4173, -5, -2, 25}// 25	26	27	
			, {2451, -7, -4, 14}, {3892, 2, 1, 5}, {4173, -7, 0, 11}// 28	30	31	
			, {1701, -6, -3, 25}, {3656, -7, 0, 17}, {9446, -7, 0, 10}// 32	33	34	
			, {5995, -5, -2, 10}, {1709, -2, -2, 25}, {1701, 0, -1, 5}// 36	39	40	
			, {4080, -5, 1, 33}, {4080, -5, 1, 34}, {4080, -5, 1, 32}// 41	42	43	
			, {4174, -7, 0, 12}, {4080, -5, 1, 31}, {7608, -7, 0, 16}// 44	46	47	
			, {2760, -6, -2, 5}, {3750, -4, -2, 27}, {3751, 0, -2, 26}// 48	49	50	
			, {2451, -1, 1, 6}, {2860, -7, -3, 5}, {8093, -5, -2, 30}// 51	52	54	
			, {3210, 8, 1, 0}, {7617, 2, -1, 25}, {3656, -7, -4, 20}// 55	56	57	
			, {1703, 1, -1, 5}, {158, -3, -2, 0}, {4967, 2, 1, 6}// 61	62	63	
			, {4555, 2, -1, 34}, {3647, -5, -2, 26}, {4174, -7, -4, 37}// 64	65	66	
			, {3859, -5, -2, 32}, {4104, -7, -2, 10}, {7608, -7, 0, 9}// 67	68	69	
			, {4174, -7, -4, 25}, {4174, -7, 1, 28}, {4555, -7, 0, 38}// 70	71	74	
			, {5021, -7, 1, 39}, {3752, -7, -4, 25}, {5899, -1, 0, 8}// 77	79	80	
			, {4174, -7, -3, 11}, {4104, 2, -2, 32}, {2765, -7, -2, 5}// 82	83	85	
			, {4174, -7, -4, 17}, {2767, -4, -1, 5}, {4174, -7, -3, 17}// 88	89	90	
			, {3877, -3, -4, 20}, {2765, -7, -4, 5}, {4174, -7, -4, 31}// 92	93	97	
			, {3262, 6, -3, 0}, {7695, -7, -3, 11}, {7618, 2, -2, 25}// 98	99	103	
			, {4174, -7, 1, 25}, {3149, 5, -4, 0}, {3896, 2, 1, 7}// 104	106	107	
			, {2869, -7, -1, 28}, {2860, -7, -1, 5}, {2860, -7, -4, 25}// 108	112	113	
			, {2860, -7, -2, 5}, {2760, -6, -1, 5}, {3093, -7, 1, 5}// 114	115	117	
			, {4174, -7, -1, 26}, {2765, -7, -3, 5}, {4173, -7, 0, 6}// 118	120	121	
			, {3877, -5, -2, 29}, {4174, -7, -1, 27}, {5643, -7, 0, 25}// 122	123	124	
			, {5643, -7, 1, 25}, {2864, -7, -1, 28}, {3834, -7, 0, 31}// 125	126	127	
			, {4173, -7, 1, 25}, {2860, -7, 0, 32}, {4174, -7, -4, 11}// 128	129	131	
			, {4174, -7, -3, 5}, {4173, -4, -2, 25}, {3873, -5, -2, 30}// 133	135	136	
			, {2860, -7, 1, 32}, {2767, -4, -4, 5}, {5643, -7, -1, 25}// 141	142	144	
			, {3553, -7, 0, 29}, {2860, -7, 0, 28}, {3553, -6, 0, 37}// 145	148	149	
			, {2765, -7, -1, 5}, {2761, -4, 0, 5}, {2767, -4, -2, 5}// 150	151	152	
			, {2768, -6, 0, 5}, {2767, -4, -3, 5}, {2760, -5, -2, 5}// 153	154	155	
			, {2760, -5, -1, 5}, {2760, -6, -3, 5}, {2768, -5, 0, 5}// 156	157	158	
			, {2760, -6, -4, 5}, {2763, -7, 0, 5}, {7758, 0, -4, 5}// 159	160	162	
			, {7756, -2, -4, 5}, {7757, -1, -4, 5}, {7755, -2, -3, 5}// 163	164	165	
			, {7754, -1, -3, 5}, {7753, 0, -3, 5}, {3180, -5, 1, 35}// 166	167	175	
			, {3892, 2, 2, 5}, {3859, -7, 2, 37}, {3707, 1, 3, 5}// 178	179	180	
			, {7695, -1, 3, 5}, {4980, -1, 2, 6}, {7697, -3, 2, 5}// 181	182	183	
			, {2462, -5, 2, 11}, {3877, -7, 2, 33}, {2512, -7, 2, 33}// 184	185	187	
			, {3348, -3, 4, 0}, {4044, -1, 4, 7}, {3892, 1, 2, 5}// 188	189	191	
			, {3519, 0, 3, 5}, {7717, -6, 2, 11}, {4041, -1, 4, 5}// 192	195	196	
			, {4553, -1, 4, 4}, {4042, -1, 4, 5}, {4469, 2, 2, 5}// 198	200	201	
			, {3859, -1, 4, 15}, {3877, -1, 4, 12}, {3873, -1, 4, 14}// 202	203	206	
			, {3149, 6, 4, 0}, {4553, -7, 2, 25}, {3873, -7, 2, 35}// 207	210	218	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new silvershouseAddonDeed();
			}
		}

		[ Constructable ]
		public silvershouseAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 4613, -5, 1, 25, 2002, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 3649, -7, 1, 29, 1719, -1, "treasure chest", 1);// 4
			AddComplexComponent( (BaseAddon) this, 4632, -4, 0, 25, 2002, -1, "", 1);// 8
			AddComplexComponent( (BaseAddon) this, 2848, -1, -1, 26, 0, 29, "", 1);// 9
			AddComplexComponent( (BaseAddon) this, 3530, 0, -1, 25, 2208, -1, "", 1);// 11
			AddComplexComponent( (BaseAddon) this, 4612, -4, 1, 25, 2002, -1, "", 1);// 12
			AddComplexComponent( (BaseAddon) this, 7069, -1, 0, 5, 1341, -1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 2854, -3, 1, 5, 0, 1, "", 1);// 20
			AddComplexComponent( (BaseAddon) this, 2842, -5, -2, 13, 0, 2, "", 1);// 24
			AddComplexComponent( (BaseAddon) this, 4632, -5, -3, 5, 1348, -1, "", 1);// 29
			AddComplexComponent( (BaseAddon) this, 7069, 0, 0, 5, 1341, -1, "", 1);// 35
			AddComplexComponent( (BaseAddon) this, 5990, 2, -2, 31, 38, -1, "", 1);// 37
			AddComplexComponent( (BaseAddon) this, 3530, -1, -2, 25, 2208, -1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 4039, 2, 1, 8, 1126, -1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 2475, -7, -4, 7, 1045, -1, "treasure chest", 1);// 53
			AddComplexComponent( (BaseAddon) this, 3530, 1, -1, 25, 2208, -1, "", 1);// 58
			AddComplexComponent( (BaseAddon) this, 3530, 2, -1, 25, 2208, -1, "", 1);// 59
			AddComplexComponent( (BaseAddon) this, 3530, 2, -2, 25, 2208, -1, "", 1);// 60
			AddComplexComponent( (BaseAddon) this, 3530, 0, -2, 25, 2208, -1, "", 1);// 72
			AddComplexComponent( (BaseAddon) this, 7127, -7, -4, 11, 0, -1, "", 2);// 73
			AddComplexComponent( (BaseAddon) this, 3960, 2, -1, 37, 0, -1, "", 2);// 75
			AddComplexComponent( (BaseAddon) this, 3530, -1, -1, 25, 2208, -1, "", 1);// 76
			AddComplexComponent( (BaseAddon) this, 2594, -7, -1, 35, 0, 29, "", 1);// 78
			AddComplexComponent( (BaseAddon) this, 9035, 2, -2, 36, 0, -1, "Happy Mothers Day 2012", 1);// 81
			AddComplexComponent( (BaseAddon) this, 4613, -6, -2, 5, 1348, -1, "", 1);// 84
			AddComplexComponent( (BaseAddon) this, 2842, -4, 1, 33, 0, 2, "", 1);// 86
			AddComplexComponent( (BaseAddon) this, 4633, -1, 1, 5, 1348, -1, "", 1);// 87
			AddComplexComponent( (BaseAddon) this, 5018, -7, -2, 36, 1348, -1, "", 1);// 91
			AddComplexComponent( (BaseAddon) this, 6218, -3, -2, 40, 0, 2, "", 1);// 94
			AddComplexComponent( (BaseAddon) this, 4632, -6, -3, 5, 1348, -1, "", 1);// 95
			AddComplexComponent( (BaseAddon) this, 7127, -7, -4, 17, 0, -1, "", 2);// 96
			AddComplexComponent( (BaseAddon) this, 4632, 1, -2, 25, 1109, -1, "", 1);// 100
			AddComplexComponent( (BaseAddon) this, 5990, 2, -1, 31, 38, -1, "", 1);// 101
			AddComplexComponent( (BaseAddon) this, 3530, 1, -2, 25, 2208, -1, "", 1);// 102
			AddComplexComponent( (BaseAddon) this, 3976, -5, -2, 31, 0, -1, "", 2);// 105
			AddComplexComponent( (BaseAddon) this, 3821, -7, 0, 29, 0, -1, "", 6);// 109
			AddComplexComponent( (BaseAddon) this, 2885, -7, -2, 25, 1348, -1, "", 1);// 110
			AddComplexComponent( (BaseAddon) this, 2886, -7, -2, 31, 1348, -1, "", 1);// 111
			AddComplexComponent( (BaseAddon) this, 6218, -7, -1, 35, 0, 2, "", 1);// 116
			AddComplexComponent( (BaseAddon) this, 7888, -7, 0, 30, 0, 2, "", 1);// 119
			AddComplexComponent( (BaseAddon) this, 7127, -7, -4, 6, 0, -1, "", 2);// 130
			AddComplexComponent( (BaseAddon) this, 7885, -4, 1, 33, 0, 2, "", 1);// 132
			AddComplexComponent( (BaseAddon) this, 4634, -6, -1, 5, 1348, -1, "", 1);// 134
			AddComplexComponent( (BaseAddon) this, 4612, -5, -2, 5, 1348, -1, "", 1);// 137
			AddComplexComponent( (BaseAddon) this, 4634, -5, -1, 5, 1348, -1, "", 1);// 138
			AddComplexComponent( (BaseAddon) this, 6218, -1, 0, 20, 0, 2, "", 1);// 139
			AddComplexComponent( (BaseAddon) this, 2594, -3, -2, 40, 0, 29, "", 1);// 140
			AddComplexComponent( (BaseAddon) this, 3553, -7, 0, 30, 0, -1, "", 2);// 143
			AddComplexComponent( (BaseAddon) this, 6585, -7, 0, 26, 1109, -1, "", 1);// 146
			AddComplexComponent( (BaseAddon) this, 7885, 0, -1, 25, 0, 2, "", 1);// 147
			AddComplexComponent( (BaseAddon) this, 2594, -1, 0, 20, 47, 29, "a twilight lantern", 1);// 161
			AddComplexComponent( (BaseAddon) this, 2512, -4, 1, 33, 0, -1, "", 2);// 168
			AddComplexComponent( (BaseAddon) this, 4632, -5, 0, 25, 2002, -1, "", 1);// 169
			AddComplexComponent( (BaseAddon) this, 8457, -1, 0, 13, 47, -1, "", 1);// 170
			AddComplexComponent( (BaseAddon) this, 8457, 1, -1, 37, 997, -1, "", 1);// 171
			AddComplexComponent( (BaseAddon) this, 5168, -7, -2, 10, 0, 2, "", 1);// 172
			AddComplexComponent( (BaseAddon) this, 2512, -1, 1, 7, 0, -1, "", 2);// 173
			AddComplexComponent( (BaseAddon) this, 6570, 1, -4, 25, 0, 29, "", 1);// 174
			AddComplexComponent( (BaseAddon) this, 7136, -7, 0, 25, 1140, -1, "a yule log", 1);// 176
			AddComplexComponent( (BaseAddon) this, 6571, -7, 0, 29, 0, 0, "", 1);// 177
			AddComplexComponent( (BaseAddon) this, 4633, -1, 2, 5, 1348, -1, "", 1);// 186
			AddComplexComponent( (BaseAddon) this, 9035, -3, 2, 13, 0, -1, "Happy Mothers Day 2012", 1);// 190
			AddComplexComponent( (BaseAddon) this, 7069, -1, 4, 15, 1645, -1, "", 1);// 193
			AddComplexComponent( (BaseAddon) this, 7069, 0, 4, 3, 1645, -1, "", 1);// 194
			AddComplexComponent( (BaseAddon) this, 4634, -4, 2, 5, 1109, -1, "", 1);// 197
			AddComplexComponent( (BaseAddon) this, 4613, -6, 2, 5, 1109, -1, "", 1);// 199
			AddComplexComponent( (BaseAddon) this, 4612, -5, 2, 5, 1109, -1, "", 1);// 204
			AddComplexComponent( (BaseAddon) this, 3976, 2, 2, 11, 0, -1, "", 2);// 205
			AddComplexComponent( (BaseAddon) this, 6218, -7, 2, 20, 0, 2, "", 1);// 208
			AddComplexComponent( (BaseAddon) this, 2594, -7, 2, 20, 0, 29, "", 1);// 209
			AddComplexComponent( (BaseAddon) this, 6218, -1, 3, 20, 0, 2, "", 1);// 211
			AddComplexComponent( (BaseAddon) this, 7068, 0, 4, 2, 545, -1, "Antler of The Arcadia Stag", 1);// 212
			AddComplexComponent( (BaseAddon) this, 5168, 2, 2, 6, 0, 2, "", 1);// 213
			AddComplexComponent( (BaseAddon) this, 4634, -4, 2, 25, 2002, -1, "", 1);// 214
			AddComplexComponent( (BaseAddon) this, 3702, -1, 2, 8, 243, -1, "Bag of Halloween Candy", 1);// 215
			AddComplexComponent( (BaseAddon) this, 4634, -5, 2, 25, 2002, -1, "", 1);// 216
			AddComplexComponent( (BaseAddon) this, 2594, -1, 3, 20, 47, 29, "a twilight lantern", 1);// 217

		}

		public silvershouseAddon( Serial serial ) : base( serial )
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

	public class silvershouseAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new silvershouseAddon();
			}
		}

		[Constructable]
		public silvershouseAddonDeed()
		{
			Name = "silvershouse";
		}

		public silvershouseAddonDeed( Serial serial ) : base( serial )
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