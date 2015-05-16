using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
    public interface IRefreshItemID
    {
        void RefreshItemID(int mod);
    }

    public abstract class BaseGalleonItem : BaseShipItem, IRefreshItemID
    {
        private int m_BaseItemID;

        protected int BaseItemID { get { return m_BaseItemID; } }
        
        protected BaseGalleonItem(BaseGalleon galleon, int northItemID)
            : this(galleon, northItemID, Point3D.Zero)
        {
        }

        protected BaseGalleonItem(BaseGalleon galleon, int northItemID, Point3D initOffset)
            : base(galleon, northItemID, initOffset)
        {
            m_BaseItemID = northItemID;
        }

        public BaseGalleonItem(Serial serial)
            : base(serial)
        {
        }

        public virtual void RefreshItemID(int itemIDModifier)
        {
            SetItemIDOnSmooth(itemIDModifier + m_BaseItemID);
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)m_BaseItemID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
            m_BaseItemID = reader.ReadInt();
        }
        #endregion
    }

    public abstract class BaseGalleonContainer : BaseShipContainer, IRefreshItemID
    {
        private int m_BaseItemID;

        protected BaseGalleonContainer(BaseGalleon galleon, int northItemID, Point3D initOffset)
            : base(galleon, northItemID, initOffset)
        {
            m_BaseItemID = northItemID;
        }

        public BaseGalleonContainer(Serial serial)
            : base(serial)
        { }

        public virtual void RefreshItemID(int itemIDModifier)
        {
            SetItemIDOnSmooth(itemIDModifier + m_BaseItemID);
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)m_BaseItemID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
            m_BaseItemID = reader.ReadInt();
        }
        #endregion
    }

    public abstract class BaseGalleonMultiItem : BaseGalleonItem
    {
        private List<GalleonMultiComponent> m_Components;

        protected BaseGalleonMultiItem(BaseGalleon galleon, int northItemId, Point3D initOffset)
            : base(galleon, northItemId, initOffset)
        {
            m_Components = new List<GalleonMultiComponent>();
        }

        public BaseGalleonMultiItem(Serial serial)
            : base(serial)
        {
        }

        public void AddComponent(GalleonMultiComponent comp)
        {
            m_Components.Add(comp);
        }

        public override void RefreshItemID(int itemIDModifier)
        {
            base.RefreshItemID(itemIDModifier);
            m_Components.ForEach(comp => comp.RefreshItemID(itemIDModifier));
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            m_Components.ForEach(comp => comp.Delete());
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((int)m_Components.Count);
            foreach (GalleonMultiComponent comp in m_Components)
                writer.Write((Item)comp);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();

            m_Components = new List<GalleonMultiComponent>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
                m_Components.Add(reader.ReadItem() as GalleonMultiComponent);            
        }
        #endregion
    }

    public abstract class BaseGalleonMultiContainer : BaseGalleonContainer
    {
        private List<GalleonMultiComponent> m_Components;

        protected BaseGalleonMultiContainer(BaseGalleon galleon, int northItemId, Point3D initOffset)
            : base(galleon, northItemId, initOffset)
        {
            m_Components = new List<GalleonMultiComponent>();
        }

        public BaseGalleonMultiContainer(Serial serial)
            : base(serial)
        {
        }

        public void AddComponent(GalleonMultiComponent comp)
        {
            m_Components.Add(comp);
        }

        public override void RefreshItemID(int itemIDModifier)
        {
            base.RefreshItemID(itemIDModifier);
            foreach(GalleonMultiComponent component in m_Components)
                component.RefreshItemID(itemIDModifier);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            m_Components.ForEach(comp => comp.Delete());
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)m_Components.Count);
            m_Components.ForEach(comp => writer.Write((Item)comp));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt(); // version

            m_Components = new List<GalleonMultiComponent>();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
                m_Components.Add(reader.ReadItem() as GalleonMultiComponent);
        }
        #endregion
    }

    public class GalleonMultiComponent : BaseGalleonItem
    {
        private Item m_ParentItem;

        public GalleonMultiComponent(int northItemId, BaseGalleonMultiItem parent, Point3D initOffSet)
            : base(parent.Transport as BaseGalleon, northItemId)
        {
			Name = " ";
            m_ParentItem = parent;
            Location = new Point3D(parent.X + initOffSet.X, parent.Y + initOffSet.Y, parent.Z + initOffSet.Z);
        }

        public GalleonMultiComponent(int northItemId, BaseGalleonMultiContainer parent, Point3D initOffSet)
            : base(parent.Transport as BaseGalleon, northItemId)
        {
			Name = " ";
            m_ParentItem = parent;
            Location = new Point3D(parent.X + initOffSet.X, parent.Y + initOffSet.Y, parent.Z + initOffSet.Z);
        }

        public GalleonMultiComponent(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_ParentItem != null)
                m_ParentItem.OnDoubleClick(from);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_ParentItem != null)
                m_ParentItem.Delete();
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((Item)m_ParentItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_ParentItem = reader.ReadItem();

            if (m_ParentItem == null)
                Delete();
        }
        #endregion
    }
}
