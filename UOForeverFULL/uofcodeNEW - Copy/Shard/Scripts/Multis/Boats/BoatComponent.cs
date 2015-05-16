using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Movement;
using Server.Network;
using Server.Misc;

namespace Server.Multis
{
    public class BoatComponent : Item
    {        
        public static int[] ForeCannonItemIDs = new int[] { /*orc*/ 31012, /*tokuno*/ 37324, /*gargoyle*/ 0x8516 };
        public static int[] PortCannonItemIDs = new int[] { /*orc*/ 31093, 31065, 31037,  /*tokuno*/ 37255, 37290, /*gargoyle*/ 0x84fD, 0x8489, 0x84AA };
        public static int[] StarboardCannonItemIDs = new int[] { /*orc*/ 31041, 31069, 31097, /*tokuno*/ 37286, 37251, /*gargoyle*/ 0x84FF, 0x848E, 0x84AC };
        public static int[] AftCannonItemIDs = new int[] {  };

        public static int[] HoldItemIDs = new int[] { /*orc*/ 31117, 31115, 31120, 31124, 31122, 31127, /*tokuno*/ 37239, 37238, 37240, 37232, 37231, 37233, /*gargoyle*/ 34001, 34002, 34007, 34008, 34009, 34014, 34015, 34016, 34021, 34017 };
        public static int[] SteeringWheelIDs = new int[] { /*orc*/ 31141, 31140, 31142, /*tokuno*/ 37654, /*gargoyle*/ 34208 };
        public static int[] MastIDs = new int[] { /*orc*/ 31168, /*tokuno*/ 37579, /*gargoyle*/ 34165 };

        private BoatComponentType m_ComponentType = BoatComponentType.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public BoatComponentType ComponentType { get { return m_ComponentType; } set { m_ComponentType = value; } }


        public BoatComponentType GetBoatComponentType()
        {
            return GetBoatComponentType(DirectionalItemIDs[0, 0]);
        }

        public static BoatComponentType GetBoatComponentType(int baseItemID)
        {
            if (baseItemID == 0x14FA) { return BoatComponentType.Rope; }
            foreach (int cannonID in ForeCannonItemIDs) { if (cannonID == baseItemID) { return BoatComponentType.CannonSpot; } }
            foreach (int cannonID in PortCannonItemIDs) { if (cannonID == baseItemID) { return BoatComponentType.CannonSpot; } }
            foreach (int cannonID in StarboardCannonItemIDs) { if (cannonID == baseItemID) { return BoatComponentType.CannonSpot; } }
            foreach (int cannonID in AftCannonItemIDs) { if (cannonID == baseItemID) { return BoatComponentType.CannonSpot; } }
            
            foreach (int mastID in MastIDs) { if (mastID == baseItemID) { return BoatComponentType.Mast; } }

            foreach (int holdID in HoldItemIDs) { if (holdID == baseItemID) { return BoatComponentType.Hold; } }
            foreach (int steeringID in SteeringWheelIDs) { if (steeringID == baseItemID) { return BoatComponentType.SteeringWheel; } }
            return BoatComponentType.None;
        }

        public virtual void SetFacing(Direction dir)
        {
            if (m_Boat == null) { if (!this.Deleted) { Delete(); }; return; }
            int direction = 0;
            int damageLevel = 0;
            switch (dir)
            {
                case Direction.East: direction = 1; break;
                case Direction.South: direction = 2; break;
                case Direction.West: direction = 3; break;
            }
            if (m_Boat.Hits > BaseBoat.DamagedHits)
            {
                // don't change damagelevel
            }
            else if (m_Boat.Hits > BaseBoat.CriticalHits)
            {
                damageLevel = 1;
            }
            else
            {
                damageLevel = 2;
            }

            ItemID = m_DirectionalItemIDs[damageLevel, direction];
            return;
        }

        private Point3D m_Offset;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset { get { return m_Offset; } set { m_Offset = value; } }

        private int[,] m_DirectionalItemIDs = new int[3, 4]; // this is a 3 x 4 matrix, each row being a damage level, and each column being a direction, 0 = North, 1 = East, 2 = South, 3 = West
        [CommandProperty(AccessLevel.GameMaster)]
        public int[,] DirectionalItemIDs { get { return m_DirectionalItemIDs; } }

        private BaseBoat m_Boat;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat Boat { get { return m_Boat; } set { m_Boat = value; } }

