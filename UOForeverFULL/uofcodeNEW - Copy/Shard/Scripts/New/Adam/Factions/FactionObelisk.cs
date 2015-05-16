#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Accounting;
using Server.Engines.CentralGump;
using Server.Factions;
using Server.Mobiles;
using Server.Network;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Items
{
    public class FactionObelisks
    {
        private static readonly List<FactionObelisk> m_Obelisks = new List<FactionObelisk>();

        public static List<FactionObelisk> Obelisks { get { return m_Obelisks; } }

        public static void Register(FactionObelisk obelisk)
        {
            m_Obelisks.Add(obelisk);
        }
    }

    public enum ObeliskType
    {
        SkillGain,
        Power,
        Bloodshed
    }

    public class FactionObelisk : Item
    {
        private string _OwnedName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string OwningFaction { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool FullyControlled { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharge { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentCharge { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int OwnerHue { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ObeliskType ObeliskType { get; set; }

        public Dictionary<Account, DateTime> ObeliskUsers { get; set; }

        private InternalTimer timer { get; set; }

        [Constructable]
        public FactionObelisk(int itemid)
            : base(itemid)
        {
            Movable = false;
            timer = new InternalTimer(this);
            timer.Start();
            FactionObelisks.Register(this);

            ObeliskUsers = new Dictionary<Account, DateTime>();
        }

        public FactionObelisk(Serial serial)
            : base(serial)
        {}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            if (!string.IsNullOrEmpty(_OwnedName) && FullyControlled)
            {
                LabelTo(from, "Owned by: " + _OwnedName, Hue);
            }
            else
            {
                LabelTo(from, "Being assaulted by: " + _OwnedName, Hue);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            var pm = from as PlayerMobile;
            var acc = from.Account as Account;
            if (pm != null && acc != null && !String.IsNullOrEmpty(pm.FactionName) &&
                !string.IsNullOrEmpty(OwningFaction))
            {
                Faction faction = Faction.Find(pm);
                PlayerState state = PlayerState.Find(pm);
                if (faction != null && state != null && OwningFaction == faction.Definition.Name &&
                    !ObeliskUsers.ContainsKey(acc))
                {
                    if (DateTime.UtcNow <= state.LastKill + TimeSpan.FromHours(2) ||
                        from.AccessLevel > AccessLevel.Player)
                    {
                        Effects.SendIndividualFlashEffect(from, (FlashType) 2);
                        from.SendMessage(61, "The obelisk fills you with great power.");
                        from.BoltEffect(2049);
                        from.HueMod = faction.Definition.HuePrimary;
                        ObeliskUsers.Add(acc, DateTime.UtcNow + TimeSpan.FromHours(2));
                    }
                    else
                    {
                        from.SendMessage(61,
                            "You must honor thy faction with blood. Seek out an enemy faction member and slay them.  Only then will you be worthy of the focal point's power.");
                    }
                }
                else
                {
                    from.SendMessage(61, "You already wield the power of this focal point.");
                }
            }
            else
            {
                from.SendMessage(61,
                    "You must be a part of factions and your faction must have control of this focal point in order for you to use it.");
            }
            base.OnDoubleClick(from);
        }

        public void RemoveUsers()
        {
            foreach (KeyValuePair<Account, DateTime> kvp in ObeliskUsers.ToList())
            {
                if (kvp.Value <= DateTime.UtcNow)
                {
                    foreach (Mobile online in kvp.Key.Mobiles)
                    {
                        if (online != null && online.NetState != null)
                        {
                            online.SendMessage(61, "You feel the power of one of the focal points leave your body.");
                        }
                    }
                    ObeliskUsers.Remove(kvp.Key);
                }
            }
        }

        public void DetermineOwner(List<PlayerMobile> players)
        {
            var minaxCount = players.Where(x => x.FactionPlayerState.Faction == Minax.Instance).Count();

            if (minaxCount > 5)
                minaxCount = 5;

            var tbCount = players.Where(x => x.FactionPlayerState.Faction == TrueBritannians.Instance).Count();

            if (tbCount > 5)
                tbCount = 5;

            var CoMCount = players.Where(x => x.FactionPlayerState.Faction == CouncilOfMages.Instance).Count();

            if (CoMCount > 5)
                CoMCount = 5;

            var SLCount = players.Where(x => x.FactionPlayerState.Faction == Shadowlords.Instance).Count();

            if (SLCount > 5)
                SLCount = 5;

            if (string.IsNullOrEmpty(_OwnedName))
            {
                if (SLCount > CoMCount && SLCount > tbCount && SLCount > minaxCount)
                {
                    _OwnedName = "The Shadowlords";
                    OwnerHue = Shadowlords.Instance.Definition.HuePrimary;
                    OwningFaction = Shadowlords.Instance.Definition.Name;
                }
                else if (minaxCount > CoMCount && minaxCount > tbCount && minaxCount > SLCount)
                {
                    _OwnedName = "Minax";
                    OwnerHue = Minax.Instance.Definition.HuePrimary;
                    OwningFaction = Minax.Instance.Definition.Name;
                }
                else if (tbCount > CoMCount && tbCount > minaxCount && tbCount > SLCount)
                {
                    _OwnedName = "The True Britannians";
                    OwnerHue = TrueBritannians.Instance.Definition.HuePrimary;
                    OwningFaction = TrueBritannians.Instance.Definition.Name;
                }
                else if (CoMCount > SLCount && CoMCount > tbCount && CoMCount > minaxCount)
                {
                    _OwnedName = "The Council of Mages";
                    OwnerHue = CouncilOfMages.Instance.Definition.HuePrimary;
                    OwningFaction = CouncilOfMages.Instance.Definition.Name;
                }
            }
            else
            {
                switch (OwningFaction)
                {
                    case "MINAX":
                    {
                        CurrentCharge += minaxCount;

                        CurrentCharge = CurrentCharge - tbCount - SLCount - CoMCount;
                    }
                    break;
                    case "SHADOWLORDS":
                    {
                        CurrentCharge += SLCount;

                        CurrentCharge = CurrentCharge - tbCount - minaxCount - CoMCount;
                    }
                    break;
                    case "COUNCIL OF MAGES":
                    {
                        CurrentCharge += CoMCount;

                        CurrentCharge = CurrentCharge - tbCount - SLCount - minaxCount;
                    }
                    break;
                    case "LORD BRITISH":
                    {
                        CurrentCharge += tbCount;

                        CurrentCharge = CurrentCharge - minaxCount - SLCount - CoMCount;
                    }
                    break;
                }

                if (CurrentCharge >= 30 && !FullyControlled)
                {
                    FullyControlled = true;
                    Hue = OwnerHue;
                    SendFactionMessage(String.Format("{0} has captured the {1}.",_OwnedName, Name));
                }

                if (CurrentCharge > MaxCharge)
                    CurrentCharge = MaxCharge;

                if (CurrentCharge <= 0)
                {
                    SendFactionMessage(String.Format("{0} has lost control of the {1}.", _OwnedName, Name));
                    _OwnedName = null;
                    Hue = 0;
                    OwnerHue = 0;
                    OwningFaction = null;
                    CurrentCharge = 0;
                    FullyControlled = false;
                }
            }

            foreach (var playerMobile in players.ToArray())
            {
                var scoregump = new ProgressBarGump(playerMobile, this).Send<ProgressBarGump>();
            }
        }

        public void SendFactionMessage(string message)
        {
            foreach (Faction faction in Faction.Factions)
            {
                List<PlayerState> members = faction.Members;

                for (int i = 0; i < members.Count; ++i)
                {
                    if (!CentralGump.EnsureProfile(members[i].Mobile as PlayerMobile).FactionPoint)
                    members[i].Mobile.SendMessage(faction.Definition.HueBroadcast,
                        "[{0}]: {1}", faction.Definition.FriendlyName, message);
                }
            }
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(MaxCharge);

            writer.Write(CurrentCharge);

            writer.Write((int) ObeliskType);

            writer.Write(OwningFaction);

            writer.Write(_OwnedName);
        }

        public override void Deserialize(GenericReader reader)
        {
            ObeliskUsers = new Dictionary<Account, DateTime>();
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    MaxCharge = reader.ReadInt();

                    CurrentCharge = reader.ReadInt();

                    ObeliskType = (ObeliskType) reader.ReadInt();

                    OwningFaction = reader.ReadString();

                    _OwnedName = reader.ReadString();
                }
                    break;
            }

            timer = new InternalTimer(this);
            timer.Start();

            FactionObelisks.Register(this);
        }

        private class InternalTimer : Timer
        {
            private readonly FactionObelisk m_Obelisk;
            private DateTime _NextAnnounce;

            public InternalTimer(FactionObelisk obelisk)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(3.0))
            {
                m_Obelisk = obelisk;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Obelisk.RemoveUsers();
                IPooledEnumerable mobs = m_Obelisk.GetMobilesInRange(5);

                List<PlayerMobile> players =
                    (from Mobile mob in mobs
                        where
                            mob is PlayerMobile && mob.AccessLevel >= AccessLevel.Player && mob.Alive &&
                            ((PlayerMobile) mob).FactionPlayerState != null
                        select mob as PlayerMobile).ToList();
                if (players.Count > 0)
                {
                    if (m_Obelisk.CurrentCharge < m_Obelisk.MaxCharge && DateTime.UtcNow > _NextAnnounce)
                    {
                        _NextAnnounce = DateTime.UtcNow + TimeSpan.FromSeconds(30);
                        m_Obelisk.SendFactionMessage(String.Format("The {0} is under assault.", m_Obelisk.Name));
                    }
                    m_Obelisk.DetermineOwner(players);
                }
            }
        }
    }
}