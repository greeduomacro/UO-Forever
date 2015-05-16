
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class VineyardAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {3355, -3, 3, 0}, {3357, -3, -2, 0}, {3358, -3, -1, 0}// 5	6	7	
			, {3358, -3, 1, 0}, {3362, -3, -2, 0}, {12792, -3, -1, 0}// 8	9	10	
			, {12789, -3, 1, 0}, {6022, -3, -1, 0}, {6022, -3, 1, 0}// 11	12	13	
			, {3264, -3, 1, 0}, {3264, -3, -1, 0}, {10290, -3, 3, 4}// 14	15	16	
			, {3360, 3, -2, 0}, {3363, -1, -2, 0}, {3363, 1, -2, 0}// 51	52	53	
			, {12793, 1, -2, 0}, {12791, -1, -2, 0}, {6023, 1, -2, 0}// 54	55	56	
			, {6023, -1, -2, 0}, {2928, 0, 2, 0}, {2929, 1, 2, 0}// 57	58	59	
			, {2930, 1, 1, 0}, {2931, 0, 1, 0}, {4604, 1, 0, 0}// 60	61	62	
			, {4604, 0, 0, 0}, {4604, -1, 0, 0}, {4604, -1, 1, 0}// 63	64	65	
			, {4604, -1, 2, 0}, {2501, 1, 2, 10}, {2453, 0, 2, 6}// 66	67	68	
			, {2453, 0, 1, 6}, {2454, 1, 1, 6}, {2455, 1, 2, 3}// 69	70	71	
			, {3264, -1, -2, 0}, {3264, 1, -2, 0}, {2451, 2, -1, 0}// 72	73	74	
			, {2513, 2, -1, 2}, {2513, 2, -1, 0}// 75	76	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new VineyardAddonDeed();
			}
		}

		[ Constructable ]
		public VineyardAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 1231, -3, 0, 0, 1072, -1, "", 1);// 1
			AddComplexComponent( (BaseAddon) this, 1231, -3, 2, 0, 1072, -1, "", 1);// 2
			AddComplexComponent( (BaseAddon) this, 1232, -3, 3, 0, 1072, -1, "", 1);// 3
			AddComplexComponent( (BaseAddon) this, 1233, -3, -2, 0, 1072, -1, "", 1);// 4
			AddComplexComponent( (BaseAddon) this, 1228, -2, -2, 0, 1072, -1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 1228, 0, -2, 0, 1072, -1, "", 1);// 18
			AddComplexComponent( (BaseAddon) this, 1228, 2, -2, 0, 1072, -1, "", 1);// 19
			AddComplexComponent( (BaseAddon) this, 1226, -2, 3, 0, 1072, -1, "", 1);// 20
			AddComplexComponent( (BaseAddon) this, 1226, -1, 3, 0, 1072, -1, "", 1);// 21
			AddComplexComponent( (BaseAddon) this, 1226, 0, 3, 0, 1072, -1, "", 1);// 22
			AddComplexComponent( (BaseAddon) this, 1226, 1, 3, 0, 1072, -1, "", 1);// 23
			AddComplexComponent( (BaseAddon) this, 1226, 2, 3, 0, 1072, -1, "", 1);// 24
			AddComplexComponent( (BaseAddon) this, 1230, 3, 2, 0, 1072, -1, "", 1);// 25
			AddComplexComponent( (BaseAddon) this, 1230, 3, 1, 0, 1072, -1, "", 1);// 26
			AddComplexComponent( (BaseAddon) this, 1230, 3, 0, 0, 1072, -1, "", 1);// 27
			AddComplexComponent( (BaseAddon) this, 1230, 3, -1, 0, 1072, -1, "", 1);// 28
			AddComplexComponent( (BaseAddon) this, 1234, 3, 3, 0, 1072, -1, "", 1);// 29
			AddComplexComponent( (BaseAddon) this, 1235, 3, -2, 0, 1072, -1, "", 1);// 30
			AddComplexComponent( (BaseAddon) this, 1225, -2, -1, 0, 1072, -1, "", 1);// 31
			AddComplexComponent( (BaseAddon) this, 1225, -2, 0, 0, 1072, -1, "", 1);// 32
			AddComplexComponent( (BaseAddon) this, 1225, -2, 1, 0, 1072, -1, "", 1);// 33
			AddComplexComponent( (BaseAddon) this, 1225, -2, 2, 0, 1072, -1, "", 1);// 34
			AddComplexComponent( (BaseAddon) this, 1225, -1, 2, 0, 1072, -1, "", 1);// 35
			AddComplexComponent( (BaseAddon) this, 1225, -1, 1, 0, 1072, -1, "", 1);// 36
			AddComplexComponent( (BaseAddon) this, 1225, -1, 0, 0, 1072, -1, "", 1);// 37
			AddComplexComponent( (BaseAddon) this, 1225, -1, -1, 0, 1072, -1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 1225, 0, -1, 0, 1072, -1, "", 1);// 39
			AddComplexComponent( (BaseAddon) this, 1225, 0, 0, 0, 1072, -1, "", 1);// 40
			AddComplexComponent( (BaseAddon) this, 1225, 0, 1, 0, 1072, -1, "", 1);// 41
			AddComplexComponent( (BaseAddon) this, 1225, 0, 2, 0, 1072, -1, "", 1);// 42
			AddComplexComponent( (BaseAddon) this, 1225, 1, 2, 0, 1072, -1, "", 1);// 43
			AddComplexComponent( (BaseAddon) this, 1225, 2, 2, 0, 1072, -1, "", 1);// 44
			AddComplexComponent( (BaseAddon) this, 1225, 2, 1, 0, 1072, -1, "", 1);// 45
			AddComplexComponent( (BaseAddon) this, 1225, 1, 1, 0, 1072, -1, "", 1);// 46
			AddComplexComponent( (BaseAddon) this, 1225, 1, 0, 0, 1072, -1, "", 1);// 47
			AddComplexComponent( (BaseAddon) this, 1225, 1, -1, 0, 1072, -1, "", 1);// 48
			AddComplexComponent( (BaseAddon) this, 1225, 2, -1, 0, 1072, -1, "", 1);// 49
			AddComplexComponent( (BaseAddon) this, 1225, 2, 0, 0, 1072, -1, "", 1);// 50

		}

		public VineyardAddon( Serial serial ) : base( serial )
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

	public class VineyardAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new VineyardAddon();
			}
		}

		[Constructable]
		public VineyardAddonDeed()
		{
			Name = "Vineyard";
		}

		public VineyardAddonDeed( Serial serial ) : base( serial )
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