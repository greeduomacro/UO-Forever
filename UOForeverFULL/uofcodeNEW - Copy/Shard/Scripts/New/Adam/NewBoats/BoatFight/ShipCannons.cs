using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Quests.Haven;
using Server.Engines.XmlSpawner2;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using VitaNex.FX;
using VitaNex.Network;

namespace Server.Items
{
    public class LightShipCannon : BaseShipWeapon, ICannon, IFacingChange
    {
	
		private SingleCannonPlace m_LinkedSingleCannonPlace;
		private MultiCannonPlace m_LinkedMultiCannonPlace;
		private bool m_SingleCannon;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace LinkedSingleCannonPlace { get { return m_LinkedSingleCannonPlace; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace LinkedMultiCannonPlace { get { return m_LinkedMultiCannonPlace; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool SingleCannon { get { return m_SingleCannon; } }
	
        // facing 0
        public static int[] CannonWest = new int[] { 0x4217, 0x4217, 0x4217 };
        public static int[] CannonWestXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonWestYOffset = new int[] { 0, 0, 0 };
        // facing 1
        public static int[] CannonNorth = new int[] { 0x4218, 0x4218, 0x4218 };
        public static int[] CannonNorthXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonNorthYOffset = new int[] { 0, 0, 0 };
        // facing 2
        public static int[] CannonEast = new int[] { 0x4219, 0x4219, 0x4219 };
        public static int[] CannonEastXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonEastYOffset = new int[] { 0, 0, 0 };
        // facing 3
        public static int[] CannonSouth = new int[] { 0x4216, 0x4216, 0x4216 };
        public static int[] CannonSouthXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonSouthYOffset = new int[] { 0, 0, 0 };
        private readonly Type[] m_allowedprojectiles = new Type[] { typeof(ShipCannonball) };
        
        public LightShipCannon(BaseGalleon galleon, int northItemID, Point3D initOffset, SingleCannonPlace targetedSingleCannonPlace, MultiCannonPlace targetedMultiCannonPlace)
            : this(0, galleon, northItemID, initOffset, targetedSingleCannonPlace, targetedMultiCannonPlace)
        {
        }

        public LightShipCannon(int facing, BaseGalleon galleon, int northItemID, Point3D initOffset, SingleCannonPlace targetedSingleCannonPlace, MultiCannonPlace targetedMultiCannonPlace)
			: base(galleon, northItemID, initOffset)
        {
		
			if (targetedSingleCannonPlace != null)
			{
				m_LinkedSingleCannonPlace = targetedSingleCannonPlace;
				targetedSingleCannonPlace.LinkedCannon = this;
				m_SingleCannon = true;
				m_LinkedMultiCannonPlace = null;
			}
			else if (targetedMultiCannonPlace != null)
			{
				m_LinkedSingleCannonPlace = null;
				targetedMultiCannonPlace.LinkedCannon = this;
				m_SingleCannon = false;
				m_LinkedMultiCannonPlace = targetedMultiCannonPlace;
			}
		
            // add the components
            /*for (int i = 0; i < CannonNorth.Length; i++)
            *{
            *    this.AddComponent(new ShipComponent(0, this.Name), 0, 0, 0);
            *}
			*/

            // assign the facing
            if (facing < 0)
                facing = 3;
            if (facing > 3)
                facing = 0;
            this.Facing = facing;

            // set the default props
            this.Name = "Light Ship Cannon";
            this.Weight = 50;


            // and draggable
            //XmlAttach.AttachTo(this, new XmlDrag());

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            this.Hue = 0;
        }

        public LightShipCannon(Serial serial)
            : base(serial)
        {
        }
		
		public void SetFacing(Direction oldFacing, Direction newFacing)
		{
		
			bool singleCannon = false;
			bool multiCannon = false;
			if (m_LinkedSingleCannonPlace != null)
				singleCannon = true;
			else if (m_LinkedMultiCannonPlace != null)
				multiCannon = true;
							
			if ((singleCannon == false) && (multiCannon == false))
				return;	
				
			if (singleCannon)
			{

				object objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
				SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;	

				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
				SingleCannonPlace CannonSupSx = objValue as SingleCannonPlace;	

				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
				SingleCannonPlace CannonSupDx = objValue as SingleCannonPlace;	
				
				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSx");
				SingleCannonPlace CannonMidSx = objValue as SingleCannonPlace;	

				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidDx");
				SingleCannonPlace CannonMidDx = objValue as SingleCannonPlace;

				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
				SingleCannonPlace CannonInfSx = objValue as SingleCannonPlace;	

				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
				SingleCannonPlace CannonInfDx = objValue as SingleCannonPlace;					
			
				if (newFacing == Direction.North)
				{
					if (m_LinkedSingleCannonPlace == CannonSupSx)
						Facing = 0;
					if (m_LinkedSingleCannonPlace == CannonSupDx)
						Facing = 2;
                    if (m_LinkedSingleCannonPlace == CannonMidSx)
						Facing = 0;
                    if (m_LinkedSingleCannonPlace == CannonInfSx)
                        Facing = 0;
					if (m_LinkedSingleCannonPlace == CannonInfDx)
						Facing = 2;
                    if (m_LinkedSingleCannonPlace == CannonMidDx)
                        Facing = 2;
					if (m_LinkedSingleCannonPlace == CannonCenter)
						Facing = 1;						
				}
				else if (newFacing == Direction.East)
				{
					if (m_LinkedSingleCannonPlace == CannonSupSx)
						Facing = 1;
					if (m_LinkedSingleCannonPlace == CannonSupDx)
						Facing = 3;
					if (m_LinkedSingleCannonPlace == CannonInfSx)
						Facing = 1;
                    if (m_LinkedSingleCannonPlace == CannonMidSx)
                        Facing = 1;
					if (m_LinkedSingleCannonPlace == CannonInfDx)
						Facing = 3;
                    if (m_LinkedSingleCannonPlace == CannonMidDx)
                        Facing = 3;
					if (m_LinkedSingleCannonPlace == CannonCenter)
						Facing = 2;							
				}
				else if (newFacing == Direction.South)
				{
					if (m_LinkedSingleCannonPlace == CannonSupSx)
						Facing = 2;
					if (m_LinkedSingleCannonPlace == CannonSupDx)
						Facing = 0;
					if (m_LinkedSingleCannonPlace == CannonInfSx)
						Facing = 2;
                    if (m_LinkedSingleCannonPlace == CannonMidSx)
                        Facing = 2;
					if (m_LinkedSingleCannonPlace == CannonInfDx)
						Facing = 0;
                    if (m_LinkedSingleCannonPlace == CannonMidDx)
                        Facing = 0;
					if (m_LinkedSingleCannonPlace == CannonCenter)
						Facing = 3;							
				}
				else if (newFacing == Direction.West)
				{
					if (m_LinkedSingleCannonPlace == CannonSupSx)
						Facing = 3;
					if (m_LinkedSingleCannonPlace == CannonSupDx)
						Facing = 1;
					if (m_LinkedSingleCannonPlace == CannonInfSx)
						Facing = 3;
                    if (m_LinkedSingleCannonPlace == CannonMidSx)
                        Facing = 3;
					if (m_LinkedSingleCannonPlace == CannonInfDx)
						Facing = 1;
                    if (m_LinkedSingleCannonPlace == CannonMidDx)
                        Facing = 1;
					if (m_LinkedSingleCannonPlace == CannonCenter)
						Facing = 0;					
				}
			}
			else if (multiCannon)
			{	
				object objValue = m_LinkedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
				MultiCannonPlace CannonSupSx = objValue as MultiCannonPlace;	

				objValue = m_LinkedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
				MultiCannonPlace CannonSupDx = objValue as MultiCannonPlace;	

				objValue = m_LinkedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
				MultiCannonPlace CannonInfSx = objValue as MultiCannonPlace;	

				objValue = m_LinkedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
				MultiCannonPlace CannonInfDx = objValue as MultiCannonPlace;	
				
			
				if (newFacing == Direction.North)
				{
					if (m_LinkedMultiCannonPlace == CannonSupSx)
						Facing = 0;
					if (m_LinkedMultiCannonPlace == CannonSupDx)
						Facing = 2;
					if (m_LinkedMultiCannonPlace == CannonInfSx)
						Facing = 0;
					if (m_LinkedMultiCannonPlace == CannonInfDx)
						Facing = 2;						
				}
				else if (newFacing == Direction.East)
				{
					if (m_LinkedMultiCannonPlace == CannonSupSx)
						Facing = 1;
					if (m_LinkedMultiCannonPlace == CannonSupDx)
						Facing = 3;
					if (m_LinkedMultiCannonPlace == CannonInfSx)
						Facing = 1;
					if (m_LinkedMultiCannonPlace == CannonInfDx)
						Facing = 3;							
				}
				else if (newFacing == Direction.South)
				{
					if (m_LinkedMultiCannonPlace == CannonSupSx)
						Facing = 2;
					if (m_LinkedMultiCannonPlace == CannonSupDx)
						Facing = 0;
					if (m_LinkedMultiCannonPlace == CannonInfSx)
						Facing = 2;
					if (m_LinkedMultiCannonPlace == CannonInfDx)
						Facing = 0;							
				}
				else if (newFacing == Direction.West)
				{
					if (m_LinkedMultiCannonPlace == CannonSupSx)
						Facing = 3;
					if (m_LinkedMultiCannonPlace == CannonSupDx)
						Facing = 1;
					if (m_LinkedMultiCannonPlace == CannonInfSx)
						Facing = 3;
					if (m_LinkedMultiCannonPlace == CannonInfDx)
						Facing = 1;							
				}
			}
		}

        public override double WeaponLoadingDelay
        {
            get
            {
                return 15;
            }
        }// base delay for loading this weapon
        public override double WeaponStorageDelay
        {
            get
            {
                return 15.0;
            }
        }// base delay for packing away this weapon
        public override double WeaponDamageFactor
        {
            get
            {
                return base.WeaponDamageFactor * 1.2;
            }
        }// damage multiplier for the weapon
        public override double WeaponRangeFactor
        {
            get
            {
                return base.WeaponRangeFactor * 1.2;
            }
        }//  range multiplier for the weapon
        public override int MinTargetRange
        {
            get
            {
                return 1;
            }
        }// target must be further away than this
        public override int MinStorageRange
        {
            get
            {
                return 2;
            }
        }// player must be at least this close to store the weapon
        public override int MinFiringRange
        {
            get
            {
                return 3;
            }
        }// player must be at least this close to fire the weapon
        public override bool CheckLOS
        {
            get
            {
                return true;
            }
        }// whether the weapon needs to consider line of sight when selecting a target
        public override Type[] AllowedProjectiles
        {
            get
            {
                return this.m_allowedprojectiles;
            }
        }
        public override Point3D ProjectileLaunchPoint
        {
            get
            {
                /*if (this.Components != null && this.Components.Count > 0)
                *{
                *    switch (this.Facing)
                *    {
                *        case 0:
                *            return new Point3D(CannonWestXOffset[0] + this.Location.X - 1, CannonWestYOffset[0] + this.Location.Y, this.Location.Z + 1);
                *        case 1:
                *            return new Point3D(CannonNorthXOffset[0] + this.Location.X - 1, CannonNorthYOffset[0] + this.Location.Y - 1, this.Location.Z + 1);
                *        case 2:
                *            return new Point3D(CannonEastXOffset[0] + this.Location.X, CannonEastYOffset[0] + this.Location.Y - 1, this.Location.Z + 1);
                *        case 3:
                *            return new Point3D(CannonSouthXOffset[0] + this.Location.X - 1, CannonSouthYOffset[0] + this.Location.Y, this.Location.Z + 1);
                *    }
                *}
				*/

                return (this.Location);
            }
        }

        public override void UpdateDisplay()
        {
            /*if (this.Components != null && this.Components.Count > 2)
            {*/
                //int z = ((AddonComponent)this.Components[1]).Location.Z;

                int[] itemid = null;
                int[] xoffset = null;
                int[] yoffset = null;
				int itemidmod = 0;

                switch (this.Facing)
                {
                    case 0: // West
                        itemid = CannonWest;
						itemidmod = -1;
                        xoffset = CannonWestXOffset;
                        yoffset = CannonWestYOffset;
                        break;
                    case 1: // North
                        itemid = CannonNorth;
                        xoffset = CannonNorthXOffset;
                        yoffset = CannonNorthYOffset;
                        break;
                    case 2: // East
                        itemid = CannonEast;
						itemidmod = 1;
                        xoffset = CannonEastXOffset;
                        yoffset = CannonEastYOffset;
                        break;
                    case 3: // South
                        itemid = CannonSouth;
						itemidmod = -2;
                        xoffset = CannonSouthXOffset;
                        yoffset = CannonSouthYOffset;
                        break;
                }

                if (itemid != null && xoffset != null && yoffset != null /*&& this.Components.Count == itemid.Length*/)
                {
                    /*for (int i = 0; i < this.Components.Count; i++)
                    {*/
						this.ItemID = itemid[0];
						this.RefreshItemID(itemidmod);
                        //((AddonComponent)this.Components[i]).ItemID = itemid[i];
                        //Point3D newoffset = new Point3D(xoffset[i], yoffset[i], 0);
                        //((AddonComponent)this.Components[i]).Offset = newoffset;
                        //((AddonComponent)this.Components[i]).Location = new Point3D(newoffset.X + this.X, newoffset.Y + this.Y, z);
                    //}
                }
            //}
        }
		
        public override void OnAfterDelete()
        {

        }
		
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
			base.GetContextMenuEntries(from, list);
            list.Add(new BackpackEntry(from, this));
        }
		
		public virtual void OnChop(Mobile from)
        {
			BaseGalleon galleon = null;
			if (this.LinkedSingleCannonPlace != null)
				galleon = this.LinkedSingleCannonPlace.Galleon;
			
			if (this.LinkedMultiCannonPlace != null)
				galleon = this.LinkedMultiCannonPlace.Galleon;

            if (galleon != null && (galleon.Owner == from) && galleon.Contains(this))
            {
                Effects.PlaySound(this.GetWorldLocation(), this.Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.
				
				this.Delete();

                LightCannonDeed deed = new LightCannonDeed();

                from.AddToBackpack(deed);
            }
        }

        public void DoFireEffect(IPoint3D target, Mobile from)
        {
            Map map = Map;

            if (target == null || map == null)
            {
                return;
            }

            //NextUse = DateTime.UtcNow + TimeSpan.FromMinutes(1);

            var startloc = new Point3D(Location);
            IPoint3D point = target;

            switch (Facing)
            {
                case 3:
                    {
                        Effects.SendLocationEffect(new Point3D(X, Y + 1, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                        break;
                    }
                case 1:
                    {
                        Effects.SendLocationEffect(new Point3D(X, Y - 1, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                        break;
                    }
                case 2:
                    {
                        Effects.SendLocationEffect(new Point3D(X + 1, Y, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                        break;
                    }
                case 0:
                    {
                        Effects.SendLocationEffect(new Point3D(X - 1, Y + 1, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                        break;
                    }
            }
            Effects.PlaySound(target, map, Utility.RandomList(0x11B, 0x11C, 0x11D));

            var queue = new EffectQueue();
            queue.Deferred = false;

            queue.Add(
                new MovingEffectInfo(
                    startloc,
                    point,
                    Map,
                    3699,
                    0,
                    10,
                    EffectRender.Normal,
                    TimeSpan.FromSeconds(0.1),
                    () =>
                    {
                        for (int count = 8; count > 0; count--)
                        {
                            IPoint3D location = new Point3D(target.X + Utility.RandomMinMax(-1, 1),
                                target.Y + Utility.RandomMinMax(-1, 1), target.Z);
                            int effect = Utility.RandomList(0x36B0);
                            Effects.SendLocationEffect(location, map, effect, 25, 1);
                            Effects.PlaySound(target, map, Utility.RandomList(0x11B, 0x11C, 0x11D));
                        }
                    }));
            queue.Process();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
			
			//version 1
			writer.Write(m_SingleCannon);
			
			if (m_SingleCannon)
				writer.Write(m_LinkedSingleCannonPlace);
			else
				writer.Write(m_LinkedMultiCannonPlace);
				
			
			
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch (version)
			{
				case 1 :
				{
					m_SingleCannon = reader.ReadBool();
					
					if (m_SingleCannon)
						m_LinkedSingleCannonPlace = reader.ReadItem() as SingleCannonPlace;
					else
						m_LinkedMultiCannonPlace = reader.ReadItem() as MultiCannonPlace;
						
					break;
				}
			}
        }
		
		private class BackpackEntry : ContextMenuEntry
		{
			private readonly LightShipCannon m_cannon;
			private readonly Mobile m_from;
			public BackpackEntry(Mobile from, LightShipCannon cannon)
				: base(2139)
			{
				this.m_cannon = cannon;
				this.m_from = from;
			}

			public override void OnClick()
			{
				if (this.m_cannon != null)
				{
					this.m_cannon.OnChop(this.m_from);
				}
			}
		}			
    }

    public class HeavyShipCannon : BaseShipWeapon, ICannon, IFacingChange
    {
	
		private SingleCannonPlace m_LinkedSingleCannonPlace;
		private MultiCannonPlace m_LinkedMultiCannonPlace;
		private bool m_SingleCannon;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace LinkedSingleCannonPlace { get { return m_LinkedSingleCannonPlace; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace LinkedMultiCannonPlace { get { return m_LinkedMultiCannonPlace; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool SingleCannon { get { return m_SingleCannon; } }
	
        // facing 0
        public static int[] CannonWest = new int[] { 0x421B, 0x421B, 0x421B };
        public static int[] CannonWestXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonWestYOffset = new int[] { 0, 0, 0 };
        // facing 1
        public static int[] CannonNorth = new int[] { 0x421C, 0x421C, 0x421C };
        public static int[] CannonNorthXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonNorthYOffset = new int[] { 0, 0, 0 };
        // facing 2
        public static int[] CannonEast = new int[] { 0x421D, 0x421D, 0x421D };
        public static int[] CannonEastXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonEastYOffset = new int[] { 0, 0, 0 };
        // facing 3
        public static int[] CannonSouth = new int[] { 0x421A, 0x421A, 0x421A };
        public static int[] CannonSouthXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonSouthYOffset = new int[] { 0, 0, 0 };
        private readonly Type[] m_allowedprojectiles = new Type[] { typeof(ShipCannonball) };
        
        public HeavyShipCannon(BaseGalleon galleon, int northItemID, Point3D initOffset, SingleCannonPlace targetedSingleCannonPlace, MultiCannonPlace targetedMultiCannonPlace)
            : this(0, galleon, northItemID, initOffset, targetedSingleCannonPlace, targetedMultiCannonPlace)
        {
        }

        public HeavyShipCannon(int facing, BaseGalleon galleon, int northItemID, Point3D initOffset, SingleCannonPlace targetedSingleCannonPlace, MultiCannonPlace targetedMultiCannonPlace)
			: base(galleon, northItemID, initOffset)
        {
		
			if (targetedSingleCannonPlace != null)
			{
				m_LinkedSingleCannonPlace = targetedSingleCannonPlace;
				targetedSingleCannonPlace.LinkedHeavyCannon = this;
				m_SingleCannon = true;
				m_LinkedMultiCannonPlace = null;
			}
			else if (targetedMultiCannonPlace != null)
			{
				m_LinkedSingleCannonPlace = null;
				targetedMultiCannonPlace.LinkedHeavyCannon = this;
				m_SingleCannon = false;
				m_LinkedMultiCannonPlace = targetedMultiCannonPlace;
			}
		
            // add the components
            /*for (int i = 0; i < CannonNorth.Length; i++)
            *{
            *    this.AddComponent(new ShipComponent(0, this.Name), 0, 0, 0);
            *}
			*/

            // assign the facing
            if (facing < 0)
                facing = 3;
            if (facing > 3)
                facing = 0;
            this.Facing = facing;

            // set the default props
            this.Name = "Heavy Ship Cannon";
            this.Weight = 50;

            this.Hue = 0;
        }

        public HeavyShipCannon(Serial serial)
            : base(serial)
        {
        }
		
		public void SetFacing(Direction oldFacing, Direction newFacing)
		{
		
			bool singleCannon = false;
			bool multiCannon = false;
			if (m_LinkedSingleCannonPlace != null)
				singleCannon = true;
			else if (m_LinkedMultiCannonPlace != null)
				multiCannon = true;
							
			if ((singleCannon == false) && (multiCannon == false))
				return;	
				
			if (singleCannon)
			{

				object objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
				SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;	

				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
				SingleCannonPlace CannonSupSx = objValue as SingleCannonPlace;	

				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
				SingleCannonPlace CannonSupDx = objValue as SingleCannonPlace;	
				
				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSx");
				SingleCannonPlace CannonMidSx = objValue as SingleCannonPlace;	

				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidDx");
				SingleCannonPlace CannonMidDx = objValue as SingleCannonPlace;

				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
				SingleCannonPlace CannonInfSx = objValue as SingleCannonPlace;	

				objValue = m_LinkedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
				SingleCannonPlace CannonInfDx = objValue as SingleCannonPlace;					
			
				if (newFacing == Direction.North)
				{
					if (m_LinkedSingleCannonPlace == CannonSupSx)
						Facing = 0;
					if (m_LinkedSingleCannonPlace == CannonSupDx)
						Facing = 2;
					if (m_LinkedSingleCannonPlace == CannonInfSx)
						Facing = 0;
					if (m_LinkedSingleCannonPlace == CannonInfDx)
						Facing = 2;
					if (m_LinkedSingleCannonPlace == CannonCenter)
						Facing = 1;						
				}
				else if (newFacing == Direction.East)
				{
					if (m_LinkedSingleCannonPlace == CannonSupSx)
						Facing = 1;
					if (m_LinkedSingleCannonPlace == CannonSupDx)
						Facing = 3;
					if (m_LinkedSingleCannonPlace == CannonInfSx)
						Facing = 1;
					if (m_LinkedSingleCannonPlace == CannonInfDx)
						Facing = 3;
					if (m_LinkedSingleCannonPlace == CannonCenter)
						Facing = 2;							
				}
				else if (newFacing == Direction.South)
				{
					if (m_LinkedSingleCannonPlace == CannonSupSx)
						Facing = 2;
					if (m_LinkedSingleCannonPlace == CannonSupDx)
						Facing = 0;
					if (m_LinkedSingleCannonPlace == CannonInfSx)
						Facing = 2;
					if (m_LinkedSingleCannonPlace == CannonInfDx)
						Facing = 0;
					if (m_LinkedSingleCannonPlace == CannonCenter)
						Facing = 3;							
				}
				else if (newFacing == Direction.West)
				{
					if (m_LinkedSingleCannonPlace == CannonSupSx)
						Facing = 3;
					if (m_LinkedSingleCannonPlace == CannonSupDx)
						Facing = 1;
					if (m_LinkedSingleCannonPlace == CannonInfSx)
						Facing = 3;
					if (m_LinkedSingleCannonPlace == CannonInfDx)
						Facing = 1;
					if (m_LinkedSingleCannonPlace == CannonCenter)
						Facing = 0;					
				}
			}
			else if (multiCannon)
			{	
				object objValue = m_LinkedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
				MultiCannonPlace CannonSupSx = objValue as MultiCannonPlace;	

				objValue = m_LinkedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
				MultiCannonPlace CannonSupDx = objValue as MultiCannonPlace;	

				objValue = m_LinkedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
				MultiCannonPlace CannonInfSx = objValue as MultiCannonPlace;	

				objValue = m_LinkedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
				MultiCannonPlace CannonInfDx = objValue as MultiCannonPlace;	
				
			
				if (newFacing == Direction.North)
				{
					if (m_LinkedMultiCannonPlace == CannonSupSx)
						Facing = 0;
					if (m_LinkedMultiCannonPlace == CannonSupDx)
						Facing = 2;
					if (m_LinkedMultiCannonPlace == CannonInfSx)
						Facing = 0;
					if (m_LinkedMultiCannonPlace == CannonInfDx)
						Facing = 2;						
				}
				else if (newFacing == Direction.East)
				{
					if (m_LinkedMultiCannonPlace == CannonSupSx)
						Facing = 1;
					if (m_LinkedMultiCannonPlace == CannonSupDx)
						Facing = 3;
					if (m_LinkedMultiCannonPlace == CannonInfSx)
						Facing = 1;
					if (m_LinkedMultiCannonPlace == CannonInfDx)
						Facing = 3;							
				}
				else if (newFacing == Direction.South)
				{
					if (m_LinkedMultiCannonPlace == CannonSupSx)
						Facing = 2;
					if (m_LinkedMultiCannonPlace == CannonSupDx)
						Facing = 0;
					if (m_LinkedMultiCannonPlace == CannonInfSx)
						Facing = 2;
					if (m_LinkedMultiCannonPlace == CannonInfDx)
						Facing = 0;							
				}
				else if (newFacing == Direction.West)
				{
					if (m_LinkedMultiCannonPlace == CannonSupSx)
						Facing = 3;
					if (m_LinkedMultiCannonPlace == CannonSupDx)
						Facing = 1;
					if (m_LinkedMultiCannonPlace == CannonInfSx)
						Facing = 3;
					if (m_LinkedMultiCannonPlace == CannonInfDx)
						Facing = 1;							
				}
			}
		}		

        public override double WeaponLoadingDelay
        {
            get
            {
                return 15;
            }
        }// base delay for loading this weapon
        public override double WeaponStorageDelay
        {
            get
            {
                return 15.0;
            }
        }// base delay for packing away this weapon
        public override double WeaponDamageFactor
        {
            get
            {
                return base.WeaponDamageFactor * 1.2;
            }
        }// damage multiplier for the weapon
        public override double WeaponRangeFactor
        {
            get
            {
                return base.WeaponRangeFactor * 1.2;
            }
        }//  range multiplier for the weapon
        public override int MinTargetRange
        {
            get
            {
                return 1;
            }
        }// target must be further away than this
        public override int MinStorageRange
        {
            get
            {
                return 2;
            }
        }// player must be at least this close to store the weapon
        public override int MinFiringRange
        {
            get
            {
                return 3;
            }
        }// player must be at least this close to fire the weapon
        public override bool CheckLOS
        {
            get
            {
                return true;
            }
        }// whether the weapon needs to consider line of sight when selecting a target
        public override Type[] AllowedProjectiles
        {
            get
            {
                return this.m_allowedprojectiles;
            }
        }
        public override Point3D ProjectileLaunchPoint
        {
            get
            {
                /*if (this.Components != null && this.Components.Count > 0)
                *{
                *    switch (this.Facing)
                *    {
                *        case 0:
                *            return new Point3D(CannonWestXOffset[0] + this.Location.X - 1, CannonWestYOffset[0] + this.Location.Y, this.Location.Z + 1);
                *        case 1:
                *            return new Point3D(CannonNorthXOffset[0] + this.Location.X - 1, CannonNorthYOffset[0] + this.Location.Y - 1, this.Location.Z + 1);
                *        case 2:
                *            return new Point3D(CannonEastXOffset[0] + this.Location.X, CannonEastYOffset[0] + this.Location.Y - 1, this.Location.Z + 1);
                *        case 3:
                *            return new Point3D(CannonSouthXOffset[0] + this.Location.X - 1, CannonSouthYOffset[0] + this.Location.Y, this.Location.Z + 1);
                *    }
                *}
				*/

                return (this.Location);
            }
        }


        public override void UpdateDisplay()
        {
            /*if (this.Components != null && this.Components.Count > 2)
            {*/
                //int z = ((AddonComponent)this.Components[1]).Location.Z;

                int[] itemid = null;
                int[] xoffset = null;
                int[] yoffset = null;
				int itemidmod = 0;

                switch (this.Facing)
                {
                    case 0: // West
                        itemid = CannonWest;
						itemidmod = -1;
                        xoffset = CannonWestXOffset;
                        yoffset = CannonWestYOffset;
                        break;
                    case 1: // North
                        itemid = CannonNorth;
                        xoffset = CannonNorthXOffset;
                        yoffset = CannonNorthYOffset;
                        break;
                    case 2: // East
                        itemid = CannonEast;
						itemidmod = 1;
                        xoffset = CannonEastXOffset;
                        yoffset = CannonEastYOffset;
                        break;
                    case 3: // South
                        itemid = CannonSouth;
						itemidmod = -2;
                        xoffset = CannonSouthXOffset;
                        yoffset = CannonSouthYOffset;
                        break;
                }

                if (itemid != null && xoffset != null && yoffset != null /*&& this.Components.Count == itemid.Length*/)
                {
                    /*for (int i = 0; i < this.Components.Count; i++)
                    {*/
						this.ItemID = itemid[0];
						this.RefreshItemID(itemidmod);
                        //((AddonComponent)this.Components[i]).ItemID = itemid[i];
                        //Point3D newoffset = new Point3D(xoffset[i], yoffset[i], 0);
                        //((AddonComponent)this.Components[i]).Offset = newoffset;
                        //((AddonComponent)this.Components[i]).Location = new Point3D(newoffset.X + this.X, newoffset.Y + this.Y, z);
                    //}
                }
            //}
        }
		
        public override void OnAfterDelete()
        {

        }		
		
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
			base.GetContextMenuEntries(from, list);
            list.Add(new BackpackEntry(from, this));
        }
		
		public virtual void OnChop(Mobile from)
        {
			BaseGalleon galleon = null;
			if (this.LinkedSingleCannonPlace != null)
				galleon = this.LinkedSingleCannonPlace.Galleon;
			
			if (this.LinkedMultiCannonPlace != null)
				galleon = this.LinkedMultiCannonPlace.Galleon;

            if (galleon != null && (galleon.Owner == from) && galleon.Contains(this))
            {
                Effects.PlaySound(this.GetWorldLocation(), this.Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.
				
				this.Delete();

                HeavyCannonDeed deed = new HeavyCannonDeed();

                from.AddToBackpack(deed);
            }
        }
		

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
			
			//version 1
			writer.Write(m_SingleCannon);
			
			if (m_SingleCannon)
				writer.Write(m_LinkedSingleCannonPlace);
			else
				writer.Write(m_LinkedMultiCannonPlace);
				
			
			
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch (version)
			{
				case 1 :
				{
					m_SingleCannon = reader.ReadBool();
					
					if (m_SingleCannon)
						m_LinkedSingleCannonPlace = reader.ReadItem() as SingleCannonPlace;
					else
						m_LinkedMultiCannonPlace = reader.ReadItem() as MultiCannonPlace;
						
					break;
				}
			}
        }
		
		private class BackpackEntry : ContextMenuEntry
		{
			private readonly HeavyShipCannon m_cannon;
			private readonly Mobile m_from;
			public BackpackEntry(Mobile from, HeavyShipCannon cannon)
				: base(2139)
			{
				this.m_cannon = cannon;
				this.m_from = from;
			}

			public override void OnClick()
			{
				if (this.m_cannon != null)
				{
					this.m_cannon.OnChop(this.m_from);
				}
			}
		}	

       public override void OnDoubleClick(Mobile from)
        {			
            if (this.Parent != null)
                return;

            // check the range between the player and weapon
            if (!from.InRange(this.Location, this.MinFiringRange) || from.Map != this.Map)
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (this.Storing)
            {
                from.SendMessage("{0} being stored", this.Name);
                return;
            }

            if (this.Projectile == null || this.Projectile.Deleted)
            {
                from.SendMessage("{0} empty", this.Name);
                return;
            }

            // check if the cannon is cool enough to fire
            if (this.NextFiring.Seconds > 0)
            {
                from.SendMessage("Not ready yet.");
                return;
            }
			
			//new way to fire
			Map map = Map;

			if ( map == null )
				return;

			int rx = 0, ry = 0;

			if ( Facing == 0 )
				rx = -1;
			else if ( Facing == 1 )
				ry = -1;
			else if ( Facing == 2 )
				rx = 1;
			else if ( Facing == 3 )
				ry = 1;

			for ( int i = 1; i <= 15; ++i )
			{
				int x = X + (i*rx);
				int y = Y + (i*ry);
				int z;

				for ( int j = -16; j <= 16; ++j )
				{
					z = from.Z + j;

					Point3D currentLocation = new Point3D( x, y, z );
					Item goldToken = new Gold(100);
					goldToken.Visible = false;
					goldToken.Map = Map;
					goldToken.Location = currentLocation;
					
					foreach (Item item in goldToken.GetItemsInRange(10))
						if (item is BaseShip)
						{
							BaseShip target = (BaseShip) item;
																	

							//Console.WriteLine("attacking {0} at {1}:{2}", multiitem, tileloc, ((StaticTarget)targeted).Location);
							// may have to reconsider the use tileloc vs target loc
							//m_cannon.AttackTarget(from, multiitem, ((StaticTarget)targeted).Location);
							if (target != this.Transport)
							{
								goldToken.Delete();
								return;
							}
							
						}

					goldToken.Delete();
						
				}

				z = map.GetAverageZ( x, y );


				Point3D currentLocation2 = new Point3D( x, y, z );
				Item goldToken2 = new Gold(100);
				goldToken2.Visible = false;
				goldToken2.Map = Map;
				goldToken2.Location = currentLocation2;
				foreach (Item item in goldToken2.GetItemsInRange(10))
					if (item is BaseShip)
					{
						BaseShip target = (BaseShip) item;
																

						//Console.WriteLine("attacking {0} at {1}:{2}", multiitem, tileloc, ((StaticTarget)targeted).Location);
						// may have to reconsider the use tileloc vs target loc
						//m_cannon.AttackTarget(from, multiitem, ((StaticTarget)targeted).Location);												
						if (target != this.Transport)
						{
							goldToken2.Delete();
							return;
						}						
					}	
					
				goldToken2.Delete();
			}			
        }				
    }	
}