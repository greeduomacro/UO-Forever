#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Commands;
using Server.Mobiles;
using VitaNex.Modules.TrashCollection;

#endregion

namespace Server.Engines.Conquests
{
    public static partial class Conquests
    {
        /*public static event Action<ConquestProgressEventArgs> OnProgress;
        public static event Action<ConquestTierCompletedEventArgs> OnTierCompleted;
        public static event Action<ConquestCompletedEventArgs> OnCompleted;

        public static void InvokeProgress(ConquestProgressEventArgs e)
        {
            if (e != null && OnProgress != null)
            {
                OnProgress(e);
            }
        }

        public static void InvokeTierCompleted(ConquestTierCompletedEventArgs e)
        {
            if (e != null && OnTierCompleted != null)
            {
                OnTierCompleted(e);
            }
        }

        public static void InvokeCompleted(ConquestCompletedContainer e)
        {
            if (e != null && OnCompleted != null)
            {
                OnCompleted(e);
            }
        }

        private static void SetEvents()
        {
            EventSink.PlayerDeath += HandlePlayerDeath;
            EventSink.Speech += HandleSpeech;
            //EventSink.Login += HandleLogin;
            //EventSink.Logout += HandleLogout;
            EventSink.Movement += HandleMovement;
            EventSink.CastSpellRequest += HandleCastSpellRequest;
            EventSink.Command += HandleCommand;
            //EventSink.ClientVersionReceived += HandleClientVersionReceived;

            EventSink.OnConsume += HandleOnItemConsume;
            EventSink.OnItemUse += HandleOnItemUse;

            TrashProfile.TokensReceived += HandleTrashTokens;

            OnProgress += HandleProgress;
            OnTierCompleted += HandleTierCompleted;
            OnCompleted += HandleCompleted;
        }

        private static void UnsetEvents()
        {
            EventSink.PlayerDeath -= HandlePlayerDeath;
            EventSink.Speech -= HandleSpeech;
            //EventSink.Login -= HandleLogin;
            //EventSink.Logout -= HandleLogout;
            EventSink.Movement -= HandleMovement;
            EventSink.CastSpellRequest -= HandleCastSpellRequest;
            EventSink.Command -= HandleCommand;
            //EventSink.ClientVersionReceived -= HandleClientVersionReceived;

            EventSink.OnConsume -= HandleOnItemConsume;
            EventSink.OnItemUse -= HandleOnItemUse;

            TrashProfile.TokensReceived -= HandleTrashTokens;

            OnProgress -= HandleProgress;
            OnTierCompleted -= HandleTierCompleted;
            OnCompleted -= HandleCompleted;
        }*/

        public static void HandleCreatureDeath(CreatureConquestContainer e)
        {
            if (e == null || e.Creature == null || !(e.Creature is BaseCreature))
            {
                return;
            }

            var creature = (BaseCreature) e.Creature;

            List<PlayerMobile> team =
                creature.DamageEntries.Not(de => de.DamageGiven <= 0 || de.HasExpired)
                    .Select(de => de.Damager)
                    .OfType<PlayerMobile>()
                    .ToList();

            if (e.Killer != null)
            {
                var killer = e.Killer as PlayerMobile;
                if (killer == null && e.Killer is BaseCreature)
                {
                    killer = ((BaseCreature) e.Killer).GetMaster<PlayerMobile>();
                }

                if (killer == null)
                {
                    killer = creature.GetLastKiller<PlayerMobile>();
                }

                if (killer != null && !team.Contains(killer))
                {
                    team.Add(killer);
                }
            }

            team.ForEach(p => CheckProgress<CreatureKillConquest>(p, new CreatureConquestContainer(creature, p, e.Corpse)));
        }

        public static void HandlePlayerDeath(PlayerConquestContainer e)
        {
            if (e == null || e.Mobile == null || !(e.Mobile is PlayerMobile))
            {
                return;
            }

            var player = (PlayerMobile) e.Mobile;

            CheckProgress<PlayerDeathConquest>(player, e);

            List<PlayerMobile> team =
                player.DamageEntries.Not(de => de.DamageGiven <= 0 || de.HasExpired)
                    .Select(de => de.Damager)
                    .OfType<PlayerMobile>()
                    .ToList();

            var killer = e.Killer as PlayerMobile;

            if (killer == null && e.Killer is BaseCreature)
            {
                killer = ((BaseCreature) e.Killer).GetMaster<PlayerMobile>();
            }

            if (killer == null)
            {
                killer = player.GetLastKiller<PlayerMobile>();
            }

            if (killer != null && !team.Contains(killer))
            {
                team.Add(killer);
            }

            team.ForEach(k => CheckProgress<PlayerKillConquest>(k, new PlayerConquestContainer(player, k, e.Corpse)));
        }

