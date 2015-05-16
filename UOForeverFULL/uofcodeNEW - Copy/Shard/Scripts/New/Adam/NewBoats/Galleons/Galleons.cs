using System;
using System.Collections.Generic;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{
    public class TokunoGalleon : BaseGalleon
    {
        #region Static Field
        private static int[] tokunoMultiIDs = new int[] { 0x38, 0x34, 0x30 }; // low, mid, full
        private static int[,] tokunoItemIDMods = new int[,] 
        {
            { 1330, 1195, 1470, 1610 }, // low
            { 732, 867, 462, 597 },     // mid
            { 0, 135, -270, -135 },     // full
        };

        protected override int[] multiIDs { get { return tokunoMultiIDs; } }
        protected override int[,] itemIDMods { get { return tokunoItemIDMods; } }
        #endregion

        private SingleCannonPlace m_CannonCenter;
        private SingleCannonPlace m_CannonSupSx;
        private SingleCannonPlace m_CannonSupDx;
        private SingleCannonPlace m_CannonInfSx;
        private SingleCannonPlace m_CannonInfDx;
        private IHelm m_Helm;
        private GalleonHold m_Hold;
        private BoatRope2 m_PRope, m_SRope;
		private TillerManHS m_TillerMan;	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedGalleon DockedGalleon { get { return new DockedTokunoGalleon(this); } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonCenter { get { return m_CannonCenter; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupSx { get { return m_CannonSupSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupDx { get { return m_CannonSupDx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfSx { get { return m_CannonInfSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfDx { get { return m_CannonInfDx; } }			
	
        [Constructable]
        public TokunoGalleon()
            : base(0x30)
        {
            m_CannonCenter = new SingleCannonPlace(this, 0x91CC, new Point3D(0, -9, 0));
            m_CannonSupSx = new SingleCannonPlace(this, 0x91AA, new Point3D(-2, -3, 0));
            m_CannonSupDx = new SingleCannonPlace(this, 0x91A6, new Point3D(2, -3, 0));
            m_CannonInfSx = new SingleCannonPlace(this, 0x9187, new Point3D(-2, 2, 0));
            m_CannonInfDx = new SingleCannonPlace(this, 0x9183, new Point3D(2, 2, 0));
            m_Helm = new SingleHelm(this, 0x9316, new Point3D(0, 7, 1));
            m_Hold = new GalleonHold(this, 0x9177, new Point3D(0, 4, 0),
                new List<Tuple<int, Point3D>>() 
                {
                    new Tuple<int, Point3D>(0x9170, new Point3D(0, 1, 0)),
                    new Tuple<int, Point3D>(0x9178, new Point3D(-1, 0, 0)),
                    new Tuple<int, Point3D>(0x9171, new Point3D(-1, 1, 0)),
                    new Tuple<int, Point3D>(0x9176, new Point3D(1, 0, 0)),
                    new Tuple<int, Point3D>(0x916F, new Point3D(1, 1, 0)),                    
                }, 16000);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-2, -2, 6), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(2, -2, 6), BoatRopeSide.Port, 0); 
			Ropes.Add(m_PRope);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-2, 3, 6), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(2, 3, 6), BoatRopeSide.Port, 0);
			Ropes.Add(m_PRope);
			m_TillerMan = new TillerManHS(this, 0, new Point3D(0, -2, 6) );
			Name = "a Tokuno Galleon";
			
            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone)

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            this.Hue = 0;			
			
        }

        public TokunoGalleon(Serial serial)
            : base(serial)
        {
        }
		
        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, -1, 7);
            }
        }

        protected override void OnHullStatusChange()
        {
            m_CannonCenter.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonCenter.LinkedCannon != null)
				m_CannonCenter.LinkedCannon.RefreshItemID(CannonItemIdModifier);
				
			if (m_CannonCenter.LinkedHeavyCannon != null)
				m_CannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
				
            m_CannonSupSx.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonSupSx.LinkedCannon != null)
				m_CannonSupSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
				
			if (m_CannonSupSx.LinkedHeavyCannon != null)
				m_CannonSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
			
            m_CannonSupDx.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonSupDx.LinkedCannon != null)
				m_CannonSupDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
				
			if (m_CannonSupDx.LinkedHeavyCannon != null)
				m_CannonSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
			
            m_CannonInfSx.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonInfSx.LinkedCannon != null)
				m_CannonInfSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
				
			if (m_CannonInfSx.LinkedHeavyCannon != null)
				m_CannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
			
            m_CannonInfDx.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonInfDx.LinkedCannon != null)
				m_CannonInfDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
				
			if (m_CannonInfDx.LinkedHeavyCannon != null)
				m_CannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
			
            m_Hold.RefreshItemID(CurrentItemIdModifier);

            int newModifier = 0;
            if (Status != ShipStatus.Low)
            {
                switch (Facing)
                {
                    //case Server.Direction.North: newModifier = 0; break;
                    case Server.Direction.East: newModifier = 2; break;
                    case Server.Direction.South: newModifier = -4; break;
                    case Server.Direction.West: newModifier = -2; break;
                }
            }
            else
            {
                switch (Facing)
                {
                    //case Server.Direction.North: newModifier = 0; break;
                    case Server.Direction.East: newModifier = -3; break;
                    case Server.Direction.South: newModifier = 2; break;
                    case Server.Direction.West: newModifier = 4; break;
                }
            }

            switch (Status)
            {
                case ShipStatus.Half: newModifier += 732; break;
                case ShipStatus.Low: newModifier += 1494; break;
            }

            m_Helm.RefreshItemID(newModifier);
        }
		

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_CannonCenter = reader.ReadItem() as SingleCannonPlace;
            m_CannonSupSx = reader.ReadItem() as SingleCannonPlace;
            m_CannonSupDx = reader.ReadItem() as SingleCannonPlace;
            m_CannonInfSx = reader.ReadItem() as SingleCannonPlace;
            m_CannonInfDx = reader.ReadItem() as SingleCannonPlace;
            m_Helm = reader.ReadItem() as SingleHelm;
            m_Hold = reader.ReadItem() as GalleonHold;		
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((Item)m_CannonCenter);
            writer.Write((Item)m_CannonSupSx);
            writer.Write((Item)m_CannonSupDx);
            writer.Write((Item)m_CannonInfSx);
            writer.Write((Item)m_CannonInfDx);
            writer.Write((Item)m_Helm);
            writer.Write((Item)m_Hold);
        }
        #endregion
    }
	
	public class TokunoGalleonDeed : BaseGalleonDeed
	{
        public override BaseGalleon Boat { get { return new TokunoGalleon(); } }

		[Constructable]
		public TokunoGalleonDeed() : base( 0x30, new Point3D( 0, -1, 0 ) )
		{
			Name = "Tokuno Galleon Deed";
		}

		public TokunoGalleonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}
	
	public class DockedTokunoGalleon : BaseDockedGalleon
	{
		public override BaseGalleon Galleon{ get{ return new TokunoGalleon(); } }

		public DockedTokunoGalleon( BaseGalleon boat ) : base( 0x30, Point3D.Zero, boat )
		{
		}

		public DockedTokunoGalleon( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}

    public class GargoyleGalleon : BaseGalleon
    {
        #region Static Field
        private static int[] gargoyleMultiIDs = new int[] { 0x2C, 0x28, 0x24 };
        private static int[,] gargoyleItemIDMods = new int[,] 
        {
            { 2076, 1092, 2378, 2680 },
            { 908, 604, -15005, 1512 },
            { 0, 303, -607, -302 },
        };

        protected override int[] multiIDs { get { return gargoyleMultiIDs; } }
        protected override int[,] itemIDMods { get { return gargoyleItemIDMods; } }
        #endregion

		
        private SingleCannonPlace m_CannonCenter;
        private SingleCannonPlace m_CannonSupSx;
        private SingleCannonPlace m_CannonSupDx;
        private SingleCannonPlace m_CannonMidSx;
        private SingleCannonPlace m_CannonMidDx;
        private SingleCannonPlace m_CannonInfSx;
        private SingleCannonPlace m_CannonInfDx;
        private IHelm m_Helm;
        private GalleonHold m_Hold;
        private BoatRope2 m_PRope, m_SRope;	
		private TillerManHS m_TillerMan;			

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedGalleon DockedGalleon { get { return new DockedGargoyleGalleon(this); } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonCenter { get { return m_CannonCenter; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupSx { get { return m_CannonSupSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupDx { get { return m_CannonSupDx; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidSx { get { return m_CannonMidSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidDx { get { return m_CannonMidDx; } }			

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfSx { get { return m_CannonInfSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfDx { get { return m_CannonInfDx; } }	
		
        [Constructable]
        public GargoyleGalleon()
            : base(0x24)
        {
            m_CannonCenter = new SingleCannonPlace(this, 0x8516, new Point3D(0, -8, 0));
            m_CannonSupSx = new SingleCannonPlace(this, 0x84FD, new Point3D(-2, -5, 0));
            m_CannonSupDx = new SingleCannonPlace(this, 0x84FF, new Point3D(2, -5, 0));
            m_CannonMidSx = new SingleCannonPlace(this, 0x8489, new Point3D(-2, -2, 0));
            m_CannonMidDx = new SingleCannonPlace(this, 0x848E, new Point3D(2, -2, 0));
            m_CannonInfSx = new SingleCannonPlace(this, 0x84AA, new Point3D(-2, 1, 0));
            m_CannonInfDx = new SingleCannonPlace(this, 0x84AC, new Point3D(2, 1, 0));
            m_Helm = new SingleHelm(this, 0x85A0, new Point3D(0, 2, 0));
            m_Hold = new GalleonHold(this, 0x84CA, new Point3D(0, 5, 0),
                new List<Tuple<int, Point3D>>() 
                {
                    new Tuple<int, Point3D>(0x84CC, new Point3D(-2, 0, 0)), // stiva lato sx 1/4
                    new Tuple<int, Point3D>(0x84D3, new Point3D(-2, 1, 0)), // stiva lato sx 2/4
                    new Tuple<int, Point3D>(0x84DA, new Point3D(-2, 2, 0)), // stiva lato sx 3/4
                    new Tuple<int, Point3D>(0x84E1, new Point3D(-2, 3, 0)), // stiva lato sx 4/4

                    new Tuple<int, Point3D>(0x84CB, new Point3D(-1, 0, 0)), // stiva sup sx       
                    new Tuple<int, Point3D>(0x84D2, new Point3D(-1, 1, 0)), // stiva Centro sx 1/2
                    new Tuple<int, Point3D>(0x84D9, new Point3D(-1, 2, 0)), // stiva Centro sx 2/2
                    new Tuple<int, Point3D>(0x84E0, new Point3D(-1, 3, 0)), // stiva inf sx
   
                    //new Tuple<int, Point3D>(0x84CA, new Point3D(0, 0, 0)),  // stiva sup ( container )
                    new Tuple<int, Point3D>(0x84D1, new Point3D(0, 1, 0)),  // stiva Centro 1/2
                    new Tuple<int, Point3D>(0x84D8, new Point3D(0, 2, 0)),  // stiva Centro 2/2 
                    new Tuple<int, Point3D>(0x84DF, new Point3D(0, 3, 0)),  // stiva inf

                    new Tuple<int, Point3D>(0x84D0, new Point3D(1, 0, 0)), // stiva sup dx 
                    new Tuple<int, Point3D>(0x84D7, new Point3D(1, 1, 0)), // stiva Centro dx 1/2
                    new Tuple<int, Point3D>(0x84DE, new Point3D(1, 2, 0)), // stiva Centro dx 2/2
                    new Tuple<int, Point3D>(0x84E5, new Point3D(1, 3, 0)), // stiva inf dx

                    new Tuple<int, Point3D>(0x84CE, new Point3D(2, 0, 0)), // stiva lato dx 1/4
                    new Tuple<int, Point3D>(0x84D5, new Point3D(2, 1, 0)), // stiva lato dx 2/4
                    new Tuple<int, Point3D>(0x84DC, new Point3D(2, 2, 0)), // stiva lato dx 3/4
                    new Tuple<int, Point3D>(0x84E3, new Point3D(2, 3, 0)), // stiva lato dx 4/4
                }, 12000);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-2, -4, 14), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(2, -4, 14), BoatRopeSide.Port, 0); 
			Ropes.Add(m_PRope);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-2, -1, 14), BoatRopeSide.Starboard, 0);
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(2, -1, 14), BoatRopeSide.Port, 0);
			Ropes.Add(m_PRope);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-2, 2, 14), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(2, 2, 14), BoatRopeSide.Port, 0);
			Ropes.Add(m_PRope);
			m_TillerMan = new TillerManHS(this, 0, new Point3D(0, -4, 12) );
			Name = "a Gargoyle Galleon";
			
            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone)

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            this.Hue = 0;			
			
        }

        public GargoyleGalleon(Serial serial)
            : base(serial)
        {
        }

        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, -2, 16);
            }
        }

        protected override void OnHullStatusChange()
        {
				
            m_CannonSupSx.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonSupSx.LinkedCannon != null)
				m_CannonSupSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
				
			if (m_CannonSupSx.LinkedHeavyCannon != null)
				m_CannonSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
			
            m_CannonSupDx.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonSupDx.LinkedCannon != null)
				m_CannonSupDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
				
			if (m_CannonSupDx.LinkedHeavyCannon != null)
				m_CannonSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);

            if (Status == ShipStatus.Low || (Status == ShipStatus.Half && (Facing == Server.Direction.South)))
            {
                m_CannonCenter.RefreshItemID(CurrentItemIdModifier - 6);
				if (m_CannonCenter.LinkedCannon != null)
					m_CannonCenter.LinkedCannon.RefreshItemID(CannonItemIdModifier);
					
				if (m_CannonCenter.LinkedHeavyCannon != null)
					m_CannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
			
                m_CannonMidSx.RefreshItemID(CurrentItemIdModifier + 32);
				if (m_CannonMidSx.LinkedCannon != null)
					m_CannonMidSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonMidSx.LinkedHeavyCannon != null)
					m_CannonMidSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                m_CannonMidDx.RefreshItemID(CurrentItemIdModifier + 29);
				if (m_CannonMidDx.LinkedCannon != null)
					m_CannonMidDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonMidDx.LinkedHeavyCannon != null)
					m_CannonMidDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_CannonInfSx.RefreshItemID(CurrentItemIdModifier + 20);
				if (m_CannonInfSx.LinkedCannon != null)
					m_CannonInfSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonInfSx.LinkedHeavyCannon != null)
					m_CannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                m_CannonInfDx.RefreshItemID(CurrentItemIdModifier + 20);
				if (m_CannonInfDx.LinkedCannon != null)
					m_CannonInfDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonInfDx.LinkedHeavyCannon != null)
					m_CannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_Hold.RefreshItemID(CurrentItemIdModifier + 14);
            }
            else
            {
                m_CannonCenter.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonCenter.LinkedCannon != null)
					m_CannonCenter.LinkedCannon.RefreshItemID(CannonItemIdModifier);
					
				if (m_CannonCenter.LinkedHeavyCannon != null)
					m_CannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
				
                m_CannonMidSx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonMidSx.LinkedCannon != null)
					m_CannonMidSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonMidSx.LinkedHeavyCannon != null)
					m_CannonMidSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                m_CannonMidDx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonMidDx.LinkedCannon != null)
					m_CannonMidDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonMidDx.LinkedHeavyCannon != null)
					m_CannonMidDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_CannonInfSx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonInfSx.LinkedCannon != null)
					m_CannonInfSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonInfSx.LinkedHeavyCannon != null)
					m_CannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                m_CannonInfDx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonInfDx.LinkedCannon != null)
					m_CannonInfDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonInfDx.LinkedHeavyCannon != null)
					m_CannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_Hold.RefreshItemID(CurrentItemIdModifier);
            }

            int newModifier = 0;
            switch (Status)
            {
                case ShipStatus.Full:
                    switch (Facing)
                    {
                        //case Server.Direction.North: newModifier = 0; break;
                        case Server.Direction.East: newModifier = 302; break;
                        case Server.Direction.South: newModifier = -604; break;
                        case Server.Direction.West: newModifier = -302; break;
                    }
                    break;
                case ShipStatus.Half:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 5004; break;
                        case Server.Direction.East: newModifier = 604; break;
                        case Server.Direction.South: newModifier = -15020; break;
                        case Server.Direction.West: newModifier = 1510; break;
                    }
                    break;
                case ShipStatus.Low:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 2114; break;
                        case Server.Direction.East: newModifier = 1812; break;
                        case Server.Direction.South: newModifier = 2415; break;
                        case Server.Direction.West: newModifier = 2715; break;
                    }
                    break;
            }

            m_Helm.RefreshItemID(newModifier);
        }

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_CannonCenter = reader.ReadItem() as SingleCannonPlace;
            m_CannonSupSx = reader.ReadItem() as SingleCannonPlace;
            m_CannonSupDx = reader.ReadItem() as SingleCannonPlace;
            m_CannonMidSx = reader.ReadItem() as SingleCannonPlace;
            m_CannonMidDx = reader.ReadItem() as SingleCannonPlace;
            m_CannonInfSx = reader.ReadItem() as SingleCannonPlace;
            m_CannonInfDx = reader.ReadItem() as SingleCannonPlace;
            m_Helm = reader.ReadItem() as SingleHelm;
            m_Hold = reader.ReadItem() as GalleonHold;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((Item)m_CannonCenter);
            writer.Write((Item)m_CannonSupSx);
            writer.Write((Item)m_CannonSupDx);
            writer.Write((Item)m_CannonMidSx);
            writer.Write((Item)m_CannonMidDx);
            writer.Write((Item)m_CannonInfSx);
            writer.Write((Item)m_CannonInfDx);
            writer.Write((Item)m_Helm);
            writer.Write((Item)m_Hold);
        }
        #endregion
    }
	
	public class GargoyleGalleonDeed : BaseGalleonDeed
	{
        public override BaseGalleon Boat { get { return new GargoyleGalleon(); } }

		[Constructable]
		public GargoyleGalleonDeed() : base( 0x2C, new Point3D( 0, -1, 0 ) )
		{
			Name = "Gargoyle Galleon Deed";
		}

		public GargoyleGalleonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}
	
	public class DockedGargoyleGalleon : BaseDockedGalleon
	{
		public override BaseGalleon Galleon{ get{ return new GargoyleGalleon(); } }

		public DockedGargoyleGalleon( BaseGalleon boat ) : base( 0x2C, Point3D.Zero, boat )
		{
		}

		public DockedGargoyleGalleon( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}

    public class OrcGalleon : BaseGalleon
    {
        #region Static Field
        private static int[] orcMultiIDs = new int[] { 0x20, 0x1C, 0x18, };
        private static int[,] orcItemIDMods = new int[,] 
        {
            { 1100, 1300, 700, 901 },
            { 1900, 2100, 1500, 1700 },
            { 0, 500, -1000, -500 },
        };

        protected override int[] multiIDs { get { return orcMultiIDs; } }
        protected override int[,] itemIDMods { get { return orcItemIDMods; } }
        #endregion

        private SingleCannonPlace m_CannonCenter;
        private MultiCannonPlace m_CannonSupSx;
        private MultiCannonPlace m_CannonSupDx;
        private SingleCannonPlace m_CannonMidSx;
        private SingleCannonPlace m_CannonMidDx;
        private MultiCannonPlace m_CannonInfSx;
        private MultiCannonPlace m_CannonInfDx;
        private IHelm m_Helm;
        private GalleonHold m_Hold;
        private BoatRope2 m_PRope, m_SRope;
		private TillerManHS m_TillerMan;

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedGalleon DockedGalleon { get { return new DockedOrcGalleon(this); } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonCenter { get { return m_CannonCenter; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace CannonSupSx { get { return m_CannonSupSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace CannonSupDx { get { return m_CannonSupDx; } }	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidSx { get { return m_CannonMidSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidDx { get { return m_CannonMidDx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace CannonInfSx { get { return m_CannonInfSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace CannonInfDx { get { return m_CannonInfDx; } }			
		
        [Constructable]
        public OrcGalleon()
            : base(0x18)
        {
            m_CannonCenter = new SingleCannonPlace(this, 0x7924, new Point3D(0, -6, 0));
            m_CannonSupSx = new MultiCannonPlace(this, 0x793D, new Point3D(-2, -2, 0), 0x7944, 0x7936);
            m_CannonSupDx = new MultiCannonPlace(this, 0x7941, new Point3D(2, -2, 0), 0x7948, 0x793A);
            m_CannonMidSx = new SingleCannonPlace(this, 0x7959, new Point3D(-2, 2, 0));
            m_CannonMidDx = new SingleCannonPlace(this, 0x795D, new Point3D(2, 2, 0));
            m_CannonInfSx = new MultiCannonPlace(this, 0x796E, new Point3D(-2, 5, 0), 0x7975);
            m_CannonInfDx = new MultiCannonPlace(this, 0x7972, new Point3D(2, 5, 0), 0x7979);
            m_Helm = new MultiHelm(this, 0x79A5, new Point3D(0, 7, 1), 0x79A4, 0x79A6);
            m_Hold = new GalleonHold(this, 0x798D, new Point3D(0, 9, 0),
                new List<Tuple<int, Point3D>>() 
                {
                    new Tuple<int, Point3D>(0x7994, new Point3D(0, 1, 0)),
                    new Tuple<int, Point3D>(0x798B, new Point3D(-1, 0, 0)),
                    new Tuple<int, Point3D>(0x7992, new Point3D(-1, 1, 0)),
                    new Tuple<int, Point3D>(0x7990, new Point3D(1, 0, 0)),
                    new Tuple<int, Point3D>(0x7997, new Point3D(1, 1, 0)),                    
                }, 14000);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-2, -1, 14), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(2, -1, 14), BoatRopeSide.Port, 0); 
			Ropes.Add(m_PRope);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-2, 3, 14), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(2, 3, 14), BoatRopeSide.Port, 0);
			Ropes.Add(m_PRope);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-2, 7, 14), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(2, 7, 14), BoatRopeSide.Port, 0);
			Ropes.Add(m_PRope);
			m_TillerMan = new TillerManHS(this, 0, new Point3D(0, 2, 10) );
			Name = "an Orc Galleon";

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            this.Hue = 0;	
        }

        public OrcGalleon(Serial serial)
            : base(serial)
        {
        }

        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 4, 14);
            }
        }

        protected override void OnHullStatusChange()
        {
            m_CannonSupSx.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonSupSx.LinkedCannon != null)
				m_CannonSupSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
				
			if (m_CannonSupSx.LinkedHeavyCannon != null)
				m_CannonSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
			
            m_CannonSupDx.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonSupDx.LinkedCannon != null)
				m_CannonSupDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
				
			if (m_CannonSupDx.LinkedHeavyCannon != null)
				m_CannonSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);


            if (Status == ShipStatus.Low || (Status == ShipStatus.Half && (Facing == Server.Direction.South)))
            {
                m_CannonCenter.RefreshItemID(CurrentItemIdModifier - 6);
				if (m_CannonCenter.LinkedCannon != null)
					m_CannonCenter.LinkedCannon.RefreshItemID(CannonItemIdModifier);
					
				if (m_CannonCenter.LinkedHeavyCannon != null)
					m_CannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
			
                m_CannonMidSx.RefreshItemID(CurrentItemIdModifier + 32);
				if (m_CannonMidSx.LinkedCannon != null)
					m_CannonMidSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonMidSx.LinkedHeavyCannon != null)
					m_CannonMidSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                m_CannonMidDx.RefreshItemID(CurrentItemIdModifier + 29);
				if (m_CannonMidDx.LinkedCannon != null)
					m_CannonMidDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonMidDx.LinkedHeavyCannon != null)
					m_CannonMidDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_CannonInfSx.RefreshItemID(CurrentItemIdModifier + 20);
				if (m_CannonInfSx.LinkedCannon != null)
					m_CannonInfSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonInfSx.LinkedHeavyCannon != null)
					m_CannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                m_CannonInfDx.RefreshItemID(CurrentItemIdModifier + 20);
				if (m_CannonInfDx.LinkedCannon != null)
					m_CannonInfDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonInfDx.LinkedHeavyCannon != null)
					m_CannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_Hold.RefreshItemID(CurrentItemIdModifier + 14);
            }
            else
            {
                m_CannonCenter.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonCenter.LinkedCannon != null)
					m_CannonCenter.LinkedCannon.RefreshItemID(CannonItemIdModifier);
					
				if (m_CannonCenter.LinkedHeavyCannon != null)
					m_CannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
				
                m_CannonMidSx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonMidSx.LinkedCannon != null)
					m_CannonMidSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonMidSx.LinkedHeavyCannon != null)
					m_CannonMidSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                m_CannonMidDx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonMidDx.LinkedCannon != null)
					m_CannonMidDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonMidDx.LinkedHeavyCannon != null)
					m_CannonMidDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_CannonInfSx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonInfSx.LinkedCannon != null)
					m_CannonInfSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonInfSx.LinkedHeavyCannon != null)
					m_CannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                m_CannonInfDx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonInfDx.LinkedCannon != null)
					m_CannonInfDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonInfDx.LinkedHeavyCannon != null)
					m_CannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_Hold.RefreshItemID(CurrentItemIdModifier);
            }

            int newModifier = 0;
            switch (Status)
            {
                case ShipStatus.Full:
                    switch (Facing)
                    {
                        //case Server.Direction.North: newModifier = 0; break;
                        case Server.Direction.East: newModifier = 501; break;
                        case Server.Direction.South: newModifier = -1000; break;
                        case Server.Direction.West: newModifier = -500; break;
                    }
                    break;
                case ShipStatus.Half:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 5004; break;
                        case Server.Direction.East: newModifier = 604; break;
                        case Server.Direction.South: newModifier = -15020; break;
                        case Server.Direction.West: newModifier = 1510; break;
                    }
                    break;
                case ShipStatus.Low:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 2114; break;
                        case Server.Direction.East: newModifier = 1812; break;
                        case Server.Direction.South: newModifier = 2415; break;
                        case Server.Direction.West: newModifier = 2715; break;
                    }
                    break;
            }

            m_Helm.RefreshItemID(newModifier);
        }		

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_CannonCenter = reader.ReadItem() as SingleCannonPlace;
            m_CannonSupSx = reader.ReadItem() as MultiCannonPlace;
            m_CannonSupDx = reader.ReadItem() as MultiCannonPlace;
            m_CannonMidSx = reader.ReadItem() as SingleCannonPlace;
            m_CannonMidDx = reader.ReadItem() as SingleCannonPlace;
            m_CannonInfSx = reader.ReadItem() as MultiCannonPlace;
            m_CannonInfDx = reader.ReadItem() as MultiCannonPlace;
            m_Helm = reader.ReadItem() as MultiHelm;
            m_Hold = reader.ReadItem() as GalleonHold;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((Item)m_CannonCenter);
            writer.Write((Item)m_CannonSupSx);
            writer.Write((Item)m_CannonSupDx);
            writer.Write((Item)m_CannonMidSx);
            writer.Write((Item)m_CannonMidDx);
            writer.Write((Item)m_CannonInfSx);
            writer.Write((Item)m_CannonInfDx);
            writer.Write((Item)m_Helm);
            writer.Write((Item)m_Hold);
        }
        #endregion
    }
	
	public class OrcGalleonDeed : BaseGalleonDeed
	{
        public override BaseGalleon Boat { get { return new OrcGalleon(); } }

		[Constructable]
		public OrcGalleonDeed() : base( 0x20, new Point3D( 0, -1, 0 ) )
		{
			Name = "Orc Galleon Deed";
		}

		public OrcGalleonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}
	
	public class DockedOrcGalleon : BaseDockedGalleon
	{
		public override BaseGalleon Galleon{ get{ return new OrcGalleon(); } }

		public DockedOrcGalleon( BaseGalleon boat ) : base( 0x20, Point3D.Zero, boat )
		{
		}

		public DockedOrcGalleon( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}

    public class BritainGalleon : BaseGalleon
    {
        #region Static Field
        private static int[] britainMultiIDs = new int[] { 0x44, 0x40, 0x40 };
        private static int[,] britainMultiIDsItemIDMods = new int[,] 
        {
            { 1536, 1482, 1590, 1644 },
            { 0, -54, 54, 108 },
            { 0, -54, 54, 108 },
        };

        protected override int[] multiIDs { get { return britainMultiIDs; } }
        protected override int[,] itemIDMods { get { return britainMultiIDsItemIDMods; } }
        #endregion

        private SingleCannonPlace m_CannonCenter;
        private SingleCannonPlace m_CannonSupSx;
        private SingleCannonPlace m_CannonSupDx;
        private SingleCannonPlace m_CannonMidSupSx;
        private SingleCannonPlace m_CannonMidSupDx;
        private SingleCannonPlace m_CannonMidInfSx;
        private SingleCannonPlace m_CannonMidInfDx;
        private SingleCannonPlace m_CannonInfSx;
        private SingleCannonPlace m_CannonInfDx;
        private IHelm m_Helm;
        private GalleonHold m_Hold;
        private MainMast m_MainMast;
        private BritainHull m_Hull;
        private BoatRope2 m_PRope, m_SRope;
		private TillerManHS m_TillerMan;

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedGalleon DockedGalleon { get { return new DockedBritainGalleon(this); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonCenter { get { return m_CannonCenter; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupSx { get { return m_CannonSupSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupDx { get { return m_CannonSupDx; } }	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidSupSx { get { return m_CannonMidSupSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidSupDx { get { return m_CannonMidSupDx; } }	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidInfSx { get { return m_CannonMidInfSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidInfDx { get { return m_CannonMidInfDx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfSx { get { return m_CannonInfSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfDx { get { return m_CannonInfDx; } }	
		
        [Constructable]
        public BritainGalleon()
            : base(0x40)
        {
            m_CannonCenter = new SingleCannonPlace(this, 0x5C06, new Point3D(0, -9, 0));
            m_CannonSupSx = new SingleCannonPlace(this, 0x5C19, new Point3D(-3, -5, 0));
            m_CannonSupDx = new SingleCannonPlace(this, 0x5C18, new Point3D(3, -5, 0));
            m_CannonMidSupSx = new SingleCannonPlace(this, 0x5C1A, new Point3D(-3, -1, 0));
            m_CannonMidSupDx = new SingleCannonPlace(this, 0x5C1C, new Point3D(3, -1, 0));
            m_CannonMidInfSx = new SingleCannonPlace(this, 0x5C21, new Point3D(-3, 3, 0));
            m_CannonMidInfDx = new SingleCannonPlace(this, 0x5C1F, new Point3D(3, 3, 0));
            m_CannonInfSx = new SingleCannonPlace(this, 0x5C25, new Point3D(-3, 7, 0));
            m_CannonInfDx = new SingleCannonPlace(this, 0x5C23, new Point3D(3, 7, 0));
            m_Helm = new SingleHelm(this, 0x5C0C, new Point3D(0, 3, 1));
            m_Hold = new GalleonHold(this, 0x5C2A, new Point3D(0, 9, 0),
                new List<Tuple<int, Point3D>>() 
                {
                    new Tuple<int, Point3D>(0x5C2C, new Point3D(-1, -1, 0)),
                    new Tuple<int, Point3D>(0x5C2F, new Point3D(-1, 0, 0)),
                    new Tuple<int, Point3D>(0x5C32, new Point3D(-1, 1, 0)),

                    new Tuple<int, Point3D>(0x5C2D, new Point3D(0, 0, 0)),
                    new Tuple<int, Point3D>(0x5C30, new Point3D(0, 1, 0)),   
                 
                    new Tuple<int, Point3D>(0x5C2B, new Point3D(1, -1, 0)),
                    new Tuple<int, Point3D>(0x5C2E, new Point3D(1, 0, 0)),
                    new Tuple<int, Point3D>(0x5C31, new Point3D(1, 1, 0)),
                }, 28000);

            m_MainMast = new MainMast(this, 0x5CE3, new Point3D(0, -3, 0));
            m_Hull = new BritainHull(this, 0x58A5);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-3, -4, 16), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(3, -4, 16), BoatRopeSide.Port, 0); 
			Ropes.Add(m_PRope);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-3, 0, 16), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(3, 0, 16), BoatRopeSide.Port, 0);
			Ropes.Add(m_PRope);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-3, 4, 16), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(3, 4, 16), BoatRopeSide.Port, 0);
			Ropes.Add(m_PRope);
            m_SRope = new BoatRope2(this, 0x14F8, new Point3D(-3, 8, 16), BoatRopeSide.Starboard, 0); 
			Ropes.Add(m_SRope);
            m_PRope = new BoatRope2(this, 0x14F8, new Point3D(3, 8, 16), BoatRopeSide.Port, 0);
			Ropes.Add(m_PRope);
			m_TillerMan = new TillerManHS(this, 0, new Point3D(0, 7, 12) );
			Name = "a Britain Galleon";
			
            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone)

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            this.Hue = 0;				
			
        }

        public BritainGalleon(Serial serial)
            : base(serial)
        {
        }

        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 4, 18);
            }
        }

        protected override void OnHullStatusChange()
        {
            m_CannonSupSx.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonSupSx.LinkedCannon != null)
				m_CannonSupSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
				
			if (m_CannonSupSx.LinkedHeavyCannon != null)
				m_CannonSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
			
            m_CannonSupDx.RefreshItemID(CurrentItemIdModifier);
			if (m_CannonSupDx.LinkedCannon != null)
				m_CannonSupDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
				
			if (m_CannonSupDx.LinkedHeavyCannon != null)
				m_CannonSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);

            if (Status == ShipStatus.Low || (Status == ShipStatus.Half && (Facing == Server.Direction.South)))
            {
                m_CannonCenter.RefreshItemID(CurrentItemIdModifier - 6);
				if (m_CannonCenter.LinkedCannon != null)
					m_CannonCenter.LinkedCannon.RefreshItemID(CannonItemIdModifier);
					
				if (m_CannonCenter.LinkedHeavyCannon != null)
					m_CannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
				
				m_CannonMidSupSx.RefreshItemID(CurrentItemIdModifier + 32);
				if (m_CannonMidSupSx.LinkedCannon != null)
					m_CannonMidSupSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonMidSupSx.LinkedHeavyCannon != null)
					m_CannonMidSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
				m_CannonMidSupDx.RefreshItemID(CurrentItemIdModifier + 32);
				if (m_CannonMidSupDx.LinkedCannon != null)
					m_CannonMidSupDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonMidSupDx.LinkedHeavyCannon != null)
					m_CannonMidSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
				m_CannonMidInfSx.RefreshItemID(CurrentItemIdModifier + 29);
				if (m_CannonMidInfSx.LinkedCannon != null)
					m_CannonMidInfSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonMidInfSx.LinkedHeavyCannon != null)
					m_CannonMidInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
				m_CannonMidInfDx.RefreshItemID(CurrentItemIdModifier + 29);
				if (m_CannonMidInfDx.LinkedCannon != null)
					m_CannonMidInfDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonMidInfDx.LinkedHeavyCannon != null)
					m_CannonMidInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_CannonInfSx.RefreshItemID(CurrentItemIdModifier + 20);
				if (m_CannonInfSx.LinkedCannon != null)
					m_CannonInfSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonInfSx.LinkedHeavyCannon != null)
					m_CannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                m_CannonInfDx.RefreshItemID(CurrentItemIdModifier + 20);
				if (m_CannonInfDx.LinkedCannon != null)
					m_CannonInfDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonInfDx.LinkedHeavyCannon != null)
					m_CannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_Hold.RefreshItemID(CurrentItemIdModifier + 14);
            }
            else
            {
                m_CannonCenter.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonCenter.LinkedCannon != null)
					m_CannonCenter.LinkedCannon.RefreshItemID(CannonItemIdModifier);
					
				if (m_CannonCenter.LinkedHeavyCannon != null)
					m_CannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
				
				m_CannonMidSupSx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonMidSupSx.LinkedCannon != null)
					m_CannonMidSupSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonMidSupSx.LinkedHeavyCannon != null)
					m_CannonMidSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
				m_CannonMidSupDx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonMidSupDx.LinkedCannon != null)
					m_CannonMidSupDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonMidSupDx.LinkedHeavyCannon != null)
					m_CannonMidSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
				m_CannonMidInfSx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonMidInfSx.LinkedCannon != null)
					m_CannonMidInfSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonMidInfSx.LinkedHeavyCannon != null)
					m_CannonMidInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
				m_CannonMidInfDx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonMidInfDx.LinkedCannon != null)
					m_CannonMidInfDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonMidInfDx.LinkedHeavyCannon != null)
					m_CannonMidInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_CannonInfSx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonInfSx.LinkedCannon != null)
					m_CannonInfSx.LinkedCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (m_CannonInfSx.LinkedHeavyCannon != null)
					m_CannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                m_CannonInfDx.RefreshItemID(CurrentItemIdModifier);
				if (m_CannonInfDx.LinkedCannon != null)
					m_CannonInfDx.LinkedCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (m_CannonInfDx.LinkedHeavyCannon != null)
					m_CannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                m_Hold.RefreshItemID(CurrentItemIdModifier);
            }

            int newModifier = 0;
            switch (Status)
            {
                case ShipStatus.Full:
                    switch (Facing)
                    {
                        //case Server.Direction.North: newModifier = 0; break;
                        case Server.Direction.East: newModifier = 54; break;
                        case Server.Direction.South: newModifier = 108; break;
                        case Server.Direction.West: newModifier = -54; break;
                    }
                    break;
                case ShipStatus.Half:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 5004; break;
                        case Server.Direction.East: newModifier = 604; break;
                        case Server.Direction.South: newModifier = -15020; break;
                        case Server.Direction.West: newModifier = 1510; break;
                    }
                    break;
                case ShipStatus.Low:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 2114; break;
                        case Server.Direction.East: newModifier = 1812; break;
                        case Server.Direction.South: newModifier = 2415; break;
                        case Server.Direction.West: newModifier = 2715; break;
                    }
                    break;
            }

            m_Helm.RefreshItemID(newModifier);
        }

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_CannonCenter = reader.ReadItem() as SingleCannonPlace;
            m_CannonSupSx = reader.ReadItem() as SingleCannonPlace;
            m_CannonSupDx = reader.ReadItem() as SingleCannonPlace;
            m_CannonMidSupSx = reader.ReadItem() as SingleCannonPlace;
            m_CannonMidSupDx = reader.ReadItem() as SingleCannonPlace;
            m_CannonMidInfSx = reader.ReadItem() as SingleCannonPlace;
            m_CannonMidInfDx = reader.ReadItem() as SingleCannonPlace;
            m_CannonInfSx = reader.ReadItem() as SingleCannonPlace;
            m_CannonInfDx = reader.ReadItem() as SingleCannonPlace;
            m_Helm = reader.ReadItem() as SingleHelm;
            m_Hold = reader.ReadItem() as GalleonHold;
            m_MainMast = reader.ReadItem() as MainMast;
            m_Hull = reader.ReadItem() as BritainHull;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((Item)m_CannonCenter);
            writer.Write((Item)m_CannonSupSx);
            writer.Write((Item)m_CannonSupDx);
            writer.Write((Item)m_CannonMidSupSx);
            writer.Write((Item)m_CannonMidSupDx);
            writer.Write((Item)m_CannonMidInfSx);
            writer.Write((Item)m_CannonMidInfDx);
            writer.Write((Item)m_CannonInfSx);
            writer.Write((Item)m_CannonInfDx);
            writer.Write((Item)m_Helm);
            writer.Write((Item)m_Hold);
            writer.Write((Item)m_MainMast);
            writer.Write((Item)m_Hull);
        }
        #endregion
    }
	
	public class BritainGalleonDeed : BaseGalleonDeed
	{
		public override BaseGalleon Boat{ get{ return new BritainGalleon(); } }

		[Constructable]
		public BritainGalleonDeed() : base( 0x44, new Point3D( 0, -1, 0 ) )
		{
			Name = "Britain Galleon Deed";
		}

		public BritainGalleonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}
	}
	
	public class DockedBritainGalleon : BaseDockedGalleon
	{
		public override BaseGalleon Galleon{ get{ return new BritainGalleon(); } }

		public DockedBritainGalleon( BaseGalleon boat ) : base( 0x44, Point3D.Zero, boat )
		{
		}

		public DockedBritainGalleon( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}
}
