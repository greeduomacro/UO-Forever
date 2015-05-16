
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class garishbedAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {16401, 0, -1, 0}, {16402, 1, -1, 0}, {16400, -1, -1, 0}// 1	2	3	
			, {16407, 0, 1, 0}, {16408, 1, 1, 0}, {16404, 0, 0, 0}// 4	5	6	
			, {16406, -1, 1, 0}, {16405, 1, 0, 0}, {16403, -1, 0, 0}// 7	8	9	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new garishbedAddonDeed();
			}
		}

		[ Constructable ]
		public garishbedAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public garishbedAddon( Serial serial ) : base( serial )
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

	public class garishbedAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new garishbedAddon();
			}
		}

		[Constructable]
		public garishbedAddonDeed()
		{
			Name = "garishbed";
		}

		public garishbedAddonDeed( Serial serial ) : base( serial )
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