
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BrianDiceAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {3215, -7, -7, 0}, {1205, -7, -1, 0}, {1200, 0, 0, 0}// 1	3	5	
			, {1200, -2, 0, 0}, {3215, -7, -3, 0}, {1200, 1, 0, 0}// 6	7	8	
			, {1205, -4, -6, 0}, {3215, -7, -2, 0}, {1205, -4, -5, 0}// 9	10	11	
			, {1205, -4, -8, 0}, {3215, -7, -5, 0}, {1205, -4, 0, 0}// 12	13	14	
			, {1205, -4, -7, 0}, {1200, 0, -1, 0}, {1200, -1, 0, 0}// 15	16	17	
			, {1205, -7, -4, 0}, {1205, -7, 0, 0}, {1205, 1, -2, 0}// 18	19	20	
			, {1205, -2, -8, 0}, {1205, -2, -7, 0}, {1205, -2, -6, 0}// 21	22	23	
			, {1205, -2, -5, 0}, {1205, -2, 0, 0}, {1205, -1, -8, 0}// 24	29	30	
			, {1205, -1, -7, 0}, {1205, -1, -6, 0}, {1205, -1, -5, 0}// 31	32	33	
			, {1205, -1, -4, 0}, {1205, -1, -3, 0}, {1205, -1, -2, 0}// 34	35	36	
			, {1205, -1, -1, 0}, {1205, 0, -8, 0}, {1205, 0, -7, 0}// 37	38	39	
			, {1205, 0, -6, 0}, {1205, 0, -5, 0}, {1205, 0, -4, 0}// 40	41	42	
			, {1205, 0, -3, 0}, {1205, 0, -2, 0}, {1205, 0, -1, 0}// 43	44	45	
			, {3215, 2, -8, 0}, {3215, 3, -8, 0}, {3215, 4, -8, 0}// 46	47	48	
			, {3215, 5, -8, 0}, {3215, 6, -8, 0}, {3215, 0, -5, 0}// 49	50	51	
			, {3215, 0, -6, 0}, {3215, 0, -3, 0}, {3215, 0, -2, 0}// 52	53	54	
			, {3215, 0, -1, 0}, {3215, -2, 0, 0}, {3215, -2, -1, 0}// 55	56	57	
			, {3215, -1, -1, 0}, {3215, 0, -1, 0}, {3215, 1, -1, 0}// 58	59	60	
			, {3215, 1, 0, 0}, {1200, -1, -1, 0}, {3215, -7, -6, 0}// 61	63	64	
			, {1205, -7, -2, 0}, {1205, 1, -8, 0}, {1205, 4, -3, 0}// 65	66	67	
			, {1205, 4, -1, 0}, {1205, 4, 0, 0}, {1205, 5, -8, 0}// 69	70	71	
			, {1205, 5, -7, 0}, {1205, 5, -3, 0}, {1205, 5, -1, 0}// 72	76	78	
			, {1205, 6, -8, 0}, {1205, 6, -7, 0}, {1205, 6, -6, 0}// 80	81	82	
			, {1205, 6, -5, 0}, {1205, 6, -3, 0}, {1205, 6, -1, 0}// 83	85	87	
			, {1205, 6, 0, 0}, {1200, -2, -1, 0}, {3215, -7, -4, 0}// 88	89	90	
			, {1205, -7, -5, 0}, {1205, -7, -3, 0}, {1205, -5, -8, 0}// 91	92	94	
			, {1205, -5, -7, 0}, {1205, -5, -6, 0}, {1205, -5, -5, 0}// 95	96	97	
			, {1205, -5, -4, 0}, {1205, -5, -3, 0}, {1205, -5, -2, 0}// 98	99	100	
			, {1205, -5, 0, 0}, {1205, -6, 0, 0}, {1205, -6, -5, 0}// 102	103	104	
			, {1205, -6, -8, 0}, {1205, -6, -7, 0}, {1205, -6, -6, 0}// 109	110	111	
			, {1205, -3, -8, 0}, {1205, -3, -7, 0}, {1205, -3, -6, 0}// 112	113	114	
			, {1205, -3, -5, 0}, {1205, -3, -3, 0}, {1205, -3, -2, 0}// 115	117	118	
			, {1205, -3, 0, 0}, {3215, 0, -7, 0}, {3215, -7, -1, 0}// 120	121	122	
			, {3215, -7, 0, 0}, {3215, -6, -8, 0}, {3215, -5, -8, 0}// 123	124	125	
			, {3215, -4, -8, 0}, {3215, -3, -8, 0}, {3215, -2, -8, 0}// 126	127	128	
			, {3215, -1, -8, 0}, {3215, 0, -8, 0}, {3215, 1, -8, 0}// 129	130	131	
			, {1200, 1, -1, 0}, {1205, -7, -7, 0}, {1205, -7, -6, 0}// 132	133	134	
			, {1205, 1, -7, 0}, {1205, 1, -6, 0}, {1205, 1, -5, 0}// 135	136	137	
			, {1205, 1, -3, 0}, {1205, 1, 0, 0}, {1205, 1, -4, 0}// 138	139	140	
			, {1205, 1, -1, 0}, {3215, -7, -8, 0}, {1205, -7, -8, 0}// 141	142	143	
			, {1205, 1, -8, 0}, {1205, 1, -4, 0}, {1205, 1, -1, 0}// 144	145	146	
			, {1205, 1, 0, 0}, {1205, 2, -8, 0}, {1205, 2, -7, 0}// 147	148	149	
			, {1205, 2, -6, 0}, {1205, 2, -5, 0}, {1205, 2, -4, 0}// 150	151	152	
			, {1205, 2, -3, 0}, {1205, 2, -2, 0}, {1205, 2, -1, 0}// 153	154	155	
			, {1205, 2, 0, 0}, {1205, 3, -8, 0}, {1205, 3, -7, 0}// 156	157	158	
			, {1205, 3, -3, 0}, {1205, 3, -1, 0}, {1205, 3, 0, 0}// 162	164	165	
			, {1205, 4, -8, 0}, {1205, 4, -7, 0}, {1205, 4, -5, 0}// 166	167	169	
			, {1205, 4, -4, 0}, {1200, -1, 1, 0}, {1200, 1, 1, 0}// 170	171	172	
			, {1200, -1, 2, 0}, {1205, -5, 6, 0}, {1205, 1, 2, 0}// 173	174	175	
			, {1200, -1, 3, 0}, {1200, 0, 1, 0}, {1205, -5, 9, 0}// 176	178	179	
			, {1200, -2, 1, 0}, {1200, 0, 2, 0}, {1200, -2, 3, 0}// 181	182	183	
			, {1205, 1, 1, 0}, {1200, 0, 3, 0}, {1205, -4, 6, 0}// 184	185	186	
			, {1205, -3, 1, 0}, {1205, -3, 2, 0}, {1205, -3, 3, 0}// 187	188	189	
			, {1205, -3, 4, 0}, {1205, -3, 5, 0}, {1205, -3, 6, 0}// 190	191	192	
			, {1205, -3, 8, 0}, {1205, -3, 9, 0}, {1205, -2, 1, 0}// 194	195	196	
			, {1205, -2, 2, 0}, {1205, -2, 3, 0}, {1205, -2, 4, 0}// 197	198	199	
			, {1205, -2, 5, 0}, {1205, -2, 6, 0}, {1205, -2, 8, 0}// 200	201	203	
			, {1205, -2, 9, 0}, {1205, -1, 3, 0}, {1205, -1, 4, 0}// 204	205	206	
			, {1205, -1, 5, 0}, {1205, -1, 6, 0}, {1205, -1, 7, 0}// 207	208	209	
			, {1205, -1, 8, 0}, {1205, -1, 9, 0}, {1205, 0, 3, 0}// 210	211	212	
			, {1205, 0, 4, 0}, {1205, 0, 5, 0}, {1205, 0, 6, 0}// 213	214	215	
			, {1205, 0, 7, 0}, {3215, 0, 3, 0}, {3215, 0, 4, 0}// 216	217	218	
			, {3215, 0, 5, 0}, {3215, 0, 6, 0}, {3215, 0, 8, 0}// 219	220	221	
			, {3215, 0, 9, 0}, {3215, -2, 2, 0}, {3215, -2, 3, 0}// 222	223	224	
			, {3215, -1, 3, 0}, {3215, 0, 3, 0}, {3215, 1, 1, 0}// 225	226	227	
			, {3215, 1, 2, 0}, {3215, 1, 3, 0}, {1200, -2, 2, 0}// 228	229	230	
			, {1205, -6, 8, 0}, {1200, 1, 2, 0}, {1205, -4, 8, 0}// 232	234	235	
			, {1205, -6, 6, 0}, {1205, -7, 1, 0}, {1205, -4, 5, 0}// 236	237	238	
			, {1205, -5, 8, 0}, {1200, 1, 3, 0}, {1205, 4, 1, 0}// 239	240	241	
			, {1205, 4, 2, 0}, {1205, 4, 3, 0}, {1205, 4, 4, 0}// 242	243	244	
			, {1205, 4, 6, 0}, {1205, 4, 7, 0}, {1205, 4, 8, 0}// 246	247	248	
			, {1205, 4, 9, 0}, {1205, 5, 3, 0}, {1205, 5, 4, 0}// 249	252	253	
			, {1205, 5, 6, 0}, {1205, 5, 7, 0}, {1205, 5, 8, 0}// 255	256	257	
			, {1205, 5, 9, 0}, {1205, 6, 1, 0}, {1205, 6, 2, 0}// 258	259	260	
			, {1205, 6, 3, 0}, {1205, 6, 4, 0}, {1205, 6, 6, 0}// 261	262	264	
			, {1205, 6, 7, 0}, {1205, 6, 8, 0}, {1205, 6, 9, 0}// 265	266	267	
			, {1205, -4, 1, 0}, {1205, -6, 9, 0}, {1205, -5, 1, 0}// 268	269	270	
			, {1205, -5, 2, 0}, {1205, -5, 3, 0}, {1205, -5, 4, 0}// 271	272	273	
			, {1205, -6, 1, 0}, {1205, -6, 2, 0}, {1205, -6, 3, 0}// 274	275	276	
			, {1205, -6, 4, 0}, {1205, -7, 6, 0}, {1205, -7, 7, 0}// 277	278	279	
			, {1205, -7, 8, 0}, {1205, -7, 9, 0}, {1205, -7, 3, 0}// 280	281	282	
			, {1205, -7, 4, 0}, {1205, -7, 5, 0}, {1205, -7, 2, 0}// 283	284	285	
			, {1205, -4, 9, 0}, {3215, -2, 1, 0}, {3215, -7, 1, 0}// 286	287	288	
			, {3215, -7, 2, 0}, {3215, -7, 3, 0}, {3215, -7, 4, 0}// 289	290	291	
			, {3215, -7, 5, 0}, {3215, -7, 6, 0}, {3215, -7, 7, 0}// 292	293	294	
			, {3215, -7, 8, 0}, {3215, -7, 9, 0}, {1205, 1, 4, 0}// 295	296	297	
			, {1205, 1, 9, 0}, {1205, 1, 3, 0}, {1205, 1, 7, 0}// 299	300	301	
			, {1205, 1, 6, 0}, {1205, 1, 8, 0}, {1205, 1, 5, 0}// 302	303	304	
			, {1205, -5, 5, 0}, {1205, -6, 5, 0}, {1205, 0, 8, 0}// 306	307	308	
			, {1205, 0, 9, 0}, {1205, 1, 1, 0}, {1205, 1, 2, 0}// 309	310	311	
			, {1205, 1, 3, 0}, {1205, 1, 7, 0}, {1205, 2, 1, 0}// 312	313	314	
			, {1205, 2, 2, 0}, {1205, 2, 3, 0}, {1205, 2, 4, 0}// 315	316	317	
			, {1205, 2, 5, 0}, {1205, 2, 6, 0}, {1205, 2, 7, 0}// 318	319	320	
			, {1205, 2, 8, 0}, {1205, 2, 9, 0}, {1205, 3, 1, 0}// 321	322	323	
			, {1205, 3, 2, 0}, {1205, 3, 3, 0}, {1205, 3, 4, 0}// 324	325	326	
			, {1205, 3, 9, 0}, {3215, 7, -8, 0}, {1205, 7, -8, 0}// 331	332	333	
			, {1205, 7, -7, 0}, {1205, 7, -3, 0}, {1205, 7, -1, 0}// 334	338	340	
			, {1205, 7, 0, 0}, {1205, 7, 1, 0}, {1205, 7, 2, 0}// 341	342	343	
			, {1205, 7, 3, 0}, {1205, 7, 4, 0}, {1205, 7, 6, 0}// 344	345	347	
			, {1205, 7, 7, 0}, {1205, 7, 8, 0}, {1205, 7, 9, 0}// 348	349	350	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new BrianDiceAddonDeed();
			}
		}

		[ Constructable ]
		public BrianDiceAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 1205, -4, -1, 0, 1, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 1205, -4, -2, 0, 1, -1, "", 1);// 4
			AddComplexComponent( (BaseAddon) this, 1205, -2, -4, 0, 1, -1, "", 1);// 25
			AddComplexComponent( (BaseAddon) this, 1205, -2, -3, 0, 1, -1, "", 1);// 26
			AddComplexComponent( (BaseAddon) this, 1205, -2, -2, 0, 1, -1, "", 1);// 27
			AddComplexComponent( (BaseAddon) this, 1205, -2, -1, 0, 1, -1, "", 1);// 28
			AddComplexComponent( (BaseAddon) this, 1205, -4, -4, 0, 1, -1, "", 1);// 62
			AddComplexComponent( (BaseAddon) this, 1205, 4, -2, 0, 1, -1, "", 1);// 68
			AddComplexComponent( (BaseAddon) this, 1205, 5, -6, 0, 1, -1, "", 1);// 73
			AddComplexComponent( (BaseAddon) this, 1205, 5, -5, 0, 1, -1, "", 1);// 74
			AddComplexComponent( (BaseAddon) this, 1205, 5, -4, 0, 1, -1, "", 1);// 75
			AddComplexComponent( (BaseAddon) this, 1205, 5, -2, 0, 1, -1, "", 1);// 77
			AddComplexComponent( (BaseAddon) this, 1205, 5, 0, 0, 1, -1, "", 1);// 79
			AddComplexComponent( (BaseAddon) this, 1205, 6, -4, 0, 1, -1, "", 1);// 84
			AddComplexComponent( (BaseAddon) this, 1205, 6, -2, 0, 1, -1, "", 1);// 86
			AddComplexComponent( (BaseAddon) this, 1205, -4, -3, 0, 1, -1, "", 1);// 93
			AddComplexComponent( (BaseAddon) this, 1205, -5, -1, 0, 1, -1, "", 1);// 101
			AddComplexComponent( (BaseAddon) this, 1205, -6, -4, 0, 1, -1, "", 1);// 105
			AddComplexComponent( (BaseAddon) this, 1205, -6, -3, 0, 1, -1, "", 1);// 106
			AddComplexComponent( (BaseAddon) this, 1205, -6, -2, 0, 1, -1, "", 1);// 107
			AddComplexComponent( (BaseAddon) this, 1205, -6, -1, 0, 1, -1, "", 1);// 108
			AddComplexComponent( (BaseAddon) this, 1205, -3, -4, 0, 1, -1, "", 1);// 116
			AddComplexComponent( (BaseAddon) this, 1205, -3, -1, 0, 1, -1, "", 1);// 119
			AddComplexComponent( (BaseAddon) this, 1205, 3, -6, 0, 1, -1, "", 1);// 159
			AddComplexComponent( (BaseAddon) this, 1205, 3, -5, 0, 1, -1, "", 1);// 160
			AddComplexComponent( (BaseAddon) this, 1205, 3, -4, 0, 1, -1, "", 1);// 161
			AddComplexComponent( (BaseAddon) this, 1205, 3, -2, 0, 1, -1, "", 1);// 163
			AddComplexComponent( (BaseAddon) this, 1205, 4, -6, 0, 1, -1, "", 1);// 168
			AddComplexComponent( (BaseAddon) this, 1205, -4, 4, 0, 1, -1, "", 1);// 177
			AddComplexComponent( (BaseAddon) this, 1205, -4, 3, 0, 1, -1, "", 1);// 180
			AddComplexComponent( (BaseAddon) this, 1205, -3, 7, 0, 1, -1, "", 1);// 193
			AddComplexComponent( (BaseAddon) this, 1205, -2, 7, 0, 1, -1, "", 1);// 202
			AddComplexComponent( (BaseAddon) this, 1205, -4, 7, 0, 1, -1, "", 1);// 231
			AddComplexComponent( (BaseAddon) this, 1205, -4, 2, 0, 1, -1, "", 1);// 233
			AddComplexComponent( (BaseAddon) this, 1205, 4, 5, 0, 1, -1, "", 1);// 245
			AddComplexComponent( (BaseAddon) this, 1205, 5, 1, 0, 1, -1, "", 1);// 250
			AddComplexComponent( (BaseAddon) this, 1205, 5, 2, 0, 1, -1, "", 1);// 251
			AddComplexComponent( (BaseAddon) this, 1205, 5, 5, 0, 1, -1, "", 1);// 254
			AddComplexComponent( (BaseAddon) this, 1205, 6, 5, 0, 1, -1, "", 1);// 263
			AddComplexComponent( (BaseAddon) this, 1205, -6, 7, 0, 1, -1, "", 1);// 298
			AddComplexComponent( (BaseAddon) this, 1205, -5, 7, 0, 1, -1, "", 1);// 305
			AddComplexComponent( (BaseAddon) this, 1205, 3, 5, 0, 1, -1, "", 1);// 327
			AddComplexComponent( (BaseAddon) this, 1205, 3, 6, 0, 1, -1, "", 1);// 328
			AddComplexComponent( (BaseAddon) this, 1205, 3, 7, 0, 1, -1, "", 1);// 329
			AddComplexComponent( (BaseAddon) this, 1205, 3, 8, 0, 1, -1, "", 1);// 330
			AddComplexComponent( (BaseAddon) this, 1205, 7, -6, 0, 1, -1, "", 1);// 335
			AddComplexComponent( (BaseAddon) this, 1205, 7, -5, 0, 1, -1, "", 1);// 336
			AddComplexComponent( (BaseAddon) this, 1205, 7, -4, 0, 1, -1, "", 1);// 337
			AddComplexComponent( (BaseAddon) this, 1205, 7, -2, 0, 1, -1, "", 1);// 339
			AddComplexComponent( (BaseAddon) this, 1205, 7, 5, 0, 1, -1, "", 1);// 346

		}

		public BrianDiceAddon( Serial serial ) : base( serial )
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

	public class BrianDiceAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new BrianDiceAddon();
			}
		}

		[Constructable]
		public BrianDiceAddonDeed()
		{
			Name = "BrianDice";
		}

		public BrianDiceAddonDeed( Serial serial ) : base( serial )
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