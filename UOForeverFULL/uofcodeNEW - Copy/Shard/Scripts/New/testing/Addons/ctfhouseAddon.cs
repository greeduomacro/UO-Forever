
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ctfhouseAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {3213, -3, -9, 0}, {3213, -4, -9, 0}, {3213, -2, -9, 0}// 3	4	6	
			, {223, -3, -6, 30}, {3213, -5, -9, 0}, {5655, -2, -1, 32}// 11	12	24	
			, {5673, -2, -3, 32}, {5658, -2, 1, 32}, {4554, -2, 2, 30}// 25	33	35	
			, {5657, -2, 2, 32}, {4824, -4, 9, 0}, {5659, -2, 0, 32}// 36	41	43	
			, {4554, -2, -3, 30}, {5671, -2, 4, 32}, {5660, -2, 0, 32}// 80	81	82	
			, {5656, -2, -2, 32}, {5673, -2, 3, 32}, {3213, 1, -9, 0}// 83	96	103	
			, {3213, 0, -9, 0}, {3213, 3, -9, 0}, {3213, -1, -9, 0}// 104	108	116	
			, {3213, 2, -9, 0}, {3378, 6, -7, 0}, {13842, 6, -7, 2}// 117	118	120	
			, {3213, 4, -9, 0}, {3213, 5, -9, 0}, {3213, 6, -9, 0}// 129	130	131	
			, {2760, 0, -1, 31}, {2760, 0, 0, 31}, {2768, 0, 4, 31}// 164	165	176	
			, {2766, 1, -4, 31}, {3310, 6, 8, 0}, {5384, -1, 4, 30}// 177	184	204	
			, {2767, 2, -3, 31}, {2767, 2, -2, 31}, {2768, 1, 4, 31}// 209	210	211	
			, {3378, 5, 8, 0}, {2760, 1, -2, 31}, {2760, 1, -3, 31}// 218	221	222	
			, {2764, 2, -4, 31}, {2760, 1, 1, 31}, {2760, 1, 2, 31}// 223	224	225	
			, {2760, 1, 3, 31}, {2760, 1, -1, 31}, {2760, 1, 0, 31}// 226	227	228	
			, {2765, -1, -3, 31}, {2767, 2, 3, 31}, {2760, 0, 1, 31}// 229	230	240	
			, {2760, 0, 2, 31}, {2760, 0, 3, 31}, {2765, -1, 3, 31}// 241	242	243	
			, {2763, -1, 4, 31}, {2766, 0, -4, 31}, {2760, 0, -3, 31}// 244	245	246	
			, {2760, 0, -2, 31}, {2762, -1, -4, 31}, {2765, -1, -2, 31}// 247	248	249	
			, {2765, -1, 0, 31}, {11618, 2, 1, 45}, {13842, 6, 7, 0}// 250	261	267	
			, {2767, 2, -1, 31}, {2767, 2, 0, 31}, {2767, 2, 1, 31}// 287	288	289	
			, {2767, 2, 2, 31}, {2765, -1, 1, 31}, {2765, -1, 2, 31}// 290	311	312	
			, {2761, 2, 4, 31}, {5384, -1, -5, 30}, {2765, -1, -1, 30}// 319	321	338	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new ctfhouseAddonDeed();
			}
		}

		[ Constructable ]
		public ctfhouseAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 1875, -2, -6, 5, 1175, -1, "", 1);// 1
			AddComplexComponent( (BaseAddon) this, 1875, -2, -7, 0, 1175, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 1875, -3, -7, 0, 1175, -1, "", 1);// 5
			AddComplexComponent( (BaseAddon) this, 222, -2, -6, 30, 1175, -1, "", 1);// 7
			AddComplexComponent( (BaseAddon) this, 1877, -3, -6, 5, 1175, -1, "", 1);// 8
			AddComplexComponent( (BaseAddon) this, 1876, -4, -6, 0, 1175, -1, "", 1);// 9
			AddComplexComponent( (BaseAddon) this, 1877, -4, -7, 0, 1175, -1, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 1313, -2, 2, 10, 1175, -1, "", 1);// 13
			AddComplexComponent( (BaseAddon) this, 1313, -2, 3, 10, 1175, -1, "", 1);// 14
			AddComplexComponent( (BaseAddon) this, 1313, -2, 4, 10, 1175, -1, "", 1);// 15
			AddComplexComponent( (BaseAddon) this, 1313, -2, 5, 10, 1175, -1, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 1876, -4, 0, 0, 1175, -1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 1876, -4, 1, 0, 1175, -1, "", 1);// 18
			AddComplexComponent( (BaseAddon) this, 1876, -4, 2, 0, 1175, -1, "", 1);// 19
			AddComplexComponent( (BaseAddon) this, 1876, -4, 3, 0, 1175, -1, "", 1);// 20
			AddComplexComponent( (BaseAddon) this, 1876, -4, 4, 0, 1175, -1, "", 1);// 21
			AddComplexComponent( (BaseAddon) this, 1873, -2, 6, 5, 1175, -1, "", 1);// 22
			AddComplexComponent( (BaseAddon) this, 1876, -4, 6, 0, 1175, -1, "", 1);// 23
			AddComplexComponent( (BaseAddon) this, 969, -3, -1, 33, 1175, -1, "", 1);// 26
			AddComplexComponent( (BaseAddon) this, 969, -3, -5, 33, 1175, -1, "", 1);// 27
			AddComplexComponent( (BaseAddon) this, 969, -3, -4, 33, 1175, -1, "", 1);// 28
			AddComplexComponent( (BaseAddon) this, 969, -3, 4, 33, 1175, -1, "", 1);// 29
			AddComplexComponent( (BaseAddon) this, 969, -3, 2, 33, 1175, -1, "", 1);// 30
			AddComplexComponent( (BaseAddon) this, 969, -3, 3, 33, 1175, -1, "", 1);// 31
			AddComplexComponent( (BaseAddon) this, 5671, -2, -4, 32, 1175, -1, "", 1);// 32
			AddComplexComponent( (BaseAddon) this, 1876, -4, 5, 0, 1175, -1, "", 1);// 34
			AddComplexComponent( (BaseAddon) this, 222, -2, 5, 30, 1175, -1, "", 1);// 37
			AddComplexComponent( (BaseAddon) this, 1876, -4, -5, 0, 1175, -1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 1876, -4, -4, 0, 1175, -1, "", 1);// 39
			AddComplexComponent( (BaseAddon) this, 1876, -4, -3, 0, 1175, -1, "", 1);// 40
			AddComplexComponent( (BaseAddon) this, 969, -3, 1, 33, 1175, -1, "", 1);// 42
			AddComplexComponent( (BaseAddon) this, 1876, -3, 3, 5, 1175, -1, "", 1);// 44
			AddComplexComponent( (BaseAddon) this, 1876, -3, 4, 5, 1175, -1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 1876, -3, 5, 5, 1175, -1, "", 1);// 46
			AddComplexComponent( (BaseAddon) this, 1873, -3, 7, 0, 1175, -1, "", 1);// 47
			AddComplexComponent( (BaseAddon) this, 1876, -4, -2, 0, 1175, -1, "", 1);// 48
			AddComplexComponent( (BaseAddon) this, 1876, -4, -1, 0, 1175, -1, "", 1);// 49
			AddComplexComponent( (BaseAddon) this, 1880, -4, 7, 0, 1175, -1, "", 1);// 50
			AddComplexComponent( (BaseAddon) this, 1873, -2, 7, 0, 1175, -1, "", 1);// 51
			AddComplexComponent( (BaseAddon) this, 221, -3, -3, 30, 1175, -1, "", 1);// 52
			AddComplexComponent( (BaseAddon) this, 221, -3, -2, 30, 1175, -1, "", 1);// 53
			AddComplexComponent( (BaseAddon) this, 221, -3, -1, 30, 1175, -1, "", 1);// 54
			AddComplexComponent( (BaseAddon) this, 221, -3, 0, 30, 1175, -1, "", 1);// 55
			AddComplexComponent( (BaseAddon) this, 221, -3, 1, 30, 1175, -1, "", 1);// 56
			AddComplexComponent( (BaseAddon) this, 221, -3, 2, 30, 1175, -1, "", 1);// 57
			AddComplexComponent( (BaseAddon) this, 221, -3, 3, 30, 1175, -1, "", 1);// 58
			AddComplexComponent( (BaseAddon) this, 221, -3, 4, 30, 1175, -1, "", 1);// 59
			AddComplexComponent( (BaseAddon) this, 221, -3, 5, 30, 1175, -1, "", 1);// 60
			AddComplexComponent( (BaseAddon) this, 1313, -2, -5, 10, 1175, -1, "", 1);// 61
			AddComplexComponent( (BaseAddon) this, 1313, -2, -4, 10, 1175, -1, "", 1);// 62
			AddComplexComponent( (BaseAddon) this, 1313, -2, -2, 10, 1175, -1, "", 1);// 63
			AddComplexComponent( (BaseAddon) this, 1313, -2, -1, 10, 1175, -1, "", 1);// 64
			AddComplexComponent( (BaseAddon) this, 1313, -2, 0, 10, 1175, -1, "", 1);// 65
			AddComplexComponent( (BaseAddon) this, 1313, -2, 1, 10, 1175, -1, "", 1);// 66
			AddComplexComponent( (BaseAddon) this, 1313, -2, -5, 30, 2021, -1, "", 1);// 67
			AddComplexComponent( (BaseAddon) this, 1313, -2, -4, 30, 2021, -1, "", 1);// 68
			AddComplexComponent( (BaseAddon) this, 1313, -2, -3, 30, 2021, -1, "", 1);// 69
			AddComplexComponent( (BaseAddon) this, 1313, -2, -2, 30, 2021, -1, "", 1);// 70
			AddComplexComponent( (BaseAddon) this, 1313, -2, -1, 30, 2021, -1, "", 1);// 71
			AddComplexComponent( (BaseAddon) this, 1313, -2, 0, 30, 2021, -1, "", 1);// 72
			AddComplexComponent( (BaseAddon) this, 1313, -2, 1, 30, 2021, -1, "", 1);// 73
			AddComplexComponent( (BaseAddon) this, 1313, -2, 2, 30, 2021, -1, "", 1);// 74
			AddComplexComponent( (BaseAddon) this, 1313, -2, 3, 30, 2021, -1, "", 1);// 75
			AddComplexComponent( (BaseAddon) this, 1313, -2, 4, 30, 2021, -1, "", 1);// 76
			AddComplexComponent( (BaseAddon) this, 1313, -2, 5, 30, 2021, -1, "", 1);// 77
			AddComplexComponent( (BaseAddon) this, 969, -3, 5, 33, 1175, -1, "", 1);// 78
			AddComplexComponent( (BaseAddon) this, 969, -3, 0, 33, 1175, -1, "", 1);// 79
			AddComplexComponent( (BaseAddon) this, 1313, -2, -3, 10, 1175, -1, "", 1);// 84
			AddComplexComponent( (BaseAddon) this, 1880, -3, 6, 5, 1175, -1, "", 1);// 85
			AddComplexComponent( (BaseAddon) this, 1876, -3, -5, 5, 1175, -1, "", 1);// 86
			AddComplexComponent( (BaseAddon) this, 1876, -3, -4, 5, 1175, -1, "", 1);// 87
			AddComplexComponent( (BaseAddon) this, 1876, -3, -3, 5, 1175, -1, "", 1);// 88
			AddComplexComponent( (BaseAddon) this, 1876, -3, -2, 5, 1175, -1, "", 1);// 89
			AddComplexComponent( (BaseAddon) this, 1876, -3, -1, 5, 1175, -1, "", 1);// 90
			AddComplexComponent( (BaseAddon) this, 1876, -3, 0, 5, 1175, -1, "", 1);// 91
			AddComplexComponent( (BaseAddon) this, 1876, -3, 1, 5, 1175, -1, "", 1);// 92
			AddComplexComponent( (BaseAddon) this, 1876, -3, 2, 5, 1175, -1, "", 1);// 93
			AddComplexComponent( (BaseAddon) this, 221, -3, -4, 30, 1175, -1, "", 1);// 94
			AddComplexComponent( (BaseAddon) this, 221, -3, -5, 30, 1175, -1, "", 1);// 95
			AddComplexComponent( (BaseAddon) this, 969, -3, -2, 33, 1175, -1, "", 1);// 97
			AddComplexComponent( (BaseAddon) this, 969, -3, -3, 33, 1175, -1, "", 1);// 98
			AddComplexComponent( (BaseAddon) this, 222, 0, -6, 30, 1175, -1, "", 1);// 99
			AddComplexComponent( (BaseAddon) this, 222, 1, -6, 30, 1175, -1, "", 1);// 100
			AddComplexComponent( (BaseAddon) this, 222, 2, -6, 30, 1175, -1, "", 1);// 101
			AddComplexComponent( (BaseAddon) this, 1875, -1, -7, 0, 1175, -1, "", 1);// 102
			AddComplexComponent( (BaseAddon) this, 1875, 4, -7, 0, 1175, -1, "", 1);// 105
			AddComplexComponent( (BaseAddon) this, 1875, 0, -7, 0, 1175, -1, "", 1);// 106
			AddComplexComponent( (BaseAddon) this, 222, 3, -6, 30, 1175, -1, "", 1);// 107
			AddComplexComponent( (BaseAddon) this, 6587, 6, -7, 20, 0, 1, "", 1);// 109
			AddComplexComponent( (BaseAddon) this, 6571, 6, -7, 22, 0, 0, "", 1);// 110
			AddComplexComponent( (BaseAddon) this, 222, -1, -6, 30, 1175, -1, "", 1);// 111
			AddComplexComponent( (BaseAddon) this, 1874, 5, -6, 0, 1175, -1, "", 1);// 112
			AddComplexComponent( (BaseAddon) this, 1875, -1, -6, 5, 1175, -1, "", 1);// 113
			AddComplexComponent( (BaseAddon) this, 1879, 5, -7, 0, 1175, -1, "", 1);// 114
			AddComplexComponent( (BaseAddon) this, 1875, 0, -6, 5, 1175, -1, "", 1);// 115
			AddComplexComponent( (BaseAddon) this, 1879, 4, -6, 5, 1175, -1, "", 1);// 119
			AddComplexComponent( (BaseAddon) this, 1875, 1, -6, 0, 1175, -1, "", 1);// 121
			AddComplexComponent( (BaseAddon) this, 1875, 2, -7, 0, 1175, -1, "", 1);// 122
			AddComplexComponent( (BaseAddon) this, 1875, 1, -7, 0, 1175, -1, "", 1);// 123
			AddComplexComponent( (BaseAddon) this, 1875, 3, -7, 0, 1175, -1, "", 1);// 124
			AddComplexComponent( (BaseAddon) this, 1875, 3, -6, 5, 1175, -1, "", 1);// 125
			AddComplexComponent( (BaseAddon) this, 1313, 0, -6, 3, 1175, -1, "", 1);// 126
			AddComplexComponent( (BaseAddon) this, 1875, 1, -6, 5, 1175, -1, "", 1);// 127
			AddComplexComponent( (BaseAddon) this, 1875, 2, -6, 5, 1175, -1, "", 1);// 128
			AddComplexComponent( (BaseAddon) this, 1313, 1, -5, 30, 2021, -1, "", 1);// 132
			AddComplexComponent( (BaseAddon) this, 1313, 1, -4, 30, 2021, -1, "", 1);// 133
			AddComplexComponent( (BaseAddon) this, 1313, 1, -3, 30, 1175, -1, "", 1);// 134
			AddComplexComponent( (BaseAddon) this, 1313, 2, 5, 30, 2021, -1, "", 1);// 135
			AddComplexComponent( (BaseAddon) this, 221, 3, -3, 30, 1175, -1, "", 1);// 136
			AddComplexComponent( (BaseAddon) this, 1313, 3, -5, 30, 2021, -1, "", 1);// 137
			AddComplexComponent( (BaseAddon) this, 1313, 3, -4, 30, 2021, -1, "", 1);// 138
			AddComplexComponent( (BaseAddon) this, 1313, 3, -3, 30, 2021, -1, "", 1);// 139
			AddComplexComponent( (BaseAddon) this, 1313, 3, -2, 30, 2021, -1, "", 1);// 140
			AddComplexComponent( (BaseAddon) this, 1873, 3, 7, 0, 1175, -1, "", 1);// 141
			AddComplexComponent( (BaseAddon) this, 1313, -1, -4, 10, 1175, -1, "", 1);// 142
			AddComplexComponent( (BaseAddon) this, 1874, 4, 4, 5, 1175, -1, "", 1);// 143
			AddComplexComponent( (BaseAddon) this, 1313, -1, -2, 30, 2021, -1, "", 1);// 144
			AddComplexComponent( (BaseAddon) this, 1313, -1, -1, 30, 2021, -1, "", 1);// 145
			AddComplexComponent( (BaseAddon) this, 1313, -1, 0, 30, 2021, -1, "", 1);// 146
			AddComplexComponent( (BaseAddon) this, 1313, -1, 1, 30, 2021, -1, "", 1);// 147
			AddComplexComponent( (BaseAddon) this, 1313, -1, 2, 30, 2021, -1, "", 1);// 148
			AddComplexComponent( (BaseAddon) this, 1313, -1, 3, 30, 2021, -1, "", 1);// 149
			AddComplexComponent( (BaseAddon) this, 1313, 2, 2, 10, 1175, -1, "", 1);// 150
			AddComplexComponent( (BaseAddon) this, 1313, 2, 3, 10, 1175, -1, "", 1);// 151
			AddComplexComponent( (BaseAddon) this, 1313, 2, 4, 10, 1175, -1, "", 1);// 152
			AddComplexComponent( (BaseAddon) this, 1313, 2, 5, 10, 1175, -1, "", 1);// 153
			AddComplexComponent( (BaseAddon) this, 1313, 3, -5, 10, 1175, -1, "", 1);// 154
			AddComplexComponent( (BaseAddon) this, 1313, 3, -4, 10, 1175, -1, "", 1);// 155
			AddComplexComponent( (BaseAddon) this, 1313, 3, -3, 10, 1175, -1, "", 1);// 156
			AddComplexComponent( (BaseAddon) this, 1313, 3, -2, 10, 1175, -1, "", 1);// 157
			AddComplexComponent( (BaseAddon) this, 1313, 3, -1, 10, 1175, -1, "", 1);// 158
			AddComplexComponent( (BaseAddon) this, 1313, 3, 0, 10, 1175, -1, "", 1);// 159
			AddComplexComponent( (BaseAddon) this, 1313, 3, 1, 10, 1175, -1, "", 1);// 160
			AddComplexComponent( (BaseAddon) this, 1313, 3, 2, 10, 1175, -1, "", 1);// 161
			AddComplexComponent( (BaseAddon) this, 1313, 3, 3, 10, 1175, -1, "", 1);// 162
			AddComplexComponent( (BaseAddon) this, 1313, 3, 4, 10, 1175, -1, "", 1);// 163
			AddComplexComponent( (BaseAddon) this, 1313, 2, -5, 30, 2021, -1, "", 1);// 166
			AddComplexComponent( (BaseAddon) this, 1313, 2, -4, 30, 2021, -1, "", 1);// 167
			AddComplexComponent( (BaseAddon) this, 1313, 2, -3, 30, 2021, -1, "", 1);// 168
			AddComplexComponent( (BaseAddon) this, 1313, 2, -2, 30, 2021, -1, "", 1);// 169
			AddComplexComponent( (BaseAddon) this, 1313, 2, -1, 30, 2021, -1, "", 1);// 170
			AddComplexComponent( (BaseAddon) this, 1313, 2, 0, 30, 2021, -1, "", 1);// 171
			AddComplexComponent( (BaseAddon) this, 1313, 2, 1, 30, 2021, -1, "", 1);// 172
			AddComplexComponent( (BaseAddon) this, 1313, 2, 2, 30, 2021, -1, "", 1);// 173
			AddComplexComponent( (BaseAddon) this, 1313, 2, 3, 30, 2021, -1, "", 1);// 174
			AddComplexComponent( (BaseAddon) this, 1313, 2, 4, 30, 2021, -1, "", 1);// 175
			AddComplexComponent( (BaseAddon) this, 4633, 0, 0, 30, 2224, -1, "King James", 1);// 178
			AddComplexComponent( (BaseAddon) this, 1313, 0, 5, 30, 2021, -1, "", 1);// 179
			AddComplexComponent( (BaseAddon) this, 221, 3, -5, 30, 1175, -1, "", 1);// 180
			AddComplexComponent( (BaseAddon) this, 1313, 0, 0, 30, 1175, -1, "", 1);// 181
			AddComplexComponent( (BaseAddon) this, 1874, 5, -1, 0, 1175, -1, "", 1);// 182
			AddComplexComponent( (BaseAddon) this, 6587, 6, 7, 20, 0, 1, "", 1);// 183
			AddComplexComponent( (BaseAddon) this, 1874, 5, 5, 0, 1175, -1, "", 1);// 185
			AddComplexComponent( (BaseAddon) this, 1874, 5, 4, 0, 1175, -1, "", 1);// 186
			AddComplexComponent( (BaseAddon) this, 1874, 5, 1, 0, 1175, -1, "", 1);// 187
			AddComplexComponent( (BaseAddon) this, 1874, 5, 0, 0, 1175, -1, "", 1);// 188
			AddComplexComponent( (BaseAddon) this, 222, 0, 5, 30, 1175, -1, "", 1);// 189
			AddComplexComponent( (BaseAddon) this, 221, 3, -2, 30, 1175, -1, "", 1);// 190
			AddComplexComponent( (BaseAddon) this, 1874, 5, -4, 0, 1175, -1, "", 1);// 191
			AddComplexComponent( (BaseAddon) this, 1874, 4, 3, 5, 1175, -1, "", 1);// 192
			AddComplexComponent( (BaseAddon) this, 1313, 0, -1, 10, 1175, -1, "", 1);// 193
			AddComplexComponent( (BaseAddon) this, 1313, 0, 0, 10, 1175, -1, "", 1);// 194
			AddComplexComponent( (BaseAddon) this, 1313, 0, 1, 10, 1175, -1, "", 1);// 195
			AddComplexComponent( (BaseAddon) this, 1313, 0, 4, 30, 2021, -1, "", 1);// 196
			AddComplexComponent( (BaseAddon) this, 1313, 1, 0, 30, 1175, -1, "", 1);// 197
			AddComplexComponent( (BaseAddon) this, 1313, 1, 1, 30, 1175, -1, "", 1);// 198
			AddComplexComponent( (BaseAddon) this, 1313, 1, 2, 30, 1175, -1, "", 1);// 199
			AddComplexComponent( (BaseAddon) this, 1313, 1, 3, 30, 1175, -1, "", 1);// 200
			AddComplexComponent( (BaseAddon) this, 1313, 1, 4, 30, 2021, -1, "", 1);// 201
			AddComplexComponent( (BaseAddon) this, 1313, 1, 5, 30, 2021, -1, "", 1);// 202
			AddComplexComponent( (BaseAddon) this, 221, 3, -4, 30, 1175, -1, "", 1);// 203
			AddComplexComponent( (BaseAddon) this, 7612, 1, -2, 30, 2224, -1, "", 1);// 205
			AddComplexComponent( (BaseAddon) this, 7611, 1, 1, 30, 2224, -1, "", 1);// 206
			AddComplexComponent( (BaseAddon) this, 1313, 3, 5, 10, 1175, -1, "", 1);// 207
			AddComplexComponent( (BaseAddon) this, 2854, 2, 4, 30, 0, 1, "", 1);// 208
			AddComplexComponent( (BaseAddon) this, 1873, 0, 7, 0, 1175, -1, "", 1);// 212
			AddComplexComponent( (BaseAddon) this, 1873, 1, 7, 0, 1175, -1, "", 1);// 213
			AddComplexComponent( (BaseAddon) this, 1873, 2, 7, 0, 1175, -1, "", 1);// 214
			AddComplexComponent( (BaseAddon) this, 1313, 3, -1, 30, 2021, -1, "", 1);// 215
			AddComplexComponent( (BaseAddon) this, 222, 1, 5, 30, 1175, -1, "", 1);// 216
			AddComplexComponent( (BaseAddon) this, 7613, 1, 0, 30, 2224, -1, "", 1);// 217
			AddComplexComponent( (BaseAddon) this, 1313, -1, -3, 10, 1175, -1, "", 1);// 219
			AddComplexComponent( (BaseAddon) this, 1313, -1, -2, 10, 1175, -1, "", 1);// 220
			AddComplexComponent( (BaseAddon) this, 1313, 0, -5, 10, 1175, -1, "", 1);// 231
			AddComplexComponent( (BaseAddon) this, 1313, 0, -4, 10, 1175, -1, "", 1);// 232
			AddComplexComponent( (BaseAddon) this, 1313, 0, -3, 10, 1175, -1, "", 1);// 233
			AddComplexComponent( (BaseAddon) this, 1313, 0, -2, 10, 1175, -1, "", 1);// 234
			AddComplexComponent( (BaseAddon) this, 2842, 1, -2, 36, 0, 1, "", 1);// 235
			AddComplexComponent( (BaseAddon) this, 7186, 1, -1, 38, 0, -1, "Bell of CTF", 1);// 236
			AddComplexComponent( (BaseAddon) this, 222, -1, 5, 30, 1175, -1, "", 1);// 237
			AddComplexComponent( (BaseAddon) this, 1874, 4, -1, 5, 1175, -1, "", 1);// 238
			AddComplexComponent( (BaseAddon) this, 220, 3, 5, 30, 1175, -1, "", 1);// 239
			AddComplexComponent( (BaseAddon) this, 1313, 0, 1, 30, 1175, -1, "", 1);// 251
			AddComplexComponent( (BaseAddon) this, 1313, 0, 2, 30, 1175, -1, "", 1);// 252
			AddComplexComponent( (BaseAddon) this, 1878, 4, 6, 5, 1175, -1, "", 1);// 253
			AddComplexComponent( (BaseAddon) this, 1874, 5, -2, 0, 1175, -1, "", 1);// 254
			AddComplexComponent( (BaseAddon) this, 1874, 5, 3, 0, 1175, -1, "", 1);// 255
			AddComplexComponent( (BaseAddon) this, 1874, 4, -2, 5, 1175, -1, "", 1);// 256
			AddComplexComponent( (BaseAddon) this, 1874, 5, 2, 0, 1175, -1, "", 1);// 257
			AddComplexComponent( (BaseAddon) this, 1313, -1, -5, 10, 1175, -1, "", 1);// 258
			AddComplexComponent( (BaseAddon) this, 6571, 6, 7, 22, 0, 0, "", 1);// 259
			AddComplexComponent( (BaseAddon) this, 1874, 5, -3, 0, 1175, -1, "", 1);// 260
			AddComplexComponent( (BaseAddon) this, 7613, 1, -1, 30, 2224, -1, "", 1);// 262
			AddComplexComponent( (BaseAddon) this, 1313, 0, 3, 30, 1175, -1, "", 1);// 263
			AddComplexComponent( (BaseAddon) this, 1313, -1, 4, 30, 2021, -1, "", 1);// 264
			AddComplexComponent( (BaseAddon) this, 1313, -1, 5, 30, 2021, -1, "", 1);// 265
			AddComplexComponent( (BaseAddon) this, 1313, 0, -5, 30, 2021, -1, "", 1);// 266
			AddComplexComponent( (BaseAddon) this, 1313, 0, -4, 30, 2021, -1, "", 1);// 268
			AddComplexComponent( (BaseAddon) this, 1313, 0, -3, 30, 1175, -1, "", 1);// 269
			AddComplexComponent( (BaseAddon) this, 1313, 0, -2, 30, 1175, -1, "", 1);// 270
			AddComplexComponent( (BaseAddon) this, 1313, 0, -1, 30, 1175, -1, "", 1);// 271
			AddComplexComponent( (BaseAddon) this, 222, 2, 5, 30, 1175, -1, "", 1);// 272
			AddComplexComponent( (BaseAddon) this, 1313, 1, -4, 10, 1175, -1, "", 1);// 273
			AddComplexComponent( (BaseAddon) this, 1313, 1, -3, 10, 1175, -1, "", 1);// 274
			AddComplexComponent( (BaseAddon) this, 1313, 1, -2, 10, 1175, -1, "", 1);// 275
			AddComplexComponent( (BaseAddon) this, 1313, 1, -1, 10, 1175, -1, "", 1);// 276
			AddComplexComponent( (BaseAddon) this, 1313, 1, 0, 10, 1175, -1, "", 1);// 277
			AddComplexComponent( (BaseAddon) this, 1313, 1, 1, 10, 1175, -1, "", 1);// 278
			AddComplexComponent( (BaseAddon) this, 1313, 1, 2, 10, 1175, -1, "", 1);// 279
			AddComplexComponent( (BaseAddon) this, 1313, 1, 3, 10, 1175, -1, "", 1);// 280
			AddComplexComponent( (BaseAddon) this, 1313, 1, 4, 10, 1175, -1, "", 1);// 281
			AddComplexComponent( (BaseAddon) this, 1313, -1, -5, 30, 2021, -1, "", 1);// 282
			AddComplexComponent( (BaseAddon) this, 1313, -1, -4, 30, 2021, -1, "", 1);// 283
			AddComplexComponent( (BaseAddon) this, 1313, -1, -3, 30, 2021, -1, "", 1);// 284
			AddComplexComponent( (BaseAddon) this, 1874, 4, 1, 5, 1175, -1, "", 1);// 285
			AddComplexComponent( (BaseAddon) this, 1874, 5, -5, 0, 1175, -1, "", 1);// 286
			AddComplexComponent( (BaseAddon) this, 1874, 4, 5, 5, 1175, -1, "", 1);// 291
			AddComplexComponent( (BaseAddon) this, 1313, 3, 0, 30, 2021, -1, "", 1);// 292
			AddComplexComponent( (BaseAddon) this, 1313, 3, 1, 30, 2021, -1, "", 1);// 293
			AddComplexComponent( (BaseAddon) this, 1313, 3, 2, 30, 2021, -1, "", 1);// 294
			AddComplexComponent( (BaseAddon) this, 1313, 3, 3, 30, 2021, -1, "", 1);// 295
			AddComplexComponent( (BaseAddon) this, 1313, 3, 4, 30, 2021, -1, "", 1);// 296
			AddComplexComponent( (BaseAddon) this, 7187, 1, 0, 39, 0, -1, "Book of Capture the Flag", 1);// 297
			AddComplexComponent( (BaseAddon) this, 1313, -1, 0, 10, 1175, -1, "", 1);// 298
			AddComplexComponent( (BaseAddon) this, 1313, -1, 1, 10, 1175, -1, "", 1);// 299
			AddComplexComponent( (BaseAddon) this, 1313, -1, 2, 10, 1175, -1, "", 1);// 300
			AddComplexComponent( (BaseAddon) this, 1313, -1, 3, 10, 1175, -1, "", 1);// 301
			AddComplexComponent( (BaseAddon) this, 1313, -1, 4, 10, 1175, -1, "", 1);// 302
			AddComplexComponent( (BaseAddon) this, 1313, -1, 5, 10, 1175, -1, "", 1);// 303
			AddComplexComponent( (BaseAddon) this, 2854, 2, -5, 30, 0, 1, "", 1);// 304
			AddComplexComponent( (BaseAddon) this, 1313, -1, -1, 10, 1175, -1, "", 1);// 305
			AddComplexComponent( (BaseAddon) this, 1313, 0, 2, 10, 1175, -1, "", 1);// 306
			AddComplexComponent( (BaseAddon) this, 1313, 0, 3, 10, 1175, -1, "", 1);// 307
			AddComplexComponent( (BaseAddon) this, 1313, 0, 4, 10, 1175, -1, "", 1);// 308
			AddComplexComponent( (BaseAddon) this, 1313, 0, 5, 10, 1175, -1, "", 1);// 309
			AddComplexComponent( (BaseAddon) this, 1873, -1, 7, 0, 1175, -1, "", 1);// 310
			AddComplexComponent( (BaseAddon) this, 1874, 4, 2, 5, 1175, -1, "", 1);// 313
			AddComplexComponent( (BaseAddon) this, 1874, 4, 0, 5, 1175, -1, "", 1);// 314
			AddComplexComponent( (BaseAddon) this, 1878, 5, 7, 0, 1175, -1, "", 1);// 315
			AddComplexComponent( (BaseAddon) this, 1874, 5, 6, 0, 1175, -1, "", 1);// 316
			AddComplexComponent( (BaseAddon) this, 1874, 4, -5, 5, 1175, -1, "", 1);// 317
			AddComplexComponent( (BaseAddon) this, 1874, 4, -4, 5, 1175, -1, "", 1);// 318
			AddComplexComponent( (BaseAddon) this, 2842, 1, 1, 36, 0, 1, "", 1);// 320
			AddComplexComponent( (BaseAddon) this, 1313, 1, 5, 10, 1175, -1, "", 1);// 322
			AddComplexComponent( (BaseAddon) this, 1313, 2, -5, 10, 1175, -1, "", 1);// 323
			AddComplexComponent( (BaseAddon) this, 1313, 2, -4, 10, 1175, -1, "", 1);// 324
			AddComplexComponent( (BaseAddon) this, 1313, 2, -2, 10, 1175, -1, "", 1);// 325
			AddComplexComponent( (BaseAddon) this, 1313, 2, -3, 10, 1175, -1, "", 1);// 326
			AddComplexComponent( (BaseAddon) this, 1313, 2, -1, 10, 1175, -1, "", 1);// 327
			AddComplexComponent( (BaseAddon) this, 1313, 2, 0, 10, 1175, -1, "", 1);// 328
			AddComplexComponent( (BaseAddon) this, 1313, 2, 1, 10, 1175, -1, "", 1);// 329
			AddComplexComponent( (BaseAddon) this, 1313, 1, -2, 30, 1175, -1, "", 1);// 330
			AddComplexComponent( (BaseAddon) this, 1313, 1, -1, 30, 1175, -1, "", 1);// 331
			AddComplexComponent( (BaseAddon) this, 1873, 4, 7, 0, 1175, -1, "", 1);// 332
			AddComplexComponent( (BaseAddon) this, 1873, 3, 6, 5, 1175, -1, "", 1);// 333
			AddComplexComponent( (BaseAddon) this, 1873, 2, 6, 5, 1175, -1, "", 1);// 334
			AddComplexComponent( (BaseAddon) this, 1873, 1, 6, 5, 1175, -1, "", 1);// 335
			AddComplexComponent( (BaseAddon) this, 1873, 0, 6, 5, 1175, -1, "", 1);// 336
			AddComplexComponent( (BaseAddon) this, 1873, -1, 6, 5, 1175, -1, "", 1);// 337
			AddComplexComponent( (BaseAddon) this, 1313, 3, 5, 30, 2021, -1, "", 1);// 339
			AddComplexComponent( (BaseAddon) this, 221, 3, -1, 30, 1175, -1, "", 1);// 340
			AddComplexComponent( (BaseAddon) this, 221, 3, 0, 30, 1175, -1, "", 1);// 341
			AddComplexComponent( (BaseAddon) this, 221, 3, 1, 30, 1175, -1, "", 1);// 342
			AddComplexComponent( (BaseAddon) this, 221, 3, 2, 30, 1175, -1, "", 1);// 343
			AddComplexComponent( (BaseAddon) this, 221, 3, 3, 30, 1175, -1, "", 1);// 344
			AddComplexComponent( (BaseAddon) this, 221, 3, 4, 30, 1175, -1, "", 1);// 345
			AddComplexComponent( (BaseAddon) this, 1874, 4, -3, 5, 1175, -1, "", 1);// 346
			AddComplexComponent( (BaseAddon) this, 1313, 1, -5, 10, 1175, -1, "", 1);// 347

		}

		public ctfhouseAddon( Serial serial ) : base( serial )
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

	public class ctfhouseAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new ctfhouseAddon();
			}
		}

		[Constructable]
		public ctfhouseAddonDeed()
		{
			Name = "ctfhouse";
		}

		public ctfhouseAddonDeed( Serial serial ) : base( serial )
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