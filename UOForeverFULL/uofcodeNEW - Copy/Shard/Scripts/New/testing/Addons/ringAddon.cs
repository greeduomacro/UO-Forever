
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ringAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {128, -10, -9, 0}, {128, -10, -8, 0}, {128, -10, -7, 0}// 1	2	3	
			, {128, -10, -6, 0}, {128, -10, -5, 0}, {128, -10, -4, 0}// 4	5	6	
			, {128, -10, -3, 0}, {128, -10, -2, 0}, {128, -10, -1, 0}// 7	8	9	
			, {128, -10, 0, 0}, {128, -10, 1, 0}, {128, -10, 2, 0}// 10	11	12	
			, {128, -10, 3, 0}, {128, -9, -9, 0}, {128, -8, -9, 0}// 13	14	15	
			, {128, -7, -9, 0}, {128, -6, -9, 0}, {128, -10, 4, 0}// 16	17	18	
			, {128, -10, 5, 0}, {128, -10, 6, 0}, {128, -10, 7, 0}// 19	20	21	
			, {128, -10, 8, 0}, {128, -10, 9, 0}, {128, -9, 9, 0}// 22	23	24	
			, {128, -8, 9, 0}, {128, -7, 9, 0}, {128, -6, 9, 0}// 25	26	27	
			, {128, -5, -9, 0}, {128, -4, -9, 0}, {128, -3, -9, 0}// 28	29	30	
			, {128, -2, -9, 0}, {128, -1, -9, 0}, {128, 0, -9, 0}// 31	32	33	
			, {128, 1, -9, 0}, {128, 2, -9, 0}, {128, 3, -9, 0}// 34	35	36	
			, {128, 4, -9, 0}, {128, 5, -9, 0}, {128, 6, -9, 0}// 37	38	39	
			, {128, 7, -9, 0}, {128, 8, -9, 0}, {128, 9, -9, 0}// 40	41	42	
			, {128, 10, -9, 0}, {128, 10, -8, 0}, {128, 10, -7, 0}// 43	44	45	
			, {128, 10, -6, 0}, {128, 10, -5, 0}, {128, 10, -4, 0}// 46	47	48	
			, {128, 10, -3, 0}, {128, 10, -2, 0}, {128, 10, -1, 0}// 49	50	51	
			, {128, 10, 0, 0}, {128, 10, 1, 0}, {128, 10, 2, 0}// 52	53	54	
			, {128, 10, 3, 0}, {128, -5, 9, 0}, {128, -4, 9, 0}// 55	56	57	
			, {128, -3, 9, 0}, {128, -2, 9, 0}, {128, -1, 9, 0}// 58	59	60	
			, {128, 0, 9, 0}, {128, 1, 9, 0}, {128, 2, 9, 0}// 61	62	63	
			, {128, 3, 9, 0}, {128, 4, 9, 0}, {128, 5, 9, 0}// 64	65	66	
			, {128, 6, 9, 0}, {128, 7, 9, 0}, {128, 8, 9, 0}// 67	68	69	
			, {128, 9, 9, 0}, {128, 10, 4, 0}, {128, 10, 5, 0}// 70	71	72	
			, {128, 10, 6, 0}, {128, 10, 7, 0}, {128, 10, 8, 0}// 73	74	75	
			, {128, 10, 9, 0}// 76	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new ringAddonDeed();
			}
		}

		[ Constructable ]
		public ringAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public ringAddon( Serial serial ) : base( serial )
		{
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

	public class ringAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new ringAddon();
			}
		}

		[Constructable]
		public ringAddonDeed()
		{
			Name = "ring";
		}

		public ringAddonDeed( Serial serial ) : base( serial )
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