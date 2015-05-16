using Server.Multis;

namespace Server.Items
{
    public abstract class BaseShipItem : Item, IShareHue
    {
        #region Properties
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool ShareHue { get { return true; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set 
            {
                if (IsEmbarked && ShareHue)
                    base.Hue = Transport.Hue;
                else
                    base.Hue = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster, true)]
        public override BaseSmoothMulti Transport
        {
            get { return base.Transport as BaseShip; }
            set { if (value is BaseShip) base.Transport = value; }
        }
        #endregion

        protected BaseShipItem(BaseShip ship, int itemID)
            : this(ship, itemID, Point3D.Zero)
        {
        }

        protected BaseShipItem(BaseShip ship, int itemID, Point3D initOffset)
            : base(itemID)
        {
            Location = initOffset;
            Movable = false;
            ship.Embark(this);
        }

        public BaseShipItem(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (Transport != null && !Transport.Deleted)
                Transport.Delete();
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            if (Transport == null)
                Delete();
        }
        #endregion
    }

    public abstract class BaseShipContainer : Container, IShareHue
    {
        #region Properties
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool ShareHue { get { return true; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set
            {
                if (IsEmbarked && ShareHue)
                    base.Hue = Transport.Hue;
                else
                    base.Hue = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster, true)]
        public override BaseSmoothMulti Transport
        {
            get { return base.Transport as BaseShip; }
            set { if (value is BaseShip) base.Transport = value; }
        }
        #endregion

        protected BaseShipContainer(BaseShip ship, int itemID)
            : this(ship, itemID, Point3D.Zero)
        {
        }

        protected BaseShipContainer(BaseShip ship, int itemID, Point3D initOffset)
            : base(itemID)
        {
            Location = initOffset;
            Movable = false;
            ship.Embark(this);
        }

        public BaseShipContainer(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (Transport != null && !Transport.Deleted)
                Transport.Delete();
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            if (Transport == null)
                Delete();
        }
        #endregion
    }
}
