//Customized By boba
using System;
using Server;
using Server.Items;

namespace Server.Items
{
              public class IceSandals: Sandals
{
	      public override int ArtifactRarity{ get{ return 6; } }

            
              [Constructable]
              public IceSandals()
{

                          Weight = 7;
                          Name = "Ice Sandals";
                          Hue = 1150;
              
                  }
              public IceSandals( Serial serial ) : base( serial )
                      {
                      }
              
              public override void Serialize( GenericWriter writer )
                      {
                          base.Serialize( writer );
                          writer.Write( (int) 0 );
                      }
              
              public override void Deserialize(GenericReader reader)
                      {
                          base.Deserialize( reader );
                          int version = reader.ReadInt();
                      }
                  }
              }
