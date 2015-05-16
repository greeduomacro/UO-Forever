
////////////////////////////////////////
//                                     //
//     //
// Addon Generator  //
//          //
//             //
//                                     //
////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class TrashPileLargeAddonAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {7093, 0, 2, 0}, {7078, 0, 1, 0}, {7079, 0, 0, 0}// 1	2	3	
			, {7080, 0, -1, 0}, {7081, 0, -2, 0}, {7074, -1, 2, 0}// 4	5	6	
			, {7075, -1, 1, 0}, {7076, -1, 0, 0}, {7077, -1, -1, 0}// 7	8	9	
			, {7089, 1, 1, 0}, {7088, 1, 0, 0}, {7087, 1, -1, 0}// 10	11	12	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new TrashPileLargeAddonAddonDeed();
			}
		}

		[ Constructable ]
		public TrashPileLargeAddonAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public TrashPileLargeAddonAddon( Serial serial ) : base( serial )
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

	public class TrashPileLargeAddonAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new TrashPileLargeAddonAddon();
			}
		}

		[Constructable]
		public TrashPileLargeAddonAddonDeed()
		{
			Name = "large trash pile deed";
		}

		public TrashPileLargeAddonAddonDeed( Serial serial ) : base( serial )
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