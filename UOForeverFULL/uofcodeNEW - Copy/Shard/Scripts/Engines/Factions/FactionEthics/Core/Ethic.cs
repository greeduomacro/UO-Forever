#region References

using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Commands;
using Server.Commands.Generic;
using Server.Ethics.Evil;
using Server.Ethics.Hero;
using Server.Mobiles;
using VitaNex;
using VitaNex.Targets;

#endregion

namespace Server.Ethics
{
    public abstract class Ethic
    {
        public static readonly bool Enabled = true;

        public static void Initialize()
        {
            if (Enabled)
                EventSink.Speech += new SpeechEventHandler(EventSink_Speech);

            CommandUtility.Register(
                "GrantEPL",
                AccessLevel.Administrator,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    if (e.Arguments.Length == 0)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <amount>");
                        return;
                    }

                    int value;

                    if (!Int32.TryParse(e.Arguments[0], out value))
                    {
                        e.Mobile.SendMessage(0x22, "Format: <amount>");
                        return;
                    }

                    GrantEPLTarget(e.Mobile as PlayerMobile, value);
                });

            CommandUtility.Register(
                "GrantSpheres",
                AccessLevel.Administrator,
                e =>
                {
                    if (!(e.Mobile is PlayerMobile))
                    {
                        return;
                    }

                    if (e.Arguments.Length == 0)
                    {
                        e.Mobile.SendMessage(0x22, "Format: <amount>");
                        return;
                    }

                    int value;

                    if (!Int32.TryParse(e.Arguments[0], out value))
                    {
                        e.Mobile.SendMessage(0x22, "Format: <amount>");
                        return;
                    }

                    GrantSpheresTarget(e.Mobile as PlayerMobile, value);
                });
        }

        public static void GrantEPLTarget(PlayerMobile pm, int value)
        {
            MobileSelectTarget<PlayerMobile>.Begin(pm, (m, t) => GrantEPL(pm, t, value), null);
        }

        public static void GrantEPL(PlayerMobile pm, PlayerMobile target, int value)
        {
            if (pm == null || pm.Deleted || target == null || target.Deleted)
            {
                return;
            }

            Player TargetEPL = Player.Find(target);


            TargetEPL.Power += value;

            target.SendMessage("{0} has granted you {1} life force.", pm.RawName, value);
            pm.SendMessage("You have granted {0} {1} life force.", pm.RawName, value);
        }

        public static void GrantSpheresTarget(PlayerMobile pm, int value)
        {
            MobileSelectTarget<PlayerMobile>.Begin(pm, (m, t) => GrantSpheres(pm, t, value), null);
        }

        public static void GrantSpheres(PlayerMobile pm, PlayerMobile target, int value)
        {
            if (pm == null || pm.Deleted || target == null || target.Deleted)
            {
                return;
            }

            Player TargetEPL = Player.Find(target);


            TargetEPL.Sphere += value;

            target.SendMessage("{0} has granted you {1} influence spheres.", pm.RawName, value);
            pm.SendMessage("You have granted {0} {1} influence spheres.", pm.RawName, value);
        }

        public static Ethic Find(Item item)
        {
            if (item is IEthicsItem)
            {
                EthicsItem ethicsItem = ((IEthicsItem) item).EthicsItemState;
                if (ethicsItem != null)
                {
                    return ethicsItem.Ethic;
                }
            }

            return null;
        }

        public static bool CheckTrade(Mobile from, Mobile to, Mobile newOwner, Item item)
        {
            Ethic itemEthic = Find(item);

            if (itemEthic == null || Find(newOwner) == itemEthic || from.AccessLevel >= AccessLevel.GameMaster ||
                to.AccessLevel >= AccessLevel.GameMaster)
            {
                return true;
            }

            if (itemEthic == Hero)
            {
                (from == newOwner ? to : from).SendMessage("Only heroes may receive this item.");
            }
            else if (itemEthic == Evil)
            {
                (from == newOwner ? to : from).SendMessage("Only evil may receive this item.");
            }

            return false;
        }

        public static bool CheckEquip(Mobile from, Item item)
        {
            Ethic itemEthic = Find(item);

            if (itemEthic == null || Find(from) == itemEthic)
            {
                return true;
            }

            if (itemEthic == Hero)
            {
                from.SendMessage("Only heroes may wear this item.");
            }
            else if (itemEthic == Evil)
            {
                from.SendMessage("Only evil may wear this item.");
            }

            if (Find(from) == null)
            {
                DestoryItem(from, item);
            }

            return false;
        }

        public static void DestoryItem(Mobile from, Item item)
        {
            Effects.PlaySound(from.Location, from.Map, 0x307);
            Effects.SendLocationEffect(from.Location, from.Map, 0x36BD, 9, 10, 0, 0);
            item.Delete();
        }

        public static bool IsImbued(Item item)
        {
            return IsImbued(item, false);
        }

        public static bool IsImbued(Item item, bool recurse)
        {
            if (Find(item) != null)
            {
                return true;
            }

            if (recurse)
            {
                foreach (Item child in item.Items)
                {
                    if (IsImbued(child, true))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsEthicsDeath(Mobile from)
        {
            Player fromState = Player.Find(from);


            Ethic fromEthic = fromState.Ethic;

            int ethicdamage = 0;
            int nonethicdamage = 0;

            for (int i = from.DamageEntries.Count - 1; i >= 0; --i)
            {
                if (i >= from.DamageEntries.Count)
                {
                    continue;
                }

                DamageEntry de = from.DamageEntries[i];

                if (de.HasExpired)
                {
                    from.DamageEntries.RemoveAt(i);
                }
                else if (de.Damager != from)
                {
                    Ethic ethic = Find(de.Damager);
                    if (ethic != null && ethic != fromEthic)
                    {
                        if (i == from.DamageEntries.Count - 1)
                        {
                            return true; //Most recent damager
                        }

                        ethicdamage += de.DamageGiven;
                    }
                    else
                    {
                        nonethicdamage += de.DamageGiven;
                    }
                }
            }

            return ethicdamage > nonethicdamage; //At least 50% ethic damage
        }

        public static void EventSink_Speech(SpeechEventArgs e)
        {
            if (e.Blocked || e.Handled)
                return;

            if (Enabled)
            {
                Mobile m = e.Mobile;

                Player pl = Player.Find(m);

                if (pl != null)
                {
                    if (e.Mobile is PlayerMobile && (e.Mobile as PlayerMobile).DuelContext != null)
                        return;

                    Ethic ethic = pl.Ethic;

                    for (int i = 0; i < ethic.Definition.Powers.Length; ++i)
                    {
                        Power power = ethic.Definition.Powers[i];

                        if (!Insensitive.Equals(power.Definition.Phrase.String, e.Speech))
                            continue;

                        if (!power.CheckInvoke(pl))
                            continue;

                        power.BeginInvoke(pl);
                        e.Handled = true;

                        break;
                    }
                }
            }
        }

        public virtual void AddAggressor(Mobile source)
        {
            if (!m_Aggressors.Contains(source))
            {
                m_Aggressors.Add(source);
            }
        }

        public virtual void RemoveAggressor(Mobile source)
        {
            if (m_Aggressors.Contains(source))
            {
                m_Aggressors.Remove(source);
            }
        }

        public virtual bool IsAggressed(Mobile source)
        {
            return m_Aggressors.Contains(source);
        }

        /*(public static void HandleDeath( Mobile mob )
		{
			HandleDeath( mob, null );
		}*/

        /*public static void HandleDeath( Mobile victim, Mobile killer )
		{
			if ( killer != null && victim != null && killer != victim && !(killer is BaseCreature) )
			{
				if ( victim is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)victim;
					Region homeregion = Region.Find( bc.Home, victim.Map );

					if ( ( homeregion == null || !homeregion.IsPartOf( typeof( ChampionSpawnRegion ) ) ) && ( killer.Region == null || !killer.Region.IsPartOf( typeof( ChampionSpawnRegion ) ) ) )
					{
						if ( !(victim is BaseVendor) && bc.GetEthicAllegiance( killer ) == BaseCreature.Allegiance.Enemy )
						{
							Player killerEPL = Player.Find( killer );

							if ( killerEPL != null && killerEPL.Power < Player.MaxPower )
							{
								killerEPL.Power += 3;

                                if (0.05 >= Utility.RandomDouble())
                                {
                                    killerEPL.Sphere += 1;
                                    killer.SendMessage("You have gained a sphere of influence for slaying a minion of {0}.", killerEPL.Ethic == Ethic.Evil ? "justice" : "evil");
                                }

								killer.SendMessage( "You gain a little life force for slaying a minion of {0}.", killerEPL.Ethic == Ethic.Evil ? "justice" : "evil" );
							}
						}
					}
				}
				else if ( victim.Guild == null || killer.Guild == null || killer.Guild != victim.Guild ) //not guild mates
				{
					var killerEPL = Player.Find( killer );
					var victimEPL = Player.Find( victim );

                     if ( killerEPL != null && !killerEPL.HasFallen ) //Killer is in ethics
					{
						if ( victimEPL != null && !victimEPL.HasFallen ) //Victim is in ethics
						{
							if ( killerEPL.Ethic != victimEPL.Ethic )
							{
								if ( !killer.Criminal && killer.Kills < Mobile.MurderCount && victimEPL.Power > 0 )
								{
									int powerTransfer = Math.Min( Math.Max( 1, victimEPL.Power / 5 ), Player.MaxPower - killerEPL.Power );

									if ( powerTransfer > 0 )
									{
										victimEPL.Power -= powerTransfer;
										killerEPL.Power += powerTransfer;

										killer.FixedEffect( 0x373A, 10, 30 );
										killer.PlaySound( 0x209 );

										killer.SendMessage( "You have gained {0} life force for killing {1}.", powerTransfer, victim.Name );
										victim.SendMessage( "You have lost {0} life force for falling victim to {1}.", powerTransfer, killer.Name );
									}

									if ( victimEPL.Sphere > 0 && ( victimEPL.Rank > 3 || ( killerEPL.Rank - victimEPL.Rank < 3 ) ) )
									{
										int sphereTransfer = Math.Max( 1, 1 + (victimEPL.Rank - killerEPL.Rank) );
										//Always at least 1pt
										victimEPL.Sphere -= sphereTransfer;
										killerEPL.Sphere += sphereTransfer;
									}
								}
							}
							//else //Both are of the same ethic - PENALTY!
							//	applyskillloss = true;
						}
						else if ( killerEPL.Ethic.IsAggressed( victim ) ) //Were they grey to this Ethic?
							killerEPL.Ethic.RemoveAggressor( victim );
						else if ( victimState == null || killerState == null ) //Not in ethics - PENALTY!
						{
							if ( !victim.Criminal && victim.Kills < Mobile.MurderCount )
							{
								ApplySkillLoss( killer );
								killerEPL.Power -= Math.Min( killerEPL.Power, 20 );
								killer.PlaySound( 0x133 );
								killer.FixedParticles( 0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist );
							}
						}
					}
					else if ( victimEPL != null && !(killer is BaseCreature) ) //Killer is NOT in ethics, but victim is.
					{
						if ( victimEPL.Ethic == Ethic.Evil )
						{
							if ( killer.Kills < Mobile.MurderCount && !killer.Criminal )
							{
								killer.FixedEffect( 0x373A, 10, 30 );
								killer.PlaySound( 0x209 );

								killer.SendMessage( "You slew an evil soul, embracing the path of heroes." );
							}
						}
						else if ( victimEPL.Ethic == Ethic.Hero )
						{
							killer.FixedEffect( 0x373A, 10, 30 );
							killer.PlaySound( 0x209 );

							killer.SendMessage( "You slew a heroic soul, embracing the path of evil." );
						}
					}

				}
			}
		}*/

        public static readonly TimeSpan FallenPeriod = TimeSpan.FromHours(72.0);

        protected EthicDefinition m_Definition;

        protected List<Player> m_Players;
        protected List<EthicsItem> m_EthicItems;
        protected List<Mobile> m_Aggressors;

        public EthicDefinition Definition { get { return m_Definition; } }

        public List<Player> Players { get { return m_Players; } }

        public List<EthicsItem> EthicItems { get { return m_EthicItems; } }

        public List<Mobile> Aggressors { get { return m_Aggressors; } }

        public static Ethic Find(Mobile mob)
        {
            return Find(mob, false);
        }

        public static Ethic Find(Mobile mob, bool inherit)
        {
            return Find(mob, inherit, false);
        }

        public static Ethic Find(Mobile mob, bool inherit, bool allegiance)
        {
            Player pl = Player.Find(mob);

            if (pl != null)
            {
                return pl.Ethic;
            }

            if (inherit && mob is BaseCreature)
            {
                var bc = (BaseCreature) mob;

                if (bc.Controlled)
                {
                    return Find(bc.ControlMaster, false);
                }
                else if (bc.Summoned)
                {
                    return Find(bc.SummonMaster, false);
                }
                else if (allegiance)
                {
                    return bc.EthicAllegiance;
                }
            }

            return null;
        }

        public Ethic()
        {
            m_Players = new List<Player>();
            m_EthicItems = new List<EthicsItem>();
            m_Aggressors = new List<Mobile>();
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                {
                    int itemCount = reader.ReadEncodedInt();

                    for (int i = 0; i < itemCount; ++i)
                    {
                        var ethicsItem = new EthicsItem(reader, this);

                        Timer.DelayCall(TimeSpan.Zero, new TimerCallback(ethicsItem.CheckAttach)); // sandbox attachment
                    }

                    goto case 0;
                }
                case 0:
                {
                    int playerCount = reader.ReadEncodedInt();

                    for (int i = 0; i < playerCount; ++i)
                    {
                        var pl = new Player(this, reader);

                        if (pl.Mobile != null)
                        {
                            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(pl.CheckAttach));
                        }
                    }

                    break;
                }
            }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(1); // version

            writer.WriteEncodedInt(m_EthicItems.Count);

            for (int i = 0; i < m_EthicItems.Count; ++i)
            {
                m_EthicItems[i].Serialize(writer);
            }

            writer.WriteEncodedInt(m_Players.Count);

            for (int i = 0; i < m_Players.Count; ++i)
            {
                m_Players[i].Serialize(writer);
            }
        }

        public static readonly Ethic Hero = new HeroEthic();
        public static readonly Ethic Evil = new EvilEthic();

        public static readonly Ethic[] Ethics = new Ethic[]
        {
            Hero,
            Evil
        };
    }
    public enum EthicKickType
    {
        Kick,
        Ban,
        Unban
    }

    public class EthicKickCommand : BaseCommand
    {
        private EthicKickType m_KickType;

        public EthicKickCommand(EthicKickType kickType)
        {
            m_KickType = kickType;

            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.AllMobiles;
            ObjectTypes = ObjectTypes.Mobiles;

            switch (m_KickType)
            {
                case EthicKickType.Kick:
                    {
                        Commands = new string[] { "EthicKick" };
                        Usage = "EthicKick";
                        Description = "Kicks the targeted player out of the ethics system. This does not prevent them from rejoining.";
                        break;
                    }
                case EthicKickType.Ban:
                    {
                        Commands = new string[] { "EthicBan" };
                        Usage = "EthicBan";
                        Description = "Bans the account of a targeted player from joining heroes/evil. All players on the account are removed from the ethics system, if any.";
                        break;
                    }
                case EthicKickType.Unban:
                    {
                        Commands = new string[] { "EthicUnban" };
                        Usage = "EthicUnban";
                        Description = "Unbans the account of a targeted player from joining heroes/evil.";
                        break;
                    }
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile mob = (Mobile)obj;

            switch (m_KickType)
            {
                case EthicKickType.Kick:
                    {
                        Player pl = Player.Find(mob);

                        if (pl != null)
                        {
                            pl.Detach();
                            mob.SendMessage("You have been kicked from heroes/evil.");
                            AddResponse("They have been kicked from heroes/evil.");
                        }
                        else
                            LogFailure("They are not in heroes/evils.");

                        break;
                    }
                case EthicKickType.Ban:
                    {
                        Account acct = mob.Account as Account;

                        if (acct != null)
                        {
                            if (acct.GetTag("EthicsBanned") == null)
                            {
                                acct.SetTag("EthicsBanned", "true");
                                AddResponse("The account has been banned from joining heroes/evil.");
                            }
                            else
                                AddResponse("The account is already banned from joining heroes/evil.");

                            for (int i = 0; i < acct.Length; ++i)
                            {
                                mob = acct[i];

                                if (mob != null)
                                {
                                    Player pl = Player.Find(mob);

                                    if (pl != null)
                                    {
                                        pl.Detach();
                                        mob.SendMessage("You have been kicked from heroes/evil.");
                                        AddResponse("They have been kicked from heroes/evil.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            LogFailure("They have no assigned account.");
                        }

                        break;
                    }
                case EthicKickType.Unban:
                    {
                        Account acct = mob.Account as Account;

                        if (acct != null)
                        {
                            if (acct.GetTag("EthicsBanned") == null)
                                AddResponse("The account is not banned from joining heroes/evil.");
                            else
                            {
                                acct.RemoveTag("EthicsBanned");
                                AddResponse("The account may now freely join heroes/evil.");
                            }
                        }
                        else
                        {
                            LogFailure("They have no assigned account.");
                        }

                        break;
                    }
            }
        }
    }
}