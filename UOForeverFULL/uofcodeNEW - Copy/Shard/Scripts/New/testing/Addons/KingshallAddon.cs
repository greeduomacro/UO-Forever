
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
	public class KingsHallAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {4766, -10, -2, 15}, {2910, -5, -10, 0}, {2729, -6, -9, 0}// 1	2	4	
			, {2729, -6, -6, 0}, {2729, -3, -5, 0}, {2765, 1, -6, 10}// 6	7	8	
			, {2729, -6, -8, 0}, {2760, 5, -8, 10}, {2760, 3, -6, 10}// 9	10	11	
			, {10114, -7, -11, 1}, {2729, -5, -8, 0}, {2729, -4, -7, 0}// 12	14	15	
			, {3751, -4, -11, 6}, {3748, -9, -9, 10}, {5429, -5, -11, 6}// 16	17	18	
			, {4554, 5, -9, 10}, {2765, 1, -7, 10}, {2729, -5, -7, 0}// 19	20	21	
			, {2729, -5, -11, 0}, {2729, -3, -9, 0}, {2766, 3, -9, 10}// 22	23	24	
			, {310, 3, -10, 10}, {2760, 3, -8, 10}, {2729, -5, -4, 0}// 25	26	27	
			, {2729, -7, -5, 0}, {10115, -9, -10, 0}, {2729, -7, -7, 0}// 28	29	30	
			, {311, 0, -8, 10}, {2729, -4, -8, 0}, {2867, 3, -7, 10}// 31	32	33	
			, {311, 0, -9, 10}, {7643, -8, -10, 0}, {3589, -5, -11, 11}// 34	36	37	
			, {3314, -1, -5, 10}, {7641, -8, -7, 0}, {2729, -3, -8, 0}// 38	39	40	
			, {2729, -3, -11, 0}, {3785, -9, -5, 10}, {3130, 1, -8, 10}// 41	42	43	
			, {10137, -4, -11, 0}, {3130, -9, -6, 0}, {2729, -7, -11, 0}// 44	45	46	
			, {2729, -9, -10, 0}, {2766, 5, -9, 10}, {2729, -3, -4, 0}// 47	48	49	
			, {2729, -7, -6, 0}, {2729, -6, -7, 0}, {2729, -4, -6, 0}// 50	51	52	
			, {2729, -8, -6, 0}, {2729, -9, -8, 0}, {3745, -9, -7, 19}// 53	54	55	
			, {7637, -5, -10, 0}, {3589, -9, -7, 5}, {3743, -7, -11, 5}// 57	59	61	
			, {2760, 5, -7, 10}, {2767, 6, -7, 10}, {2767, 6, -6, 10}// 62	63	64	
			, {2729, -7, -4, 0}, {2760, 2, -8, 10}, {2729, -7, -9, 0}// 65	66	67	
			, {3815, -8, -11, 5}, {310, 5, -10, 10}, {2866, 4, -8, 10}// 68	69	70	
			, {2762, 1, -9, 10}, {2729, -4, -9, 0}, {2766, 4, -9, 10}// 71	72	73	
			, {169, 0, -10, 10}, {7643, -7, -10, 0}, {2760, 4, -7, 10}// 74	75	77	
			, {3997, -9, -9, 6}, {2729, -4, -4, 0}, {317, 0, -6, 10}// 78	80	81	
			, {2518, 4, -7, 20}, {2766, 2, -9, 10}, {2768, 5, -5, 10}// 82	83	84	
			, {4553, 1, -5, 10}, {2764, 6, -9, 10}, {2729, -6, -4, 0}// 85	86	87	
			, {2761, 6, -5, 10}, {5463, -2, -7, 7}, {2729, -5, -9, 0}// 88	89	90	
			, {320, 0, -7, 10}, {2760, 4, -6, 10}, {5463, -2, -6, 7}// 91	92	93	
			, {3750, -5, -11, 5}, {2760, 2, -6, 10}, {310, 1, -10, 10}// 94	95	96	
			, {3154, 1, -9, 23}, {5073, 1, -8, 10}, {5072, 1, -8, 10}// 98	99	100	
			, {2729, -9, -11, 0}, {2729, -9, -7, 0}, {2765, 1, -8, 10}// 101	102	103	
			, {2910, -7, -10, 0}, {2620, -5, -11, 0}, {2620, -7, -11, 0}// 104	106	107	
			, {2621, -6, -11, 0}, {2621, -8, -11, 0}, {2628, -9, -7, 0}// 108	109	110	
			, {2629, -9, -8, 0}, {2729, -8, -8, 0}, {2729, -4, -11, 0}// 111	112	113	
			, {2729, -4, -10, 0}, {2910, -8, -9, 0}, {2910, -8, -7, 0}// 114	115	116	
			, {2729, -8, -11, 0}, {3782, -9, -11, 0}, {2729, -9, -6, 0}// 117	118	119	
			, {3130, -9, -9, 0}, {2729, -5, -9, 0}, {2729, -8, -5, 0}// 121	122	123	
			, {2760, 3, -7, 10}, {2767, 6, -8, 10}, {2760, 4, -8, 10}// 124	125	126	
			, {2768, 2, -5, 10}, {2760, 2, -7, 10}, {2729, -9, -5, 0}// 127	128	129	
			, {2729, -5, -5, 0}, {7644, -8, -9, 0}, {2729, -5, -6, 0}// 130	131	132	
			, {2768, 3, -5, 10}, {2729, -6, -10, 0}, {2729, -9, -9, 0}// 133	134	135	
			, {2729, -9, -4, 0}, {2768, 4, -5, 10}, {2729, -3, -10, 0}// 136	137	138	
			, {2729, -8, -4, 0}, {2729, -4, -5, 0}, {2729, -7, -8, 0}// 139	140	141	
			, {2763, 1, -5, 10}, {2729, -6, -5, 0}, {2760, 5, -6, 10}// 142	143	144	
			, {2729, -6, -11, 0}, {2887, 1, -9, 10}, {10265, 4, -7, 10}// 145	146	147	
			, {10329, 3, -9, 10}, {5995, 3, -9, 20}, {3205, 1, -9, 17}// 148	149	150	
			, {7775, -1, -9, 10}, {3155, 1, -9, 19}, {1076, 0, -3, 7}// 151	152	166	
			, {5666, -1, 6, 5}, {5664, -1, 7, 5}, {1826, -1, -2, 0}// 176	208	210	
			, {4553, -1, 2, 0}, {4553, -1, 5, 0}, {2748, -1, 5, 0}// 216	222	223	
			, {1826, -1, -3, 5}, {1825, -1, -3, 0}, {2748, 1, 4, 0}// 247	250	271	
			, {4553, -1, 9, 0}, {310, 7, -10, 10}, {7020, 8, -8, 10}// 273	296	297	
			, {311, 7, -8, 10}, {18323, 8, -6, 10}, {2591, 7, -11, 10}// 298	300	301	
			, {311, 7, -9, 10}, {3310, 8, -11, 10}, {1824, 9, -3, 0}// 304	305	322	
			, {1826, 9, -2, 0}, {1826, 9, -3, 5}// 349	360	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new KingsHallAddonDeed();
			}
		}

		[ Constructable ]
		public KingsHallAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 2562, 5, -9, 23, 0, 1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 5057, -9, -6, 0, 1348, -1, "", 1);// 5
			AddComplexComponent( (BaseAddon) this, 11599, 4, -7, 14, 0, -1, "a fruit bowl", 1);// 13
			AddComplexComponent( (BaseAddon) this, 314, 2, -10, 10, 0, 1, "", 1);// 35
			AddComplexComponent( (BaseAddon) this, 2567, -9, -7, 15, 0, 1, "", 1);// 56
			AddComplexComponent( (BaseAddon) this, 5058, -9, -9, 0, 1348, -1, "", 1);// 58
			AddComplexComponent( (BaseAddon) this, 2854, -4, -6, 0, 0, 1, "", 1);// 60
			AddComplexComponent( (BaseAddon) this, 3995, -9, -11, 0, 2003, -1, "", 1);// 76
			AddComplexComponent( (BaseAddon) this, 3992, -9, -11, 0, 38, -1, "", 1);// 79
			AddComplexComponent( (BaseAddon) this, 314, 6, -10, 10, 0, 1, "", 1);// 97
			AddComplexComponent( (BaseAddon) this, 2562, 3, -9, 23, 0, 1, "", 1);// 105
			AddComplexComponent( (BaseAddon) this, 2574, -6, -11, 16, 0, 1, "", 1);// 120
			AddComplexComponent( (BaseAddon) this, 2748, 6, 7, 0, 785, -1, "", 1);// 153
			AddComplexComponent( (BaseAddon) this, 2748, 0, 1, 0, 785, -1, "", 1);// 154
			AddComplexComponent( (BaseAddon) this, 2748, 6, 0, 0, 785, -1, "", 1);// 155
			AddComplexComponent( (BaseAddon) this, 2748, 5, 11, 0, 785, -1, "", 1);// 156
			AddComplexComponent( (BaseAddon) this, 2748, 1, 0, 0, 785, -1, "", 1);// 157
			AddComplexComponent( (BaseAddon) this, 5661, -1, 4, 5, 38, -1, "", 1);// 158
			AddComplexComponent( (BaseAddon) this, 2748, 3, 2, 0, 785, -1, "", 1);// 159
			AddComplexComponent( (BaseAddon) this, 2748, 6, 6, 0, 785, -1, "", 1);// 160
			AddComplexComponent( (BaseAddon) this, 2748, 4, 6, 0, 785, -1, "", 1);// 161
			AddComplexComponent( (BaseAddon) this, 2748, 3, 11, 0, 785, -1, "", 1);// 162
			AddComplexComponent( (BaseAddon) this, 2748, 2, 8, 0, 785, -1, "", 1);// 163
			AddComplexComponent( (BaseAddon) this, 2748, 1, -3, 0, 785, -1, "", 1);// 164
			AddComplexComponent( (BaseAddon) this, 2748, 0, 9, 0, 785, -1, "", 1);// 165
			AddComplexComponent( (BaseAddon) this, 2748, 6, 4, 0, 785, -1, "", 1);// 167
			AddComplexComponent( (BaseAddon) this, 2748, 6, -3, 0, 785, -1, "", 1);// 168
			AddComplexComponent( (BaseAddon) this, 2748, 5, 6, 0, 785, -1, "", 1);// 169
			AddComplexComponent( (BaseAddon) this, 2748, 0, 4, 0, 785, -1, "", 1);// 170
			AddComplexComponent( (BaseAddon) this, 2748, -1, 6, 0, 785, -1, "", 1);// 171
			AddComplexComponent( (BaseAddon) this, 2748, 0, 3, 0, 785, -1, "", 1);// 172
			AddComplexComponent( (BaseAddon) this, 2748, 2, 2, 0, 785, -1, "", 1);// 173
			AddComplexComponent( (BaseAddon) this, 2748, 6, 2, 0, 785, -1, "", 1);// 174
			AddComplexComponent( (BaseAddon) this, 5527, -1, 1, 5, 38, -1, "", 1);// 175
			AddComplexComponent( (BaseAddon) this, 2748, -1, 1, 0, 785, -1, "", 1);// 177
			AddComplexComponent( (BaseAddon) this, 2748, 4, 9, 0, 785, -1, "", 1);// 178
			AddComplexComponent( (BaseAddon) this, 2748, 5, 4, 0, 785, -1, "", 1);// 179
			AddComplexComponent( (BaseAddon) this, 2748, -1, 4, 0, 785, -1, "", 1);// 180
			AddComplexComponent( (BaseAddon) this, 2748, -1, 2, 0, 785, -1, "", 1);// 181
			AddComplexComponent( (BaseAddon) this, 2748, 2, -1, 0, 785, -1, "", 1);// 182
			AddComplexComponent( (BaseAddon) this, 2748, 3, -3, 0, 785, -1, "", 1);// 183
			AddComplexComponent( (BaseAddon) this, 2748, 2, -3, 0, 785, -1, "", 1);// 184
			AddComplexComponent( (BaseAddon) this, 2748, 2, -2, 0, 785, -1, "", 1);// 185
			AddComplexComponent( (BaseAddon) this, 2748, 6, -2, 0, 785, -1, "", 1);// 186
			AddComplexComponent( (BaseAddon) this, 2748, 5, -2, 0, 785, -1, "", 1);// 187
			AddComplexComponent( (BaseAddon) this, 2748, 3, -2, 0, 785, -1, "", 1);// 188
			AddComplexComponent( (BaseAddon) this, 2748, 0, -1, 0, 785, -1, "", 1);// 189
			AddComplexComponent( (BaseAddon) this, 2748, 1, -2, 0, 785, -1, "", 1);// 190
			AddComplexComponent( (BaseAddon) this, 2748, 6, -1, 0, 785, -1, "", 1);// 191
			AddComplexComponent( (BaseAddon) this, 2748, 4, -1, 0, 785, -1, "", 1);// 192
			AddComplexComponent( (BaseAddon) this, 2748, 5, -1, 0, 785, -1, "", 1);// 193
			AddComplexComponent( (BaseAddon) this, 2748, -1, -1, 0, 785, -1, "", 1);// 194
			AddComplexComponent( (BaseAddon) this, 2748, 3, 12, 0, 785, -1, "", 1);// 195
			AddComplexComponent( (BaseAddon) this, 2748, 1, -1, 0, 785, -1, "", 1);// 196
			AddComplexComponent( (BaseAddon) this, 2748, 6, 8, 0, 785, -1, "", 1);// 197
			AddComplexComponent( (BaseAddon) this, 2748, 3, 4, 0, 785, -1, "", 1);// 198
			AddComplexComponent( (BaseAddon) this, 2748, 2, 9, 0, 785, -1, "", 1);// 199
			AddComplexComponent( (BaseAddon) this, 2854, -1, 0, 0, 0, 1, "", 1);// 200
			AddComplexComponent( (BaseAddon) this, 2748, 5, 0, 0, 785, -1, "", 1);// 201
			AddComplexComponent( (BaseAddon) this, 2748, 2, 0, 0, 785, -1, "", 1);// 202
			AddComplexComponent( (BaseAddon) this, 2748, -1, 9, 0, 785, -1, "", 1);// 203
			AddComplexComponent( (BaseAddon) this, 2748, 4, -2, 0, 785, -1, "", 1);// 204
			AddComplexComponent( (BaseAddon) this, 2748, 4, 8, 0, 785, -1, "", 1);// 205
			AddComplexComponent( (BaseAddon) this, 2748, 6, 12, 0, 785, -1, "", 1);// 206
			AddComplexComponent( (BaseAddon) this, 2748, 5, 5, 0, 785, -1, "", 1);// 207
			AddComplexComponent( (BaseAddon) this, 2748, 1, 1, 0, 785, -1, "", 1);// 209
			AddComplexComponent( (BaseAddon) this, 2748, 2, 11, 0, 785, -1, "", 1);// 211
			AddComplexComponent( (BaseAddon) this, 2748, 6, 5, 0, 785, -1, "", 1);// 212
			AddComplexComponent( (BaseAddon) this, 2748, 3, 7, 0, 785, -1, "", 1);// 213
			AddComplexComponent( (BaseAddon) this, 2748, 1, 4, 1, 785, -1, "", 1);// 214
			AddComplexComponent( (BaseAddon) this, 2748, 0, 0, 0, 785, -1, "", 1);// 215
			AddComplexComponent( (BaseAddon) this, 2748, 4, 3, 0, 785, -1, "", 1);// 217
			AddComplexComponent( (BaseAddon) this, 4842, 0, -3, 10, 1156, -1, "", 1);// 218
			AddComplexComponent( (BaseAddon) this, 2748, 0, 5, 0, 785, -1, "", 1);// 219
			AddComplexComponent( (BaseAddon) this, 2748, 1, 7, 0, 785, -1, "", 1);// 220
			AddComplexComponent( (BaseAddon) this, 2748, 3, -1, 0, 785, -1, "", 1);// 221
			AddComplexComponent( (BaseAddon) this, 2748, 3, 5, 0, 785, -1, "", 1);// 224
			AddComplexComponent( (BaseAddon) this, 2748, 2, 5, 0, 785, -1, "", 1);// 225
			AddComplexComponent( (BaseAddon) this, 2748, 2, 6, 0, 785, -1, "", 1);// 226
			AddComplexComponent( (BaseAddon) this, 2748, 3, 6, 0, 785, -1, "", 1);// 227
			AddComplexComponent( (BaseAddon) this, 2748, 1, 6, 0, 785, -1, "", 1);// 228
			AddComplexComponent( (BaseAddon) this, 2748, 2, 7, 0, 785, -1, "", 1);// 229
			AddComplexComponent( (BaseAddon) this, 2748, 0, 6, 0, 785, -1, "", 1);// 230
			AddComplexComponent( (BaseAddon) this, 2748, 5, 8, 0, 785, -1, "", 1);// 231
			AddComplexComponent( (BaseAddon) this, 2748, 0, 8, 0, 785, -1, "", 1);// 232
			AddComplexComponent( (BaseAddon) this, 2748, 3, 8, 0, 785, -1, "", 1);// 233
			AddComplexComponent( (BaseAddon) this, 2748, 1, 9, 0, 785, -1, "", 1);// 234
			AddComplexComponent( (BaseAddon) this, 2748, -1, 11, 0, 785, -1, "", 1);// 235
			AddComplexComponent( (BaseAddon) this, 2748, 1, 8, 0, 785, -1, "", 1);// 236
			AddComplexComponent( (BaseAddon) this, 2748, 1, 5, 0, 785, -1, "", 1);// 237
			AddComplexComponent( (BaseAddon) this, 2854, -1, 8, 0, 0, 1, "", 1);// 238
			AddComplexComponent( (BaseAddon) this, 2748, 5, 9, 0, 785, -1, "", 1);// 239
			AddComplexComponent( (BaseAddon) this, 2748, 4, 1, 0, 785, -1, "", 1);// 240
			AddComplexComponent( (BaseAddon) this, 2748, 5, -3, 0, 785, -1, "", 1);// 241
			AddComplexComponent( (BaseAddon) this, 2748, 5, 1, 0, 785, -1, "", 1);// 242
			AddComplexComponent( (BaseAddon) this, 2748, 3, 1, 0, 785, -1, "", 1);// 243
			AddComplexComponent( (BaseAddon) this, 2748, 4, 2, 0, 785, -1, "", 1);// 244
			AddComplexComponent( (BaseAddon) this, 2748, 4, 12, 0, 785, -1, "", 1);// 245
			AddComplexComponent( (BaseAddon) this, 2748, 4, 11, 0, 785, -1, "", 1);// 246
			AddComplexComponent( (BaseAddon) this, 5669, -1, 10, 5, 38, -1, "", 1);// 248
			AddComplexComponent( (BaseAddon) this, 2748, 3, 3, 0, 785, -1, "", 1);// 249
			AddComplexComponent( (BaseAddon) this, 2748, 1, 3, 0, 785, -1, "", 1);// 251
			AddComplexComponent( (BaseAddon) this, 5677, -1, 0, 5, 38, -1, "", 1);// 252
			AddComplexComponent( (BaseAddon) this, 2748, 4, 4, 0, 785, -1, "", 1);// 253
			AddComplexComponent( (BaseAddon) this, 2748, 6, 11, 0, 785, -1, "", 1);// 254
			AddComplexComponent( (BaseAddon) this, 2748, 5, 12, 0, 785, -1, "", 1);// 255
			AddComplexComponent( (BaseAddon) this, 2748, 5, 3, 0, 785, -1, "", 1);// 256
			AddComplexComponent( (BaseAddon) this, 2748, 2, 3, 0, 785, -1, "", 1);// 257
			AddComplexComponent( (BaseAddon) this, 2748, -1, 0, 0, 785, -1, "", 1);// 258
			AddComplexComponent( (BaseAddon) this, 2748, 1, 11, 0, 785, -1, "", 1);// 259
			AddComplexComponent( (BaseAddon) this, 2748, 6, 3, 0, 785, -1, "", 1);// 260
			AddComplexComponent( (BaseAddon) this, 2748, 1, 2, 0, 785, -1, "", 1);// 261
			AddComplexComponent( (BaseAddon) this, 2748, 5, 10, 0, 785, -1, "", 1);// 262
			AddComplexComponent( (BaseAddon) this, 2748, 6, 10, 0, 785, -1, "", 1);// 263
			AddComplexComponent( (BaseAddon) this, 2748, 1, 10, 0, 785, -1, "", 1);// 264
			AddComplexComponent( (BaseAddon) this, 2748, 3, 9, 0, 785, -1, "", 1);// 265
			AddComplexComponent( (BaseAddon) this, 2748, 4, 10, 0, 785, -1, "", 1);// 266
			AddComplexComponent( (BaseAddon) this, 2748, 0, 11, 0, 785, -1, "", 1);// 267
			AddComplexComponent( (BaseAddon) this, 2748, 3, 10, 0, 785, -1, "", 1);// 268
			AddComplexComponent( (BaseAddon) this, 2748, 4, -3, 0, 785, -1, "", 1);// 269
			AddComplexComponent( (BaseAddon) this, 2748, 0, 12, 0, 785, -1, "", 1);// 270
			AddComplexComponent( (BaseAddon) this, 2748, 2, 10, 0, 785, -1, "", 1);// 272
			AddComplexComponent( (BaseAddon) this, 2748, 2, 1, 0, 785, -1, "", 1);// 274
			AddComplexComponent( (BaseAddon) this, 2748, -1, 3, 0, 785, -1, "", 1);// 275
			AddComplexComponent( (BaseAddon) this, 2748, 5, 7, 0, 785, -1, "", 1);// 276
			AddComplexComponent( (BaseAddon) this, 2748, -1, 7, 0, 785, -1, "", 1);// 277
			AddComplexComponent( (BaseAddon) this, 2748, -1, 10, 0, 785, -1, "", 1);// 278
			AddComplexComponent( (BaseAddon) this, 2748, 3, 0, 0, 785, -1, "", 1);// 279
			AddComplexComponent( (BaseAddon) this, 2748, 6, 1, 0, 785, -1, "", 1);// 280
			AddComplexComponent( (BaseAddon) this, 2748, 5, 2, 0, 785, -1, "", 1);// 281
			AddComplexComponent( (BaseAddon) this, 2748, 2, 12, 0, 785, -1, "", 1);// 282
			AddComplexComponent( (BaseAddon) this, 2748, 2, 4, 0, 785, -1, "", 1);// 283
			AddComplexComponent( (BaseAddon) this, 2748, 4, 7, 0, 785, -1, "", 1);// 284
			AddComplexComponent( (BaseAddon) this, 5663, -1, 3, 5, 38, -1, "", 1);// 285
			AddComplexComponent( (BaseAddon) this, 5667, -1, 11, 5, 38, -1, "", 1);// 286
			AddComplexComponent( (BaseAddon) this, 2748, 0, 7, 0, 785, -1, "", 1);// 287
			AddComplexComponent( (BaseAddon) this, 2748, 4, 5, 0, 785, -1, "", 1);// 288
			AddComplexComponent( (BaseAddon) this, 2748, 0, 2, 0, 785, -1, "", 1);// 289
			AddComplexComponent( (BaseAddon) this, 2748, 1, 12, 0, 785, -1, "", 1);// 290
			AddComplexComponent( (BaseAddon) this, 2748, 0, 10, 0, 785, -1, "", 1);// 291
			AddComplexComponent( (BaseAddon) this, 2748, 4, 0, 0, 785, -1, "", 1);// 292
			AddComplexComponent( (BaseAddon) this, 2748, 6, 9, 0, 785, -1, "", 1);// 293
			AddComplexComponent( (BaseAddon) this, 2748, -1, 8, 0, 785, -1, "", 1);// 294
			AddComplexComponent( (BaseAddon) this, 2748, -1, 12, 0, 785, -1, "", 1);// 295
			AddComplexComponent( (BaseAddon) this, 2583, 8, -10, 29, 46, 1, "", 1);// 299
			AddComplexComponent( (BaseAddon) this, 2583, 7, -10, 28, 785, 1, "", 1);// 302
			AddComplexComponent( (BaseAddon) this, 315, 7, -7, 10, 0, 1, "", 1);// 303
			AddComplexComponent( (BaseAddon) this, 2854, 8, 10, 0, 0, 1, "", 1);// 306
			AddComplexComponent( (BaseAddon) this, 2748, 7, 7, 0, 785, -1, "", 1);// 307
			AddComplexComponent( (BaseAddon) this, 2748, 10, 11, 0, 785, -1, "", 1);// 308
			AddComplexComponent( (BaseAddon) this, 2748, 9, 2, 0, 785, -1, "", 1);// 309
			AddComplexComponent( (BaseAddon) this, 2748, 9, 10, 0, 785, -1, "", 1);// 310
			AddComplexComponent( (BaseAddon) this, 2748, 8, 2, 0, 785, -1, "", 1);// 311
			AddComplexComponent( (BaseAddon) this, 2748, 9, 11, 0, 785, -1, "", 1);// 312
			AddComplexComponent( (BaseAddon) this, 2748, 10, 7, 0, 785, -1, "", 1);// 313
			AddComplexComponent( (BaseAddon) this, 2748, 10, 9, 0, 785, -1, "", 1);// 314
			AddComplexComponent( (BaseAddon) this, 2748, 10, 4, 0, 785, -1, "", 1);// 315
			AddComplexComponent( (BaseAddon) this, 2748, 8, 8, 0, 785, -1, "", 1);// 316
			AddComplexComponent( (BaseAddon) this, 2748, 7, 2, 0, 785, -1, "", 1);// 317
			AddComplexComponent( (BaseAddon) this, 2748, 8, 4, 0, 785, -1, "", 1);// 318
			AddComplexComponent( (BaseAddon) this, 2748, 7, 3, 0, 785, -1, "", 1);// 319
			AddComplexComponent( (BaseAddon) this, 2748, 7, 4, 0, 785, -1, "", 1);// 320
			AddComplexComponent( (BaseAddon) this, 2748, 8, 3, 0, 785, -1, "", 1);// 321
			AddComplexComponent( (BaseAddon) this, 2748, 9, -1, 0, 785, -1, "", 1);// 323
			AddComplexComponent( (BaseAddon) this, 2748, 7, -2, 0, 785, -1, "", 1);// 324
			AddComplexComponent( (BaseAddon) this, 2748, 7, -3, 0, 785, -1, "", 1);// 325
			AddComplexComponent( (BaseAddon) this, 2748, 8, 5, 0, 785, -1, "", 1);// 326
			AddComplexComponent( (BaseAddon) this, 2748, 7, 6, 0, 785, -1, "", 1);// 327
			AddComplexComponent( (BaseAddon) this, 2748, 10, 1, 0, 785, -1, "", 1);// 328
			AddComplexComponent( (BaseAddon) this, 2748, 7, 0, 0, 785, -1, "", 1);// 329
			AddComplexComponent( (BaseAddon) this, 2748, 9, 0, 0, 785, -1, "", 1);// 330
			AddComplexComponent( (BaseAddon) this, 2748, 10, 5, 0, 785, -1, "", 1);// 331
			AddComplexComponent( (BaseAddon) this, 2748, 10, -2, 0, 785, -1, "", 1);// 332
			AddComplexComponent( (BaseAddon) this, 2748, 10, 10, 0, 785, -1, "", 1);// 333
			AddComplexComponent( (BaseAddon) this, 2748, 10, 12, 0, 785, -1, "", 1);// 334
			AddComplexComponent( (BaseAddon) this, 2748, 10, 8, 0, 785, -1, "", 1);// 335
			AddComplexComponent( (BaseAddon) this, 2748, 9, 7, 0, 785, -1, "", 1);// 336
			AddComplexComponent( (BaseAddon) this, 4842, 10, -3, 10, 1156, -1, "", 1);// 337
			AddComplexComponent( (BaseAddon) this, 2748, 7, 12, 0, 785, -1, "", 1);// 338
			AddComplexComponent( (BaseAddon) this, 2748, 10, 6, 0, 785, -1, "", 1);// 339
			AddComplexComponent( (BaseAddon) this, 2748, 10, 3, 0, 785, -1, "", 1);// 340
			AddComplexComponent( (BaseAddon) this, 2748, 8, 7, 0, 785, -1, "", 1);// 341
			AddComplexComponent( (BaseAddon) this, 2748, 9, 9, 0, 785, -1, "", 1);// 342
			AddComplexComponent( (BaseAddon) this, 2748, 7, 8, 0, 785, -1, "", 1);// 343
			AddComplexComponent( (BaseAddon) this, 2748, 7, 5, 0, 785, -1, "", 1);// 344
			AddComplexComponent( (BaseAddon) this, 2748, 10, 0, 0, 785, -1, "", 1);// 345
			AddComplexComponent( (BaseAddon) this, 2748, 7, 11, 0, 785, -1, "", 1);// 346
			AddComplexComponent( (BaseAddon) this, 2748, 10, 2, 0, 785, -1, "", 1);// 347
			AddComplexComponent( (BaseAddon) this, 2748, 9, 1, 0, 785, -1, "", 1);// 348
			AddComplexComponent( (BaseAddon) this, 2748, 9, 3, 0, 785, -1, "", 1);// 350
			AddComplexComponent( (BaseAddon) this, 2748, 9, 4, 0, 785, -1, "", 1);// 351
			AddComplexComponent( (BaseAddon) this, 2748, 7, 10, 0, 785, -1, "", 1);// 352
			AddComplexComponent( (BaseAddon) this, 2748, 8, 10, 0, 785, -1, "", 1);// 353
			AddComplexComponent( (BaseAddon) this, 2748, 8, 0, 0, 785, -1, "", 1);// 354
			AddComplexComponent( (BaseAddon) this, 2748, 8, 11, 0, 785, -1, "", 1);// 355
			AddComplexComponent( (BaseAddon) this, 2748, 8, -1, 0, 785, -1, "", 1);// 356
			AddComplexComponent( (BaseAddon) this, 2748, 7, 9, 0, 785, -1, "", 1);// 357
			AddComplexComponent( (BaseAddon) this, 2748, 9, 12, 0, 785, -1, "", 1);// 358
			AddComplexComponent( (BaseAddon) this, 2748, 7, 1, 0, 785, -1, "", 1);// 359
			AddComplexComponent( (BaseAddon) this, 2748, 8, 12, 0, 785, -1, "", 1);// 361
			AddComplexComponent( (BaseAddon) this, 2748, 8, 6, 0, 785, -1, "", 1);// 362
			AddComplexComponent( (BaseAddon) this, 2748, 9, 5, 0, 785, -1, "", 1);// 363
			AddComplexComponent( (BaseAddon) this, 2748, 7, -1, 0, 785, -1, "", 1);// 364
			AddComplexComponent( (BaseAddon) this, 2748, 8, 1, 0, 785, -1, "", 1);// 365
			AddComplexComponent( (BaseAddon) this, 2748, 10, -1, 0, 785, -1, "", 1);// 366
			AddComplexComponent( (BaseAddon) this, 2748, 9, 6, 0, 785, -1, "", 1);// 367
			AddComplexComponent( (BaseAddon) this, 2748, 8, 9, 0, 785, -1, "", 1);// 368
			AddComplexComponent( (BaseAddon) this, 2748, 9, 8, 0, 785, -1, "", 1);// 369

		}

		public KingsHallAddon( Serial serial ) : base( serial )
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

	public class KingsHallAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new KingsHallAddon();
			}
		}

		[Constructable]
		public KingsHallAddonDeed()
		{
			Name = "KingsHall";
		}

		public KingsHallAddonDeed( Serial serial ) : base( serial )
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