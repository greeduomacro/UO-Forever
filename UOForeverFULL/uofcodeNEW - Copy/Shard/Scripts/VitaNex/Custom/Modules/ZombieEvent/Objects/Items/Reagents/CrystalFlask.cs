#region References

using System;
using System.Drawing;
using System.Linq;
using Server.Engines.ZombieEvent;
using Server.Mobiles;
using Server.Network;
using VitaNex.Notify;

#endregion

namespace Server.Items
{
    public sealed class CureWinner : NotifyGump
    {
        private static void InitSettings(NotifySettings settings)
        {
            settings.Name = "Cure Winner";
            settings.CanIgnore = true;
        }

        public CureWinner(PlayerMobile user, string html)
            : base(user, html)
        {}
    }

    public class CrystalFlask : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Filled { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Bad { get; set; }

        [Constructable]
        public CrystalFlask()
            : base(6212)
        {
            Name = "crystal flask";
            Hue = 1266;
        }

        public CrystalFlask(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (Filled && from is ZombieAvatar)
                {
                    var avatar = from as ZombieAvatar;
                    PlayerZombieProfile profile = ZombieEvent.EnsureProfile(avatar.Owner);
                    from.SendMessage(61, "You drink the contents of the crystal flask.");
                    Consume();
                    if (Bad)
                    {
                        from.SendMessage(54, "Whatever was in the flask wasn't the cure.");
                        from.Poison = Poison.Lethal;
                        from.Hits = 5;
                    }
                    else
                    {
                        from.SendMessage(61, "You are now immune to the Zombie Plague!");
                        from.Hue = 61;
                        ZombieInstance instance = ZombieEvent.GetInstance();
                        if (instance != null)
                        {
                            if (instance.CureWinner == null)
                            {
                                NetState.Instances.Where(ns => ns != null && ns.Mobile is PlayerMobile)
                                    .ForEach(
                                        ns =>
                                            ns.Mobile.SendNotification<CureWinner>(
                                                profile.Owner.RawName +
                                                " was the first person to innoculate themselves against the Zombie Plague!",
                                                true, 1.0, 30.0, Color.LawnGreen));

                                instance.CureWinner = avatar.Owner;
                                profile.OverallScore += 1500;
                                profile.SpendablePoints += 1500;
                            }
                            else
                            {
                                instance.CureCompleters.Add(avatar.Owner);
                                profile.OverallScore += 800;
                                profile.SpendablePoints += 800;
                            }
                        }
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write(Filled);
            writer.Write(Bad);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Filled = reader.ReadBool();
            Bad = reader.ReadBool();
        }
    }
}