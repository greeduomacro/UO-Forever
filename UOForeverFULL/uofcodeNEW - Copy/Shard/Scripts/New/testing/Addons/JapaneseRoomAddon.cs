
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class JapaneseRoomAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {1179, 0, -2, 0}, {1179, 0, -1, 0}, {1179, 0, 0, 0}// 1	2	3	
			, {1179, 0, 1, 0}, {1179, -1, -2, 0}, {1179, -1, -1, 0}// 4	5	6	
			, {1179, -1, 0, 0}, {1179, -1, 1, 0}, {1179, -2, -2, 0}// 7	8	9	
			, {1179, -2, -1, 0}, {1179, -2, 0, 0}, {1179, -2, 1, 0}// 10	11	12	
			, {1260, 1, -2, 0}, {1260, 1, -1, 0}, {1260, 1, 0, 0}// 13	14	15	
			, {1260, 1, 1, 0}, {1258, 2, -2, 0}, {1258, 2, -1, 0}// 16	17	18	
			, {1258, 2, 0, 0}, {1258, 2, 1, 0}, {1262, 1, -3, 0}// 19	20	21	
			, {1263, 2, -3, 0}, {1179, -2, -3, 0}, {1179, -1, -3, 0}// 22	23	24	
			, {1179, 0, -3, 0}, {1801, -1, 0, 0}, {1801, -1, 1, 0}// 25	26	27	
			, {1801, -1, -1, 0}, {10321, 0, 1, 10}, {10374, -2, 1, 0}// 28	29	30	
			, {7630, 0, -1, 0}, {7631, 0, 0, 0}, {7629, 0, 1, 0}// 31	32	33	
			, {1179, 0, 2, 0}, {1179, -1, 2, 0}, {1179, -2, 2, 0}// 34	35	36	
			, {1260, 1, 2, 0}, {1258, 2, 2, 0}, {1264, 2, 3, 0}// 37	38	39	
			, {1265, 1, 3, 0}, {1179, 0, 3, 0}, {1179, -1, 3, 0}// 40	41	42	
			, {1179, -2, 3, 0}// 43	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new JapaneseRoomAddonDeed();
			}
		}

		[ Constructable ]
		public JapaneseRoomAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public JapaneseRoomAddon( Serial serial ) : base( serial )
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

	public class JapaneseRoomAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new JapaneseRoomAddon();
			}
		}

		[Constructable]
		public JapaneseRoomAddonDeed()
		{
			Name = "JapaneseRoom";
		}

		public JapaneseRoomAddonDeed( Serial serial ) : base( serial )
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