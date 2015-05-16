
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class GarishBedEastAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {16415, -1, 1, 0}, {16416, 0, 1, 0}, {16417, 1, 1, 0}// 1	2	3	
			, {16414, 1, 0, 0}, {16413, 0, 0, 0}, {16412, -1, 0, 0}// 4	5	6	
			, {16411, 1, -1, 0}, {16410, 0, -1, 0}, {16409, -1, -1, 0}// 7	8	9	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new GarishBedEastAddonDeed();
			}
		}

		[ Constructable ]
		public GarishBedEastAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public GarishBedEastAddon( Serial serial ) : base( serial )
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

	public class GarishBedEastAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new GarishBedEastAddon();
			}
		}

		[Constructable]
		public GarishBedEastAddonDeed()
		{
			Name = "GarishBedEast";
            LootType = LootType.Blessed;
		}

		public GarishBedEastAddonDeed( Serial serial ) : base( serial )
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