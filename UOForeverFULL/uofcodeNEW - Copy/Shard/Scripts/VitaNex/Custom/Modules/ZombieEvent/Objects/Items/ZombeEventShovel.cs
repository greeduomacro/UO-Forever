using System;
using System.Linq;
using Server;
using Server.Engines.Conquests;
using Server.Engines.Harvest;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Targeting;
using VitaNex.Targets;

namespace Server.Items
{
    public class ZombieEventShovel : BaseHarvestTool
	{
		public override HarvestSystem HarvestSystem{ get{ return Mining.System; } }

        public Timer DigTimer;

		[Constructable]
		public ZombieEventShovel() : this( 50 )
		{
		}

		[Constructable]
		public ZombieEventShovel( int uses ) : base( uses, 0xF39 )
		{
		    Hue = 61;
			Weight = 5.0;
		}

        public ZombieEventShovel(Serial serial)
            : base(serial)
		{
		}

	    public override void OnDoubleClick(Mobile @from)
	    {
	        if (DigTimer == null || !DigTimer.Running)
	        {
	            from.SendMessage(
	                "If you wish to bury a cache, target it now.  If you wish to dig up ore or another cache, target the area you wish to dig.");
	            var pm = from as ZombieAvatar;
	            if (pm != null)
	                pm.Target = new GenericSelectTarget<object>((m, t) => DetermineTarget(pm, t), m => { }, -1, true,
	                    TargetFlags.None);
	            //base.OnDoubleClick(@from);
	        }
	        else
	        {
	            from.SendMessage(54, "You are already using this shovel to dig something up.");
	        }
	    }

        public void DetermineTarget(ZombieAvatar pm, object target)
	    {
            IEntity targetedEntity = target as IEntity;

	        if (targetedEntity is CacheChest)
	        {
	            pm.SendMessage(54, "Where do you wish to bury this cache chest?");
                pm.Target = new GenericSelectTarget<object>((m, t) => DetermineBuryLocation(pm, t, targetedEntity), m => { }, -1, true, TargetFlags.None);
	            return;
	        }
	        if (target is LandTarget)
	        {
	            var tile = ((LandTarget) target);
	            if (!Mining.m_MountainAndCaveTiles.Contains(tile.TileID))
	            {
                    pm.PlaySound(Utility.RandomList(0x126));
                    pm.Animate(11, 5, 1, true, false, 0);
                    pm.PublicOverheadMessage(MessageType.Label, pm.SpeechHue, true, "*Begins to dig*");
                    var list = tile.Location.GetEntitiesInRange(Map.ZombieLand, 3);
                    if (list.Exists(x => x is CacheChest && ((CacheChest)x).Buried))
                    {
                        pm.SendMessage(54, "You have found a cache!");
                        var chest = list.First(x => x is CacheChest) as CacheChest;
                        if (chest != null)
                        {
                            DigTimer = new InternalDigTimer(chest, pm);
                            DigTimer.Start();
                            pm.Frozen = true;
                        }
                        return;
                    }
                    pm.SendMessage(54, "There is nothing of note in this location.");
	            }
	            else
	            {
                    if (!HarvestSystem.CheckHarvest(pm, this))
                    {
                        return;
                    }

                    DoHarvest(pm, target);            
	            }
	        }
	    }

        public void DetermineBuryLocation(ZombieAvatar pm, object target, IEntity cache)
        {
            if (target is LandTarget)
            {
                var loc = ((LandTarget)target).Location;
                var list = loc.GetEntitiesInRange(Map.ZombieLand, 3);
                if (list.Exists(x => x is CacheChest && ((CacheChest)x).Buried))
                {
                    pm.SendMessage(54, "You cannot bury a cache here.  There is one already present in this location!  Try to dig it up first.");
                }
                else if (DigTimer == null || !DigTimer.Running)
                {
                    var chest = cache as CacheChest;
                    if (chest != null)
                    {
                        pm.PublicOverheadMessage(MessageType.Label, pm.SpeechHue, true, "*Begins to bury a cache*");
                        chest.MoveToWorld(loc, Map.ZombieLand);
                        DigTimer = new InternalBuryTimer(chest, pm);
                        DigTimer.Start();
                        pm.Frozen = true;
                    }
                }
                else
                {
                    pm.SendMessage(54, "You are already digging with this shovel!");
                }
            }
        }

        public void DoHarvest(ZombieAvatar pm, object targeted)
	    {
            IEntity targetedEntity = targeted as IEntity;
            if (XmlScript.HasTrigger(targetedEntity, TriggerName.onTargeted) && UberScriptTriggers.Trigger(targetedEntity, pm, TriggerName.onTargeted, this))
            {
                return;
            }

            CustomRegion customRegion = pm.Region as CustomRegion;
            if (customRegion != null && customRegion.Controller != null)
            {
                SkillName skill = SkillName.Spellweaving; // placeholder
                if (HarvestSystem is Mining) skill = SkillName.Mining;

                if (customRegion.Controller.IsRestrictedSkill((int)skill))
                {
                    pm.SendMessage("You cannot use that skill here.");
                    return;
                }
            }

            //conquest skill check
		    Skill tskill = null;
            if (HarvestSystem is Mining)
		    {
                tskill = pm.Skills[SkillName.Mining];		        
		    }

            HarvestSystem.StartHarvesting(pm, this, targeted);
		}

        private class InternalBuryTimer : Timer
        {
            private CacheChest _Cache;
            private Mobile _User;
            private int _Count;

            public InternalBuryTimer(CacheChest chest, Mobile user)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                _Cache = chest;

                _User = user;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (_User.Alive && _Count < 10)
                {
                    _User.PlaySound(Utility.RandomList(0x126));
                    _User.Animate(11, 5, 1, true, false, 0);
                    _Cache.Z--;
                    _Count++;
                    _Cache.Buried = true;
                    _Cache.Movable = false;
                }
                if (!_Cache.GetMobilesInRange(_Cache.Map, 3).Contains(_User) && _Count < 10)
                {
                    _Cache.Z += _Count-1;
                    _Cache.Movable = true;
                    _Cache.Visible = true;
                    _Cache.Buried = false;
                    _User.Frozen = false;
                    Stop();
                }
                if (_Count == 10)
                {
                    _User.Say("*Finished burying a cache chest*");
                    Stop();
                    _Cache.Visible = false;
                    _Cache.Breakable = false;
                    _User.Frozen = false;
                }
            }
        }

        private class InternalDigTimer : Timer
        {
            private CacheChest _Cache;
            private Mobile _User;
            private int _Count;

            public InternalDigTimer(CacheChest chest, Mobile user)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                _Cache = chest;

                _User = user;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (_User.Alive && _Count < 10)
                {
                    _User.PlaySound(Utility.RandomList(0x126));
                    _User.Animate(11, 5, 1, true, false, 0);
                    _Cache.Z++;
                    _Count++;
                    _Cache.Buried = false;
                    _Cache.Visible = true;
                }
                if (!_Cache.GetMobilesInRange(_Cache.Map, 3).Contains(_User) && _Count < 10)
                {
                    _Cache.Z -= _Count-1;
                    _Cache.Movable = false;
                    _Cache.Visible = false;
                    _Cache.Buried = true;
                    Stop();
                    _User.Frozen = false;
                }
                if (_Count == 10)
                {
                    _User.Say("*Finished digging up a cache chest*");
                    Stop();
                    _Cache.Movable = true;
                    _Cache.Breakable = true;
                    _User.Frozen = false;
                }
            }
        }
	      
	    public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}