
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
	public class TrashPileMediumAddonAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {7093, 0, 1, 0}, {7094, 0, 0, 0}, {7095, 0, -1, 0}// 1	2	3	
			, {7100, 1, 0, 0}, {7099, 1, -1, 0}// 4	5	
		};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new TrashPileMediumAddonAddonDeed();
			}
		}

		[ Constructable ]
		public TrashPileMediumAddonAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public TrashPileMediumAddonAddon( Serial serial ) : base( serial )
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

	public class TrashPileMediumAddonAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new TrashPileMediumAddonAddon();
			}
		}

		[Constructable]
		public TrashPileMediumAddonAddonDeed()
		{
			Name = "medium trash pile deed";
		}

		public TrashPileMediumAddonAddonDeed( Serial serial ) : base( serial )
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