using System;
using Server;
using Server.Network;

namespace Server.Items
{
	[Flipable( 0x2A69, 0x2A6D )]
	public class CreepyPortraitArtifact : Item
	{
		public override int LabelNumber { get { return 1074481; } } // Creepy portrait
		public override bool HandlesOnMovement { get { return true; } }

        [Constructable]
		public CreepyPortraitArtifact() : base( 0x2A69 )
		{
		    Name = "creepy portrait";
		}

        public CreepyPortraitArtifact(Serial serial)
            : base(serial)
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Utility.InRange( Location, from.Location, 2 ) )
				Effects.PlaySound( Location, Map, Utility.RandomMinMax( 0x565, 0x566 ) );
			else
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
		}

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);
            LabelTo(m,
                "[Champion Artifact]",
                134);
        }

		public override void OnMovement( Mobile m, Point3D old )
		{
			if ( m.Alive && m.Player && ( m.AccessLevel == AccessLevel.Player || !m.Hidden ) )
			{
				if ( !Utility.InRange( old, Location, 2 ) && Utility.InRange( m.Location, Location, 2 ) )
				{
					if ( ItemID == 0x2A69 || ItemID == 0x2A6D )
					{
						Up();
						Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds( 0.5 ), 2, new TimerCallback( Up ) );
					}
				}
				else if ( Utility.InRange( old, Location, 2 ) && !Utility.InRange( m.Location, Location, 2 ) )
				{
					if ( ItemID == 0x2A6C || ItemID == 0x2A70 )
					{
						Down();
						Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds( 0.5 ), 2, new TimerCallback( Down ) );
					}
				}
			}
		}

		private void Up()
		{
			ItemID++;
		}

		private void Down()
		{
			ItemID--;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			if ( version == 0 && ItemID != 0x2A69 && ItemID != 0x2A6D )
				ItemID = 0x2A69;
		}
	}
}