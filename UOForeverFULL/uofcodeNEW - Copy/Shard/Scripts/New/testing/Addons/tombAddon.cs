
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class tombAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {12913, 4, -8, 0}, {12913, 4, -7, 0}, {11827, 4, -7, 0}// 1	2	3	
			, {11827, 4, -8, 0}, {3793, -1, 1, 5}, {3793, 3, -4, 0}// 4	114	115	
			, {3790, 2, -1, 2}, {3792, -3, 3, 0}, {3789, -2, -3, 2}// 116	117	118	
			, {4313, -3, 0, 2}, {6929, -3, 1, 19}, {6884, -3, 1, 19}// 119	121	122	
			, {6930, -3, 1, 19}, {7385, -1, -6, 0}, {7385, -1, -5, 0}// 123	126	127	
			, {7385, -1, -4, 0}, {7385, 0, -6, 0}, {7385, 0, -5, 0}// 128	129	130	
			, {7385, 0, -4, 0}, {7385, 1, -6, 0}, {7385, 1, -5, 0}// 131	132	133	
			, {7385, 1, -4, 0}, {7385, 2, -6, 0}, {7385, 2, -5, 0}// 134	135	136	
			, {7385, 2, -4, 0}, {7385, -3, 3, 0}, {7385, -2, 3, 0}// 137	138	139	
			, {7385, -1, 3, 0}, {7385, 0, 3, 0}, {7381, 2, -7, 0}// 140	141	142	
			, {7382, 1, -7, 0}, {7382, 0, -7, 0}, {7378, -2, -4, 0}// 143	144	145	
			, {7378, -2, -6, 0}, {7377, -2, -5, 0}, {7377, -4, 3, 0}// 146	147	148	
			, {7376, 3, -7, 0}, {7375, 3, -6, 0}, {7375, -2, -7, 0}// 149	150	151	
			, {7382, -1, -7, 0}, {7386, 1, 3, 0}, {7386, 2, 3, 0}// 152	153	154	
			, {7386, 3, 3, 0}, {7386, 3, -6, 0}, {7386, 3, -5, 0}// 155	156	157	
			, {7386, 3, -4, 0}, {7386, 4, -6, 0}, {7386, 4, -5, 0}// 158	159	160	
			, {7386, 4, -4, 0}, {7386, 4, -6, 0}, {7386, 4, -5, 0}// 161	162	163	
			, {7386, 4, -4, 0}, {7385, -3, 4, 0}, {7385, -3, 5, 0}// 164	169	170	
			, {7385, -3, 6, 0}, {7385, -3, 7, 0}, {7385, -2, 4, 0}// 171	172	173	
			, {7385, -2, 5, 0}, {7385, -2, 6, 0}, {7385, -2, 7, 0}// 174	175	176	
			, {7385, -1, 4, 0}, {7385, -1, 5, 0}, {7385, -1, 6, 0}// 177	178	179	
			, {7385, -1, 7, 0}, {7385, 0, 4, 0}, {7385, 0, 5, 0}// 180	181	182	
			, {7385, 0, 6, 0}, {7385, 0, 7, 0}, {7389, 1, 4, 0}// 183	184	185	
			, {7380, 0, 8, 0}, {7379, -3, 8, 0}, {7378, -4, 6, 0}// 186	187	188	
			, {7378, -4, 4, 0}, {7377, -4, 5, 0}, {7374, 1, 8, 0}// 189	190	191	
			, {7373, -4, 8, 0}, {7377, -4, 7, 0}, {7380, -1, 8, 0}// 192	193	194	
			, {7380, -2, 8, 0}, {7387, 1, 5, 0}, {7388, 2, 5, 0}// 195	196	197	
			, {7389, 2, 4, 0}, {7390, 1, 6, 0}, {7390, -3, 4, 0}// 198	199	200	
			, {7391, -2, 7, 0}, {7386, 1, 6, 0}, {7386, 1, 7, 0}// 201	202	203	
			, {7386, 2, 6, 0}, {7386, 2, 7, 0}, {7386, 3, 5, 0}// 204	205	206	
			, {7386, 3, 6, 0}, {7386, 3, 7, 0}, {7386, 4, 5, 0}// 207	208	209	
			, {7386, 4, 6, 0}, {7386, 4, 7, 0}, {7386, 1, 4, 0}// 210	211	212	
			, {7386, 2, 4, 0}, {7386, 3, 4, 0}, {4542, 4, 6, 1}// 213	214	216	
			, {6875, 3, 5, 0}, {6875, 3, 6, 0}, {6873, 4, 6, 5}// 218	219	220	
			, {6873, 4, 6, 2}, {6873, 4, 6, 2}// 221	222	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new tombAddonDeed();
			}
		}

		[ Constructable ]
		public tombAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 11827, 4, -3, 0, 1155, -1, "tomb", 1);// 5
			AddComplexComponent( (BaseAddon) this, 12913, 4, -3, 0, 1155, -1, "tomb", 1);// 6
			AddComplexComponent( (BaseAddon) this, 4541, -4, 2, 0, 32, -1, "tomb", 1);// 7
			AddComplexComponent( (BaseAddon) this, 4541, -3, 2, 0, 32, -1, "tomb", 1);// 8
			AddComplexComponent( (BaseAddon) this, 4541, -2, 2, 0, 32, -1, "tomb", 1);// 9
			AddComplexComponent( (BaseAddon) this, 4541, -1, 2, 0, 32, -1, "tomb", 1);// 10
			AddComplexComponent( (BaseAddon) this, 4541, 0, 2, 0, 32, -1, "tomb", 1);// 11
			AddComplexComponent( (BaseAddon) this, 4541, 1, 2, 0, 32, -1, "tomb", 1);// 12
			AddComplexComponent( (BaseAddon) this, 4541, 2, 2, 0, 32, -1, "tomb", 1);// 13
			AddComplexComponent( (BaseAddon) this, 4541, 3, 2, 0, 32, -1, "tomb", 1);// 14
			AddComplexComponent( (BaseAddon) this, 4541, 4, 2, 0, 32, -1, "tomb", 1);// 15
			AddComplexComponent( (BaseAddon) this, 4543, -4, 1, 0, 1155, -1, "tomb", 1);// 16
			AddComplexComponent( (BaseAddon) this, 4543, -3, 1, 0, 1155, -1, "tomb", 1);// 17
			AddComplexComponent( (BaseAddon) this, 4543, -2, 1, 0, 1155, -1, "tomb", 1);// 18
			AddComplexComponent( (BaseAddon) this, 4543, -1, 1, 0, 1155, -1, "tomb", 1);// 19
			AddComplexComponent( (BaseAddon) this, 4543, 0, 1, 0, 1155, -1, "tomb", 1);// 20
			AddComplexComponent( (BaseAddon) this, 4543, 1, 1, 0, 1155, -1, "tomb", 1);// 21
			AddComplexComponent( (BaseAddon) this, 4543, 2, 1, 0, 1155, -1, "tomb", 1);// 22
			AddComplexComponent( (BaseAddon) this, 4543, 3, 1, 0, 1155, -1, "tomb", 1);// 23
			AddComplexComponent( (BaseAddon) this, 4543, 4, 1, 0, 1155, -1, "tomb", 1);// 24
			AddComplexComponent( (BaseAddon) this, 4543, -4, -3, 0, 32, -1, "tomb", 1);// 25
			AddComplexComponent( (BaseAddon) this, 4543, -3, -3, 0, 32, -1, "tomb", 1);// 26
			AddComplexComponent( (BaseAddon) this, 4543, -2, -3, 0, 32, -1, "tomb", 1);// 27
			AddComplexComponent( (BaseAddon) this, 4543, -1, -3, 0, 32, -1, "tomb", 1);// 28
			AddComplexComponent( (BaseAddon) this, 4543, 0, -3, 0, 32, -1, "tomb", 1);// 29
			AddComplexComponent( (BaseAddon) this, 4543, 1, -3, 0, 32, -1, "tomb", 1);// 30
			AddComplexComponent( (BaseAddon) this, 4543, 2, -3, 0, 32, -1, "tomb", 1);// 31
			AddComplexComponent( (BaseAddon) this, 4543, 3, -3, 0, 32, -1, "tomb", 1);// 32
			AddComplexComponent( (BaseAddon) this, 4543, 4, -3, 0, 33, -1, "tomb", 1);// 33
			AddComplexComponent( (BaseAddon) this, 4541, -4, -2, 0, 1155, -1, "tomb", 1);// 34
			AddComplexComponent( (BaseAddon) this, 4541, -3, -2, 0, 1155, -1, "tomb", 1);// 35
			AddComplexComponent( (BaseAddon) this, 4541, -2, -2, 0, 1155, -1, "tomb", 1);// 36
			AddComplexComponent( (BaseAddon) this, 4541, -1, -2, 0, 1155, -1, "tomb", 1);// 37
			AddComplexComponent( (BaseAddon) this, 4541, 0, -2, 0, 1155, -1, "tomb", 1);// 38
			AddComplexComponent( (BaseAddon) this, 4541, 1, -2, 0, 1155, -1, "tomb", 1);// 39
			AddComplexComponent( (BaseAddon) this, 4541, 2, -2, 0, 1155, -1, "tomb", 1);// 40
			AddComplexComponent( (BaseAddon) this, 4541, 3, -2, 0, 1155, -1, "tomb", 1);// 41
			AddComplexComponent( (BaseAddon) this, 4541, 4, -2, 0, 1155, -1, "tomb", 1);// 42
			AddComplexComponent( (BaseAddon) this, 3928, 2, -1, 0, 0, -1, "tomb", 1);// 43
			AddComplexComponent( (BaseAddon) this, 3928, 0, -1, 0, 0, -1, "tomb", 1);// 44
			AddComplexComponent( (BaseAddon) this, 3928, -2, -1, 0, 0, -1, "tomb", 1);// 45
			AddComplexComponent( (BaseAddon) this, 3928, -1, -1, 0, 0, -1, "tomb", 1);// 46
			AddComplexComponent( (BaseAddon) this, 3928, 1, -1, 0, 0, -1, "tomb", 1);// 47
			AddComplexComponent( (BaseAddon) this, 3928, 3, -1, 0, 0, -1, "tomb", 1);// 48
			AddComplexComponent( (BaseAddon) this, 3928, -4, -1, 0, 0, -1, "tomb", 1);// 49
			AddComplexComponent( (BaseAddon) this, 7385, -4, -2, 5, 1, -1, "tomb", 1);// 50
			AddComplexComponent( (BaseAddon) this, 7385, -4, -1, 5, 1, -1, "tomb", 1);// 51
			AddComplexComponent( (BaseAddon) this, 7385, -4, 0, 5, 1, -1, "tomb", 1);// 52
			AddComplexComponent( (BaseAddon) this, 7385, -4, 1, 5, 1, -1, "tomb", 1);// 53
			AddComplexComponent( (BaseAddon) this, 7385, -3, -2, 5, 1, -1, "tomb", 1);// 54
			AddComplexComponent( (BaseAddon) this, 7385, -3, -1, 5, 1, -1, "tomb", 1);// 55
			AddComplexComponent( (BaseAddon) this, 7385, -3, 0, 5, 1, -1, "tomb", 1);// 56
			AddComplexComponent( (BaseAddon) this, 7385, -3, 1, 5, 1, -1, "tomb", 1);// 57
			AddComplexComponent( (BaseAddon) this, 7385, -2, -2, 5, 1, -1, "tomb", 1);// 58
			AddComplexComponent( (BaseAddon) this, 7385, -2, 1, 5, 1, -1, "tomb", 1);// 59
			AddComplexComponent( (BaseAddon) this, 7385, -1, -2, 5, 1, -1, "tomb", 1);// 60
			AddComplexComponent( (BaseAddon) this, 7385, -1, -1, 5, 1, -1, "tomb", 1);// 61
			AddComplexComponent( (BaseAddon) this, 7385, -1, 0, 5, 1, -1, "tomb", 1);// 62
			AddComplexComponent( (BaseAddon) this, 7385, -1, 1, 5, 1, -1, "tomb", 1);// 63
			AddComplexComponent( (BaseAddon) this, 7385, 0, -2, 5, 1, -1, "tomb", 1);// 64
			AddComplexComponent( (BaseAddon) this, 7385, 0, -1, 5, 1, -1, "tomb", 1);// 65
			AddComplexComponent( (BaseAddon) this, 7385, 0, 0, 5, 1, -1, "tomb", 1);// 66
			AddComplexComponent( (BaseAddon) this, 7385, 0, 1, 5, 1, -1, "tomb", 1);// 67
			AddComplexComponent( (BaseAddon) this, 7385, 1, -2, 5, 1, -1, "tomb", 1);// 68
			AddComplexComponent( (BaseAddon) this, 7385, 1, -1, 5, 1, -1, "tomb", 1);// 69
			AddComplexComponent( (BaseAddon) this, 7385, 1, 0, 5, 1, -1, "tomb", 1);// 70
			AddComplexComponent( (BaseAddon) this, 7385, 1, 1, 5, 1, -1, "tomb", 1);// 71
			AddComplexComponent( (BaseAddon) this, 7385, 2, -2, 5, 1, -1, "tomb", 1);// 72
			AddComplexComponent( (BaseAddon) this, 7385, 2, -1, 5, 1, -1, "tomb", 1);// 73
			AddComplexComponent( (BaseAddon) this, 7385, 2, 0, 5, 1, -1, "tomb", 1);// 74
			AddComplexComponent( (BaseAddon) this, 7385, 2, 1, 5, 1, -1, "tomb", 1);// 75
			AddComplexComponent( (BaseAddon) this, 7385, 3, -2, 5, 1, -1, "tomb", 1);// 76
			AddComplexComponent( (BaseAddon) this, 7385, 3, -1, 5, 1, -1, "tomb", 1);// 77
			AddComplexComponent( (BaseAddon) this, 7385, 3, 0, 5, 1, -1, "tomb", 1);// 78
			AddComplexComponent( (BaseAddon) this, 7385, 3, 1, 5, 1, -1, "tomb", 1);// 79
			AddComplexComponent( (BaseAddon) this, 7385, 4, -2, 5, 1, -1, "tomb", 1);// 80
			AddComplexComponent( (BaseAddon) this, 7385, 4, -1, 5, 1, -1, "tomb", 1);// 81
			AddComplexComponent( (BaseAddon) this, 7385, 4, 0, 5, 1, -1, "tomb", 1);// 82
			AddComplexComponent( (BaseAddon) this, 7385, 4, 1, 5, 1, -1, "tomb", 1);// 83
			AddComplexComponent( (BaseAddon) this, 1300, -4, -1, 0, 0, -1, "tomb", 1);// 84
			AddComplexComponent( (BaseAddon) this, 1300, -4, 0, 0, 0, -1, "tomb", 1);// 85
			AddComplexComponent( (BaseAddon) this, 1300, -4, 1, 0, 0, -1, "tomb", 1);// 86
			AddComplexComponent( (BaseAddon) this, 1300, -3, -1, 0, 0, -1, "tomb", 1);// 87
			AddComplexComponent( (BaseAddon) this, 1300, -3, 0, 0, 0, -1, "tomb", 1);// 88
			AddComplexComponent( (BaseAddon) this, 1300, -3, 1, 0, 0, -1, "tomb", 1);// 89
			AddComplexComponent( (BaseAddon) this, 1300, -2, -1, 0, 0, -1, "tomb", 1);// 90
			AddComplexComponent( (BaseAddon) this, 1300, -2, 0, 0, 0, -1, "tomb", 1);// 91
			AddComplexComponent( (BaseAddon) this, 1300, -2, 1, 0, 0, -1, "tomb", 1);// 92
			AddComplexComponent( (BaseAddon) this, 1300, -1, -1, 0, 0, -1, "tomb", 1);// 93
			AddComplexComponent( (BaseAddon) this, 1300, -1, 0, 0, 0, -1, "tomb", 1);// 94
			AddComplexComponent( (BaseAddon) this, 1300, -1, 1, 0, 0, -1, "tomb", 1);// 95
			AddComplexComponent( (BaseAddon) this, 1300, 0, -1, 0, 0, -1, "tomb", 1);// 96
			AddComplexComponent( (BaseAddon) this, 1300, 0, 0, 0, 0, -1, "tomb", 1);// 97
			AddComplexComponent( (BaseAddon) this, 1300, 0, 1, 0, 0, -1, "tomb", 1);// 98
			AddComplexComponent( (BaseAddon) this, 1300, 1, -1, 0, 0, -1, "tomb", 1);// 99
			AddComplexComponent( (BaseAddon) this, 1300, 1, 0, 0, 0, -1, "tomb", 1);// 100
			AddComplexComponent( (BaseAddon) this, 1300, 1, 1, 0, 0, -1, "tomb", 1);// 101
			AddComplexComponent( (BaseAddon) this, 1300, 2, -1, 0, 0, -1, "tomb", 1);// 102
			AddComplexComponent( (BaseAddon) this, 1300, 2, 0, 0, 0, -1, "tomb", 1);// 103
			AddComplexComponent( (BaseAddon) this, 1300, 2, 1, 0, 0, -1, "tomb", 1);// 104
			AddComplexComponent( (BaseAddon) this, 1300, 3, -1, 0, 0, -1, "tomb", 1);// 105
			AddComplexComponent( (BaseAddon) this, 1300, 3, 0, 0, 0, -1, "tomb", 1);// 106
			AddComplexComponent( (BaseAddon) this, 1300, 3, 1, 0, 0, -1, "tomb", 1);// 107
			AddComplexComponent( (BaseAddon) this, 1300, 4, -1, 0, 0, -1, "tomb", 1);// 108
			AddComplexComponent( (BaseAddon) this, 1300, 4, 0, 0, 0, -1, "tomb", 1);// 109
			AddComplexComponent( (BaseAddon) this, 1300, 4, 1, 0, 0, -1, "tomb", 1);// 110
			AddComplexComponent( (BaseAddon) this, 3799, -3, -1, 0, 0, -1, "tomb", 1);// 111
			AddComplexComponent( (BaseAddon) this, 7385, -2, -1, 5, 1, -1, "tomb", 1);// 112
			AddComplexComponent( (BaseAddon) this, 7385, -2, 0, 5, 1, -1, "tomb", 1);// 113
			AddComplexComponent( (BaseAddon) this, 2578, -1, -1, 2, 0, 1, "", 1);// 120
			AddComplexComponent( (BaseAddon) this, 5570, -4, 0, 5, 2341, -1, "", 1);// 124
			AddComplexComponent( (BaseAddon) this, 5570, 4, 0, 5, 2341, -1, "", 1);// 125
			AddComplexComponent( (BaseAddon) this, 4542, 4, -6, 2, 32, -1, "", 1);// 165
			AddComplexComponent( (BaseAddon) this, 4542, 4, -4, 2, 32, -1, "", 1);// 166
			AddComplexComponent( (BaseAddon) this, 4542, 4, -5, 2, 32, -1, "", 1);// 167
			AddComplexComponent( (BaseAddon) this, 4542, 4, -3, 2, 32, -1, "", 1);// 168
			AddComplexComponent( (BaseAddon) this, 4542, 4, 5, 0, 32, -1, "", 1);// 215
			AddComplexComponent( (BaseAddon) this, 4542, 4, 7, 0, 32, -1, "", 1);// 217

		}

		public tombAddon( Serial serial ) : base( serial )
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

	public class tombAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new tombAddon();
			}
		}

		[Constructable]
		public tombAddonDeed()
		{
			Name = "tomb";
		}

		public tombAddonDeed( Serial serial ) : base( serial )
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