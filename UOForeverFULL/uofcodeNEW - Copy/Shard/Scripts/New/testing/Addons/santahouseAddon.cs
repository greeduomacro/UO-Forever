
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class santahouseAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {6092, -1, -6, 11}, {2778, -1, 2, 1}, {2786, 1, 7, 1}// 1	2	4	
			, {6091, -2, -7, 10}, {14974, -2, -6, 10}, {11233, 1, -4, 17}// 5	6	7	
			, {9009, -3, 0, 11}, {2783, -3, 5, 1}, {9008, 1, -6, 16}// 8	9	14	
			, {6077, -2, -6, 10}, {2784, 2, 1, 1}, {11001, 1, -3, 1}// 16	18	23	
			, {2786, 0, 7, 1}, {2783, -3, 3, 1}, {9007, 1, -6, 11}// 24	25	26	
			, {2785, 3, 4, 1}, {6077, -2, -5, 10}, {6077, -2, -4, 11}// 27	29	30	
			, {6088, -3, -2, 11}, {6092, -1, -5, 11}, {2783, -3, 4, 1}// 31	32	33	
			, {11233, 1, -4, 16}, {6087, -1, -2, 11}, {9003, 3, -4, 13}// 34	35	36	
			, {2778, 2, 6, 1}, {6090, -3, -5, 10}, {2778, -2, 5, 1}// 37	38	39	
			, {11233, 1, -4, 15}, {2778, 1, 6, 1}, {2785, 3, 5, 1}// 41	42	44	
			, {2785, 3, 6, 1}, {6090, -3, -4, 11}, {7869, -3, -7, 10}// 46	47	48	
			, {2780, -3, 1, 1}, {2778, -1, 4, 1}, {6086, -1, -7, 11}// 49	51	52	
			, {6090, -3, -3, 10}, {9008, -3, 4, 12}, {3379, -2, -4, 11}// 55	57	61	
			, {9002, -2, 1, 1}, {3310, -2, -7, 0}, {3310, -1, -7, 0}// 62	63	67	
			, {2786, -1, 7, 1}, {2778, 0, 6, 1}, {2783, -3, 2, 1}// 68	69	72	
			, {9003, 2, 7, 11}, {2778, 0, 2, 1}, {2778, 2, 5, 1}// 73	74	76	
			, {2778, -1, 5, 1}, {2784, -1, 1, 1}, {2778, 0, 5, 1}// 78	82	83	
			, {2778, -1, 6, 1}, {2778, 1, 5, 1}, {2778, 1, 2, 1}// 84	85	86	
			, {2781, -3, 7, 1}, {14973, -2, -5, 10}, {9007, 1, 0, 6}// 87	89	90	
			, {2783, -3, 6, 1}, {2779, 3, 7, 1}, {2778, -1, 3, 1}// 93	95	96	
			, {2778, 2, 2, 1}, {2784, 0, 1, 1}, {6089, -2, -2, 11}// 97	99	100	
			, {2778, -2, 2, 1}, {9008, -3, -2, 14}, {2778, -2, 3, 1}// 101	104	107	
			, {14944, -2, -3, 10}, {9008, 2, 6, 12}, {9007, 1, -4, 13}// 108	110	111	
			, {6092, -1, -4, 11}, {2782, 3, 1, 1}, {2786, -2, 7, 1}// 112	113	115	
			, {2786, 2, 7, 1}, {6092, -1, -3, 11}, {2784, -2, 1, 1}// 116	117	118	
			, {2778, 0, 3, 1}, {2778, 0, 4, 1}, {11229, 1, -4, 17}// 119	120	121	
			, {2778, -2, 4, 1}, {3314, -3, -4, 0}, {6077, -2, -3, 11}// 122	123	124	
			, {2778, -2, 6, 1}, {2785, 3, 2, 1}, {2616, -3, 2, 1}// 125	126	127	
			, {2616, -3, 2, 9}, {3314, -3, -5, 0}, {3314, -3, -6, 0}// 128	129	130	
			, {2778, 1, 4, 1}, {2778, 1, 3, 1}, {2778, 2, 3, 1}// 131	132	133	
			, {2778, 2, 4, 1}, {2785, 3, 3, 1}, {2784, 1, 1, 1}// 134	135	136	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new santahouseAddonDeed();
			}
		}

		[ Constructable ]
		public santahouseAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 9004, 2, -6, 13, 38, -1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 3132, -3, -7, 10, 38, -1, "", 1);// 10
			AddComplexComponent( (BaseAddon) this, 3131, -3, -4, 10, 38, -1, "", 1);// 11
			AddComplexComponent( (BaseAddon) this, 5168, -2, -2, 10, 38, 1, "", 1);// 12
			AddComplexComponent( (BaseAddon) this, 3131, -3, -5, 10, 38, -1, "", 1);// 13
			AddComplexComponent( (BaseAddon) this, 9002, 3, -6, 19, 133, -1, "", 1);// 15
			AddComplexComponent( (BaseAddon) this, 9002, -3, 5, 11, 38, -1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 9002, -3, 6, 11, 53, -1, "", 1);// 19
			AddComplexComponent( (BaseAddon) this, 5164, -1, -7, 10, 2003, 1, "", 1);// 20
			AddComplexComponent( (BaseAddon) this, 5168, -1, -5, 11, 38, 1, "", 1);// 21
			AddComplexComponent( (BaseAddon) this, 5168, -1, -2, 10, 38, 1, "", 1);// 22
			AddComplexComponent( (BaseAddon) this, 9004, 3, -6, 13, 38, -1, "", 1);// 28
			AddComplexComponent( (BaseAddon) this, 9002, 3, -5, 13, 38, -1, "", 1);// 40
			AddComplexComponent( (BaseAddon) this, 9005, -3, 5, 3, 1161, -1, "", 1);// 43
			AddComplexComponent( (BaseAddon) this, 9003, 2, -6, 16, 2075, -1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 3131, -3, -6, 10, 38, -1, "", 1);// 50
			AddComplexComponent( (BaseAddon) this, 5168, -1, -4, 10, 38, 1, "", 1);// 53
			AddComplexComponent( (BaseAddon) this, 5172, -1, -5, 10, 2003, 1, "", 1);// 54
			AddComplexComponent( (BaseAddon) this, 2854, -3, -1, 1, 0, 1, "", 1);// 56
			AddComplexComponent( (BaseAddon) this, 5164, -1, -2, 10, 2003, 1, "", 1);// 58
			AddComplexComponent( (BaseAddon) this, 5172, -1, -7, 10, 2003, 1, "", 1);// 59
			AddComplexComponent( (BaseAddon) this, 2846, 1, 0, 11, 0, 1, "", 1);// 60
			AddComplexComponent( (BaseAddon) this, 5164, -2, -2, 10, 2003, 1, "", 1);// 64
			AddComplexComponent( (BaseAddon) this, 5164, -1, -5, 9, 2003, 1, "", 1);// 65
			AddComplexComponent( (BaseAddon) this, 9002, 3, -6, 13, 3, -1, "", 1);// 66
			AddComplexComponent( (BaseAddon) this, 5168, -1, -7, 10, 38, 1, "", 1);// 70
			AddComplexComponent( (BaseAddon) this, 5172, -1, -2, 10, 2003, 1, "", 1);// 71
			AddComplexComponent( (BaseAddon) this, 5172, -1, -3, 10, 2003, 1, "", 1);// 75
			AddComplexComponent( (BaseAddon) this, 3132, -2, -7, 10, 38, -1, "", 1);// 77
			AddComplexComponent( (BaseAddon) this, 3132, -1, -7, 10, 38, -1, "", 1);// 79
			AddComplexComponent( (BaseAddon) this, 2854, -3, 1, 1, 0, 1, "", 1);// 80
			AddComplexComponent( (BaseAddon) this, 9002, 2, -6, 19, 283, -1, "", 1);// 81
			AddComplexComponent( (BaseAddon) this, 2557, -3, 6, 15, 0, 1, "", 1);// 88
			AddComplexComponent( (BaseAddon) this, 5168, -1, -3, 10, 38, 1, "", 1);// 91
			AddComplexComponent( (BaseAddon) this, 5168, -1, -6, 10, 38, 1, "", 1);// 92
			AddComplexComponent( (BaseAddon) this, 9005, -3, 3, 2, 38, -1, "", 1);// 94
			AddComplexComponent( (BaseAddon) this, 9002, 2, -5, 13, 1367, -1, "", 1);// 98
			AddComplexComponent( (BaseAddon) this, 9003, 2, 4, 11, 1367, -1, "", 1);// 102
			AddComplexComponent( (BaseAddon) this, 9003, 3, -6, 16, 43, -1, "", 1);// 103
			AddComplexComponent( (BaseAddon) this, 9005, -3, 7, 3, 38, -1, "", 1);// 105
			AddComplexComponent( (BaseAddon) this, 9003, 2, 5, 11, 1365, -1, "", 1);// 106
			AddComplexComponent( (BaseAddon) this, 9002, 2, -6, 13, 122, -1, "", 1);// 109
			AddComplexComponent( (BaseAddon) this, 9002, -3, 7, 11, 2003, -1, "", 1);// 114

		}

		public santahouseAddon( Serial serial ) : base( serial )
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

	public class santahouseAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new santahouseAddon();
			}
		}

		[Constructable]
		public santahouseAddonDeed()
		{
			Name = "santahouse";
		}

		public santahouseAddonDeed( Serial serial ) : base( serial )
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