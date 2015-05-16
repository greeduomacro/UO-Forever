//Customized By boba
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class IceSash : BodySash
  {
public override int ArtifactRarity{ get{ return 6; } }


      
      [Constructable]
		public IceSash()
		{
          Name = "Ice Sash";
          Hue = 1150;
		}

		public IceSash( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
