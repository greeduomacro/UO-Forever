using System;
using Server.Items;

namespace Server.Multis
{
    public abstract class NewBaseBoat : BaseShip
    {
        private NewTillerMan m_TillerMan;
        private NewPlank m_PPlank;
        private NewPlank m_SPlank;
        private NewHold m_Hold;

        protected abstract int NorthID { get; }

        #region Properties
        [CommandProperty(AccessLevel.GameMaster)]
        public NewTillerMan TillerMan { get { return m_TillerMan; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public NewPlank PPlank { get { return m_PPlank; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public NewPlank SPlank { get { return m_SPlank; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public NewHold NewHold { get { return m_Hold; } }

        #endregion

        protected NewBaseBoat(int ItemID, int maxhullDurability, int maxsailDurability, Point3D tillermanOffset, Point3D pPlankOffset, Point3D sPlankOffset, Point3D holdOffset)
            : base(ItemID, maxhullDurability, maxsailDurability)
        {

            m_TillerMan = new NewTillerMan(this, tillermanOffset);
            m_PPlank = new NewPlank(this, pPlankOffset, PlankSide.Port, 0);
            m_SPlank = new NewPlank(this, sPlankOffset, PlankSide.Starboard, 0);
            m_Hold = new NewHold(this, holdOffset);
        }

        public NewBaseBoat(Serial serial)
            : base(serial)
        { }
    
        protected override int GetMultiId(Direction newFacing)
        {
            return NorthID + ((int)newFacing / 2);
        }

        protected override bool IsEnabledLandID(int landID)
        {
            if (landID > 167 && landID < 172)
                return true;

            if (landID == 310 || landID == 311)
                return true;

            return false;
        }

        protected override bool IsEnabledStaticID(int staticID)
        {
            if (staticID > 0x1795 && staticID < 0x17B3)
                return true;

            return false;
        }

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_TillerMan = reader.ReadItem() as NewTillerMan;
            m_PPlank = reader.ReadItem() as NewPlank;
            m_SPlank = reader.ReadItem() as NewPlank;
            m_Hold = reader.ReadItem() as NewHold;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((Item)m_TillerMan);
            writer.Write((Item)m_PPlank);
            writer.Write((Item)m_SPlank);
            writer.Write((Item)m_Hold);
        }
        #endregion
    }
}