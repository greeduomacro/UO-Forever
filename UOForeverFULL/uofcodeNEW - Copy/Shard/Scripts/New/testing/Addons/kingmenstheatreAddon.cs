
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class kingmenstheatreAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {3710, -11, 7, 0}, {3710, -11, 8, 0}, {3644, -11, 9, 0}// 1	2	3	
			, {4766, -9, -2, 15}, {3709, -11, 10, 0}, {2903, -11, 5, 0}// 4	5	6	
			, {2903, -11, 6, 0}, {2903, -10, 5, 0}, {2903, -10, 6, 0}// 7	8	9	
			, {3710, -11, 8, 3}, {3710, -11, 7, 3}, {3710, -11, 8, 6}// 10	11	12	
			, {2910, -4, -10, 0}, {2729, -5, -9, 0}, {2729, -5, -6, 0}// 13	15	17	
			, {2729, -2, -5, 0}, {2765, 2, -6, 10}, {2729, -5, -8, 0}// 18	19	20	
			, {2760, 6, -8, 10}, {2760, 4, -6, 10}, {2729, -4, -8, 0}// 21	22	24	
			, {2729, -3, -7, 0}, {3751, -3, -11, 6}, {3748, -8, -9, 10}// 25	26	27	
			, {5429, -4, -11, 6}, {4554, 6, -9, 10}, {2765, 2, -7, 10}// 28	29	30	
			, {2729, -4, -7, 0}, {2729, -4, -11, 0}, {2729, -2, -9, 0}// 31	32	33	
			, {2766, 4, -9, 10}, {310, 4, -10, 10}, {2760, 4, -8, 10}// 34	35	36	
			, {2729, -4, -4, 0}, {2729, -6, -5, 0}, {10115, -8, -10, 0}// 37	38	39	
			, {2729, -6, -7, 0}, {311, 1, -8, 10}, {2729, -3, -8, 0}// 40	41	42	
			, {2867, 4, -7, 10}, {311, 1, -9, 10}, {7643, -7, -10, 0}// 43	44	46	
			, {3589, -4, -11, 11}, {3314, 0, -5, 10}, {7641, -7, -7, 0}// 47	48	49	
			, {2729, -2, -8, 0}, {2729, -2, -11, 0}, {3785, -8, -5, 10}// 50	51	52	
			, {3130, 2, -8, 10}, {3130, -8, -6, 0}, {2729, -6, -11, 0}// 53	54	55	
			, {2729, -8, -10, 0}, {2766, 6, -9, 10}, {2729, -2, -4, 0}// 56	57	58	
			, {2729, -6, -6, 0}, {2729, -5, -7, 0}, {2729, -3, -6, 0}// 59	60	61	
			, {2729, -7, -6, 0}, {2729, -8, -8, 0}, {3745, -8, -7, 19}// 62	63	64	
			, {7637, -4, -10, 0}, {3589, -8, -7, 5}, {3743, -6, -11, 5}// 66	68	70	
			, {2760, 6, -7, 10}, {2767, 7, -7, 10}, {2767, 7, -6, 10}// 71	72	73	
			, {2729, -6, -4, 0}, {2760, 3, -8, 10}, {2729, -6, -9, 0}// 74	75	76	
			, {3815, -7, -11, 5}, {310, 6, -10, 10}, {2866, 5, -8, 10}// 77	78	79	
			, {2762, 2, -9, 10}, {2729, -3, -9, 0}, {2766, 5, -9, 10}// 80	81	82	
			, {169, 1, -10, 10}, {7643, -6, -10, 0}, {1701, 5, -10, 10}// 83	84	86	
			, {2760, 5, -7, 10}, {3997, -8, -9, 6}, {2729, -3, -4, 0}// 87	88	90	
			, {317, 1, -6, 10}, {2518, 5, -7, 20}, {2766, 3, -9, 10}// 91	92	93	
			, {2768, 6, -5, 10}, {4553, 2, -5, 10}, {2764, 7, -9, 10}// 94	95	96	
			, {2729, -5, -4, 0}, {2761, 7, -5, 10}, {5463, -1, -7, 7}// 97	98	99	
			, {2729, -4, -9, 0}, {320, 1, -7, 10}, {2760, 5, -6, 10}// 100	101	102	
			, {5463, -1, -6, 7}, {3750, -4, -11, 5}, {2760, 3, -6, 10}// 103	104	105	
			, {310, 2, -10, 10}, {3154, 2, -9, 23}, {5073, 2, -8, 10}// 106	108	109	
			, {5072, 2, -8, 10}, {2729, -8, -11, 0}, {2729, -8, -7, 0}// 110	111	112	
			, {2765, 2, -8, 10}, {2910, -6, -10, 0}, {2620, -4, -11, 0}// 113	114	116	
			, {2620, -6, -11, 0}, {2621, -5, -11, 0}, {2621, -7, -11, 0}// 117	118	119	
			, {2628, -8, -7, 0}, {2629, -8, -8, 0}, {2729, -7, -8, 0}// 120	121	122	
			, {2729, -3, -11, 0}, {2729, -3, -10, 0}, {2910, -7, -9, 0}// 123	124	125	
			, {2910, -7, -7, 0}, {2729, -7, -11, 0}, {3782, -8, -11, 0}// 126	127	128	
			, {2729, -8, -6, 0}, {3130, -8, -9, 0}, {2729, -4, -9, 0}// 129	131	132	
			, {2729, -7, -5, 0}, {2760, 4, -7, 10}, {2767, 7, -8, 10}// 133	134	135	
			, {2760, 5, -8, 10}, {2768, 3, -5, 10}, {2760, 3, -7, 10}// 136	137	138	
			, {2729, -8, -5, 0}, {2729, -4, -5, 0}, {7644, -7, -9, 0}// 139	140	141	
			, {2729, -4, -6, 0}, {2768, 4, -5, 10}, {2729, -5, -10, 0}// 142	143	144	
			, {2729, -8, -9, 0}, {2729, -8, -4, 0}, {2768, 5, -5, 10}// 145	146	147	
			, {2729, -2, -10, 0}, {2729, -7, -4, 0}, {2729, -3, -5, 0}// 148	149	150	
			, {2729, -6, -8, 0}, {2763, 2, -5, 10}, {2729, -5, -5, 0}// 151	152	153	
			, {2760, 6, -6, 10}, {2729, -5, -11, 0}, {2887, 2, -9, 10}// 154	155	156	
			, {10265, 5, -7, 10}, {10329, 4, -9, 10}, {5995, 4, -9, 20}// 157	158	159	
			, {3205, 2, -9, 17}, {7775, 0, -9, 10}, {3155, 2, -9, 19}// 160	161	162	
			, {1701, -5, -4, 0}, {1703, -4, -4, 0}, {1076, 1, -3, 7}// 163	164	178	
			, {5666, 0, 6, 5}, {5664, 0, 7, 5}, {1826, 0, -2, 0}// 188	220	222	
			, {4553, 0, 2, 0}, {4553, 0, 5, 0}, {2748, 0, 5, 0}// 228	234	235	
			, {3086, -7, -3, 0}, {1826, 0, -3, 5}, {1825, 0, -3, 0}// 250	261	264	
			, {2748, 2, 4, 0}, {4553, 0, 9, 0}, {1703, 3, 12, 0}// 285	287	310	
			, {1701, 2, 12, 0}, {310, 8, -10, 10}, {7020, 9, -8, 10}// 311	312	313	
			, {311, 8, -8, 10}, {2591, 8, -11, 10}, {311, 8, -9, 10}// 314	316	319	
			, {3310, 9, -11, 10}, {1824, 10, -3, 0}, {1826, 10, -2, 0}// 320	337	364	
			, {1826, 10, -3, 5}, {1703, 9, 12, 0}, {1701, 8, 12, 0}// 375	385	386	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new kingmenstheatreAddonDeed();
			}
		}

		[ Constructable ]
		public kingmenstheatreAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 2562, 6, -9, 23, 0, 1, "", 1);// 14
			AddComplexComponent( (BaseAddon) this, 5057, -8, -6, 0, 1348, -1, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 11599, 5, -7, 14, 0, -1, "a fruit bowl", 1);// 23
			AddComplexComponent( (BaseAddon) this, 314, 3, -10, 10, 0, 1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 2567, -8, -7, 15, 0, 1, "", 1);// 65
			AddComplexComponent( (BaseAddon) this, 5058, -8, -9, 0, 1348, -1, "", 1);// 67
			AddComplexComponent( (BaseAddon) this, 2854, -3, -6, 0, 0, 1, "", 1);// 69
			AddComplexComponent( (BaseAddon) this, 3995, -8, -11, 0, 2003, -1, "", 1);// 85
			AddComplexComponent( (BaseAddon) this, 3992, -8, -11, 0, 38, -1, "", 1);// 89
			AddComplexComponent( (BaseAddon) this, 314, 7, -10, 10, 0, 1, "", 1);// 107
			AddComplexComponent( (BaseAddon) this, 2562, 4, -9, 23, 0, 1, "", 1);// 115
			AddComplexComponent( (BaseAddon) this, 2574, -5, -11, 16, 0, 1, "", 1);// 130
			AddComplexComponent( (BaseAddon) this, 2748, 7, 7, 0, 785, -1, "", 1);// 165
			AddComplexComponent( (BaseAddon) this, 2748, 1, 1, 0, 785, -1, "", 1);// 166
			AddComplexComponent( (BaseAddon) this, 2748, 7, 0, 0, 785, -1, "", 1);// 167
			AddComplexComponent( (BaseAddon) this, 2748, 6, 11, 0, 785, -1, "", 1);// 168
			AddComplexComponent( (BaseAddon) this, 2748, 2, 0, 0, 785, -1, "", 1);// 169
			AddComplexComponent( (BaseAddon) this, 5661, 0, 4, 5, 38, -1, "", 1);// 170
			AddComplexComponent( (BaseAddon) this, 2748, 4, 2, 0, 785, -1, "", 1);// 171
			AddComplexComponent( (BaseAddon) this, 2748, 7, 6, 0, 785, -1, "", 1);// 172
			AddComplexComponent( (BaseAddon) this, 2748, 5, 6, 0, 785, -1, "", 1);// 173
			AddComplexComponent( (BaseAddon) this, 2748, 4, 11, 0, 785, -1, "", 1);// 174
			AddComplexComponent( (BaseAddon) this, 2748, 3, 8, 0, 785, -1, "", 1);// 175
			AddComplexComponent( (BaseAddon) this, 2748, 2, -3, 0, 785, -1, "", 1);// 176
			AddComplexComponent( (BaseAddon) this, 2748, 1, 9, 0, 785, -1, "", 1);// 177
			AddComplexComponent( (BaseAddon) this, 2748, 7, 4, 0, 785, -1, "", 1);// 179
			AddComplexComponent( (BaseAddon) this, 2748, 7, -3, 0, 785, -1, "", 1);// 180
			AddComplexComponent( (BaseAddon) this, 2748, 6, 6, 0, 785, -1, "", 1);// 181
			AddComplexComponent( (BaseAddon) this, 2748, 1, 4, 0, 785, -1, "", 1);// 182
			AddComplexComponent( (BaseAddon) this, 2748, 0, 6, 0, 785, -1, "", 1);// 183
			AddComplexComponent( (BaseAddon) this, 2748, 1, 3, 0, 785, -1, "", 1);// 184
			AddComplexComponent( (BaseAddon) this, 2748, 3, 2, 0, 785, -1, "", 1);// 185
			AddComplexComponent( (BaseAddon) this, 2748, 7, 2, 0, 785, -1, "", 1);// 186
			AddComplexComponent( (BaseAddon) this, 5527, 0, 1, 5, 38, -1, "", 1);// 187
			AddComplexComponent( (BaseAddon) this, 2748, 0, 1, 0, 785, -1, "", 1);// 189
			AddComplexComponent( (BaseAddon) this, 2748, 5, 9, 0, 785, -1, "", 1);// 190
			AddComplexComponent( (BaseAddon) this, 2748, 6, 4, 0, 785, -1, "", 1);// 191
			AddComplexComponent( (BaseAddon) this, 2748, 0, 4, 0, 785, -1, "", 1);// 192
			AddComplexComponent( (BaseAddon) this, 2748, 0, 2, 0, 785, -1, "", 1);// 193
			AddComplexComponent( (BaseAddon) this, 2748, 3, -1, 0, 785, -1, "", 1);// 194
			AddComplexComponent( (BaseAddon) this, 2748, 4, -3, 0, 785, -1, "", 1);// 195
			AddComplexComponent( (BaseAddon) this, 2748, 3, -3, 0, 785, -1, "", 1);// 196
			AddComplexComponent( (BaseAddon) this, 2748, 3, -2, 0, 785, -1, "", 1);// 197
			AddComplexComponent( (BaseAddon) this, 2748, 7, -2, 0, 785, -1, "", 1);// 198
			AddComplexComponent( (BaseAddon) this, 2748, 6, -2, 0, 785, -1, "", 1);// 199
			AddComplexComponent( (BaseAddon) this, 2748, 4, -2, 0, 785, -1, "", 1);// 200
			AddComplexComponent( (BaseAddon) this, 2748, 1, -1, 0, 785, -1, "", 1);// 201
			AddComplexComponent( (BaseAddon) this, 2748, 2, -2, 0, 785, -1, "", 1);// 202
			AddComplexComponent( (BaseAddon) this, 2748, 7, -1, 0, 785, -1, "", 1);// 203
			AddComplexComponent( (BaseAddon) this, 2748, 5, -1, 0, 785, -1, "", 1);// 204
			AddComplexComponent( (BaseAddon) this, 2748, 6, -1, 0, 785, -1, "", 1);// 205
			AddComplexComponent( (BaseAddon) this, 2748, 0, -1, 0, 785, -1, "", 1);// 206
			AddComplexComponent( (BaseAddon) this, 2748, 4, 12, 0, 785, -1, "", 1);// 207
			AddComplexComponent( (BaseAddon) this, 2748, 2, -1, 0, 785, -1, "", 1);// 208
			AddComplexComponent( (BaseAddon) this, 2748, 7, 8, 0, 785, -1, "", 1);// 209
			AddComplexComponent( (BaseAddon) this, 2748, 4, 4, 0, 785, -1, "", 1);// 210
			AddComplexComponent( (BaseAddon) this, 2748, 3, 9, 0, 785, -1, "", 1);// 211
			AddComplexComponent( (BaseAddon) this, 2854, 0, 0, 0, 0, 1, "", 1);// 212
			AddComplexComponent( (BaseAddon) this, 2748, 6, 0, 0, 785, -1, "", 1);// 213
			AddComplexComponent( (BaseAddon) this, 2748, 3, 0, 0, 785, -1, "", 1);// 214
			AddComplexComponent( (BaseAddon) this, 2748, 0, 9, 0, 785, -1, "", 1);// 215
			AddComplexComponent( (BaseAddon) this, 2748, 5, -2, 0, 785, -1, "", 1);// 216
			AddComplexComponent( (BaseAddon) this, 2748, 5, 8, 0, 785, -1, "", 1);// 217
			AddComplexComponent( (BaseAddon) this, 2748, 7, 12, 0, 785, -1, "", 1);// 218
			AddComplexComponent( (BaseAddon) this, 2748, 6, 5, 0, 785, -1, "", 1);// 219
			AddComplexComponent( (BaseAddon) this, 2748, 2, 1, 0, 785, -1, "", 1);// 221
			AddComplexComponent( (BaseAddon) this, 2748, 3, 11, 0, 785, -1, "", 1);// 223
			AddComplexComponent( (BaseAddon) this, 2748, 7, 5, 0, 785, -1, "", 1);// 224
			AddComplexComponent( (BaseAddon) this, 2748, 4, 7, 0, 785, -1, "", 1);// 225
			AddComplexComponent( (BaseAddon) this, 2748, 2, 4, 1, 785, -1, "", 1);// 226
			AddComplexComponent( (BaseAddon) this, 2748, 1, 0, 0, 785, -1, "", 1);// 227
			AddComplexComponent( (BaseAddon) this, 2748, 5, 3, 0, 785, -1, "", 1);// 229
			AddComplexComponent( (BaseAddon) this, 4842, 1, -3, 10, 1156, -1, "", 1);// 230
			AddComplexComponent( (BaseAddon) this, 2748, 1, 5, 0, 785, -1, "", 1);// 231
			AddComplexComponent( (BaseAddon) this, 2748, 2, 7, 0, 785, -1, "", 1);// 232
			AddComplexComponent( (BaseAddon) this, 2748, 4, -1, 0, 785, -1, "", 1);// 233
			AddComplexComponent( (BaseAddon) this, 2748, 4, 5, 0, 785, -1, "", 1);// 236
			AddComplexComponent( (BaseAddon) this, 2748, 3, 5, 0, 785, -1, "", 1);// 237
			AddComplexComponent( (BaseAddon) this, 2748, 3, 6, 0, 785, -1, "", 1);// 238
			AddComplexComponent( (BaseAddon) this, 2748, 4, 6, 0, 785, -1, "", 1);// 239
			AddComplexComponent( (BaseAddon) this, 2748, 2, 6, 0, 785, -1, "", 1);// 240
			AddComplexComponent( (BaseAddon) this, 2748, 3, 7, 0, 785, -1, "", 1);// 241
			AddComplexComponent( (BaseAddon) this, 2748, 1, 6, 0, 785, -1, "", 1);// 242
			AddComplexComponent( (BaseAddon) this, 2748, 6, 8, 0, 785, -1, "", 1);// 243
			AddComplexComponent( (BaseAddon) this, 2748, 1, 8, 0, 785, -1, "", 1);// 244
			AddComplexComponent( (BaseAddon) this, 2748, 4, 8, 0, 785, -1, "", 1);// 245
			AddComplexComponent( (BaseAddon) this, 2748, 2, 9, 0, 785, -1, "", 1);// 246
			AddComplexComponent( (BaseAddon) this, 2748, 0, 11, 0, 785, -1, "", 1);// 247
			AddComplexComponent( (BaseAddon) this, 2748, 2, 8, 0, 785, -1, "", 1);// 248
			AddComplexComponent( (BaseAddon) this, 2748, 2, 5, 0, 785, -1, "", 1);// 249
			AddComplexComponent( (BaseAddon) this, 2854, 0, 8, 0, 0, 1, "", 1);// 251
			AddComplexComponent( (BaseAddon) this, 2748, 6, 9, 0, 785, -1, "", 1);// 252
			AddComplexComponent( (BaseAddon) this, 2748, 5, 1, 0, 785, -1, "", 1);// 253
			AddComplexComponent( (BaseAddon) this, 2848, -8, 4, 0, 0, 29, "", 1);// 254
			AddComplexComponent( (BaseAddon) this, 2748, 6, -3, 0, 785, -1, "", 1);// 255
			AddComplexComponent( (BaseAddon) this, 2748, 6, 1, 0, 785, -1, "", 1);// 256
			AddComplexComponent( (BaseAddon) this, 2748, 4, 1, 0, 785, -1, "", 1);// 257
			AddComplexComponent( (BaseAddon) this, 2748, 5, 2, 0, 785, -1, "", 1);// 258
			AddComplexComponent( (BaseAddon) this, 2748, 5, 12, 0, 785, -1, "", 1);// 259
			AddComplexComponent( (BaseAddon) this, 2748, 5, 11, 0, 785, -1, "", 1);// 260
			AddComplexComponent( (BaseAddon) this, 5669, 0, 10, 5, 38, -1, "", 1);// 262
			AddComplexComponent( (BaseAddon) this, 2748, 4, 3, 0, 785, -1, "", 1);// 263
			AddComplexComponent( (BaseAddon) this, 2748, 2, 3, 0, 785, -1, "", 1);// 265
			AddComplexComponent( (BaseAddon) this, 5677, 0, 0, 5, 38, -1, "", 1);// 266
			AddComplexComponent( (BaseAddon) this, 2748, 5, 4, 0, 785, -1, "", 1);// 267
			AddComplexComponent( (BaseAddon) this, 2748, 7, 11, 0, 785, -1, "", 1);// 268
			AddComplexComponent( (BaseAddon) this, 2748, 6, 12, 0, 785, -1, "", 1);// 269
			AddComplexComponent( (BaseAddon) this, 2748, 6, 3, 0, 785, -1, "", 1);// 270
			AddComplexComponent( (BaseAddon) this, 2748, 3, 3, 0, 785, -1, "", 1);// 271
			AddComplexComponent( (BaseAddon) this, 2748, 0, 0, 0, 785, -1, "", 1);// 272
			AddComplexComponent( (BaseAddon) this, 2748, 2, 11, 0, 785, -1, "", 1);// 273
			AddComplexComponent( (BaseAddon) this, 2748, 7, 3, 0, 785, -1, "", 1);// 274
			AddComplexComponent( (BaseAddon) this, 2748, 2, 2, 0, 785, -1, "", 1);// 275
			AddComplexComponent( (BaseAddon) this, 2748, 6, 10, 0, 785, -1, "", 1);// 276
			AddComplexComponent( (BaseAddon) this, 2748, 7, 10, 0, 785, -1, "", 1);// 277
			AddComplexComponent( (BaseAddon) this, 2748, 2, 10, 0, 785, -1, "", 1);// 278
			AddComplexComponent( (BaseAddon) this, 2748, 4, 9, 0, 785, -1, "", 1);// 279
			AddComplexComponent( (BaseAddon) this, 2748, 5, 10, 0, 785, -1, "", 1);// 280
			AddComplexComponent( (BaseAddon) this, 2748, 1, 11, 0, 785, -1, "", 1);// 281
			AddComplexComponent( (BaseAddon) this, 2748, 4, 10, 0, 785, -1, "", 1);// 282
			AddComplexComponent( (BaseAddon) this, 2748, 5, -3, 0, 785, -1, "", 1);// 283
			AddComplexComponent( (BaseAddon) this, 2748, 1, 12, 0, 785, -1, "", 1);// 284
			AddComplexComponent( (BaseAddon) this, 2748, 3, 10, 0, 785, -1, "", 1);// 286
			AddComplexComponent( (BaseAddon) this, 2748, 3, 1, 0, 785, -1, "", 1);// 288
			AddComplexComponent( (BaseAddon) this, 2748, 0, 3, 0, 785, -1, "", 1);// 289
			AddComplexComponent( (BaseAddon) this, 2748, 6, 7, 0, 785, -1, "", 1);// 290
			AddComplexComponent( (BaseAddon) this, 2748, 0, 7, 0, 785, -1, "", 1);// 291
			AddComplexComponent( (BaseAddon) this, 2748, 0, 10, 0, 785, -1, "", 1);// 292
			AddComplexComponent( (BaseAddon) this, 2748, 4, 0, 0, 785, -1, "", 1);// 293
			AddComplexComponent( (BaseAddon) this, 2748, 7, 1, 0, 785, -1, "", 1);// 294
			AddComplexComponent( (BaseAddon) this, 2748, 6, 2, 0, 785, -1, "", 1);// 295
			AddComplexComponent( (BaseAddon) this, 2748, 3, 12, 0, 785, -1, "", 1);// 296
			AddComplexComponent( (BaseAddon) this, 2748, 3, 4, 0, 785, -1, "", 1);// 297
			AddComplexComponent( (BaseAddon) this, 2748, 5, 7, 0, 785, -1, "", 1);// 298
			AddComplexComponent( (BaseAddon) this, 5663, 0, 3, 5, 38, -1, "", 1);// 299
			AddComplexComponent( (BaseAddon) this, 5667, 0, 11, 5, 38, -1, "", 1);// 300
			AddComplexComponent( (BaseAddon) this, 2748, 1, 7, 0, 785, -1, "", 1);// 301
			AddComplexComponent( (BaseAddon) this, 2748, 5, 5, 0, 785, -1, "", 1);// 302
			AddComplexComponent( (BaseAddon) this, 2748, 1, 2, 0, 785, -1, "", 1);// 303
			AddComplexComponent( (BaseAddon) this, 2748, 2, 12, 0, 785, -1, "", 1);// 304
			AddComplexComponent( (BaseAddon) this, 2748, 1, 10, 0, 785, -1, "", 1);// 305
			AddComplexComponent( (BaseAddon) this, 2748, 5, 0, 0, 785, -1, "", 1);// 306
			AddComplexComponent( (BaseAddon) this, 2748, 7, 9, 0, 785, -1, "", 1);// 307
			AddComplexComponent( (BaseAddon) this, 2748, 0, 8, 0, 785, -1, "", 1);// 308
			AddComplexComponent( (BaseAddon) this, 2748, 0, 12, 0, 785, -1, "", 1);// 309
			AddComplexComponent( (BaseAddon) this, 2583, 9, -10, 29, 46, 1, "", 1);// 315
			AddComplexComponent( (BaseAddon) this, 2583, 8, -10, 28, 785, 1, "", 1);// 317
			AddComplexComponent( (BaseAddon) this, 315, 8, -7, 10, 0, 1, "", 1);// 318
			AddComplexComponent( (BaseAddon) this, 2854, 9, 10, 0, 0, 1, "", 1);// 321
			AddComplexComponent( (BaseAddon) this, 2748, 8, 7, 0, 785, -1, "", 1);// 322
			AddComplexComponent( (BaseAddon) this, 2748, 11, 11, 0, 785, -1, "", 1);// 323
			AddComplexComponent( (BaseAddon) this, 2748, 10, 2, 0, 785, -1, "", 1);// 324
			AddComplexComponent( (BaseAddon) this, 2748, 10, 10, 0, 785, -1, "", 1);// 325
			AddComplexComponent( (BaseAddon) this, 2748, 9, 2, 0, 785, -1, "", 1);// 326
			AddComplexComponent( (BaseAddon) this, 2748, 10, 11, 0, 785, -1, "", 1);// 327
			AddComplexComponent( (BaseAddon) this, 2748, 11, 7, 0, 785, -1, "", 1);// 328
			AddComplexComponent( (BaseAddon) this, 2748, 11, 9, 0, 785, -1, "", 1);// 329
			AddComplexComponent( (BaseAddon) this, 2748, 11, 4, 0, 785, -1, "", 1);// 330
			AddComplexComponent( (BaseAddon) this, 2748, 9, 8, 0, 785, -1, "", 1);// 331
			AddComplexComponent( (BaseAddon) this, 2748, 8, 2, 0, 785, -1, "", 1);// 332
			AddComplexComponent( (BaseAddon) this, 2748, 9, 4, 0, 785, -1, "", 1);// 333
			AddComplexComponent( (BaseAddon) this, 2748, 8, 3, 0, 785, -1, "", 1);// 334
			AddComplexComponent( (BaseAddon) this, 2748, 8, 4, 0, 785, -1, "", 1);// 335
			AddComplexComponent( (BaseAddon) this, 2748, 9, 3, 0, 785, -1, "", 1);// 336
			AddComplexComponent( (BaseAddon) this, 2748, 10, -1, 0, 785, -1, "", 1);// 338
			AddComplexComponent( (BaseAddon) this, 2748, 8, -2, 0, 785, -1, "", 1);// 339
			AddComplexComponent( (BaseAddon) this, 2748, 8, -3, 0, 785, -1, "", 1);// 340
			AddComplexComponent( (BaseAddon) this, 2748, 9, 5, 0, 785, -1, "", 1);// 341
			AddComplexComponent( (BaseAddon) this, 2748, 8, 6, 0, 785, -1, "", 1);// 342
			AddComplexComponent( (BaseAddon) this, 2748, 11, 1, 0, 785, -1, "", 1);// 343
			AddComplexComponent( (BaseAddon) this, 2748, 8, 0, 0, 785, -1, "", 1);// 344
			AddComplexComponent( (BaseAddon) this, 2748, 10, 0, 0, 785, -1, "", 1);// 345
			AddComplexComponent( (BaseAddon) this, 2748, 11, 5, 0, 785, -1, "", 1);// 346
			AddComplexComponent( (BaseAddon) this, 2748, 11, -2, 0, 785, -1, "", 1);// 347
			AddComplexComponent( (BaseAddon) this, 2748, 11, 10, 0, 785, -1, "", 1);// 348
			AddComplexComponent( (BaseAddon) this, 2748, 11, 12, 0, 785, -1, "", 1);// 349
			AddComplexComponent( (BaseAddon) this, 2748, 11, 8, 0, 785, -1, "", 1);// 350
			AddComplexComponent( (BaseAddon) this, 2748, 10, 7, 0, 785, -1, "", 1);// 351
			AddComplexComponent( (BaseAddon) this, 4842, 11, -3, 10, 1156, -1, "", 1);// 352
			AddComplexComponent( (BaseAddon) this, 2748, 8, 12, 0, 785, -1, "", 1);// 353
			AddComplexComponent( (BaseAddon) this, 2748, 11, 6, 0, 785, -1, "", 1);// 354
			AddComplexComponent( (BaseAddon) this, 2748, 11, 3, 0, 785, -1, "", 1);// 355
			AddComplexComponent( (BaseAddon) this, 2748, 9, 7, 0, 785, -1, "", 1);// 356
			AddComplexComponent( (BaseAddon) this, 2748, 10, 9, 0, 785, -1, "", 1);// 357
			AddComplexComponent( (BaseAddon) this, 2748, 8, 8, 0, 785, -1, "", 1);// 358
			AddComplexComponent( (BaseAddon) this, 2748, 8, 5, 0, 785, -1, "", 1);// 359
			AddComplexComponent( (BaseAddon) this, 2748, 11, 0, 0, 785, -1, "", 1);// 360
			AddComplexComponent( (BaseAddon) this, 2748, 8, 11, 0, 785, -1, "", 1);// 361
			AddComplexComponent( (BaseAddon) this, 2748, 11, 2, 0, 785, -1, "", 1);// 362
			AddComplexComponent( (BaseAddon) this, 2748, 10, 1, 0, 785, -1, "", 1);// 363
			AddComplexComponent( (BaseAddon) this, 2748, 10, 3, 0, 785, -1, "", 1);// 365
			AddComplexComponent( (BaseAddon) this, 2748, 10, 4, 0, 785, -1, "", 1);// 366
			AddComplexComponent( (BaseAddon) this, 2748, 8, 10, 0, 785, -1, "", 1);// 367
			AddComplexComponent( (BaseAddon) this, 2748, 9, 10, 0, 785, -1, "", 1);// 368
			AddComplexComponent( (BaseAddon) this, 2748, 9, 0, 0, 785, -1, "", 1);// 369
			AddComplexComponent( (BaseAddon) this, 2748, 9, 11, 0, 785, -1, "", 1);// 370
			AddComplexComponent( (BaseAddon) this, 2748, 9, -1, 0, 785, -1, "", 1);// 371
			AddComplexComponent( (BaseAddon) this, 2748, 8, 9, 0, 785, -1, "", 1);// 372
			AddComplexComponent( (BaseAddon) this, 2748, 10, 12, 0, 785, -1, "", 1);// 373
			AddComplexComponent( (BaseAddon) this, 2748, 8, 1, 0, 785, -1, "", 1);// 374
			AddComplexComponent( (BaseAddon) this, 2748, 9, 12, 0, 785, -1, "", 1);// 376
			AddComplexComponent( (BaseAddon) this, 2748, 9, 6, 0, 785, -1, "", 1);// 377
			AddComplexComponent( (BaseAddon) this, 2748, 10, 5, 0, 785, -1, "", 1);// 378
			AddComplexComponent( (BaseAddon) this, 2748, 8, -1, 0, 785, -1, "", 1);// 379
			AddComplexComponent( (BaseAddon) this, 2748, 9, 1, 0, 785, -1, "", 1);// 380
			AddComplexComponent( (BaseAddon) this, 2748, 11, -1, 0, 785, -1, "", 1);// 381
			AddComplexComponent( (BaseAddon) this, 2748, 10, 6, 0, 785, -1, "", 1);// 382
			AddComplexComponent( (BaseAddon) this, 2748, 9, 9, 0, 785, -1, "", 1);// 383
			AddComplexComponent( (BaseAddon) this, 2748, 10, 8, 0, 785, -1, "", 1);// 384

		}

		public kingmenstheatreAddon( Serial serial ) : base( serial )
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

	public class kingmenstheatreAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new kingmenstheatreAddon();
			}
		}

		[Constructable]
		public kingmenstheatreAddonDeed()
		{
			Name = "kingmenstheatre";
		}

		public kingmenstheatreAddonDeed( Serial serial ) : base( serial )
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