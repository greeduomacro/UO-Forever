
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class FOreverChristmasAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {6089, -2, 4, 0}, {6078, -3, 1, 0}, {9003, 1, 2, 0}// 1	2	3	
			, {6090, -4, 2, 0}, {6082, -3, 3, 0}, {6087, 3, 4, 0}// 4	5	6	
			, {6078, 0, -1, 0}, {6085, -3, -2, 0}, {6078, 1, 2, 0}// 7	8	9	
			, {6078, 2, 3, 0}, {6092, 4, 1, 0}, {6090, -4, 0, 0}// 10	11	13	
			, {6091, -1, -2, 0}, {6092, 4, 2, 0}, {6086, 4, -1, 0}// 14	18	19	
			, {6078, 0, 3, 0}, {6088, -4, 3, 0}, {6091, 0, -2, 0}// 20	21	22	
			, {6078, 0, 1, 0}, {6078, 2, 1, 0}, {6078, -2, -1, 0}// 23	24	25	
			, {6078, -1, -1, 0}, {6078, 2, -1, 0}, {6091, -2, -2, 0}// 26	27	28	
			, {6078, -2, 2, 0}, {6078, -3, 2, 0}, {6078, 1, 1, 0}// 29	30	31	
			, {6078, 2, 2, 0}, {6078, 1, 0, 0}, {6085, -2, -2, 0}// 32	33	34	
			, {6085, -4, -1, 0}, {6078, -1, 1, 0}, {6078, 1, 3, 0}// 35	36	39	
			, {9079, -1, -3, 0}, {9079, 2, -3, 0}, {9054, 0, -3, 0}// 40	42	43	
			, {6090, -4, 1, 0}, {14186, 0, 1, 20}, {2602, 0, -2, 0}// 44	45	46	
			, {6078, -1, 0, 0}, {2602, 1, -2, 0}, {6089, -1, 4, 0}// 47	48	53	
			, {6078, -2, 3, 0}, {6078, -2, 0, 0}, {6091, 1, -2, 0}// 54	55	56	
			, {6078, 3, 2, 0}, {6089, 0, 4, 0}, {6078, 0, 0, 0}// 57	58	59	
			, {6092, 4, 0, 0}, {6078, 3, 0, 0}, {6083, -3, -1, 0}// 60	61	63	
			, {6091, 2, -2, 0}, {6084, 3, -1, 0}, {6078, 2, 0, 0}// 64	65	66	
			, {6078, 3, 1, 0}, {6088, -3, 4, 0}, {6086, 3, -2, 0}// 67	68	72	
			, {6078, -1, 3, 0}, {6078, 0, 2, 0}, {6081, 3, 3, 0}// 73	74	75	
			, {6078, -2, 1, 0}, {6089, 1, 4, 0}, {6078, -1, 2, 0}// 76	77	78	
			, {6078, -3, 0, 0}, {6089, 2, 4, 0}, {6087, 4, 3, 0}// 79	80	81	
			, {6077, 1, -1, 0}, {3287, 0, 1, 0}, {3286, 0, 1, 0}// 82	83	84	
			, {3874, 1, 2, 13}, {3864, 1, 2, 20}, {3872, 1, 2, 26}// 85	86	87	
			, {3863, 1, 2, 30}, {3876, 1, 2, 31}, {3871, 1, 2, 31}// 88	89	90	
			, {3865, 1, 2, 31}, {3867, 1, 2, 32}, {3887, 1, 2, 39}// 91	92	93	
			, {3875, 1, 2, 41}, {3882, 1, 2, 43}, {3888, 1, 2, 44}// 94	95	96	
			, {3881, 1, 2, 45}, {3862, 1, 3, 18}, {3870, 1, 3, 18}// 97	98	99	
			, {3855, 1, 3, 23}, {3859, 1, 3, 24}, {3858, 1, 3, 29}// 100	101	102	
			, {3861, 1, 3, 30}, {3880, 1, 3, 36}, {3866, 1, 3, 40}// 103	104	105	
			, {3883, 1, 3, 48}, {3856, 2, 2, 24}, {3868, 2, 2, 25}// 106	107	108	
			, {3860, 2, 2, 27}, {3878, 2, 2, 28}, {3879, 2, 2, 33}// 109	110	111	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new FOreverChristmasAddonDeed();
			}
		}

		[ Constructable ]
		public FOreverChristmasAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


			AddComplexComponent( (BaseAddon) this, 9003, -1, 1, 0, 593, -1, "", 1);// 12
			AddComplexComponent( (BaseAddon) this, 3631, 1, 1, 3, 0, 2, "a snowy scene of Vesper", 1);// 15
			AddComplexComponent( (BaseAddon) this, 9056, 1, -3, 0, 0, 1, "", 1);// 16
			AddComplexComponent( (BaseAddon) this, 9003, -1, 0, 0, 38, -1, "", 1);// 17
			AddComplexComponent( (BaseAddon) this, 9003, 0, 0, 0, 1359, -1, "", 1);// 37
			AddComplexComponent( (BaseAddon) this, 9002, 0, 2, 0, 77, -1, "", 1);// 38
			AddComplexComponent( (BaseAddon) this, 9000, 3, 2, 0, 769, -1, "", 1);// 41
			AddComplexComponent( (BaseAddon) this, 9003, 1, 1, 0, 4, -1, "", 1);// 49
			AddComplexComponent( (BaseAddon) this, 9003, 0, 2, 0, 88, -1, "", 1);// 50
			AddComplexComponent( (BaseAddon) this, 9003, -1, 2, 0, 38, -1, "", 1);// 51
			AddComplexComponent( (BaseAddon) this, 9003, 1, 2, 6, 193, -1, "", 1);// 52
			AddComplexComponent( (BaseAddon) this, 9003, -1, 1, 6, 108, -1, "", 1);// 62
			AddComplexComponent( (BaseAddon) this, 5703, 1, 2, 9, 0, 1, "", 1);// 69
			AddComplexComponent( (BaseAddon) this, 5703, 0, 1, 2, 0, 1, "", 1);// 70
			AddComplexComponent( (BaseAddon) this, 5703, 1, 1, 0, 0, 1, "", 1);// 71

		}

		public FOreverChristmasAddon( Serial serial ) : base( serial )
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

	public class FOreverChristmasAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new FOreverChristmasAddon();
			}
		}

		[Constructable]
		public FOreverChristmasAddonDeed()
		{
			Name = "FOreverChristmas";
		}

		public FOreverChristmasAddonDeed( Serial serial ) : base( serial )
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