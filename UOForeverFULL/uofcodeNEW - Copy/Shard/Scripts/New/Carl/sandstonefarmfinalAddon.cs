
////////////////////////////////////////
//                                    //
//   Generated by CEO's YAAAG - V1.2  //
// (Yet Another Arya Addon Generator) //
//                                    //
////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class sandstonefarmfinalAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {514, -10, -6, 1}, {1476, -10, -6, 41}, {514, -10, -6, 21}// 1	2	3	
			, {518, -10, -5, 21}, {518, -10, -5, 1}, {1476, -10, -5, 41}// 4	5	6	
			, {516, -10, -4, 21}, {516, -10, -4, 1}, {1476, -10, -4, 41}// 7	8	9	
			, {516, -10, -3, 1}, {516, -10, -3, 21}, {1476, -10, -3, 41}// 10	11	12	
			, {516, -10, -2, 21}, {1476, -10, -2, 41}, {516, -10, -2, 1}// 13	14	15	
			, {516, -10, -1, 1}, {1476, -10, -1, 41}, {513, -10, -1, 21}// 17	18	19	
			, {1121, -10, 0, 1}, {1476, -10, 0, 41}, {518, -10, 0, 21}// 20	21	22	
			, {516, -10, 1, 21}, {516, -10, 1, 1}, {1476, -10, 1, 41}// 23	24	25	
			, {516, -10, 2, 21}, {516, -10, 2, 1}, {1476, -10, 2, 41}// 26	27	28	
			, {516, -10, 3, 1}, {516, -10, 3, 21}, {1476, -10, 3, 41}// 29	30	31	
			, {516, -10, 4, 21}, {516, -10, 4, 1}, {1476, -10, 4, 41}// 32	33	34	
			, {513, -10, 5, 1}, {1476, -10, 5, 41}, {513, -10, 5, 21}// 35	36	37	
			, {1476, -10, 6, 41}, {517, -9, -6, 21}, {517, -9, -6, 1}// 38	39	40	
			, {1476, -9, -6, 44}, {1181, -9, -5, 1}, {1900, -9, -5, 1}// 41	42	43	
			, {1900, -9, -5, 6}, {1900, -9, -5, 11}, {1900, -9, -5, 16}// 44	45	46	
			, {1476, -9, -5, 44}, {6416, -9, -4, 1}, {1476, -9, -4, 44}// 47	49	50	
			, {1181, -9, -4, 21}, {1181, -9, -4, 1}, {6424, -9, -3, 1}// 51	52	54	
			, {1476, -9, -3, 44}, {1181, -9, -3, 21}, {1181, -9, -3, 1}// 55	56	57	
			, {2461, -9, -3, 11}, {1476, -9, -2, 44}, {1181, -9, -2, 21}// 58	60	61	
			, {1181, -9, -2, 1}, {3130, -9, -2, 3}, {3133, -9, -2, 3}// 62	63	64	
			, {3135, -9, -2, 3}, {1476, -9, -1, 44}, {1181, -9, -1, 21}// 65	67	68	
			, {1181, -9, -1, 1}, {517, -9, -1, 21}, {1476, -9, 0, 44}// 69	70	71	
			, {1181, -9, 0, 21}, {1181, -9, 0, 1}, {4553, -9, 0, 21}// 72	73	74	
			, {6425, -9, 1, 1}, {1476, -9, 1, 44}, {1181, -9, 1, 21}// 75	77	78	
			, {1181, -9, 1, 1}, {2519, -9, 1, 9}, {2519, -9, 1, 10}// 79	80	81	
			, {2519, -9, 1, 11}, {6039, -9, 2, 6}, {1476, -9, 2, 44}// 82	84	86	
			, {1181, -9, 2, 21}, {1181, -9, 2, 1}, {11224, -9, 2, 21}// 87	88	89	
			, {2527, -9, 3, 10}, {4100, -9, 3, 16}, {6039, -9, 3, 6}// 90	91	92	
			, {1476, -9, 3, 44}, {1181, -9, 3, 21}, {1181, -9, 3, 1}// 95	96	97	
			, {11223, -9, 3, 21}, {6425, -9, 4, 0}, {1476, -9, 4, 44}// 98	99	100	
			, {1181, -9, 4, 21}, {1181, -9, 4, 1}, {2478, -9, 4, 10}// 101	102	103	
			, {517, -9, 5, 21}, {517, -9, 5, 1}, {1476, -9, 5, 44}// 104	105	106	
			, {523, -9, 5, 41}, {1181, -9, 5, 21}, {1181, -9, 5, 1}// 107	108	109	
			, {1476, -9, 6, 44}, {515, -8, -6, 21}, {515, -8, -6, 1}// 110	111	112	
			, {1476, -8, -6, 47}, {1181, -8, -5, 1}, {1902, -8, -5, 16}// 113	114	115	
			, {1900, -8, -5, 1}, {1900, -8, -5, 6}, {1900, -8, -5, 11}// 116	117	118	
			, {1476, -8, -5, 47}, {6426, -8, -4, 1}, {1476, -8, -4, 47}// 119	120	121	
			, {1181, -8, -4, 21}, {1181, -8, -4, 1}, {2233, -8, -4, 21}// 122	123	124	
			, {1476, -8, -3, 47}, {1181, -8, -3, 21}, {1181, -8, -3, 1}// 125	126	127	
			, {1476, -8, -2, 47}, {1181, -8, -2, 21}, {1181, -8, -2, 1}// 128	129	130	
			, {1476, -8, -1, 47}, {1181, -8, -1, 21}, {1181, -8, -1, 1}// 131	132	133	
			, {515, -8, -1, 21}, {1476, -8, 0, 47}, {1181, -8, 0, 21}// 134	135	136	
			, {1181, -8, 0, 1}, {2639, -8, 0, 21}, {1476, -8, 1, 47}// 137	138	139	
			, {1181, -8, 1, 21}, {1181, -8, 1, 1}, {1476, -8, 2, 47}// 140	141	142	
			, {1181, -8, 2, 21}, {1181, -8, 2, 1}, {1476, -8, 3, 47}// 143	144	145	
			, {1181, -8, 3, 21}, {1181, -8, 3, 1}, {2602, -8, 3, 21}// 146	147	148	
			, {1476, -8, 4, 47}, {1181, -8, 4, 22}, {1181, -8, 4, 1}// 149	150	151	
			, {515, -8, 5, 1}, {1476, -8, 5, 47}, {523, -8, 5, 41}// 152	153	154	
			, {1181, -8, 5, 21}, {1181, -8, 5, 1}, {515, -8, 5, 21}// 155	156	157	
			, {1476, -8, 6, 47}, {515, -7, -6, 21}, {515, -7, -6, 1}// 158	159	160	
			, {1476, -7, -6, 50}, {1181, -7, -5, 1}, {1902, -7, -5, 11}// 161	162	163	
			, {1900, -7, -5, 1}, {1900, -7, -5, 6}, {1476, -7, -5, 50}// 164	165	166	
			, {6426, -7, -4, 1}, {1476, -7, -4, 50}, {1181, -7, -4, 21}// 167	168	169	
			, {1181, -7, -4, 1}, {2590, -7, -4, 10}, {2231, -7, -4, 21}// 170	171	172	
			, {1476, -7, -3, 50}, {1181, -7, -3, 21}, {1181, -7, -3, 1}// 173	174	175	
			, {1476, -7, -2, 50}, {1181, -7, -2, 21}, {1181, -7, -2, 1}// 176	177	178	
			, {1476, -7, -1, 50}, {1181, -7, -1, 21}, {1181, -7, -1, 1}// 179	180	181	
			, {512, -7, -1, 21}, {1476, -7, 0, 50}, {1181, -7, 0, 21}// 182	183	184	
			, {1181, -7, 0, 1}, {1476, -7, 1, 50}, {1181, -7, 1, 21}// 185	186	187	
			, {1181, -7, 1, 1}, {7752, -7, 1, 21}, {1476, -7, 2, 50}// 188	189	190	
			, {1181, -7, 2, 21}, {1181, -7, 2, 1}, {7751, -7, 2, 21}// 191	192	193	
			, {1476, -7, 3, 50}, {1181, -7, 3, 21}, {1181, -7, 3, 1}// 194	195	196	
			, {7750, -7, 3, 21}, {1476, -7, 4, 50}, {1181, -7, 4, 21}// 197	198	199	
			, {1181, -7, 4, 1}, {1476, -7, 5, 50}, {523, -7, 5, 41}// 200	203	204	
			, {523, -7, 5, 44}, {1181, -7, 5, 21}, {1181, -7, 5, 1}// 205	206	207	
			, {1476, -7, 6, 50}, {515, -6, -6, 21}, {515, -6, -6, 1}// 208	209	210	
			, {1476, -6, -6, 53}, {1181, -6, -5, 1}, {1902, -6, -5, 6}// 211	212	213	
			, {1900, -6, -5, 1}, {1476, -6, -5, 53}, {6426, -6, -4, 1}// 214	215	216	
			, {1476, -6, -4, 53}, {1181, -6, -4, 21}, {1181, -6, -4, 1}// 217	218	219	
			, {5643, -6, -4, 10}, {2231, -6, -4, 21}, {1476, -6, -3, 53}// 220	221	222	
			, {1181, -6, -3, 21}, {1181, -6, -3, 1}, {1476, -6, -2, 53}// 223	224	225	
			, {1181, -6, -2, 21}, {1181, -6, -2, 1}, {1476, -6, -1, 53}// 226	227	228	
			, {1181, -6, -1, 21}, {1181, -6, -1, 1}, {1476, -6, 0, 53}// 229	230	231	
			, {1181, -6, 0, 21}, {1181, -6, 0, 1}, {1476, -6, 1, 53}// 232	233	235	
			, {1181, -6, 1, 21}, {1181, -6, 1, 1}, {7747, -6, 1, 21}// 236	237	239	
			, {1476, -6, 2, 53}, {1181, -6, 2, 21}, {1181, -6, 2, 1}// 240	241	242	
			, {7748, -6, 2, 21}, {1476, -6, 3, 53}, {1181, -6, 3, 21}// 243	244	245	
			, {1181, -6, 3, 1}, {7749, -6, 3, 21}, {1476, -6, 4, 53}// 246	247	248	
			, {1181, -6, 4, 21}, {1181, -6, 4, 1}, {1476, -6, 5, 53}// 249	250	252	
			, {523, -6, 5, 41}, {523, -6, 5, 41}, {523, -6, 5, 44}// 253	254	255	
			, {523, -6, 5, 46}, {1181, -6, 5, 21}, {1181, -6, 5, 1}// 256	257	258	
			, {515, -6, 5, 21}, {1476, -6, 6, 53}, {515, -5, -6, 21}// 259	260	261	
			, {515, -5, -6, 1}, {1476, -5, -6, 56}, {1181, -5, -5, 1}// 262	263	264	
			, {1902, -5, -5, 1}, {1476, -5, -5, 56}, {1476, -5, -4, 56}// 265	266	267	
			, {1181, -5, -4, 21}, {1181, -5, -4, 1}, {4154, -5, -4, 2}// 268	269	270	
			, {4153, -5, -4, 6}, {2231, -5, -4, 21}, {1476, -5, -3, 56}// 271	272	273	
			, {1181, -5, -3, 21}, {1181, -5, -3, 1}, {1476, -5, -2, 56}// 274	275	276	
			, {1181, -5, -2, 21}, {1181, -5, -2, 1}, {1476, -5, -1, 56}// 277	278	279	
			, {1181, -5, -1, 21}, {1181, -5, -1, 1}, {2931, -5, -1, 1}// 280	281	282	
			, {517, -5, -1, 21}, {1476, -5, 0, 56}, {1181, -5, 0, 21}// 283	284	285	
			, {1181, -5, 0, 1}, {2931, -5, 0, 1}, {1476, -5, 1, 56}// 286	287	288	
			, {1181, -5, 1, 21}, {1181, -5, 1, 1}, {2931, -5, 1, 1}// 289	290	291	
			, {7746, -5, 1, 21}, {1476, -5, 2, 56}, {1181, -5, 2, 21}// 292	293	294	
			, {1181, -5, 2, 1}, {2928, -5, 2, 1}, {7745, -5, 2, 21}// 295	296	297	
			, {1476, -5, 3, 56}, {1181, -5, 3, 21}, {1181, -5, 3, 1}// 298	299	300	
			, {7744, -5, 3, 21}, {1476, -5, 4, 56}, {1181, -5, 4, 21}// 301	302	303	
			, {1181, -5, 4, 1}, {515, -5, 5, 1}, {1476, -5, 5, 56}// 304	306	307	
			, {523, -5, 5, 41}, {523, -5, 5, 44}, {523, -5, 5, 46}// 308	309	310	
			, {1181, -5, 5, 21}, {1181, -5, 5, 1}, {1476, -5, 6, 56}// 311	312	313	
			, {515, -4, -6, 21}, {515, -4, -6, 1}, {1475, -4, -6, 56}// 314	315	316	
			, {1181, -4, -5, 1}, {1475, -4, -5, 56}, {1181, -4, -5, 21}// 317	318	319	
			, {2232, -4, -5, 21}, {1475, -4, -4, 56}, {1181, -4, -4, 21}// 320	321	322	
			, {1181, -4, -4, 1}, {2230, -4, -4, 21}, {1475, -4, -3, 56}// 323	324	325	
			, {1181, -4, -3, 21}, {1181, -4, -3, 1}, {1475, -4, -2, 56}// 326	327	328	
			, {1181, -4, -2, 21}, {1181, -4, -2, 1}, {1475, -4, -1, 56}// 329	330	332	
			, {1181, -4, -1, 21}, {1181, -4, -1, 1}, {2930, -4, -1, 1}// 333	334	335	
			, {515, -4, -1, 21}, {1475, -4, 0, 56}, {1181, -4, 0, 21}// 336	337	338	
			, {1181, -4, 0, 1}, {2931, -4, 0, 1}, {2604, -4, 0, 21}// 339	340	341	
			, {1475, -4, 1, 56}, {1181, -4, 1, 21}, {1181, -4, 1, 1}// 342	343	344	
			, {2931, -4, 1, 1}, {1475, -4, 2, 56}, {1181, -4, 2, 21}// 345	347	348	
			, {1181, -4, 2, 1}, {2929, -4, 2, 1}, {2451, -4, 2, 12}// 349	350	351	
			, {1475, -4, 3, 56}, {1181, -4, 3, 21}, {1181, -4, 3, 1}// 352	353	354	
			, {1475, -4, 4, 56}, {1181, -4, 4, 21}, {1181, -4, 4, 1}// 356	357	358	
			, {512, -4, 5, 1}, {1475, -4, 5, 56}, {523, -4, 5, 41}// 359	360	361	
			, {523, -4, 5, 44}, {1181, -4, 5, 21}, {1181, -4, 5, 1}// 362	363	364	
			, {515, -4, 5, 21}, {518, -4, 6, 1}, {1475, -4, 6, 56}// 365	366	367	
			, {2148, -4, 6, 21}, {515, -3, -6, 21}, {1475, -3, -6, 53}// 368	369	371	
			, {1181, -3, -5, 1}, {1475, -3, -5, 53}, {1181, -3, -5, 21}// 372	373	374	
			, {1475, -3, -4, 53}, {1181, -3, -4, 21}, {1181, -3, -4, 1}// 375	376	377	
			, {1475, -3, -3, 53}, {1181, -3, -3, 21}, {1181, -3, -3, 1}// 378	379	380	
			, {1475, -3, -2, 53}, {1181, -3, -2, 21}, {1181, -3, -2, 1}// 381	382	383	
			, {1475, -3, -1, 53}, {1181, -3, -1, 21}, {1181, -3, -1, 1}// 384	385	386	
			, {515, -3, -1, 21}, {1475, -3, 0, 53}, {1181, -3, 0, 21}// 387	388	389	
			, {1181, -3, 0, 1}, {2691, -3, 0, 21}, {1475, -3, 1, 53}// 390	392	393	
			, {1181, -3, 1, 21}, {1181, -3, 1, 1}, {2687, -3, 1, 21}// 394	395	397	
			, {1475, -3, 2, 53}, {1181, -3, 2, 21}, {1181, -3, 2, 1}// 398	399	400	
			, {1475, -3, 3, 53}, {1181, -3, 3, 21}, {1181, -3, 3, 1}// 401	402	403	
			, {1475, -3, 4, 53}, {1181, -3, 4, 21}, {1181, -3, 4, 1}// 404	405	406	
			, {1475, -3, 5, 53}, {523, -3, 5, 41}, {1181, -3, 5, 21}// 407	408	409	
			, {1181, -3, 5, 1}, {512, -3, 5, 21}, {1475, -3, 6, 53}// 410	411	412	
			, {1181, -3, 6, 21}, {1181, -3, 6, 1}, {4497, -3, 6, 1}// 413	414	415	
			, {3205, -3, 6, 15}, {515, -2, -6, 21}, {515, -2, -6, 1}// 417	418	419	
			, {1475, -2, -6, 50}, {1181, -2, -5, 1}, {1475, -2, -5, 50}// 420	421	422	
			, {1181, -2, -5, 21}, {3743, -2, -5, 21}, {1475, -2, -4, 50}// 423	424	425	
			, {1181, -2, -4, 21}, {1181, -2, -4, 1}, {1475, -2, -3, 50}// 426	427	428	
			, {1181, -2, -3, 21}, {1181, -2, -3, 1}, {1475, -2, -2, 50}// 429	430	431	
			, {1181, -2, -2, 21}, {1181, -2, -2, 1}, {1475, -2, -1, 50}// 432	433	434	
			, {1181, -2, -1, 21}, {1181, -2, -1, 1}, {515, -2, -1, 21}// 435	436	437	
			, {1475, -2, 0, 50}, {1181, -2, 0, 21}, {1181, -2, 0, 1}// 438	439	440	
			, {2690, -2, 0, 21}, {1475, -2, 1, 50}, {1181, -2, 1, 21}// 441	442	443	
			, {1181, -2, 1, 1}, {2686, -2, 1, 21}, {1475, -2, 2, 50}// 444	445	446	
			, {1181, -2, 2, 21}, {1181, -2, 2, 1}, {1475, -2, 3, 50}// 447	448	449	
			, {1181, -2, 3, 21}, {1181, -2, 3, 1}, {1475, -2, 4, 50}// 450	451	452	
			, {1181, -2, 4, 21}, {1181, -2, 4, 1}, {1475, -2, 5, 50}// 453	454	455	
			, {523, -2, 5, 41}, {1181, -2, 5, 21}, {1181, -2, 5, 1}// 456	457	458	
			, {1475, -2, 6, 50}, {1181, -2, 6, 21}, {1181, -2, 6, 1}// 459	460	461	
			, {515, -1, -6, 21}, {515, -1, -6, 1}, {1475, -1, -6, 47}// 462	463	464	
			, {1181, -1, -5, 1}, {1475, -1, -5, 47}, {1181, -1, -5, 21}// 465	466	467	
			, {1475, -1, -4, 47}, {1181, -1, -4, 21}, {1181, -1, -4, 1}// 468	469	470	
			, {1475, -1, -3, 47}, {1181, -1, -3, 21}, {1181, -1, -3, 1}// 471	472	473	
			, {1475, -1, -2, 47}, {1181, -1, -2, 21}, {1181, -1, -2, 1}// 474	475	476	
			, {1475, -1, -1, 47}, {1181, -1, -1, 21}, {1181, -1, -1, 1}// 477	478	479	
			, {515, -1, -1, 21}, {1475, -1, 0, 47}, {1181, -1, 0, 21}// 480	481	482	
			, {1181, -1, 0, 1}, {1475, -1, 1, 47}, {1181, -1, 1, 21}// 483	484	485	
			, {1181, -1, 1, 1}, {1475, -1, 2, 47}, {1181, -1, 2, 21}// 486	487	488	
			, {1181, -1, 2, 1}, {1475, -1, 3, 47}, {1181, -1, 3, 21}// 489	490	491	
			, {1181, -1, 3, 1}, {1475, -1, 4, 47}, {1181, -1, 4, 21}// 492	493	494	
			, {1181, -1, 4, 1}, {1475, -1, 5, 47}, {1181, -1, 5, 21}// 495	496	497	
			, {1181, -1, 5, 1}, {517, -1, 5, 21}, {1475, -1, 6, 47}// 498	499	500	
			, {1181, -1, 6, 21}, {1181, -1, 6, 1}, {9, 0, -9, 1}// 501	502	503	
			, {2149, 0, -9, 21}, {11, 0, -8, 1}, {2148, 0, -8, 21}// 504	505	506	
			, {8, 0, -7, 1}, {2148, 0, -7, 21}, {512, 0, -6, 21}// 507	508	509	
			, {1475, 0, -6, 44}, {512, 0, -6, 1}, {13, 0, -6, 1}// 510	511	512	
			, {2148, 0, -6, 21}, {1181, 0, -5, 1}, {518, 0, -5, 21}// 513	514	515	
			, {518, 0, -5, 1}, {1475, 0, -5, 44}, {1181, 0, -5, 21}// 516	517	518	
			, {1475, 0, -4, 44}, {1181, 0, -4, 21}, {1181, 0, -4, 1}// 519	520	521	
			, {516, 0, -4, 1}, {513, 0, -4, 21}, {516, 0, -3, 1}// 522	523	524	
			, {1475, 0, -3, 44}, {1181, 0, -3, 21}, {1181, 0, -3, 1}// 525	526	527	
			, {514, 0, -2, 1}, {513, 0, -2, 1}, {1475, 0, -2, 44}// 528	529	530	
			, {1181, 0, -2, 21}, {1181, 0, -2, 1}, {518, 0, -2, 21}// 531	532	533	
			, {518, 0, -1, 1}, {1475, 0, -1, 44}, {1181, 0, -1, 21}// 534	535	536	
			, {1181, 0, -1, 1}, {511, 0, -1, 21}, {1475, 0, 0, 44}// 537	538	540	
			, {1181, 0, 0, 21}, {1181, 0, 0, 1}, {518, 0, 0, 21}// 541	542	543	
			, {516, 0, 1, 1}, {1475, 0, 1, 44}, {1181, 0, 1, 21}// 544	546	547	
			, {1181, 0, 1, 1}, {516, 0, 2, 21}, {1475, 0, 2, 44}// 548	549	551	
			, {1181, 0, 2, 21}, {1181, 0, 2, 1}, {513, 0, 3, 1}// 552	553	555	
			, {1475, 0, 3, 44}, {1181, 0, 3, 21}, {1181, 0, 3, 1}// 556	557	558	
			, {516, 0, 4, 21}, {518, 0, 4, 1}, {1475, 0, 4, 44}// 559	560	561	
			, {1181, 0, 4, 21}, {1181, 0, 4, 1}, {513, 0, 5, 1}// 562	563	564	
			, {1475, 0, 5, 44}, {1181, 0, 5, 21}, {1181, 0, 5, 1}// 565	566	567	
			, {511, 0, 5, 21}, {1475, 0, 6, 44}, {2148, 0, 6, 21}// 568	569	570	
			, {1181, 0, 6, 21}, {1181, 0, 6, 1}, {10, 1, -9, 1}// 571	572	573	
			, {2147, 1, -9, 21}, {12788, 1, -8, 1}, {7869, 1, -8, 1}// 574	575	576	
			, {1181, 1, -8, 21}, {3208, 1, -8, 30}, {12789, 1, -7, 1}// 577	579	580	
			, {1181, 1, -7, 21}, {1475, 1, -6, 41}, {12788, 1, -6, 1}// 581	582	583	
			, {1181, 1, -6, 21}, {1475, 1, -5, 41}, {12788, 1, -5, 1}// 584	585	586	
			, {2147, 1, -5, 1}, {1181, 1, -5, 21}, {1475, 1, -4, 41}// 587	588	589	
			, {12788, 1, -4, 1}, {1181, 1, -4, 21}, {1475, 1, -3, 41}// 590	591	592	
			, {12788, 1, -3, 1}, {1181, 1, -3, 21}, {1475, 1, -2, 41}// 593	594	595	
			, {12788, 1, -2, 1}, {10, 1, -2, 1}, {1181, 1, -2, 21}// 596	597	598	
			, {2147, 1, -2, 21}, {12788, 1, -1, 1}, {3374, 1, -1, 2}// 599	600	601	
			, {3375, 1, -1, 2}, {1475, 1, -1, 41}, {12788, 1, 0, 1}// 602	603	604	
			, {1475, 1, 0, 41}, {3203, 1, 0, 2}, {12788, 1, 1, 1}// 605	606	607	
			, {1475, 1, 1, 41}, {3220, 1, 1, 2}, {12788, 1, 2, 1}// 608	609	610	
			, {1475, 1, 2, 41}, {3231, 1, 2, 1}, {368, 1, 3, 1}// 611	612	613	
			, {12788, 1, 3, 1}, {4971, 1, 3, 1}, {1475, 1, 3, 41}// 614	615	616	
			, {3204, 1, 3, 2}, {12788, 1, 4, 1}, {1475, 1, 4, 41}// 617	618	619	
			, {3206, 1, 4, 1}, {1475, 1, 5, 41}, {1181, 1, 5, 1}// 620	621	622	
			, {1475, 1, 6, 41}, {1181, 1, 6, 1}, {7, 2, -9, 1}// 623	624	625	
			, {2147, 2, -9, 21}, {12788, 2, -8, 1}, {21, 2, -8, 6}// 626	627	628	
			, {21, 2, -8, 1}, {21, 2, -8, 11}, {1181, 2, -8, 21}// 629	630	631	
			, {7627, 2, -8, 21}, {2148, 2, -7, 1}, {12789, 2, -7, 1}// 632	633	634	
			, {1181, 2, -7, 21}, {12788, 2, -6, 1}, {1181, 2, -6, 21}// 635	636	637	
			, {12788, 2, -5, 1}, {2147, 2, -5, 1}, {2148, 2, -5, 1}// 638	639	640	
			, {1181, 2, -5, 21}, {12788, 2, -4, 1}, {2148, 2, -4, 1}// 641	642	643	
			, {1181, 2, -4, 21}, {12788, 2, -3, 1}, {1181, 2, -3, 21}// 644	645	646	
			, {7627, 2, -3, 21}, {12788, 2, -2, 1}, {7, 2, -2, 1}// 647	648	649	
			, {2148, 2, -2, 1}, {1181, 2, -2, 21}, {2147, 2, -2, 21}// 650	651	652	
			, {12788, 2, -1, 1}, {6001, 2, -1, 3}, {3309, 2, -1, 1}// 653	654	655	
			, {12788, 2, 0, 1}, {3235, 2, 0, 1}, {3206, 2, 0, 2}// 656	657	658	
			, {12788, 2, 1, 1}, {3241, 2, 1, 1}, {12788, 2, 2, 1}// 659	660	661	
			, {370, 2, 3, 1}, {12788, 2, 3, 1}, {3203, 2, 3, 2}// 662	663	664	
			, {1181, 2, 4, 1}, {12788, 2, 5, 1}, {12788, 2, 6, 1}// 665	666	667	
			, {7, 3, -9, 1}, {2147, 3, -9, 21}, {12788, 3, -8, 1}// 668	669	670	
			, {1193, 3, -8, 6}, {1196, 3, -8, 12}, {22, 3, -8, 1}// 671	672	673	
			, {6869, 3, -8, 6}, {2485, 3, -8, 14}, {1181, 3, -8, 21}// 674	675	676	
			, {7628, 3, -8, 21}, {12788, 3, -7, 1}, {1181, 3, -7, 21}// 677	678	679	
			, {12788, 3, -6, 1}, {1181, 3, -6, 21}, {12788, 3, -5, 1}// 680	681	682	
			, {1181, 3, -5, 21}, {12788, 3, -4, 1}, {1181, 3, -4, 21}// 683	684	685	
			, {12788, 3, -3, 1}, {1181, 3, -3, 21}, {7628, 3, -3, 21}// 686	687	688	
			, {12788, 3, -2, 1}, {7, 3, -2, 1}, {1181, 3, -2, 21}// 689	690	691	
			, {2147, 3, -2, 21}, {12788, 3, -1, 1}, {3367, 3, -1, 1}// 692	693	694	
			, {3308, 3, -1, 1}, {12788, 3, 0, 1}, {3212, 3, 0, 2}// 695	696	697	
			, {12788, 3, 1, 1}, {3377, 3, 1, 3}, {12788, 3, 2, 1}// 698	699	700	
			, {3224, 3, 2, 2}, {365, 3, 3, 1}, {12788, 3, 3, 1}// 701	702	703	
			, {12788, 3, 4, 1}, {1181, 3, 5, 1}, {1181, 3, 6, 1}// 704	705	706	
			, {7, 4, -9, 1}, {2147, 4, -9, 21}, {12788, 4, -8, 1}// 707	708	709	
			, {1193, 4, -8, 6}, {1194, 4, -8, 12}, {6869, 4, -8, 13}// 710	711	712	
			, {8401, 4, -8, 16}, {22, 4, -8, 1}, {21, 4, -8, 6}// 713	714	715	
			, {1181, 4, -8, 21}, {7626, 4, -8, 21}, {12788, 4, -7, 1}// 716	717	718	
			, {4022, 4, -7, 1}, {1181, 4, -7, 21}, {12788, 4, -6, 1}// 719	720	721	
			, {1181, 4, -6, 21}, {12788, 4, -5, 1}, {4151, 4, -5, 1}// 722	723	724	
			, {1181, 4, -5, 21}, {12788, 4, -4, 1}, {1181, 4, -4, 21}// 725	726	727	
			, {12788, 4, -3, 1}, {1181, 4, -3, 21}, {7626, 4, -3, 21}// 728	729	730	
			, {12788, 4, -2, 1}, {7, 4, -2, 1}, {1181, 4, -2, 21}// 731	732	733	
			, {2147, 4, -2, 21}, {12788, 4, -1, 1}, {3310, 4, -1, 1}// 734	735	736	
			, {12788, 4, 0, 1}, {3237, 4, 0, 1}, {12788, 4, 1, 1}// 737	738	739	
			, {3262, 4, 1, 2}, {3265, 4, 1, 2}, {12788, 4, 2, 1}// 740	741	742	
			, {3233, 4, 2, 1}, {368, 4, 3, 1}, {12788, 4, 3, 1}// 743	744	745	
			, {3235, 4, 3, 2}, {1181, 4, 4, 1}, {12788, 4, 5, 1}// 746	747	748	
			, {12788, 4, 6, 1}, {6019, 4, 6, 1}, {7, 5, -9, 1}// 749	750	751	
			, {2147, 5, -9, 21}, {12788, 5, -8, 1}, {20, 5, -8, 1}// 752	753	754	
			, {1194, 5, -8, 6}, {6868, 5, -8, 6}, {1181, 5, -8, 21}// 755	756	757	
			, {3206, 5, -8, 22}, {3209, 5, -8, 23}, {8401, 5, -8, 10}// 758	759	760	
			, {12788, 5, -7, 1}, {1181, 5, -7, 21}, {7624, 5, -7, 21}// 761	762	763	
			, {12788, 5, -6, 1}, {1181, 5, -6, 21}, {7625, 5, -6, 21}// 764	765	766	
			, {12788, 5, -5, 1}, {1181, 5, -5, 21}, {7625, 5, -5, 21}// 767	768	769	
			, {12788, 5, -4, 1}, {1181, 5, -4, 21}, {7623, 5, -4, 21}// 770	771	772	
			, {12788, 5, -3, 1}, {1181, 5, -3, 21}, {3206, 5, -3, 22}// 773	774	775	
			, {3209, 5, -3, 23}, {12788, 5, -2, 1}, {7, 5, -2, 1}// 776	777	778	
			, {1181, 5, -2, 21}, {2147, 5, -2, 21}, {12788, 5, -1, 1}// 779	780	781	
			, {3376, 5, -1, 1}, {3236, 5, -1, 1}, {3309, 5, -1, 1}// 782	783	784	
			, {12788, 5, 0, 1}, {3203, 5, 0, 1}, {3206, 5, 0, 2}// 785	786	787	
			, {12788, 5, 1, 1}, {3220, 5, 1, 2}, {12788, 5, 2, 1}// 788	789	790	
			, {370, 5, 3, 1}, {12788, 5, 3, 1}, {3365, 5, 3, 1}// 791	792	793	
			, {12788, 5, 4, 1}, {6025, 5, 4, 1}, {6023, 5, 5, 1}// 794	795	796	
			, {12, 6, -9, 1}, {2147, 6, -9, 21}, {11, 6, -8, 1}// 797	798	799	
			, {12788, 6, -8, 1}, {1181, 6, -8, 21}, {2148, 6, -8, 21}// 800	801	802	
			, {12788, 6, -7, 1}, {1181, 6, -7, 21}, {2148, 6, -7, 21}// 803	805	806	
			, {12788, 6, -6, 1}, {13, 6, -6, 1}, {1181, 6, -6, 21}// 807	808	809	
			, {2148, 6, -6, 21}, {12788, 6, -5, 1}, {1181, 6, -5, 21}// 810	811	812	
			, {2148, 6, -5, 21}, {12788, 6, -4, 1}, {11, 6, -4, 1}// 813	814	815	
			, {1181, 6, -4, 21}, {2148, 6, -4, 21}, {12788, 6, -3, 1}// 816	817	818	
			, {1181, 6, -3, 21}, {2148, 6, -3, 21}, {6, 6, -2, 1}// 820	821	822	
			, {12788, 6, -2, 1}, {1181, 6, -2, 21}, {2146, 6, -2, 21}// 823	824	825	
			, {367, 6, -1, 1}, {12788, 6, -1, 1}, {6019, 6, -1, 1}// 826	827	828	
			, {3372, 6, -1, 2}, {3373, 6, -1, 1}, {366, 6, 0, 1}// 829	830	831	
			, {12788, 6, 0, 1}, {6019, 6, 0, 1}, {367, 6, 1, 1}// 832	833	834	
			, {12788, 6, 1, 1}, {6019, 6, 1, 1}, {6008, 6, 1, 1}// 835	836	837	
			, {369, 6, 2, 1}, {12788, 6, 2, 1}, {6019, 6, 2, 1}// 838	839	840	
			, {3232, 6, 2, 2}, {364, 6, 3, 1}, {12788, 6, 3, 1}// 841	842	843	
			, {6019, 6, 3, 1}, {6023, 6, 4, 1}, {2147, 7, -9, 1}// 844	845	846	
			, {4108, 7, -8, 1}, {2147, 7, -2, 1}, {2147, 8, -9, 1}// 847	848	849	
			, {3892, 8, -7, 1}, {2147, 8, -2, 1}, {2147, 9, -9, 1}// 850	851	852	
			, {2883, 9, -8, 1}, {4150, 9, -6, 1}, {2147, 10, -9, 1}// 853	854	855	
			, {2884, 10, -8, 1}, {3899, 10, -7, 1}, {2147, 11, -9, 1}// 856	857	858	
			, {2147, 11, -2, 1}, {516, -4, 7, 1}, {2148, -4, 7, 21}// 859	860	861	
			, {513, -4, 8, 1}, {2148, -4, 8, 21}, {25, -4, 9, 23}// 862	863	864	
			, {1181, -3, 7, 21}, {1181, -3, 7, 1}, {4497, -3, 7, 1}// 865	866	867	
			, {5995, -3, 7, 11}, {517, -3, 8, 1}, {2147, -3, 8, 21}// 868	869	870	
			, {1181, -3, 8, 21}, {1181, -3, 8, 1}, {12791, -3, 9, 23}// 871	872	873	
			, {3206, -3, 9, 26}, {3204, -3, 9, 26}, {3128, -3, 9, 27}// 874	875	876	
			, {24, -3, 9, 23}, {1181, -2, 7, 21}, {1181, -2, 7, 1}// 877	878	879	
			, {2147, -2, 8, 21}, {1181, -2, 8, 21}, {1181, -2, 8, 1}// 881	882	883	
			, {12791, -2, 9, 23}, {3206, -2, 9, 26}, {3204, -2, 9, 26}// 884	885	886	
			, {3128, -2, 9, 27}, {24, -2, 9, 23}, {1181, -1, 7, 21}// 887	888	889	
			, {1181, -1, 7, 1}, {2147, -1, 8, 21}, {1181, -1, 8, 21}// 890	892	893	
			, {1181, -1, 8, 1}, {12791, -1, 9, 23}, {3206, -1, 9, 26}// 894	895	896	
			, {3204, -1, 9, 26}, {3128, -1, 9, 27}, {24, -1, 9, 23}// 897	898	899	
			, {518, 0, 7, 1}, {2148, 0, 7, 21}, {1181, 0, 7, 21}// 900	901	902	
			, {1181, 0, 7, 1}, {511, 0, 8, 1}, {2146, 0, 8, 21}// 903	904	905	
			, {1181, 0, 8, 21}, {1181, 0, 8, 1}, {12791, 0, 9, 23}// 906	907	908	
			, {3206, 0, 9, 26}, {3204, 0, 9, 26}, {3128, 0, 9, 27}// 909	910	911	
			, {25, 0, 9, 23}, {24, 0, 9, 23}, {1181, 1, 7, 1}// 912	913	914	
			, {12788, 1, 8, 1}, {6018, 1, 8, 1}, {1181, 2, 7, 1}// 915	916	917	
			, {6025, 2, 8, 1}, {6018, 2, 8, 1}, {1181, 2, 8, 1}// 918	919	920	
			, {12788, 3, 7, 1}, {6018, 3, 7, 1}, {6025, 4, 7, 1}// 921	922	923	
			, {1181, 4, 7, 1}// 924	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new sandstonefarmfinalAddonDeed();
			}
		}

		[ Constructable ]
		public sandstonefarmfinalAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 1122, -10, -1, 1, 0, 0, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 2330, -9, -4, 15, 2121, -1, "", 1);// 48
			AddComplexComponent( (BaseAddon) this, 2842, -9, -4, 11, 0, 0, "", 1);// 53
			AddComplexComponent( (BaseAddon) this, 5658, -9, -3, 21, 145, -1, "", 1);// 59
			AddComplexComponent( (BaseAddon) this, 5657, -9, -2, 21, 145, -1, "", 1);// 66
			AddComplexComponent( (BaseAddon) this, 1091, -9, 1, 6, 1753, -1, "", 1);// 76
			AddComplexComponent( (BaseAddon) this, 1092, -9, 2, 6, 1753, -1, "", 1);// 83
			AddComplexComponent( (BaseAddon) this, 1801, -9, 2, 1, 1753, -1, "", 1);// 85
			AddComplexComponent( (BaseAddon) this, 1090, -9, 3, 6, 1753, -1, "", 1);// 93
			AddComplexComponent( (BaseAddon) this, 1801, -9, 3, 1, 1753, -1, "", 1);// 94
			AddComplexComponent( (BaseAddon) this, 521, -7, 5, 21, 0, 0, "", 1);// 201
			AddComplexComponent( (BaseAddon) this, 521, -7, 5, 1, 0, 0, "", 1);// 202
			AddComplexComponent( (BaseAddon) this, 2894, -6, 0, 1, 2121, -1, "", 1);// 234
			AddComplexComponent( (BaseAddon) this, 2894, -6, 1, 1, 2121, -1, "", 1);// 238
			AddComplexComponent( (BaseAddon) this, 521, -6, 5, 1, 0, 0, "", 1);// 251
			AddComplexComponent( (BaseAddon) this, 521, -5, 5, 21, 0, 0, "", 1);// 305
			AddComplexComponent( (BaseAddon) this, 2895, -4, -2, 1, 2121, -1, "", 1);// 331
			AddComplexComponent( (BaseAddon) this, 2845, -4, 1, 15, 0, 0, "", 1);// 346
			AddComplexComponent( (BaseAddon) this, 2896, -4, 3, 1, 2121, -1, "", 1);// 355
			AddComplexComponent( (BaseAddon) this, 521, -3, -6, 1, 0, 0, "", 1);// 370
			AddComplexComponent( (BaseAddon) this, 2897, -3, 0, 1, 2121, -1, "", 1);// 391
			AddComplexComponent( (BaseAddon) this, 2897, -3, 1, 1, 2121, -1, "", 1);// 396
			AddComplexComponent( (BaseAddon) this, 2886, -3, 6, 10, 2121, -1, "", 1);// 416
			AddComplexComponent( (BaseAddon) this, 522, 0, 0, 1, 0, 0, "", 1);// 539
			AddComplexComponent( (BaseAddon) this, 522, 0, 1, 21, 0, 0, "", 1);// 545
			AddComplexComponent( (BaseAddon) this, 522, 0, 2, 1, 0, 0, "", 1);// 550
			AddComplexComponent( (BaseAddon) this, 522, 0, 3, 21, 0, 0, "", 1);// 554
			AddComplexComponent( (BaseAddon) this, 2885, 1, -8, 21, 145, -1, "", 1);// 578
			AddComplexComponent( (BaseAddon) this, 15, 6, -7, 1, 0, 0, "", 1);// 804
			AddComplexComponent( (BaseAddon) this, 15, 6, -3, 1, 0, 0, "", 1);// 819
			AddComplexComponent( (BaseAddon) this, 521, -2, 8, 1, 0, 0, "", 1);// 880
			AddComplexComponent( (BaseAddon) this, 521, -1, 8, 1, 0, 0, "", 1);// 891

		}

		public sandstonefarmfinalAddon( Serial serial ) : base( serial )
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

	public class sandstonefarmfinalAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new sandstonefarmfinalAddon();
			}
		}

		[Constructable]
		public sandstonefarmfinalAddonDeed()
		{
			Name = "sandstonefarmfinal";
		}

		public sandstonefarmfinalAddonDeed( Serial serial ) : base( serial )
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