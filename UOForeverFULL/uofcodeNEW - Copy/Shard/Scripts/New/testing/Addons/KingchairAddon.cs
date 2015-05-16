
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class KingchairAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {3562, 1, 0, 0}, {3562, 1, 0, 120}, {3562, 1, 0, 6}// 1	 2	 3	 
			, {3562, 1, 0, 40}, {3562, 1, 0, 80}, {3660, 1, 0, 160}// 4	 5	 6	 
			, {3596, 1, 0, 180}, {3562, -1, -1, 120}, {3562, -1, -1, 6}// 7	 8	 9	 
			, {3562, -1, -1, 0}, {3562, -1, -1, 40}, {3562, -1, -1, 80}// 10	 11	 12	 
			, {3585, -1, -1, 180}, {3660, -1, -1, 160}, {3562, -1, 0, 120}// 13	 14	 15	 
			, {3562, -1, 0, 6}, {3562, -1, 0, 0}, {3562, -1, 0, 40}// 16	 17	 18	 
			, {3562, -1, 0, 80}, {3660, -1, 0, 160}, {3594, -1, 0, 180}// 19	 20	 21	 
			, {3562, 0, 0, 120}, {3562, 0, 0, 6}, {3562, 0, 0, 0}// 22	 23	 24	 
			, {3562, 0, 0, 40}, {3562, 0, 0, 80}, {3660, 0, 0, 160}// 25	 26	 27	 
			, {3595, 0, 0, 180}, {3562, 0, -1, 120}, {3562, 0, -1, 6}// 28	 29	 30	 
			, {3562, 0, -1, 0}, {3562, 0, -1, 40}, {3562, 0, -1, 80}// 31	 32	 33	 
			, {3586, 0, -1, 180}, {3660, 0, -1, 160}, {3562, 2, 0, 0}// 34	 35	 36	 
			, {3562, 2, 0, 120}, {3562, 2, 0, 6}, {3562, 2, 0, 40}// 37	 38	 39	 
			, {3562, 2, 0, 80}, {3660, 2, 0, 160}, {3597, 2, 0, 180}// 40	 41	 42	 
			, {3562, 2, -1, 0}, {3562, 2, -1, 120}, {3562, 2, -1, 6}// 43	 44	 45	 
			, {3562, 2, -1, 40}, {3562, 2, -1, 80}, {3588, 2, -1, 180}// 46	 47	 48	 
			, {3660, 2, -1, 160}, {3562, 2, 1, 0}, {3562, 2, 1, 120}// 49	 50	 51	 
			, {4400, 2, 1, 60}, {3375, 2, 1, 0}, {3562, 2, 1, 6}// 52	 53	 54	 
			, {3562, 2, 1, 40}, {3562, 2, 1, 80}, {3660, 2, 1, 160}// 55	 56	 57	 
			, {3606, 2, 1, 180}, {3562, 1, 1, 0}, {3562, 1, 1, 120}// 58	 59	 60	 
			, {4400, 1, 1, 60}, {3375, 1, 1, 0}, {3562, 1, 1, 6}// 61	 62	 63	 
			, {3562, 1, 1, 40}, {3562, 1, 1, 80}, {3660, 1, 1, 160}// 64	 65	 66	 
			, {3605, 1, 1, 180}, {3562, 0, 1, 120}, {4373, 0, 1, 40}// 67	 68	 69	 
			, {4400, 0, 1, 60}, {3375, 0, 1, 0}, {3562, 0, 1, 6}// 70	 71	 72	 
			, {3562, 0, 1, 0}, {3562, 0, 1, 40}, {3562, 0, 1, 80}// 73	 74	 75	 
			, {3660, 0, 1, 160}, {3604, 0, 1, 180}, {3562, -1, 1, 120}// 76	 77	 78	 
			, {4400, -1, 1, 60}, {3375, -1, 1, 0}, {4411, -1, 1, 40}// 79	 80	 81	 
			, {3562, -1, 1, 6}, {3562, -1, 1, 0}, {3562, -1, 1, 40}// 82	 83	 84	 
			, {3562, -1, 1, 80}, {3660, -1, 1, 160}, {3603, -1, 1, 180}// 85	 86	 87	 
			, {3562, 1, -1, 0}, {3562, 1, -1, 120}, {3562, 1, -1, 6}// 88	 89	 90	 
			, {3562, 1, -1, 40}, {3562, 1, -1, 80}, {3587, 1, -1, 180}// 91	 92	 93	 
			, {3660, 1, -1, 160}// 94	 
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new KingchairAddonDeed();
			}
		}

		[ Constructable ]
		public KingchairAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public KingchairAddon( Serial serial ) : base( serial )
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

	public class KingchairAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new KingchairAddon();
			}
		}

		[Constructable]
		public KingchairAddonDeed()
		{
			Name = "Kingchair";
		}

		public KingchairAddonDeed( Serial serial ) : base( serial )
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