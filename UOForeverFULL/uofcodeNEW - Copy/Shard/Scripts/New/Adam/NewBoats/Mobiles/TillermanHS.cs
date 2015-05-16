using System;
using System.Collections;
using System.Collections.Generic;

using Server;
using Server.ContextMenus;
using Server.Items;
using Server.Movement;
using Server.Network;
using Server.Multis;
using Server.Gumps;

namespace Server.Mobiles
{
    public class TillerManHS : Mobile
    {
        private BaseGalleon m_Galleon;
		private uint m_KeyValue;

        public TillerManHS(BaseGalleon galleon, uint keyValue, Point3D initOffset)
            : base()
        {
			Location = initOffset;
			galleon.Embark(this);
			galleon.TillerManMobile = this;
            m_Galleon = galleon;
			m_KeyValue = keyValue;
            CantWalk = true;
			Blessed = true;

            InitStats(31, 41, 51);

            SpeechHue = Utility.RandomDyedHue();

            Hue = Utility.RandomSkinHue();


            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
                AddItem(new Kilt(Utility.RandomDyedHue()));
                AddItem(new Shirt(Utility.RandomDyedHue()));
                AddItem(new ThighBoots());
                Title = "the tillerman";
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
                AddItem(new ShortPants(Utility.RandomNeutralHue()));
                AddItem(new Shirt(Utility.RandomDyedHue()));
                AddItem(new Sandals());
                Title = "the tillerman";
            }

            AddItem(new Bandana(Utility.RandomDyedHue()));

            Utility.AssignRandomHair(this);

            Container pack = new Backpack();

            pack.DropItem(new Gold(250, 300));

            pack.Movable = false;

            AddItem(pack);
        }

        public TillerManHS(Serial serial)
            : base(serial)
        {
        }
		
		
        [CommandProperty(AccessLevel.GameMaster)]
        public uint KeyValue { get { return m_KeyValue; } set { m_KeyValue = value; } }		

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get { return m_Galleon; } }		

        public void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.South: Direction = Server.Direction.South; break;
                case Direction.North: Direction = Server.Direction.North; break;
                case Direction.West: Direction = Server.Direction.West; break;
                case Direction.East: Direction = Server.Direction.East; break;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            if (m_Galleon != null)
				switch ((int)m_Galleon.Status)
				{
					case 2:
						{
							list.Add(1116580);

							break;
						}
						
					case 1:
						{
							list.Add(1116582);

							break;
						}
						
					case 0:
						{
							list.Add(1116583);

							break;
						}
				}          
        }

        public void TillerManSay(int number)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, number);
        }

        public void TillerManSay(int number, string args)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, number, args);
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            if (m_Galleon != null && m_Galleon.ShipName != null)
                list.Add(1042884, m_Galleon.ShipName); // the tiller man of the ~1_SHIP_NAME~
            else
                base.AddNameProperties(list);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Galleon != null && m_Galleon.ShipName != null)
                from.SendLocalizedMessage(1042884, m_Galleon.ShipName); // the tiller man of the ~1_SHIP_NAME~
            else
                base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendGump(new BoatGump(from, m_Galleon));
				
			/*
			* Removed while building the Tillerman Gump
			*
            *if (m_Galleon != null && m_Galleon.Contains(from))
            *    m_Galleon.BeginRename(from);
            *else if (m_Galleon != null)
            *    m_Galleon.BeginDryDock(from);
			*/
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is MapItem && m_Galleon != null && m_Galleon.CanCommand(from) && m_Galleon.Contains(from))
            {
                m_Galleon.AssociateMap((MapItem)dropped);
            }

            return false;
        }

        public override void OnAfterDelete()
        {
            if (m_Galleon != null)
                m_Galleon.Delete();
        }
		
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
			base.GetContextMenuEntries(from, list);
            list.Add(new SecuritySettingsEntry(from, (BaseGalleon)this.Transport));
        }		
		

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version

            writer.Write(m_Galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Galleon = reader.ReadItem() as BaseGalleon;

                        if (m_Galleon == null)
                            Delete();

                        break;
                    }
            }
        }
		
		private class SecuritySettingsEntry : ContextMenuEntry
		{			
			private readonly Mobile m_from;
			private BaseGalleon m_Galleon;
			
			public SecuritySettingsEntry(Mobile from, BaseGalleon galleon)
				: base(1116567)
			{				
				m_from = from;
				m_Galleon = galleon;
			}

			public override void OnClick()
			{
				if (m_from != null)
				{
				
					Dictionary<int, PlayerMobile> PlayersAboard = new Dictionary<int,PlayerMobile>();
					IPooledEnumerable eable = m_from.Map.GetClientsInRange(m_from.Location, m_Galleon.GetMaxUpdateRange());
					int i = 0;
					foreach (NetState state in eable)
					{
						Mobile m = state.Mobile;

						if (m is PlayerMobile)						
							if (m_Galleon.IsOnBoard(m))											
								PlayersAboard.Add(i++,(PlayerMobile)m);																            
					}
					eable.Free();					
				
					m_from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, m_from, (BaseShip)m_Galleon, PlayersAboard, 1, null));
				}
			}
		}		
    }
}