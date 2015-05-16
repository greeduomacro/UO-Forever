#region References

using System;
using Server.Engines.ZombieEvent;
using Server.Items;
using Server.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("a corpse")]
    public class ZombieAvatar : BaseCreature
    {
        private int m_OwningSerial = -1;
        public int OwningSerial { get { return m_OwningSerial; } set { m_OwningSerial = value; } }

        public PlayerMobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ZombieSwingTime { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SuicideTimer { get; set; }

        [Constructable]
        public ZombieAvatar(PlayerMobile owner)
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.25, 0.8)
        {
            SpeechHue = Utility.RandomDyedHue();
            Name = "a human";
            Hue = Utility.RandomSkinHue();
            if (Utility.RandomBool())
            {
                Body = 0x191;
                Female = true;
            }
            else
            {
                Body = 0x190;
            }

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            VirtualArmor = 0;
            BardImmuneCustom = true;
            WeaponDamage = true;
            TakesNormalDamage = true;
            Pseu_AllowInterrupts = true;

            Pseu_SpellDelay = TimeSpan.Zero;
            ClearHandsOnCast = true;
            PowerWords = true;

            Fame = 0;
            Karma = 0;
            Owner = owner;

            FightMode = FightMode.Evil;
            Pseu_SpellBookRequired = true;
            Pseu_ConsumeReagents = true;
        }

        public ZombieAvatar(Serial serial)
            : base(serial)
        {}

        public override void OnNetStateChanged()
        {
            if (NetState == null)
            {
                if (ZombieEvent.GetInstance() != null && Owner != null)
                {
                    var profile = ZombieEvent.EnsureProfile(Owner);

                    profile.DisconnectTime = DateTime.UtcNow;

                    if (profile.Active)
                    {
                        profile.Active = false;
                    }
                }
            }
            base.OnNetStateChanged();
        }

        public override void OnDamage(int amount, Mobile @from, bool willKill)
        {
            if (Owner != null)
            {
                PlayerZombieProfile profile = ZombieEvent.EnsureProfile(Owner);
                ZombieInstance zevent = ZombieEvent.GetInstance();

                if (zevent != null && profile.LeaveEventTimer != null && profile.LeaveEventTimer.Running)
                {
                    profile.LeaveEventTimer.Stop();
                    SendMessage(54,
                        "Your attempt to leave was interrupted.  Either initate another leave attempt or close and reopen your client to log instantly.");
                }
            }
            base.OnDamage(amount, @from, willKill);
        }

        public override bool OnBeforeDeath()
        {
            if (Owner != null)
            {
                PlayerZombieProfile profile = ZombieEvent.EnsureProfile(Owner);
                ZombieInstance zevent = ZombieEvent.GetInstance();

                if (NetState == null)
                {
                    Blessed = true;
                    Hidden = true;
                    CantWalk = true;
                    IgnoreMobiles = true;
                    profile.ZombieSavePoint = Point3D.Zero;
                }

                if (zevent != null)
                {
                    return zevent.HandleAvatarDeath(profile, LastKiller);
                }
            }

            return OnBeforeDeath();
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (Map == Map.ZombieLand && m is BaseCreature)
            {
                return false;
            }

            return base.OnMoveOver(m);
        }

        public override void DoSpeech(string text, int[] keywords, MessageType type, int hue)
        {
            if ((text.ToLower() == "i don't want to live" || text.ToLower() == "i dont want to live"))
            {
                if (SuicideTimer <= DateTime.UtcNow)
                {
                    SuicideTimer = DateTime.UtcNow + TimeSpan.FromMinutes(1);
                    Kill();
                }
                else
                {
                    SendMessage(54, "You must wait 1 minute between suicides.");
                }
            }

            var wep = Weapon as BaseWeapon;
            if (wep != null && DateTime.UtcNow > ZombieSwingTime)
            {
                switch (text)
                {
                    case "!9":
                    {
                        ZombieEvent.ZombieSwingDirection(this, Direction.North);
                        ZombieSwingTime = DateTime.UtcNow +
                                          TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
                        return;
                    }
                    case "!6":
                    {
                        ZombieEvent.ZombieSwingDirection(this, Direction.Right);
                        ZombieSwingTime = DateTime.UtcNow +
                                          TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
                        return;
                    }
                    case "!3":
                    {
                        ZombieEvent.ZombieSwingDirection(this, Direction.East);
                        ZombieSwingTime = DateTime.UtcNow +
                                          TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
                        return;
                    }
                    case "!2":
                    {
                        ZombieEvent.ZombieSwingDirection(this, Direction.Down);
                        ZombieSwingTime = DateTime.UtcNow +
                                          TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
                        return;
                    }
                    case "!1":
                    {
                        ZombieEvent.ZombieSwingDirection(this, Direction.South);
                        ZombieSwingTime = DateTime.UtcNow + TimeSpan.FromSeconds(0.6 * wep.DamageMax / 24);
                        return;
                    }
                    case "!4":
                    {
                        ZombieEvent.ZombieSwingDirection(this, Direction.Left);
                        ZombieSwingTime = DateTime.UtcNow +
                                          TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
                        return;
                    }
                    case "!7":
                    {
                        ZombieEvent.ZombieSwingDirection(this, Direction.West);
                        ZombieSwingTime = DateTime.UtcNow +
                                          TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
                        return;
                    }
                    case "!8":
                    {
                        ZombieEvent.ZombieSwingDirection(this, Direction.Up);
                        ZombieSwingTime = DateTime.UtcNow +
                                          TimeSpan.FromSeconds(0.6 * Math.Pow(wep.DamageMax / 48.0, 0.5));
                        return;
                    }
                }
            }
            else if ((text == "!2" || text == "!3" || text == "!4" || text == "!1" || text == "!6" || text == "!7" ||
                      text == "!8" || text == "!9") && DateTime.UtcNow <= ZombieSwingTime)
            {
                SendMessage(38, "You fumble your weapon attempting to swing it too quickly.");
                ZombieSwingTime = DateTime.UtcNow +
                                  TimeSpan.FromSeconds(0.8 * Math.Pow(wep.DamageMax / 48.0, 0.5));
                return;
            }

            base.DoSpeech(text, keywords, type, hue);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 1); // version 
            writer.Write((int) m_OwningSerial);
            writer.Write(Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version >= 1)
            {
                m_OwningSerial = reader.ReadInt();
                Owner = reader.ReadMobile<PlayerMobile>();
            }
        }
    }
}