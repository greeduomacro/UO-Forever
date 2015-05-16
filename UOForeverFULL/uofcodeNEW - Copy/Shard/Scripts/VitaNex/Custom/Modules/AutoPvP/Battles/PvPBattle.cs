#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Commands;
using Server.Engines.CentralGump;
using Server.Engines.EventScores;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.PvPTemplates;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using Server.Spells.Third;
using Server.Twitch;
using VitaNex.Items;
using VitaNex.SuperGumps;

#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
    public class UOF_PvPBattle : PvPBattle, IUOFBattle
    {
        [CommandProperty(AutoPvP.Access)]
        public virtual bool UseTemplates { get; set; }

        [CommandProperty(AutoPvP.Access)]
        public virtual bool UseTemplateEquipment { get; set; }

        [CommandProperty(AutoPvP.Access)]
        public virtual bool NoConsume { get; set; }

        [CommandProperty(AutoPvP.Access)]
        public virtual bool IncognitoMode { get; set; }

        [CommandProperty(AutoPvP.Access)]
        public virtual bool GiveTrophies { get; set; }

        [CommandProperty(AutoPvP.Access)]
        public virtual bool GiveRankTrophies { get; set; }

        [CommandProperty(AutoPvP.Access)]
        public virtual bool PlayerStarted { get; set; }

        [CommandProperty(AutoPvP.Access)]
        public virtual bool ThrowableMode { get; set; }

        [CommandProperty(AutoPvP.Access)]
        public virtual int ThrowableID { get; set; }

        [CommandProperty(AutoPvP.Access)]
        public virtual int ThrowableHue { get; set; }

        public Dictionary<PlayerMobile, PvPTeam> TeamStats { get; private set; }

        public PvPTeam[] Winners { get; private set; }

        private BattleWell _BattleWell;

        private Dictionary<PlayerMobile, ThrowableAxe> _Axes;

        [CommandProperty(AutoPvP.Access)]
        public BattleWell BattleWell
        {
            get { return _BattleWell; }
            set
            {
                if (_BattleWell != null)
                {
                    _BattleWell.Battle = null;
                }

                _BattleWell = value;

                if (_BattleWell != null)
                {
                    _BattleWell.Battle = this;
                }
            }
        }

        [CommandProperty(AutoPvP.Access)]
        public MapPoint BattleWellLoc { get; set; }

        protected override void EnsureConstructDefaults()
        {
            base.EnsureConstructDefaults();

            TeamStats = new Dictionary<PlayerMobile, PvPTeam>();

            BattleWellLoc = MapPoint.Empty;
        }

        public UOF_PvPBattle()
        {
            Ranked = true;

            Options.Rules.AllowBeneficial = true;
            Options.Rules.AllowHarmful = true;
            Options.Rules.AllowHousing = false;
            Options.Rules.AllowPets = false;
            Options.Rules.AllowSpawn = false;
            Options.Rules.AllowSpeech = true;
            Options.Rules.CanBeDamaged = true;
            Options.Rules.CanDamageEnemyTeam = true;
            Options.Rules.CanDamageOwnTeam = false;
            Options.Rules.CanDie = false;
            Options.Rules.CanHeal = true;
            Options.Rules.CanHealEnemyTeam = false;
            Options.Rules.CanHealOwnTeam = true;
            Options.Rules.CanMount = false;
            Options.Rules.CanResurrect = false;
            Options.Rules.CanUseStuckMenu = false;
            Options.Rules.CanMountEthereal = false;
            Options.Rules.CanMoveThrough = false;
        }

        public UOF_PvPBattle(GenericReader reader)
            : base(reader)
        {}

        protected override void RegisterSubCommands()
        {
            base.RegisterSubCommands();

            RegisterSubCommand(
                "templates",
                e =>
                {
                    PvPTemplates.DisplayManagerGump(e.Mobile);
                    return true;
                },
                "Opens your PvP template manager.",
                "",
                AccessLevel.Player);

            RegisterSubCommand(
                "scoreboard",
                state =>
                {
                    if (state == null || state.Mobile == null || state.Mobile.Deleted)
                    {
                        return false;
                    }

                    SuperGump g = CreateResultsGump(state.Mobile).Send();

                    return true;
                },
                "Displays the battles scores.",
                "",
                AccessLevel.Player);

            RegisterSubCommandAlias("score");
        }


        public override bool AddTeam(string name, int minCapacity, int capacity, int color)
        {
            return AddTeam(new UOF_PvPTeam(this, name, minCapacity, capacity, color));
        }

        public override bool AddTeam(PvPTeam team)
        {
            return team != null && !team.Deleted &&
                   (team is UOF_PvPTeam
                       ? base.AddTeam(team)
                       : AddTeam(team.Name, team.MinCapacity, team.MinCapacity, team.Color));
        }

        public override bool Validate(Mobile viewer, List<string> errors, bool pop = true)
        {
            if (!base.Validate(viewer, errors, pop) && pop)
            {
                return false;
            }

            if (Teams.Any(t => !(t is UOF_PvPTeam)))
            {
                errors.Add("One or more teams are not of the UOF_PvPTeam type.");
                errors.Add("[Options] -> [View Teams]");

                if (pop)
                {
                    return false;
                }
            }

            return true;
        }

        public override void InvalidateGates()
        {
            base.InvalidateGates();

            InvalidateBattleWell();
        }

        public virtual void InvalidateBattleWell()
        {
            if (State == PvPBattleState.Internal || BattleWellLoc.Internal || BattleWellLoc.Zero)
            {
                if (BattleWell != null)
                {
                    BattleWell.Delete();
                    BattleWell = null;
                }

                return;
            }

            if (BattleWell == null || BattleWell.Deleted)
            {
                BattleWell = new BattleWell(this);

                if (!BattleWellLoc.MoveToWorld(BattleWell))
                {
                    BattleWell.MoveToWorld(BattleWellLoc, BattleWellLoc);
                }
            }

            if (BattleWell.Battle == null)
            {
                BattleWell.Battle = this;
            }
        }

        public override bool CanSendInvite(PlayerMobile pm)
        {
            return base.CanSendInvite(pm) && !ActionCams.IsCamera(pm);
        }

        public override bool CheckSpeech(SpeechEventArgs args)
        {
            if (Insensitive.Equals(args.Speech, "i wish to duel"))
            {
                return false;
            }

            return base.CheckSpeech(args);
        }

        public override void OnDismounted(Mobile m, IMount mount)
        {
            if (m == null || m.Deleted)
            {
                return;
            }

            if (mount is BaseCreature)
            {
                InvalidateStray((BaseCreature) mount);
            }

            m.SendMessage("You have been dismounted.  If your mount was not an ethereal, it has been stabled.");
        }

        protected override void OnSpellCastAccept(Mobile caster, ISpell spell)
        {
            base.OnSpellCastAccept(caster, spell);

            var pm = caster as PlayerMobile;

            if (pm == null)
            {
                return;
            }

            if (IsParticipant(pm) && spell is Spell && (spell is WallOfStoneSpell || spell is EnergyFieldSpell))
            {
                EnsureStatistics(pm)["Walls Cast"]++;
            }
        }

        protected override void OnEjected(PlayerMobile pm)
        {
            if (IncognitoMode)
            {
                pm.SetHairMods(-1, -1);

                pm.BodyMod = 0;
                pm.HueMod = -1;
                pm.NameMod = null;
                pm.EndAction(typeof(IncognitoSpell));

                BaseArmor.ValidateMobile(pm);
                BaseClothing.ValidateMobile(pm);
            }

            base.OnEjected(pm);
        }

        public void IncogMode(PlayerMobile pm)
        {
            string originalName = pm.Name;
            pm.HueMod = pm.Race.RandomSkinHue();
            pm.NameMod = pm.Female ? NameList.RandomName("female") : NameList.RandomName("male");

            LoggingCustom.LogDisguise(DateTime.Now + "\t" + originalName + "\t" + pm.NameMod);

            if (pm.Race != null)
            {
                pm.SetHairMods(pm.Race.RandomHair(pm.Female), pm.Race.RandomFacialHair(pm.Female));
                pm.HairHue = pm.Race.RandomHairHue();
                pm.FacialHairHue = pm.Race.RandomHairHue();
            }

            pm.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
            pm.PlaySound(0x3BD);

            BaseArmor.ValidateMobile(pm);
            BaseClothing.ValidateMobile(pm);

            //BuffInfo.AddBuff( Caster, new BuffInfo( BuffIcon.Incognito, 1075819, length, Caster ) );
        }

        public override void OnTeamMemberAdded(PvPTeam team, PlayerMobile pm)
        {
            if (TeamStats.ContainsKey(pm))
            {
                TeamStats[pm] = team;
            }
            else
            {
                TeamStats.Add(pm, team);
            }

            base.OnTeamMemberAdded(team, pm);

            if (UseTemplates)
            {
                PvPTemplates.FetchProfile(pm).ApplyDelta(UseTemplateEquipment);
            }

            if (IncognitoMode)
            {
                IncogMode(pm);
            }

            if (ThrowableMode)
            {
                if (_Axes == null)
                {
                    _Axes = new Dictionary<PlayerMobile, ThrowableAxe>();
                }
                if (ThrowableID == 0)
                {
                    ThrowableID = 0x13FB;
                }
                if (ThrowableHue == 0)
                {
                    ThrowableHue = 1161;        
                }
                var Axe = new ThrowableAxe
                {
                    Movable = false,
                    ItemID = ThrowableID,
                };
                if (Utility.RandomDouble() <= 0.1)
                {
                    Axe.Name = "Stone Chair";
                    Axe.ItemID = 4632;
                }
                Axe.EffectID = Axe.ItemID;
                Axe.Hue = ThrowableHue;
                Axe.EffectHue = Axe.Hue-1;
                Axe.Stackable = false;
                if (_Axes != null && !_Axes.ContainsKey(pm))
                {
                    _Axes.Add(pm, Axe);
                }
                if (pm.Backpack != null)
                {
                    pm.AddToBackpack(Axe);
                }
            }
        }

        public override bool CheckAllowHarmful(Mobile m, Mobile target)
        {
            if (State != PvPBattleState.Internal && !Hidden && ThrowableMode)
            {
                return false;
            }
            return base.CheckAllowHarmful(m, target);
        }

        protected override void OnAllowHarmfulDeny(Mobile m, Mobile target)
        {
            if (m != null && !m.Deleted && target != null && !target.Deleted && m != target && !ThrowableMode)
            {
                m.SendMessage("You can not perform harmful actions on your target.");
            }
        }

        public override void OnTeamMemberRemoved(PvPTeam team, PlayerMobile pm)
        {
            base.OnTeamMemberRemoved(team, pm);

            if (IncognitoMode)
            {
                pm.SetHairMods(-1, -1);

                pm.BodyMod = 0;
                pm.HueMod = -1;
                pm.NameMod = null;
                pm.EndAction(typeof(IncognitoSpell));

                BaseArmor.ValidateMobile(pm);
                BaseClothing.ValidateMobile(pm);
            }

            if (ThrowableMode)
            {
                if (_Axes != null && _Axes.ContainsKey(pm))
                {
                    ThrowableAxe axe = _Axes[pm];
                    if (axe != null)
                    {
                        _Axes.Remove(pm);
                        axe.Delete();
                    }
                }
            }

            PvPTemplates.FetchProfile(pm).ClearDelta();
        }

        public override void OnAfterTeamMemberDeath(PvPTeam team, PlayerMobile pm)
        {
            PvPTemplates.FetchProfile(pm).ClearDelta();

            base.OnAfterTeamMemberDeath(team, pm);
        }

        public override void OnAfterTeamMemberResurrected(PvPTeam team, PlayerMobile pm)
        {
            if (UseTemplates)
            {
                PvPTemplates.FetchProfile(pm).ApplyDelta(UseTemplateEquipment);
            }

            base.OnAfterTeamMemberResurrected(team, pm);
        }

        public override void OnTeamWin(PvPTeam team)
        {
            if (team != null && !team.Deleted)
            {
                OnTeamWin(team, GetTeamRank(team));
            }

            base.OnTeamWin(team);
        }

        protected virtual void OnTeamWin(PvPTeam team, int rank)
        {
            if (team == null || team.Deleted || rank < 0)
            {
                return;
            }

            if (GiveRankTrophies)
            {
                team.ForEachMember(m => AwardRankTrophy(m, rank));
            }
        }

        public override void OnTeamLose(PvPTeam team)
        {
            if (team != null && !team.Deleted)
            {
                OnTeamLose(team, GetTeamRank(team));
            }

            base.OnTeamLose(team);
        }

        protected virtual void OnTeamLose(PvPTeam team, int rank)
        {
            if (team == null || team.Deleted || rank < 0)
            {
                return;
            }

            if (GiveRankTrophies)
            {
                team.ForEachMember(m => AwardRankTrophy(m, rank));
            }
        }

        protected override void OnReset()
        {
            base.OnReset();

            Winners = new PvPTeam[0];

            TeamStats.Clear();
        }

        protected override void OnBattleOpened(DateTime when)
        {
            base.OnBattleOpened(when);

            if (PlayerStarted)
            {
                GiveTrophies = true;
            }

            foreach (GlobalJoinGump g in
                NetState.Instances.Select(s => s.Mobile)
                    .OfType<PlayerMobile>()
                    .Not(m => IsQueued(m) || ActionCams.IsCamera(m) || CentralGump.EnsureProfile(m).IgnoreBattles)
                    .Select(CreateJoinGump)
                    .Where(g => g != null))
            {
                g.Send();
            }
        }

        protected override void OnBattlePreparing(DateTime when)
        {
            base.OnBattlePreparing(when);

            foreach (GlobalJoinGump g in
                NetState.Instances.Select(s => s.Mobile)
                    .OfType<PlayerMobile>()
                    .Not(m => IsQueued(m) || IsParticipant(m) || ActionCams.IsCamera(m) || CentralGump.EnsureProfile(m).IgnoreBattles)
                    .Select(CreateJoinGump)
                    .Where(g => g != null))
            {
                g.Send();
            }
        }

        protected override void OnBattleStarted(DateTime when)
        {
            base.OnBattleStarted(when);

            ActionCams.ForcetoBattleCams();
        }

        protected override void OnBattleEnded(DateTime when)
        {
            Winners = GetWinningTeams().ToArray();

            if (StatisticsCache.Count > 0)
            {
                /*foreach (PlayerMobile player in StatisticsCache.Keys)
                {
                    PlayerEventScoreProfile eventprofile = EventScores.EnsureProfile(player);
                    eventprofile.AddBattle(Name, (int) StatisticsCache[player].PointsGained, player);
                }*/

                foreach (BattleResultsGump g in
                    StatisticsCache.Keys.With(Spectators).Select(CreateResultsGump).Where(g => g != null))
                {
                    g.Send();
                }

                SendResultsToCameras();
            }

            if (GiveTrophies)
            {
                AwardTrophies();

                if (PlayerStarted)
                {
                    GiveTrophies = false;
                }
            }

            base.OnBattleEnded(when);

            Hidden = true;

            if (PlayerStarted)
            {
                //Hidden = !IgnoreCapacity && (Schedule == null || !Schedule.Enabled || !Schedule.Running);

                //Hidden will probably always be false anyways.  I cannot forsee a case where it will be playerstarted and not be hidden

                PlayerStarted = false;
            }

            if (ThrowableMode)
            {
                if (_Axes != null)
                {
                    foreach (KeyValuePair<PlayerMobile, ThrowableAxe> kvp in _Axes.Where(kvp => kvp.Value != null))
                    {
                        kvp.Value.Delete();
                    }
                    _Axes.Clear();
                }
            }

            ActionCams.ForceToNormalCams();
        }

        protected override void OnBattleCancelled(DateTime when)
        {
            if (GiveTrophies && PlayerStarted)
            {
                GiveTrophies = false;
            }

            base.OnBattleCancelled(when);

            Hidden = true;

            if (PlayerStarted)
            {
                //Hidden = !IgnoreCapacity && (Schedule == null || !Schedule.Enabled || !Schedule.Running);

                PlayerStarted = false;
            }

            ActionCams.ForceToNormalCams();
        }

        public virtual void SendResultsToCameras()
        {
            foreach (BattleResultsGump g in
                ActionCams.DeathCamsEvents.Keys.Where(m => BattleRegion.Contains(m.Location, m.Map))
                    .Select(CreateResultsGump))
            {
                g.Send();

                Timer.DelayCall(TimeSpan.FromSeconds(30.0), g.Close, true);
            }
        }

        protected override void OnDeleted()
        {
            if (BattleWell != null)
            {
                BattleWell.Delete();
                BattleWell = null;
            }

            base.OnDeleted();
        }

        public virtual BattleResultsGump CreateResultsGump(PlayerMobile pm)
        {
            return new BattleResultsGump(pm, StatisticsCache, TeamStats, Winners);
        }

        public virtual GlobalJoinGump CreateJoinGump(PlayerMobile pm)
        {
            return new GlobalJoinGump(pm, this);
        }

        public virtual void AwardTrophies()
        {
            PlayerMobile topkiller = null;
            long topkills = 0;

            PlayerMobile topdamager = null;
            long topdamage = 0;

            PlayerMobile tophealer = null;
            long topheals = 0;

            foreach (KeyValuePair<PlayerMobile, PvPProfileHistoryEntry> kv in StatisticsCache)
            {
                if (kv.Value.Kills > topkills)
                {
                    topkills = kv.Value.Kills;
                    topkiller = kv.Key;
                }

                if (kv.Value.DamageDone > topdamage)
                {
                    topdamage = kv.Value.DamageDone;
                    topdamager = kv.Key;
                }

                if (kv.Value.HealingDone > topheals)
                {
                    topheals = kv.Value.HealingDone;
                    tophealer = kv.Key;
                }
            }

            if (topkiller != null)
            {
                topkiller.SendMessage(54, "You had the top kills in the {0}.", Name);
                topkiller.PublicOverheadMessage(MessageType.Label, 54, true, topkiller.Name + ": Top Kills!");

                BankBox bank = topkiller.FindBank(Map.Expansion);

                if (bank != null)
                {
                    bank.DropItem(
                        new BattlesTrophy(Name + " - Top Kills: " + topkills, TrophyType.Kills)
                        {
                            Owner = topkiller
                        });

                    topkiller.SendMessage(54, "A trophy has been placed in your bankbox.");
                }
            }

            if (topdamager != null)
            {
                topdamager.SendMessage(54, "You had the top damage done in the {0}.", Name);
                topdamager.PublicOverheadMessage(MessageType.Label, 54, true, topdamager.Name + ": Top Damage!");

                BankBox bank = topdamager.FindBank(Map.Expansion);

                if (bank != null)
                {
                    bank.DropItem(
                        new BattlesTrophy(Name + " - Top Damage: " + topdamage, TrophyType.Damage)
                        {
                            Owner = topdamager
                        });

                    topdamager.SendMessage(54, "A trophy has been placed in your bankbox.");
                }
            }

            if (tophealer != null)
            {
                tophealer.SendMessage(54, "You had the top healing done in the {0}.", Name);
                tophealer.PublicOverheadMessage(MessageType.Label, 54, true, tophealer.Name + ": Top Healing!");

                BankBox bank = tophealer.FindBank(Map.Expansion);

                if (bank != null)
                {
                    bank.DropItem(
                        new BattlesTrophy(Name + " - Top Heals: " + topheals, TrophyType.Healing)
                        {
                            Owner = tophealer
                        });

                    tophealer.SendMessage(54, "A trophy has been placed in your bankbox.");
                }
            }
        }

        public virtual void AwardRankTrophy(PlayerMobile pm, int rank)
        {
            switch (rank)
            {
                case 0:
                {
                    pm.SendMessage(54, "You took first place in the {0}.", Name);
                    pm.PublicOverheadMessage(MessageType.Label, 54, true, "FIRST PLACE!");

                    BankBox bank = pm.FindBank(Map.Expansion);

                    if (bank != null)
                    {
                        bank.DropItem(
                            new BattlesTrophy(Name + " - First Place", TrophyType.First)
                            {
                                Owner = pm
                            });

                        pm.SendMessage(54, "A trophy has been placed in your bankbox.");
                    }
                }
                    break;
                case 1:
                {
                    pm.SendMessage(54, "You took second place in the {0}.", Name);
                    pm.PublicOverheadMessage(MessageType.Label, 54, true, "SECOND PLACE!");

                    BankBox bank = pm.FindBank(Map.Expansion);

                    if (bank != null)
                    {
                        bank.DropItem(
                            new BattlesTrophy(Name + " - Second Place", TrophyType.Second)
                            {
                                Owner = pm
                            });

                        pm.SendMessage(54, "A trophy has been placed in your bankbox.");
                    }
                }
                    break;
                case 2:
                {
                    pm.SendMessage(54, "You took third place in the {0}.", Name);
                    pm.PublicOverheadMessage(MessageType.Label, 54, true, "THIRD PLACE!");

                    BankBox bank = pm.FindBank(Map.Expansion);

                    if (bank != null)
                    {
                        bank.DropItem(
                            new BattlesTrophy(Name + " - Third Place", TrophyType.Third)
                            {
                                Owner = pm
                            });

                        pm.SendMessage(54, "A trophy has been placed in your bankbox.");
                    }
                }
                    break;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(4);

            if (_Axes == null)
            {
                _Axes = new Dictionary<PlayerMobile, ThrowableAxe>();
            }

            switch (version)
            {
                case 4:
                    {
                        writer.Write(ThrowableID);
                        writer.Write(ThrowableHue);
                    }
                    goto case 3;
                case 3:
                {
                    writer.Write(IncognitoMode);
                }
                    goto case 2;
                case 2:
                {
                    writer.Write(ThrowableMode);

                    writer.Write(_Axes.Count);

                    foreach (KeyValuePair<PlayerMobile, ThrowableAxe> kvp in _Axes)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value);
                    }
                }
                    goto case 1;
                case 1:
                    BattleWellLoc.Serialize(writer);
                    goto case 0;
                case 0:
                {
                    writer.Write(BattleWell);
                    writer.Write(GiveRankTrophies);
                    writer.Write(GiveTrophies);
                    writer.Write(NoConsume);
                    writer.Write(PlayerStarted);

                    writer.Write("");

                    writer.Write(UseTemplates);
                    writer.Write(UseTemplateEquipment);
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();

            _Axes = new Dictionary<PlayerMobile, ThrowableAxe>();

            switch (version)
            {
                case 4:
                {
                    ThrowableID = reader.ReadInt();
                    ThrowableHue = reader.ReadInt();
                }
                    goto case 3;
                case 3:
                {
                    IncognitoMode = reader.ReadBool();
                }
                    goto case 2;
                case 2:
                {
                    ThrowableMode = reader.ReadBool();

                    int count = reader.ReadInt();

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            var player = reader.ReadMobile<PlayerMobile>();
                            var axe = reader.ReadItem<ThrowableAxe>();
                            _Axes.Add(player, axe);
                        }
                    }
                }
                    goto case 1;
                case 1:
                    BattleWellLoc = new MapPoint(reader);
                    goto case 0;
                case 0:
                {
                    BattleWell = reader.ReadItem<BattleWell>();
                    GiveRankTrophies = reader.ReadBool();
                    GiveTrophies = reader.ReadBool();
                    NoConsume = reader.ReadBool();
                    PlayerStarted = reader.ReadBool();

                    reader.ReadString();

                    UseTemplates = reader.ReadBool();
                    UseTemplateEquipment = reader.ReadBool();
                }
                    break;
            }
        }
    }
}