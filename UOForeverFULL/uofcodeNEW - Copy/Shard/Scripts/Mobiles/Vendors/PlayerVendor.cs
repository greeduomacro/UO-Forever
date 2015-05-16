#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Server.Commands;
using Server.ContextMenus;
using Server.Engines.BulkOrders;
using Server.Engines.XmlSpawner2;
using Server.Ethics;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Prompts;
using Server.Targeting;

#endregion

namespace Server.Mobiles
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PlayerVendorTargetAttribute : Attribute
    {}

    public class VendorItem
    {
        private string m_Description;
        private readonly DateTime m_Created;

        public Item Item { get; private set; }
        public int Price { get; private set; }

        public string FormattedPrice { get { return Price.ToString("#,0"); } }

        public string Description
        {
            get { return m_Description; }
            set
            {
                if (value != null)
                {
                    m_Description = value;
                }
                else
                {
                    m_Description = "";
                }

                if (Valid)
                {
                    Item.InvalidateProperties();
                }
            }
        }

        public DateTime Created { get { return m_Created; } }

        public bool IsForSale { get { return Price >= 0; } }
        public bool IsForFree { get { return Price == 0; } }

        public bool Valid { get; private set; }

        public VendorItem(Item item, int price, string description, DateTime created)
        {
            Item = item;
            Price = price;

            if (description != null)
            {
                m_Description = description;
            }
            else
            {
                m_Description = "";
            }

            m_Created = created;

            Valid = true;
        }

        public void Invalidate()
        {
            Valid = false;
        }
    }

    public class VendorBackpack : Backpack
    {
        public VendorBackpack()
        {
            Layer = Layer.Backpack;
            Weight = 1.0;
        }

        public override int DefaultMaxWeight { get { return 0; } }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (!base.CheckHold(m, item, message, checkItems, plusItems, plusWeight))
            {
                return false;
            }

            if (Ethic.IsImbued(item, true))
            {
                if (message)
                {
                    m.SendMessage("Imbued items may not be sold here.");
                }

                return false;
            }

            if (!BaseHouse.NewVendorSystem && Parent is PlayerVendor)
            {
                BaseHouse house = ((PlayerVendor) Parent).House;

                if (house != null && house.IsAosRules && !house.CheckAosStorage(1 + item.TotalItems + plusItems))
                {
                    if (message)
                    {
                        m.SendLocalizedMessage(1061839);
                            // This action would exceed the secure storage limit of the house.
                    }

                    return false;
                }
            }

            return true;
        }

        public override bool IsAccessibleTo(Mobile m)
        {
            return true;
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (base.CheckItemUse(from, item))
            {
                if (item is Container || item is BulkOrderBook || item is Runebook)
                {
                    return true;
                }

                from.SendLocalizedMessage(500447); // That is not accessible.
            }

            return false;
        }

        public override bool CheckTarget(Mobile from, Target targ, object targeted)
        {
            if (!base.CheckTarget(from, targ, targeted))
            {
                return false;
            }

            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                return true;
            }

            return base.CheckTarget(from, targ, targeted) &&
                   (from.AccessLevel >= AccessLevel.GameMaster ||
                    targ.GetType().IsDefined(typeof(PlayerVendorTargetAttribute), false));
        }

        public override void GetChildContextMenuEntries(Mobile from, List<ContextMenuEntry> list, Item item)
        {
            base.GetChildContextMenuEntries(from, list, item);

            var pv = RootParent as PlayerVendor;

            if (pv == null || pv.IsOwner(from))
            {
                return;
            }

            VendorItem vi = pv.GetVendorItem(item);

            if (vi != null)
            {
                list.Add(new BuyEntry(item));
            }
        }

        private class BuyEntry : ContextMenuEntry
        {
            private readonly Item m_Item;

            public BuyEntry(Item item)
                : base(6103)
            {
                m_Item = item;
            }

            public override bool NonLocalUse { get { return true; } }

            public override void OnClick()
            {
                if (m_Item.Deleted)
                {
                    return;
                }

                PlayerVendor.TryToBuy(m_Item, Owner.From);
            }
        }

        public override void GetChildNameProperties(ObjectPropertyList list, Item item)
        {
            base.GetChildNameProperties(list, item);

            var pv = RootParent as PlayerVendor;

            if (pv != null)
            {
                VendorItem vi = pv.GetVendorItem(item);

                if (vi != null)
                {
                    if (!vi.IsForSale)
                    {
                        list.Add(1043307); // Price: Not for sale.
                    }
                    else if (vi.IsForFree)
                    {
                        list.Add(1043306); // Price: FREE!
                    }
                    else
                    {
                        list.Add(1043304, vi.FormattedPrice); // Price: ~1_COST~
                    }
                }
            }
        }

        public override void GetChildProperties(ObjectPropertyList list, Item item)
        {
            base.GetChildProperties(list, item);

            var pv = RootParent as PlayerVendor;

            if (pv != null)
            {
                VendorItem vi = pv.GetVendorItem(item);

                if (vi != null && vi.Description != null && vi.Description.Length > 0)
                {
                    list.Add(1043305, vi.Description); // <br>Seller's Description:<br>"~1_DESC~"
                }
            }
        }

        public override void OnSingleClickContained(Mobile from, Item item)
        {
            if (RootParent is PlayerVendor)
            {
                var vendor = (PlayerVendor) RootParent;

                VendorItem vi = vendor.GetVendorItem(item);

                if (vi != null)
                {
                    if (!vi.IsForSale)
                    {
                        item.LabelTo(from, 1043307); // Price: Not for sale.
                    }
                    else if (vi.IsForFree)
                    {
                        item.LabelTo(from, 1043306); // Price: FREE!
                    }
                    else
                    {
                        item.LabelTo(from, 1043304, vi.FormattedPrice); // Price: ~1_COST~
                    }

                    if (!String.IsNullOrEmpty(vi.Description))
                    {
                        // The localized message (1043305) is no longer valid - <br>Seller's Description:<br>"~1_DESC~"
                        item.LabelTo(from, "Description: {0}", vi.Description);
                    }
                }
            }

            base.OnSingleClickContained(from, item);
        }

        public VendorBackpack(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }

    public class PlayerVendor : Mobile
    {
        private Dictionary<Item, VendorItem> m_SellItems;
        private BaseHouse m_House;
        private string m_ShopName;
        private Timer m_PayTimer;
        private DateTime _LastActivity = DateTime.UtcNow;

        private readonly TimeSpan _InactiveSpan = TimeSpan.FromDays(14);

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual DateTime LastActivity { get { return _LastActivity; } set { _LastActivity = value; } }

        public virtual Type TypeOfCurrency
        {
            get { return Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold); }
        }

        public PlayerVendor(Mobile owner, BaseHouse house)
        {
            Owner = owner;
            House = house;

            if (BaseHouse.NewVendorSystem)
            {
                BankAccount = 0;
                HoldCurrency = 4;
            }
            else
            {
                BankAccount = 1000;
                HoldCurrency = 0;
            }

            ShopName = "Shop Not Yet Named";

            m_SellItems = new Dictionary<Item, VendorItem>();

            CantWalk = true;

            InitStats(75, 75, 75);
            InitBody();
            InitOutfit();

            TimeSpan delay = PayTimer.GetInterval();

            m_PayTimer = new PayTimer(this, delay);
            m_PayTimer.Start();

            NextPayTime = DateTime.UtcNow + delay;

            LastActivity = DateTime.UtcNow;
        }

        public PlayerVendor(Serial serial)
            : base(serial)
        {}

        protected override void OnExpansionChanged(Expansion old)
        {
            base.OnExpansionChanged(old);

            NameHue = EraAOS ? -1 : 0x35;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write(_LastActivity);
            writer.Write(BaseHouse.NewVendorSystem);
            writer.Write(m_ShopName);
            writer.WriteDeltaTime(NextPayTime);
            writer.Write(House);

            writer.Write(Owner);
            writer.Write(BankAccount);
            writer.Write(HoldCurrency);

            writer.Write(m_SellItems.Count);

            foreach (VendorItem vi in m_SellItems.Values)
            {
                writer.Write(vi.Item);
                writer.Write(vi.Price);
                writer.Write(vi.Description);

                writer.Write(vi.Created);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            bool newVendorSystem = false;

            switch (version)
            {
                case 2:
                {
                    _LastActivity = reader.ReadDateTime();

                    if (_LastActivity == DateTime.MinValue)
                    {
                        _LastActivity = DateTime.UtcNow;
                    }
                    else if (_LastActivity + _InactiveSpan <= DateTime.UtcNow)
                    {
                        NameMod = string.Empty;
                    }
                    else if (_LastActivity + TimeSpan.FromDays(7) <= DateTime.UtcNow)
                    {
                        NameHue = 0;
                    }
                }
                    goto case 1;
                case 1:
                {
                    newVendorSystem = reader.ReadBool();
                    m_ShopName = reader.ReadString();
                    NextPayTime = reader.ReadDeltaTime();
                    House = reader.ReadItem<BaseHouse>();
                }
                    goto case 0;
                case 0:
                {
                    Owner = reader.ReadMobile();
                    BankAccount = reader.ReadInt();
                    HoldCurrency = reader.ReadInt();

                    m_SellItems = new Dictionary<Item, VendorItem>();

                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Item item = reader.ReadItem();

                        int price = reader.ReadInt();
                        if (price > 100000000)
                        {
                            price = 100000000;
                        }

                        string description = reader.ReadString();

                        DateTime created = version < 1 ? DateTime.UtcNow : reader.ReadDateTime();

                        if (item != null)
                        {
                            SetVendorItem(item, version < 1 && price <= 0 ? -1 : price, description, created);
                        }
                    }
                }
                    break;
            }

            bool newVendorSystemActivated = BaseHouse.NewVendorSystem && !newVendorSystem;

            if (version < 1 || newVendorSystemActivated)
            {
                if (version < 1)
                {
                    m_ShopName = "Shop Not Yet Named";
                    Timer.DelayCall(TimeSpan.Zero, UpgradeFromVersion0, newVendorSystemActivated);
                }
                else
                {
                    Timer.DelayCall(TimeSpan.Zero, FixDresswear);
                }

                NextPayTime = DateTime.UtcNow + PayTimer.GetInterval();

                if (newVendorSystemActivated)
                {
                    HoldCurrency += BankAccount;
                    BankAccount = 0;
                }
            }

            TimeSpan delay = NextPayTime - DateTime.UtcNow;

            m_PayTimer = new PayTimer(this, delay > TimeSpan.Zero ? delay : TimeSpan.Zero);
            m_PayTimer.Start();

            Blessed = false;

            if (EraAOS && NameHue == 0x35)
            {
                NameHue = -1;
            }
        }

        private void UpgradeFromVersion0(bool newVendorSystem)
        {
            var toRemove = new List<Item>();

            foreach (VendorItem vi in m_SellItems.Values)
            {
                if (!CanBeVendorItem(vi.Item))
                {
                    toRemove.Add(vi.Item);
                }
                else
                {
                    vi.Description = Utility.FixHtml(vi.Description);
                }
            }

            foreach (Item item in toRemove)
            {
                RemoveVendorItem(item);
            }

            House = BaseHouse.FindHouseAt(this);

            if (newVendorSystem)
            {
                ActivateNewVendorSystem();
            }
        }

        private void ActivateNewVendorSystem()
        {
            FixDresswear();

            if (House != null && !House.IsOwner(Owner))
            {
                Destroy(true);
            }
        }

        public void InitBody()
        {
            Hue = Utility.RandomSkinHue();
            SpeechHue = 0x3B2;

            Female = Utility.RandomBool();

            if (Female)
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }
        }

        public virtual void InitOutfit()
        {
            Item item = new FancyShirt(Utility.RandomNeutralHue());
            item.Layer = Layer.InnerTorso;
            AddItem(item);

            AddItem(new LongPants(Utility.RandomNeutralHue()));
            AddItem(new BodySash(Utility.RandomNeutralHue()));
            AddItem(new Boots(Utility.RandomNeutralHue()));
            AddItem(new Cloak(Utility.RandomNeutralHue()));

            Utility.AssignRandomHair(this);

            Container pack = new VendorBackpack();
            pack.Movable = false;
            AddItem(pack);
        }

        public string ReturnedStuffStatus = "none";

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BankAccount { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HoldCurrency { get; set; }

        [CommandProperty(AccessLevel.Lead)]
        public bool DestroyVendorAndReturnStuff
        {
            get { return false; }
            set
            {
                if (value)
                {
                    Destroy(true);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ShopName
        {
            get { return m_ShopName; }
            set
            {
                m_ShopName = value ?? String.Empty;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster, true)]
        public DateTime NextPayTime { get; private set; }

        public PlayerVendorPlaceholder Placeholder { get; set; }

        public BaseHouse House
        {
            get { return m_House; }
            set
            {
                if (m_House != null)
                {
                    m_House.PlayerVendors.Remove(this);
                }

                if (value != null)
                {
                    value.PlayerVendors.Add(this);
                }

                m_House = value;
            }
        }

        public int ChargePerDay
        {
            get
            {
                if (BaseHouse.NewVendorSystem)
                {
                    return ChargePerRealWorldDay / 12;
                }

                long total = m_SellItems.Values.Sum(vi => (long) vi.Price);

                total -= 500;

                if (total < 0)
                {
                    total = 0;
                }

                return (int) (20 + (total / 500));
            }
        }

        public int ChargePerRealWorldDay
        {
            get
            {
                if (BaseHouse.NewVendorSystem)
                {
                    long total = m_SellItems.Values.Sum(vi => (long) vi.Price);

                    return (int) (60 + (total / 500) * 3);
                }

                return ChargePerDay * 12;
            }
        }

        public virtual bool IsOwner(Mobile m)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
            {
                return true;
            }

            if (BaseHouse.NewVendorSystem && House != null)
            {
                return House.IsOwner(m);
            }

            return m == Owner;
        }

        protected List<Item> GetItems()
        {
            List<Item> list =
                Items.Where(
                    item =>
                        item.Movable && item != Backpack && item.Layer != Layer.Face && item.Layer != Layer.Hair &&
                        item.Layer != Layer.FacialHair).ToList();

            if (Backpack != null)
            {
                list.AddRange(Backpack.Items);
            }

            return list;
        }

        public virtual void Destroy(bool toBackpack)
        {
            if (Deleted)
            {
                return;
            }

            try
            {
                Return();

                if (!BaseHouse.NewVendorSystem)
                {
                    FixDresswear();
                }

                /* Possible cases regarding item return:
                 *
                 * 1. No item must be returned
                 *       -> do nothing.
                 * 2. ( toBackpack is false OR the vendor is in the internal map ) AND the vendor is associated with a AOS house
                 *       -> put the items into the moving crate or a vendor inventory,
                 *          depending on whether the vendor owner is also the house owner.
                 * 3. ( toBackpack is true OR the vendor isn't associated with any AOS house ) AND the vendor isn't in the internal map
                 *       -> put the items into a backpack.
                 * 4. The vendor isn't associated with any house AND it's in the internal map
                 *       -> do nothing (we can't do anything).
                 */

                List<Item> list = GetItems();

                if (Owner == null)
                {
                    // move the items to the ground
                    foreach (Item item in list)
                    {
                        item.MoveToWorld(Location, Map);
                    }

                    ReturnedStuffStatus = "owner null, stuff dropped to ground";
                    Delete();
                    return;
                }

                BankBox bank = Owner is PlayerMobile
                    ? ((PlayerMobile) Owner).BankBoxes.FirstOrDefault(b => b != null && b.Expansion == Expansion)
                    : Owner.BankBox;

                if (bank == null)
                {
                    // move the items to the ground
                    foreach (Item item in list)
                    {
                        item.MoveToWorld(Location, Map);
                    }

                    ReturnedStuffStatus = "owner bankbox null, stuff dropped to ground";
                    Delete();
                    return;
                }

                if (list.Count > 0 || HoldCurrency > 0) // No case 1
                {
                    if (toBackpack || House == null) // Case 3 - Move to backpack
                    {
                        Container backpack = new Backpack();

                        if (HoldCurrency > 0)
                        {
                            Banker.Deposit(backpack, TypeOfCurrency, HoldCurrency);
                        }

                        foreach (Item item in list)
                        {
                            backpack.DropItem(item);
                        }

                        bank.DropItem(backpack);
                        ReturnedStuffStatus = "returned to owner bank";
                    }
                }
                else
                {
                    ReturnedStuffStatus = "nothing to return";
                }
            }
            catch (Exception e)
            {
                LoggingCustom.Log("ERROR_PlayerVendor.txt", DateTime.Now + "\t" + e.Message + "\n" + e.StackTrace);
            }

            Delete();
        }

        private void FixDresswear()
        {
            foreach (Item item in Items)
            {
                if (item is BaseHat)
                {
                    item.Layer = Layer.Helm;
                }
                else if (item is BaseMiddleTorso)
                {
                    item.Layer = Layer.MiddleTorso;
                }
                else if (item is BaseOuterLegs)
                {
                    item.Layer = Layer.OuterLegs;
                }
                else if (item is BaseOuterTorso)
                {
                    item.Layer = Layer.OuterTorso;
                }
                else if (item is BasePants)
                {
                    item.Layer = Layer.Pants;
                }
                else if (item is BaseShirt)
                {
                    item.Layer = Layer.Shirt;
                }
                else if (item is BaseWaist)
                {
                    item.Layer = Layer.Waist;
                }
                else if (item is BaseShoes)
                {
                    if (item is Sandals)
                    {
                        item.Hue = 0;
                    }

                    item.Layer = Layer.Shoes;
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            LoggingCustom.Log(
                "LOG_VendorDeleted",
                DateTime.Now + "\tVendor: " + this + "\tLocation: " + Location + "\tOwner: " + Owner +
                "\tReturnStatus: " +
                ReturnedStuffStatus);

            m_PayTimer.Stop();

            House = null;

            if (Placeholder != null)
            {
                Placeholder.Delete();
            }
        }

        public override bool IsSnoop(Mobile from)
        {
            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (BaseHouse.NewVendorSystem)
            {
                list.Add(1062449, ShopName); // Shop Name: ~1_NAME~
            }
        }

        public VendorItem GetVendorItem(Item item)
        {
            VendorItem vi;
            m_SellItems.TryGetValue(item, out vi);
            return vi;
        }

        public VendorItem SetVendorItem(Item item, int price, string description)
        {
            return SetVendorItem(item, price, description, DateTime.UtcNow);
        }

        public VendorItem SetVendorItem(Item item, int price, string description, DateTime created)
        {
            RemoveVendorItem(item);

            VendorItem vi = m_SellItems[item] = new VendorItem(item, price, description, created);

            item.InvalidateProperties();

            return vi;
        }

        public void RemoveVendorItem(Item item)
        {
            VendorItem vi = GetVendorItem(item);

            if (vi == null)
            {
                return;
            }

            vi.Invalidate();

            m_SellItems.Remove(item);

            foreach (Item subItem in item.Items.ToArray())
            {
                RemoveVendorItem(subItem);
            }

            item.InvalidateProperties();
        }

        private bool CanBeVendorItem(Item item)
        {
            var parent = item.Parent as Item;

            if (parent == Backpack)
            {
                return true;
            }

            if (parent is Container)
            {
                VendorItem parentVI = GetVendorItem(parent);

                if (parentVI != null)
                {
                    return !parentVI.IsForSale;
                }
            }

            return false;
        }

        public override void OnSubItemAdded(Item item)
        {
            base.OnSubItemAdded(item);

            if (GetVendorItem(item) == null && CanBeVendorItem(item))
            {
                // TODO: default price should be dependent to the type of object
                SetVendorItem(item, 999, "");
            }
        }

        public override void OnSubItemRemoved(Item item)
        {
            base.OnSubItemRemoved(item);

            if (item.GetBounce() == null)
            {
                RemoveVendorItem(item);
            }
        }

        public override void OnSubItemBounceCleared(Item item)
        {
            base.OnSubItemBounceCleared(item);

            if (!CanBeVendorItem(item))
            {
                RemoveVendorItem(item);
            }
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (item != Backpack)
            {
                return;
            }

            foreach (Item subItem in item.Items)
            {
                RemoveVendorItem(subItem);
            }
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            // trigger returns true if returnoverride
            if (XmlScript.HasTrigger(this, TriggerName.onDragDrop) &&
                UberScriptTriggers.Trigger(this, from, TriggerName.onDragDrop, item))
            {
                return true;
            }

            if (!IsOwner(from))
            {
                SayTo(from, 503209); // I can only take item from the shop owner.
                return false;
            }

            if (item is CommodityDeed && ((CommodityDeed) item).Commodity == null)
            {
                SayTo(from, "I refuse to sell empty commodity deeds!");
                return false;
            }

            if (item.TypeEquals(TypeOfCurrency))
            {
                if (BaseHouse.NewVendorSystem)
                {
                    if (HoldCurrency < 1000000)
                    {
                        SayTo(from, 503210); // I'll take that to fund my services.

                        HoldCurrency += item.Amount;
                        item.Delete();

                        return true;
                    }

                    from.SendMessage("Your vendor has sufficient funds for operation and cannot accept this {0}.",
                        TypeOfCurrency.Name);
                    return false;
                }

                if (BankAccount < 1000000)
                {
                    SayTo(from, 503210); // I'll take that to fund my services.

                    BankAccount += item.Amount;
                    item.Delete();

                    return true;
                }

                from.SendMessage("Your vendor has sufficient funds for operation and cannot accept this {0}.",
                    TypeOfCurrency.Name);
                return false;
            }

            bool newItem = (GetVendorItem(item) == null);

            if (Backpack != null && Backpack.TryDropItem(from, item, false))
            {
                if (newItem)
                {
                    OnItemGiven(from, item);
                }

                return true;
            }

            SayTo(from, 503211); // I can't carry any more.
            return false;
        }

        public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
        {
            if (IsOwner(from))
            {
                if (item is CommodityDeed && ((CommodityDeed) item).Commodity == null)
                {
                    SayTo(from, "I refuse to sell empty commodity deeds!");
                    return false;
                }
                if (GetVendorItem(item) == null)
                {
                    // We must wait until the item is added
                    Timer.DelayCall(TimeSpan.Zero, NonLocalDropCallback, new NonLocalDropInfo(from, item));
                }

                return true;
            }

            SayTo(from, 503209); // I can only take item from the shop owner.
            return false;
        }

        private class NonLocalDropInfo
        {
            public Mobile From { get; private set; }
            public Item Item { get; private set; }

            public NonLocalDropInfo(Mobile from, Item item)
            {
                From = from;
                Item = item;
            }
        }

        private void NonLocalDropCallback(NonLocalDropInfo info)
        {
            OnItemGiven(info.From, info.Item);
        }

        private void OnItemGiven(Mobile from, Item item)
        {
            VendorItem vi = GetVendorItem(item);

            if (vi != null)
            {
                string name;

                if (String.IsNullOrEmpty(item.Name))
                {
                    name = "#" + item.LabelNumber.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    name = item.Name;
                }

                from.SendLocalizedMessage(1043303, name);
                    // Type in a price and description for ~1_ITEM~ (ESC=not for sale)
                from.Prompt = new VendorPricePrompt(this, vi);
            }
            _LastActivity = DateTime.UtcNow;
            NameMod = null;
            NameHue = -1;
        }

        public override bool AllowEquipFrom(Mobile from)
        {
            if (BaseHouse.NewVendorSystem && IsOwner(from))
            {
                return true;
            }

            return base.AllowEquipFrom(from);
        }

        public override bool CheckNonlocalLift(Mobile from, Item item)
        {
            if (item.IsChildOf(Backpack))
            {
                if (IsOwner(from))
                {
                    return true;
                }

                SayTo(from, 503223); // If you'd like to purchase an item, just ask.
                return false;
            }

            if (BaseHouse.NewVendorSystem && IsOwner(from))
            {
                return true;
            }

            return base.CheckNonlocalLift(from, item);
        }

        public bool CanInteractWith(Mobile from, bool ownerOnly)
        {
            if (!from.CanSee(this) || !Utility.InUpdateRange(from, this) || !from.CheckAlive())
            {
                return false;
            }

            if (ownerOnly)
            {
                return IsOwner(from);
            }

            if (House != null && House.IsBanned(from) && !IsOwner(from))
            {
                // You can't shop from this home as you have been banned from this establishment.
                from.SendLocalizedMessage(1062674);
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsOwner(from))
            {
                SendOwnerGump(from);
            }
            else if (CanInteractWith(from, false))
            {
                OpenBackpack(from);
            }
        }

        public override void DisplayPaperdollTo(Mobile m)
        {
            if (BaseHouse.NewVendorSystem)
            {
                base.DisplayPaperdollTo(m);
            }
            else if (CanInteractWith(m, false))
            {
                OpenBackpack(m);
            }
        }

        public void SendOwnerGump(Mobile to)
        {
            if (BaseHouse.NewVendorSystem)
            {
                to.CloseGump(typeof(NewPlayerVendorOwnerGump));
                to.CloseGump(typeof(NewPlayerVendorCustomizeGump));

                to.SendGump(new NewPlayerVendorOwnerGump(this));
            }
            else
            {
                to.CloseGump(typeof(PlayerVendorOwnerGump));
                to.CloseGump(typeof(PlayerVendorCustomizeGump));

                to.SendGump(new PlayerVendorOwnerGump(this));
            }
        }

        public void OpenBackpack(Mobile from)
        {
            if (Backpack == null)
            {
                return;
            }

            SayTo(from, IsOwner(from) ? 1010642 : 503208); // Take a look at my/your goods.

            Backpack.DisplayTo(from);
        }

        public static void TryToBuy(Item item, Mobile from)
        {
            var vendor = item.RootParent as PlayerVendor;

            if (vendor == null || !vendor.CanInteractWith(from, false))
            {
                return;
            }

            if (vendor.IsOwner(from))
            {
                vendor.SayTo(from, 503212); // You own this shop, just take what you want.
                return;
            }

            VendorItem vi = vendor.GetVendorItem(item);

            if (vi == null)
            {
                vendor.SayTo(from, 503216); // You can't buy that.
            }
            else if (!vi.IsForSale)
            {
                vendor.SayTo(from, 503202); // This item is not for sale.
            }
            else if (vi.Created + TimeSpan.FromMinutes(1.0) > DateTime.UtcNow)
            {
                from.SendMessage("You cannot buy this item right now.  Please wait one minute and try again.");
            }
            else
            {
                from.CloseGump(typeof(PlayerVendorBuyGump));
                from.SendGump(new PlayerVendorBuyGump(vendor, vi));
            }
        }

        public void CollectCurrency(Mobile to)
        {
            if (HoldCurrency > 0)
            {
                SayTo(to, "How much of the {0:#,0} {1} that I'm holding would you like?", HoldCurrency,
                    TypeOfCurrency.Name);
                to.SendMessage("Enter the amount of {0} you wish to withdraw (ESC = CANCEL):", TypeOfCurrency.Name);

                to.Prompt = new CollectCurrencyPrompt(this);
            }
            else
            {
                SayTo(to, "I am holding no {0} for you.", TypeOfCurrency.Name);
            }
        }

        public int GiveCurrency(Mobile to, int amount)
        {
            if (amount <= 0)
            {
                return 0;
            }

            if (amount > HoldCurrency)
            {
                SayTo(to, "I'm sorry, but I'm only holding {0:#,0} {1} for you.", HoldCurrency, TypeOfCurrency.Name);
                return 0;
            }

            int amountGiven = Banker.DepositUpTo(to, TypeOfCurrency, amount);

            HoldCurrency -= amountGiven;

            if (amountGiven > 0)
            {
                to.SendMessage("{0:#,0} {1} has been deposited into your bank box.", amountGiven, TypeOfCurrency.Name);
            }

            if (amountGiven == 0)
            {
                SayTo(to,
                    "Your bank box cannot hold the {0} you are requesting.  I will keep the {0} until you can take it.",
                    TypeOfCurrency.Name);
            }
            else if (amount > amountGiven)
            {
                SayTo(to,
                    "I can only give you part of the {0} now, as your bank box is too full to hold the full amount.",
                    TypeOfCurrency.Name);
            }
            else if (HoldCurrency > 0)
            {
                SayTo(to, "Your {0} has been transferred.", TypeOfCurrency.Name);
            }
            else
            {
                SayTo(to, "All the {0} I have been carrying for you has been deposited into your bank account.",
                    TypeOfCurrency.Name);
            }

            return amountGiven;
        }

        public void Dismiss(Mobile from)
        {
            Container pack = Backpack;

            if (pack != null && pack.Items.Count > 0)
            {
                SayTo(from, 1038325); // You cannot dismiss me while I am holding your goods.
                return;
            }

            if (HoldCurrency > 0)
            {
                GiveCurrency(from, HoldCurrency);

                if (HoldCurrency > 0)
                {
                    return;
                }
            }

            Destroy(true);
        }

        public void Rename(Mobile from)
        {
            from.SendLocalizedMessage(1062494); // Enter a new name for your vendor (20 characters max):

            from.Prompt = new VendorNamePrompt(this);
        }

        public void RenameShop(Mobile from)
        {
            from.SendLocalizedMessage(1062433); // Enter a new name for your shop (20 chars max):

            from.Prompt = new ShopNamePrompt(this);
        }

        public bool CheckTeleport(Mobile to)
        {
            if (Deleted || !IsOwner(to) || House == null || Map == Map.Internal)
            {
                return false;
            }

            if (to.AccessLevel >= AccessLevel.GameMaster || House.IsInside(to) || to.Map != House.Map ||
                !House.InRange(to, 5))
            {
                return false;
            }

            if (Placeholder == null)
            {
                Placeholder = new PlayerVendorPlaceholder(this);
                Placeholder.MoveToWorld(Location, Map);

                MoveToWorld(to.Location, to.Map);

                // This vendor has been moved out of the house to your current location temporarily.  The vendor will return home automatically after two minutes have passed once you are done managing its inventory or customizing it.
                to.SendLocalizedMessage(1062431);
            }
            else
            {
                Placeholder.RestartTimer();

                // This vendor is currently temporarily in a location outside its house.  The vendor will return home automatically after two minutes have passed once you are done managing its inventory or customizing it.
                to.SendLocalizedMessage(1062430);
            }

            return true;
        }

        public void Return()
        {
            if (Placeholder != null)
            {
                Placeholder.Delete();
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && Placeholder != null && IsOwner(from))
            {
                list.Add(new ReturnVendorEntry(this));
            }

            base.GetContextMenuEntries(from, list);
        }

        private class ReturnVendorEntry : ContextMenuEntry
        {
            private readonly PlayerVendor m_Vendor;

            public ReturnVendorEntry(PlayerVendor vendor)
                : base(6214)
            {
                m_Vendor = vendor;
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (!m_Vendor.Deleted && m_Vendor.IsOwner(from) && from.CheckAlive())
                {
                    m_Vendor.Return();
                }
            }
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return (from.Alive && from.GetDistanceToSqrt(this) <= 3);
        }

        public bool WasNamed(string speech)
        {
            return Name != null && Insensitive.StartsWith(speech, Name);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (XmlScript.HasTrigger(this, TriggerName.onSpeech) &&
                UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
            {
                return;
            }

            Mobile from = e.Mobile;

            if (e.Handled || !from.Alive || from.GetDistanceToSqrt(this) > 3)
            {
                return;
            }

            if (e.HasKeyword(0x3C) || (e.HasKeyword(0x171) && WasNamed(e.Speech))) // vendor buy, *buy*
            {
                if (IsOwner(from))
                {
                    SayTo(from, 503212); // You own this shop, just take what you want.
                }
                else if (House == null || !House.IsBanned(from))
                {
                    from.SendLocalizedMessage(503213); // Select the item you wish to buy.
                    from.Target = new PVBuyTarget();

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x3D) || (e.HasKeyword(0x172) && WasNamed(e.Speech))) // vendor browse, *browse
            {
                if (House != null && House.IsBanned(from) && !IsOwner(from))
                {
                    SayTo(from, 1062674);
                        // You can't shop from this home as you have been banned from this establishment.
                }
                else
                {
                    if (WasNamed(e.Speech))
                    {
                        OpenBackpack(from);
                    }
                    else
                    {
                        IPooledEnumerable mobiles = from.GetMobilesInRange(2);

                        foreach (
                            PlayerVendor m in mobiles.OfType<PlayerVendor>().Where(m => m.CanSee(from) && m.InLOS(from))
                            )
                        {
                            m.OpenBackpack(from);
                        }

                        mobiles.Free();
                    }

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x3E) || (e.HasKeyword(0x173) && WasNamed(e.Speech))) // vendor collect, *collect
            {
                if (IsOwner(from))
                {
                    CollectCurrency(from);

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x3F) || (e.HasKeyword(0x174) && WasNamed(e.Speech))) // vendor status, *status
            {
                if (IsOwner(from))
                {
                    SendOwnerGump(from);

                    e.Handled = true;
                }
                else
                {
                    SayTo(from, 503226); // What do you care? You don't run this shop.
                }
            }
            else if (e.HasKeyword(0x40) || (e.HasKeyword(0x175) && WasNamed(e.Speech))) // vendor dismiss, *dismiss
            {
                if (IsOwner(from))
                {
                    Dismiss(from);

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x41) || (e.HasKeyword(0x176) && WasNamed(e.Speech))) // vendor cycle, *cycle
            {
                if (IsOwner(from))
                {
                    Direction = GetDirectionTo(from);

                    e.Handled = true;
                }
            }
        }

        private class PayTimer : Timer
        {
            public static TimeSpan GetInterval()
            {
                return BaseHouse.NewVendorSystem ? TimeSpan.FromDays(1.0) : TimeSpan.FromMinutes(Clock.MinutesPerUODay);
            }

            private readonly PlayerVendor m_Vendor;

            public PayTimer(PlayerVendor vendor, TimeSpan delay)
                : base(delay, GetInterval())
            {
                m_Vendor = vendor;

                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                m_Vendor.NextPayTime = DateTime.UtcNow + Interval;

                int pay;
                int totalCurrency;

                if (BaseHouse.NewVendorSystem)
                {
                    pay = m_Vendor.ChargePerRealWorldDay;
                    totalCurrency = m_Vendor.HoldCurrency;
                }
                else
                {
                    pay = m_Vendor.ChargePerDay;
                    totalCurrency = m_Vendor.BankAccount + m_Vendor.HoldCurrency;
                }

                if (pay > totalCurrency)
                {
                    m_Vendor.Destroy(true);
                }
                else
                {
                    if (!BaseHouse.NewVendorSystem)
                    {
                        if (m_Vendor.BankAccount >= pay)
                        {
                            m_Vendor.BankAccount -= pay;
                            pay = 0;
                        }
                        else
                        {
                            pay -= m_Vendor.BankAccount;
                            m_Vendor.BankAccount = 0;
                        }
                    }

                    m_Vendor.HoldCurrency -= pay;
                }

                if (m_Vendor.LastActivity + m_Vendor._InactiveSpan <= DateTime.UtcNow)
                {
                    m_Vendor.NameMod = string.Empty;
                }
                else if (m_Vendor.LastActivity + TimeSpan.FromDays(7) <= DateTime.UtcNow)
                {
                    m_Vendor.NameHue = 0;
                }
            }
        }

        [PlayerVendorTarget]
        private class PVBuyTarget : Target
        {
            public PVBuyTarget()
                : base(3, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Item)
                {
                    TryToBuy((Item) targeted, from);
                }
            }
        }

        private class VendorPricePrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;
            private readonly VendorItem m_VI;

            public VendorPricePrompt(PlayerVendor vendor, VendorItem vi)
            {
                m_Vendor = vendor;
                m_VI = vi;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_VI.Valid || !m_Vendor.CanInteractWith(from, true))
                {
                    return;
                }

                int sep = text.IndexOfAny(new[] {' ', ','});
                string firstWord = sep >= 0 ? text.Substring(0, sep) : text;

                int price;
                string description;

                if (int.TryParse(firstWord, out price))
                {
                    description = sep >= 0 ? text.Substring(sep + 1).Trim() : "";
                }
                else
                {
                    price = -1;
                    description = text.Trim();
                }

                SetInfo(from, price, Utility.FixHtml(description));
            }

            public override void OnCancel(Mobile from)
            {
                if (!m_VI.Valid || !m_Vendor.CanInteractWith(from, true))
                {
                    return;
                }

                SetInfo(from, -1, "");
            }

            private void SetInfo(Mobile from, int price, string description)
            {
                Item item = m_VI.Item;

                bool setPrice = false;

                if (price < 0) // Not for sale
                {
                    price = -1;

                    if (item is Container)
                    {
                        if (item is LockableContainer && ((LockableContainer) item).Locked)
                        {
                            m_Vendor.SayTo(from, 1043298); // Locked items may not be made not-for-sale.
                        }
                        else if (item.Items.Count > 0)
                        {
                            m_Vendor.SayTo(from, 1043299);
                                // To be not for sale, all items in a container must be for sale.
                        }
                        else
                        {
                            setPrice = true;
                        }
                    }
                    else if (item is BaseBook || item is BulkOrderBook)
                    {
                        setPrice = true;
                    }
                    else
                    {
                        // Only the following may be made not-for-sale: books, containers, keyrings, and items in for-sale containers.
                        m_Vendor.SayTo(from, 1043301);
                    }
                }
                else
                {
                    if (price > 100000000)
                    {
                        price = 100000000;
                        from.SendMessage("You cannot price items above 100,000,000 {0}.  The price has been adjusted.",
                            m_Vendor.TypeOfCurrency.Name);
                    }

                    setPrice = true;
                }

                if (setPrice)
                {
                    m_Vendor.SetVendorItem(item, price, description);

                    int pay = m_Vendor.ChargePerRealWorldDay;
                    int totalCurrency = m_Vendor.HoldCurrency;

                    if (totalCurrency < pay)
                    {
                        from.SendMessage(
                            38,
                            "WARNING! Your vendor costs {0:#,0} {2} per real-world day, and only has {1:#,0} {2} on it. If you do not pay your vendor, it will disappear within a day!",
                            pay, totalCurrency, m_Vendor.TypeOfCurrency.Name);
                    }
                    else
                    {
                        from.SendMessage(
                            0x38,
                            "Your vendor has enough money to operate {0:#,0} real world days based on the price of your wares.",
                            totalCurrency / pay);
                    }
                }
                else
                {
                    m_VI.Description = description;
                }
            }
        }

        private class CollectCurrencyPrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;

            public CollectCurrencyPrompt(PlayerVendor vendor)
            {
                m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_Vendor.CanInteractWith(from, true))
                {
                    return;
                }

                text = text.Trim();

                int amount;

                if (!int.TryParse(text, out amount))
                {
                    amount = 0;
                }

                GiveCurrency(from, amount);
            }

            public override void OnCancel(Mobile from)
            {
                if (m_Vendor.CanInteractWith(from, true))
                {
                    GiveCurrency(from, 0);
                }
            }

            private void GiveCurrency(Mobile to, int amount)
            {
                if (amount <= 0)
                {
                    m_Vendor.SayTo(to, "Very well. I will hold on to the money for now then.");
                }
                else
                {
                    m_Vendor.GiveCurrency(to, amount);
                }
            }
        }

        private class VendorNamePrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;

            public VendorNamePrompt(PlayerVendor vendor)
            {
                m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_Vendor.CanInteractWith(from, true))
                {
                    return;
                }

                string name = text.Trim();

                if (NameVerification.Validate(name, 1, 20, true, true, true, 0, NameVerification.Empty) !=
                    NameResultMessage.Allowed)
                {
                    m_Vendor.SayTo(from, "That name is unacceptable.");
                    return;
                }

                m_Vendor.Name = Utility.FixHtml(name);

                from.SendLocalizedMessage(1062496); // Your vendor has been renamed.

                from.SendGump(new NewPlayerVendorOwnerGump(m_Vendor));
            }
        }

        private class ShopNamePrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;

            public ShopNamePrompt(PlayerVendor vendor)
            {
                m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_Vendor.CanInteractWith(from, true))
                {
                    return;
                }

                string name = text.Trim();

                if (NameVerification.Validate(name, 1, 20, true, true, true, 0, NameVerification.Empty) !=
                    NameResultMessage.Allowed)
                {
                    m_Vendor.SayTo(from, "That name is unacceptable.");
                    return;
                }

                m_Vendor.ShopName = Utility.FixHtml(name);

                from.SendGump(new NewPlayerVendorOwnerGump(m_Vendor));
            }
        }

        public override bool CanBeDamaged()
        {
            return false;
        }
    }

    public class PlayerVendorPlaceholder : Item
    {
        private PlayerVendor m_Vendor;
        private readonly ExpireTimer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerVendor Vendor { get { return m_Vendor; } }

        public PlayerVendorPlaceholder(PlayerVendor vendor)
            : base(0x1F28)
        {
            Hue = 0x672;
            Movable = false;

            m_Vendor = vendor;

            m_Timer = new ExpireTimer(this);
            m_Timer.Start();
        }

        public PlayerVendorPlaceholder(Serial serial)
            : base(serial)
        {}

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Vendor != null)
            {
                list.Add(1062498, m_Vendor.Name); // reserved for vendor ~1_NAME~
            }
        }

        public void RestartTimer()
        {
            m_Timer.Stop();
            m_Timer.Start();
        }

        private class ExpireTimer : Timer
        {
            private readonly PlayerVendorPlaceholder m_Placeholder;

            public ExpireTimer(PlayerVendorPlaceholder placeholder)
                : base(TimeSpan.FromMinutes(2.0))
            {
                m_Placeholder = placeholder;

                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                m_Placeholder.Delete();
            }
        }

        public override void OnDelete()
        {
            if (m_Vendor != null && !m_Vendor.Deleted)
            {
                m_Vendor.MoveToWorld(Location, Map);
                m_Vendor.Placeholder = null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0);

            writer.Write(m_Vendor);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadEncodedInt();

            m_Vendor = (PlayerVendor) reader.ReadMobile();

            Timer.DelayCall(TimeSpan.Zero, Delete);
        }
    }
}