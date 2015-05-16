using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Multis
{
    public abstract class BaseGalleon : BaseShip
    {
        protected abstract int[,] itemIDMods { get; }
        protected abstract int[] multiIDs { get; }

        private int m_CurrentItemIdModifier;
		private int m_CannonItemIdModifier;
		private int m_CannonItemIdModifierSx;
		private int m_CannonItemIdModifierDx;

        private List<BoatRope2> m_Ropes;
		

        #region Properties
        [CommandProperty(AccessLevel.GameMaster)]
        public List<BoatRope2> Ropes { get { return m_Ropes; } set { m_Ropes = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RopesCount { get { return m_Ropes.Count; } }		
		
		

        public int CurrentItemIdModifier { get { return m_CurrentItemIdModifier; } }
		
		public int CannonItemIdModifier { get { return m_CannonItemIdModifier; } }
		
		public int CannonItemIdModifierSx { get { return m_CannonItemIdModifierSx; } }
		
		public int CannonItemIdModifierDx { get { return m_CannonItemIdModifierDx; } }	
		
		
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual BaseDockedGalleon DockedGalleon { get { return null; } }
        #endregion
		
        protected BaseGalleon(int northMultiID)
            : base(northMultiID, 100, 100)
        {
            if (m_Ropes == null)
                m_Ropes = new List<BoatRope2>();			
			
        }

        public BaseGalleon(Serial serial)
            : base(serial)
        { }


        protected override void OnHullStatusChange()
        {
            ContainedObjects.ForEachItem(item =>
            {
                if (item is IRefreshItemID)
                    ((IRefreshItemID)item).RefreshItemID(m_CurrentItemIdModifier);
            });
        }

        protected override void UpdateContainedItemsFacing(Direction oldFacing, Direction newFacing, int count)
        {
            OnHullStatusChange();
            base.UpdateContainedItemsFacing(oldFacing, newFacing, count);
        }

        protected virtual int UpdateIDModifiers(ShipStatus status, Direction facing)
        {
            int intStatus = (int)status;
            int intFacing = (int)facing / 2;

            m_CurrentItemIdModifier = itemIDMods[intStatus, intFacing];
			
			switch (intFacing)
			{
				case 0: 
				{ 
					m_CannonItemIdModifier = 0; 
					m_CannonItemIdModifierSx = -1; 
					m_CannonItemIdModifierDx = 1; 					
					
					break;
				}
				case 1:
				{
					m_CannonItemIdModifier = 1; 
					m_CannonItemIdModifierSx = 0; 
					m_CannonItemIdModifierDx = -2; 					
					
					break;
				}
				case 2: 
				{
					m_CannonItemIdModifier = -2;
					m_CannonItemIdModifierSx = 1; 
					m_CannonItemIdModifierDx = -1; 		
					
					break;
				}
				case 3: 
				{
					m_CannonItemIdModifier = -1;
					m_CannonItemIdModifierSx = -2; 
					m_CannonItemIdModifierDx = 0; 					
					
					break;
				}
			}
			
            return multiIDs[intStatus] + intFacing;
        }

        protected override int GetMultiId(Direction newFacing)
        {
            return UpdateIDModifiers(Status, newFacing);
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
		
        public enum DryDockResult { Valid, Dead, NoKey, NotAnchored, Mobiles, Items, Hold, Decaying }

        public DryDockResult CheckDryDock(Mobile from)
        {
            if (CheckDecay())
                return DryDockResult.Decaying;

            if (!from.Alive)
                return DryDockResult.Dead;

            Container pack = from.Backpack;
            if ((m_Ropes == null || !Key.ContainsKey(pack, m_Ropes[0].KeyValue) && Status != ShipStatus.Low))
                return DryDockResult.NoKey;

            if (!Anchored)
                return DryDockResult.NotAnchored;

            if (Hold != null && Hold.Items.Count > 0)
                return DryDockResult.Hold;

            Map map = Map;

            if (map == null || map == Map.Internal)
                return DryDockResult.Items;

            List<Mobile> tomove = new List<Mobile>();

            MultiComponentList mcl = Components;

            IPooledEnumerable eable = map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (object o in eable)
            {
                if (o == this || o is BoatRope2 || o is TillerManHS || o is GalleonHold || o is SingleCannonPlace || o is SingleHelm || o is GalleonMultiComponent || o is MultiCannonPlace || o is MultiHelm || o is MainMast || o is BritainHull)
                    continue;

                if (o is Item && Contains((Item)o))
                {
                    eable.Free();
                    return DryDockResult.Items;
                }
                else if (o is Mobile && Contains((Mobile)o))
                {
                    var mobile = o as Mobile;
                    if (mobile.Alive)
                    {
                        eable.Free();
                        return DryDockResult.Mobiles;
                    }
                    else
                    {
                        mobile.SendMessage(61, "You have been ejected from the boat.");
                        tomove.Add(mobile);
                    }
                }
            }

            eable.Free();

            foreach (var mobile in tomove)
            {
                mobile.MoveToWorld(from.Location, from.Map);
            }
            return DryDockResult.Valid;
        }
		
        public void BeginDryDock(Mobile from)
        {
            if (CheckDecay())
                return;

            DryDockResult result = CheckDryDock(from);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.
            else if (result == DryDockResult.NoKey)
                from.SendLocalizedMessage(502494); // You must have a key to the ship to dock the boat.
            else if (result == DryDockResult.NotAnchored)
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the boat.
            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!
            else if (result == DryDockResult.Items)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.
            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!
            else if (result == DryDockResult.Valid)
                from.SendGump(new GalleonConfirmDryDockGump(from, this));
        }

        public void EndDryDock(Mobile from)
        {
            if (Deleted || CheckDecay())
                return;

            DryDockResult result = CheckDryDock(from);

            if (result == DryDockResult.Dead)
                from.SendLocalizedMessage(502493); // You appear to be dead.
            else if (result == DryDockResult.NoKey)
                from.SendLocalizedMessage(502494); // You must have a key to the ship to dock the boat.
            else if (result == DryDockResult.NotAnchored)
                from.SendLocalizedMessage(1010570); // You must lower the anchor to dock the boat.
            else if (result == DryDockResult.Mobiles)
                from.SendLocalizedMessage(502495); // You cannot dock the ship with beings on board!
            else if (result == DryDockResult.Items)
                from.SendLocalizedMessage(502496); // You cannot dock the ship with a cluttered deck.
            else if (result == DryDockResult.Hold)
                from.SendLocalizedMessage(502497); // Make sure your hold is empty, and try again!

            if (result != DryDockResult.Valid)
                return;

            BaseDockedGalleon boat = DockedGalleon;

            if (boat == null)
                return;

            RemoveKeys(from);

            from.AddToBackpack(boat);
			if (TillerManMobile != null)
				TillerManMobile.Delete();
            Delete();			
        }
		
		public void RemoveKeys(Mobile m)
        {
            uint keyValue = 0;

            if (this.m_Ropes != null)
                keyValue = this.m_Ropes[0].KeyValue;

            Key.RemoveKeys(m, keyValue);
        }
		
        public static BaseGalleon FindGalleonAt(IPoint2D loc, Map map)
        {
            Sector sector = map.GetSector(loc);

            for (int i = 0; i < sector.Multis.Count; i++)
            {
                BaseGalleon galleon = sector.Multis[i] as BaseGalleon;

                if (galleon != null && galleon.Contains(loc.X, loc.Y))
                    return galleon;
            }

            return null;
        }		
		
        public override void OnAfterDelete()
        {
            foreach (IEntity obj in ContainedObjects.ToList()) // toList necessary for enumeration modification
            {
                if (obj is Item)
                    ((Item)obj).Delete();
                else if (obj is ICrew)
                    ((ICrew)obj).Delete();
                else if (obj is Mobile)
                    ((Mobile)obj).Transport = null;
            }

            if (CurrentMoveTimer != null)
                CurrentMoveTimer.Stop();

            if (CurrentTurnTimer != null)
                CurrentTurnTimer.Stop();

			this.TillerManMobile.Delete();	
				
            Instances.Remove(this);
			
        }		
		

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{				
					
					int toread = reader.ReadInt();

                    m_Ropes = new List<BoatRope2>();
					for (int i = 0; i < toread; i++)
					{
                        BoatRope2 rope = reader.ReadItem() as BoatRope2;
						m_Ropes.Add(rope);		
					}
					goto case 0;
				}
				
				case 0:
				{
					ItemID = GetMultiId(Facing);
					break;
				}
			}
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
			
			//version 1
            writer.Write((int)m_Ropes.Count);
            for (int i = 0; i < m_Ropes.Count; i++)
			{
                BoatRope2 rope = (BoatRope2)m_Ropes[i];
				writer.Write((Item)rope);		
			}
        }
        #endregion
    }
}