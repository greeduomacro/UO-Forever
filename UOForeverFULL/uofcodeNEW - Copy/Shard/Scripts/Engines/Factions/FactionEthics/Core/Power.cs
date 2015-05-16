#region References

using System;
using Server.Network;
using Server.Spells;

#endregion

namespace Server.Ethics
{
    public abstract class Power : Spell
    {
        protected PowerDefinition m_Definition;

        public PowerDefinition Definition { get { return m_Definition; } }

        public Player EthicCaster;

        public Power(Mobile Caster, SpellInfo info)
            : base(Caster, null, info)
        {}

        public virtual bool CheckInvoke(Player from)
        {
            if (!from.Mobile.CheckAlive() && m_Definition.Power != 100)
            {
                return false;
            }

            if (from.Power < m_Definition.Power)
            {
                from.Mobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                    "You lack the power to invoke this ability.");
                return false;
            }

            return true;
        }

        public override bool ClearHandsOnCast { get { return false; } }
        public override bool RevealOnCast { get { return false; } }

        public override TimeSpan GetCastRecovery()
        {
            return TimeSpan.Zero;
        }

        public override double CastDelayFastScalar { get { return 0; } }

        public override TimeSpan CastDelayBase
        {
            get
            {
                //return TimeSpan.FromSeconds( ((m_Mount.IsDonationItem && RewardSystem.GetRewardLevel( m_Rider ) < 3)? 10.5 : 3.0) );
                return TimeSpan.FromSeconds(2.5);
            }
        }

        public override int GetMana()
        {
            return 0;
        }

        public override bool ConsumeReagents()
        {
            return true;
        }

        public override bool CheckFizzle()
        {
            return true;
        }

        private bool m_Stop;

        public void Stop()
        {
            m_Stop = true;
            Disturb(DisturbType.Hurt, false, false);
        }

        public override bool CheckDisturb(DisturbType type, bool checkFirst, bool resistable)
        {
            if (type == DisturbType.EquipRequest || type == DisturbType.UseRequest || type == DisturbType.Hurt)
            {
                return false;
            }

            return true;
        }

        public override void DoHurtFizzle()
        {
            if (!m_Stop)
            {
                base.DoHurtFizzle();
            }
        }

        public override void DoFizzle()
        {
            if (!m_Stop)
            {
                base.DoFizzle();
            }
        }

        public override void OnCast()
        {}

        public override void OnDisturb(DisturbType type, bool message)
        {
            if (message && !m_Stop)
            {
                Caster.SendMessage(54, "Your ethics spell was disturbed!");
            }
        }

        public abstract void BeginInvoke(Player from);

        public virtual void FinishInvoke(Player from)
        {
            from.Power -= m_Definition.Power;
        }
    }
}