        public static void HandleSpeech(SpeechConquestContainer e)
        {
            if (e != null && e.Mobile != null && e.Mobile is PlayerMobile)
            {
                CheckProgress<SpeechConquest>((PlayerMobile) e.Mobile, e);
            }
        }

        /*private static void HandleLogin(LoginEventArgs e)
		{
			if (e == null || e.Mobile == null || !(e.Mobile is PlayerMobile))
			{
				return;
			}

			CheckProgress<LoginConquest>((PlayerMobile)e.Mobile, e);
			CheckProgress<AccountAgeConquest>((PlayerMobile)e.Mobile, e.Mobile.Account);
		}

		private static void HandleLogout(LogoutEventArgs e)
		{
			if (e != null && e.Mobile != null && e.Mobile is PlayerMobile)
			{
				CheckProgress<LogoutConquest>((PlayerMobile)e.Mobile, e);
			}
		}*/

        public static void HandleMovement(MovementConquestContainer e)
        {
            if (e != null && e.Mobile != null && e.Mobile is PlayerMobile)
            {
                CheckProgress<MoveConquest>((PlayerMobile) e.Mobile, e);
                CheckProgress<LocationConquest>((PlayerMobile) e.Mobile, e);
            }
        }

        /*private static void HandleCastSpellRequest(CastSpellRequestEventArgs e)
        {
            if (e != null && e.Mobile != null && e.Mobile is PlayerMobile)
            {
                CheckProgress<CastSpellConquest>((PlayerMobile) e.Mobile, e);
            }
        }

        private static void HandleCommand(CommandEventArgs e)
        {
            if (e != null && e.Mobile != null && e.Mobile is PlayerMobile)
            {
                CheckProgress<CommandConquest>((PlayerMobile) e.Mobile, e);
            }
        }*/

        /*private static void HandleClientVersionReceived(ClientVersionReceivedArgs e)
		{
			if (e != null && e.State != null && e.State.Mobile is PlayerMobile)
			{
				CheckProgress<ClientVersionConquest>((PlayerMobile)e.State.Mobile, e);
			}
		}*/

        public static void HandleOnItemConsume(OnConsumeEventArgs e)
        {
            if (e != null && e.Consumed != null && e.Consumer is PlayerMobile)
            {
                CheckProgress<ItemConquest>((PlayerMobile) e.Consumer, e.Consumed);

                //CheckProgress<ConsumeItemConquest>((PlayerMobile)e.Consumer, e);
            }
        }

        public static void HandleOnItemUse(OnItemUseEventArgs e)
        {
            if (e != null && e.Item != null && e.User is PlayerMobile)
            {
                //CheckProgress<UseItemConquest>((PlayerMobile)e.User, e);
            }
        }

        public static void HandleTrashTokens(TrashProfile p, TrashToken t)
        {
            if (p != null && p.Owner != null && p.Owner is PlayerMobile)
            {
                CheckProgress<ItemConquest>((PlayerMobile) p.Owner, t);
            }
        }

        public static void HandleProgress(ConquestProgressContainer e)
        {
            if (e != null && e.State != null && e.State.User != null)
            {
                CheckProgress<ConquestProgressConquest>(e.State.User, e);
            }
            else if (e != null && e.State != null)
            {
                CheckProgress<ConquestProgressConquest>(e.State.Owner, e);
            }
        }

        public static void HandleTierCompleted(ConquestTierCompletedContainer e)
        {
            if (e != null && e.State != null && e.State.User != null)
            {
                CheckProgress<ConquestTierCompletedConquest>(e.State.User, e);
            }
            else if (e != null && e.State != null)
            {
                CheckProgress<ConquestTierCompletedConquest>(e.State.Owner, e);
            }
        }

        public static void HandleCompleted(ConquestCompletedContainer e)
        {
            if (e != null && e.State != null && e.State.User != null)
            {
                CheckProgress<ConquestCompletedConquest>(e.State.User, e);
            }
            else if (e != null && e.State != null)
            {
                CheckProgress<ConquestCompletedConquest>(e.State.Owner, e);
            }
        }
    }

    /*public abstract class ConquestStateEventArgs : EventArgs
    {
        public ConquestState State { get; private set; }

        public ConquestStateEventArgs(ConquestState s)
        {
            State = s;
        }
    }

    public sealed class ConquestProgressEventArgs : ConquestStateEventArgs
    {
        public int Offset { get; set; }

        public ConquestProgressEventArgs(ConquestState s, int offset)
            : base(s)
        {
            Offset = offset;
        }
    }

    public sealed class ConquestTierCompletedEventArgs : ConquestStateEventArgs
    {
        public int Tier { get; set; }

        public ConquestTierCompletedEventArgs(ConquestState s, int tier)
            : base(s)
        {
            Tier = tier;
        }
    }

    public sealed class ConquestCompletedEventArgs : ConquestStateEventArgs
    {
        public ConquestCompletedEventArgs(ConquestState s)
            : base(s)
        {}
    }*/
}