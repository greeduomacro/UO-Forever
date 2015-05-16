
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class CTFDungeonAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1305, -37, -47, 22}, {1305, -37, -43, 22}, {1305, -37, -44, 22}// 13	19	29	
			, {1305, -39, -37, 22}, {1305, -37, -40, 22}, {1305, -37, -48, 22}// 30	43	47	
			, {1305, -37, -42, 22}, {1305, -37, -39, 22}, {1305, -45, -37, 22}// 49	50	57	
			, {1305, -43, -37, 22}, {7107, -46, -46, 0}, {1305, -37, -45, 22}// 59	60	65	
			, {1305, -38, -38, 22}, {1305, -38, -37, 22}, {1305, -41, -37, 22}// 66	68	72	
			, {1305, -47, -37, 22}, {1305, -40, -37, 22}, {1822, -36, -38, 22}// 73	74	78	
			, {1305, -48, -37, 22}, {1305, -37, -46, 22}, {1305, -42, -37, 22}// 79	82	83	
			, {1305, -46, -37, 22}, {1305, -44, -37, 22}, {1305, -37, -41, 22}// 84	87	89	
			, {1305, -37, -38, 22}, {1305, -39, -23, 22}, {2512, -36, -27, 22}// 90	92	93	
			, {1305, -43, -24, 22}, {1305, -39, -24, 22}, {2882, -36, -23, 22}// 94	95	96	
			, {1305, -44, -26, 22}, {1305, -44, -25, 22}, {1305, -44, -24, 22}// 97	98	99	
			, {1305, -44, -23, 22}, {2512, -36, -27, 22}, {4086, -36, -26, 22}// 100	102	103	
			, {2512, -36, -27, 22}, {1305, -43, -23, 22}, {2512, -36, -27, 22}// 104	105	106	
			, {1305, -41, -23, 22}, {1305, -40, -26, 22}, {2512, -36, -27, 22}// 107	108	113	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 117	118	119	
			, {1305, -43, -26, 22}, {1305, -43, -27, 22}, {4086, -36, -26, 22}// 120	121	123	
			, {4086, -36, -26, 22}, {1305, -40, -24, 22}, {1305, -45, -27, 22}// 126	127	128	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {7107, -43, -28, 22}// 129	130	133	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 134	135	136	
			, {4086, -36, -26, 22}, {2512, -36, -27, 22}, {1305, -43, -25, 22}// 137	138	139	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {1305, -42, -26, 22}// 140	141	142	
			, {2512, -36, -27, 22}, {1305, -40, -25, 22}, {1305, -42, -28, 22}// 143	144	146	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 147	148	150	
			, {1305, -37, -27, 22}, {1305, -38, -28, 22}, {1305, -45, -28, 22}// 151	152	153	
			, {1305, -45, -26, 22}, {1305, -45, -25, 22}, {1305, -45, -24, 22}// 154	155	156	
			, {1305, -45, -23, 22}, {1305, -44, -28, 22}, {1305, -44, -27, 22}// 157	158	159	
			, {1305, -38, -26, 22}, {1305, -38, -25, 22}, {1305, -38, -24, 22}// 160	161	162	
			, {1305, -38, -23, 22}, {1305, -37, -28, 22}, {4086, -36, -26, 22}// 163	164	165	
			, {1305, -37, -23, 22}, {1305, -42, -23, 22}, {1305, -41, -27, 22}// 167	168	170	
			, {1305, -41, -24, 22}, {2512, -36, -27, 22}, {1305, -36, -23, 22}// 172	173	180	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 181	182	183	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 185	187	188	
			, {4086, -36, -26, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 189	190	191	
			, {1305, -41, -26, 22}, {2512, -36, -27, 22}, {1305, -38, -27, 22}// 193	194	195	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 199	200	202	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {1305, -42, -24, 22}// 203	204	205	
			, {1305, -42, -25, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 207	211	212	
			, {2881, -36, -24, 22}, {2512, -36, -27, 22}, {1305, -36, -28, 22}// 213	214	216	
			, {1305, -36, -27, 22}, {1305, -36, -26, 22}, {1305, -36, -25, 22}// 217	218	219	
			, {1305, -36, -24, 22}, {1305, -37, -26, 22}, {1305, -37, -25, 22}// 220	221	222	
			, {1305, -37, -24, 22}, {4086, -36, -26, 22}, {2512, -36, -27, 22}// 223	225	226	
			, {1305, -41, -25, 22}, {7107, -39, -28, 22}, {4086, -36, -26, 22}// 227	228	229	
			, {4086, -36, -26, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 230	231	232	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 233	234	235	
			, {7107, -41, -28, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 236	237	238	
			, {2512, -36, -27, 22}, {1305, -40, -27, 22}, {4086, -36, -26, 22}// 239	240	241	
			, {2512, -36, -27, 22}, {2512, -36, -27, 22}, {2512, -36, -27, 22}// 242	244	245	
			, {1305, -42, -27, 22}, {1305, -40, -23, 22}, {1305, -39, -27, 22}// 246	247	249	
			, {1305, -40, -28, 22}, {1305, -39, -26, 22}, {1305, -39, -25, 22}// 250	253	254	
			, {1313, -47, 35, 22}, {1313, -46, 35, 22}, {1313, -45, 35, 22}// 313	314	315	
			, {1313, -37, 44, 22}, {1313, -37, 45, 22}, {1313, -37, 46, 22}// 355	356	357	
			, {1313, -38, 36, 22}, {1313, -37, 36, 22}, {1313, -37, 37, 22}// 358	362	363	
			, {1313, -48, 35, 22}, {1313, -37, 40, 22}, {1313, -37, 41, 22}// 380	404	405	
			, {1313, -37, 42, 22}, {1313, -37, 43, 22}, {1313, -37, 38, 22}// 406	407	428	
			, {1313, -37, 39, 22}, {1313, -44, 35, 22}, {1313, -43, 35, 22}// 429	446	447	
			, {1313, -42, 35, 22}, {1313, -41, 35, 22}, {1313, -40, 35, 22}// 448	449	450	
			, {1313, -39, 35, 22}, {1313, -38, 35, 22}, {1955, -46, 49, 0}// 451	452	454	
			, {1955, -46, 49, 0}, {1300, -30, -2, 42}, {1300, -29, -3, 42}// 455	463	466	
			, {1300, -29, -2, 42}, {1300, -29, -1, 42}, {1300, -29, -4, 42}// 467	468	471	
			, {1300, -30, -1, 42}, {1300, -30, -3, 42}, {1300, -30, -4, 42}// 476	477	479	
			, {1300, -29, 2, 42}, {1300, -29, 0, 42}, {1300, -29, 1, 42}// 485	486	491	
			, {1300, -30, 1, 42}, {1300, -29, 3, 42}, {1300, -30, 3, 42}// 492	495	496	
			, {1300, -30, 2, 42}, {1300, -30, 0, 42}, {1305, -23, 38, 22}// 497	498	499	
			, {1305, -23, 39, 22}, {1305, -23, 40, 22}, {1305, -23, 41, 22}// 500	501	504	
			, {1305, -23, 42, 22}, {7107, -28, 39, 22}, {1305, -28, 33, 22}// 506	507	511	
			, {1305, -28, 34, 22}, {1305, -28, 35, 22}, {1305, -28, 36, 22}// 512	513	514	
			, {7107, -28, 37, 22}, {1305, -27, 44, 22}, {1305, -26, 33, 22}// 518	519	520	
			, {1305, -26, 34, 22}, {1305, -26, 35, 22}, {1305, -26, 36, 22}// 521	522	523	
			, {1305, -26, 37, 22}, {1305, -26, 38, 22}, {1305, -26, 39, 22}// 524	525	526	
			, {1305, -26, 40, 22}, {1305, -28, 38, 22}, {1305, -28, 40, 22}// 527	530	532	
			, {1305, -23, 36, 22}, {1305, -23, 37, 22}, {1305, -26, 41, 22}// 553	554	564	
			, {1305, -26, 42, 22}, {1305, -26, 43, 22}, {1305, -26, 44, 22}// 565	566	567	
			, {1305, -25, 33, 22}, {1305, -25, 34, 22}, {1305, -25, 35, 22}// 568	569	570	
			, {1305, -25, 36, 22}, {1305, -25, 37, 22}, {1305, -25, 38, 22}// 571	572	573	
			, {1305, -25, 39, 22}, {1305, -25, 40, 22}, {1305, -25, 41, 22}// 574	575	576	
			, {1305, -25, 42, 22}, {1305, -25, 43, 22}, {1305, -25, 44, 22}// 577	578	579	
			, {1305, -24, 33, 22}, {1305, -24, 34, 22}, {1305, -24, 35, 22}// 580	581	582	
			, {1305, -24, 36, 22}, {1305, -24, 37, 22}, {1305, -24, 38, 22}// 583	584	585	
			, {1305, -24, 39, 22}, {1305, -24, 40, 22}, {1305, -23, 43, 22}// 586	587	589	
			, {1305, -23, 44, 22}, {1305, -24, 41, 22}, {1305, -24, 42, 22}// 590	592	593	
			, {1305, -24, 43, 22}, {1305, -24, 44, 22}, {1305, -23, 33, 22}// 594	595	596	
			, {1305, -23, 34, 22}, {1305, -23, 35, 22}, {7107, -28, 41, 22}// 597	598	599	
			, {1305, -28, 42, 22}, {1305, -28, 43, 22}, {1305, -28, 44, 22}// 601	602	603	
			, {1305, -27, 33, 22}, {1305, -27, 34, 22}, {1305, -27, 35, 22}// 604	605	606	
			, {1305, -27, 36, 22}, {1305, -27, 37, 22}, {1305, -27, 38, 22}// 607	608	609	
			, {1305, -27, 39, 22}, {1305, -27, 41, 22}, {1305, -27, 42, 22}// 610	611	612	
			, {1305, -27, 43, 22}, {1305, -27, 40, 22}, {2866, 5, -12, 66}// 613	614	675	
			, {1305, 4, -13, 66}, {1305, 5, -12, 66}, {1305, 6, -11, 66}// 676	677	678	
			, {1305, 4, -12, 66}, {1305, 6, -13, 66}, {1305, 4, -11, 66}// 679	685	688	
			, {1305, 6, -11, 66}, {1305, 6, -12, 66}, {1305, 5, -11, 66}// 689	690	691	
			, {1305, 5, -13, 66}, {1305, 22, -43, 22}, {1305, 27, -35, 22}// 693	737	740	
			, {1305, 22, -44, 22}, {7107, 27, -40, 22}, {1305, 27, -44, 22}// 741	742	743	
			, {1305, 27, -43, 22}, {1305, 25, -43, 22}, {1305, 26, -36, 22}// 744	746	748	
			, {1305, 26, -35, 22}, {1305, 26, -43, 22}, {1305, 26, -42, 22}// 749	750	751	
			, {1305, 27, -41, 22}, {1305, 24, -41, 22}, {1305, 23, -43, 22}// 753	756	757	
			, {1305, 27, -36, 22}, {1305, 23, -36, 22}, {1305, 22, -38, 22}// 765	767	768	
			, {1305, 22, -37, 22}, {1305, 24, -35, 22}, {1305, 25, -44, 22}// 769	770	771	
			, {1305, 25, -42, 22}, {1305, 26, -38, 22}, {1305, 27, -39, 22}// 772	773	774	
			, {1305, 24, -43, 22}, {1305, 23, -44, 22}, {1305, 23, -37, 22}// 776	779	783	
			, {1305, 25, -37, 22}, {1305, 25, -36, 22}, {1305, 25, -35, 22}// 786	787	788	
			, {1305, 26, -44, 22}, {1305, 25, -41, 22}, {1305, 26, -41, 22}// 789	790	791	
			, {1305, 26, -40, 22}, {1305, 26, -39, 22}, {1305, 25, -38, 22}// 792	793	794	
			, {1305, 27, -37, 22}, {1305, 26, -37, 22}, {1305, 23, -35, 22}// 795	797	798	
			, {1305, 23, -41, 22}, {1305, 25, -40, 22}, {1305, 25, -39, 22}// 800	802	803	
			, {1305, 24, -37, 22}, {1305, 24, -36, 22}, {1305, 24, -42, 22}// 804	805	806	
			, {1305, 22, -40, 22}, {1305, 24, -38, 22}, {1305, 24, -40, 22}// 807	808	810	
			, {1305, 23, -42, 22}, {1305, 22, -36, 22}, {1305, 24, -39, 22}// 812	815	816	
			, {1305, 24, -44, 22}, {7107, 27, -38, 22}, {1305, 22, -39, 22}// 817	820	822	
			, {1305, 23, -39, 22}, {1305, 22, -35, 22}, {1305, 22, -41, 22}// 823	824	826	
			, {1305, 22, -42, 22}, {1305, 23, -40, 22}, {7107, 27, -42, 22}// 827	829	830	
			, {1305, 23, -38, 22}, {1305, 27, -2, 42}, {1305, 28, -1, 42}// 831	833	834	
			, {1305, 26, -5, 42}, {1305, 25, -1, 42}, {1305, 27, -3, 42}// 835	837	839	
			, {1305, 28, -2, 42}, {1305, 28, -5, 42}, {1305, 25, -5, 42}// 840	843	845	
			, {1305, 26, -3, 42}, {1305, 28, -3, 42}, {1305, 26, -1, 42}// 846	847	848	
			, {1305, 27, -1, 42}, {1305, 27, -5, 42}, {1305, 25, -2, 42}// 849	850	851	
			, {1305, 25, -3, 42}, {1305, 25, -4, 42}, {1305, 26, -2, 42}// 853	854	855	
			, {1305, 28, -4, 42}, {1305, 26, -4, 42}, {1305, 27, -4, 42}// 857	860	863	
			, {1305, 25, 0, 42}, {1305, 26, 0, 42}, {1305, 28, 0, 42}// 867	869	870	
			, {1305, 27, 0, 42}, {1313, 35, -38, 22}, {1313, 35, -37, 22}// 871	884	885	
			, {1313, 36, -37, 22}, {1313, 46, -37, 22}, {1313, 45, -37, 22}// 886	887	888	
			, {1313, 43, -37, 22}, {1313, 42, -37, 22}, {1313, 44, -37, 22}// 889	890	898	
			, {1313, 35, -41, 22}, {1313, 35, -40, 22}, {1313, 35, -43, 22}// 900	901	915	
			, {1313, 35, -42, 22}, {1313, 36, -38, 22}, {1313, 41, -37, 22}// 916	943	946	
			, {1313, 40, -37, 22}, {1313, 39, -37, 22}, {1313, 38, -37, 22}// 947	948	981	
			, {1313, 37, -37, 22}, {7107, 44, -46, 0}, {1313, 35, -39, 22}// 983	992	1001	
			, {1313, 35, -47, 22}, {1313, 35, -46, 22}, {1313, 35, -45, 22}// 1019	1021	1022	
			, {1313, 35, -44, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1023	1069	1070	
			, {7107, 39, 27, 22}, {2512, 44, 23, 22}, {2882, 44, 27, 22}// 1071	1072	1074	
			, {2512, 44, 23, 22}, {2881, 44, 26, 22}, {2512, 44, 23, 22}// 1075	1076	1077	
			, {2512, 44, 23, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1078	1079	1081	
			, {2512, 44, 23, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1082	1085	1086	
			, {2512, 44, 23, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1087	1089	1094	
			, {2512, 44, 23, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1095	1096	1097	
			, {4086, 44, 24, 22}, {1306, 40, 23, 22}, {1306, 40, 24, 22}// 1102	1103	1104	
			, {7107, 41, 27, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1105	1106	1107	
			, {4086, 44, 24, 22}, {2512, 44, 23, 22}, {1306, 35, 26, 22}// 1108	1109	1110	
			, {1306, 35, 27, 22}, {1306, 36, 23, 22}, {1306, 36, 24, 22}// 1111	1112	1113	
			, {1306, 35, 23, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1114	1115	1116	
			, {1306, 36, 25, 22}, {1306, 36, 26, 22}, {1306, 36, 27, 22}// 1117	1118	1119	
			, {1306, 37, 23, 22}, {1306, 37, 24, 22}, {1306, 37, 25, 22}// 1120	1121	1122	
			, {4086, 44, 24, 22}, {1306, 35, 24, 22}, {4086, 44, 24, 22}// 1123	1128	1130	
			, {2512, 44, 23, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1131	1133	1134	
			, {1306, 37, 26, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1135	1137	1138	
			, {2512, 44, 23, 22}, {4086, 44, 24, 22}, {4086, 44, 24, 22}// 1139	1140	1141	
			, {2512, 44, 23, 22}, {2512, 44, 23, 22}, {4086, 44, 24, 22}// 1142	1143	1144	
			, {7107, 37, 27, 22}, {1306, 40, 25, 22}, {1306, 40, 26, 22}// 1145	1146	1147	
			, {1306, 40, 27, 22}, {1306, 41, 23, 22}, {1306, 41, 24, 22}// 1148	1149	1150	
			, {1306, 41, 25, 22}, {1306, 41, 26, 22}, {2512, 44, 23, 22}// 1151	1152	1153	
			, {4086, 44, 24, 22}, {1306, 44, 25, 22}, {1306, 44, 26, 22}// 1154	1156	1157	
			, {1306, 44, 27, 22}, {2512, 44, 23, 22}, {1306, 35, 25, 22}// 1158	1163	1164	
			, {2512, 44, 23, 22}, {1306, 42, 23, 22}, {1306, 42, 24, 22}// 1165	1167	1168	
			, {2512, 44, 23, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1169	1170	1178	
			, {4086, 44, 24, 22}, {2512, 44, 23, 22}, {1306, 39, 23, 22}// 1179	1180	1182	
			, {1306, 39, 24, 22}, {1306, 39, 25, 22}, {1306, 39, 26, 22}// 1183	1184	1185	
			, {1306, 42, 25, 22}, {1306, 42, 26, 22}, {1306, 42, 27, 22}// 1187	1188	1189	
			, {1306, 43, 23, 22}, {1306, 43, 24, 22}, {1306, 43, 25, 22}// 1190	1191	1192	
			, {1306, 43, 26, 22}, {1306, 43, 27, 22}, {1306, 44, 23, 22}// 1193	1194	1195	
			, {1306, 44, 24, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1196	1197	1198	
			, {4086, 44, 24, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1200	1201	1204	
			, {2512, 44, 23, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1205	1209	1210	
			, {2512, 44, 23, 22}, {2512, 44, 23, 22}, {2512, 44, 23, 22}// 1211	1212	1216	
			, {2512, 44, 23, 22}, {2512, 44, 23, 22}, {1306, 38, 23, 22}// 1217	1218	1220	
			, {1306, 38, 24, 22}, {1306, 38, 25, 22}, {1306, 38, 26, 22}// 1221	1222	1223	
			, {1306, 38, 27, 22}, {2512, 44, 23, 22}, {7107, 44, 44, 0}// 1224	1226	1259	
			, {1313, 42, 35, 22}, {1313, 35, 37, 22}, {1313, 46, 35, 22}// 1260	1273	1275	
			, {1313, 35, 36, 22}, {1313, 45, 35, 22}, {1313, 35, 38, 22}// 1276	1290	1297	
			, {1313, 36, 36, 22}, {1313, 41, 35, 22}, {1313, 44, 35, 22}// 1309	1327	1330	
			, {1313, 40, 35, 22}, {1313, 39, 35, 22}, {1313, 38, 35, 22}// 1337	1338	1339	
			, {1313, 36, 35, 22}, {1313, 35, 46, 22}, {1313, 35, 45, 22}// 1345	1346	1349	
			, {1313, 35, 44, 22}, {1313, 35, 43, 22}, {1313, 35, 42, 22}// 1350	1351	1352	
			, {1313, 35, 41, 22}, {1313, 37, 35, 22}, {1313, 43, 35, 22}// 1353	1354	1355	
			, {1313, 35, 40, 22}, {1313, 35, 39, 22}// 1365	1366	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new CTFDungeonAddonDeed();
			}
		}

		[ Constructable ]
		public CTFDungeonAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 128, -37, -45, 0, 1368, -1, "", 1);// 1
			AddComplexComponent( (BaseAddon) this, 1822, -41, -36, 22, 1368, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 1822, -36, -42, 22, 1368, -1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 8610, -33, -40, 22, 0, -1, "no line of sight", 1);// 4
			AddComplexComponent( (BaseAddon) this, 8610, -33, -39, 22, 0, -1, "no line of sight", 1);// 5
			AddComplexComponent( (BaseAddon) this, 128, -44, -37, 0, 1368, -1, "", 1);// 6
			AddComplexComponent( (BaseAddon) this, 1822, -38, -36, 22, 1368, -1, "", 1);// 7
			AddComplexComponent( (BaseAddon) this, 7774, -41, -48, 0, 0, -1, "CTF Score board", 1);// 8
			AddComplexComponent( (BaseAddon) this, 8610, -33, -34, 22, 0, -1, "no line of sight", 1);// 9
			AddComplexComponent( (BaseAddon) this, 8611, -47, -36, 22, 0, -1, "Block Everything", 1);// 10
			AddComplexComponent( (BaseAddon) this, 128, -37, -47, 0, 1368, -1, "", 1);// 11
			AddComplexComponent( (BaseAddon) this, 1822, -36, -45, 22, 1368, -1, "", 1);// 12
			AddComplexComponent( (BaseAddon) this, 128, -37, -40, 0, 1368, -1, "", 1);// 14
			AddComplexComponent( (BaseAddon) this, 14138, -46, -46, 1, 1368, -1, "", 1);// 15
			AddComplexComponent( (BaseAddon) this, 128, -40, -37, 0, 1368, -1, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 8610, -33, -33, 22, 0, -1, "no line of sight", 1);// 17
			AddComplexComponent( (BaseAddon) this, 1822, -44, -36, 22, 1368, -1, "", 1);// 18
			AddComplexComponent( (BaseAddon) this, 8611, -48, -36, 22, 0, -1, "Block Everything", 1);// 20
			AddComplexComponent( (BaseAddon) this, 8611, -38, -36, 22, 0, -1, "Block Everything", 1);// 21
			AddComplexComponent( (BaseAddon) this, 8610, -33, -38, 22, 0, -1, "no line of sight", 1);// 22
			AddComplexComponent( (BaseAddon) this, 1822, -36, -41, 22, 1368, -1, "", 1);// 23
			AddComplexComponent( (BaseAddon) this, 1822, -36, -44, 22, 1368, -1, "", 1);// 24
			AddComplexComponent( (BaseAddon) this, 128, -37, -38, 0, 1368, -1, "", 1);// 25
			AddComplexComponent( (BaseAddon) this, 8611, -36, -41, 22, 0, -1, "Block Everything", 1);// 26
			AddComplexComponent( (BaseAddon) this, 1822, -48, -37, 22, 1368, -1, "", 1);// 27
			AddComplexComponent( (BaseAddon) this, 1822, -36, -47, 22, 1368, -1, "", 1);// 28
			AddComplexComponent( (BaseAddon) this, 8611, -42, -36, 22, 0, -1, "Block Everything", 1);// 31
			AddComplexComponent( (BaseAddon) this, 1822, -45, -36, 22, 1368, -1, "", 1);// 32
			AddComplexComponent( (BaseAddon) this, 8611, -36, -38, 22, 0, -1, "Block Everything", 1);// 33
			AddComplexComponent( (BaseAddon) this, 8611, -36, -44, 22, 0, -1, "Block Everything", 1);// 34
			AddComplexComponent( (BaseAddon) this, 8611, -45, -36, 22, 0, -1, "Block Everything", 1);// 35
			AddComplexComponent( (BaseAddon) this, 128, -46, -37, 0, 1368, -1, "", 1);// 36
			AddComplexComponent( (BaseAddon) this, 128, -47, -37, 0, 1368, -1, "", 1);// 37
			AddComplexComponent( (BaseAddon) this, 128, -43, -37, 0, 1368, -1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 1822, -42, -36, 22, 1368, -1, "", 1);// 39
			AddComplexComponent( (BaseAddon) this, 8610, -34, -33, 22, 0, -1, "no line of sight", 1);// 40
			AddComplexComponent( (BaseAddon) this, 1822, -36, -48, 22, 1368, -1, "", 1);// 41
			AddComplexComponent( (BaseAddon) this, 8611, -36, -39, 22, 0, -1, "Block Everything", 1);// 42
			AddComplexComponent( (BaseAddon) this, 128, -39, -37, 0, 1368, -1, "", 1);// 44
			AddComplexComponent( (BaseAddon) this, 128, -38, -37, 0, 1368, -1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 128, -41, -37, 0, 1368, -1, "", 1);// 46
			AddComplexComponent( (BaseAddon) this, 8611, -36, -48, 22, 0, -1, "Block Everything", 1);// 48
			AddComplexComponent( (BaseAddon) this, 128, -37, -39, 0, 1368, -1, "", 1);// 51
			AddComplexComponent( (BaseAddon) this, 8611, -47, -36, 22, 0, -1, "Block Everything", 1);// 52
			AddComplexComponent( (BaseAddon) this, 128, -37, -46, 0, 1368, -1, "", 1);// 53
			AddComplexComponent( (BaseAddon) this, 128, -37, -41, 0, 1368, -1, "", 1);// 54
			AddComplexComponent( (BaseAddon) this, 128, -37, -43, 0, 1368, -1, "", 1);// 55
			AddComplexComponent( (BaseAddon) this, 1822, -37, -37, 22, 1368, -1, "", 1);// 56
			AddComplexComponent( (BaseAddon) this, 1822, -36, -38, 22, 1368, -1, "", 1);// 58
			AddComplexComponent( (BaseAddon) this, 8610, -33, -35, 22, 0, -1, "no line of sight", 1);// 61
			AddComplexComponent( (BaseAddon) this, 128, -48, -37, 0, 1368, -1, "", 1);// 62
			AddComplexComponent( (BaseAddon) this, 8611, -36, -42, 22, 0, -1, "Block Everything", 1);// 63
			AddComplexComponent( (BaseAddon) this, 128, -38, -38, 0, 1368, -1, "", 1);// 64
			AddComplexComponent( (BaseAddon) this, 8611, -44, -36, 22, 0, -1, "Block Everything", 1);// 67
			AddComplexComponent( (BaseAddon) this, 1822, -39, -36, 22, 1368, -1, "", 1);// 69
			AddComplexComponent( (BaseAddon) this, 1822, -48, -36, 22, 1368, -1, "", 1);// 70
			AddComplexComponent( (BaseAddon) this, 1822, -38, -36, 22, 1368, -1, "", 1);// 71
			AddComplexComponent( (BaseAddon) this, 1822, -36, -39, 22, 1368, -1, "", 1);// 75
			AddComplexComponent( (BaseAddon) this, 8611, -41, -36, 22, 0, -1, "Block Everything", 1);// 76
			AddComplexComponent( (BaseAddon) this, 8611, -37, -37, 22, 0, -1, "Block Everything", 1);// 77
			AddComplexComponent( (BaseAddon) this, 5671, -47, -47, 22, 1367, -1, "", 1);// 80
			AddComplexComponent( (BaseAddon) this, 8611, -36, -45, 22, 0, -1, "Block Everything", 1);// 81
			AddComplexComponent( (BaseAddon) this, 1822, -47, -36, 22, 1368, -1, "", 1);// 85
			AddComplexComponent( (BaseAddon) this, 128, -37, -42, 0, 1368, -1, "", 1);// 86
			AddComplexComponent( (BaseAddon) this, 8611, -36, -47, 22, 0, -1, "Block Everything", 1);// 88
			AddComplexComponent( (BaseAddon) this, 8610, -33, -41, 22, 0, -1, "no line of sight", 1);// 91
			AddComplexComponent( (BaseAddon) this, 8611, -46, -28, 22, 0, -1, "Block Everything", 1);// 101
			AddComplexComponent( (BaseAddon) this, 8611, -35, -27, 22, 0, -1, "Block Everything", 1);// 109
			AddComplexComponent( (BaseAddon) this, 8611, -35, -25, 22, 0, -1, "Block Everything", 1);// 110
			AddComplexComponent( (BaseAddon) this, 8611, -35, -28, 22, 0, -1, "Block Everything", 1);// 111
			AddComplexComponent( (BaseAddon) this, 8611, -42, -22, 22, 0, -1, "Block Everything", 1);// 112
			AddComplexComponent( (BaseAddon) this, 8611, -35, -22, 22, 0, -1, "Block Everything", 1);// 114
			AddComplexComponent( (BaseAddon) this, 7955, -36, -27, 22, 0, -1, "Spawner", 1);// 115
			AddComplexComponent( (BaseAddon) this, 8611, -35, -26, 22, 0, -1, "Block Everything", 1);// 116
			AddComplexComponent( (BaseAddon) this, 8611, -41, -29, 22, 0, -1, "Block Everything", 1);// 122
			AddComplexComponent( (BaseAddon) this, 8611, -38, -22, 22, 0, -1, "Block Everything", 1);// 124
			AddComplexComponent( (BaseAddon) this, 8611, -36, -22, 22, 0, -1, "Block Everything", 1);// 125
			AddComplexComponent( (BaseAddon) this, 8611, -44, -29, 22, 0, -1, "Block Everything", 1);// 131
			AddComplexComponent( (BaseAddon) this, 8611, -44, -22, 22, 0, -1, "Block Everything", 1);// 132
			AddComplexComponent( (BaseAddon) this, 1305, -43, -28, 22, 1368, -1, "", 1);// 145
			AddComplexComponent( (BaseAddon) this, 8611, -36, -29, 22, 0, -1, "Block Everything", 1);// 149
			AddComplexComponent( (BaseAddon) this, 8611, -43, -29, 22, 0, -1, "Block Everything", 1);// 166
			AddComplexComponent( (BaseAddon) this, 1305, -41, -28, 22, 1368, -1, "", 1);// 169
			AddComplexComponent( (BaseAddon) this, 8611, -37, -29, 22, 0, -1, "Block Everything", 1);// 171
			AddComplexComponent( (BaseAddon) this, 8611, -35, -29, 22, 0, -1, "Block Everything", 1);// 174
			AddComplexComponent( (BaseAddon) this, 8611, -46, -22, 22, 0, -1, "Block Everything", 1);// 175
			AddComplexComponent( (BaseAddon) this, 8611, -45, -22, 22, 0, -1, "Block Everything", 1);// 176
			AddComplexComponent( (BaseAddon) this, 8611, -41, -22, 22, 0, -1, "Block Everything", 1);// 177
			AddComplexComponent( (BaseAddon) this, 8611, -40, -22, 22, 0, -1, "Block Everything", 1);// 178
			AddComplexComponent( (BaseAddon) this, 8611, -39, -22, 22, 0, -1, "Block Everything", 1);// 179
			AddComplexComponent( (BaseAddon) this, 8611, -46, -26, 22, 0, -1, "Block Everything", 1);// 184
			AddComplexComponent( (BaseAddon) this, 8611, -45, -29, 22, 0, -1, "Block Everything", 1);// 186
			AddComplexComponent( (BaseAddon) this, 8611, -37, -22, 22, 0, -1, "Block Everything", 1);// 192
			AddComplexComponent( (BaseAddon) this, 8611, -43, -22, 22, 0, -1, "Block Everything", 1);// 196
			AddComplexComponent( (BaseAddon) this, 4483, -45, -25, 22, 1368, -1, "Game Supply Stone", 1);// 197
			AddComplexComponent( (BaseAddon) this, 8611, -46, -25, 22, 0, -1, "Block Everything", 1);// 198
			AddComplexComponent( (BaseAddon) this, 8611, -46, -29, 22, 0, -1, "Block Everything", 1);// 201
			AddComplexComponent( (BaseAddon) this, 8611, -46, -27, 22, 0, -1, "Block Everything", 1);// 206
			AddComplexComponent( (BaseAddon) this, 8611, -40, -29, 22, 0, -1, "Block Everything", 1);// 208
			AddComplexComponent( (BaseAddon) this, 8611, -39, -29, 22, 0, -1, "Block Everything", 1);// 209
			AddComplexComponent( (BaseAddon) this, 8611, -38, -29, 22, 0, -1, "Block Everything", 1);// 210
			AddComplexComponent( (BaseAddon) this, 8611, -42, -29, 22, 0, -1, "Block Everything", 1);// 215
			AddComplexComponent( (BaseAddon) this, 8611, -46, -23, 22, 0, -1, "Block Everything", 1);// 224
			AddComplexComponent( (BaseAddon) this, 7955, -36, -26, 22, 0, -1, "Spawner", 1);// 243
			AddComplexComponent( (BaseAddon) this, 1305, -39, -28, 22, 1368, -1, "", 1);// 248
			AddComplexComponent( (BaseAddon) this, 3796, -45, -26, 22, 1368, -1, "a reagent stone", 1);// 251
			AddComplexComponent( (BaseAddon) this, 8611, -35, -24, 22, 0, -1, "Block Everything", 1);// 252
			AddComplexComponent( (BaseAddon) this, 8611, -35, -23, 22, 0, -1, "Block Everything", 1);// 255
			AddComplexComponent( (BaseAddon) this, 8611, -46, -24, 22, 0, -1, "Block Everything", 1);// 256
			AddComplexComponent( (BaseAddon) this, 62, -35, -3, 22, 1109, -1, "", 1);// 257
			AddComplexComponent( (BaseAddon) this, 62, -34, -3, 22, 1109, -1, "", 1);// 258
			AddComplexComponent( (BaseAddon) this, 8611, -36, -3, 22, 0, -1, "Block Everything", 1);// 259
			AddComplexComponent( (BaseAddon) this, 8611, -35, -3, 22, 0, -1, "Block Everything", 1);// 260
			AddComplexComponent( (BaseAddon) this, 8611, -34, -3, 22, 0, -1, "Block Everything", 1);// 261
			AddComplexComponent( (BaseAddon) this, 8611, -33, -3, 22, 0, -1, "Block Everything", 1);// 262
			AddComplexComponent( (BaseAddon) this, 8610, -36, -3, 27, 0, -1, "no line of sight", 1);// 263
			AddComplexComponent( (BaseAddon) this, 8610, -37, -3, 27, 0, -1, "no line of sight", 1);// 264
			AddComplexComponent( (BaseAddon) this, 62, -37, -3, 22, 1109, -1, "", 1);// 265
			AddComplexComponent( (BaseAddon) this, 62, -36, -3, 22, 1109, -1, "", 1);// 266
			AddComplexComponent( (BaseAddon) this, 8611, -37, -3, 22, 0, -1, "Block Everything", 1);// 267
			AddComplexComponent( (BaseAddon) this, 62, -33, -3, 22, 1109, -1, "", 1);// 268
			AddComplexComponent( (BaseAddon) this, 8611, -37, 9, 22, 0, -1, "Block Everything", 1);// 269
			AddComplexComponent( (BaseAddon) this, 62, -39, 1, 22, 1109, -1, "", 1);// 270
			AddComplexComponent( (BaseAddon) this, 62, -38, 9, 22, 1109, -1, "", 1);// 271
			AddComplexComponent( (BaseAddon) this, 8610, -37, 5, 27, 0, -1, "no line of sight", 1);// 272
			AddComplexComponent( (BaseAddon) this, 62, -34, 5, 22, 1109, -1, "", 1);// 273
			AddComplexComponent( (BaseAddon) this, 8611, -39, 9, 22, 0, -1, "Block Everything", 1);// 274
			AddComplexComponent( (BaseAddon) this, 62, -38, 1, 22, 1109, -1, "", 1);// 275
			AddComplexComponent( (BaseAddon) this, 62, -37, 5, 22, 1109, -1, "", 1);// 276
			AddComplexComponent( (BaseAddon) this, 8611, -37, 5, 22, 0, -1, "Block Everything", 1);// 277
			AddComplexComponent( (BaseAddon) this, 8611, -40, 1, 22, 0, -1, "Block Everything", 1);// 278
			AddComplexComponent( (BaseAddon) this, 8611, -39, 1, 22, 0, -1, "Block Everything", 1);// 279
			AddComplexComponent( (BaseAddon) this, 8611, -38, 1, 22, 0, -1, "Block Everything", 1);// 280
			AddComplexComponent( (BaseAddon) this, 8611, -37, 1, 22, 0, -1, "Block Everything", 1);// 281
			AddComplexComponent( (BaseAddon) this, 8611, -35, 5, 22, 0, -1, "Block Everything", 1);// 282
			AddComplexComponent( (BaseAddon) this, 8611, -33, 5, 22, 0, -1, "Block Everything", 1);// 283
			AddComplexComponent( (BaseAddon) this, 8610, -38, 1, 27, 0, -1, "no line of sight", 1);// 284
			AddComplexComponent( (BaseAddon) this, 62, -33, 5, 22, 1109, -1, "", 1);// 285
			AddComplexComponent( (BaseAddon) this, 62, -35, 5, 22, 1109, -1, "", 1);// 286
			AddComplexComponent( (BaseAddon) this, 8610, -37, 9, 27, 0, -1, "no line of sight", 1);// 287
			AddComplexComponent( (BaseAddon) this, 8610, -38, 9, 27, 0, -1, "no line of sight", 1);// 288
			AddComplexComponent( (BaseAddon) this, 8611, -34, 5, 22, 0, -1, "Block Everything", 1);// 289
			AddComplexComponent( (BaseAddon) this, 62, -37, 1, 22, 1109, -1, "", 1);// 290
			AddComplexComponent( (BaseAddon) this, 8610, -37, 1, 27, 0, -1, "no line of sight", 1);// 291
			AddComplexComponent( (BaseAddon) this, 8610, -36, 5, 27, 0, -1, "no line of sight", 1);// 292
			AddComplexComponent( (BaseAddon) this, 8611, -38, 9, 22, 0, -1, "Block Everything", 1);// 293
			AddComplexComponent( (BaseAddon) this, 62, -39, 9, 22, 1109, -1, "", 1);// 294
			AddComplexComponent( (BaseAddon) this, 62, -37, 9, 22, 1109, -1, "", 1);// 295
			AddComplexComponent( (BaseAddon) this, 62, -40, 9, 22, 1109, -1, "", 1);// 296
			AddComplexComponent( (BaseAddon) this, 8611, -40, 9, 22, 0, -1, "Block Everything", 1);// 297
			AddComplexComponent( (BaseAddon) this, 62, -40, 1, 22, 1109, -1, "", 1);// 298
			AddComplexComponent( (BaseAddon) this, 8611, -36, 5, 22, 0, -1, "Block Everything", 1);// 299
			AddComplexComponent( (BaseAddon) this, 62, -36, 5, 22, 1109, -1, "", 1);// 300
			AddComplexComponent( (BaseAddon) this, 8610, -38, 31, 22, 0, -1, "no line of sight", 1);// 301
			AddComplexComponent( (BaseAddon) this, 8610, -35, 31, 22, 0, -1, "no line of sight", 1);// 302
			AddComplexComponent( (BaseAddon) this, 8610, -34, 31, 22, 0, -1, "no line of sight", 1);// 303
			AddComplexComponent( (BaseAddon) this, 8610, -39, 31, 22, 0, -1, "no line of sight", 1);// 304
			AddComplexComponent( (BaseAddon) this, 8610, -40, 31, 22, 0, -1, "no line of sight", 1);// 305
			AddComplexComponent( (BaseAddon) this, 8610, -33, 31, 22, 0, -1, "no line of sight", 1);// 306
			AddComplexComponent( (BaseAddon) this, 1955, -47, 44, 5, 1109, -1, "", 1);// 307
			AddComplexComponent( (BaseAddon) this, 1955, -47, 43, 5, 1109, -1, "", 1);// 308
			AddComplexComponent( (BaseAddon) this, 1955, -48, 44, 0, 1109, -1, "", 1);// 309
			AddComplexComponent( (BaseAddon) this, 1955, -48, 45, 0, 1109, -1, "", 1);// 310
			AddComplexComponent( (BaseAddon) this, 1955, -48, 46, 0, 1109, -1, "", 1);// 311
			AddComplexComponent( (BaseAddon) this, 1957, -43, 46, 0, 1109, -1, "", 1);// 312
			AddComplexComponent( (BaseAddon) this, 8611, -36, 36, 27, 0, -1, "Block Everything", 1);// 316
			AddComplexComponent( (BaseAddon) this, 8611, -36, 37, 27, 0, -1, "Block Everything", 1);// 317
			AddComplexComponent( (BaseAddon) this, 1957, -43, 45, 0, 1109, -1, "", 1);// 318
			AddComplexComponent( (BaseAddon) this, 1955, -45, 45, 5, 1109, -1, "", 1);// 319
			AddComplexComponent( (BaseAddon) this, 1822, -36, 46, 22, 1109, -1, "", 1);// 320
			AddComplexComponent( (BaseAddon) this, 1955, -44, 45, 0, 1109, -1, "", 1);// 321
			AddComplexComponent( (BaseAddon) this, 1955, -48, 45, 5, 1109, -1, "", 1);// 322
			AddComplexComponent( (BaseAddon) this, 1822, -48, 34, 22, 1109, -1, "", 1);// 323
			AddComplexComponent( (BaseAddon) this, 1822, -47, 34, 22, 1109, -1, "", 1);// 324
			AddComplexComponent( (BaseAddon) this, 1822, -45, 34, 22, 1109, -1, "", 1);// 325
			AddComplexComponent( (BaseAddon) this, 1822, -44, 34, 22, 1109, -1, "", 1);// 326
			AddComplexComponent( (BaseAddon) this, 1822, -42, 34, 22, 1109, -1, "", 1);// 327
			AddComplexComponent( (BaseAddon) this, 1822, -41, 34, 22, 1109, -1, "", 1);// 328
			AddComplexComponent( (BaseAddon) this, 1957, -44, 45, 5, 1109, -1, "", 1);// 329
			AddComplexComponent( (BaseAddon) this, 1957, -44, 46, 5, 1109, -1, "", 1);// 330
			AddComplexComponent( (BaseAddon) this, 1955, -46, 45, 0, 1109, -1, "", 1);// 331
			AddComplexComponent( (BaseAddon) this, 1955, -47, 44, 10, 1109, -1, "", 1);// 332
			AddComplexComponent( (BaseAddon) this, 1955, -47, 44, 0, 1109, -1, "", 1);// 333
			AddComplexComponent( (BaseAddon) this, 1822, -36, 39, 22, 1109, -1, "", 1);// 334
			AddComplexComponent( (BaseAddon) this, 1822, -36, 40, 22, 1109, -1, "", 1);// 335
			AddComplexComponent( (BaseAddon) this, 1955, -46, 46, 0, 1109, -1, "", 1);// 336
			AddComplexComponent( (BaseAddon) this, 1958, -48, 43, 10, 1109, -1, "", 1);// 337
			AddComplexComponent( (BaseAddon) this, 1955, -48, 44, 5, 1109, -1, "", 1);// 338
			AddComplexComponent( (BaseAddon) this, 14138, -46, 44, 0, 1109, -1, "", 1);// 339
			AddComplexComponent( (BaseAddon) this, 8611, -36, 43, 27, 0, -1, "Block Everything", 1);// 340
			AddComplexComponent( (BaseAddon) this, 8611, -36, 45, 27, 0, -1, "Block Everything", 1);// 341
			AddComplexComponent( (BaseAddon) this, 8611, -36, 46, 27, 0, -1, "Block Everything", 1);// 342
			AddComplexComponent( (BaseAddon) this, 1955, -46, 45, 5, 1109, -1, "", 1);// 343
			AddComplexComponent( (BaseAddon) this, 1955, -48, 43, 0, 1109, -1, "", 1);// 344
			AddComplexComponent( (BaseAddon) this, 1955, -47, 45, 0, 1109, -1, "", 1);// 345
			AddComplexComponent( (BaseAddon) this, 1822, -39, 34, 22, 1109, -1, "", 1);// 346
			AddComplexComponent( (BaseAddon) this, 1822, -38, 34, 22, 1109, -1, "", 1);// 347
			AddComplexComponent( (BaseAddon) this, 1822, -37, 35, 22, 1109, -1, "", 1);// 348
			AddComplexComponent( (BaseAddon) this, 1955, -47, 46, 0, 1109, -1, "", 1);// 349
			AddComplexComponent( (BaseAddon) this, 8611, -45, 34, 27, 0, -1, "Block Everything", 1);// 350
			AddComplexComponent( (BaseAddon) this, 8611, -44, 34, 27, 0, -1, "Block Everything", 1);// 351
			AddComplexComponent( (BaseAddon) this, 1955, -48, 44, 5, 1109, -1, "", 1);// 352
			AddComplexComponent( (BaseAddon) this, 1955, -45, 46, 0, 1109, -1, "", 1);// 353
			AddComplexComponent( (BaseAddon) this, 1955, -48, 43, 5, 1109, -1, "", 1);// 354
			AddComplexComponent( (BaseAddon) this, 1822, -36, 37, 22, 1109, -1, "", 1);// 359
			AddComplexComponent( (BaseAddon) this, 1958, -47, 43, 10, 1109, -1, "", 1);// 360
			AddComplexComponent( (BaseAddon) this, 1955, -45, 45, 0, 1109, -1, "", 1);// 361
			AddComplexComponent( (BaseAddon) this, 1955, -46, 45, 0, 1109, -1, "", 1);// 364
			AddComplexComponent( (BaseAddon) this, 1957, -45, 46, 10, 1109, -1, "", 1);// 365
			AddComplexComponent( (BaseAddon) this, 1955, -48, 44, 0, 1109, -1, "", 1);// 366
			AddComplexComponent( (BaseAddon) this, 8611, -48, 34, 27, 0, -1, "Block Everything", 1);// 367
			AddComplexComponent( (BaseAddon) this, 128, -37, 37, 0, 1109, -1, "", 1);// 368
			AddComplexComponent( (BaseAddon) this, 128, -47, 35, 0, 1109, -1, "", 1);// 369
			AddComplexComponent( (BaseAddon) this, 1957, -46, 45, 15, 1109, -1, "", 1);// 370
			AddComplexComponent( (BaseAddon) this, 1955, -47, 45, 5, 1109, -1, "", 1);// 371
			AddComplexComponent( (BaseAddon) this, 1955, -47, 44, 5, 1109, -1, "", 1);// 372
			AddComplexComponent( (BaseAddon) this, 1955, -45, 46, 5, 1109, -1, "", 1);// 373
			AddComplexComponent( (BaseAddon) this, 5671, -47, 46, 20, 1109, -1, "", 1);// 374
			AddComplexComponent( (BaseAddon) this, 1958, -47, 44, 15, 1109, -1, "", 1);// 375
			AddComplexComponent( (BaseAddon) this, 1955, -47, 44, 7, 1109, -1, "", 1);// 376
			AddComplexComponent( (BaseAddon) this, 1957, -45, 45, 10, 1109, -1, "", 1);// 377
			AddComplexComponent( (BaseAddon) this, 1955, -48, 42, 0, 1109, -1, "", 1);// 378
			AddComplexComponent( (BaseAddon) this, 1958, -47, 42, 5, 1109, -1, "", 1);// 379
			AddComplexComponent( (BaseAddon) this, 8611, -36, 39, 27, 0, -1, "Block Everything", 1);// 381
			AddComplexComponent( (BaseAddon) this, 8611, -36, 40, 27, 0, -1, "Block Everything", 1);// 382
			AddComplexComponent( (BaseAddon) this, 8611, -36, 42, 27, 0, -1, "Block Everything", 1);// 383
			AddComplexComponent( (BaseAddon) this, 1955, -46, 46, 10, 1109, -1, "", 1);// 384
			AddComplexComponent( (BaseAddon) this, 1955, -48, 44, 10, 1109, -1, "", 1);// 385
			AddComplexComponent( (BaseAddon) this, 128, -41, 35, 0, 1109, -1, "", 1);// 386
			AddComplexComponent( (BaseAddon) this, 128, -46, 35, 0, 1109, -1, "", 1);// 387
			AddComplexComponent( (BaseAddon) this, 128, -37, 41, 0, 1109, -1, "", 1);// 388
			AddComplexComponent( (BaseAddon) this, 128, -37, 42, 0, 1109, -1, "", 1);// 389
			AddComplexComponent( (BaseAddon) this, 128, -45, 35, 0, 1109, -1, "", 1);// 390
			AddComplexComponent( (BaseAddon) this, 128, -37, 44, 0, 1109, -1, "", 1);// 391
			AddComplexComponent( (BaseAddon) this, 128, -37, 38, 0, 1109, -1, "", 1);// 392
			AddComplexComponent( (BaseAddon) this, 128, -37, 36, 0, 1109, -1, "", 1);// 393
			AddComplexComponent( (BaseAddon) this, 128, -37, 45, 0, 1109, -1, "", 1);// 394
			AddComplexComponent( (BaseAddon) this, 1955, -47, 45, 10, 1109, -1, "", 1);// 395
			AddComplexComponent( (BaseAddon) this, 5671, -47, 46, 25, 1109, -1, "", 1);// 396
			AddComplexComponent( (BaseAddon) this, 1958, -48, 42, 5, 1109, -1, "", 1);// 397
			AddComplexComponent( (BaseAddon) this, 1955, -47, 43, 0, 1109, -1, "", 1);// 398
			AddComplexComponent( (BaseAddon) this, 1957, -46, 46, 15, 1109, -1, "", 1);// 399
			AddComplexComponent( (BaseAddon) this, 8611, -47, 34, 27, 0, -1, "Block Everything", 1);// 400
			AddComplexComponent( (BaseAddon) this, 1955, -46, 46, 5, 1109, -1, "", 1);// 401
			AddComplexComponent( (BaseAddon) this, 1955, -48, 46, 5, 1109, -1, "", 1);// 402
			AddComplexComponent( (BaseAddon) this, 1955, -44, 46, 0, 1109, -1, "", 1);// 403
			AddComplexComponent( (BaseAddon) this, 7774, -45, 36, 0, 0, -1, "CTF Score board", 1);// 408
			AddComplexComponent( (BaseAddon) this, 1822, -36, 42, 22, 1109, -1, "", 1);// 409
			AddComplexComponent( (BaseAddon) this, 1822, -36, 43, 22, 1109, -1, "", 1);// 410
			AddComplexComponent( (BaseAddon) this, 1955, -47, 44, 0, 1109, -1, "", 1);// 411
			AddComplexComponent( (BaseAddon) this, 1822, -36, 45, 22, 1109, -1, "", 1);// 412
			AddComplexComponent( (BaseAddon) this, 1958, -48, 44, 15, 1109, -1, "", 1);// 413
			AddComplexComponent( (BaseAddon) this, 7774, -41, 36, 0, 0, -1, "CTF Score board", 1);// 414
			AddComplexComponent( (BaseAddon) this, 8611, -37, 35, 27, 0, -1, "Block Everything", 1);// 415
			AddComplexComponent( (BaseAddon) this, 1955, -46, 46, 0, 1109, -1, "", 1);// 416
			AddComplexComponent( (BaseAddon) this, 128, -37, 40, 0, 1109, -1, "", 1);// 417
			AddComplexComponent( (BaseAddon) this, 1955, -46, 46, 5, 1109, -1, "", 1);// 418
			AddComplexComponent( (BaseAddon) this, 1958, -48, 41, 0, 1109, -1, "", 1);// 419
			AddComplexComponent( (BaseAddon) this, 1958, -47, 41, 0, 1109, -1, "", 1);// 420
			AddComplexComponent( (BaseAddon) this, 1955, -48, 45, 10, 1109, -1, "", 1);// 421
			AddComplexComponent( (BaseAddon) this, 1955, -48, 46, 10, 1109, -1, "", 1);// 422
			AddComplexComponent( (BaseAddon) this, 1955, -47, 46, 10, 1109, -1, "", 1);// 423
			AddComplexComponent( (BaseAddon) this, 1955, -48, 45, 15, 1109, -1, "", 1);// 424
			AddComplexComponent( (BaseAddon) this, 1955, -48, 46, 15, 1109, -1, "", 1);// 425
			AddComplexComponent( (BaseAddon) this, 1955, -47, 45, 15, 1109, -1, "", 1);// 426
			AddComplexComponent( (BaseAddon) this, 1955, -47, 46, 15, 1109, -1, "", 1);// 427
			AddComplexComponent( (BaseAddon) this, 1822, -36, 36, 22, 1109, -1, "", 1);// 430
			AddComplexComponent( (BaseAddon) this, 1955, -47, 46, 5, 1109, -1, "", 1);// 431
			AddComplexComponent( (BaseAddon) this, 1955, -47, 42, 0, 1109, -1, "", 1);// 432
			AddComplexComponent( (BaseAddon) this, 128, -42, 35, 0, 1109, -1, "", 1);// 433
			AddComplexComponent( (BaseAddon) this, 128, -44, 35, 0, 1109, -1, "", 1);// 434
			AddComplexComponent( (BaseAddon) this, 128, -38, 36, 0, 1109, -1, "", 1);// 435
			AddComplexComponent( (BaseAddon) this, 128, -38, 35, 0, 1109, -1, "", 1);// 436
			AddComplexComponent( (BaseAddon) this, 128, -39, 35, 0, 1109, -1, "", 1);// 437
			AddComplexComponent( (BaseAddon) this, 128, -40, 35, 0, 1109, -1, "", 1);// 438
			AddComplexComponent( (BaseAddon) this, 1955, -46, 45, 5, 1109, -1, "", 1);// 439
			AddComplexComponent( (BaseAddon) this, 1955, -46, 45, 10, 1109, -1, "", 1);// 440
			AddComplexComponent( (BaseAddon) this, 8611, -42, 34, 27, 0, -1, "Block Everything", 1);// 441
			AddComplexComponent( (BaseAddon) this, 8611, -41, 34, 27, 0, -1, "Block Everything", 1);// 442
			AddComplexComponent( (BaseAddon) this, 8611, -39, 34, 27, 0, -1, "Block Everything", 1);// 443
			AddComplexComponent( (BaseAddon) this, 8611, -38, 34, 27, 0, -1, "Block Everything", 1);// 444
			AddComplexComponent( (BaseAddon) this, 7107, -46, 44, 0, 1109, -1, "", 1);// 445
			AddComplexComponent( (BaseAddon) this, 128, -37, 46, 0, 1109, -1, "", 1);// 453
			AddComplexComponent( (BaseAddon) this, 1955, -45, 49, 5, 1109, -1, "", 1);// 456
			AddComplexComponent( (BaseAddon) this, 1955, -43, 49, 5, 1109, -1, "", 1);// 457
			AddComplexComponent( (BaseAddon) this, 1955, -44, 49, 5, 1109, -1, "", 1);// 458
			AddComplexComponent( (BaseAddon) this, 1955, -46, 49, 0, 1109, -1, "", 1);// 459
			AddComplexComponent( (BaseAddon) this, 1957, -43, 49, 0, 1109, -1, "", 1);// 460
			AddComplexComponent( (BaseAddon) this, 1957, -42, 49, 0, 1109, -1, "", 1);// 461
			AddComplexComponent( (BaseAddon) this, 1955, -47, 49, 0, 1109, -1, "", 1);// 462
			AddComplexComponent( (BaseAddon) this, 8611, -28, -1, 42, 0, -1, "Block Everything", 1);// 464
			AddComplexComponent( (BaseAddon) this, 8611, -30, -4, 42, 0, -1, "Block Everything", 1);// 465
			AddComplexComponent( (BaseAddon) this, 8611, -28, -4, 42, 0, -1, "Block Everything", 1);// 469
			AddComplexComponent( (BaseAddon) this, 8611, -28, -2, 42, 0, -1, "Block Everything", 1);// 470
			AddComplexComponent( (BaseAddon) this, 8611, -31, -2, 42, 0, -1, "Block Everything", 1);// 472
			AddComplexComponent( (BaseAddon) this, 8611, -31, -4, 42, 0, -1, "Block Everything", 1);// 473
			AddComplexComponent( (BaseAddon) this, 8611, -29, -4, 42, 0, -1, "Block Everything", 1);// 474
			AddComplexComponent( (BaseAddon) this, 8611, -31, -3, 42, 0, -1, "Block Everything", 1);// 475
			AddComplexComponent( (BaseAddon) this, 8611, -31, -1, 42, 0, -1, "Block Everything", 1);// 478
			AddComplexComponent( (BaseAddon) this, 8611, -28, -3, 42, 0, -1, "Block Everything", 1);// 480
			AddComplexComponent( (BaseAddon) this, 8611, -31, 1, 42, 0, -1, "Block Everything", 1);// 481
			AddComplexComponent( (BaseAddon) this, 8611, -29, 3, 42, 0, -1, "Block Everything", 1);// 482
			AddComplexComponent( (BaseAddon) this, 8611, -31, 3, 42, 0, -1, "Block Everything", 1);// 483
			AddComplexComponent( (BaseAddon) this, 8611, -31, 2, 42, 0, -1, "Block Everything", 1);// 484
			AddComplexComponent( (BaseAddon) this, 8611, -30, 3, 42, 0, -1, "Block Everything", 1);// 487
			AddComplexComponent( (BaseAddon) this, 8611, -28, 2, 42, 0, -1, "Block Everything", 1);// 488
			AddComplexComponent( (BaseAddon) this, 8611, -31, 0, 42, 0, -1, "Block Everything", 1);// 489
			AddComplexComponent( (BaseAddon) this, 8611, -28, 1, 42, 0, -1, "Block Everything", 1);// 490
			AddComplexComponent( (BaseAddon) this, 8611, -28, 3, 42, 0, -1, "Block Everything", 1);// 493
			AddComplexComponent( (BaseAddon) this, 8611, -28, 0, 42, 0, -1, "Block Everything", 1);// 494
			AddComplexComponent( (BaseAddon) this, 8611, -28, 45, 22, 0, -1, "Block Everything", 1);// 502
			AddComplexComponent( (BaseAddon) this, 8611, -28, 32, 22, 0, -1, "Block Everything", 1);// 503
			AddComplexComponent( (BaseAddon) this, 8611, -29, 33, 22, 0, -1, "Block Everything", 1);// 505
			AddComplexComponent( (BaseAddon) this, 8611, -27, 45, 22, 0, -1, "Block Everything", 1);// 508
			AddComplexComponent( (BaseAddon) this, 8611, -26, 45, 22, 0, -1, "Block Everything", 1);// 509
			AddComplexComponent( (BaseAddon) this, 8611, -26, 32, 22, 0, -1, "Block Everything", 1);// 510
			AddComplexComponent( (BaseAddon) this, 1305, -28, 37, 22, 1109, -1, "", 1);// 515
			AddComplexComponent( (BaseAddon) this, 8611, -22, 39, 22, 0, -1, "Block Everything", 1);// 516
			AddComplexComponent( (BaseAddon) this, 8611, -22, 33, 22, 0, -1, "Block Everything", 1);// 517
			AddComplexComponent( (BaseAddon) this, 8611, -22, 32, 22, 0, -1, "Block Everything", 1);// 528
			AddComplexComponent( (BaseAddon) this, 8611, -24, 32, 22, 0, -1, "Block Everything", 1);// 529
			AddComplexComponent( (BaseAddon) this, 1305, -28, 39, 22, 1109, -1, "", 1);// 531
			AddComplexComponent( (BaseAddon) this, 1305, -28, 41, 22, 1109, -1, "", 1);// 533
			AddComplexComponent( (BaseAddon) this, 8611, -23, 32, 22, 0, -1, "Block Everything", 1);// 534
			AddComplexComponent( (BaseAddon) this, 8611, -29, 34, 22, 0, -1, "Block Everything", 1);// 535
			AddComplexComponent( (BaseAddon) this, 8611, -29, 35, 22, 0, -1, "Block Everything", 1);// 536
			AddComplexComponent( (BaseAddon) this, 8611, -29, 36, 22, 0, -1, "Block Everything", 1);// 537
			AddComplexComponent( (BaseAddon) this, 8611, -29, 37, 22, 0, -1, "Block Everything", 1);// 538
			AddComplexComponent( (BaseAddon) this, 8611, -29, 38, 22, 0, -1, "Block Everything", 1);// 539
			AddComplexComponent( (BaseAddon) this, 8611, -29, 39, 22, 0, -1, "Block Everything", 1);// 540
			AddComplexComponent( (BaseAddon) this, 8611, -29, 40, 22, 0, -1, "Block Everything", 1);// 541
			AddComplexComponent( (BaseAddon) this, 8611, -29, 41, 22, 0, -1, "Block Everything", 1);// 542
			AddComplexComponent( (BaseAddon) this, 8611, -29, 42, 22, 0, -1, "Block Everything", 1);// 543
			AddComplexComponent( (BaseAddon) this, 8611, -29, 43, 22, 0, -1, "Block Everything", 1);// 544
			AddComplexComponent( (BaseAddon) this, 8611, -29, 44, 22, 0, -1, "Block Everything", 1);// 545
			AddComplexComponent( (BaseAddon) this, 8611, -29, 45, 22, 0, -1, "Block Everything", 1);// 546
			AddComplexComponent( (BaseAddon) this, 8611, -22, 40, 22, 0, -1, "Block Everything", 1);// 547
			AddComplexComponent( (BaseAddon) this, 8611, -22, 41, 22, 0, -1, "Block Everything", 1);// 548
			AddComplexComponent( (BaseAddon) this, 8611, -22, 42, 22, 0, -1, "Block Everything", 1);// 549
			AddComplexComponent( (BaseAddon) this, 8611, -22, 43, 22, 0, -1, "Block Everything", 1);// 550
			AddComplexComponent( (BaseAddon) this, 8611, -22, 44, 22, 0, -1, "Block Everything", 1);// 551
			AddComplexComponent( (BaseAddon) this, 8611, -22, 45, 22, 0, -1, "Block Everything", 1);// 552
			AddComplexComponent( (BaseAddon) this, 8611, -27, 32, 22, 0, -1, "Block Everything", 1);// 555
			AddComplexComponent( (BaseAddon) this, 8611, -25, 45, 22, 0, -1, "Block Everything", 1);// 556
			AddComplexComponent( (BaseAddon) this, 8611, -24, 45, 22, 0, -1, "Block Everything", 1);// 557
			AddComplexComponent( (BaseAddon) this, 8611, -23, 45, 22, 0, -1, "Block Everything", 1);// 558
			AddComplexComponent( (BaseAddon) this, 8611, -22, 34, 22, 0, -1, "Block Everything", 1);// 559
			AddComplexComponent( (BaseAddon) this, 8611, -22, 35, 22, 0, -1, "Block Everything", 1);// 560
			AddComplexComponent( (BaseAddon) this, 8611, -22, 36, 22, 0, -1, "Block Everything", 1);// 561
			AddComplexComponent( (BaseAddon) this, 8611, -22, 37, 22, 0, -1, "Block Everything", 1);// 562
			AddComplexComponent( (BaseAddon) this, 8611, -22, 38, 22, 0, -1, "Block Everything", 1);// 563
			AddComplexComponent( (BaseAddon) this, 3796, -25, 33, 22, 1109, -1, "Game Supply Stone", 1);// 588
			AddComplexComponent( (BaseAddon) this, 8611, -25, 32, 22, 0, -1, "Block Everything", 1);// 591
			AddComplexComponent( (BaseAddon) this, 8611, -29, 32, 22, 0, -1, "Block Everything", 1);// 600
			AddComplexComponent( (BaseAddon) this, 7885, -27, 33, 22, 1109, 29, "", 1);// 615
			AddComplexComponent( (BaseAddon) this, 63, -12, -40, 22, 1368, -1, "", 1);// 616
			AddComplexComponent( (BaseAddon) this, 63, -8, -34, 22, 1368, -1, "", 1);// 617
			AddComplexComponent( (BaseAddon) this, 63, -8, -37, 22, 1368, -1, "", 1);// 618
			AddComplexComponent( (BaseAddon) this, 63, -12, -39, 22, 1368, -1, "", 1);// 619
			AddComplexComponent( (BaseAddon) this, 63, -12, -37, 22, 1368, -1, "", 1);// 620
			AddComplexComponent( (BaseAddon) this, 8611, -12, -39, 22, 0, -1, "Block Everything", 1);// 621
			AddComplexComponent( (BaseAddon) this, 63, -4, -40, 22, 1368, -1, "", 1);// 622
			AddComplexComponent( (BaseAddon) this, 63, -8, -33, 22, 1368, -1, "", 1);// 623
			AddComplexComponent( (BaseAddon) this, 63, -4, -39, 22, 1368, -1, "", 1);// 624
			AddComplexComponent( (BaseAddon) this, 8610, -8, -36, 27, 0, -1, "no line of sight", 1);// 625
			AddComplexComponent( (BaseAddon) this, 8611, -8, -37, 22, 0, -1, "Block Everything", 1);// 626
			AddComplexComponent( (BaseAddon) this, 8610, -12, -37, 27, 0, -1, "no line of sight", 1);// 627
			AddComplexComponent( (BaseAddon) this, 8611, -12, -40, 22, 0, -1, "Block Everything", 1);// 628
			AddComplexComponent( (BaseAddon) this, 8611, -12, -37, 22, 0, -1, "Block Everything", 1);// 629
			AddComplexComponent( (BaseAddon) this, 8611, -4, -37, 22, 0, -1, "Block Everything", 1);// 630
			AddComplexComponent( (BaseAddon) this, 8610, -8, -37, 27, 0, -1, "no line of sight", 1);// 631
			AddComplexComponent( (BaseAddon) this, 8610, -4, -38, 27, 0, -1, "no line of sight", 1);// 632
			AddComplexComponent( (BaseAddon) this, 8610, -12, -38, 27, 0, -1, "no line of sight", 1);// 633
			AddComplexComponent( (BaseAddon) this, 63, -8, -36, 22, 1368, -1, "", 1);// 634
			AddComplexComponent( (BaseAddon) this, 63, -12, -38, 22, 1368, -1, "", 1);// 635
			AddComplexComponent( (BaseAddon) this, 63, -4, -38, 22, 1368, -1, "", 1);// 636
			AddComplexComponent( (BaseAddon) this, 63, -4, -37, 22, 1368, -1, "", 1);// 637
			AddComplexComponent( (BaseAddon) this, 63, -8, -35, 22, 1368, -1, "", 1);// 638
			AddComplexComponent( (BaseAddon) this, 8611, -4, -38, 22, 0, -1, "Block Everything", 1);// 639
			AddComplexComponent( (BaseAddon) this, 8611, -4, -39, 22, 0, -1, "Block Everything", 1);// 640
			AddComplexComponent( (BaseAddon) this, 8611, -4, -40, 22, 0, -1, "Block Everything", 1);// 641
			AddComplexComponent( (BaseAddon) this, 8611, -8, -33, 22, 0, -1, "Block Everything", 1);// 642
			AddComplexComponent( (BaseAddon) this, 8611, -8, -34, 22, 0, -1, "Block Everything", 1);// 643
			AddComplexComponent( (BaseAddon) this, 8611, -8, -35, 22, 0, -1, "Block Everything", 1);// 644
			AddComplexComponent( (BaseAddon) this, 8611, -8, -36, 22, 0, -1, "Block Everything", 1);// 645
			AddComplexComponent( (BaseAddon) this, 8610, -4, -37, 27, 0, -1, "no line of sight", 1);// 646
			AddComplexComponent( (BaseAddon) this, 8611, -12, -38, 22, 0, -1, "Block Everything", 1);// 647
			AddComplexComponent( (BaseAddon) this, 577, -4, -9, 44, 1367, -1, "", 1);// 648
			AddComplexComponent( (BaseAddon) this, 577, -5, -9, 44, 1367, -1, "", 1);// 649
			AddComplexComponent( (BaseAddon) this, 8611, -2, 32, 22, 0, -1, "Block Everything", 1);// 650
			AddComplexComponent( (BaseAddon) this, 8611, -2, 34, 22, 0, -1, "Block Everything", 1);// 651
			AddComplexComponent( (BaseAddon) this, 8610, -2, 34, 27, 0, -1, "no line of sight", 1);// 652
			AddComplexComponent( (BaseAddon) this, 8611, -2, 35, 22, 0, -1, "Block Everything", 1);// 653
			AddComplexComponent( (BaseAddon) this, 63, -2, 33, 22, 1365, -1, "", 1);// 654
			AddComplexComponent( (BaseAddon) this, 63, -2, 32, 22, 1365, -1, "", 1);// 655
			AddComplexComponent( (BaseAddon) this, 8610, -2, 35, 27, 0, -1, "no line of sight", 1);// 656
			AddComplexComponent( (BaseAddon) this, 8611, -2, 33, 22, 0, -1, "Block Everything", 1);// 657
			AddComplexComponent( (BaseAddon) this, 1305, -2, 32, 22, 1365, -1, "", 1);// 658
			AddComplexComponent( (BaseAddon) this, 63, -2, 35, 22, 1365, -1, "", 1);// 659
			AddComplexComponent( (BaseAddon) this, 63, -2, 34, 22, 1365, -1, "", 1);// 660
			AddComplexComponent( (BaseAddon) this, 63, 0, -35, 22, 1368, -1, "", 1);// 661
			AddComplexComponent( (BaseAddon) this, 63, 0, -34, 22, 1368, -1, "", 1);// 662
			AddComplexComponent( (BaseAddon) this, 63, 0, -33, 22, 1368, -1, "", 1);// 663
			AddComplexComponent( (BaseAddon) this, 8611, 0, -33, 22, 0, -1, "Block Everything", 1);// 664
			AddComplexComponent( (BaseAddon) this, 8611, 0, -35, 22, 0, -1, "Block Everything", 1);// 665
			AddComplexComponent( (BaseAddon) this, 8611, 0, -37, 22, 0, -1, "Block Everything", 1);// 666
			AddComplexComponent( (BaseAddon) this, 8611, 0, -36, 22, 0, -1, "Block Everything", 1);// 667
			AddComplexComponent( (BaseAddon) this, 8611, 0, -34, 22, 0, -1, "Block Everything", 1);// 668
			AddComplexComponent( (BaseAddon) this, 63, 0, -37, 22, 1368, -1, "", 1);// 669
			AddComplexComponent( (BaseAddon) this, 63, 0, -36, 22, 1368, -1, "", 1);// 670
			AddComplexComponent( (BaseAddon) this, 8610, 0, -36, 27, 0, -1, "no line of sight", 1);// 671
			AddComplexComponent( (BaseAddon) this, 8610, 0, -37, 27, 0, -1, "no line of sight", 1);// 672
			AddComplexComponent( (BaseAddon) this, 3804, 7, -24, 22, 0, -1, "Game Control Stone", 1);// 673
			AddComplexComponent( (BaseAddon) this, 7774, 0, -8, 43, 0, -1, "CTF Score board", 1);// 674
			AddComplexComponent( (BaseAddon) this, 7774, 2, -8, 43, 0, -1, "CTF Score board", 1);// 680
			AddComplexComponent( (BaseAddon) this, 3804, 4, -13, 66, 0, -1, "Game Control Stone", 1);// 681
			AddComplexComponent( (BaseAddon) this, 8611, 4, -11, 66, 0, -1, "Block Everything", 1);// 682
			AddComplexComponent( (BaseAddon) this, 8611, 6, -11, 66, 0, -1, "Block Everything", 1);// 683
			AddComplexComponent( (BaseAddon) this, 8611, 3, -11, 66, 0, -1, "Block Everything", 1);// 684
			AddComplexComponent( (BaseAddon) this, 8611, 3, -13, 66, 0, -1, "Block Everything", 1);// 686
			AddComplexComponent( (BaseAddon) this, 8611, 3, -12, 66, 0, -1, "Block Everything", 1);// 687
			AddComplexComponent( (BaseAddon) this, 8611, 5, -11, 66, 0, -1, "Block Everything", 1);// 692
			AddComplexComponent( (BaseAddon) this, 3948, 5, -13, 66, 0, 29, "", 1);// 694
			AddComplexComponent( (BaseAddon) this, 577, 4, 7, 44, 1365, -1, "", 1);// 695
			AddComplexComponent( (BaseAddon) this, 577, 3, 7, 44, 1365, -1, "", 1);// 696
			AddComplexComponent( (BaseAddon) this, 63, 10, 39, 22, 1365, -1, "", 1);// 697
			AddComplexComponent( (BaseAddon) this, 63, 2, 39, 22, 1365, -1, "", 1);// 698
			AddComplexComponent( (BaseAddon) this, 8611, 6, 35, 22, 0, -1, "Block Everything", 1);// 699
			AddComplexComponent( (BaseAddon) this, 8610, 2, 35, 27, 0, -1, "no line of sight", 1);// 700
			AddComplexComponent( (BaseAddon) this, 8611, 10, 36, 22, 0, -1, "Block Everything", 1);// 701
			AddComplexComponent( (BaseAddon) this, 8611, 2, 38, 22, 0, -1, "Block Everything", 1);// 702
			AddComplexComponent( (BaseAddon) this, 8611, 10, 38, 22, 0, -1, "Block Everything", 1);// 703
			AddComplexComponent( (BaseAddon) this, 63, 2, 36, 22, 1365, -1, "", 1);// 704
			AddComplexComponent( (BaseAddon) this, 8611, 6, 33, 22, 0, -1, "Block Everything", 1);// 705
			AddComplexComponent( (BaseAddon) this, 63, 6, 33, 22, 1365, -1, "", 1);// 706
			AddComplexComponent( (BaseAddon) this, 63, 6, 35, 22, 1365, -1, "", 1);// 707
			AddComplexComponent( (BaseAddon) this, 8611, 6, 34, 22, 0, -1, "Block Everything", 1);// 708
			AddComplexComponent( (BaseAddon) this, 8611, 2, 36, 22, 0, -1, "Block Everything", 1);// 709
			AddComplexComponent( (BaseAddon) this, 8611, 6, 32, 22, 0, -1, "Block Everything", 1);// 710
			AddComplexComponent( (BaseAddon) this, 63, 10, 35, 22, 1365, -1, "", 1);// 711
			AddComplexComponent( (BaseAddon) this, 8610, 6, 35, 27, 0, -1, "no line of sight", 1);// 712
			AddComplexComponent( (BaseAddon) this, 63, 2, 38, 22, 1365, -1, "", 1);// 713
			AddComplexComponent( (BaseAddon) this, 8611, 2, 37, 22, 0, -1, "Block Everything", 1);// 714
			AddComplexComponent( (BaseAddon) this, 63, 10, 37, 22, 1365, -1, "", 1);// 715
			AddComplexComponent( (BaseAddon) this, 8610, 10, 35, 27, 0, -1, "no line of sight", 1);// 716
			AddComplexComponent( (BaseAddon) this, 8611, 2, 35, 22, 0, -1, "Block Everything", 1);// 717
			AddComplexComponent( (BaseAddon) this, 63, 6, 32, 22, 1365, -1, "", 1);// 718
			AddComplexComponent( (BaseAddon) this, 8611, 10, 37, 22, 0, -1, "Block Everything", 1);// 719
			AddComplexComponent( (BaseAddon) this, 63, 2, 37, 22, 1365, -1, "", 1);// 720
			AddComplexComponent( (BaseAddon) this, 8611, 10, 39, 22, 0, -1, "Block Everything", 1);// 721
			AddComplexComponent( (BaseAddon) this, 8611, 10, 35, 22, 0, -1, "Block Everything", 1);// 722
			AddComplexComponent( (BaseAddon) this, 8611, 2, 39, 22, 0, -1, "Block Everything", 1);// 723
			AddComplexComponent( (BaseAddon) this, 63, 2, 35, 22, 1365, -1, "", 1);// 724
			AddComplexComponent( (BaseAddon) this, 63, 10, 36, 22, 1365, -1, "", 1);// 725
			AddComplexComponent( (BaseAddon) this, 8610, 10, 36, 27, 0, -1, "no line of sight", 1);// 726
			AddComplexComponent( (BaseAddon) this, 8610, 2, 36, 27, 0, -1, "no line of sight", 1);// 727
			AddComplexComponent( (BaseAddon) this, 63, 10, 38, 22, 1365, -1, "", 1);// 728
			AddComplexComponent( (BaseAddon) this, 63, 6, 34, 22, 1365, -1, "", 1);// 729
			AddComplexComponent( (BaseAddon) this, 8610, 6, 34, 27, 0, -1, "no line of sight", 1);// 730
			AddComplexComponent( (BaseAddon) this, 8611, 25, -34, 22, 0, -1, "Block Everything", 1);// 731
			AddComplexComponent( (BaseAddon) this, 8611, 27, -45, 22, 0, -1, "Block Everything", 1);// 732
			AddComplexComponent( (BaseAddon) this, 8611, 28, -34, 22, 0, -1, "Block Everything", 1);// 733
			AddComplexComponent( (BaseAddon) this, 8611, 21, -35, 22, 0, -1, "Block Everything", 1);// 734
			AddComplexComponent( (BaseAddon) this, 8611, 28, -45, 22, 0, -1, "Block Everything", 1);// 735
			AddComplexComponent( (BaseAddon) this, 8611, 26, -45, 22, 0, -1, "Block Everything", 1);// 736
			AddComplexComponent( (BaseAddon) this, 8611, 28, -36, 22, 0, -1, "Block Everything", 1);// 738
			AddComplexComponent( (BaseAddon) this, 8611, 21, -40, 22, 0, -1, "Block Everything", 1);// 739
			AddComplexComponent( (BaseAddon) this, 8611, 28, -37, 22, 0, -1, "Block Everything", 1);// 745
			AddComplexComponent( (BaseAddon) this, 8611, 21, -45, 22, 0, -1, "Block Everything", 1);// 747
			AddComplexComponent( (BaseAddon) this, 1305, 27, -42, 22, 1645, -1, "", 1);// 752
			AddComplexComponent( (BaseAddon) this, 8611, 21, -41, 22, 0, -1, "Block Everything", 1);// 754
			AddComplexComponent( (BaseAddon) this, 8611, 28, -43, 22, 0, -1, "Block Everything", 1);// 755
			AddComplexComponent( (BaseAddon) this, 8611, 27, -34, 22, 0, -1, "Block Everything", 1);// 758
			AddComplexComponent( (BaseAddon) this, 8611, 28, -38, 22, 0, -1, "Block Everything", 1);// 759
			AddComplexComponent( (BaseAddon) this, 8611, 21, -39, 22, 0, -1, "Block Everything", 1);// 760
			AddComplexComponent( (BaseAddon) this, 8611, 28, -44, 22, 0, -1, "Block Everything", 1);// 761
			AddComplexComponent( (BaseAddon) this, 8611, 24, -34, 22, 0, -1, "Block Everything", 1);// 762
			AddComplexComponent( (BaseAddon) this, 8611, 21, -35, 22, 0, -1, "Block Everything", 1);// 763
			AddComplexComponent( (BaseAddon) this, 8611, 28, -39, 22, 0, -1, "Block Everything", 1);// 764
			AddComplexComponent( (BaseAddon) this, 3796, 25, -44, 22, 1645, -1, "Game Supply Stone", 1);// 766
			AddComplexComponent( (BaseAddon) this, 1305, 27, -38, 22, 1645, -1, "", 1);// 775
			AddComplexComponent( (BaseAddon) this, 8611, 28, -41, 22, 0, -1, "Block Everything", 1);// 777
			AddComplexComponent( (BaseAddon) this, 8611, 24, -45, 22, 0, -1, "Block Everything", 1);// 778
			AddComplexComponent( (BaseAddon) this, 8611, 28, -35, 22, 0, -1, "Block Everything", 1);// 780
			AddComplexComponent( (BaseAddon) this, 8611, 28, -42, 22, 0, -1, "Block Everything", 1);// 781
			AddComplexComponent( (BaseAddon) this, 8611, 21, -36, 22, 0, -1, "Block Everything", 1);// 782
			AddComplexComponent( (BaseAddon) this, 8611, 21, -34, 22, 0, -1, "Block Everything", 1);// 784
			AddComplexComponent( (BaseAddon) this, 8611, 21, -43, 22, 0, -1, "Block Everything", 1);// 785
			AddComplexComponent( (BaseAddon) this, 8611, 21, -38, 22, 0, -1, "Block Everything", 1);// 796
			AddComplexComponent( (BaseAddon) this, 8611, 21, -42, 22, 0, -1, "Block Everything", 1);// 799
			AddComplexComponent( (BaseAddon) this, 8611, 28, -40, 22, 0, -1, "Block Everything", 1);// 801
			AddComplexComponent( (BaseAddon) this, 8611, 26, -34, 22, 0, -1, "Block Everything", 1);// 809
			AddComplexComponent( (BaseAddon) this, 8611, 22, -45, 22, 0, -1, "Block Everything", 1);// 811
			AddComplexComponent( (BaseAddon) this, 1305, 27, -40, 22, 1645, -1, "", 1);// 813
			AddComplexComponent( (BaseAddon) this, 8611, 23, -45, 22, 0, -1, "Block Everything", 1);// 814
			AddComplexComponent( (BaseAddon) this, 8611, 21, -37, 22, 0, -1, "Block Everything", 1);// 818
			AddComplexComponent( (BaseAddon) this, 8611, 25, -45, 22, 0, -1, "Block Everything", 1);// 819
			AddComplexComponent( (BaseAddon) this, 8611, 23, -34, 22, 0, -1, "Block Everything", 1);// 821
			AddComplexComponent( (BaseAddon) this, 8611, 22, -34, 22, 0, -1, "Block Everything", 1);// 825
			AddComplexComponent( (BaseAddon) this, 8611, 21, -44, 22, 0, -1, "Block Everything", 1);// 828
			AddComplexComponent( (BaseAddon) this, 7885, 23, -44, 22, 1645, 29, "", 1);// 832
			AddComplexComponent( (BaseAddon) this, 8611, 28, -2, 42, 0, -1, "Block Everything", 1);// 836
			AddComplexComponent( (BaseAddon) this, 8611, 28, -4, 42, 0, -1, "Block Everything", 1);// 838
			AddComplexComponent( (BaseAddon) this, 8611, 25, -4, 42, 0, -1, "Block Everything", 1);// 841
			AddComplexComponent( (BaseAddon) this, 8611, 25, -2, 42, 0, -1, "Block Everything", 1);// 842
			AddComplexComponent( (BaseAddon) this, 8611, 26, -5, 42, 0, -1, "Block Everything", 1);// 844
			AddComplexComponent( (BaseAddon) this, 8611, 25, -1, 42, 0, -1, "Block Everything", 1);// 852
			AddComplexComponent( (BaseAddon) this, 8611, 28, -3, 42, 0, -1, "Block Everything", 1);// 856
			AddComplexComponent( (BaseAddon) this, 8611, 25, -5, 42, 0, -1, "Block Everything", 1);// 858
			AddComplexComponent( (BaseAddon) this, 8611, 27, -5, 42, 0, -1, "Block Everything", 1);// 859
			AddComplexComponent( (BaseAddon) this, 8611, 28, -1, 42, 0, -1, "Block Everything", 1);// 861
			AddComplexComponent( (BaseAddon) this, 8611, 28, -5, 42, 0, -1, "Block Everything", 1);// 862
			AddComplexComponent( (BaseAddon) this, 8611, 25, -3, 42, 0, -1, "Block Everything", 1);// 864
			AddComplexComponent( (BaseAddon) this, 8611, 26, 0, 42, 0, -1, "Block Everything", 1);// 865
			AddComplexComponent( (BaseAddon) this, 8611, 27, 0, 42, 0, -1, "Block Everything", 1);// 866
			AddComplexComponent( (BaseAddon) this, 8611, 25, 0, 42, 0, -1, "Block Everything", 1);// 868
			AddComplexComponent( (BaseAddon) this, 8611, 28, 0, 42, 0, -1, "Block Everything", 1);// 872
			AddComplexComponent( (BaseAddon) this, 8610, 31, 41, 22, 0, -1, "no line of sight", 1);// 873
			AddComplexComponent( (BaseAddon) this, 8610, 31, 39, 22, 0, -1, "no line of sight", 1);// 874
			AddComplexComponent( (BaseAddon) this, 8610, 31, 34, 22, 0, -1, "no line of sight", 1);// 875
			AddComplexComponent( (BaseAddon) this, 8610, 31, 33, 22, 0, -1, "no line of sight", 1);// 876
			AddComplexComponent( (BaseAddon) this, 8610, 31, 38, 22, 0, -1, "no line of sight", 1);// 877
			AddComplexComponent( (BaseAddon) this, 8610, 31, 40, 22, 0, -1, "no line of sight", 1);// 878
			AddComplexComponent( (BaseAddon) this, 8610, 31, 37, 22, 0, -1, "no line of sight", 1);// 879
			AddComplexComponent( (BaseAddon) this, 8610, 31, 32, 22, 0, -1, "no line of sight", 1);// 880
			AddComplexComponent( (BaseAddon) this, 8611, 46, -36, 27, 0, -1, "Block Everything", 1);// 881
			AddComplexComponent( (BaseAddon) this, 1955, 46, -48, 15, 1645, -1, "", 1);// 882
			AddComplexComponent( (BaseAddon) this, 1955, 46, -46, 5, 1645, -1, "", 1);// 883
			AddComplexComponent( (BaseAddon) this, 1955, 45, -46, 5, 1645, -1, "", 1);// 891
			AddComplexComponent( (BaseAddon) this, 8611, 34, -41, 27, 0, -1, "Block Everything", 1);// 892
			AddComplexComponent( (BaseAddon) this, 8611, 35, -48, 27, 0, -1, "Block Everything", 1);// 893
			AddComplexComponent( (BaseAddon) this, 1822, 46, -36, 22, 1645, -1, "", 1);// 894
			AddComplexComponent( (BaseAddon) this, 1955, 45, -48, 5, 1645, -1, "", 1);// 895
			AddComplexComponent( (BaseAddon) this, 1959, 43, -48, 10, 1645, -1, "", 1);// 896
			AddComplexComponent( (BaseAddon) this, 5671, 46, -48, 20, 1645, -1, "", 1);// 897
			AddComplexComponent( (BaseAddon) this, 1959, 41, -48, 0, 1645, -1, "", 1);// 899
			AddComplexComponent( (BaseAddon) this, 8610, 39, -33, 22, 0, -1, "no line of sight", 1);// 902
			AddComplexComponent( (BaseAddon) this, 1955, 44, -48, 10, 1645, -1, "", 1);// 903
			AddComplexComponent( (BaseAddon) this, 5671, 45, -48, 20, 1645, -1, "", 1);// 904
			AddComplexComponent( (BaseAddon) this, 1956, 45, -44, 5, 1645, -1, "", 1);// 905
			AddComplexComponent( (BaseAddon) this, 1955, 45, -45, 5, 1645, -1, "", 1);// 906
			AddComplexComponent( (BaseAddon) this, 1956, 45, -45, 10, 1645, -1, "", 1);// 907
			AddComplexComponent( (BaseAddon) this, 1955, 43, -47, 0, 1645, -1, "", 1);// 908
			AddComplexComponent( (BaseAddon) this, 1955, 43, -48, 0, 1645, -1, "", 1);// 909
			AddComplexComponent( (BaseAddon) this, 8611, 35, -37, 27, 0, -1, "Block Everything", 1);// 910
			AddComplexComponent( (BaseAddon) this, 8611, 34, -47, 27, 0, -1, "Block Everything", 1);// 911
			AddComplexComponent( (BaseAddon) this, 1822, 45, -36, 22, 1645, -1, "", 1);// 912
			AddComplexComponent( (BaseAddon) this, 8611, 34, -45, 27, 0, -1, "Block Everything", 1);// 913
			AddComplexComponent( (BaseAddon) this, 8611, 34, -44, 27, 0, -1, "Block Everything", 1);// 914
			AddComplexComponent( (BaseAddon) this, 1956, 46, -44, 5, 1645, -1, "", 1);// 917
			AddComplexComponent( (BaseAddon) this, 1956, 46, -43, 0, 1645, -1, "", 1);// 918
			AddComplexComponent( (BaseAddon) this, 1822, 34, -47, 22, 1645, -1, "", 1);// 919
			AddComplexComponent( (BaseAddon) this, 1822, 40, -36, 22, 1645, -1, "", 1);// 920
			AddComplexComponent( (BaseAddon) this, 1822, 42, -36, 22, 1645, -1, "", 1);// 921
			AddComplexComponent( (BaseAddon) this, 1822, 43, -36, 22, 1645, -1, "", 1);// 922
			AddComplexComponent( (BaseAddon) this, 1955, 45, -47, 0, 1645, -1, "", 1);// 923
			AddComplexComponent( (BaseAddon) this, 128, 38, -37, 0, 1645, -1, "", 1);// 924
			AddComplexComponent( (BaseAddon) this, 1955, 46, -47, 0, 1645, -1, "", 1);// 925
			AddComplexComponent( (BaseAddon) this, 1955, 46, -48, 0, 1645, -1, "", 1);// 926
			AddComplexComponent( (BaseAddon) this, 1959, 43, -47, 10, 1645, -1, "", 1);// 927
			AddComplexComponent( (BaseAddon) this, 1959, 44, -48, 15, 1645, -1, "", 1);// 928
			AddComplexComponent( (BaseAddon) this, 1959, 44, -47, 15, 1645, -1, "", 1);// 929
			AddComplexComponent( (BaseAddon) this, 1959, 42, -48, 5, 1645, -1, "", 1);// 930
			AddComplexComponent( (BaseAddon) this, 1959, 42, -47, 5, 1645, -1, "", 1);// 931
			AddComplexComponent( (BaseAddon) this, 1959, 41, -47, 0, 1645, -1, "", 1);// 932
			AddComplexComponent( (BaseAddon) this, 8611, 37, -36, 27, 0, -1, "Block Everything", 1);// 933
			AddComplexComponent( (BaseAddon) this, 8611, 34, -42, 27, 0, -1, "Block Everything", 1);// 934
			AddComplexComponent( (BaseAddon) this, 1955, 45, -47, 5, 1645, -1, "", 1);// 935
			AddComplexComponent( (BaseAddon) this, 128, 35, -48, 0, 1645, -1, "", 1);// 936
			AddComplexComponent( (BaseAddon) this, 128, 35, -47, 0, 1645, -1, "", 1);// 937
			AddComplexComponent( (BaseAddon) this, 128, 35, -46, 0, 1645, -1, "", 1);// 938
			AddComplexComponent( (BaseAddon) this, 128, 35, -44, 0, 1645, -1, "", 1);// 939
			AddComplexComponent( (BaseAddon) this, 128, 35, -43, 0, 1645, -1, "", 1);// 940
			AddComplexComponent( (BaseAddon) this, 128, 35, -40, 0, 1645, -1, "", 1);// 941
			AddComplexComponent( (BaseAddon) this, 128, 35, -39, 0, 1645, -1, "", 1);// 942
			AddComplexComponent( (BaseAddon) this, 128, 35, -38, 0, 1645, -1, "", 1);// 944
			AddComplexComponent( (BaseAddon) this, 1955, 44, -48, 5, 1645, -1, "", 1);// 945
			AddComplexComponent( (BaseAddon) this, 8610, 38, -33, 22, 0, -1, "no line of sight", 1);// 949
			AddComplexComponent( (BaseAddon) this, 1955, 45, -46, 10, 1645, -1, "", 1);// 950
			AddComplexComponent( (BaseAddon) this, 1955, 46, -48, 10, 1645, -1, "", 1);// 951
			AddComplexComponent( (BaseAddon) this, 1955, 46, -47, 10, 1645, -1, "", 1);// 952
			AddComplexComponent( (BaseAddon) this, 1955, 45, -47, 10, 1645, -1, "", 1);// 953
			AddComplexComponent( (BaseAddon) this, 1955, 45, -48, 10, 1645, -1, "", 1);// 954
			AddComplexComponent( (BaseAddon) this, 128, 37, -37, 0, 1645, -1, "", 1);// 955
			AddComplexComponent( (BaseAddon) this, 1955, 45, -46, 0, 1645, -1, "", 1);// 956
			AddComplexComponent( (BaseAddon) this, 1955, 45, -45, 0, 1645, -1, "", 1);// 957
			AddComplexComponent( (BaseAddon) this, 128, 36, -37, 0, 1645, -1, "", 1);// 958
			AddComplexComponent( (BaseAddon) this, 1822, 39, -36, 22, 1645, -1, "", 1);// 959
			AddComplexComponent( (BaseAddon) this, 128, 40, -37, 0, 1645, -1, "", 1);// 960
			AddComplexComponent( (BaseAddon) this, 1955, 46, -45, 0, 1645, -1, "", 1);// 961
			AddComplexComponent( (BaseAddon) this, 1955, 43, -47, 5, 1645, -1, "", 1);// 962
			AddComplexComponent( (BaseAddon) this, 128, 41, -37, 0, 1645, -1, "", 1);// 963
			AddComplexComponent( (BaseAddon) this, 1955, 46, -47, 15, 1645, -1, "", 1);// 964
			AddComplexComponent( (BaseAddon) this, 1955, 44, -47, 0, 1645, -1, "", 1);// 965
			AddComplexComponent( (BaseAddon) this, 128, 43, -37, 0, 1645, -1, "", 1);// 966
			AddComplexComponent( (BaseAddon) this, 1955, 42, -47, 0, 1645, -1, "", 1);// 967
			AddComplexComponent( (BaseAddon) this, 1955, 45, -48, 0, 1645, -1, "", 1);// 968
			AddComplexComponent( (BaseAddon) this, 128, 39, -37, 0, 1645, -1, "", 1);// 969
			AddComplexComponent( (BaseAddon) this, 1955, 46, -46, 0, 1645, -1, "", 1);// 970
			AddComplexComponent( (BaseAddon) this, 1955, 46, -47, 5, 1645, -1, "", 1);// 971
			AddComplexComponent( (BaseAddon) this, 8610, 33, -33, 22, 0, -1, "no line of sight", 1);// 972
			AddComplexComponent( (BaseAddon) this, 8611, 34, -38, 27, 0, -1, "Block Everything", 1);// 973
			AddComplexComponent( (BaseAddon) this, 8611, 36, -36, 27, 0, -1, "Block Everything", 1);// 974
			AddComplexComponent( (BaseAddon) this, 1955, 44, -48, 0, 1645, -1, "", 1);// 975
			AddComplexComponent( (BaseAddon) this, 1955, 46, -48, 5, 1645, -1, "", 1);// 976
			AddComplexComponent( (BaseAddon) this, 128, 44, -37, 0, 1645, -1, "", 1);// 977
			AddComplexComponent( (BaseAddon) this, 8611, 45, -36, 27, 0, -1, "Block Everything", 1);// 978
			AddComplexComponent( (BaseAddon) this, 1956, 45, -43, 0, 1645, -1, "", 1);// 979
			AddComplexComponent( (BaseAddon) this, 1956, 46, -45, 10, 1645, -1, "", 1);// 980
			AddComplexComponent( (BaseAddon) this, 1822, 37, -36, 22, 1645, -1, "", 1);// 982
			AddComplexComponent( (BaseAddon) this, 8611, 34, -48, 27, 0, -1, "Block Everything", 1);// 984
			AddComplexComponent( (BaseAddon) this, 1822, 35, -37, 22, 1645, -1, "", 1);// 985
			AddComplexComponent( (BaseAddon) this, 1822, 36, -36, 22, 1645, -1, "", 1);// 986
			AddComplexComponent( (BaseAddon) this, 1955, 46, -46, 10, 1645, -1, "", 1);// 987
			AddComplexComponent( (BaseAddon) this, 8611, 39, -36, 27, 0, -1, "Block Everything", 1);// 988
			AddComplexComponent( (BaseAddon) this, 8611, 40, -36, 27, 0, -1, "Block Everything", 1);// 989
			AddComplexComponent( (BaseAddon) this, 8611, 42, -36, 27, 0, -1, "Block Everything", 1);// 990
			AddComplexComponent( (BaseAddon) this, 8611, 43, -36, 27, 0, -1, "Block Everything", 1);// 991
			AddComplexComponent( (BaseAddon) this, 1955, 43, -48, 5, 1645, -1, "", 1);// 993
			AddComplexComponent( (BaseAddon) this, 128, 45, -37, 0, 1645, -1, "", 1);// 994
			AddComplexComponent( (BaseAddon) this, 7774, 38, -36, 0, 0, -1, "CTF Score board", 1);// 995
			AddComplexComponent( (BaseAddon) this, 8610, 37, -33, 22, 0, -1, "no line of sight", 1);// 996
			AddComplexComponent( (BaseAddon) this, 8610, 32, -33, 22, 0, -1, "no line of sight", 1);// 997
			AddComplexComponent( (BaseAddon) this, 8610, 34, -33, 22, 0, -1, "no line of sight", 1);// 998
			AddComplexComponent( (BaseAddon) this, 1955, 42, -48, 0, 1645, -1, "", 1);// 999
			AddComplexComponent( (BaseAddon) this, 1955, 46, -45, 5, 1645, -1, "", 1);// 1000
			AddComplexComponent( (BaseAddon) this, 1955, 45, -44, 0, 1645, -1, "", 1);// 1002
			AddComplexComponent( (BaseAddon) this, 1955, 45, -48, 15, 1645, -1, "", 1);// 1003
			AddComplexComponent( (BaseAddon) this, 1955, 45, -47, 15, 1645, -1, "", 1);// 1004
			AddComplexComponent( (BaseAddon) this, 8611, 34, -39, 27, 0, -1, "Block Everything", 1);// 1005
			AddComplexComponent( (BaseAddon) this, 1955, 46, -44, 0, 1645, -1, "", 1);// 1006
			AddComplexComponent( (BaseAddon) this, 1956, 46, -46, 15, 1645, -1, "", 1);// 1007
			AddComplexComponent( (BaseAddon) this, 1956, 45, -46, 15, 1645, -1, "", 1);// 1008
			AddComplexComponent( (BaseAddon) this, 1822, 34, -48, 22, 1645, -1, "", 1);// 1009
			AddComplexComponent( (BaseAddon) this, 1822, 34, -45, 22, 1645, -1, "", 1);// 1010
			AddComplexComponent( (BaseAddon) this, 7774, 38, -48, 0, 0, -1, "CTF Score board", 1);// 1011
			AddComplexComponent( (BaseAddon) this, 14138, 44, -46, 0, 1645, -1, "", 1);// 1012
			AddComplexComponent( (BaseAddon) this, 1822, 34, -44, 22, 1645, -1, "", 1);// 1013
			AddComplexComponent( (BaseAddon) this, 1822, 34, -42, 22, 1645, -1, "", 1);// 1014
			AddComplexComponent( (BaseAddon) this, 1822, 34, -41, 22, 1645, -1, "", 1);// 1015
			AddComplexComponent( (BaseAddon) this, 128, 35, -41, 0, 1645, -1, "", 1);// 1016
			AddComplexComponent( (BaseAddon) this, 1822, 34, -39, 22, 1645, -1, "", 1);// 1017
			AddComplexComponent( (BaseAddon) this, 1822, 34, -38, 22, 1645, -1, "", 1);// 1018
			AddComplexComponent( (BaseAddon) this, 1822, 35, -48, 22, 1645, -1, "", 1);// 1020
			AddComplexComponent( (BaseAddon) this, 8611, 35, -12, 22, 0, -1, "Block Everything", 1);// 1024
			AddComplexComponent( (BaseAddon) this, 8611, 36, -12, 22, 0, -1, "Block Everything", 1);// 1025
			AddComplexComponent( (BaseAddon) this, 8611, 37, -12, 22, 0, -1, "Block Everything", 1);// 1026
			AddComplexComponent( (BaseAddon) this, 8611, 38, -12, 22, 0, -1, "Block Everything", 1);// 1027
			AddComplexComponent( (BaseAddon) this, 8611, 39, -12, 22, 0, -1, "Block Everything", 1);// 1028
			AddComplexComponent( (BaseAddon) this, 62, 32, -8, 22, 1645, -1, "", 1);// 1029
			AddComplexComponent( (BaseAddon) this, 62, 33, -8, 22, 1645, -1, "", 1);// 1030
			AddComplexComponent( (BaseAddon) this, 62, 34, -8, 22, 1645, -1, "", 1);// 1031
			AddComplexComponent( (BaseAddon) this, 62, 35, -8, 22, 1645, -1, "", 1);// 1032
			AddComplexComponent( (BaseAddon) this, 62, 35, -4, 22, 1645, -1, "", 1);// 1033
			AddComplexComponent( (BaseAddon) this, 62, 36, -12, 22, 1645, -1, "", 1);// 1034
			AddComplexComponent( (BaseAddon) this, 8611, 34, -8, 22, 0, -1, "Block Everything", 1);// 1035
			AddComplexComponent( (BaseAddon) this, 8611, 35, -8, 22, 0, -1, "Block Everything", 1);// 1036
			AddComplexComponent( (BaseAddon) this, 8611, 32, -8, 22, 0, -1, "Block Everything", 1);// 1037
			AddComplexComponent( (BaseAddon) this, 8611, 35, -4, 22, 0, -1, "Block Everything", 1);// 1038
			AddComplexComponent( (BaseAddon) this, 8611, 36, -4, 22, 0, -1, "Block Everything", 1);// 1039
			AddComplexComponent( (BaseAddon) this, 8611, 37, -4, 22, 0, -1, "Block Everything", 1);// 1040
			AddComplexComponent( (BaseAddon) this, 8611, 38, -4, 22, 0, -1, "Block Everything", 1);// 1041
			AddComplexComponent( (BaseAddon) this, 8611, 39, -4, 22, 0, -1, "Block Everything", 1);// 1042
			AddComplexComponent( (BaseAddon) this, 8610, 36, -12, 32, 0, -1, "no line of sight", 1);// 1043
			AddComplexComponent( (BaseAddon) this, 8610, 34, -8, 32, 0, -1, "no line of sight", 1);// 1044
			AddComplexComponent( (BaseAddon) this, 8610, 36, -4, 32, 0, -1, "no line of sight", 1);// 1045
			AddComplexComponent( (BaseAddon) this, 8610, 35, -8, 32, 0, -1, "no line of sight", 1);// 1046
			AddComplexComponent( (BaseAddon) this, 62, 39, -12, 22, 1645, -1, "", 1);// 1047
			AddComplexComponent( (BaseAddon) this, 62, 38, -4, 22, 1645, -1, "", 1);// 1048
			AddComplexComponent( (BaseAddon) this, 62, 39, -4, 22, 1645, -1, "", 1);// 1049
			AddComplexComponent( (BaseAddon) this, 8610, 35, -4, 32, 0, -1, "no line of sight", 1);// 1050
			AddComplexComponent( (BaseAddon) this, 62, 38, -12, 22, 1645, -1, "", 1);// 1051
			AddComplexComponent( (BaseAddon) this, 62, 37, -12, 22, 1645, -1, "", 1);// 1052
			AddComplexComponent( (BaseAddon) this, 62, 35, -12, 22, 1645, -1, "", 1);// 1053
			AddComplexComponent( (BaseAddon) this, 8610, 35, -12, 32, 0, -1, "no line of sight", 1);// 1054
			AddComplexComponent( (BaseAddon) this, 8611, 33, -8, 22, 0, -1, "Block Everything", 1);// 1055
			AddComplexComponent( (BaseAddon) this, 62, 36, -4, 22, 1645, -1, "", 1);// 1056
			AddComplexComponent( (BaseAddon) this, 62, 37, -4, 22, 1645, -1, "", 1);// 1057
			AddComplexComponent( (BaseAddon) this, 8611, 33, 0, 22, 0, -1, "Block Everything", 1);// 1058
			AddComplexComponent( (BaseAddon) this, 8611, 35, 0, 22, 0, -1, "Block Everything", 1);// 1059
			AddComplexComponent( (BaseAddon) this, 8611, 34, 0, 22, 0, -1, "Block Everything", 1);// 1060
			AddComplexComponent( (BaseAddon) this, 8610, 35, 0, 32, 0, -1, "no line of sight", 1);// 1061
			AddComplexComponent( (BaseAddon) this, 8610, 34, 0, 32, 0, -1, "no line of sight", 1);// 1062
			AddComplexComponent( (BaseAddon) this, 62, 33, 0, 22, 1645, -1, "", 1);// 1063
			AddComplexComponent( (BaseAddon) this, 62, 32, 0, 22, 1645, -1, "", 1);// 1064
			AddComplexComponent( (BaseAddon) this, 62, 34, 0, 22, 1645, -1, "", 1);// 1065
			AddComplexComponent( (BaseAddon) this, 62, 35, 0, 22, 1645, -1, "", 1);// 1066
			AddComplexComponent( (BaseAddon) this, 8611, 32, 0, 22, 0, -1, "Block Everything", 1);// 1067
			AddComplexComponent( (BaseAddon) this, 8611, 43, 24, 0, 0, -1, "Block Everything", 1);// 1068
			AddComplexComponent( (BaseAddon) this, 8611, 36, 28, 22, 0, -1, "Block Everything", 1);// 1073
			AddComplexComponent( (BaseAddon) this, 8611, 34, 26, 22, 0, -1, "Block Everything", 1);// 1080
			AddComplexComponent( (BaseAddon) this, 8611, 43, 26, 0, 0, -1, "Block Everything", 1);// 1083
			AddComplexComponent( (BaseAddon) this, 8611, 41, 22, 22, 0, -1, "Block Everything", 1);// 1084
			AddComplexComponent( (BaseAddon) this, 3796, 35, 25, 22, 1365, -1, "a reagent stone", 1);// 1088
			AddComplexComponent( (BaseAddon) this, 8611, 45, 28, 22, 0, -1, "Block Everything", 1);// 1090
			AddComplexComponent( (BaseAddon) this, 8611, 34, 24, 22, 0, -1, "Block Everything", 1);// 1091
			AddComplexComponent( (BaseAddon) this, 8611, 34, 25, 22, 0, -1, "Block Everything", 1);// 1092
			AddComplexComponent( (BaseAddon) this, 8611, 43, 28, 22, 0, -1, "Block Everything", 1);// 1093
			AddComplexComponent( (BaseAddon) this, 8611, 45, 24, 20, 0, -1, "Block Everything", 1);// 1098
			AddComplexComponent( (BaseAddon) this, 8611, 35, 22, 22, 0, -1, "Block Everything", 1);// 1099
			AddComplexComponent( (BaseAddon) this, 8611, 34, 22, 22, 0, -1, "Block Everything", 1);// 1100
			AddComplexComponent( (BaseAddon) this, 8611, 42, 22, 22, 0, -1, "Block Everything", 1);// 1101
			AddComplexComponent( (BaseAddon) this, 8611, 45, 24, 0, 0, -1, "Block Everything", 1);// 1124
			AddComplexComponent( (BaseAddon) this, 8611, 44, 26, 0, 0, -1, "Block Everything", 1);// 1125
			AddComplexComponent( (BaseAddon) this, 8611, 44, 25, 0, 0, -1, "Block Everything", 1);// 1126
			AddComplexComponent( (BaseAddon) this, 8611, 44, 24, 0, 0, -1, "Block Everything", 1);// 1127
			AddComplexComponent( (BaseAddon) this, 7955, 44, 24, 22, 0, -1, "Spawner", 1);// 1129
			AddComplexComponent( (BaseAddon) this, 8611, 45, 26, 20, 0, -1, "Block Everything", 1);// 1132
			AddComplexComponent( (BaseAddon) this, 1306, 37, 27, 22, 1365, -1, "", 1);// 1136
			AddComplexComponent( (BaseAddon) this, 8611, 37, 28, 22, 0, -1, "Block Everything", 1);// 1155
			AddComplexComponent( (BaseAddon) this, 8611, 43, 22, 22, 0, -1, "Block Everything", 1);// 1159
			AddComplexComponent( (BaseAddon) this, 4483, 35, 26, 22, 1365, -1, "Game Supply Stone", 1);// 1160
			AddComplexComponent( (BaseAddon) this, 8611, 34, 28, 22, 0, -1, "Block Everything", 1);// 1161
			AddComplexComponent( (BaseAddon) this, 8611, 45, 22, 22, 0, -1, "Block Everything", 1);// 1162
			AddComplexComponent( (BaseAddon) this, 1306, 41, 27, 22, 1365, -1, "", 1);// 1166
			AddComplexComponent( (BaseAddon) this, 8611, 43, 25, 0, 0, -1, "Block Everything", 1);// 1171
			AddComplexComponent( (BaseAddon) this, 8611, 40, 22, 22, 0, -1, "Block Everything", 1);// 1172
			AddComplexComponent( (BaseAddon) this, 8611, 39, 22, 22, 0, -1, "Block Everything", 1);// 1173
			AddComplexComponent( (BaseAddon) this, 8611, 38, 22, 22, 0, -1, "Block Everything", 1);// 1174
			AddComplexComponent( (BaseAddon) this, 8611, 37, 22, 22, 0, -1, "Block Everything", 1);// 1175
			AddComplexComponent( (BaseAddon) this, 8611, 36, 22, 22, 0, -1, "Block Everything", 1);// 1176
			AddComplexComponent( (BaseAddon) this, 8611, 42, 28, 22, 0, -1, "Block Everything", 1);// 1177
			AddComplexComponent( (BaseAddon) this, 8611, 40, 28, 22, 0, -1, "Block Everything", 1);// 1181
			AddComplexComponent( (BaseAddon) this, 1306, 39, 27, 22, 1365, -1, "", 1);// 1186
			AddComplexComponent( (BaseAddon) this, 8611, 44, 28, 22, 0, -1, "Block Everything", 1);// 1199
			AddComplexComponent( (BaseAddon) this, 8611, 45, 25, 20, 0, -1, "Block Everything", 1);// 1202
			AddComplexComponent( (BaseAddon) this, 8611, 39, 28, 22, 0, -1, "Block Everything", 1);// 1203
			AddComplexComponent( (BaseAddon) this, 7955, 44, 23, 22, 0, -1, "Spawner", 1);// 1206
			AddComplexComponent( (BaseAddon) this, 8611, 38, 28, 22, 0, -1, "Block Everything", 1);// 1207
			AddComplexComponent( (BaseAddon) this, 8611, 45, 23, 20, 0, -1, "Block Everything", 1);// 1208
			AddComplexComponent( (BaseAddon) this, 8611, 34, 23, 22, 0, -1, "Block Everything", 1);// 1213
			AddComplexComponent( (BaseAddon) this, 8611, 45, 27, 22, 0, -1, "Block Everything", 1);// 1214
			AddComplexComponent( (BaseAddon) this, 8611, 41, 28, 22, 0, -1, "Block Everything", 1);// 1215
			AddComplexComponent( (BaseAddon) this, 8611, 44, 22, 22, 0, -1, "Block Everything", 1);// 1219
			AddComplexComponent( (BaseAddon) this, 8611, 35, 28, 22, 0, -1, "Block Everything", 1);// 1225
			AddComplexComponent( (BaseAddon) this, 8611, 34, 27, 22, 0, -1, "Block Everything", 1);// 1227
			AddComplexComponent( (BaseAddon) this, 8611, 45, 26, 0, 0, -1, "Block Everything", 1);// 1228
			AddComplexComponent( (BaseAddon) this, 8611, 45, 25, 0, 0, -1, "Block Everything", 1);// 1229
			AddComplexComponent( (BaseAddon) this, 1955, 46, 43, 0, 1365, -1, "", 1);// 1230
			AddComplexComponent( (BaseAddon) this, 8611, 34, 39, 22, 0, -1, "Block Everything", 1);// 1231
			AddComplexComponent( (BaseAddon) this, 128, 35, 45, 0, 1365, -1, "", 1);// 1232
			AddComplexComponent( (BaseAddon) this, 8611, 42, 34, 22, 0, -1, "Block Everything", 1);// 1233
			AddComplexComponent( (BaseAddon) this, 1955, 44, 46, 0, 1365, -1, "", 1);// 1234
			AddComplexComponent( (BaseAddon) this, 1958, 45, 42, 5, 1365, -1, "", 1);// 1235
			AddComplexComponent( (BaseAddon) this, 1955, 41, 45, 0, 1365, -1, "", 1);// 1236
			AddComplexComponent( (BaseAddon) this, 1955, 46, 45, 15, 1365, -1, "", 1);// 1237
			AddComplexComponent( (BaseAddon) this, 1958, 45, 43, 10, 1365, -1, "", 1);// 1238
			AddComplexComponent( (BaseAddon) this, 8611, 37, 34, 22, 0, -1, "Block Everything", 1);// 1239
			AddComplexComponent( (BaseAddon) this, 1955, 45, 44, 5, 1365, -1, "", 1);// 1240
			AddComplexComponent( (BaseAddon) this, 1955, 46, 44, 5, 1365, -1, "", 1);// 1241
			AddComplexComponent( (BaseAddon) this, 1955, 45, 43, 5, 1365, -1, "", 1);// 1242
			AddComplexComponent( (BaseAddon) this, 1955, 46, 46, 5, 1365, -1, "", 1);// 1243
			AddComplexComponent( (BaseAddon) this, 1955, 44, 46, 15, 1365, -1, "", 1);// 1244
			AddComplexComponent( (BaseAddon) this, 1822, 42, 34, 22, 1365, -1, "", 1);// 1245
			AddComplexComponent( (BaseAddon) this, 1958, 46, 42, 5, 1365, -1, "", 1);// 1246
			AddComplexComponent( (BaseAddon) this, 128, 35, 40, 0, 1365, -1, "", 1);// 1247
			AddComplexComponent( (BaseAddon) this, 128, 35, 38, 0, 1365, -1, "", 1);// 1248
			AddComplexComponent( (BaseAddon) this, 128, 36, 36, 0, 1365, -1, "", 1);// 1249
			AddComplexComponent( (BaseAddon) this, 1958, 46, 41, 0, 1365, -1, "", 1);// 1250
			AddComplexComponent( (BaseAddon) this, 1959, 41, 45, 5, 1365, -1, "", 1);// 1251
			AddComplexComponent( (BaseAddon) this, 1959, 41, 46, 5, 1365, -1, "", 1);// 1252
			AddComplexComponent( (BaseAddon) this, 8611, 34, 43, 22, 0, -1, "Block Everything", 1);// 1253
			AddComplexComponent( (BaseAddon) this, 1822, 34, 37, 22, 1365, -1, "", 1);// 1254
			AddComplexComponent( (BaseAddon) this, 128, 35, 36, 0, 1365, -1, "", 1);// 1255
			AddComplexComponent( (BaseAddon) this, 8611, 34, 40, 22, 0, -1, "Block Everything", 1);// 1256
			AddComplexComponent( (BaseAddon) this, 128, 35, 41, 0, 1365, -1, "", 1);// 1257
			AddComplexComponent( (BaseAddon) this, 128, 35, 37, 0, 1365, -1, "", 1);// 1258
			AddComplexComponent( (BaseAddon) this, 1955, 46, 46, 0, 1365, -1, "", 1);// 1261
			AddComplexComponent( (BaseAddon) this, 128, 41, 35, 0, 1365, -1, "", 1);// 1262
			AddComplexComponent( (BaseAddon) this, 128, 38, 35, 0, 1365, -1, "", 1);// 1263
			AddComplexComponent( (BaseAddon) this, 8611, 46, 35, 27, 0, -1, "Block Everything", 1);// 1264
			AddComplexComponent( (BaseAddon) this, 1822, 40, 34, 22, 1365, -1, "", 1);// 1265
			AddComplexComponent( (BaseAddon) this, 1822, 43, 34, 22, 1365, -1, "", 1);// 1266
			AddComplexComponent( (BaseAddon) this, 1822, 34, 39, 22, 1365, -1, "", 1);// 1267
			AddComplexComponent( (BaseAddon) this, 1955, 46, 46, 15, 1365, -1, "", 1);// 1268
			AddComplexComponent( (BaseAddon) this, 1822, 34, 42, 22, 1365, -1, "", 1);// 1269
			AddComplexComponent( (BaseAddon) this, 5671, 45, 46, 20, 1365, -1, "", 1);// 1270
			AddComplexComponent( (BaseAddon) this, 1958, 45, 44, 15, 1365, -1, "", 1);// 1271
			AddComplexComponent( (BaseAddon) this, 8611, 34, 42, 22, 0, -1, "Block Everything", 1);// 1272
			AddComplexComponent( (BaseAddon) this, 128, 45, 35, 0, 1365, -1, "", 1);// 1274
			AddComplexComponent( (BaseAddon) this, 1955, 44, 46, 10, 1365, -1, "", 1);// 1277
			AddComplexComponent( (BaseAddon) this, 8611, 34, 37, 22, 0, -1, "Block Everything", 1);// 1278
			AddComplexComponent( (BaseAddon) this, 1955, 43, 46, 0, 1365, -1, "", 1);// 1279
			AddComplexComponent( (BaseAddon) this, 1955, 44, 46, 5, 1365, -1, "", 1);// 1280
			AddComplexComponent( (BaseAddon) this, 1955, 44, 45, 5, 1365, -1, "", 1);// 1281
			AddComplexComponent( (BaseAddon) this, 1955, 43, 45, 10, 1365, -1, "", 1);// 1282
			AddComplexComponent( (BaseAddon) this, 1955, 43, 46, 5, 1365, -1, "", 1);// 1283
			AddComplexComponent( (BaseAddon) this, 1955, 43, 46, 10, 1365, -1, "", 1);// 1284
			AddComplexComponent( (BaseAddon) this, 1959, 42, 46, 10, 1365, -1, "", 1);// 1285
			AddComplexComponent( (BaseAddon) this, 1959, 42, 45, 10, 1365, -1, "", 1);// 1286
			AddComplexComponent( (BaseAddon) this, 1955, 43, 45, 5, 1365, -1, "", 1);// 1287
			AddComplexComponent( (BaseAddon) this, 1955, 46, 45, 10, 1365, -1, "", 1);// 1288
			AddComplexComponent( (BaseAddon) this, 1955, 46, 44, 10, 1365, -1, "", 1);// 1289
			AddComplexComponent( (BaseAddon) this, 8611, 35, 35, 22, 0, -1, "Block Everything", 1);// 1291
			AddComplexComponent( (BaseAddon) this, 128, 42, 35, 0, 1365, -1, "", 1);// 1292
			AddComplexComponent( (BaseAddon) this, 8611, 43, 34, 22, 0, -1, "Block Everything", 1);// 1293
			AddComplexComponent( (BaseAddon) this, 1958, 45, 41, 0, 1365, -1, "", 1);// 1294
			AddComplexComponent( (BaseAddon) this, 128, 35, 39, 0, 1365, -1, "", 1);// 1295
			AddComplexComponent( (BaseAddon) this, 1958, 46, 44, 15, 1365, -1, "", 1);// 1296
			AddComplexComponent( (BaseAddon) this, 1822, 35, 35, 22, 1365, -1, "", 1);// 1298
			AddComplexComponent( (BaseAddon) this, 1955, 46, 42, 0, 1365, -1, "", 1);// 1299
			AddComplexComponent( (BaseAddon) this, 1955, 45, 43, 0, 1365, -1, "", 1);// 1300
			AddComplexComponent( (BaseAddon) this, 1955, 42, 46, 0, 1365, -1, "", 1);// 1301
			AddComplexComponent( (BaseAddon) this, 1955, 45, 42, 0, 1365, -1, "", 1);// 1302
			AddComplexComponent( (BaseAddon) this, 1959, 40, 45, 0, 1365, -1, "", 1);// 1303
			AddComplexComponent( (BaseAddon) this, 1955, 45, 45, 0, 1365, -1, "", 1);// 1304
			AddComplexComponent( (BaseAddon) this, 1955, 45, 46, 0, 1365, -1, "", 1);// 1305
			AddComplexComponent( (BaseAddon) this, 1955, 45, 44, 0, 1365, -1, "", 1);// 1306
			AddComplexComponent( (BaseAddon) this, 128, 35, 43, 0, 1365, -1, "", 1);// 1307
			AddComplexComponent( (BaseAddon) this, 1955, 41, 46, 0, 1365, -1, "", 1);// 1308
			AddComplexComponent( (BaseAddon) this, 1955, 42, 45, 0, 1365, -1, "", 1);// 1310
			AddComplexComponent( (BaseAddon) this, 8611, 39, 34, 22, 0, -1, "Block Everything", 1);// 1311
			AddComplexComponent( (BaseAddon) this, 1955, 43, 45, 0, 1365, -1, "", 1);// 1312
			AddComplexComponent( (BaseAddon) this, 1822, 34, 45, 22, 1365, -1, "", 1);// 1313
			AddComplexComponent( (BaseAddon) this, 1822, 34, 46, 22, 1365, -1, "", 1);// 1314
			AddComplexComponent( (BaseAddon) this, 1822, 36, 34, 22, 1365, -1, "", 1);// 1315
			AddComplexComponent( (BaseAddon) this, 128, 44, 35, 0, 1365, -1, "", 1);// 1316
			AddComplexComponent( (BaseAddon) this, 1822, 34, 40, 22, 1365, -1, "", 1);// 1317
			AddComplexComponent( (BaseAddon) this, 1955, 46, 45, 5, 1365, -1, "", 1);// 1318
			AddComplexComponent( (BaseAddon) this, 1822, 34, 43, 22, 1365, -1, "", 1);// 1319
			AddComplexComponent( (BaseAddon) this, 1822, 37, 34, 22, 1365, -1, "", 1);// 1320
			AddComplexComponent( (BaseAddon) this, 1822, 45, 34, 22, 1365, -1, "", 1);// 1321
			AddComplexComponent( (BaseAddon) this, 128, 37, 35, 0, 1365, -1, "", 1);// 1322
			AddComplexComponent( (BaseAddon) this, 1822, 34, 36, 22, 1365, -1, "", 1);// 1323
			AddComplexComponent( (BaseAddon) this, 1955, 46, 45, 0, 1365, -1, "", 1);// 1324
			AddComplexComponent( (BaseAddon) this, 1955, 46, 44, 0, 1365, -1, "", 1);// 1325
			AddComplexComponent( (BaseAddon) this, 14138, 44, 44, 0, 1365, -1, "", 1);// 1326
			AddComplexComponent( (BaseAddon) this, 7774, 38, 36, 0, 0, -1, "CTF Score board", 1);// 1328
			AddComplexComponent( (BaseAddon) this, 8611, 46, 34, 22, 0, -1, "Block Everything", 1);// 1329
			AddComplexComponent( (BaseAddon) this, 8611, 34, 45, 22, 0, -1, "Block Everything", 1);// 1331
			AddComplexComponent( (BaseAddon) this, 8611, 45, 34, 22, 0, -1, "Block Everything", 1);// 1332
			AddComplexComponent( (BaseAddon) this, 1958, 46, 43, 10, 1365, -1, "", 1);// 1333
			AddComplexComponent( (BaseAddon) this, 1822, 39, 34, 22, 1365, -1, "", 1);// 1334
			AddComplexComponent( (BaseAddon) this, 128, 36, 35, 0, 1365, -1, "", 1);// 1335
			AddComplexComponent( (BaseAddon) this, 1959, 43, 45, 15, 1365, -1, "", 1);// 1336
			AddComplexComponent( (BaseAddon) this, 1822, 46, 35, 22, 1365, -1, "", 1);// 1340
			AddComplexComponent( (BaseAddon) this, 1955, 42, 46, 5, 1365, -1, "", 1);// 1341
			AddComplexComponent( (BaseAddon) this, 1955, 42, 45, 5, 1365, -1, "", 1);// 1342
			AddComplexComponent( (BaseAddon) this, 1955, 45, 44, 10, 1365, -1, "", 1);// 1343
			AddComplexComponent( (BaseAddon) this, 8611, 34, 36, 22, 0, -1, "Block Everything", 1);// 1344
			AddComplexComponent( (BaseAddon) this, 1955, 44, 45, 10, 1365, -1, "", 1);// 1347
			AddComplexComponent( (BaseAddon) this, 128, 39, 35, 0, 1365, -1, "", 1);// 1348
			AddComplexComponent( (BaseAddon) this, 8611, 40, 34, 22, 0, -1, "Block Everything", 1);// 1356
			AddComplexComponent( (BaseAddon) this, 128, 46, 35, 0, 1365, -1, "", 1);// 1357
			AddComplexComponent( (BaseAddon) this, 8611, 36, 34, 22, 0, -1, "Block Everything", 1);// 1358
			AddComplexComponent( (BaseAddon) this, 1959, 40, 46, 0, 1365, -1, "", 1);// 1359
			AddComplexComponent( (BaseAddon) this, 1822, 46, 34, 22, 1365, -1, "", 1);// 1360
			AddComplexComponent( (BaseAddon) this, 1955, 44, 45, 0, 1365, -1, "", 1);// 1361
			AddComplexComponent( (BaseAddon) this, 128, 35, 44, 0, 1365, -1, "", 1);// 1362
			AddComplexComponent( (BaseAddon) this, 1955, 44, 45, 15, 1365, -1, "", 1);// 1363
			AddComplexComponent( (BaseAddon) this, 1959, 43, 46, 15, 1365, -1, "", 1);// 1364
			AddComplexComponent( (BaseAddon) this, 1955, 46, 46, 10, 1365, -1, "", 1);// 1367
			AddComplexComponent( (BaseAddon) this, 1955, 45, 46, 15, 1365, -1, "", 1);// 1368
			AddComplexComponent( (BaseAddon) this, 1955, 45, 45, 15, 1365, -1, "", 1);// 1369
			AddComplexComponent( (BaseAddon) this, 1955, 45, 46, 10, 1365, -1, "", 1);// 1370
			AddComplexComponent( (BaseAddon) this, 1955, 45, 45, 10, 1365, -1, "", 1);// 1371
			AddComplexComponent( (BaseAddon) this, 1955, 45, 46, 5, 1365, -1, "", 1);// 1372
			AddComplexComponent( (BaseAddon) this, 1955, 45, 45, 5, 1365, -1, "", 1);// 1373
			AddComplexComponent( (BaseAddon) this, 1955, 46, 43, 5, 1365, -1, "", 1);// 1374
			AddComplexComponent( (BaseAddon) this, 1955, 49, -46, 0, 1645, -1, "", 1);// 1375
			AddComplexComponent( (BaseAddon) this, 1955, 49, -48, 0, 1645, -1, "", 1);// 1376
			AddComplexComponent( (BaseAddon) this, 1955, 49, -47, 0, 1645, -1, "", 1);// 1377
			AddComplexComponent( (BaseAddon) this, 1955, 49, -48, 5, 1645, -1, "", 1);// 1378
			AddComplexComponent( (BaseAddon) this, 1955, 49, -47, 5, 1645, -1, "", 1);// 1379
			AddComplexComponent( (BaseAddon) this, 1955, 49, -46, 5, 1645, -1, "", 1);// 1380
			AddComplexComponent( (BaseAddon) this, 1955, 49, -45, 5, 1645, -1, "", 1);// 1381
			AddComplexComponent( (BaseAddon) this, 1955, 49, -45, 0, 1645, -1, "", 1);// 1382
			AddComplexComponent( (BaseAddon) this, 1955, 49, -44, 0, 1645, -1, "", 1);// 1383
			AddComplexComponent( (BaseAddon) this, 1956, 49, -43, 0, 1645, -1, "", 1);// 1384
			AddComplexComponent( (BaseAddon) this, 1956, 49, -42, 0, 1645, -1, "", 1);// 1385

		}

		public CTFDungeonAddon( Serial serial ) : base( serial )
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

	public class CTFDungeonAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new CTFDungeonAddon();
			}
		}

		[Constructable]
		public CTFDungeonAddonDeed()
		{
			Name = "CTFDungeon";
		}

		public CTFDungeonAddonDeed( Serial serial ) : base( serial )
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