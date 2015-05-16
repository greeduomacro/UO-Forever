

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class Fancy2pillowbedAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {19573, 1, 1, 0}, {19574, 1, -1, 0}, {19576, 0, 0, 0}// 1	2	3	
			, {19577, 0, 1, 0}, {19575, 0, -1, 0}, {19572, 1, 0, 0}// 4	5	6	
					};

 
            
		public override BaseAddonDeed Deed
		{
			get
			{
				return new Fancy2pillowbedAddonDeed();
			}
		}

		[ Constructable ]
		public Fancy2pillowbedAddon()
		{

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );


		}

		public Fancy2pillowbedAddon( Serial serial ) : base( serial )
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

	public class Fancy2pillowbedAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new Fancy2pillowbedAddon();
			}
		}

		[Constructable]
		public Fancy2pillowbedAddonDeed()
		{
			Name = "Fancy2pillowbed";
		}

		public Fancy2pillowbedAddonDeed( Serial serial ) : base( serial )
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