
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class MiniCTFAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {12788, 0, -10, 0}, {1313, -8, -6, 5}, {12788, -2, -9, 0}// 1	2	3	
			, {1313, 0, -6, 5}, {12788, -1, -11, 0}, {12788, -3, -9, 0}// 5	10	11	
			, {1313, -5, -6, 5}, {12788, -1, -9, 0}, {3206, 0, -6, 10}// 12	13	15	
			, {1313, -1, -6, 5}, {12788, -3, -11, 0}, {12788, 0, -8, 0}// 17	18	19	
			, {12788, -3, -8, 0}, {12788, -1, -8, 0}, {12788, -2, -10, 0}// 20	21	22	
			, {12788, -3, -10, 0}, {12788, -2, -11, 0}, {1313, -6, -6, 5}// 23	24	27	
			, {1313, -10, -6, 5}, {12788, 0, -9, 0}, {1313, -3, -6, 5}// 28	30	31	
			, {1313, -4, -6, 5}, {12788, -2, -8, 0}, {12788, 0, -11, 0}// 32	33	35	
			, {12788, -1, -10, 0}, {1313, -2, -6, 5}, {1313, -9, -6, 5}// 36	37	38	
			, {1313, -1, -4, 5}, {1313, -5, 2, 5}, {1313, -2, 2, 5}// 44	46	47	
			, {1313, -4, -2, 5}, {1313, -2, -2, 5}, {1313, -5, 5, 5}// 48	50	51	
			, {1313, -10, -3, 5}, {3203, -6, 1, 10}, {1313, -2, 4, 5}// 53	57	60	
			, {3203, 0, 5, 10}, {1313, -6, -5, 5}, {3206, 0, 6, 10}// 61	62	64	
			, {1313, -1, 6, 5}, {1313, -1, -2, 5}, {1313, -1, 4, 5}// 65	66	67	
			, {3203, -5, 0, 10}, {1313, -6, 1, 5}, {1313, -6, 7, 5}// 68	71	72	
			, {1313, -6, 6, 5}, {1313, -6, 4, 5}, {1313, -5, -1, 5}// 73	75	76	
			, {1313, -9, -5, 5}, {3206, -3, 1, 10}, {1313, -4, -4, 5}// 77	78	81	
			, {1313, -4, -5, 5}, {1313, -4, -1, 5}, {1313, -5, 4, 5}// 82	83	84	
			, {1313, -6, 2, 5}, {1313, -3, 2, 5}, {1313, -2, -4, 5}// 99	101	102	
			, {1313, -2, -3, 5}, {1313, -3, 10, 5}, {1313, -3, 9, 5}// 103	104	105	
			, {3206, 0, -4, 10}, {1313, -1, 2, 5}, {3203, 0, -5, 10}// 108	110	114	
			, {1313, -2, 6, 5}, {3206, -6, 0, 10}, {3206, 0, 4, 10}// 115	116	118	
			, {1313, -1, -3, 5}, {1313, -5, 6, 5}, {1313, -1, 3, 5}// 122	127	129	
			, {1313, -6, 9, 5}, {3203, 0, 2, 10}, {1313, -8, -3, 5}// 131	132	140	
			, {1313, -4, 2, 5}, {1313, -4, 3, 5}, {1313, -4, 4, 5}// 141	142	143	
			, {1313, -6, 10, 5}, {1313, -4, 5, 5}, {1313, -4, 6, 5}// 144	145	146	
			, {1313, -1, 5, 5}, {1313, -4, 7, 5}, {1313, -5, 10, 5}// 147	148	149	
			, {1313, -2, 0, 5}, {1313, -3, -5, 5}, {1313, -3, -4, 5}// 150	151	152	
			, {3206, -5, 1, 10}, {1313, -3, -3, 5}, {1313, -6, -4, 5}// 153	154	155	
			, {1313, -5, 3, 5}, {1313, -3, 3, 5}, {1313, -3, 4, 5}// 156	157	158	
			, {1313, -3, 5, 5}, {3206, -1, 1, 10}, {1313, -8, -5, 5}// 159	160	164	
			, {3203, -1, 0, 10}, {1313, -1, 7, 5}, {3203, -3, 0, 10}// 167	170	171	
			, {1313, -4, 10, 5}, {6039, 0, 1, 10}, {1313, -5, -3, 5}// 174	175	176	
			, {3206, -4, 0, 10}, {1313, -6, -3, 5}, {1313, -5, -4, 5}// 177	179	186	
			, {1313, -2, 7, 5}, {1313, -10, -5, 5}, {1313, -1, -5, 5}// 187	188	196	
			, {1313, -1, -1, 5}, {1313, -6, -2, 5}, {3203, -4, 1, 10}// 197	198	199	
			, {1313, -3, 7, 5}, {3206, 0, -1, 10}, {1313, -4, -3, 5}// 200	201	202	
			, {1313, -2, -1, 5}, {1313, -3, -2, 5}, {1313, -3, -1, 5}// 203	204	205	
			, {1313, -2, -5, 5}, {1313, -3, 6, 5}, {1313, -9, -3, 5}// 207	208	209	
			, {3203, 0, -3, 10}, {1313, 0, -2, 5}, {1313, -2, 3, 5}// 210	211	212	
			, {1313, -5, -2, 5}, {1313, -9, -4, 5}, {1313, -4, 9, 5}// 213	214	215	
			, {6039, 0, 0, 10}, {1313, -2, 1, 5}, {1313, -8, -4, 5}// 217	218	221	
			, {3203, 0, 7, 10}, {1313, -6, 3, 5}, {1313, -6, 5, 5}// 222	223	227	
			, {1313, -5, 7, 5}, {1313, -5, -5, 5}, {1313, -10, -4, 5}// 232	233	234	
			, {1313, 0, 7, 5}, {1313, -6, 0, 5}, {1313, -6, -1, 5}// 236	237	238	
			, {1313, -2, 5, 5}, {1313, 0, 3, 5}, {1313, -5, 11, 5}// 239	241	252	
			, {1313, -6, 11, 5}, {1313, -3, 11, 5}, {1313, -4, 11, 5}// 255	256	259	
			, {12788, 1, -10, 0}, {3203, 1, -6, 10}, {1313, 5, -6, 5}// 283	285	287	
			, {1313, 6, -6, 5}, {1313, 7, -6, 5}, {12788, 1, -8, 0}// 288	289	290	
			, {12788, 1, -9, 0}, {1313, 4, -9, 5}, {1313, 4, -10, 5}// 291	292	296	
			, {1313, 4, -8, 5}, {1313, 5, -10, 5}, {1313, 5, -9, 5}// 298	299	300	
			, {1313, 5, -8, 5}, {1313, 6, -10, 5}, {1313, 6, -9, 5}// 301	302	303	
			, {1313, 7, -10, 5}, {1313, 7, -9, 5}, {1313, 7, -8, 5}// 305	306	307	
			, {12788, 1, -11, 0}, {1313, 1, -6, 5}, {1313, 2, -6, 5}// 308	310	311	
			, {1313, 3, -6, 5}, {1313, 4, -6, 5}, {3203, 5, 0, 10}// 312	313	315	
			, {1313, 2, -2, 5}, {3203, 1, 4, 10}, {6039, 1, 1, 10}// 316	318	319	
			, {3206, 5, 1, 10}, {1313, 10, 4, 5}, {1313, 1, 3, 5}// 322	323	325	
			, {1313, 3, 1, 5}, {3206, 1, 7, 10}, {3206, 2, 0, 10}// 336	351	352	
			, {3203, 1, -1, 10}, {3206, 1, 5, 10}, {3203, 7, 0, 10}// 354	355	361	
			, {3206, 4, 0, 10}, {3203, 1, 6, 10}, {1313, 9, 6, 5}// 364	366	367	
			, {1313, 5, -5, 5}, {1313, 5, -4, 5}, {1313, 5, -3, 5}// 368	369	370	
			, {1313, 5, -2, 5}, {1313, 5, -1, 5}, {1313, 5, 2, 5}// 371	372	373	
			, {1313, 5, 3, 5}, {1313, 5, 4, 5}, {1313, 5, 5, 5}// 374	375	376	
			, {1313, 5, 6, 5}, {1313, 5, 7, 5}, {1313, 6, -5, 5}// 377	378	379	
			, {1313, 6, -4, 5}, {1313, 6, -3, 5}, {1313, 6, -2, 5}// 380	381	382	
			, {1313, 6, -1, 5}, {1313, 6, 2, 5}, {1313, 6, 3, 5}// 383	384	385	
			, {1313, 6, 4, 5}, {1313, 6, 5, 5}, {1313, 6, 6, 5}// 386	387	388	
			, {1313, 6, 7, 5}, {1313, 7, -5, 5}, {1313, 7, -4, 5}// 389	390	391	
			, {1313, 7, -3, 5}, {1313, 7, -2, 5}, {1313, 7, -1, 5}// 392	393	394	
			, {1313, 7, 0, 5}, {1313, 7, 1, 5}, {1313, 7, 2, 5}// 395	396	397	
			, {1313, 7, 3, 5}, {1313, 7, 4, 5}, {1313, 7, 5, 5}// 398	399	400	
			, {1313, 7, 6, 5}, {1313, 7, 7, 5}, {1313, 9, 7, 5}// 401	402	410	
			, {1313, 10, 5, 5}, {3203, 4, 1, 10}, {1313, 3, 0, 5}// 411	413	414	
			, {1313, 9, 5, 5}, {1313, 2, -1, 5}, {3206, 7, 1, 10}// 415	418	422	
			, {3206, 1, -3, 10}, {1313, 10, 7, 5}, {3206, 1, -5, 10}// 423	442	443	
			, {3203, 2, 1, 10}, {1313, 11, 5, 5}, {6039, 1, 0, 10}// 451	462	464	
			, {1313, 11, 6, 5}, {1313, 11, 7, 5}, {1313, 10, 6, 5}// 465	466	468	
			, {3206, 6, 0, 10}, {1313, 11, 4, 5}, {1313, 9, 4, 5}// 474	475	477	
			, {1313, 1, -2, 5}, {3206, 1, 2, 10}, {1313, 2, 2, 5}// 483	484	485	
			, {3203, 6, 1, 10}, {3203, 1, -4, 10}, {1313, 2, -5, 5}// 487	488	490	
			, {1313, 2, -4, 5}, {1313, 2, -3, 5}, {1313, 2, 3, 5}// 491	492	494	
			, {1313, 2, 4, 5}, {1313, 2, 5, 5}, {1313, 2, 6, 5}// 495	496	497	
			, {1313, 2, 7, 5}, {1313, 3, -5, 5}, {1313, 3, -4, 5}// 498	499	500	
			, {1313, 3, -3, 5}, {1313, 3, -2, 5}, {1313, 3, -1, 5}// 501	502	503	
			, {1313, 3, 2, 5}, {1313, 3, 3, 5}, {1313, 3, 4, 5}// 504	505	506	
			, {1313, 3, 5, 5}, {1313, 3, 6, 5}, {1313, 3, 7, 5}// 507	508	509	
			, {1313, 4, -5, 5}, {1313, 4, -4, 5}, {1313, 4, -3, 5}// 510	511	512	
			, {1313, 4, -2, 5}, {1313, 4, -1, 5}, {1313, 4, 2, 5}// 513	514	515	
			, {1313, 4, 3, 5}, {1313, 4, 4, 5}, {1313, 4, 5, 5}// 516	517	518	
			, {1313, 4, 6, 5}, {1313, 4, 7, 5}// 519	520	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new MiniCTFAddonDeed();
			}
		}

		[ Constructable ]
		public MiniCTFAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 1822, -7, -7, 5, 1365, -1, "", 1);// 4
			AddComplexComponent( (BaseAddon) this, 1822, 0, -7, 5, 1365, -1, "", 1);// 6
			AddComplexComponent( (BaseAddon) this, 1822, -3, -7, 5, 1365, -1, "", 1);// 7
			AddComplexComponent( (BaseAddon) this, 1822, -2, -7, 5, 1365, -1, "", 1);// 8
			AddComplexComponent( (BaseAddon) this, 1822, -11, -6, 5, 1365, -1, "", 1);// 9
			AddComplexComponent( (BaseAddon) this, 1822, -10, -7, 5, 1365, -1, "", 1);// 14
			AddComplexComponent( (BaseAddon) this, 1822, -6, -7, 5, 1365, -1, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 1822, -11, -7, 5, 1365, -1, "", 1);// 25
			AddComplexComponent( (BaseAddon) this, 1822, -8, -7, 5, 1365, -1, "", 1);// 26
			AddComplexComponent( (BaseAddon) this, 1822, -7, -6, 5, 1365, -1, "", 1);// 29
			AddComplexComponent( (BaseAddon) this, 1822, 0, -6, 5, 1365, -1, "", 1);// 34
			AddComplexComponent( (BaseAddon) this, 1822, -9, -7, 5, 1365, -1, "", 1);// 39
			AddComplexComponent( (BaseAddon) this, 1822, -4, -7, 5, 1365, -1, "", 1);// 40
			AddComplexComponent( (BaseAddon) this, 1822, -5, -7, 5, 1365, -1, "", 1);// 41
			AddComplexComponent( (BaseAddon) this, 1822, -1, -7, 5, 1365, -1, "", 1);// 42
			AddComplexComponent( (BaseAddon) this, 1822, -5, -2, 5, 1365, -1, "", 1);// 43
			AddComplexComponent( (BaseAddon) this, 1822, -6, 1, 5, 1109, -1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 1822, -2, 9, 5, 1109, -1, "", 1);// 49
			AddComplexComponent( (BaseAddon) this, 1822, -6, 0, 5, 1365, -1, "", 1);// 52
			AddComplexComponent( (BaseAddon) this, 1822, -2, -4, 5, 1365, -1, "", 1);// 54
			AddComplexComponent( (BaseAddon) this, 1822, -3, 3, 5, 1109, -1, "", 1);// 55
			AddComplexComponent( (BaseAddon) this, 1822, 0, 8, 5, 1109, -1, "", 1);// 56
			AddComplexComponent( (BaseAddon) this, 1822, -2, 10, 5, 1109, -1, "", 1);// 58
			AddComplexComponent( (BaseAddon) this, 1822, -6, 8, 0, 1109, -1, "", 1);// 59
			AddComplexComponent( (BaseAddon) this, 1822, -5, 0, 5, 1365, -1, "", 1);// 63
			AddComplexComponent( (BaseAddon) this, 1822, -5, 3, 5, 1109, -1, "", 1);// 69
			AddComplexComponent( (BaseAddon) this, 1822, -2, -3, 5, 1365, -1, "", 1);// 70
			AddComplexComponent( (BaseAddon) this, 1822, -3, -2, 5, 1365, -1, "", 1);// 74
			AddComplexComponent( (BaseAddon) this, 1822, -7, -4, 5, 1365, -1, "", 1);// 79
			AddComplexComponent( (BaseAddon) this, 1822, -7, 9, 5, 1109, -1, "", 1);// 80
			AddComplexComponent( (BaseAddon) this, 1822, -7, 10, 5, 1109, -1, "", 1);// 85
			AddComplexComponent( (BaseAddon) this, 1822, -10, -2, 5, 1365, -1, "", 1);// 86
			AddComplexComponent( (BaseAddon) this, 1822, -11, -3, 5, 1365, -1, "", 1);// 87
			AddComplexComponent( (BaseAddon) this, 1822, -11, -2, 5, 1365, -1, "", 1);// 88
			AddComplexComponent( (BaseAddon) this, 1822, -9, -2, 5, 1365, -1, "", 1);// 89
			AddComplexComponent( (BaseAddon) this, 1822, -8, -2, 5, 1365, -1, "", 1);// 90
			AddComplexComponent( (BaseAddon) this, 1822, -10, -2, 0, 1365, -1, "", 1);// 91
			AddComplexComponent( (BaseAddon) this, 1822, -2, 10, 0, 1109, -1, "", 1);// 92
			AddComplexComponent( (BaseAddon) this, 1822, -8, -2, 0, 1365, -1, "", 1);// 93
			AddComplexComponent( (BaseAddon) this, 1822, -2, 9, 0, 1109, -1, "", 1);// 94
			AddComplexComponent( (BaseAddon) this, 1822, -9, -2, 0, 1365, -1, "", 1);// 95
			AddComplexComponent( (BaseAddon) this, 1822, 0, -5, 5, 1365, -1, "", 1);// 96
			AddComplexComponent( (BaseAddon) this, 1822, -2, -3, 10, 1365, -1, "", 1);// 97
			AddComplexComponent( (BaseAddon) this, 1822, 0, 7, 5, 1109, -1, "", 1);// 98
			AddComplexComponent( (BaseAddon) this, 1822, -3, 3, 10, 1109, -1, "", 1);// 100
			AddComplexComponent( (BaseAddon) this, 1822, -2, 5, 10, 1109, -1, "", 1);// 106
			AddComplexComponent( (BaseAddon) this, 1822, -1, 1, 5, 1109, -1, "", 1);// 107
			AddComplexComponent( (BaseAddon) this, 1822, -3, -2, 10, 1365, -1, "", 1);// 109
			AddComplexComponent( (BaseAddon) this, 1822, 0, 8, 0, 1109, -1, "", 1);// 111
			AddComplexComponent( (BaseAddon) this, 1822, -7, -1, 5, 1365, -1, "", 1);// 112
			AddComplexComponent( (BaseAddon) this, 1822, -1, 8, 0, 1109, -1, "", 1);// 113
			AddComplexComponent( (BaseAddon) this, 1822, 0, -1, 5, 1365, -1, "", 1);// 117
			AddComplexComponent( (BaseAddon) this, 1822, -2, 8, 5, 1109, -1, "", 1);// 119
			AddComplexComponent( (BaseAddon) this, 1822, -7, 0, 5, 1365, -1, "", 1);// 120
			AddComplexComponent( (BaseAddon) this, 1822, -4, -2, 5, 1365, -1, "", 1);// 121
			AddComplexComponent( (BaseAddon) this, 1822, -5, 8, 5, 1109, -1, "", 1);// 123
			AddComplexComponent( (BaseAddon) this, 1822, -6, 8, 5, 1109, -1, "", 1);// 124
			AddComplexComponent( (BaseAddon) this, 1822, -7, 1, 5, 1109, -1, "", 1);// 125
			AddComplexComponent( (BaseAddon) this, 1822, -7, 4, 5, 1109, -1, "", 1);// 126
			AddComplexComponent( (BaseAddon) this, 1822, -2, 3, 10, 1109, -1, "", 1);// 128
			AddComplexComponent( (BaseAddon) this, 1822, -4, 3, 5, 1109, -1, "", 1);// 130
			AddComplexComponent( (BaseAddon) this, 1822, -7, 2, 5, 1109, -1, "", 1);// 133
			AddComplexComponent( (BaseAddon) this, 1822, -2, -2, 10, 1365, -1, "", 1);// 134
			AddComplexComponent( (BaseAddon) this, 1822, 0, 0, 5, 1365, -1, "", 1);// 135
			AddComplexComponent( (BaseAddon) this, 1822, 0, 1, 5, 1109, -1, "", 1);// 136
			AddComplexComponent( (BaseAddon) this, 1822, 0, 4, 5, 1109, -1, "", 1);// 137
			AddComplexComponent( (BaseAddon) this, 1822, 0, 5, 5, 1109, -1, "", 1);// 138
			AddComplexComponent( (BaseAddon) this, 1822, 0, 6, 5, 1109, -1, "", 1);// 139
			AddComplexComponent( (BaseAddon) this, 1822, -2, 4, 10, 1109, -1, "", 1);// 161
			AddComplexComponent( (BaseAddon) this, 1822, -7, -2, 5, 1365, -1, "", 1);// 162
			AddComplexComponent( (BaseAddon) this, 1313, -5, 9, 5, 1109, -1, "", 1);// 163
			AddComplexComponent( (BaseAddon) this, 1822, -2, 8, 0, 1109, -1, "", 1);// 165
			AddComplexComponent( (BaseAddon) this, 1822, -5, 8, 0, 1109, -1, "", 1);// 166
			AddComplexComponent( (BaseAddon) this, 1822, -1, 8, 5, 1109, -1, "", 1);// 168
			AddComplexComponent( (BaseAddon) this, 1822, -2, -4, 10, 1365, -1, "", 1);// 169
			AddComplexComponent( (BaseAddon) this, 1822, -3, 8, 5, 1109, -1, "", 1);// 172
			AddComplexComponent( (BaseAddon) this, 1822, -4, -2, 10, 1365, -1, "", 1);// 173
			AddComplexComponent( (BaseAddon) this, 1822, -7, -5, 5, 1365, -1, "", 1);// 178
			AddComplexComponent( (BaseAddon) this, 1822, -2, 3, 5, 1109, -1, "", 1);// 180
			AddComplexComponent( (BaseAddon) this, 1822, -2, 4, 5, 1109, -1, "", 1);// 181
			AddComplexComponent( (BaseAddon) this, 1822, -2, 5, 5, 1109, -1, "", 1);// 182
			AddComplexComponent( (BaseAddon) this, 1822, -2, 6, 5, 1109, -1, "", 1);// 183
			AddComplexComponent( (BaseAddon) this, 1822, -2, -5, 5, 1365, -1, "", 1);// 184
			AddComplexComponent( (BaseAddon) this, 1822, -7, -3, 5, 1365, -1, "", 1);// 185
			AddComplexComponent( (BaseAddon) this, 1822, -5, 1, 5, 1109, -1, "", 1);// 189
			AddComplexComponent( (BaseAddon) this, 1822, -4, 0, 5, 1365, -1, "", 1);// 190
			AddComplexComponent( (BaseAddon) this, 1822, -4, 1, 5, 1109, -1, "", 1);// 191
			AddComplexComponent( (BaseAddon) this, 1822, -3, 0, 5, 1365, -1, "", 1);// 192
			AddComplexComponent( (BaseAddon) this, 1822, -3, 1, 5, 1109, -1, "", 1);// 193
			AddComplexComponent( (BaseAddon) this, 1822, -7, 6, 5, 1109, -1, "", 1);// 194
			AddComplexComponent( (BaseAddon) this, 1822, -7, 7, 5, 1109, -1, "", 1);// 195
			AddComplexComponent( (BaseAddon) this, 1822, -4, 3, 10, 1109, -1, "", 1);// 206
			AddComplexComponent( (BaseAddon) this, 1822, -11, -5, 5, 1365, -1, "", 1);// 216
			AddComplexComponent( (BaseAddon) this, 1822, -3, 8, 0, 1109, -1, "", 1);// 219
			AddComplexComponent( (BaseAddon) this, 1822, -4, 8, 0, 1109, -1, "", 1);// 220
			AddComplexComponent( (BaseAddon) this, 1822, -7, 8, 5, 1109, -1, "", 1);// 224
			AddComplexComponent( (BaseAddon) this, 1822, -1, 0, 5, 1365, -1, "", 1);// 225
			AddComplexComponent( (BaseAddon) this, 1822, 0, 2, 5, 1109, -1, "", 1);// 226
			AddComplexComponent( (BaseAddon) this, 1822, -4, 8, 5, 1109, -1, "", 1);// 228
			AddComplexComponent( (BaseAddon) this, 1822, -2, -2, 5, 1365, -1, "", 1);// 229
			AddComplexComponent( (BaseAddon) this, 1822, -7, 5, 5, 1109, -1, "", 1);// 230
			AddComplexComponent( (BaseAddon) this, 1822, -11, -4, 5, 1365, -1, "", 1);// 231
			AddComplexComponent( (BaseAddon) this, 1822, -7, 3, 5, 1109, -1, "", 1);// 235
			AddComplexComponent( (BaseAddon) this, 1822, 0, -3, 5, 1365, -1, "", 1);// 240
			AddComplexComponent( (BaseAddon) this, 1822, 0, -4, 5, 1365, -1, "", 1);// 242
			AddComplexComponent( (BaseAddon) this, 1822, -11, -2, 0, 1365, -1, "", 1);// 243
			AddComplexComponent( (BaseAddon) this, 1822, -3, 12, 5, 1109, -1, "", 1);// 244
			AddComplexComponent( (BaseAddon) this, 1822, -5, 12, 0, 1109, -1, "", 1);// 245
			AddComplexComponent( (BaseAddon) this, 1822, -6, 12, 5, 1109, -1, "", 1);// 246
			AddComplexComponent( (BaseAddon) this, 1822, -2, 11, 0, 1109, -1, "", 1);// 247
			AddComplexComponent( (BaseAddon) this, 1822, -7, 12, 0, 1109, -1, "", 1);// 248
			AddComplexComponent( (BaseAddon) this, 1822, -4, 12, 0, 1109, -1, "", 1);// 249
			AddComplexComponent( (BaseAddon) this, 1822, -3, 12, 0, 1109, -1, "", 1);// 250
			AddComplexComponent( (BaseAddon) this, 1822, -2, 12, 0, 1109, -1, "", 1);// 251
			AddComplexComponent( (BaseAddon) this, 1822, -4, 12, 5, 1109, -1, "", 1);// 253
			AddComplexComponent( (BaseAddon) this, 1822, -2, 12, 5, 1109, -1, "", 1);// 254
			AddComplexComponent( (BaseAddon) this, 1822, -5, 12, 5, 1109, -1, "", 1);// 257
			AddComplexComponent( (BaseAddon) this, 1822, -6, 12, 0, 1109, -1, "", 1);// 258
			AddComplexComponent( (BaseAddon) this, 1822, -2, 11, 5, 1109, -1, "", 1);// 260
			AddComplexComponent( (BaseAddon) this, 1822, -7, 11, 5, 1109, -1, "", 1);// 261
			AddComplexComponent( (BaseAddon) this, 1822, -7, 12, 5, 1109, -1, "", 1);// 262
			AddComplexComponent( (BaseAddon) this, 1822, 1, -6, 5, 1645, -1, "", 1);// 263
			AddComplexComponent( (BaseAddon) this, 1822, 5, -7, 5, 1645, -1, "", 1);// 264
			AddComplexComponent( (BaseAddon) this, 1822, 6, -7, 5, 1645, -1, "", 1);// 265
			AddComplexComponent( (BaseAddon) this, 1822, 7, -7, 5, 1645, -1, "", 1);// 266
			AddComplexComponent( (BaseAddon) this, 1822, 6, -11, 5, 1645, -1, "", 1);// 267
			AddComplexComponent( (BaseAddon) this, 1822, 2, -7, 5, 1645, -1, "", 1);// 268
			AddComplexComponent( (BaseAddon) this, 1822, 3, -7, 5, 1645, -1, "", 1);// 269
			AddComplexComponent( (BaseAddon) this, 1822, 4, -7, 5, 1645, -1, "", 1);// 270
			AddComplexComponent( (BaseAddon) this, 1822, 4, -11, 5, 1645, -1, "", 1);// 271
			AddComplexComponent( (BaseAddon) this, 1822, 3, -10, 5, 1645, -1, "", 1);// 272
			AddComplexComponent( (BaseAddon) this, 1822, 3, -9, 5, 1645, -1, "", 1);// 273
			AddComplexComponent( (BaseAddon) this, 1822, 3, -8, 5, 1645, -1, "", 1);// 274
			AddComplexComponent( (BaseAddon) this, 1822, 8, -10, 0, 1645, -1, "", 1);// 275
			AddComplexComponent( (BaseAddon) this, 1822, 8, -11, 5, 1645, -1, "", 1);// 276
			AddComplexComponent( (BaseAddon) this, 1822, 8, -10, 5, 1645, -1, "", 1);// 277
			AddComplexComponent( (BaseAddon) this, 1822, 8, -9, 5, 1645, -1, "", 1);// 278
			AddComplexComponent( (BaseAddon) this, 1822, 8, -8, 5, 1645, -1, "", 1);// 279
			AddComplexComponent( (BaseAddon) this, 1822, 7, -11, 5, 1645, -1, "", 1);// 280
			AddComplexComponent( (BaseAddon) this, 1822, 8, -8, 0, 1645, -1, "", 1);// 281
			AddComplexComponent( (BaseAddon) this, 1822, 8, -9, 0, 1645, -1, "", 1);// 282
			AddComplexComponent( (BaseAddon) this, 1822, 5, -11, 5, 1645, -1, "", 1);// 284
			AddComplexComponent( (BaseAddon) this, 1822, 1, -7, 5, 1645, -1, "", 1);// 286
			AddComplexComponent( (BaseAddon) this, 1822, 3, -11, 5, 1645, -1, "", 1);// 293
			AddComplexComponent( (BaseAddon) this, 1822, 8, -6, 0, 1645, -1, "", 1);// 294
			AddComplexComponent( (BaseAddon) this, 1822, 8, -7, 5, 1645, -1, "", 1);// 295
			AddComplexComponent( (BaseAddon) this, 1822, 8, -6, 5, 1645, -1, "", 1);// 297
			AddComplexComponent( (BaseAddon) this, 1313, 6, -8, 5, 1645, -1, "", 1);// 304
			AddComplexComponent( (BaseAddon) this, 1822, 8, -7, 0, 1645, -1, "", 1);// 309
			AddComplexComponent( (BaseAddon) this, 1822, 8, -11, 0, 1645, -1, "", 1);// 314
			AddComplexComponent( (BaseAddon) this, 1822, 1, 8, 5, 1367, -1, "", 1);// 317
			AddComplexComponent( (BaseAddon) this, 1822, 1, 7, 5, 1367, -1, "", 1);// 320
			AddComplexComponent( (BaseAddon) this, 1822, 3, -4, 5, 1645, -1, "", 1);// 321
			AddComplexComponent( (BaseAddon) this, 1822, 2, 0, 5, 1645, -1, "", 1);// 324
			AddComplexComponent( (BaseAddon) this, 1822, 8, 8, 0, 1367, -1, "", 1);// 326
			AddComplexComponent( (BaseAddon) this, 1822, 1, 0, 5, 1645, -1, "", 1);// 327
			AddComplexComponent( (BaseAddon) this, 1822, 4, 3, 5, 1367, -1, "", 1);// 328
			AddComplexComponent( (BaseAddon) this, 1822, 8, 1, 5, 1367, -1, "", 1);// 329
			AddComplexComponent( (BaseAddon) this, 1822, 3, 3, 10, 1367, -1, "", 1);// 330
			AddComplexComponent( (BaseAddon) this, 1822, 8, -1, 5, 1645, -1, "", 1);// 331
			AddComplexComponent( (BaseAddon) this, 1822, 3, 4, 5, 1367, -1, "", 1);// 332
			AddComplexComponent( (BaseAddon) this, 1822, 6, -2, 5, 1645, -1, "", 1);// 333
			AddComplexComponent( (BaseAddon) this, 1822, 5, -2, 5, 1645, -1, "", 1);// 334
			AddComplexComponent( (BaseAddon) this, 1822, 5, 3, 10, 1367, -1, "", 1);// 335
			AddComplexComponent( (BaseAddon) this, 1822, 10, 3, 5, 1367, -1, "", 1);// 337
			AddComplexComponent( (BaseAddon) this, 1822, 12, 6, 5, 1367, -1, "", 1);// 338
			AddComplexComponent( (BaseAddon) this, 1822, 9, 8, 5, 1367, -1, "", 1);// 339
			AddComplexComponent( (BaseAddon) this, 1822, 12, 5, 5, 1367, -1, "", 1);// 340
			AddComplexComponent( (BaseAddon) this, 1822, 9, 3, 5, 1367, -1, "", 1);// 341
			AddComplexComponent( (BaseAddon) this, 1822, 10, 8, 5, 1367, -1, "", 1);// 342
			AddComplexComponent( (BaseAddon) this, 1822, 11, 8, 5, 1367, -1, "", 1);// 343
			AddComplexComponent( (BaseAddon) this, 1822, 12, 8, 5, 1367, -1, "", 1);// 344
			AddComplexComponent( (BaseAddon) this, 1822, 12, 4, 5, 1367, -1, "", 1);// 345
			AddComplexComponent( (BaseAddon) this, 1822, 11, 3, 5, 1367, -1, "", 1);// 346
			AddComplexComponent( (BaseAddon) this, 1822, 12, 3, 5, 1367, -1, "", 1);// 347
			AddComplexComponent( (BaseAddon) this, 1822, 12, 7, 5, 1367, -1, "", 1);// 348
			AddComplexComponent( (BaseAddon) this, 1822, 9, 8, 0, 1367, -1, "", 1);// 349
			AddComplexComponent( (BaseAddon) this, 1822, 8, 5, 5, 1367, -1, "", 1);// 350
			AddComplexComponent( (BaseAddon) this, 1822, 2, 8, 5, 1367, -1, "", 1);// 353
			AddComplexComponent( (BaseAddon) this, 1822, 8, 7, 0, 1367, -1, "", 1);// 356
			AddComplexComponent( (BaseAddon) this, 1822, 8, 4, 5, 1367, -1, "", 1);// 357
			AddComplexComponent( (BaseAddon) this, 1822, 8, 5, 0, 1367, -1, "", 1);// 358
			AddComplexComponent( (BaseAddon) this, 1822, 8, 3, 0, 1367, -1, "", 1);// 359
			AddComplexComponent( (BaseAddon) this, 1822, 8, -5, 5, 1645, -1, "", 1);// 360
			AddComplexComponent( (BaseAddon) this, 1822, 3, -3, 10, 1645, -1, "", 1);// 362
			AddComplexComponent( (BaseAddon) this, 1822, 3, 6, 5, 1367, -1, "", 1);// 363
			AddComplexComponent( (BaseAddon) this, 1822, 8, 7, 5, 1367, -1, "", 1);// 365
			AddComplexComponent( (BaseAddon) this, 1822, 8, 0, 5, 1645, -1, "", 1);// 403
			AddComplexComponent( (BaseAddon) this, 1822, 8, -2, 5, 1645, -1, "", 1);// 404
			AddComplexComponent( (BaseAddon) this, 1822, 1, -5, 5, 1645, -1, "", 1);// 405
			AddComplexComponent( (BaseAddon) this, 1822, 1, 2, 5, 1367, -1, "", 1);// 406
			AddComplexComponent( (BaseAddon) this, 1822, 8, 8, 5, 1367, -1, "", 1);// 407
			AddComplexComponent( (BaseAddon) this, 1822, 3, 5, 5, 1367, -1, "", 1);// 408
			AddComplexComponent( (BaseAddon) this, 1822, 4, -2, 5, 1645, -1, "", 1);// 409
			AddComplexComponent( (BaseAddon) this, 1822, 4, -2, 10, 1645, -1, "", 1);// 412
			AddComplexComponent( (BaseAddon) this, 1822, 6, 8, 5, 1367, -1, "", 1);// 416
			AddComplexComponent( (BaseAddon) this, 1822, 8, -3, 5, 1645, -1, "", 1);// 417
			AddComplexComponent( (BaseAddon) this, 1822, 4, 8, 5, 1367, -1, "", 1);// 419
			AddComplexComponent( (BaseAddon) this, 1822, 4, 3, 10, 1367, -1, "", 1);// 420
			AddComplexComponent( (BaseAddon) this, 1822, 3, 8, 5, 1367, -1, "", 1);// 421
			AddComplexComponent( (BaseAddon) this, 1822, 3, -5, 5, 1645, -1, "", 1);// 424
			AddComplexComponent( (BaseAddon) this, 1822, 7, 8, 0, 1367, -1, "", 1);// 425
			AddComplexComponent( (BaseAddon) this, 1822, 6, 8, 0, 1367, -1, "", 1);// 426
			AddComplexComponent( (BaseAddon) this, 1822, 5, 8, 0, 1367, -1, "", 1);// 427
			AddComplexComponent( (BaseAddon) this, 1822, 4, 8, 0, 1367, -1, "", 1);// 428
			AddComplexComponent( (BaseAddon) this, 1822, 3, 8, 0, 1367, -1, "", 1);// 429
			AddComplexComponent( (BaseAddon) this, 1822, 2, 8, 0, 1367, -1, "", 1);// 430
			AddComplexComponent( (BaseAddon) this, 1822, 1, 8, 0, 1367, -1, "", 1);// 431
			AddComplexComponent( (BaseAddon) this, 1822, 5, -2, 10, 1645, -1, "", 1);// 432
			AddComplexComponent( (BaseAddon) this, 1822, 2, 1, 5, 1367, -1, "", 1);// 433
			AddComplexComponent( (BaseAddon) this, 1822, 8, 4, 0, 1367, -1, "", 1);// 434
			AddComplexComponent( (BaseAddon) this, 1822, 8, 2, 0, 1367, -1, "", 1);// 435
			AddComplexComponent( (BaseAddon) this, 1822, 8, 1, 0, 1367, -1, "", 1);// 436
			AddComplexComponent( (BaseAddon) this, 1822, 8, 0, 0, 1645, -1, "", 1);// 437
			AddComplexComponent( (BaseAddon) this, 1822, 8, -1, 0, 1645, -1, "", 1);// 438
			AddComplexComponent( (BaseAddon) this, 1822, 8, -2, 0, 1645, -1, "", 1);// 439
			AddComplexComponent( (BaseAddon) this, 1822, 8, -3, 0, 1645, -1, "", 1);// 440
			AddComplexComponent( (BaseAddon) this, 1822, 3, -2, 10, 1645, -1, "", 1);// 441
			AddComplexComponent( (BaseAddon) this, 1822, 8, 6, 5, 1367, -1, "", 1);// 444
			AddComplexComponent( (BaseAddon) this, 1822, 3, -4, 10, 1645, -1, "", 1);// 445
			AddComplexComponent( (BaseAddon) this, 1822, 3, 3, 5, 1367, -1, "", 1);// 446
			AddComplexComponent( (BaseAddon) this, 1822, 3, 5, 10, 1367, -1, "", 1);// 447
			AddComplexComponent( (BaseAddon) this, 1822, 3, -2, 5, 1645, -1, "", 1);// 448
			AddComplexComponent( (BaseAddon) this, 1822, 1, -4, 5, 1645, -1, "", 1);// 449
			AddComplexComponent( (BaseAddon) this, 1822, 1, -3, 5, 1645, -1, "", 1);// 450
			AddComplexComponent( (BaseAddon) this, 1822, 1, 1, 5, 1109, -1, "", 1);// 452
			AddComplexComponent( (BaseAddon) this, 1822, 1, 4, 5, 1367, -1, "", 1);// 453
			AddComplexComponent( (BaseAddon) this, 1822, 1, 5, 5, 1367, -1, "", 1);// 454
			AddComplexComponent( (BaseAddon) this, 1822, 1, 6, 5, 1367, -1, "", 1);// 455
			AddComplexComponent( (BaseAddon) this, 1822, 4, 0, 5, 1645, -1, "", 1);// 456
			AddComplexComponent( (BaseAddon) this, 1822, 4, 1, 5, 1367, -1, "", 1);// 457
			AddComplexComponent( (BaseAddon) this, 1822, 5, 0, 5, 1645, -1, "", 1);// 458
			AddComplexComponent( (BaseAddon) this, 1822, 5, 1, 5, 1367, -1, "", 1);// 459
			AddComplexComponent( (BaseAddon) this, 1822, 6, 0, 5, 1645, -1, "", 1);// 460
			AddComplexComponent( (BaseAddon) this, 1822, 6, 1, 5, 1367, -1, "", 1);// 461
			AddComplexComponent( (BaseAddon) this, 1822, 8, -4, 5, 1645, -1, "", 1);// 463
			AddComplexComponent( (BaseAddon) this, 1822, 8, 3, 5, 1367, -1, "", 1);// 467
			AddComplexComponent( (BaseAddon) this, 1822, 8, 2, 5, 1367, -1, "", 1);// 469
			AddComplexComponent( (BaseAddon) this, 1822, 7, 8, 5, 1367, -1, "", 1);// 470
			AddComplexComponent( (BaseAddon) this, 1822, 7, 0, 5, 1645, -1, "", 1);// 471
			AddComplexComponent( (BaseAddon) this, 1822, 3, 4, 10, 1367, -1, "", 1);// 472
			AddComplexComponent( (BaseAddon) this, 1822, 3, -3, 5, 1645, -1, "", 1);// 473
			AddComplexComponent( (BaseAddon) this, 1822, 7, 1, 5, 1367, -1, "", 1);// 476
			AddComplexComponent( (BaseAddon) this, 1822, 5, 8, 5, 1367, -1, "", 1);// 478
			AddComplexComponent( (BaseAddon) this, 1822, 8, 6, 0, 1367, -1, "", 1);// 479
			AddComplexComponent( (BaseAddon) this, 1822, 6, 3, 5, 1367, -1, "", 1);// 480
			AddComplexComponent( (BaseAddon) this, 1822, 8, -4, 0, 1645, -1, "", 1);// 481
			AddComplexComponent( (BaseAddon) this, 1822, 8, -5, 0, 1645, -1, "", 1);// 482
			AddComplexComponent( (BaseAddon) this, 1822, 5, 3, 5, 1367, -1, "", 1);// 486
			AddComplexComponent( (BaseAddon) this, 1313, 1, 7, 5, 1109, -1, "", 1);// 489
			AddComplexComponent( (BaseAddon) this, 1822, 1, -1, 5, 1645, -1, "", 1);// 493
			AddComplexComponent( (BaseAddon) this, 1822, 10, 8, 0, 1367, -1, "", 1);// 521
			AddComplexComponent( (BaseAddon) this, 1822, 11, 8, 0, 1367, -1, "", 1);// 522
			AddComplexComponent( (BaseAddon) this, 1822, 12, 8, 0, 1367, -1, "", 1);// 523
			AddComplexComponent( (BaseAddon) this, 1822, 12, 3, 0, 1367, -1, "", 1);// 524
			AddComplexComponent( (BaseAddon) this, 1822, 12, 4, 0, 1367, -1, "", 1);// 525
			AddComplexComponent( (BaseAddon) this, 1822, 12, 5, 0, 1367, -1, "", 1);// 526
			AddComplexComponent( (BaseAddon) this, 1822, 12, 6, 0, 1367, -1, "", 1);// 527
			AddComplexComponent( (BaseAddon) this, 1822, 12, 7, 0, 1367, -1, "", 1);// 528

		}

		public MiniCTFAddon( Serial serial ) : base( serial )
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

	public class MiniCTFAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new MiniCTFAddon();
			}
		}

		[Constructable]
		public MiniCTFAddonDeed()
		{
			Name = "MiniCTF";
		}

		public MiniCTFAddonDeed( Serial serial ) : base( serial )
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