        public BoatComponent(BaseBoat boat, Point3D offset, int[,] directionalItemIDs)
            : base()
        {
            m_Boat = boat;
            Offset = offset;
            Movable = false;
            if (directionalItemIDs != null) // it is null for ship cannons
            {
                m_DirectionalItemIDs = directionalItemIDs;
            }
            if (m_Boat != null)
            {
                Point3D loc = m_Boat.GetRotatedLocation(offset.X, offset.Y);
                loc.Z = m_Boat.Z + offset.Z;
                MoveToWorld(loc, m_Boat.Map);
                SetFacing(m_Boat.Direction);
            }
            m_ComponentType = GetBoatComponentType();
            if (m_ComponentType == BoatComponentType.SteeringWheel)
            {
                Name = "ship's wheel";
            }
            else if (m_ComponentType == BoatComponentType.CannonSpot)
            {
                Name = "cannon holder";
            }
            else if (m_ComponentType == BoatComponentType.Rope)
            {
                // don't need to change the name
            }
            /*
            else if (m_ComponentType == BoatComponentType.Mast)
            {
                Name = "a mast";
                if (UberScriptFileName != null)
                {
                    XmlScript script = new XmlScript(UberScriptFileName);
                    script.Name = "cannon";
                    XmlAttach.AttachTo(this, script);
                }
            }*/
            else if (m_ComponentType == BoatComponentType.Hold)
            {
                Name = "hold";
            }
        }

        public BoatComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            // version 1
            for (int i = 0; i < m_DirectionalItemIDs.GetLength(0); i++)
            {
                for (int j = 0; j < m_DirectionalItemIDs.GetLength(1); j++)
                {
                    writer.Write((int)m_DirectionalItemIDs[i, j]);
                }
            }
            writer.Write((byte)m_ComponentType);
            // version 0
            writer.Write(m_Boat);
            writer.Write(m_Offset);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    {
                        for (int i = 0; i < m_DirectionalItemIDs.GetLength(0); i++)
                        {
                            for (int j = 0; j < m_DirectionalItemIDs.GetLength(1); j++)
                            {
                                m_DirectionalItemIDs[i, j] = reader.ReadInt();
                            }
                        }
                        m_ComponentType = (BoatComponentType)reader.ReadByte();
                        goto case 0;
                    }

                case 0:
                    {
                        m_Boat = reader.ReadItem() as BaseBoat;
                        m_Offset = reader.ReadPoint3D();
                        break;
                    }
            }
            if (Boat == null)
            {
                Delete();
            }
        }

        public override void OnSingleClick(Mobile from)
        {
	        if (m_ComponentType == BoatComponentType.SteeringWheel)
	        {
		        if (m_Boat != null && m_Boat.ShipName != null)
				{
					LabelToExpansion(from);

			        if (from.NetState != null)
			        {
				        from.NetState.Send(
					        new UnicodeMessage(
								this.Serial,
								this.ItemID,
								MessageType.Label,
								0x3B2,
								3,
								from.Language,
								"",
								"ship's wheel of the " + m_Boat.ShipName));
			        }

			        LabelTo(from, m_Boat.Status); // the tiller man of the ~1_SHIP_NAME~
		        }
		        else
		        {
			        base.OnSingleClick(from);

			        if (m_Boat != null)
				        LabelTo(from, m_Boat.Status); // the tiller man of the ~1_SHIP_NAME~
		        }
	        }
	        else
			{
				LabelToExpansion(from);
			}
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_ComponentType == BoatComponentType.Hold)
            {
                if (Boat != null && Boat.Hold != null) { Boat.Hold.OnDoubleClick(from); }
            }
            else if (m_ComponentType == BoatComponentType.SteeringWheel)
            {
                if (Boat != null && Boat.TillerMan != null) { Boat.TillerMan.OnDoubleClick(from); }
            }
            base.OnDoubleClick(from);
        }
    }

    public class BoatHoldSpot : BoatComponent
    {
        public BoatHoldSpot(BaseBoat boat, Point3D offset, int[,] directionalItemIDs) : base(boat, offset, directionalItemIDs) { }

        public BoatHoldSpot(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public enum BoatComponentType : byte
    {
        Mast,
        Rope,
        Hold,
        CannonSpot,
        SteeringWheel,
        None,
        Unassigned // hasn't been double-clicked yet
    }
}
