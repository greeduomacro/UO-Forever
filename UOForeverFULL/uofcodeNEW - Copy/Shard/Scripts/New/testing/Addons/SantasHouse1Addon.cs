
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SantasHouse1Addon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {6090, -5, 3, 19}, {6088, -5, 4, 19}, {6079, 6, 3, 19}// 1	2	3	
			, {6088, -3, 2, 59}, {6088, -4, 2, 59}, {11528, -2, 1, 39}// 5	6	7	
			, {6077, 0, -4, 59}, {6077, -3, -1, 59}, {6083, 4, -5, 20}// 8	9	11	
			, {6077, 2, -3, 59}, {6077, 1, -2, 59}, {6086, 5, 1, 19}// 12	13	14	
			, {3379, 3, -4, 62}, {14967, -1, 1, 59}, {6092, 3, -3, 59}// 15	16	19	
			, {6089, -3, 2, 59}, {3379, 2, 2, 59}, {6093, 3, 2, 59}// 20	21	22	
			, {12377, 1, -2, 43}, {6086, 4, 0, 19}, {6077, -3, 1, 59}// 23	24	25	
			, {6089, -1, 3, 59}, {11761, -2, -2, 49}, {6077, -4, -3, 59}// 28	29	30	
			, {6077, 2, -2, 59}, {3379, 3, 1, 62}, {6077, 0, -3, 59}// 31	32	36	
			, {3379, 3, 3, 62}, {6092, 3, -2, 59}, {6077, -4, 0, 59}// 37	38	39	
			, {6078, 4, -4, 20}, {6077, -2, 0, 59}, {6087, 1, 3, 59}// 40	41	46	
			, {3379, 6, 0, 18}, {6077, 0, 1, 59}, {6088, -2, 3, 59}// 47	49	50	
			, {6092, 3, -1, 59}, {3312, 4, -2, 19}, {6077, 4, 2, 19}// 51	52	54	
			, {3379, 1, 3, 62}, {6084, 5, 2, 19}, {6087, 4, -2, 20}// 55	56	57	
			, {12376, 1, -1, 42}, {6077, 5, 3, 19}, {6077, 0, 2, 59}// 58	60	61	
			, {6087, 5, -3, 20}, {3379, -3, 3, 62}, {6077, -4, -1, 59}// 62	64	65	
			, {6077, -2, -3, 59}, {6092, 5, -4, 20}, {6086, 3, -4, 59}// 67	68	69	
			, {6087, 3, 1, 59}, {3379, -3, 2, 59}, {6077, 4, 3, 19}// 70	71	72	
			, {6077, 1, -1, 59}, {6077, -1, -4, 59}, {6077, 1, -3, 59}// 73	74	75	
			, {12375, 0, 0, 42}, {6077, 1, 0, 59}, {3312, 4, 1, 19}// 76	77	78	
			, {6077, -4, -4, 59}, {3314, 4, -3, 21}, {11526, -2, -3, 19}// 79	80	81	
			, {6084, 4, 1, 19}, {6077, -1, -1, 59}, {6081, 4, -3, 20}// 82	83	85	
			, {6077, 0, 0, 59}, {3349, 4, 2, 19}, {6086, 5, -5, 0}// 87	89	90	
			, {6091, 6, 2, 19}, {6092, 3, 0, 59}, {3350, 4, -3, 20}// 91	92	94	
			, {6077, 1, -4, 59}, {3342, 4, 1, 19}, {3345, 4, 1, 19}// 95	96	97	
			, {3342, 4, -3, 20}, {3348, 4, -2, 19}, {6077, 1, 1, 59}// 98	99	100	
			, {6081, 2, 1, 59}, {12374, 1, 0, 42}, {6077, -2, -2, 59}// 101	102	106	
			, {6081, 1, 2, 59}, {6093, 5, -2, 19}, {6077, -4, -2, 59}// 107	110	111	
			, {6077, -1, 0, 59}, {6077, -3, 0, 59}, {6089, 0, 3, 59}// 112	113	114	
			, {6077, -2, -4, 59}, {6087, 2, 2, 59}, {3379, -4, 2, 59}// 115	116	117	
			, {6077, -3, -3, 59}, {6084, 2, -4, 59}, {14966, 0, 1, 59}// 118	119	120	
			, {6077, -3, -4, 59}, {6077, -3, -2, 59}, {6077, -1, 1, 59}// 121	122	123	
			, {6077, -1, -2, 59}, {6077, 0, -1, 59}, {6077, -1, 2, 59}// 124	125	126	
			, {6082, -4, 1, 59}, {6077, -2, 1, 59}, {6077, -2, -1, 59}// 127	128	129	
			, {6077, -1, -3, 59}, {6077, 2, 0, 59}, {6077, 2, -1, 59}// 130	131	132	
			, {6077, 0, -2, 59}, {6082, -2, 2, 59}, {6093, -3, 3, 62}// 133	134	135	
			, {6093, 1, 3, 59}, {11740, 2, -4, 39}, {11761, -2, -2, 39}// 136	139	148	
			, {11761, -2, -2, 44}, {1209, -3, -3, 34}, {1209, -2, -2, 39}// 149	155	156	
			, {10944, 0, -4, 39}, {11528, -2, 0, 39}, {11528, -2, 2, 39}// 157	158	166	
			, {11526, -2, -4, 19}, {6089, 3, 4, 19}, {3308, -2, 4, 19}// 167	189	190	
			, {6089, 6, 4, 20}, {6089, 2, 4, 19}, {6089, -2, 4, 19}// 191	192	193	
			, {6089, 1, 4, 19}, {3310, -4, 4, 19}, {6089, 5, 4, 20}// 194	195	196	
			, {3308, 0, 4, 19}, {6089, -4, 4, 19}, {6089, -3, 4, 19}// 197	198	199	
			, {6089, 0, 4, 19}, {6089, 5, 5, 30}, {6089, -1, 4, 19}// 200	201	202	
			, {3310, 3, 4, 19}, {3310, -3, 4, 19}, {3308, 2, 4, 19}// 203	204	205	
			, {3310, 1, 4, 19}, {3310, -1, 4, 19}// 206	207	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new SantasHouse1AddonDeed();
			}
		}

		[ Constructable ]
		public SantasHouse1Addon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 11763, -2, -1, 39, 1157, -1, "", 1);// 4
			AddComplexComponent( (BaseAddon) this, 11763, -2, -1, 49, 1157, -1, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 2796, 2, 1, 39, 1161, -1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 2796, 0, -3, 19, 2067, -1, "", 1);// 18
			AddComplexComponent( (BaseAddon) this, 2796, -1, -3, 39, 1161, -1, "", 1);// 26
			AddComplexComponent( (BaseAddon) this, 9070, 0, -2, 25, 1157, 0, "", 1);// 27
			AddComplexComponent( (BaseAddon) this, 11763, -2, -1, 44, 1157, -1, "", 1);// 33
			AddComplexComponent( (BaseAddon) this, 5534, -4, -2, 39, 1157, -1, "", 1);// 34
			AddComplexComponent( (BaseAddon) this, 2796, -2, 1, 39, 1161, -1, "", 1);// 35
			AddComplexComponent( (BaseAddon) this, 2796, 2, 0, 39, 1161, -1, "", 1);// 42
			AddComplexComponent( (BaseAddon) this, 2796, -1, -3, 19, 2067, -1, "", 1);// 43
			AddComplexComponent( (BaseAddon) this, 2802, -2, 0, 19, 2067, -1, "", 1);// 44
			AddComplexComponent( (BaseAddon) this, 2796, -2, -1, 39, 1161, -1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 2854, -1, -4, 39, 0, 1, "", 1);// 48
			AddComplexComponent( (BaseAddon) this, 2800, -2, 1, 19, 2067, -1, "", 1);// 53
			AddComplexComponent( (BaseAddon) this, 2796, 1, 1, 39, 1161, -1, "", 1);// 59
			AddComplexComponent( (BaseAddon) this, 2804, 1, 0, 19, 2067, -1, "", 1);// 63
			AddComplexComponent( (BaseAddon) this, 9007, 0, -2, 23, 2067, -1, "", 1);// 66
			AddComplexComponent( (BaseAddon) this, 2804, 1, -3, 19, 2067, -1, "", 1);// 84
			AddComplexComponent( (BaseAddon) this, 11636, -2, -4, 42, 0, -1, "Map of Sosaria", 1);// 86
			AddComplexComponent( (BaseAddon) this, 2796, 2, -1, 39, 1161, -1, "", 1);// 88
			AddComplexComponent( (BaseAddon) this, 3131, 4, 1, 24, 1365, -1, "", 1);// 93
			AddComplexComponent( (BaseAddon) this, 3131, 4, -2, 25, 1365, -1, "", 1);// 103
			AddComplexComponent( (BaseAddon) this, 2804, 1, -1, 19, 2067, -1, "", 1);// 104
			AddComplexComponent( (BaseAddon) this, 9005, 4, 2, 39, 38, -1, "", 1);// 105
			AddComplexComponent( (BaseAddon) this, 9005, 4, -3, 39, 38, -1, "", 1);// 108
			AddComplexComponent( (BaseAddon) this, 2852, 4, -2, 19, 0, 1, "", 1);// 109
			AddComplexComponent( (BaseAddon) this, 4653, 5, 0, 18, 94, -1, "water", 1);// 137
			AddComplexComponent( (BaseAddon) this, 7712, 0, -1, 25, 0, -1, "Santa's Book", 1);// 138
			AddComplexComponent( (BaseAddon) this, 5534, -4, 1, 39, 1157, -1, "", 1);// 140
			AddComplexComponent( (BaseAddon) this, 2798, 1, 1, 19, 2067, -1, "", 1);// 141
			AddComplexComponent( (BaseAddon) this, 2802, -2, -2, 19, 2067, -1, "", 1);// 142
			AddComplexComponent( (BaseAddon) this, 2796, 0, 1, 39, 1161, -1, "", 1);// 143
			AddComplexComponent( (BaseAddon) this, 2796, -2, -3, 39, 1161, -1, "", 1);// 144
			AddComplexComponent( (BaseAddon) this, 2796, -2, 0, 39, 1161, -1, "", 1);// 145
			AddComplexComponent( (BaseAddon) this, 2796, -2, -2, 39, 1161, -1, "", 1);// 146
			AddComplexComponent( (BaseAddon) this, 2802, -2, -1, 19, 2067, -1, "", 1);// 147
			AddComplexComponent( (BaseAddon) this, 2805, 0, 1, 19, 2067, -1, "", 1);// 150
			AddComplexComponent( (BaseAddon) this, 2803, 0, -4, 19, 2067, -1, "", 1);// 151
			AddComplexComponent( (BaseAddon) this, 2804, 1, -2, 19, 2067, -1, "", 1);// 152
			AddComplexComponent( (BaseAddon) this, 2801, 1, -4, 19, 2067, -1, "", 1);// 153
			AddComplexComponent( (BaseAddon) this, 2796, 0, -2, 19, 2067, -1, "", 1);// 154
			AddComplexComponent( (BaseAddon) this, 2796, 0, -1, 19, 2067, -1, "", 1);// 159
			AddComplexComponent( (BaseAddon) this, 2796, -1, -2, 19, 2067, -1, "", 1);// 160
			AddComplexComponent( (BaseAddon) this, 2802, -2, -3, 19, 2067, -1, "", 1);// 161
			AddComplexComponent( (BaseAddon) this, 2803, -1, -4, 19, 2067, -1, "", 1);// 162
			AddComplexComponent( (BaseAddon) this, 2805, -1, 1, 19, 2067, -1, "", 1);// 163
			AddComplexComponent( (BaseAddon) this, 2796, 0, 0, 19, 2067, -1, "", 1);// 164
			AddComplexComponent( (BaseAddon) this, 2796, -1, 0, 19, 2067, -1, "", 1);// 165
			AddComplexComponent( (BaseAddon) this, 2796, -1, -1, 19, 2067, -1, "", 1);// 168
			AddComplexComponent( (BaseAddon) this, 2796, 2, -2, 39, 1161, -1, "", 1);// 169
			AddComplexComponent( (BaseAddon) this, 2796, 2, -3, 39, 1161, -1, "", 1);// 170
			AddComplexComponent( (BaseAddon) this, 2796, 1, -3, 39, 1161, -1, "", 1);// 171
			AddComplexComponent( (BaseAddon) this, 2796, 0, -3, 39, 1161, -1, "", 1);// 172
			AddComplexComponent( (BaseAddon) this, 2729, 0, 0, 39, 1161, -1, "", 1);// 173
			AddComplexComponent( (BaseAddon) this, 2729, 1, 0, 39, 1161, -1, "", 1);// 174
			AddComplexComponent( (BaseAddon) this, 2729, 1, -1, 39, 1161, -1, "", 1);// 175
			AddComplexComponent( (BaseAddon) this, 2729, 1, -2, 39, 1161, -1, "", 1);// 176
			AddComplexComponent( (BaseAddon) this, 7617, 0, 0, 19, 1107, -1, "", 1);// 177
			AddComplexComponent( (BaseAddon) this, 7618, 0, -2, 19, 1107, -1, "", 1);// 178
			AddComplexComponent( (BaseAddon) this, 7619, 0, -1, 19, 1107, -1, "", 1);// 179
			AddComplexComponent( (BaseAddon) this, 2557, -4, 1, 33, 0, 1, "", 1);// 180
			AddComplexComponent( (BaseAddon) this, 2559, -4, -4, 53, 0, 1, "", 1);// 181
			AddComplexComponent( (BaseAddon) this, 2562, 0, -4, 59, 0, 1, "", 1);// 182
			AddComplexComponent( (BaseAddon) this, 2796, -1, 1, 39, 1161, -1, "", 1);// 183
			AddComplexComponent( (BaseAddon) this, 2729, -1, -2, 39, 1161, -1, "", 1);// 184
			AddComplexComponent( (BaseAddon) this, 2729, -1, -1, 39, 1161, -1, "", 1);// 185
			AddComplexComponent( (BaseAddon) this, 2799, -2, -4, 19, 2067, -1, "", 1);// 186
			AddComplexComponent( (BaseAddon) this, 2729, -1, 0, 39, 1161, -1, "", 1);// 187
			AddComplexComponent( (BaseAddon) this, 2854, 2, 2, 19, 0, 1, "", 1);// 188

		}

		public SantasHouse1Addon( Serial serial ) : base( serial )
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

	public class SantasHouse1AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new SantasHouse1Addon();
			}
		}

		[Constructable]
		public SantasHouse1AddonDeed()
		{
			Name = "SantasHouse1";
		}

		public SantasHouse1AddonDeed( Serial serial ) : base( serial )
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