
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class seatingAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {3609, 1, -1, 6}, {3605, 1, 0, 6}, {3606, 0, 1, 6}// 1	2	3	
			, {3379, -1, 2, 0}, {4002, 0, -1, 6}, {3379, 2, 0, 0}// 4	5	6	
			, {2602, 2, 0, 0}, {3378, 1, 2, 0}, {3379, -1, -1, 0}// 7	8	9	
			, {3378, 1, -1, 0}, {3379, 1, 1, 0}, {2602, -2, 0, 0}// 10	11	12	
			, {2478, 1, 1, 9}, {2602, 0, 2, 0}, {2602, 1, 2, 0}// 13	14	15	
			, {2602, -1, -1, 0}, {3378, -1, -2, 0}, {3379, -2, 0, 0}// 16	17	18	
			, {2524, 0, 0, 5}// 19	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new seatingAddonDeed();
			}
		}

		[ Constructable ]
		public seatingAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public seatingAddon( Serial serial ) : base( serial )
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

	public class seatingAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new seatingAddon();
			}
		}

		[Constructable]
		public seatingAddonDeed()
		{
			Name = "seating";
		}

		public seatingAddonDeed( Serial serial ) : base( serial )
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