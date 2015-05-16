
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class Arena1Addon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1208, -8, -4, 0}, {1208, -8, -3, 0}, {1208, -8, -2, 0}// 238	239	240	
			, {1208, -8, -1, 0}, {1208, -8, 0, 0}, {1208, -8, 1, 0}// 241	242	243	
			, {1208, -8, 2, 0}, {1208, -8, 3, 0}, {1208, -8, 4, 0}// 244	245	246	
			, {1208, -7, -4, 0}, {1208, -7, -3, 0}, {1208, -7, -2, 0}// 254	255	256	
			, {1208, -7, -1, 0}, {1208, -7, 0, 0}, {1208, -7, 1, 0}// 257	258	259	
			, {1208, -7, 2, 0}, {1208, -7, 3, 0}, {1208, -7, 4, 0}// 260	261	262	
			, {1208, -6, -4, 0}, {1208, -6, -3, 0}, {1208, -6, -2, 0}// 270	271	272	
			, {1208, -6, -1, 0}, {1208, -6, 0, 0}, {1208, -6, 1, 0}// 273	274	275	
			, {1208, -6, 2, 0}, {1208, -6, 3, 0}, {1208, -6, 4, 0}// 276	277	278	
			, {1208, -5, -4, 0}, {1208, -5, -3, 0}, {1208, -5, -2, 0}// 286	287	288	
			, {1208, -5, -1, 0}, {1208, -5, 0, 0}, {1208, -5, 1, 0}// 289	290	291	
			, {1208, -5, 2, 0}, {1208, -5, 3, 0}, {1208, -5, 4, 0}// 292	293	294	
			, {1208, -4, -4, 0}, {1208, -4, -3, 0}, {1208, -4, -2, 0}// 302	303	304	
			, {1208, -4, -1, 0}, {1208, -4, 0, 0}, {1208, -4, 1, 0}// 305	306	307	
			, {1208, -4, 2, 0}, {1208, -4, 3, 0}, {1208, -4, 4, 0}// 308	309	310	
			, {1208, -3, -4, 0}, {1208, -3, -3, 0}, {1208, -3, -2, 0}// 318	319	320	
			, {1208, -3, -1, 0}, {1208, -3, 0, 0}, {1208, -3, 1, 0}// 321	322	323	
			, {1208, -3, 2, 0}, {1208, -3, 3, 0}, {1208, -3, 4, 0}// 324	325	326	
			, {1208, -2, -4, 0}, {1208, -2, -3, 0}, {1208, -2, -2, 0}// 334	335	336	
			, {1208, -2, -1, 0}, {1208, -2, 0, 0}, {1208, -2, 1, 0}// 337	338	339	
			, {1208, -2, 2, 0}, {1208, -2, 3, 0}, {1208, -2, 4, 0}// 340	341	342	
			, {1208, -1, -4, 0}, {1208, -1, -3, 0}, {1208, -1, -2, 0}// 350	351	352	
			, {1208, -1, -1, 0}, {1208, -1, 0, 0}, {1208, -1, 1, 0}// 353	354	355	
			, {1208, -1, 2, 0}, {1208, -1, 3, 0}, {1208, -1, 4, 0}// 356	357	358	
			, {1208, 0, -4, 0}, {1208, 0, -3, 0}, {1208, 0, -2, 0}// 366	367	368	
			, {1208, 0, -1, 0}, {1208, 0, 0, 0}, {1208, 0, 1, 0}// 369	370	371	
			, {1208, 0, 2, 0}, {1208, 0, 3, 0}, {1208, 0, 4, 0}// 372	373	374	
			, {1208, 1, -3, 0}, {1208, 1, -2, 0}, {1208, 1, -1, 0}// 383	384	385	
			, {1208, 1, 0, 0}, {1208, 1, 1, 0}, {1208, 1, 2, 0}// 386	387	388	
			, {1208, 2, -3, 0}, {1208, 2, -2, 0}, {1208, 2, -1, 0}// 399	400	401	
			, {1208, 2, 0, 0}, {1208, 2, 1, 0}, {1208, 2, 2, 0}// 402	403	404	
			, {1208, 3, -4, 0}, {1208, 3, -3, 0}, {1208, 3, -2, 0}// 414	415	416	
			, {1208, 3, -1, 0}, {1208, 3, 0, 0}, {1208, 3, 1, 0}// 417	418	419	
			, {1208, 3, 2, 0}, {1208, 3, 3, 0}, {1208, 3, 4, 0}// 420	421	422	
			, {1208, 4, -4, 0}, {1208, 4, -3, 0}, {1208, 4, -2, 0}// 430	431	432	
			, {1208, 4, -1, 0}, {1208, 4, 0, 0}, {1208, 4, 1, 0}// 433	434	435	
			, {1208, 4, 2, 0}, {1208, 4, 3, 0}, {1208, 4, 4, 0}// 436	437	438	
			, {1208, 5, -4, 0}, {1208, 5, -3, 0}, {1208, 5, -2, 0}// 446	447	448	
			, {1208, 5, -1, 0}, {1208, 5, 0, 0}, {1208, 5, 1, 0}// 449	450	451	
			, {1208, 5, 2, 0}, {1208, 5, 3, 0}, {1208, 5, 4, 0}// 452	453	454	
			, {1208, 6, -4, 0}, {1208, 6, -3, 0}, {1208, 6, -2, 0}// 462	463	464	
			, {1208, 6, -1, 0}, {1208, 6, 0, 0}, {1208, 6, 1, 0}// 465	466	467	
			, {1208, 6, 2, 0}, {1208, 6, 3, 0}, {1208, 6, 4, 0}// 468	469	470	
			, {1208, 7, -4, 0}, {1208, 7, -3, 0}, {1208, 7, -2, 0}// 478	479	480	
			, {1208, 7, -1, 0}, {1208, 7, 0, 0}, {1208, 7, 1, 0}// 481	482	483	
			, {1208, 7, 2, 0}, {1208, 7, 3, 0}, {1208, 7, 4, 0}// 484	485	486	
			, {1208, -8, 5, 0}, {1208, -7, 5, 0}, {1208, -6, 5, 0}// 503	512	521	
			, {1208, -5, 5, 0}, {1208, -4, 5, 0}, {1208, -3, 5, 0}// 530	539	548	
			, {1208, -2, 5, 0}, {1208, -1, 5, 0}, {1208, 0, 5, 0}// 557	566	575	
			, {1208, 3, 5, 0}, {1208, 4, 5, 0}, {1208, 5, 5, 0}// 602	611	620	
			, {1208, 6, 5, 0}, {1208, 7, 5, 0}, {1208, 8, -4, 0}// 629	638	704	
			, {1208, 8, -3, 0}, {1208, 8, -2, 0}, {1208, 8, -1, 0}// 705	706	707	
			, {1208, 8, 0, 0}, {1208, 8, 1, 0}, {1208, 8, 2, 0}// 708	709	710	
			, {1208, 8, 3, 0}, {1208, 8, 4, 0}, {1208, 9, -4, 0}// 711	712	720	
			, {1208, 9, -3, 0}, {1208, 9, -2, 0}, {1208, 9, -1, 0}// 721	722	723	
			, {1208, 9, 0, 0}, {1208, 9, 1, 0}, {1208, 9, 2, 0}// 724	725	726	
			, {1208, 9, 3, 0}, {1208, 9, 4, 0}, {1208, 10, -4, 0}// 727	728	736	
			, {1208, 10, -3, 0}, {1208, 10, -2, 0}, {1208, 10, -1, 0}// 737	738	739	
			, {1208, 10, 0, 0}, {1208, 10, 1, 0}, {1208, 10, 2, 0}// 740	741	742	
			, {1208, 10, 3, 0}, {1208, 10, 4, 0}, {1208, 11, -4, 0}// 743	744	752	
			, {1208, 11, -3, 0}, {1208, 11, -2, 0}, {1208, 11, -1, 0}// 753	754	755	
			, {1208, 11, 0, 0}, {1208, 11, 1, 0}, {1208, 11, 2, 0}// 756	757	758	
			, {1208, 11, 3, 0}, {1208, 11, 4, 0}, {1208, 12, -4, 0}// 759	760	768	
			, {1208, 12, -3, 0}, {1208, 12, -2, 0}, {1208, 12, -1, 0}// 769	770	771	
			, {1208, 12, 0, 0}, {1208, 12, 1, 0}, {1208, 12, 2, 0}// 772	773	774	
			, {1208, 12, 3, 0}, {1208, 12, 4, 0}, {1208, 8, 5, 0}// 775	776	863	
			, {1208, 9, 5, 0}, {1208, 10, 5, 0}, {1208, 11, 5, 0}// 872	881	890	
			, {1208, 12, 5, 0}// 899	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new Arena1AddonDeed();
			}
		}

		[ Constructable ]
		public Arena1Addon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 128, -15, -12, 0, 2052, -1, "", 1);// 1
			AddComplexComponent( (BaseAddon) this, 128, -14, -12, 0, 2052, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 128, -13, -12, 0, 2052, -1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 128, -12, -12, 0, 2052, -1, "", 1);// 4
			AddComplexComponent( (BaseAddon) this, 128, -11, -12, 0, 2052, -1, "", 1);// 5
			AddComplexComponent( (BaseAddon) this, 128, -10, -12, 0, 2052, -1, "", 1);// 6
			AddComplexComponent( (BaseAddon) this, 128, -9, -12, 0, 2052, -1, "", 1);// 7
			AddComplexComponent( (BaseAddon) this, 1208, -14, -12, 0, 2052, -1, "", 1);// 8
			AddComplexComponent( (BaseAddon) this, 1208, -13, -12, 0, 2052, -1, "", 1);// 9
			AddComplexComponent( (BaseAddon) this, 1208, -12, -12, 0, 2052, -1, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 1208, -11, -12, 0, 2052, -1, "", 1);// 11
			AddComplexComponent( (BaseAddon) this, 1208, -10, -12, 0, 2052, -1, "", 1);// 12
			AddComplexComponent( (BaseAddon) this, 1208, -9, -12, 0, 2052, -1, "", 1);// 13
			AddComplexComponent( (BaseAddon) this, 128, -15, -11, 0, 2052, -1, "", 1);// 14
			AddComplexComponent( (BaseAddon) this, 128, -15, -10, 0, 2052, -1, "", 1);// 15
			AddComplexComponent( (BaseAddon) this, 128, -15, -9, 0, 2052, -1, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 128, -15, -8, 0, 2052, -1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 128, -15, -7, 0, 2052, -1, "", 1);// 18
			AddComplexComponent( (BaseAddon) this, 128, -15, -6, 0, 2052, -1, "", 1);// 19
			AddComplexComponent( (BaseAddon) this, 128, -15, -5, 0, 2052, -1, "", 1);// 20
			AddComplexComponent( (BaseAddon) this, 128, -15, -4, 0, 2052, -1, "", 1);// 21
			AddComplexComponent( (BaseAddon) this, 128, -15, -3, 0, 2052, -1, "", 1);// 22
			AddComplexComponent( (BaseAddon) this, 128, -15, -2, 0, 2052, -1, "", 1);// 23
			AddComplexComponent( (BaseAddon) this, 128, -15, -1, 0, 2052, -1, "", 1);// 24
			AddComplexComponent( (BaseAddon) this, 128, -15, 0, 0, 2052, -1, "", 1);// 25
			AddComplexComponent( (BaseAddon) this, 128, -15, 1, 0, 2052, -1, "", 1);// 26
			AddComplexComponent( (BaseAddon) this, 128, -15, 2, 0, 2052, -1, "", 1);// 27
			AddComplexComponent( (BaseAddon) this, 128, -15, 3, 0, 2052, -1, "", 1);// 28
			AddComplexComponent( (BaseAddon) this, 128, -15, 4, 0, 2052, -1, "", 1);// 29
			AddComplexComponent( (BaseAddon) this, 1208, -14, -11, 0, 2052, -1, "", 1);// 30
			AddComplexComponent( (BaseAddon) this, 1208, -14, -10, 0, 2052, -1, "", 1);// 31
			AddComplexComponent( (BaseAddon) this, 1208, -14, -9, 0, 2052, -1, "", 1);// 32
			AddComplexComponent( (BaseAddon) this, 1208, -14, -8, 0, 2052, -1, "", 1);// 33
			AddComplexComponent( (BaseAddon) this, 1208, -14, -7, 0, 2052, -1, "", 1);// 34
			AddComplexComponent( (BaseAddon) this, 1208, -14, -6, 0, 2052, -1, "", 1);// 35
			AddComplexComponent( (BaseAddon) this, 1208, -14, -5, 0, 2052, -1, "", 1);// 36
			AddComplexComponent( (BaseAddon) this, 1208, -14, -4, 0, 2052, -1, "", 1);// 37
			AddComplexComponent( (BaseAddon) this, 1208, -14, -3, 0, 2052, -1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 1208, -14, -2, 0, 2052, -1, "", 1);// 39
			AddComplexComponent( (BaseAddon) this, 1208, -14, -1, 0, 2052, -1, "", 1);// 40
			AddComplexComponent( (BaseAddon) this, 1208, -14, 0, 0, 2052, -1, "", 1);// 41
			AddComplexComponent( (BaseAddon) this, 1208, -14, 1, 0, 2052, -1, "", 1);// 42
			AddComplexComponent( (BaseAddon) this, 1208, -14, 2, 0, 2052, -1, "", 1);// 43
			AddComplexComponent( (BaseAddon) this, 1208, -14, 3, 0, 2052, -1, "", 1);// 44
			AddComplexComponent( (BaseAddon) this, 1208, -14, 4, 0, 2052, -1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 1208, -13, -11, 0, 2052, -1, "", 1);// 46
			AddComplexComponent( (BaseAddon) this, 1208, -13, -10, 0, 2052, -1, "", 1);// 47
			AddComplexComponent( (BaseAddon) this, 1208, -13, -9, 0, 2052, -1, "", 1);// 48
			AddComplexComponent( (BaseAddon) this, 1208, -13, -8, 0, 2052, -1, "", 1);// 49
			AddComplexComponent( (BaseAddon) this, 1208, -13, -7, 0, 2052, -1, "", 1);// 50
			AddComplexComponent( (BaseAddon) this, 1208, -13, -6, 0, 2052, -1, "", 1);// 51
			AddComplexComponent( (BaseAddon) this, 1208, -13, -5, 0, 2052, -1, "", 1);// 52
			AddComplexComponent( (BaseAddon) this, 1208, -13, -4, 0, 2052, -1, "", 1);// 53
			AddComplexComponent( (BaseAddon) this, 1208, -13, -3, 0, 2052, -1, "", 1);// 54
			AddComplexComponent( (BaseAddon) this, 1208, -13, -2, 0, 2052, -1, "", 1);// 55
			AddComplexComponent( (BaseAddon) this, 1208, -13, -1, 0, 2052, -1, "", 1);// 56
			AddComplexComponent( (BaseAddon) this, 1208, -13, 0, 0, 2052, -1, "", 1);// 57
			AddComplexComponent( (BaseAddon) this, 1208, -13, 1, 0, 2052, -1, "", 1);// 58
			AddComplexComponent( (BaseAddon) this, 1208, -13, 2, 0, 2052, -1, "", 1);// 59
			AddComplexComponent( (BaseAddon) this, 1208, -13, 3, 0, 2052, -1, "", 1);// 60
			AddComplexComponent( (BaseAddon) this, 1208, -13, 4, 0, 2052, -1, "", 1);// 61
			AddComplexComponent( (BaseAddon) this, 1208, -12, -11, 0, 2052, -1, "", 1);// 62
			AddComplexComponent( (BaseAddon) this, 1208, -12, -10, 0, 2052, -1, "", 1);// 63
			AddComplexComponent( (BaseAddon) this, 1208, -12, -9, 0, 2052, -1, "", 1);// 64
			AddComplexComponent( (BaseAddon) this, 1208, -12, -8, 0, 2052, -1, "", 1);// 65
			AddComplexComponent( (BaseAddon) this, 1208, -12, -7, 0, 2052, -1, "", 1);// 66
			AddComplexComponent( (BaseAddon) this, 1208, -12, -6, 0, 2052, -1, "", 1);// 67
			AddComplexComponent( (BaseAddon) this, 1208, -12, -5, 0, 2052, -1, "", 1);// 68
			AddComplexComponent( (BaseAddon) this, 1208, -12, -4, 0, 2052, -1, "", 1);// 69
			AddComplexComponent( (BaseAddon) this, 1208, -12, -3, 0, 2052, -1, "", 1);// 70
			AddComplexComponent( (BaseAddon) this, 1208, -12, -2, 0, 2052, -1, "", 1);// 71
			AddComplexComponent( (BaseAddon) this, 1208, -12, -1, 0, 2052, -1, "", 1);// 72
			AddComplexComponent( (BaseAddon) this, 1208, -12, 0, 0, 2052, -1, "", 1);// 73
			AddComplexComponent( (BaseAddon) this, 1208, -12, 1, 0, 2052, -1, "", 1);// 74
			AddComplexComponent( (BaseAddon) this, 1208, -12, 2, 0, 2052, -1, "", 1);// 75
			AddComplexComponent( (BaseAddon) this, 1208, -12, 3, 0, 2052, -1, "", 1);// 76
			AddComplexComponent( (BaseAddon) this, 1208, -12, 4, 0, 2052, -1, "", 1);// 77
			AddComplexComponent( (BaseAddon) this, 1208, -11, -11, 0, 2052, -1, "", 1);// 78
			AddComplexComponent( (BaseAddon) this, 1208, -11, -10, 0, 2052, -1, "", 1);// 79
			AddComplexComponent( (BaseAddon) this, 1208, -11, -9, 0, 2052, -1, "", 1);// 80
			AddComplexComponent( (BaseAddon) this, 1208, -11, -8, 0, 2052, -1, "", 1);// 81
			AddComplexComponent( (BaseAddon) this, 1208, -11, -7, 0, 2052, -1, "", 1);// 82
			AddComplexComponent( (BaseAddon) this, 1208, -11, -6, 0, 2052, -1, "", 1);// 83
			AddComplexComponent( (BaseAddon) this, 1208, -11, -5, 0, 2052, -1, "", 1);// 84
			AddComplexComponent( (BaseAddon) this, 1208, -11, -4, 0, 2052, -1, "", 1);// 85
			AddComplexComponent( (BaseAddon) this, 1208, -11, -3, 0, 2052, -1, "", 1);// 86
			AddComplexComponent( (BaseAddon) this, 1208, -11, -2, 0, 2052, -1, "", 1);// 87
			AddComplexComponent( (BaseAddon) this, 1208, -11, -1, 0, 2052, -1, "", 1);// 88
			AddComplexComponent( (BaseAddon) this, 1208, -11, 0, 0, 2052, -1, "", 1);// 89
			AddComplexComponent( (BaseAddon) this, 1208, -11, 1, 0, 2052, -1, "", 1);// 90
			AddComplexComponent( (BaseAddon) this, 1208, -11, 2, 0, 2052, -1, "", 1);// 91
			AddComplexComponent( (BaseAddon) this, 1208, -11, 3, 0, 2052, -1, "", 1);// 92
			AddComplexComponent( (BaseAddon) this, 1208, -11, 4, 0, 2052, -1, "", 1);// 93
			AddComplexComponent( (BaseAddon) this, 1208, -10, -11, 0, 2052, -1, "", 1);// 94
			AddComplexComponent( (BaseAddon) this, 1208, -10, -10, 0, 2052, -1, "", 1);// 95
			AddComplexComponent( (BaseAddon) this, 1208, -10, -9, 0, 2052, -1, "", 1);// 96
			AddComplexComponent( (BaseAddon) this, 1208, -10, -8, 0, 2052, -1, "", 1);// 97
			AddComplexComponent( (BaseAddon) this, 1208, -10, -7, 0, 2052, -1, "", 1);// 98
			AddComplexComponent( (BaseAddon) this, 1208, -10, -6, 0, 2052, -1, "", 1);// 99
			AddComplexComponent( (BaseAddon) this, 1208, -10, -5, 0, 2052, -1, "", 1);// 100
			AddComplexComponent( (BaseAddon) this, 1208, -10, -4, 0, 2052, -1, "", 1);// 101
			AddComplexComponent( (BaseAddon) this, 1208, -10, -3, 0, 2052, -1, "", 1);// 102
			AddComplexComponent( (BaseAddon) this, 1208, -10, -2, 0, 2052, -1, "", 1);// 103
			AddComplexComponent( (BaseAddon) this, 1208, -10, -1, 0, 2052, -1, "", 1);// 104
			AddComplexComponent( (BaseAddon) this, 1208, -10, 0, 0, 2052, -1, "", 1);// 105
			AddComplexComponent( (BaseAddon) this, 1208, -10, 1, 0, 2052, -1, "", 1);// 106
			AddComplexComponent( (BaseAddon) this, 1208, -10, 2, 0, 2052, -1, "", 1);// 107
			AddComplexComponent( (BaseAddon) this, 1208, -10, 3, 0, 2052, -1, "", 1);// 108
			AddComplexComponent( (BaseAddon) this, 1208, -10, 4, 0, 2052, -1, "", 1);// 109
			AddComplexComponent( (BaseAddon) this, 1208, -9, -11, 0, 2052, -1, "", 1);// 110
			AddComplexComponent( (BaseAddon) this, 1208, -9, -10, 0, 2052, -1, "", 1);// 111
			AddComplexComponent( (BaseAddon) this, 1208, -9, -9, 0, 2052, -1, "", 1);// 112
			AddComplexComponent( (BaseAddon) this, 1208, -9, -8, 0, 2052, -1, "", 1);// 113
			AddComplexComponent( (BaseAddon) this, 1208, -9, -7, 0, 2052, -1, "", 1);// 114
			AddComplexComponent( (BaseAddon) this, 1208, -9, -6, 0, 2052, -1, "", 1);// 115
			AddComplexComponent( (BaseAddon) this, 1208, -9, -5, 0, 2052, -1, "", 1);// 116
			AddComplexComponent( (BaseAddon) this, 1208, -9, -4, 0, 2052, -1, "", 1);// 117
			AddComplexComponent( (BaseAddon) this, 1208, -9, -3, 0, 2052, -1, "", 1);// 118
			AddComplexComponent( (BaseAddon) this, 1208, -9, -2, 0, 2052, -1, "", 1);// 119
			AddComplexComponent( (BaseAddon) this, 1208, -9, -1, 0, 2052, -1, "", 1);// 120
			AddComplexComponent( (BaseAddon) this, 1208, -9, 0, 0, 2052, -1, "", 1);// 121
			AddComplexComponent( (BaseAddon) this, 1208, -9, 1, 0, 2052, -1, "", 1);// 122
			AddComplexComponent( (BaseAddon) this, 1208, -9, 2, 0, 2052, -1, "", 1);// 123
			AddComplexComponent( (BaseAddon) this, 1208, -9, 3, 0, 2052, -1, "", 1);// 124
			AddComplexComponent( (BaseAddon) this, 1208, -9, 4, 0, 2052, -1, "", 1);// 125
			AddComplexComponent( (BaseAddon) this, 128, -9, -4, 0, 2052, -1, "", 1);// 126
			AddComplexComponent( (BaseAddon) this, 128, -9, -3, 0, 2052, -1, "", 1);// 127
			AddComplexComponent( (BaseAddon) this, 128, -9, -2, 0, 2052, -1, "", 1);// 128
			AddComplexComponent( (BaseAddon) this, 128, -9, -1, 0, 2052, -1, "", 1);// 129
			AddComplexComponent( (BaseAddon) this, 128, -9, 0, 0, 2052, -1, "", 1);// 130
			AddComplexComponent( (BaseAddon) this, 128, -9, 1, 0, 2052, -1, "", 1);// 131
			AddComplexComponent( (BaseAddon) this, 128, -9, 2, 0, 2052, -1, "", 1);// 132
			AddComplexComponent( (BaseAddon) this, 128, -9, 3, 0, 2052, -1, "", 1);// 133
			AddComplexComponent( (BaseAddon) this, 128, -9, 4, 0, 2052, -1, "", 1);// 134
			AddComplexComponent( (BaseAddon) this, 128, -15, 5, 0, 2052, -1, "", 1);// 135
			AddComplexComponent( (BaseAddon) this, 128, -15, 6, 0, 2052, -1, "", 1);// 136
			AddComplexComponent( (BaseAddon) this, 128, -15, 7, 0, 2052, -1, "", 1);// 137
			AddComplexComponent( (BaseAddon) this, 128, -15, 8, 0, 2052, -1, "", 1);// 138
			AddComplexComponent( (BaseAddon) this, 128, -15, 9, 0, 2052, -1, "", 1);// 139
			AddComplexComponent( (BaseAddon) this, 128, -15, 10, 0, 2052, -1, "", 1);// 140
			AddComplexComponent( (BaseAddon) this, 128, -15, 11, 0, 2052, -1, "", 1);// 141
			AddComplexComponent( (BaseAddon) this, 128, -15, 12, 0, 2052, -1, "", 1);// 142
			AddComplexComponent( (BaseAddon) this, 128, -15, 13, 0, 2052, -1, "", 1);// 143
			AddComplexComponent( (BaseAddon) this, 1208, -14, 5, 0, 2052, -1, "", 1);// 144
			AddComplexComponent( (BaseAddon) this, 1208, -14, 6, 0, 2052, -1, "", 1);// 145
			AddComplexComponent( (BaseAddon) this, 1208, -14, 7, 0, 2052, -1, "", 1);// 146
			AddComplexComponent( (BaseAddon) this, 1208, -14, 8, 0, 2052, -1, "", 1);// 147
			AddComplexComponent( (BaseAddon) this, 1208, -14, 9, 0, 2052, -1, "", 1);// 148
			AddComplexComponent( (BaseAddon) this, 1208, -14, 10, 0, 2052, -1, "", 1);// 149
			AddComplexComponent( (BaseAddon) this, 1208, -14, 11, 0, 2052, -1, "", 1);// 150
			AddComplexComponent( (BaseAddon) this, 1208, -14, 12, 0, 2052, -1, "", 1);// 151
			AddComplexComponent( (BaseAddon) this, 1208, -14, 13, 0, 2052, -1, "", 1);// 152
			AddComplexComponent( (BaseAddon) this, 1208, -13, 5, 0, 2052, -1, "", 1);// 153
			AddComplexComponent( (BaseAddon) this, 1208, -13, 6, 0, 2052, -1, "", 1);// 154
			AddComplexComponent( (BaseAddon) this, 1208, -13, 7, 0, 2052, -1, "", 1);// 155
			AddComplexComponent( (BaseAddon) this, 1208, -13, 8, 0, 2052, -1, "", 1);// 156
			AddComplexComponent( (BaseAddon) this, 1208, -13, 9, 0, 2052, -1, "", 1);// 157
			AddComplexComponent( (BaseAddon) this, 1208, -13, 10, 0, 2052, -1, "", 1);// 158
			AddComplexComponent( (BaseAddon) this, 1208, -13, 11, 0, 2052, -1, "", 1);// 159
			AddComplexComponent( (BaseAddon) this, 1208, -13, 12, 0, 2052, -1, "", 1);// 160
			AddComplexComponent( (BaseAddon) this, 1208, -13, 13, 0, 2052, -1, "", 1);// 161
			AddComplexComponent( (BaseAddon) this, 1208, -12, 5, 0, 2052, -1, "", 1);// 162
			AddComplexComponent( (BaseAddon) this, 1208, -12, 6, 0, 2052, -1, "", 1);// 163
			AddComplexComponent( (BaseAddon) this, 1208, -12, 7, 0, 2052, -1, "", 1);// 164
			AddComplexComponent( (BaseAddon) this, 1208, -12, 8, 0, 2052, -1, "", 1);// 165
			AddComplexComponent( (BaseAddon) this, 1208, -12, 9, 0, 2052, -1, "", 1);// 166
			AddComplexComponent( (BaseAddon) this, 1208, -12, 10, 0, 2052, -1, "", 1);// 167
			AddComplexComponent( (BaseAddon) this, 1208, -12, 11, 0, 2052, -1, "", 1);// 168
			AddComplexComponent( (BaseAddon) this, 1208, -12, 12, 0, 2052, -1, "", 1);// 169
			AddComplexComponent( (BaseAddon) this, 1208, -12, 13, 0, 2052, -1, "", 1);// 170
			AddComplexComponent( (BaseAddon) this, 1208, -11, 5, 0, 2052, -1, "", 1);// 171
			AddComplexComponent( (BaseAddon) this, 1208, -11, 6, 0, 2052, -1, "", 1);// 172
			AddComplexComponent( (BaseAddon) this, 1208, -11, 7, 0, 2052, -1, "", 1);// 173
			AddComplexComponent( (BaseAddon) this, 1208, -11, 8, 0, 2052, -1, "", 1);// 174
			AddComplexComponent( (BaseAddon) this, 1208, -11, 9, 0, 2052, -1, "", 1);// 175
			AddComplexComponent( (BaseAddon) this, 1208, -11, 10, 0, 2052, -1, "", 1);// 176
			AddComplexComponent( (BaseAddon) this, 1208, -11, 11, 0, 2052, -1, "", 1);// 177
			AddComplexComponent( (BaseAddon) this, 1208, -11, 12, 0, 2052, -1, "", 1);// 178
			AddComplexComponent( (BaseAddon) this, 1208, -11, 13, 0, 2052, -1, "", 1);// 179
			AddComplexComponent( (BaseAddon) this, 1208, -10, 5, 0, 2052, -1, "", 1);// 180
			AddComplexComponent( (BaseAddon) this, 1208, -10, 6, 0, 2052, -1, "", 1);// 181
			AddComplexComponent( (BaseAddon) this, 1208, -10, 7, 0, 2052, -1, "", 1);// 182
			AddComplexComponent( (BaseAddon) this, 1208, -10, 8, 0, 2052, -1, "", 1);// 183
			AddComplexComponent( (BaseAddon) this, 1208, -10, 9, 0, 2052, -1, "", 1);// 184
			AddComplexComponent( (BaseAddon) this, 1208, -10, 10, 0, 2052, -1, "", 1);// 185
			AddComplexComponent( (BaseAddon) this, 1208, -10, 11, 0, 2052, -1, "", 1);// 186
			AddComplexComponent( (BaseAddon) this, 1208, -10, 12, 0, 2052, -1, "", 1);// 187
			AddComplexComponent( (BaseAddon) this, 1208, -10, 13, 0, 2052, -1, "", 1);// 188
			AddComplexComponent( (BaseAddon) this, 1208, -9, 5, 0, 2052, -1, "", 1);// 189
			AddComplexComponent( (BaseAddon) this, 1208, -9, 6, 0, 2052, -1, "", 1);// 190
			AddComplexComponent( (BaseAddon) this, 1208, -9, 7, 0, 2052, -1, "", 1);// 191
			AddComplexComponent( (BaseAddon) this, 1208, -9, 8, 0, 2052, -1, "", 1);// 192
			AddComplexComponent( (BaseAddon) this, 1208, -9, 9, 0, 2052, -1, "", 1);// 193
			AddComplexComponent( (BaseAddon) this, 1208, -9, 10, 0, 2052, -1, "", 1);// 194
			AddComplexComponent( (BaseAddon) this, 1208, -9, 11, 0, 2052, -1, "", 1);// 195
			AddComplexComponent( (BaseAddon) this, 1208, -9, 12, 0, 2052, -1, "", 1);// 196
			AddComplexComponent( (BaseAddon) this, 1208, -9, 13, 0, 2052, -1, "", 1);// 197
			AddComplexComponent( (BaseAddon) this, 128, -9, 5, 0, 2052, -1, "", 1);// 198
			AddComplexComponent( (BaseAddon) this, 128, -8, -12, 0, 2052, -1, "", 1);// 199
			AddComplexComponent( (BaseAddon) this, 128, -7, -12, 0, 2052, -1, "", 1);// 200
			AddComplexComponent( (BaseAddon) this, 128, -6, -12, 0, 2052, -1, "", 1);// 201
			AddComplexComponent( (BaseAddon) this, 128, -5, -12, 0, 2052, -1, "", 1);// 202
			AddComplexComponent( (BaseAddon) this, 128, -4, -12, 0, 2052, -1, "", 1);// 203
			AddComplexComponent( (BaseAddon) this, 128, -3, -12, 0, 2052, -1, "", 1);// 204
			AddComplexComponent( (BaseAddon) this, 128, -2, -12, 0, 2052, -1, "", 1);// 205
			AddComplexComponent( (BaseAddon) this, 128, -1, -12, 0, 2052, -1, "", 1);// 206
			AddComplexComponent( (BaseAddon) this, 128, 0, -12, 0, 2052, -1, "", 1);// 207
			AddComplexComponent( (BaseAddon) this, 128, 1, -12, 0, 2052, -1, "", 1);// 208
			AddComplexComponent( (BaseAddon) this, 128, 2, -12, 0, 2052, -1, "", 1);// 209
			AddComplexComponent( (BaseAddon) this, 128, 3, -12, 0, 2052, -1, "", 1);// 210
			AddComplexComponent( (BaseAddon) this, 128, 4, -12, 0, 2052, -1, "", 1);// 211
			AddComplexComponent( (BaseAddon) this, 128, 5, -12, 0, 2052, -1, "", 1);// 212
			AddComplexComponent( (BaseAddon) this, 128, 6, -12, 0, 2052, -1, "", 1);// 213
			AddComplexComponent( (BaseAddon) this, 128, 7, -12, 0, 2052, -1, "", 1);// 214
			AddComplexComponent( (BaseAddon) this, 1208, -8, -12, 0, 2052, -1, "", 1);// 215
			AddComplexComponent( (BaseAddon) this, 1208, -7, -12, 0, 2052, -1, "", 1);// 216
			AddComplexComponent( (BaseAddon) this, 1208, -6, -12, 0, 2052, -1, "", 1);// 217
			AddComplexComponent( (BaseAddon) this, 1208, -5, -12, 0, 2052, -1, "", 1);// 218
			AddComplexComponent( (BaseAddon) this, 1208, -4, -12, 0, 2052, -1, "", 1);// 219
			AddComplexComponent( (BaseAddon) this, 1208, -3, -12, 0, 2052, -1, "", 1);// 220
			AddComplexComponent( (BaseAddon) this, 1208, -2, -12, 0, 2052, -1, "", 1);// 221
			AddComplexComponent( (BaseAddon) this, 1208, -1, -12, 0, 2052, -1, "", 1);// 222
			AddComplexComponent( (BaseAddon) this, 1208, 0, -12, 0, 2052, -1, "", 1);// 223
			AddComplexComponent( (BaseAddon) this, 1208, 1, -12, 0, 2052, -1, "", 1);// 224
			AddComplexComponent( (BaseAddon) this, 1208, 2, -12, 0, 2052, -1, "", 1);// 225
			AddComplexComponent( (BaseAddon) this, 1208, 3, -12, 0, 2052, -1, "", 1);// 226
			AddComplexComponent( (BaseAddon) this, 1208, 4, -12, 0, 2052, -1, "", 1);// 227
			AddComplexComponent( (BaseAddon) this, 1208, 5, -12, 0, 2052, -1, "", 1);// 228
			AddComplexComponent( (BaseAddon) this, 1208, 6, -12, 0, 2052, -1, "", 1);// 229
			AddComplexComponent( (BaseAddon) this, 1208, 7, -12, 0, 2052, -1, "", 1);// 230
			AddComplexComponent( (BaseAddon) this, 1208, -8, -11, 0, 2052, -1, "", 1);// 231
			AddComplexComponent( (BaseAddon) this, 1208, -8, -10, 0, 2052, -1, "", 1);// 232
			AddComplexComponent( (BaseAddon) this, 1208, -8, -9, 0, 2052, -1, "", 1);// 233
			AddComplexComponent( (BaseAddon) this, 1208, -8, -8, 0, 2052, -1, "", 1);// 234
			AddComplexComponent( (BaseAddon) this, 1208, -8, -7, 0, 2052, -1, "", 1);// 235
			AddComplexComponent( (BaseAddon) this, 1208, -8, -6, 0, 2052, -1, "", 1);// 236
			AddComplexComponent( (BaseAddon) this, 1208, -8, -5, 0, 2052, -1, "", 1);// 237
			AddComplexComponent( (BaseAddon) this, 1208, -7, -11, 0, 2052, -1, "", 1);// 247
			AddComplexComponent( (BaseAddon) this, 1208, -7, -10, 0, 2052, -1, "", 1);// 248
			AddComplexComponent( (BaseAddon) this, 1208, -7, -9, 0, 2052, -1, "", 1);// 249
			AddComplexComponent( (BaseAddon) this, 1208, -7, -8, 0, 2052, -1, "", 1);// 250
			AddComplexComponent( (BaseAddon) this, 1208, -7, -7, 0, 2052, -1, "", 1);// 251
			AddComplexComponent( (BaseAddon) this, 1208, -7, -6, 0, 2052, -1, "", 1);// 252
			AddComplexComponent( (BaseAddon) this, 1208, -7, -5, 0, 2052, -1, "", 1);// 253
			AddComplexComponent( (BaseAddon) this, 1208, -6, -11, 0, 2052, -1, "", 1);// 263
			AddComplexComponent( (BaseAddon) this, 1208, -6, -10, 0, 2052, -1, "", 1);// 264
			AddComplexComponent( (BaseAddon) this, 1208, -6, -9, 0, 2052, -1, "", 1);// 265
			AddComplexComponent( (BaseAddon) this, 1208, -6, -8, 0, 2052, -1, "", 1);// 266
			AddComplexComponent( (BaseAddon) this, 1208, -6, -7, 0, 2052, -1, "", 1);// 267
			AddComplexComponent( (BaseAddon) this, 1208, -6, -6, 0, 2052, -1, "", 1);// 268
			AddComplexComponent( (BaseAddon) this, 1208, -6, -5, 0, 2052, -1, "", 1);// 269
			AddComplexComponent( (BaseAddon) this, 1208, -5, -11, 0, 2052, -1, "", 1);// 279
			AddComplexComponent( (BaseAddon) this, 1208, -5, -10, 0, 2052, -1, "", 1);// 280
			AddComplexComponent( (BaseAddon) this, 1208, -5, -9, 0, 2052, -1, "", 1);// 281
			AddComplexComponent( (BaseAddon) this, 1208, -5, -8, 0, 2052, -1, "", 1);// 282
			AddComplexComponent( (BaseAddon) this, 1208, -5, -7, 0, 2052, -1, "", 1);// 283
			AddComplexComponent( (BaseAddon) this, 1208, -5, -6, 0, 2052, -1, "", 1);// 284
			AddComplexComponent( (BaseAddon) this, 1208, -5, -5, 0, 2052, -1, "", 1);// 285
			AddComplexComponent( (BaseAddon) this, 1208, -4, -11, 0, 2052, -1, "", 1);// 295
			AddComplexComponent( (BaseAddon) this, 1208, -4, -10, 0, 2052, -1, "", 1);// 296
			AddComplexComponent( (BaseAddon) this, 1208, -4, -9, 0, 2052, -1, "", 1);// 297
			AddComplexComponent( (BaseAddon) this, 1208, -4, -8, 0, 2052, -1, "", 1);// 298
			AddComplexComponent( (BaseAddon) this, 1208, -4, -7, 0, 2052, -1, "", 1);// 299
			AddComplexComponent( (BaseAddon) this, 1208, -4, -6, 0, 2052, -1, "", 1);// 300
			AddComplexComponent( (BaseAddon) this, 1208, -4, -5, 0, 2052, -1, "", 1);// 301
			AddComplexComponent( (BaseAddon) this, 1208, -3, -11, 0, 2052, -1, "", 1);// 311
			AddComplexComponent( (BaseAddon) this, 1208, -3, -10, 0, 2052, -1, "", 1);// 312
			AddComplexComponent( (BaseAddon) this, 1208, -3, -9, 0, 2052, -1, "", 1);// 313
			AddComplexComponent( (BaseAddon) this, 1208, -3, -8, 0, 2052, -1, "", 1);// 314
			AddComplexComponent( (BaseAddon) this, 1208, -3, -7, 0, 2052, -1, "", 1);// 315
			AddComplexComponent( (BaseAddon) this, 1208, -3, -6, 0, 2052, -1, "", 1);// 316
			AddComplexComponent( (BaseAddon) this, 1208, -3, -5, 0, 2052, -1, "", 1);// 317
			AddComplexComponent( (BaseAddon) this, 1208, -2, -11, 0, 2052, -1, "", 1);// 327
			AddComplexComponent( (BaseAddon) this, 1208, -2, -10, 0, 2052, -1, "", 1);// 328
			AddComplexComponent( (BaseAddon) this, 1208, -2, -9, 0, 2052, -1, "", 1);// 329
			AddComplexComponent( (BaseAddon) this, 1208, -2, -8, 0, 2052, -1, "", 1);// 330
			AddComplexComponent( (BaseAddon) this, 1208, -2, -7, 0, 2052, -1, "", 1);// 331
			AddComplexComponent( (BaseAddon) this, 1208, -2, -6, 0, 2052, -1, "", 1);// 332
			AddComplexComponent( (BaseAddon) this, 1208, -2, -5, 0, 2052, -1, "", 1);// 333
			AddComplexComponent( (BaseAddon) this, 1208, -1, -11, 0, 2052, -1, "", 1);// 343
			AddComplexComponent( (BaseAddon) this, 1208, -1, -10, 0, 2052, -1, "", 1);// 344
			AddComplexComponent( (BaseAddon) this, 1208, -1, -9, 0, 2052, -1, "", 1);// 345
			AddComplexComponent( (BaseAddon) this, 1208, -1, -8, 0, 2052, -1, "", 1);// 346
			AddComplexComponent( (BaseAddon) this, 1208, -1, -7, 0, 2052, -1, "", 1);// 347
			AddComplexComponent( (BaseAddon) this, 1208, -1, -6, 0, 2052, -1, "", 1);// 348
			AddComplexComponent( (BaseAddon) this, 1208, -1, -5, 0, 2052, -1, "", 1);// 349
			AddComplexComponent( (BaseAddon) this, 1208, 0, -11, 0, 2052, -1, "", 1);// 359
			AddComplexComponent( (BaseAddon) this, 1208, 0, -10, 0, 2052, -1, "", 1);// 360
			AddComplexComponent( (BaseAddon) this, 1208, 0, -9, 0, 2052, -1, "", 1);// 361
			AddComplexComponent( (BaseAddon) this, 1208, 0, -8, 0, 2052, -1, "", 1);// 362
			AddComplexComponent( (BaseAddon) this, 1208, 0, -7, 0, 2052, -1, "", 1);// 363
			AddComplexComponent( (BaseAddon) this, 1208, 0, -6, 0, 2052, -1, "", 1);// 364
			AddComplexComponent( (BaseAddon) this, 1208, 0, -5, 0, 2052, -1, "", 1);// 365
			AddComplexComponent( (BaseAddon) this, 1208, 1, -11, 0, 2052, -1, "", 1);// 375
			AddComplexComponent( (BaseAddon) this, 1208, 1, -10, 0, 2052, -1, "", 1);// 376
			AddComplexComponent( (BaseAddon) this, 1208, 1, -9, 0, 2052, -1, "", 1);// 377
			AddComplexComponent( (BaseAddon) this, 1208, 1, -8, 0, 2052, -1, "", 1);// 378
			AddComplexComponent( (BaseAddon) this, 1208, 1, -7, 0, 2052, -1, "", 1);// 379
			AddComplexComponent( (BaseAddon) this, 1208, 1, -6, 0, 2052, -1, "", 1);// 380
			AddComplexComponent( (BaseAddon) this, 1208, 1, -5, 0, 2052, -1, "", 1);// 381
			AddComplexComponent( (BaseAddon) this, 1208, 1, -4, 0, 2052, -1, "", 1);// 382
			AddComplexComponent( (BaseAddon) this, 1208, 1, 3, 0, 2052, -1, "", 1);// 389
			AddComplexComponent( (BaseAddon) this, 1208, 1, 4, 0, 2052, -1, "", 1);// 390
			AddComplexComponent( (BaseAddon) this, 1208, 2, -11, 0, 2052, -1, "", 1);// 391
			AddComplexComponent( (BaseAddon) this, 1208, 2, -10, 0, 2052, -1, "", 1);// 392
			AddComplexComponent( (BaseAddon) this, 1208, 2, -9, 0, 2052, -1, "", 1);// 393
			AddComplexComponent( (BaseAddon) this, 1208, 2, -8, 0, 2052, -1, "", 1);// 394
			AddComplexComponent( (BaseAddon) this, 1208, 2, -7, 0, 2052, -1, "", 1);// 395
			AddComplexComponent( (BaseAddon) this, 1208, 2, -6, 0, 2052, -1, "", 1);// 396
			AddComplexComponent( (BaseAddon) this, 1208, 2, -5, 0, 2052, -1, "", 1);// 397
			AddComplexComponent( (BaseAddon) this, 1208, 2, -4, 0, 2052, -1, "", 1);// 398
			AddComplexComponent( (BaseAddon) this, 1208, 2, 3, 0, 2052, -1, "", 1);// 405
			AddComplexComponent( (BaseAddon) this, 1208, 2, 4, 0, 2052, -1, "", 1);// 406
			AddComplexComponent( (BaseAddon) this, 1208, 3, -11, 0, 2052, -1, "", 1);// 407
			AddComplexComponent( (BaseAddon) this, 1208, 3, -10, 0, 2052, -1, "", 1);// 408
			AddComplexComponent( (BaseAddon) this, 1208, 3, -9, 0, 2052, -1, "", 1);// 409
			AddComplexComponent( (BaseAddon) this, 1208, 3, -8, 0, 2052, -1, "", 1);// 410
			AddComplexComponent( (BaseAddon) this, 1208, 3, -7, 0, 2052, -1, "", 1);// 411
			AddComplexComponent( (BaseAddon) this, 1208, 3, -6, 0, 2052, -1, "", 1);// 412
			AddComplexComponent( (BaseAddon) this, 1208, 3, -5, 0, 2052, -1, "", 1);// 413
			AddComplexComponent( (BaseAddon) this, 1208, 4, -11, 0, 2052, -1, "", 1);// 423
			AddComplexComponent( (BaseAddon) this, 1208, 4, -10, 0, 2052, -1, "", 1);// 424
			AddComplexComponent( (BaseAddon) this, 1208, 4, -9, 0, 2052, -1, "", 1);// 425
			AddComplexComponent( (BaseAddon) this, 1208, 4, -8, 0, 2052, -1, "", 1);// 426
			AddComplexComponent( (BaseAddon) this, 1208, 4, -7, 0, 2052, -1, "", 1);// 427
			AddComplexComponent( (BaseAddon) this, 1208, 4, -6, 0, 2052, -1, "", 1);// 428
			AddComplexComponent( (BaseAddon) this, 1208, 4, -5, 0, 2052, -1, "", 1);// 429
			AddComplexComponent( (BaseAddon) this, 1208, 5, -11, 0, 2052, -1, "", 1);// 439
			AddComplexComponent( (BaseAddon) this, 1208, 5, -10, 0, 2052, -1, "", 1);// 440
			AddComplexComponent( (BaseAddon) this, 1208, 5, -9, 0, 2052, -1, "", 1);// 441
			AddComplexComponent( (BaseAddon) this, 1208, 5, -8, 0, 2052, -1, "", 1);// 442
			AddComplexComponent( (BaseAddon) this, 1208, 5, -7, 0, 2052, -1, "", 1);// 443
			AddComplexComponent( (BaseAddon) this, 1208, 5, -6, 0, 2052, -1, "", 1);// 444
			AddComplexComponent( (BaseAddon) this, 1208, 5, -5, 0, 2052, -1, "", 1);// 445
			AddComplexComponent( (BaseAddon) this, 1208, 6, -11, 0, 2052, -1, "", 1);// 455
			AddComplexComponent( (BaseAddon) this, 1208, 6, -10, 0, 2052, -1, "", 1);// 456
			AddComplexComponent( (BaseAddon) this, 1208, 6, -9, 0, 2052, -1, "", 1);// 457
			AddComplexComponent( (BaseAddon) this, 1208, 6, -8, 0, 2052, -1, "", 1);// 458
			AddComplexComponent( (BaseAddon) this, 1208, 6, -7, 0, 2052, -1, "", 1);// 459
			AddComplexComponent( (BaseAddon) this, 1208, 6, -6, 0, 2052, -1, "", 1);// 460
			AddComplexComponent( (BaseAddon) this, 1208, 6, -5, 0, 2052, -1, "", 1);// 461
			AddComplexComponent( (BaseAddon) this, 1208, 7, -11, 0, 2052, -1, "", 1);// 471
			AddComplexComponent( (BaseAddon) this, 1208, 7, -10, 0, 2052, -1, "", 1);// 472
			AddComplexComponent( (BaseAddon) this, 1208, 7, -9, 0, 2052, -1, "", 1);// 473
			AddComplexComponent( (BaseAddon) this, 1208, 7, -8, 0, 2052, -1, "", 1);// 474
			AddComplexComponent( (BaseAddon) this, 1208, 7, -7, 0, 2052, -1, "", 1);// 475
			AddComplexComponent( (BaseAddon) this, 1208, 7, -6, 0, 2052, -1, "", 1);// 476
			AddComplexComponent( (BaseAddon) this, 1208, 7, -5, 0, 2052, -1, "", 1);// 477
			AddComplexComponent( (BaseAddon) this, 128, -8, -4, 0, 2052, -1, "", 1);// 487
			AddComplexComponent( (BaseAddon) this, 128, -7, -4, 0, 2052, -1, "", 1);// 488
			AddComplexComponent( (BaseAddon) this, 128, -6, -4, 0, 2052, -1, "", 1);// 489
			AddComplexComponent( (BaseAddon) this, 128, -5, -4, 0, 2052, -1, "", 1);// 490
			AddComplexComponent( (BaseAddon) this, 128, -4, -4, 0, 2052, -1, "", 1);// 491
			AddComplexComponent( (BaseAddon) this, 128, -3, -4, 0, 2052, -1, "", 1);// 492
			AddComplexComponent( (BaseAddon) this, 128, -2, -4, 0, 2052, -1, "", 1);// 493
			AddComplexComponent( (BaseAddon) this, 128, -1, -4, 0, 2052, -1, "", 1);// 494
			AddComplexComponent( (BaseAddon) this, 128, 0, -4, 0, 2052, -1, "", 1);// 495
			AddComplexComponent( (BaseAddon) this, 128, 1, -4, 0, 2052, -1, "", 1);// 496
			AddComplexComponent( (BaseAddon) this, 128, 2, -4, 0, 2052, -1, "", 1);// 497
			AddComplexComponent( (BaseAddon) this, 128, 3, -4, 0, 2052, -1, "", 1);// 498
			AddComplexComponent( (BaseAddon) this, 128, 4, -4, 0, 2052, -1, "", 1);// 499
			AddComplexComponent( (BaseAddon) this, 128, 5, -4, 0, 2052, -1, "", 1);// 500
			AddComplexComponent( (BaseAddon) this, 128, 6, -4, 0, 2052, -1, "", 1);// 501
			AddComplexComponent( (BaseAddon) this, 128, 7, -4, 0, 2052, -1, "", 1);// 502
			AddComplexComponent( (BaseAddon) this, 1208, -8, 6, 0, 2052, -1, "", 1);// 504
			AddComplexComponent( (BaseAddon) this, 1208, -8, 7, 0, 2052, -1, "", 1);// 505
			AddComplexComponent( (BaseAddon) this, 1208, -8, 8, 0, 2052, -1, "", 1);// 506
			AddComplexComponent( (BaseAddon) this, 1208, -8, 9, 0, 2052, -1, "", 1);// 507
			AddComplexComponent( (BaseAddon) this, 1208, -8, 10, 0, 2052, -1, "", 1);// 508
			AddComplexComponent( (BaseAddon) this, 1208, -8, 11, 0, 2052, -1, "", 1);// 509
			AddComplexComponent( (BaseAddon) this, 1208, -8, 12, 0, 2052, -1, "", 1);// 510
			AddComplexComponent( (BaseAddon) this, 1208, -8, 13, 0, 2052, -1, "", 1);// 511
			AddComplexComponent( (BaseAddon) this, 1208, -7, 6, 0, 2052, -1, "", 1);// 513
			AddComplexComponent( (BaseAddon) this, 1208, -7, 7, 0, 2052, -1, "", 1);// 514
			AddComplexComponent( (BaseAddon) this, 1208, -7, 8, 0, 2052, -1, "", 1);// 515
			AddComplexComponent( (BaseAddon) this, 1208, -7, 9, 0, 2052, -1, "", 1);// 516
			AddComplexComponent( (BaseAddon) this, 1208, -7, 10, 0, 2052, -1, "", 1);// 517
			AddComplexComponent( (BaseAddon) this, 1208, -7, 11, 0, 2052, -1, "", 1);// 518
			AddComplexComponent( (BaseAddon) this, 1208, -7, 12, 0, 2052, -1, "", 1);// 519
			AddComplexComponent( (BaseAddon) this, 1208, -7, 13, 0, 2052, -1, "", 1);// 520
			AddComplexComponent( (BaseAddon) this, 1208, -6, 6, 0, 2052, -1, "", 1);// 522
			AddComplexComponent( (BaseAddon) this, 1208, -6, 7, 0, 2052, -1, "", 1);// 523
			AddComplexComponent( (BaseAddon) this, 1208, -6, 8, 0, 2052, -1, "", 1);// 524
			AddComplexComponent( (BaseAddon) this, 1208, -6, 9, 0, 2052, -1, "", 1);// 525
			AddComplexComponent( (BaseAddon) this, 1208, -6, 10, 0, 2052, -1, "", 1);// 526
			AddComplexComponent( (BaseAddon) this, 1208, -6, 11, 0, 2052, -1, "", 1);// 527
			AddComplexComponent( (BaseAddon) this, 1208, -6, 12, 0, 2052, -1, "", 1);// 528
			AddComplexComponent( (BaseAddon) this, 1208, -6, 13, 0, 2052, -1, "", 1);// 529
			AddComplexComponent( (BaseAddon) this, 1208, -5, 6, 0, 2052, -1, "", 1);// 531
			AddComplexComponent( (BaseAddon) this, 1208, -5, 7, 0, 2052, -1, "", 1);// 532
			AddComplexComponent( (BaseAddon) this, 1208, -5, 8, 0, 2052, -1, "", 1);// 533
			AddComplexComponent( (BaseAddon) this, 1208, -5, 9, 0, 2052, -1, "", 1);// 534
			AddComplexComponent( (BaseAddon) this, 1208, -5, 10, 0, 2052, -1, "", 1);// 535
			AddComplexComponent( (BaseAddon) this, 1208, -5, 11, 0, 2052, -1, "", 1);// 536
			AddComplexComponent( (BaseAddon) this, 1208, -5, 12, 0, 2052, -1, "", 1);// 537
			AddComplexComponent( (BaseAddon) this, 1208, -5, 13, 0, 2052, -1, "", 1);// 538
			AddComplexComponent( (BaseAddon) this, 1208, -4, 6, 0, 2052, -1, "", 1);// 540
			AddComplexComponent( (BaseAddon) this, 1208, -4, 7, 0, 2052, -1, "", 1);// 541
			AddComplexComponent( (BaseAddon) this, 1208, -4, 8, 0, 2052, -1, "", 1);// 542
			AddComplexComponent( (BaseAddon) this, 1208, -4, 9, 0, 2052, -1, "", 1);// 543
			AddComplexComponent( (BaseAddon) this, 1208, -4, 10, 0, 2052, -1, "", 1);// 544
			AddComplexComponent( (BaseAddon) this, 1208, -4, 11, 0, 2052, -1, "", 1);// 545
			AddComplexComponent( (BaseAddon) this, 1208, -4, 12, 0, 2052, -1, "", 1);// 546
			AddComplexComponent( (BaseAddon) this, 1208, -4, 13, 0, 2052, -1, "", 1);// 547
			AddComplexComponent( (BaseAddon) this, 1208, -3, 6, 0, 2052, -1, "", 1);// 549
			AddComplexComponent( (BaseAddon) this, 1208, -3, 7, 0, 2052, -1, "", 1);// 550
			AddComplexComponent( (BaseAddon) this, 1208, -3, 8, 0, 2052, -1, "", 1);// 551
			AddComplexComponent( (BaseAddon) this, 1208, -3, 9, 0, 2052, -1, "", 1);// 552
			AddComplexComponent( (BaseAddon) this, 1208, -3, 10, 0, 2052, -1, "", 1);// 553
			AddComplexComponent( (BaseAddon) this, 1208, -3, 11, 0, 2052, -1, "", 1);// 554
			AddComplexComponent( (BaseAddon) this, 1208, -3, 12, 0, 2052, -1, "", 1);// 555
			AddComplexComponent( (BaseAddon) this, 1208, -3, 13, 0, 2052, -1, "", 1);// 556
			AddComplexComponent( (BaseAddon) this, 1208, -2, 6, 0, 2052, -1, "", 1);// 558
			AddComplexComponent( (BaseAddon) this, 1208, -2, 7, 0, 2052, -1, "", 1);// 559
			AddComplexComponent( (BaseAddon) this, 1208, -2, 8, 0, 2052, -1, "", 1);// 560
			AddComplexComponent( (BaseAddon) this, 1208, -2, 9, 0, 2052, -1, "", 1);// 561
			AddComplexComponent( (BaseAddon) this, 1208, -2, 10, 0, 2052, -1, "", 1);// 562
			AddComplexComponent( (BaseAddon) this, 1208, -2, 11, 0, 2052, -1, "", 1);// 563
			AddComplexComponent( (BaseAddon) this, 1208, -2, 12, 0, 2052, -1, "", 1);// 564
			AddComplexComponent( (BaseAddon) this, 1208, -2, 13, 0, 2052, -1, "", 1);// 565
			AddComplexComponent( (BaseAddon) this, 1208, -1, 6, 0, 2052, -1, "", 1);// 567
			AddComplexComponent( (BaseAddon) this, 1208, -1, 7, 0, 2052, -1, "", 1);// 568
			AddComplexComponent( (BaseAddon) this, 1208, -1, 8, 0, 2052, -1, "", 1);// 569
			AddComplexComponent( (BaseAddon) this, 1208, -1, 9, 0, 2052, -1, "", 1);// 570
			AddComplexComponent( (BaseAddon) this, 1208, -1, 10, 0, 2052, -1, "", 1);// 571
			AddComplexComponent( (BaseAddon) this, 1208, -1, 11, 0, 2052, -1, "", 1);// 572
			AddComplexComponent( (BaseAddon) this, 1208, -1, 12, 0, 2052, -1, "", 1);// 573
			AddComplexComponent( (BaseAddon) this, 1208, -1, 13, 0, 2052, -1, "", 1);// 574
			AddComplexComponent( (BaseAddon) this, 1208, 0, 6, 0, 2052, -1, "", 1);// 576
			AddComplexComponent( (BaseAddon) this, 1208, 0, 7, 0, 2052, -1, "", 1);// 577
			AddComplexComponent( (BaseAddon) this, 1208, 0, 8, 0, 2052, -1, "", 1);// 578
			AddComplexComponent( (BaseAddon) this, 1208, 0, 9, 0, 2052, -1, "", 1);// 579
			AddComplexComponent( (BaseAddon) this, 1208, 0, 10, 0, 2052, -1, "", 1);// 580
			AddComplexComponent( (BaseAddon) this, 1208, 0, 11, 0, 2052, -1, "", 1);// 581
			AddComplexComponent( (BaseAddon) this, 1208, 0, 12, 0, 2052, -1, "", 1);// 582
			AddComplexComponent( (BaseAddon) this, 1208, 0, 13, 0, 2052, -1, "", 1);// 583
			AddComplexComponent( (BaseAddon) this, 1208, 1, 5, 0, 2052, -1, "", 1);// 584
			AddComplexComponent( (BaseAddon) this, 1208, 1, 6, 0, 2052, -1, "", 1);// 585
			AddComplexComponent( (BaseAddon) this, 1208, 1, 7, 0, 2052, -1, "", 1);// 586
			AddComplexComponent( (BaseAddon) this, 1208, 1, 8, 0, 2052, -1, "", 1);// 587
			AddComplexComponent( (BaseAddon) this, 1208, 1, 9, 0, 2052, -1, "", 1);// 588
			AddComplexComponent( (BaseAddon) this, 1208, 1, 10, 0, 2052, -1, "", 1);// 589
			AddComplexComponent( (BaseAddon) this, 1208, 1, 11, 0, 2052, -1, "", 1);// 590
			AddComplexComponent( (BaseAddon) this, 1208, 1, 12, 0, 2052, -1, "", 1);// 591
			AddComplexComponent( (BaseAddon) this, 1208, 1, 13, 0, 2052, -1, "", 1);// 592
			AddComplexComponent( (BaseAddon) this, 1208, 2, 5, 0, 2052, -1, "", 1);// 593
			AddComplexComponent( (BaseAddon) this, 1208, 2, 6, 0, 2052, -1, "", 1);// 594
			AddComplexComponent( (BaseAddon) this, 1208, 2, 7, 0, 2052, -1, "", 1);// 595
			AddComplexComponent( (BaseAddon) this, 1208, 2, 8, 0, 2052, -1, "", 1);// 596
			AddComplexComponent( (BaseAddon) this, 1208, 2, 9, 0, 2052, -1, "", 1);// 597
			AddComplexComponent( (BaseAddon) this, 1208, 2, 10, 0, 2052, -1, "", 1);// 598
			AddComplexComponent( (BaseAddon) this, 1208, 2, 11, 0, 2052, -1, "", 1);// 599
			AddComplexComponent( (BaseAddon) this, 1208, 2, 12, 0, 2052, -1, "", 1);// 600
			AddComplexComponent( (BaseAddon) this, 1208, 2, 13, 0, 2052, -1, "", 1);// 601
			AddComplexComponent( (BaseAddon) this, 1208, 3, 6, 0, 2052, -1, "", 1);// 603
			AddComplexComponent( (BaseAddon) this, 1208, 3, 7, 0, 2052, -1, "", 1);// 604
			AddComplexComponent( (BaseAddon) this, 1208, 3, 8, 0, 2052, -1, "", 1);// 605
			AddComplexComponent( (BaseAddon) this, 1208, 3, 9, 0, 2052, -1, "", 1);// 606
			AddComplexComponent( (BaseAddon) this, 1208, 3, 10, 0, 2052, -1, "", 1);// 607
			AddComplexComponent( (BaseAddon) this, 1208, 3, 11, 0, 2052, -1, "", 1);// 608
			AddComplexComponent( (BaseAddon) this, 1208, 3, 12, 0, 2052, -1, "", 1);// 609
			AddComplexComponent( (BaseAddon) this, 1208, 3, 13, 0, 2052, -1, "", 1);// 610
			AddComplexComponent( (BaseAddon) this, 1208, 4, 6, 0, 2052, -1, "", 1);// 612
			AddComplexComponent( (BaseAddon) this, 1208, 4, 7, 0, 2052, -1, "", 1);// 613
			AddComplexComponent( (BaseAddon) this, 1208, 4, 8, 0, 2052, -1, "", 1);// 614
			AddComplexComponent( (BaseAddon) this, 1208, 4, 9, 0, 2052, -1, "", 1);// 615
			AddComplexComponent( (BaseAddon) this, 1208, 4, 10, 0, 2052, -1, "", 1);// 616
			AddComplexComponent( (BaseAddon) this, 1208, 4, 11, 0, 2052, -1, "", 1);// 617
			AddComplexComponent( (BaseAddon) this, 1208, 4, 12, 0, 2052, -1, "", 1);// 618
			AddComplexComponent( (BaseAddon) this, 1208, 4, 13, 0, 2052, -1, "", 1);// 619
			AddComplexComponent( (BaseAddon) this, 1208, 5, 6, 0, 2052, -1, "", 1);// 621
			AddComplexComponent( (BaseAddon) this, 1208, 5, 7, 0, 2052, -1, "", 1);// 622
			AddComplexComponent( (BaseAddon) this, 1208, 5, 8, 0, 2052, -1, "", 1);// 623
			AddComplexComponent( (BaseAddon) this, 1208, 5, 9, 0, 2052, -1, "", 1);// 624
			AddComplexComponent( (BaseAddon) this, 1208, 5, 10, 0, 2052, -1, "", 1);// 625
			AddComplexComponent( (BaseAddon) this, 1208, 5, 11, 0, 2052, -1, "", 1);// 626
			AddComplexComponent( (BaseAddon) this, 1208, 5, 12, 0, 2052, -1, "", 1);// 627
			AddComplexComponent( (BaseAddon) this, 1208, 5, 13, 0, 2052, -1, "", 1);// 628
			AddComplexComponent( (BaseAddon) this, 1208, 6, 6, 0, 2052, -1, "", 1);// 630
			AddComplexComponent( (BaseAddon) this, 1208, 6, 7, 0, 2052, -1, "", 1);// 631
			AddComplexComponent( (BaseAddon) this, 1208, 6, 8, 0, 2052, -1, "", 1);// 632
			AddComplexComponent( (BaseAddon) this, 1208, 6, 9, 0, 2052, -1, "", 1);// 633
			AddComplexComponent( (BaseAddon) this, 1208, 6, 10, 0, 2052, -1, "", 1);// 634
			AddComplexComponent( (BaseAddon) this, 1208, 6, 11, 0, 2052, -1, "", 1);// 635
			AddComplexComponent( (BaseAddon) this, 1208, 6, 12, 0, 2052, -1, "", 1);// 636
			AddComplexComponent( (BaseAddon) this, 1208, 6, 13, 0, 2052, -1, "", 1);// 637
			AddComplexComponent( (BaseAddon) this, 1208, 7, 6, 0, 2052, -1, "", 1);// 639
			AddComplexComponent( (BaseAddon) this, 1208, 7, 7, 0, 2052, -1, "", 1);// 640
			AddComplexComponent( (BaseAddon) this, 1208, 7, 8, 0, 2052, -1, "", 1);// 641
			AddComplexComponent( (BaseAddon) this, 1208, 7, 9, 0, 2052, -1, "", 1);// 642
			AddComplexComponent( (BaseAddon) this, 1208, 7, 10, 0, 2052, -1, "", 1);// 643
			AddComplexComponent( (BaseAddon) this, 1208, 7, 11, 0, 2052, -1, "", 1);// 644
			AddComplexComponent( (BaseAddon) this, 1208, 7, 12, 0, 2052, -1, "", 1);// 645
			AddComplexComponent( (BaseAddon) this, 1208, 7, 13, 0, 2052, -1, "", 1);// 646
			AddComplexComponent( (BaseAddon) this, 128, -8, 5, 0, 2052, -1, "", 1);// 647
			AddComplexComponent( (BaseAddon) this, 128, -7, 5, 0, 2052, -1, "", 1);// 648
			AddComplexComponent( (BaseAddon) this, 128, -6, 5, 0, 2052, -1, "", 1);// 649
			AddComplexComponent( (BaseAddon) this, 128, -5, 5, 0, 2052, -1, "", 1);// 650
			AddComplexComponent( (BaseAddon) this, 128, -4, 5, 0, 2052, -1, "", 1);// 651
			AddComplexComponent( (BaseAddon) this, 128, -3, 5, 0, 2052, -1, "", 1);// 652
			AddComplexComponent( (BaseAddon) this, 128, -2, 5, 0, 2052, -1, "", 1);// 653
			AddComplexComponent( (BaseAddon) this, 128, -1, 5, 0, 2052, -1, "", 1);// 654
			AddComplexComponent( (BaseAddon) this, 128, 0, 5, 0, 2052, -1, "", 1);// 655
			AddComplexComponent( (BaseAddon) this, 128, 1, 5, 0, 2052, -1, "", 1);// 656
			AddComplexComponent( (BaseAddon) this, 128, 2, 5, 0, 2052, -1, "", 1);// 657
			AddComplexComponent( (BaseAddon) this, 128, 3, 5, 0, 2052, -1, "", 1);// 658
			AddComplexComponent( (BaseAddon) this, 128, 4, 5, 0, 2052, -1, "", 1);// 659
			AddComplexComponent( (BaseAddon) this, 128, 5, 5, 0, 2052, -1, "", 1);// 660
			AddComplexComponent( (BaseAddon) this, 128, 6, 5, 0, 2052, -1, "", 1);// 661
			AddComplexComponent( (BaseAddon) this, 128, 7, 5, 0, 2052, -1, "", 1);// 662
			AddComplexComponent( (BaseAddon) this, 128, 8, -12, 0, 2052, -1, "", 1);// 663
			AddComplexComponent( (BaseAddon) this, 128, 9, -12, 0, 2052, -1, "", 1);// 664
			AddComplexComponent( (BaseAddon) this, 128, 10, -12, 0, 2052, -1, "", 1);// 665
			AddComplexComponent( (BaseAddon) this, 128, 11, -12, 0, 2052, -1, "", 1);// 666
			AddComplexComponent( (BaseAddon) this, 128, 12, -12, 0, 2052, -1, "", 1);// 667
			AddComplexComponent( (BaseAddon) this, 128, 13, -12, 0, 2052, -1, "", 1);// 668
			AddComplexComponent( (BaseAddon) this, 128, 14, -12, 0, 2052, -1, "", 1);// 669
			AddComplexComponent( (BaseAddon) this, 128, 15, -12, 0, 2052, -1, "", 1);// 670
			AddComplexComponent( (BaseAddon) this, 128, 16, -12, 0, 2052, -1, "", 1);// 671
			AddComplexComponent( (BaseAddon) this, 1208, 8, -12, 0, 2052, -1, "", 1);// 672
			AddComplexComponent( (BaseAddon) this, 1208, 9, -12, 0, 2052, -1, "", 1);// 673
			AddComplexComponent( (BaseAddon) this, 1208, 10, -12, 0, 2052, -1, "", 1);// 674
			AddComplexComponent( (BaseAddon) this, 1208, 11, -12, 0, 2052, -1, "", 1);// 675
			AddComplexComponent( (BaseAddon) this, 1208, 12, -12, 0, 2052, -1, "", 1);// 676
			AddComplexComponent( (BaseAddon) this, 1208, 13, -12, 0, 2052, -1, "", 1);// 677
			AddComplexComponent( (BaseAddon) this, 1208, 14, -12, 0, 2052, -1, "", 1);// 678
			AddComplexComponent( (BaseAddon) this, 1208, 15, -12, 0, 2052, -1, "", 1);// 679
			AddComplexComponent( (BaseAddon) this, 1208, 16, -12, 0, 2052, -1, "", 1);// 680
			AddComplexComponent( (BaseAddon) this, 128, 16, -11, 0, 2052, -1, "", 1);// 681
			AddComplexComponent( (BaseAddon) this, 128, 16, -10, 0, 2052, -1, "", 1);// 682
			AddComplexComponent( (BaseAddon) this, 128, 16, -9, 0, 2052, -1, "", 1);// 683
			AddComplexComponent( (BaseAddon) this, 128, 16, -8, 0, 2052, -1, "", 1);// 684
			AddComplexComponent( (BaseAddon) this, 128, 16, -7, 0, 2052, -1, "", 1);// 685
			AddComplexComponent( (BaseAddon) this, 128, 16, -6, 0, 2052, -1, "", 1);// 686
			AddComplexComponent( (BaseAddon) this, 128, 16, -5, 0, 2052, -1, "", 1);// 687
			AddComplexComponent( (BaseAddon) this, 128, 16, -4, 0, 2052, -1, "", 1);// 688
			AddComplexComponent( (BaseAddon) this, 128, 16, -3, 0, 2052, -1, "", 1);// 689
			AddComplexComponent( (BaseAddon) this, 128, 16, -2, 0, 2052, -1, "", 1);// 690
			AddComplexComponent( (BaseAddon) this, 128, 16, -1, 0, 2052, -1, "", 1);// 691
			AddComplexComponent( (BaseAddon) this, 128, 16, 0, 0, 2052, -1, "", 1);// 692
			AddComplexComponent( (BaseAddon) this, 128, 16, 1, 0, 2052, -1, "", 1);// 693
			AddComplexComponent( (BaseAddon) this, 128, 16, 2, 0, 2052, -1, "", 1);// 694
			AddComplexComponent( (BaseAddon) this, 128, 16, 3, 0, 2052, -1, "", 1);// 695
			AddComplexComponent( (BaseAddon) this, 128, 16, 4, 0, 2052, -1, "", 1);// 696
			AddComplexComponent( (BaseAddon) this, 1208, 8, -11, 0, 2052, -1, "", 1);// 697
			AddComplexComponent( (BaseAddon) this, 1208, 8, -10, 0, 2052, -1, "", 1);// 698
			AddComplexComponent( (BaseAddon) this, 1208, 8, -9, 0, 2052, -1, "", 1);// 699
			AddComplexComponent( (BaseAddon) this, 1208, 8, -8, 0, 2052, -1, "", 1);// 700
			AddComplexComponent( (BaseAddon) this, 1208, 8, -7, 0, 2052, -1, "", 1);// 701
			AddComplexComponent( (BaseAddon) this, 1208, 8, -6, 0, 2052, -1, "", 1);// 702
			AddComplexComponent( (BaseAddon) this, 1208, 8, -5, 0, 2052, -1, "", 1);// 703
			AddComplexComponent( (BaseAddon) this, 1208, 9, -11, 0, 2052, -1, "", 1);// 713
			AddComplexComponent( (BaseAddon) this, 1208, 9, -10, 0, 2052, -1, "", 1);// 714
			AddComplexComponent( (BaseAddon) this, 1208, 9, -9, 0, 2052, -1, "", 1);// 715
			AddComplexComponent( (BaseAddon) this, 1208, 9, -8, 0, 2052, -1, "", 1);// 716
			AddComplexComponent( (BaseAddon) this, 1208, 9, -7, 0, 2052, -1, "", 1);// 717
			AddComplexComponent( (BaseAddon) this, 1208, 9, -6, 0, 2052, -1, "", 1);// 718
			AddComplexComponent( (BaseAddon) this, 1208, 9, -5, 0, 2052, -1, "", 1);// 719
			AddComplexComponent( (BaseAddon) this, 1208, 10, -11, 0, 2052, -1, "", 1);// 729
			AddComplexComponent( (BaseAddon) this, 1208, 10, -10, 0, 2052, -1, "", 1);// 730
			AddComplexComponent( (BaseAddon) this, 1208, 10, -9, 0, 2052, -1, "", 1);// 731
			AddComplexComponent( (BaseAddon) this, 1208, 10, -8, 0, 2052, -1, "", 1);// 732
			AddComplexComponent( (BaseAddon) this, 1208, 10, -7, 0, 2052, -1, "", 1);// 733
			AddComplexComponent( (BaseAddon) this, 1208, 10, -6, 0, 2052, -1, "", 1);// 734
			AddComplexComponent( (BaseAddon) this, 1208, 10, -5, 0, 2052, -1, "", 1);// 735
			AddComplexComponent( (BaseAddon) this, 1208, 11, -11, 0, 2052, -1, "", 1);// 745
			AddComplexComponent( (BaseAddon) this, 1208, 11, -10, 0, 2052, -1, "", 1);// 746
			AddComplexComponent( (BaseAddon) this, 1208, 11, -9, 0, 2052, -1, "", 1);// 747
			AddComplexComponent( (BaseAddon) this, 1208, 11, -8, 0, 2052, -1, "", 1);// 748
			AddComplexComponent( (BaseAddon) this, 1208, 11, -7, 0, 2052, -1, "", 1);// 749
			AddComplexComponent( (BaseAddon) this, 1208, 11, -6, 0, 2052, -1, "", 1);// 750
			AddComplexComponent( (BaseAddon) this, 1208, 11, -5, 0, 2052, -1, "", 1);// 751
			AddComplexComponent( (BaseAddon) this, 1208, 12, -11, 0, 2052, -1, "", 1);// 761
			AddComplexComponent( (BaseAddon) this, 1208, 12, -10, 0, 2052, -1, "", 1);// 762
			AddComplexComponent( (BaseAddon) this, 1208, 12, -9, 0, 2052, -1, "", 1);// 763
			AddComplexComponent( (BaseAddon) this, 1208, 12, -8, 0, 2052, -1, "", 1);// 764
			AddComplexComponent( (BaseAddon) this, 1208, 12, -7, 0, 2052, -1, "", 1);// 765
			AddComplexComponent( (BaseAddon) this, 1208, 12, -6, 0, 2052, -1, "", 1);// 766
			AddComplexComponent( (BaseAddon) this, 1208, 12, -5, 0, 2052, -1, "", 1);// 767
			AddComplexComponent( (BaseAddon) this, 1208, 13, -11, 0, 2052, -1, "", 1);// 777
			AddComplexComponent( (BaseAddon) this, 1208, 13, -10, 0, 2052, -1, "", 1);// 778
			AddComplexComponent( (BaseAddon) this, 1208, 13, -9, 0, 2052, -1, "", 1);// 779
			AddComplexComponent( (BaseAddon) this, 1208, 13, -8, 0, 2052, -1, "", 1);// 780
			AddComplexComponent( (BaseAddon) this, 1208, 13, -7, 0, 2052, -1, "", 1);// 781
			AddComplexComponent( (BaseAddon) this, 1208, 13, -6, 0, 2052, -1, "", 1);// 782
			AddComplexComponent( (BaseAddon) this, 1208, 13, -5, 0, 2052, -1, "", 1);// 783
			AddComplexComponent( (BaseAddon) this, 1208, 13, -4, 0, 2052, -1, "", 1);// 784
			AddComplexComponent( (BaseAddon) this, 1208, 13, -3, 0, 2052, -1, "", 1);// 785
			AddComplexComponent( (BaseAddon) this, 1208, 13, -2, 0, 2052, -1, "", 1);// 786
			AddComplexComponent( (BaseAddon) this, 1208, 13, -1, 0, 2052, -1, "", 1);// 787
			AddComplexComponent( (BaseAddon) this, 1208, 13, 0, 0, 2052, -1, "", 1);// 788
			AddComplexComponent( (BaseAddon) this, 1208, 13, 1, 0, 2052, -1, "", 1);// 789
			AddComplexComponent( (BaseAddon) this, 1208, 13, 2, 0, 2052, -1, "", 1);// 790
			AddComplexComponent( (BaseAddon) this, 1208, 13, 3, 0, 2052, -1, "", 1);// 791
			AddComplexComponent( (BaseAddon) this, 1208, 13, 4, 0, 2052, -1, "", 1);// 792
			AddComplexComponent( (BaseAddon) this, 1208, 14, -11, 0, 2052, -1, "", 1);// 793
			AddComplexComponent( (BaseAddon) this, 1208, 14, -10, 0, 2052, -1, "", 1);// 794
			AddComplexComponent( (BaseAddon) this, 1208, 14, -9, 0, 2052, -1, "", 1);// 795
			AddComplexComponent( (BaseAddon) this, 1208, 14, -8, 0, 2052, -1, "", 1);// 796
			AddComplexComponent( (BaseAddon) this, 1208, 14, -7, 0, 2052, -1, "", 1);// 797
			AddComplexComponent( (BaseAddon) this, 1208, 14, -6, 0, 2052, -1, "", 1);// 798
			AddComplexComponent( (BaseAddon) this, 1208, 14, -5, 0, 2052, -1, "", 1);// 799
			AddComplexComponent( (BaseAddon) this, 1208, 14, -4, 0, 2052, -1, "", 1);// 800
			AddComplexComponent( (BaseAddon) this, 1208, 14, -3, 0, 2052, -1, "", 1);// 801
			AddComplexComponent( (BaseAddon) this, 1208, 14, -2, 0, 2052, -1, "", 1);// 802
			AddComplexComponent( (BaseAddon) this, 1208, 14, -1, 0, 2052, -1, "", 1);// 803
			AddComplexComponent( (BaseAddon) this, 1208, 14, 0, 0, 2052, -1, "", 1);// 804
			AddComplexComponent( (BaseAddon) this, 1208, 14, 1, 0, 2052, -1, "", 1);// 805
			AddComplexComponent( (BaseAddon) this, 1208, 14, 2, 0, 2052, -1, "", 1);// 806
			AddComplexComponent( (BaseAddon) this, 1208, 14, 3, 0, 2052, -1, "", 1);// 807
			AddComplexComponent( (BaseAddon) this, 1208, 14, 4, 0, 2052, -1, "", 1);// 808
			AddComplexComponent( (BaseAddon) this, 1208, 15, -11, 0, 2052, -1, "", 1);// 809
			AddComplexComponent( (BaseAddon) this, 1208, 15, -10, 0, 2052, -1, "", 1);// 810
			AddComplexComponent( (BaseAddon) this, 1208, 15, -9, 0, 2052, -1, "", 1);// 811
			AddComplexComponent( (BaseAddon) this, 1208, 15, -8, 0, 2052, -1, "", 1);// 812
			AddComplexComponent( (BaseAddon) this, 1208, 15, -7, 0, 2052, -1, "", 1);// 813
			AddComplexComponent( (BaseAddon) this, 1208, 15, -6, 0, 2052, -1, "", 1);// 814
			AddComplexComponent( (BaseAddon) this, 1208, 15, -5, 0, 2052, -1, "", 1);// 815
			AddComplexComponent( (BaseAddon) this, 1208, 15, -4, 0, 2052, -1, "", 1);// 816
			AddComplexComponent( (BaseAddon) this, 1208, 15, -3, 0, 2052, -1, "", 1);// 817
			AddComplexComponent( (BaseAddon) this, 1208, 15, -2, 0, 2052, -1, "", 1);// 818
			AddComplexComponent( (BaseAddon) this, 1208, 15, -1, 0, 2052, -1, "", 1);// 819
			AddComplexComponent( (BaseAddon) this, 1208, 15, 0, 0, 2052, -1, "", 1);// 820
			AddComplexComponent( (BaseAddon) this, 1208, 15, 1, 0, 2052, -1, "", 1);// 821
			AddComplexComponent( (BaseAddon) this, 1208, 15, 2, 0, 2052, -1, "", 1);// 822
			AddComplexComponent( (BaseAddon) this, 1208, 15, 3, 0, 2052, -1, "", 1);// 823
			AddComplexComponent( (BaseAddon) this, 1208, 15, 4, 0, 2052, -1, "", 1);// 824
			AddComplexComponent( (BaseAddon) this, 1208, 16, -11, 0, 2052, -1, "", 1);// 825
			AddComplexComponent( (BaseAddon) this, 1208, 16, -10, 0, 2052, -1, "", 1);// 826
			AddComplexComponent( (BaseAddon) this, 1208, 16, -9, 0, 2052, -1, "", 1);// 827
			AddComplexComponent( (BaseAddon) this, 1208, 16, -8, 0, 2052, -1, "", 1);// 828
			AddComplexComponent( (BaseAddon) this, 1208, 16, -7, 0, 2052, -1, "", 1);// 829
			AddComplexComponent( (BaseAddon) this, 1208, 16, -6, 0, 2052, -1, "", 1);// 830
			AddComplexComponent( (BaseAddon) this, 1208, 16, -5, 0, 2052, -1, "", 1);// 831
			AddComplexComponent( (BaseAddon) this, 1208, 16, -4, 0, 2052, -1, "", 1);// 832
			AddComplexComponent( (BaseAddon) this, 1208, 16, -3, 0, 2052, -1, "", 1);// 833
			AddComplexComponent( (BaseAddon) this, 1208, 16, -2, 0, 2052, -1, "", 1);// 834
			AddComplexComponent( (BaseAddon) this, 1208, 16, -1, 0, 2052, -1, "", 1);// 835
			AddComplexComponent( (BaseAddon) this, 1208, 16, 0, 0, 2052, -1, "", 1);// 836
			AddComplexComponent( (BaseAddon) this, 1208, 16, 1, 0, 2052, -1, "", 1);// 837
			AddComplexComponent( (BaseAddon) this, 1208, 16, 2, 0, 2052, -1, "", 1);// 838
			AddComplexComponent( (BaseAddon) this, 1208, 16, 3, 0, 2052, -1, "", 1);// 839
			AddComplexComponent( (BaseAddon) this, 1208, 16, 4, 0, 2052, -1, "", 1);// 840
			AddComplexComponent( (BaseAddon) this, 128, 8, -4, 0, 2052, -1, "", 1);// 841
			AddComplexComponent( (BaseAddon) this, 128, 9, -4, 0, 2052, -1, "", 1);// 842
			AddComplexComponent( (BaseAddon) this, 128, 10, -4, 0, 2052, -1, "", 1);// 843
			AddComplexComponent( (BaseAddon) this, 128, 11, -4, 0, 2052, -1, "", 1);// 844
			AddComplexComponent( (BaseAddon) this, 128, 12, -4, 0, 2052, -1, "", 1);// 845
			AddComplexComponent( (BaseAddon) this, 128, 12, -3, 0, 2052, -1, "", 1);// 846
			AddComplexComponent( (BaseAddon) this, 128, 12, -2, 0, 2052, -1, "", 1);// 847
			AddComplexComponent( (BaseAddon) this, 128, 12, -1, 0, 2052, -1, "", 1);// 848
			AddComplexComponent( (BaseAddon) this, 128, 12, 0, 0, 2052, -1, "", 1);// 849
			AddComplexComponent( (BaseAddon) this, 128, 12, 1, 0, 2052, -1, "", 1);// 850
			AddComplexComponent( (BaseAddon) this, 128, 12, 2, 0, 2052, -1, "", 1);// 851
			AddComplexComponent( (BaseAddon) this, 128, 12, 3, 0, 2052, -1, "", 1);// 852
			AddComplexComponent( (BaseAddon) this, 128, 12, 4, 0, 2052, -1, "", 1);// 853
			AddComplexComponent( (BaseAddon) this, 128, 16, 5, 0, 2052, -1, "", 1);// 854
			AddComplexComponent( (BaseAddon) this, 128, 16, 6, 0, 2052, -1, "", 1);// 855
			AddComplexComponent( (BaseAddon) this, 128, 16, 7, 0, 2052, -1, "", 1);// 856
			AddComplexComponent( (BaseAddon) this, 128, 16, 8, 0, 2052, -1, "", 1);// 857
			AddComplexComponent( (BaseAddon) this, 128, 16, 9, 0, 2052, -1, "", 1);// 858
			AddComplexComponent( (BaseAddon) this, 128, 16, 10, 0, 2052, -1, "", 1);// 859
			AddComplexComponent( (BaseAddon) this, 128, 16, 11, 0, 2052, -1, "", 1);// 860
			AddComplexComponent( (BaseAddon) this, 128, 16, 12, 0, 2052, -1, "", 1);// 861
			AddComplexComponent( (BaseAddon) this, 128, 16, 13, 0, 2052, -1, "", 1);// 862
			AddComplexComponent( (BaseAddon) this, 1208, 8, 6, 0, 2052, -1, "", 1);// 864
			AddComplexComponent( (BaseAddon) this, 1208, 8, 7, 0, 2052, -1, "", 1);// 865
			AddComplexComponent( (BaseAddon) this, 1208, 8, 8, 0, 2052, -1, "", 1);// 866
			AddComplexComponent( (BaseAddon) this, 1208, 8, 9, 0, 2052, -1, "", 1);// 867
			AddComplexComponent( (BaseAddon) this, 1208, 8, 10, 0, 2052, -1, "", 1);// 868
			AddComplexComponent( (BaseAddon) this, 1208, 8, 11, 0, 2052, -1, "", 1);// 869
			AddComplexComponent( (BaseAddon) this, 1208, 8, 12, 0, 2052, -1, "", 1);// 870
			AddComplexComponent( (BaseAddon) this, 1208, 8, 13, 0, 2052, -1, "", 1);// 871
			AddComplexComponent( (BaseAddon) this, 1208, 9, 6, 0, 2052, -1, "", 1);// 873
			AddComplexComponent( (BaseAddon) this, 1208, 9, 7, 0, 2052, -1, "", 1);// 874
			AddComplexComponent( (BaseAddon) this, 1208, 9, 8, 0, 2052, -1, "", 1);// 875
			AddComplexComponent( (BaseAddon) this, 1208, 9, 9, 0, 2052, -1, "", 1);// 876
			AddComplexComponent( (BaseAddon) this, 1208, 9, 10, 0, 2052, -1, "", 1);// 877
			AddComplexComponent( (BaseAddon) this, 1208, 9, 11, 0, 2052, -1, "", 1);// 878
			AddComplexComponent( (BaseAddon) this, 1208, 9, 12, 0, 2052, -1, "", 1);// 879
			AddComplexComponent( (BaseAddon) this, 1208, 9, 13, 0, 2052, -1, "", 1);// 880
			AddComplexComponent( (BaseAddon) this, 1208, 10, 6, 0, 2052, -1, "", 1);// 882
			AddComplexComponent( (BaseAddon) this, 1208, 10, 7, 0, 2052, -1, "", 1);// 883
			AddComplexComponent( (BaseAddon) this, 1208, 10, 8, 0, 2052, -1, "", 1);// 884
			AddComplexComponent( (BaseAddon) this, 1208, 10, 9, 0, 2052, -1, "", 1);// 885
			AddComplexComponent( (BaseAddon) this, 1208, 10, 10, 0, 2052, -1, "", 1);// 886
			AddComplexComponent( (BaseAddon) this, 1208, 10, 11, 0, 2052, -1, "", 1);// 887
			AddComplexComponent( (BaseAddon) this, 1208, 10, 12, 0, 2052, -1, "", 1);// 888
			AddComplexComponent( (BaseAddon) this, 1208, 10, 13, 0, 2052, -1, "", 1);// 889
			AddComplexComponent( (BaseAddon) this, 1208, 11, 6, 0, 2052, -1, "", 1);// 891
			AddComplexComponent( (BaseAddon) this, 1208, 11, 7, 0, 2052, -1, "", 1);// 892
			AddComplexComponent( (BaseAddon) this, 1208, 11, 8, 0, 2052, -1, "", 1);// 893
			AddComplexComponent( (BaseAddon) this, 1208, 11, 9, 0, 2052, -1, "", 1);// 894
			AddComplexComponent( (BaseAddon) this, 1208, 11, 10, 0, 2052, -1, "", 1);// 895
			AddComplexComponent( (BaseAddon) this, 1208, 11, 11, 0, 2052, -1, "", 1);// 896
			AddComplexComponent( (BaseAddon) this, 1208, 11, 12, 0, 2052, -1, "", 1);// 897
			AddComplexComponent( (BaseAddon) this, 1208, 11, 13, 0, 2052, -1, "", 1);// 898
			AddComplexComponent( (BaseAddon) this, 1208, 12, 6, 0, 2052, -1, "", 1);// 900
			AddComplexComponent( (BaseAddon) this, 1208, 12, 7, 0, 2052, -1, "", 1);// 901
			AddComplexComponent( (BaseAddon) this, 1208, 12, 8, 0, 2052, -1, "", 1);// 902
			AddComplexComponent( (BaseAddon) this, 1208, 12, 9, 0, 2052, -1, "", 1);// 903
			AddComplexComponent( (BaseAddon) this, 1208, 12, 10, 0, 2052, -1, "", 1);// 904
			AddComplexComponent( (BaseAddon) this, 1208, 12, 11, 0, 2052, -1, "", 1);// 905
			AddComplexComponent( (BaseAddon) this, 1208, 12, 12, 0, 2052, -1, "", 1);// 906
			AddComplexComponent( (BaseAddon) this, 1208, 12, 13, 0, 2052, -1, "", 1);// 907
			AddComplexComponent( (BaseAddon) this, 1208, 13, 5, 0, 2052, -1, "", 1);// 908
			AddComplexComponent( (BaseAddon) this, 1208, 13, 6, 0, 2052, -1, "", 1);// 909
			AddComplexComponent( (BaseAddon) this, 1208, 13, 7, 0, 2052, -1, "", 1);// 910
			AddComplexComponent( (BaseAddon) this, 1208, 13, 8, 0, 2052, -1, "", 1);// 911
			AddComplexComponent( (BaseAddon) this, 1208, 13, 9, 0, 2052, -1, "", 1);// 912
			AddComplexComponent( (BaseAddon) this, 1208, 13, 10, 0, 2052, -1, "", 1);// 913
			AddComplexComponent( (BaseAddon) this, 1208, 13, 11, 0, 2052, -1, "", 1);// 914
			AddComplexComponent( (BaseAddon) this, 1208, 13, 12, 0, 2052, -1, "", 1);// 915
			AddComplexComponent( (BaseAddon) this, 1208, 13, 13, 0, 2052, -1, "", 1);// 916
			AddComplexComponent( (BaseAddon) this, 1208, 14, 5, 0, 2052, -1, "", 1);// 917
			AddComplexComponent( (BaseAddon) this, 1208, 14, 6, 0, 2052, -1, "", 1);// 918
			AddComplexComponent( (BaseAddon) this, 1208, 14, 7, 0, 2052, -1, "", 1);// 919
			AddComplexComponent( (BaseAddon) this, 1208, 14, 8, 0, 2052, -1, "", 1);// 920
			AddComplexComponent( (BaseAddon) this, 1208, 14, 9, 0, 2052, -1, "", 1);// 921
			AddComplexComponent( (BaseAddon) this, 1208, 14, 10, 0, 2052, -1, "", 1);// 922
			AddComplexComponent( (BaseAddon) this, 1208, 14, 11, 0, 2052, -1, "", 1);// 923
			AddComplexComponent( (BaseAddon) this, 1208, 14, 12, 0, 2052, -1, "", 1);// 924
			AddComplexComponent( (BaseAddon) this, 1208, 14, 13, 0, 2052, -1, "", 1);// 925
			AddComplexComponent( (BaseAddon) this, 1208, 15, 5, 0, 2052, -1, "", 1);// 926
			AddComplexComponent( (BaseAddon) this, 1208, 15, 6, 0, 2052, -1, "", 1);// 927
			AddComplexComponent( (BaseAddon) this, 1208, 15, 7, 0, 2052, -1, "", 1);// 928
			AddComplexComponent( (BaseAddon) this, 1208, 15, 8, 0, 2052, -1, "", 1);// 929
			AddComplexComponent( (BaseAddon) this, 1208, 15, 9, 0, 2052, -1, "", 1);// 930
			AddComplexComponent( (BaseAddon) this, 1208, 15, 10, 0, 2052, -1, "", 1);// 931
			AddComplexComponent( (BaseAddon) this, 1208, 15, 11, 0, 2052, -1, "", 1);// 932
			AddComplexComponent( (BaseAddon) this, 1208, 15, 12, 0, 2052, -1, "", 1);// 933
			AddComplexComponent( (BaseAddon) this, 1208, 15, 13, 0, 2052, -1, "", 1);// 934
			AddComplexComponent( (BaseAddon) this, 1208, 16, 5, 0, 2052, -1, "", 1);// 935
			AddComplexComponent( (BaseAddon) this, 1208, 16, 6, 0, 2052, -1, "", 1);// 936
			AddComplexComponent( (BaseAddon) this, 1208, 16, 7, 0, 2052, -1, "", 1);// 937
			AddComplexComponent( (BaseAddon) this, 1208, 16, 8, 0, 2052, -1, "", 1);// 938
			AddComplexComponent( (BaseAddon) this, 1208, 16, 9, 0, 2052, -1, "", 1);// 939
			AddComplexComponent( (BaseAddon) this, 1208, 16, 10, 0, 2052, -1, "", 1);// 940
			AddComplexComponent( (BaseAddon) this, 1208, 16, 11, 0, 2052, -1, "", 1);// 941
			AddComplexComponent( (BaseAddon) this, 1208, 16, 12, 0, 2052, -1, "", 1);// 942
			AddComplexComponent( (BaseAddon) this, 1208, 16, 13, 0, 2052, -1, "", 1);// 943
			AddComplexComponent( (BaseAddon) this, 128, 8, 5, 0, 2052, -1, "", 1);// 944
			AddComplexComponent( (BaseAddon) this, 128, 9, 5, 0, 2052, -1, "", 1);// 945
			AddComplexComponent( (BaseAddon) this, 128, 10, 5, 0, 2052, -1, "", 1);// 946
			AddComplexComponent( (BaseAddon) this, 128, 11, 5, 0, 2052, -1, "", 1);// 947
			AddComplexComponent( (BaseAddon) this, 128, 12, 5, 0, 2052, -1, "", 1);// 948

		}

		public Arena1Addon( Serial serial ) : base( serial )
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

	public class Arena1AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new Arena1Addon();
			}
		}

		[Constructable]
		public Arena1AddonDeed()
		{
			Name = "Arena1";
		}

		public Arena1AddonDeed( Serial serial ) : base( serial )
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