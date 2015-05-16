
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class minictfarenaAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1313, -7, -6, 5}, {1313, 1, -6, 5}, {6013, -13, -12, 5}// 1	3	8	
			, {1313, -4, -6, 5}, {3206, 1, -6, 10}, {1313, 0, -6, 5}// 9	11	13	
			, {6013, -13, -6, 5}, {1313, -5, -6, 5}, {1313, -9, -6, 5}// 14	17	18	
			, {1313, -2, -6, 5}, {1313, -3, -6, 5}, {1313, -1, -6, 5}// 20	21	23	
			, {1313, -8, -6, 5}, {6013, -13, -7, 5}, {6013, -13, -11, 5}// 24	29	30	
			, {6013, -13, -8, 5}, {6013, -13, -9, 5}, {6013, -13, -10, 5}// 31	32	33	
			, {1313, 0, -4, 5}, {1313, -4, 2, 5}, {1313, -1, 2, 5}// 35	37	38	
			, {1313, -3, -2, 5}, {1313, -1, -2, 5}, {1313, -4, 5, 5}// 39	41	42	
			, {1313, -9, -3, 5}, {3203, -5, 1, 10}, {1313, -1, 4, 5}// 45	49	52	
			, {3203, 1, 5, 10}, {1313, -5, -5, 5}, {3206, 1, 6, 10}// 53	54	56	
			, {1313, 0, 6, 5}, {1313, 0, -2, 5}, {1313, 0, 4, 5}// 57	58	59	
			, {3203, -4, 0, 10}, {1313, -5, 1, 5}, {1313, -5, 7, 5}// 60	63	64	
			, {1313, -5, 6, 5}, {1313, -5, 4, 5}, {1313, -4, -1, 5}// 65	67	68	
			, {1313, -8, -5, 5}, {3206, -2, 1, 10}, {1313, -3, -4, 5}// 69	70	73	
			, {1313, -3, -5, 5}, {1313, -3, -1, 5}, {1313, -4, 4, 5}// 74	75	76	
			, {1313, -5, 2, 5}, {1313, -2, 2, 5}, {1313, -1, -4, 5}// 91	93	94	
			, {1313, -1, -3, 5}, {1313, -2, 10, 5}, {1313, -2, 9, 5}// 95	96	97	
			, {7107, -7, -5, 5}, {3206, 1, -4, 10}, {1313, 0, 2, 5}// 100	101	103	
			, {3203, 1, -5, 10}, {1313, -1, 6, 5}, {3206, -5, 0, 10}// 107	108	109	
			, {3206, 1, 4, 10}, {1313, 0, -3, 5}, {1313, -4, 6, 5}// 111	115	120	
			, {1313, 0, 3, 5}, {1313, -5, 9, 5}, {3203, 1, 2, 10}// 123	125	126	
			, {1313, -7, -3, 5}, {1313, -3, 2, 5}, {1313, -3, 3, 5}// 134	135	136	
			, {1313, -3, 4, 5}, {1313, -3, 5, 5}, {1313, -3, 6, 5}// 137	138	139	
			, {1313, 0, 5, 5}, {1313, -3, 7, 5}, {1313, -4, 10, 5}// 140	141	142	
			, {1313, -1, 0, 5}, {1313, -2, -5, 5}, {1313, -2, -4, 5}// 143	144	145	
			, {3206, -4, 1, 10}, {1313, -2, -3, 5}, {1313, -5, -4, 5}// 146	147	148	
			, {1313, -4, 3, 5}, {1313, -2, 3, 5}, {1313, -2, 4, 5}// 149	150	151	
			, {1313, -2, 5, 5}, {3206, 0, 1, 10}, {1313, -7, -5, 5}// 152	153	157	
			, {3203, 0, 0, 10}, {1313, 0, 7, 5}, {3203, -2, 0, 10}// 160	163	164	
			, {1313, -3, 10, 5}, {6039, 1, 1, 10}, {1313, -4, -3, 5}// 167	168	169	
			, {3206, -3, 0, 10}, {1313, -5, -3, 5}, {1313, -4, -4, 5}// 170	172	179	
			, {1313, -1, 7, 5}, {1313, -9, -5, 5}, {1313, 0, -5, 5}// 180	181	189	
			, {1313, 0, -1, 5}, {1313, -5, -2, 5}, {3203, -3, 1, 10}// 190	191	192	
			, {1313, -2, 7, 5}, {3206, 1, -1, 10}, {1313, -3, -3, 5}// 193	194	195	
			, {1313, -1, -1, 5}, {1313, -2, -2, 5}, {1313, -2, -1, 5}// 196	197	198	
			, {1313, -1, -5, 5}, {1313, -2, 6, 5}, {1313, -8, -3, 5}// 200	201	202	
			, {3203, 1, -3, 10}, {1313, 1, -2, 5}, {1313, -1, 3, 5}// 203	204	205	
			, {1313, -4, -2, 5}, {1313, -8, -4, 5}, {1313, -3, 9, 5}// 206	207	208	
			, {6039, 1, 0, 10}, {1313, -1, 1, 5}, {1313, -7, -4, 5}// 210	211	214	
			, {3203, 1, 7, 10}, {1313, -5, 3, 5}, {1313, -5, 5, 5}// 215	216	220	
			, {1313, -4, 7, 5}, {1313, -4, -5, 5}, {1313, -9, -4, 5}// 225	226	227	
			, {1313, 1, 7, 5}, {1313, -5, 0, 5}, {1313, -5, -1, 5}// 229	230	231	
			, {1313, -1, 5, 5}, {1313, 1, 3, 5}, {6013, -12, -5, 0}// 232	234	237	
			, {1305, -5, 10, 5}, {1313, -4, 11, 5}, {1313, -5, 11, 5}// 238	247	250	
			, {1313, -2, 11, 5}, {1313, -3, 11, 5}, {3203, 2, -6, 10}// 251	254	279	
			, {1313, 6, -6, 5}, {1313, 7, -6, 5}, {1313, 8, -6, 5}// 281	282	283	
			, {1313, 5, -9, 5}, {1313, 5, -10, 5}, {1313, 5, -8, 5}// 284	289	291	
			, {1313, 6, -10, 5}, {1313, 6, -9, 5}, {1313, 6, -8, 5}// 292	293	294	
			, {1313, 7, -10, 5}, {1313, 7, -9, 5}, {1313, 8, -10, 5}// 295	296	298	
			, {1313, 8, -9, 5}, {1313, 8, -8, 5}, {1313, 2, -6, 5}// 299	300	302	
			, {1313, 3, -6, 5}, {1313, 4, -6, 5}, {1313, 5, -6, 5}// 303	304	305	
			, {3203, 6, 0, 10}, {1313, 3, -2, 5}, {3203, 2, 4, 10}// 307	308	310	
			, {6039, 2, 1, 10}, {3206, 6, 1, 10}, {1313, 11, 4, 5}// 311	314	315	
			, {1313, 2, 3, 5}, {1313, 4, 1, 5}, {3206, 2, 7, 10}// 317	328	343	
			, {3206, 3, 0, 10}, {3203, 2, -1, 10}, {3206, 2, 5, 10}// 344	346	347	
			, {3203, 8, 0, 10}, {3206, 5, 0, 10}, {3203, 2, 6, 10}// 354	357	359	
			, {1313, 10, 6, 5}, {1313, 6, -5, 5}, {1313, 6, -4, 5}// 360	361	362	
			, {1313, 6, -3, 5}, {1313, 6, -2, 5}, {1313, 6, -1, 5}// 363	364	365	
			, {1313, 6, 2, 5}, {1313, 6, 3, 5}, {1313, 6, 4, 5}// 366	367	368	
			, {1313, 6, 5, 5}, {1313, 6, 6, 5}, {1313, 6, 7, 5}// 369	370	371	
			, {1313, 7, -5, 5}, {1313, 7, -4, 5}, {1313, 7, -3, 5}// 372	373	374	
			, {1313, 7, -2, 5}, {1313, 7, -1, 5}, {1313, 7, 2, 5}// 375	376	377	
			, {1313, 7, 3, 5}, {1313, 7, 4, 5}, {1313, 7, 5, 5}// 378	379	380	
			, {1313, 7, 6, 5}, {1313, 7, 7, 5}, {1313, 8, -5, 5}// 381	382	383	
			, {1313, 8, -4, 5}, {1313, 8, -3, 5}, {1313, 8, -2, 5}// 384	385	386	
			, {1313, 8, -1, 5}, {1313, 8, 0, 5}, {1313, 8, 1, 5}// 387	388	389	
			, {1313, 8, 2, 5}, {1313, 8, 3, 5}, {1313, 8, 4, 5}// 390	391	392	
			, {1313, 8, 5, 5}, {1313, 8, 6, 5}, {1313, 8, 7, 5}// 393	394	395	
			, {1313, 10, 7, 5}, {1313, 11, 5, 5}, {3203, 5, 1, 10}// 403	404	406	
			, {1313, 4, 0, 5}, {1313, 10, 5, 5}, {1313, 3, -1, 5}// 407	408	411	
			, {3206, 8, 1, 10}, {3206, 2, -3, 10}, {1313, 11, 7, 5}// 415	416	435	
			, {3206, 2, -5, 10}, {3203, 3, 1, 10}, {1313, 12, 5, 5}// 436	444	455	
			, {6039, 2, 0, 10}, {1313, 12, 6, 5}, {1313, 12, 7, 5}// 457	458	459	
			, {1313, 11, 6, 5}, {3206, 7, 0, 10}, {1313, 12, 4, 5}// 461	467	468	
			, {1313, 10, 4, 5}, {1313, 2, -2, 5}, {3206, 2, 2, 10}// 470	476	477	
			, {1313, 3, 2, 5}, {3203, 7, 1, 10}, {3203, 2, -4, 10}// 478	480	481	
			, {1313, 3, -5, 5}, {1313, 3, -4, 5}, {1313, 3, -3, 5}// 483	484	485	
			, {1313, 3, 3, 5}, {1313, 3, 4, 5}, {1313, 3, 5, 5}// 487	488	489	
			, {1313, 3, 6, 5}, {1313, 3, 7, 5}, {1313, 4, -5, 5}// 490	491	492	
			, {1313, 4, -4, 5}, {1313, 4, -3, 5}, {1313, 4, -2, 5}// 493	494	495	
			, {1313, 4, -1, 5}, {1313, 4, 2, 5}, {1313, 4, 3, 5}// 496	497	498	
			, {1313, 4, 4, 5}, {1313, 4, 5, 5}, {1313, 4, 6, 5}// 499	500	501	
			, {1313, 4, 7, 5}, {1313, 5, -5, 5}, {1313, 5, -4, 5}// 502	503	504	
			, {1313, 5, -3, 5}, {1313, 5, -2, 5}, {1313, 5, -1, 5}// 505	506	507	
			, {1313, 5, 2, 5}, {1313, 5, 3, 5}, {1313, 5, 4, 5}// 508	509	510	
			, {1313, 5, 5, 5}, {1313, 5, 6, 5}, {1313, 5, 7, 5}// 511	512	513	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new minictfarenaAddonDeed();
			}
		}

		[ Constructable ]
		public minictfarenaAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 1822, -6, -7, 5, 1365, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 1822, 1, -7, 5, 1365, -1, "", 1);// 4
			AddComplexComponent( (BaseAddon) this, 1822, -2, -7, 5, 1365, -1, "", 1);// 5
			AddComplexComponent( (BaseAddon) this, 1822, -1, -7, 5, 1365, -1, "", 1);// 6
			AddComplexComponent( (BaseAddon) this, 1822, -10, -6, 5, 1365, -1, "", 1);// 7
			AddComplexComponent( (BaseAddon) this, 1822, -9, -7, 5, 1365, -1, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 1822, -5, -7, 5, 1365, -1, "", 1);// 12
			AddComplexComponent( (BaseAddon) this, 1822, -10, -7, 5, 1365, -1, "", 1);// 15
			AddComplexComponent( (BaseAddon) this, 1822, -7, -7, 5, 1365, -1, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 1822, -6, -6, 5, 1365, -1, "", 1);// 19
			AddComplexComponent( (BaseAddon) this, 1822, 1, -6, 5, 1365, -1, "", 1);// 22
			AddComplexComponent( (BaseAddon) this, 1822, -8, -7, 5, 1365, -1, "", 1);// 25
			AddComplexComponent( (BaseAddon) this, 1822, -3, -7, 5, 1365, -1, "", 1);// 26
			AddComplexComponent( (BaseAddon) this, 1822, -4, -7, 5, 1365, -1, "", 1);// 27
			AddComplexComponent( (BaseAddon) this, 1822, 0, -7, 5, 1365, -1, "", 1);// 28
			AddComplexComponent( (BaseAddon) this, 1822, -4, -2, 5, 1365, -1, "", 1);// 34
			AddComplexComponent( (BaseAddon) this, 1822, -5, 1, 5, 1109, -1, "", 1);// 36
			AddComplexComponent( (BaseAddon) this, 1822, -1, 9, 5, 1109, -1, "", 1);// 40
			AddComplexComponent( (BaseAddon) this, 1822, -4, 9, 5, 1109, -1, "", 1);// 43
			AddComplexComponent( (BaseAddon) this, 1822, -5, 0, 5, 1365, -1, "", 1);// 44
			AddComplexComponent( (BaseAddon) this, 1822, -1, -4, 5, 1365, -1, "", 1);// 46
			AddComplexComponent( (BaseAddon) this, 1822, -2, 3, 5, 1109, -1, "", 1);// 47
			AddComplexComponent( (BaseAddon) this, 1822, 1, 8, 5, 1109, -1, "", 1);// 48
			AddComplexComponent( (BaseAddon) this, 1822, -1, 10, 5, 1109, -1, "", 1);// 50
			AddComplexComponent( (BaseAddon) this, 1822, -5, 8, 0, 1109, -1, "", 1);// 51
			AddComplexComponent( (BaseAddon) this, 1822, -4, 0, 5, 1365, -1, "", 1);// 55
			AddComplexComponent( (BaseAddon) this, 1822, -4, 3, 5, 1109, -1, "", 1);// 61
			AddComplexComponent( (BaseAddon) this, 1822, -1, -3, 5, 1365, -1, "", 1);// 62
			AddComplexComponent( (BaseAddon) this, 1822, -2, -2, 5, 1365, -1, "", 1);// 66
			AddComplexComponent( (BaseAddon) this, 1822, -6, -4, 5, 1365, -1, "", 1);// 71
			AddComplexComponent( (BaseAddon) this, 1822, -6, 9, 5, 1109, -1, "", 1);// 72
			AddComplexComponent( (BaseAddon) this, 1822, -6, 10, 5, 1109, -1, "", 1);// 77
			AddComplexComponent( (BaseAddon) this, 1822, -9, -2, 5, 1365, -1, "", 1);// 78
			AddComplexComponent( (BaseAddon) this, 1822, -10, -3, 5, 1365, -1, "", 1);// 79
			AddComplexComponent( (BaseAddon) this, 1822, -10, -2, 5, 1365, -1, "", 1);// 80
			AddComplexComponent( (BaseAddon) this, 1822, -8, -2, 5, 1365, -1, "", 1);// 81
			AddComplexComponent( (BaseAddon) this, 1822, -7, -2, 5, 1365, -1, "", 1);// 82
			AddComplexComponent( (BaseAddon) this, 1822, -9, -2, 0, 1365, -1, "", 1);// 83
			AddComplexComponent( (BaseAddon) this, 1822, -1, 10, 0, 1109, -1, "", 1);// 84
			AddComplexComponent( (BaseAddon) this, 1822, -7, -2, 0, 1365, -1, "", 1);// 85
			AddComplexComponent( (BaseAddon) this, 1822, -1, 9, 0, 1109, -1, "", 1);// 86
			AddComplexComponent( (BaseAddon) this, 1822, -8, -2, 0, 1365, -1, "", 1);// 87
			AddComplexComponent( (BaseAddon) this, 1822, 1, -5, 5, 1365, -1, "", 1);// 88
			AddComplexComponent( (BaseAddon) this, 1822, -1, -3, 10, 1365, -1, "", 1);// 89
			AddComplexComponent( (BaseAddon) this, 1822, 1, 7, 5, 1109, -1, "", 1);// 90
			AddComplexComponent( (BaseAddon) this, 1822, -2, 3, 10, 1109, -1, "", 1);// 92
			AddComplexComponent( (BaseAddon) this, 1822, -1, 5, 10, 1109, -1, "", 1);// 98
			AddComplexComponent( (BaseAddon) this, 1822, 0, 1, 5, 1109, -1, "", 1);// 99
			AddComplexComponent( (BaseAddon) this, 1822, -2, -2, 10, 1365, -1, "", 1);// 102
			AddComplexComponent( (BaseAddon) this, 1822, 1, 8, 0, 1109, -1, "", 1);// 104
			AddComplexComponent( (BaseAddon) this, 1822, -6, -1, 5, 1365, -1, "", 1);// 105
			AddComplexComponent( (BaseAddon) this, 1822, 0, 8, 0, 1109, -1, "", 1);// 106
			AddComplexComponent( (BaseAddon) this, 1822, 1, -1, 5, 1365, -1, "", 1);// 110
			AddComplexComponent( (BaseAddon) this, 1822, -1, 8, 5, 1109, -1, "", 1);// 112
			AddComplexComponent( (BaseAddon) this, 1822, -6, 0, 5, 1365, -1, "", 1);// 113
			AddComplexComponent( (BaseAddon) this, 1822, -3, -2, 5, 1365, -1, "", 1);// 114
			AddComplexComponent( (BaseAddon) this, 1822, -4, 8, 5, 1109, -1, "", 1);// 116
			AddComplexComponent( (BaseAddon) this, 1822, -5, 8, 5, 1109, -1, "", 1);// 117
			AddComplexComponent( (BaseAddon) this, 1822, -6, 1, 5, 1109, -1, "", 1);// 118
			AddComplexComponent( (BaseAddon) this, 1822, -6, 4, 5, 1109, -1, "", 1);// 119
			AddComplexComponent( (BaseAddon) this, 1822, -7, -5, 5, 1365, -1, "", 1);// 121
			AddComplexComponent( (BaseAddon) this, 1822, -1, 3, 10, 1109, -1, "", 1);// 122
			AddComplexComponent( (BaseAddon) this, 1822, -3, 3, 5, 1109, -1, "", 1);// 124
			AddComplexComponent( (BaseAddon) this, 1822, -6, 2, 5, 1109, -1, "", 1);// 127
			AddComplexComponent( (BaseAddon) this, 1822, -1, -2, 10, 1365, -1, "", 1);// 128
			AddComplexComponent( (BaseAddon) this, 1822, 1, 0, 5, 1365, -1, "", 1);// 129
			AddComplexComponent( (BaseAddon) this, 1822, 1, 1, 5, 1109, -1, "", 1);// 130
			AddComplexComponent( (BaseAddon) this, 1822, 1, 4, 5, 1109, -1, "", 1);// 131
			AddComplexComponent( (BaseAddon) this, 1822, 1, 5, 5, 1109, -1, "", 1);// 132
			AddComplexComponent( (BaseAddon) this, 1822, 1, 6, 5, 1109, -1, "", 1);// 133
			AddComplexComponent( (BaseAddon) this, 1822, -1, 4, 10, 1109, -1, "", 1);// 154
			AddComplexComponent( (BaseAddon) this, 1822, -6, -2, 5, 1365, -1, "", 1);// 155
			AddComplexComponent( (BaseAddon) this, 1313, -4, 9, 5, 1109, -1, "", 1);// 156
			AddComplexComponent( (BaseAddon) this, 1822, -1, 8, 0, 1109, -1, "", 1);// 158
			AddComplexComponent( (BaseAddon) this, 1822, -4, 8, 0, 1109, -1, "", 1);// 159
			AddComplexComponent( (BaseAddon) this, 1822, 0, 8, 5, 1109, -1, "", 1);// 161
			AddComplexComponent( (BaseAddon) this, 1822, -1, -4, 10, 1365, -1, "", 1);// 162
			AddComplexComponent( (BaseAddon) this, 1822, -2, 8, 5, 1109, -1, "", 1);// 165
			AddComplexComponent( (BaseAddon) this, 1822, -3, -2, 10, 1365, -1, "", 1);// 166
			AddComplexComponent( (BaseAddon) this, 1822, -6, -5, 5, 1365, -1, "", 1);// 171
			AddComplexComponent( (BaseAddon) this, 1822, -1, 3, 5, 1109, -1, "", 1);// 173
			AddComplexComponent( (BaseAddon) this, 1822, -1, 4, 5, 1109, -1, "", 1);// 174
			AddComplexComponent( (BaseAddon) this, 1822, -1, 5, 5, 1109, -1, "", 1);// 175
			AddComplexComponent( (BaseAddon) this, 1822, -1, 6, 5, 1109, -1, "", 1);// 176
			AddComplexComponent( (BaseAddon) this, 1822, -1, -5, 5, 1365, -1, "", 1);// 177
			AddComplexComponent( (BaseAddon) this, 1822, -6, -3, 5, 1365, -1, "", 1);// 178
			AddComplexComponent( (BaseAddon) this, 1822, -4, 1, 5, 1109, -1, "", 1);// 182
			AddComplexComponent( (BaseAddon) this, 1822, -3, 0, 5, 1365, -1, "", 1);// 183
			AddComplexComponent( (BaseAddon) this, 1822, -3, 1, 5, 1109, -1, "", 1);// 184
			AddComplexComponent( (BaseAddon) this, 1822, -2, 0, 5, 1365, -1, "", 1);// 185
			AddComplexComponent( (BaseAddon) this, 1822, -2, 1, 5, 1109, -1, "", 1);// 186
			AddComplexComponent( (BaseAddon) this, 1822, -6, 6, 5, 1109, -1, "", 1);// 187
			AddComplexComponent( (BaseAddon) this, 1822, -6, 7, 5, 1109, -1, "", 1);// 188
			AddComplexComponent( (BaseAddon) this, 1822, -3, 3, 10, 1109, -1, "", 1);// 199
			AddComplexComponent( (BaseAddon) this, 1822, -10, -5, 5, 1365, -1, "", 1);// 209
			AddComplexComponent( (BaseAddon) this, 1822, -2, 8, 0, 1109, -1, "", 1);// 212
			AddComplexComponent( (BaseAddon) this, 1822, -3, 8, 0, 1109, -1, "", 1);// 213
			AddComplexComponent( (BaseAddon) this, 1822, -6, 8, 5, 1109, -1, "", 1);// 217
			AddComplexComponent( (BaseAddon) this, 1822, 0, 0, 5, 1365, -1, "", 1);// 218
			AddComplexComponent( (BaseAddon) this, 1822, 1, 2, 5, 1109, -1, "", 1);// 219
			AddComplexComponent( (BaseAddon) this, 1822, -3, 8, 5, 1109, -1, "", 1);// 221
			AddComplexComponent( (BaseAddon) this, 1822, -1, -2, 5, 1365, -1, "", 1);// 222
			AddComplexComponent( (BaseAddon) this, 1822, -6, 5, 5, 1109, -1, "", 1);// 223
			AddComplexComponent( (BaseAddon) this, 1822, -10, -4, 5, 1365, -1, "", 1);// 224
			AddComplexComponent( (BaseAddon) this, 1822, -6, 3, 5, 1109, -1, "", 1);// 228
			AddComplexComponent( (BaseAddon) this, 1822, 1, -3, 5, 1365, -1, "", 1);// 233
			AddComplexComponent( (BaseAddon) this, 1822, 1, -4, 5, 1365, -1, "", 1);// 235
			AddComplexComponent( (BaseAddon) this, 1822, -10, -2, 0, 1365, -1, "", 1);// 236
			AddComplexComponent( (BaseAddon) this, 1822, -2, 12, 5, 1109, -1, "", 1);// 239
			AddComplexComponent( (BaseAddon) this, 1822, -4, 12, 0, 1109, -1, "", 1);// 240
			AddComplexComponent( (BaseAddon) this, 1822, -5, 12, 5, 1109, -1, "", 1);// 241
			AddComplexComponent( (BaseAddon) this, 1822, -1, 11, 0, 1109, -1, "", 1);// 242
			AddComplexComponent( (BaseAddon) this, 1822, -6, 12, 0, 1109, -1, "", 1);// 243
			AddComplexComponent( (BaseAddon) this, 1822, -3, 12, 0, 1109, -1, "", 1);// 244
			AddComplexComponent( (BaseAddon) this, 1822, -2, 12, 0, 1109, -1, "", 1);// 245
			AddComplexComponent( (BaseAddon) this, 1822, -1, 12, 0, 1109, -1, "", 1);// 246
			AddComplexComponent( (BaseAddon) this, 1822, -3, 12, 5, 1109, -1, "", 1);// 248
			AddComplexComponent( (BaseAddon) this, 1822, -1, 12, 5, 1109, -1, "", 1);// 249
			AddComplexComponent( (BaseAddon) this, 1822, -4, 12, 5, 1109, -1, "", 1);// 252
			AddComplexComponent( (BaseAddon) this, 1822, -5, 12, 0, 1109, -1, "", 1);// 253
			AddComplexComponent( (BaseAddon) this, 1822, -1, 11, 5, 1109, -1, "", 1);// 255
			AddComplexComponent( (BaseAddon) this, 1822, -6, 11, 5, 1109, -1, "", 1);// 256
			AddComplexComponent( (BaseAddon) this, 1822, -6, 12, 5, 1109, -1, "", 1);// 257
			AddComplexComponent( (BaseAddon) this, 1822, 2, -6, 5, 1645, -1, "", 1);// 258
			AddComplexComponent( (BaseAddon) this, 1822, 6, -7, 5, 1645, -1, "", 1);// 259
			AddComplexComponent( (BaseAddon) this, 1822, 7, -7, 5, 1645, -1, "", 1);// 260
			AddComplexComponent( (BaseAddon) this, 1822, 8, -7, 5, 1645, -1, "", 1);// 261
			AddComplexComponent( (BaseAddon) this, 1822, 7, -11, 5, 1645, -1, "", 1);// 262
			AddComplexComponent( (BaseAddon) this, 1822, 3, -7, 5, 1645, -1, "", 1);// 263
			AddComplexComponent( (BaseAddon) this, 1822, 4, -7, 5, 1645, -1, "", 1);// 264
			AddComplexComponent( (BaseAddon) this, 1822, 5, -7, 5, 1645, -1, "", 1);// 265
			AddComplexComponent( (BaseAddon) this, 1822, 5, -11, 5, 1645, -1, "", 1);// 266
			AddComplexComponent( (BaseAddon) this, 1822, 4, -10, 5, 1645, -1, "", 1);// 267
			AddComplexComponent( (BaseAddon) this, 1822, 4, -9, 5, 1645, -1, "", 1);// 268
			AddComplexComponent( (BaseAddon) this, 1822, 4, -8, 5, 1645, -1, "", 1);// 269
			AddComplexComponent( (BaseAddon) this, 1822, 9, -10, 0, 1645, -1, "", 1);// 270
			AddComplexComponent( (BaseAddon) this, 1822, 9, -11, 5, 1645, -1, "", 1);// 271
			AddComplexComponent( (BaseAddon) this, 1822, 9, -10, 5, 1645, -1, "", 1);// 272
			AddComplexComponent( (BaseAddon) this, 1822, 9, -9, 5, 1645, -1, "", 1);// 273
			AddComplexComponent( (BaseAddon) this, 1822, 9, -8, 5, 1645, -1, "", 1);// 274
			AddComplexComponent( (BaseAddon) this, 1822, 8, -11, 5, 1645, -1, "", 1);// 275
			AddComplexComponent( (BaseAddon) this, 1822, 9, -8, 0, 1645, -1, "", 1);// 276
			AddComplexComponent( (BaseAddon) this, 1822, 9, -9, 0, 1645, -1, "", 1);// 277
			AddComplexComponent( (BaseAddon) this, 1822, 6, -11, 5, 1645, -1, "", 1);// 278
			AddComplexComponent( (BaseAddon) this, 1822, 2, -7, 5, 1645, -1, "", 1);// 280
			AddComplexComponent( (BaseAddon) this, 1822, 4, -11, 5, 1645, -1, "", 1);// 285
			AddComplexComponent( (BaseAddon) this, 1822, 7, -8, 5, 1645, -1, "", 1);// 286
			AddComplexComponent( (BaseAddon) this, 1822, 9, -6, 0, 1645, -1, "", 1);// 287
			AddComplexComponent( (BaseAddon) this, 1822, 9, -7, 5, 1645, -1, "", 1);// 288
			AddComplexComponent( (BaseAddon) this, 1822, 9, -6, 5, 1645, -1, "", 1);// 290
			AddComplexComponent( (BaseAddon) this, 1313, 7, -8, 5, 1645, -1, "", 1);// 297
			AddComplexComponent( (BaseAddon) this, 1822, 9, -7, 0, 1645, -1, "", 1);// 301
			AddComplexComponent( (BaseAddon) this, 1822, 9, -11, 0, 1645, -1, "", 1);// 306
			AddComplexComponent( (BaseAddon) this, 1822, 2, 8, 5, 1367, -1, "", 1);// 309
			AddComplexComponent( (BaseAddon) this, 1822, 2, 7, 5, 1367, -1, "", 1);// 312
			AddComplexComponent( (BaseAddon) this, 1822, 4, -4, 5, 1645, -1, "", 1);// 313
			AddComplexComponent( (BaseAddon) this, 1822, 3, 0, 5, 1645, -1, "", 1);// 316
			AddComplexComponent( (BaseAddon) this, 1822, 9, 8, 0, 1367, -1, "", 1);// 318
			AddComplexComponent( (BaseAddon) this, 1822, 2, 0, 5, 1645, -1, "", 1);// 319
			AddComplexComponent( (BaseAddon) this, 1822, 5, 3, 5, 1367, -1, "", 1);// 320
			AddComplexComponent( (BaseAddon) this, 1822, 9, 1, 5, 1367, -1, "", 1);// 321
			AddComplexComponent( (BaseAddon) this, 1822, 4, 3, 10, 1367, -1, "", 1);// 322
			AddComplexComponent( (BaseAddon) this, 1822, 9, -1, 5, 1645, -1, "", 1);// 323
			AddComplexComponent( (BaseAddon) this, 1822, 4, 4, 5, 1367, -1, "", 1);// 324
			AddComplexComponent( (BaseAddon) this, 1822, 7, -2, 5, 1645, -1, "", 1);// 325
			AddComplexComponent( (BaseAddon) this, 1822, 6, -2, 5, 1645, -1, "", 1);// 326
			AddComplexComponent( (BaseAddon) this, 1822, 6, 3, 10, 1367, -1, "", 1);// 327
			AddComplexComponent( (BaseAddon) this, 1822, 11, 3, 5, 1367, -1, "", 1);// 329
			AddComplexComponent( (BaseAddon) this, 1822, 13, 6, 5, 1367, -1, "", 1);// 330
			AddComplexComponent( (BaseAddon) this, 1822, 10, 8, 5, 1367, -1, "", 1);// 331
			AddComplexComponent( (BaseAddon) this, 1822, 13, 5, 5, 1367, -1, "", 1);// 332
			AddComplexComponent( (BaseAddon) this, 1822, 10, 3, 5, 1367, -1, "", 1);// 333
			AddComplexComponent( (BaseAddon) this, 1822, 11, 8, 5, 1367, -1, "", 1);// 334
			AddComplexComponent( (BaseAddon) this, 1822, 12, 8, 5, 1367, -1, "", 1);// 335
			AddComplexComponent( (BaseAddon) this, 1822, 13, 8, 5, 1367, -1, "", 1);// 336
			AddComplexComponent( (BaseAddon) this, 1822, 13, 4, 5, 1367, -1, "", 1);// 337
			AddComplexComponent( (BaseAddon) this, 1822, 12, 3, 5, 1367, -1, "", 1);// 338
			AddComplexComponent( (BaseAddon) this, 1822, 13, 3, 5, 1367, -1, "", 1);// 339
			AddComplexComponent( (BaseAddon) this, 1822, 13, 7, 5, 1367, -1, "", 1);// 340
			AddComplexComponent( (BaseAddon) this, 1822, 10, 8, 0, 1367, -1, "", 1);// 341
			AddComplexComponent( (BaseAddon) this, 1822, 9, 5, 5, 1367, -1, "", 1);// 342
			AddComplexComponent( (BaseAddon) this, 1822, 3, 8, 5, 1367, -1, "", 1);// 345
			AddComplexComponent( (BaseAddon) this, 1822, 9, 7, 0, 1367, -1, "", 1);// 348
			AddComplexComponent( (BaseAddon) this, 1822, 9, 4, 5, 1367, -1, "", 1);// 349
			AddComplexComponent( (BaseAddon) this, 1822, 9, 5, 0, 1367, -1, "", 1);// 350
			AddComplexComponent( (BaseAddon) this, 1822, 10, 6, 5, 1367, -1, "", 1);// 351
			AddComplexComponent( (BaseAddon) this, 1822, 9, 3, 0, 1367, -1, "", 1);// 352
			AddComplexComponent( (BaseAddon) this, 1822, 9, -5, 5, 1645, -1, "", 1);// 353
			AddComplexComponent( (BaseAddon) this, 1822, 4, -3, 10, 1645, -1, "", 1);// 355
			AddComplexComponent( (BaseAddon) this, 1822, 4, 6, 5, 1367, -1, "", 1);// 356
			AddComplexComponent( (BaseAddon) this, 1822, 9, 7, 5, 1367, -1, "", 1);// 358
			AddComplexComponent( (BaseAddon) this, 1822, 9, 0, 5, 1645, -1, "", 1);// 396
			AddComplexComponent( (BaseAddon) this, 1822, 9, -2, 5, 1645, -1, "", 1);// 397
			AddComplexComponent( (BaseAddon) this, 1822, 2, -5, 5, 1645, -1, "", 1);// 398
			AddComplexComponent( (BaseAddon) this, 1822, 2, 2, 5, 1367, -1, "", 1);// 399
			AddComplexComponent( (BaseAddon) this, 1822, 9, 8, 5, 1367, -1, "", 1);// 400
			AddComplexComponent( (BaseAddon) this, 1822, 4, 5, 5, 1367, -1, "", 1);// 401
			AddComplexComponent( (BaseAddon) this, 1822, 5, -2, 5, 1645, -1, "", 1);// 402
			AddComplexComponent( (BaseAddon) this, 1822, 5, -2, 10, 1645, -1, "", 1);// 405
			AddComplexComponent( (BaseAddon) this, 1822, 7, 8, 5, 1367, -1, "", 1);// 409
			AddComplexComponent( (BaseAddon) this, 1822, 9, -3, 5, 1645, -1, "", 1);// 410
			AddComplexComponent( (BaseAddon) this, 1822, 5, 8, 5, 1367, -1, "", 1);// 412
			AddComplexComponent( (BaseAddon) this, 1822, 5, 3, 10, 1367, -1, "", 1);// 413
			AddComplexComponent( (BaseAddon) this, 1822, 4, 8, 5, 1367, -1, "", 1);// 414
			AddComplexComponent( (BaseAddon) this, 1822, 4, -5, 5, 1645, -1, "", 1);// 417
			AddComplexComponent( (BaseAddon) this, 1822, 8, 8, 0, 1367, -1, "", 1);// 418
			AddComplexComponent( (BaseAddon) this, 1822, 7, 8, 0, 1367, -1, "", 1);// 419
			AddComplexComponent( (BaseAddon) this, 1822, 6, 8, 0, 1367, -1, "", 1);// 420
			AddComplexComponent( (BaseAddon) this, 1822, 5, 8, 0, 1367, -1, "", 1);// 421
			AddComplexComponent( (BaseAddon) this, 1822, 4, 8, 0, 1367, -1, "", 1);// 422
			AddComplexComponent( (BaseAddon) this, 1822, 3, 8, 0, 1367, -1, "", 1);// 423
			AddComplexComponent( (BaseAddon) this, 1822, 2, 8, 0, 1367, -1, "", 1);// 424
			AddComplexComponent( (BaseAddon) this, 1822, 6, -2, 10, 1645, -1, "", 1);// 425
			AddComplexComponent( (BaseAddon) this, 1822, 3, 1, 5, 1367, -1, "", 1);// 426
			AddComplexComponent( (BaseAddon) this, 1822, 9, 4, 0, 1367, -1, "", 1);// 427
			AddComplexComponent( (BaseAddon) this, 1822, 9, 2, 0, 1367, -1, "", 1);// 428
			AddComplexComponent( (BaseAddon) this, 1822, 9, 1, 0, 1367, -1, "", 1);// 429
			AddComplexComponent( (BaseAddon) this, 1822, 9, 0, 0, 1645, -1, "", 1);// 430
			AddComplexComponent( (BaseAddon) this, 1822, 9, -1, 0, 1645, -1, "", 1);// 431
			AddComplexComponent( (BaseAddon) this, 1822, 9, -2, 0, 1645, -1, "", 1);// 432
			AddComplexComponent( (BaseAddon) this, 1822, 9, -3, 0, 1645, -1, "", 1);// 433
			AddComplexComponent( (BaseAddon) this, 1822, 4, -2, 10, 1645, -1, "", 1);// 434
			AddComplexComponent( (BaseAddon) this, 1822, 9, 6, 5, 1367, -1, "", 1);// 437
			AddComplexComponent( (BaseAddon) this, 1822, 4, -4, 10, 1645, -1, "", 1);// 438
			AddComplexComponent( (BaseAddon) this, 1822, 4, 3, 5, 1367, -1, "", 1);// 439
			AddComplexComponent( (BaseAddon) this, 1822, 4, 5, 10, 1367, -1, "", 1);// 440
			AddComplexComponent( (BaseAddon) this, 1822, 4, -2, 5, 1645, -1, "", 1);// 441
			AddComplexComponent( (BaseAddon) this, 1822, 2, -4, 5, 1645, -1, "", 1);// 442
			AddComplexComponent( (BaseAddon) this, 1822, 2, -3, 5, 1645, -1, "", 1);// 443
			AddComplexComponent( (BaseAddon) this, 1822, 2, 1, 5, 1109, -1, "", 1);// 445
			AddComplexComponent( (BaseAddon) this, 1822, 2, 4, 5, 1367, -1, "", 1);// 446
			AddComplexComponent( (BaseAddon) this, 1822, 2, 5, 5, 1367, -1, "", 1);// 447
			AddComplexComponent( (BaseAddon) this, 1822, 2, 6, 5, 1367, -1, "", 1);// 448
			AddComplexComponent( (BaseAddon) this, 1822, 5, 0, 5, 1645, -1, "", 1);// 449
			AddComplexComponent( (BaseAddon) this, 1822, 5, 1, 5, 1367, -1, "", 1);// 450
			AddComplexComponent( (BaseAddon) this, 1822, 6, 0, 5, 1645, -1, "", 1);// 451
			AddComplexComponent( (BaseAddon) this, 1822, 6, 1, 5, 1367, -1, "", 1);// 452
			AddComplexComponent( (BaseAddon) this, 1822, 7, 0, 5, 1645, -1, "", 1);// 453
			AddComplexComponent( (BaseAddon) this, 1822, 7, 1, 5, 1367, -1, "", 1);// 454
			AddComplexComponent( (BaseAddon) this, 1822, 9, -4, 5, 1645, -1, "", 1);// 456
			AddComplexComponent( (BaseAddon) this, 1822, 9, 3, 5, 1367, -1, "", 1);// 460
			AddComplexComponent( (BaseAddon) this, 1822, 9, 2, 5, 1367, -1, "", 1);// 462
			AddComplexComponent( (BaseAddon) this, 1822, 8, 8, 5, 1367, -1, "", 1);// 463
			AddComplexComponent( (BaseAddon) this, 1822, 8, 0, 5, 1645, -1, "", 1);// 464
			AddComplexComponent( (BaseAddon) this, 1822, 4, 4, 10, 1367, -1, "", 1);// 465
			AddComplexComponent( (BaseAddon) this, 1822, 4, -3, 5, 1645, -1, "", 1);// 466
			AddComplexComponent( (BaseAddon) this, 1822, 8, 1, 5, 1367, -1, "", 1);// 469
			AddComplexComponent( (BaseAddon) this, 1822, 6, 8, 5, 1367, -1, "", 1);// 471
			AddComplexComponent( (BaseAddon) this, 1822, 9, 6, 0, 1367, -1, "", 1);// 472
			AddComplexComponent( (BaseAddon) this, 1822, 7, 3, 5, 1367, -1, "", 1);// 473
			AddComplexComponent( (BaseAddon) this, 1822, 9, -4, 0, 1645, -1, "", 1);// 474
			AddComplexComponent( (BaseAddon) this, 1822, 9, -5, 0, 1645, -1, "", 1);// 475
			AddComplexComponent( (BaseAddon) this, 1822, 6, 3, 5, 1367, -1, "", 1);// 479
			AddComplexComponent( (BaseAddon) this, 1313, 2, 7, 5, 1109, -1, "", 1);// 482
			AddComplexComponent( (BaseAddon) this, 1822, 2, -1, 5, 1645, -1, "", 1);// 486
			AddComplexComponent( (BaseAddon) this, 1822, 11, 8, 0, 1367, -1, "", 1);// 514
			AddComplexComponent( (BaseAddon) this, 1822, 12, 8, 0, 1367, -1, "", 1);// 515
			AddComplexComponent( (BaseAddon) this, 1822, 13, 8, 0, 1367, -1, "", 1);// 516
			AddComplexComponent( (BaseAddon) this, 1822, 13, 3, 0, 1367, -1, "", 1);// 517
			AddComplexComponent( (BaseAddon) this, 1822, 13, 4, 0, 1367, -1, "", 1);// 518
			AddComplexComponent( (BaseAddon) this, 1822, 13, 5, 0, 1367, -1, "", 1);// 519
			AddComplexComponent( (BaseAddon) this, 1822, 13, 6, 0, 1367, -1, "", 1);// 520
			AddComplexComponent( (BaseAddon) this, 1822, 13, 7, 0, 1367, -1, "", 1);// 521

		}

		public minictfarenaAddon( Serial serial ) : base( serial )
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

	public class minictfarenaAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new minictfarenaAddon();
			}
		}

		[Constructable]
		public minictfarenaAddonDeed()
		{
			Name = "minictfarena";
		}

		public minictfarenaAddonDeed( Serial serial ) : base( serial )
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