using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
    public class RootNode : UberNode // contains a list of all TriggerNodes
    {
        public List<TriggerNode> TriggerNodes = new List<TriggerNode>();
        public List<UserDefinedFunctionNode> UserDefinedFunctionNodes = new List<UserDefinedFunctionNode>();
        public List<RootNode> ChildRoots = new List<RootNode>();

        public RootNode(UberNode parent, string scriptInput) : base(parent, scriptInput) { }

        public XmlScript.TimerSubscriptionFlag TimerTriggerNodes
        {
            get 
            {
                XmlScript.TimerSubscriptionFlag flags = XmlScript.TimerSubscriptionFlag.None;
                foreach (TriggerNode node in TriggerNodes)
                {
                    //if (node.ScriptString.ToLower() == UberScriptTriggers.ON_TICK) flags |= XmlScript.TimerSubscriptionFlag.EveryTick;
                    if (node.TrigName == TriggerName.onTenMS) flags |= XmlScript.TimerSubscriptionFlag.TenMS;
                    else if (node.TrigName == TriggerName.onTwentyFiveMS) flags |= XmlScript.TimerSubscriptionFlag.TwentyFiveMS;
                    else if (node.TrigName == TriggerName.onFiftyMS) flags |= XmlScript.TimerSubscriptionFlag.FiftyMS;
                    else if (node.TrigName == TriggerName.onTwoFiftyMS) flags |= XmlScript.TimerSubscriptionFlag.TwoFiftyMS;
                    else if (node.TrigName == TriggerName.onOneSecond) flags |= XmlScript.TimerSubscriptionFlag.OneSecond;
                    else if (node.TrigName == TriggerName.onFiveSeconds) flags |= XmlScript.TimerSubscriptionFlag.FiveSeconds;
                    else if (node.TrigName == TriggerName.onOneMinute) flags |= XmlScript.TimerSubscriptionFlag.OneMinute;
                }
                return flags;
            }
        }

        public Object SpawnAndReturnObject(ProcessResult lastSiblingResult, TriggerObject trigObject)
        {
            // IF YOU EDIT THIS FUNCTION, BE SURE TO MAKE THE CORRESPONDING CHANGE TO THE Process FUNCTION!
            // this is just like Process, except it returns the spawned object (useful only in StatementNodes such that the object
            // can be assigned to the lefthand side of the statement node
            // e.g.
            // objs.test = orc
            // {
            //    name = goober
            // }
            Object prevSpawnedObject = trigObject.Spawn;
            Object callingObj = trigObject.Spawn != null ? trigObject.Spawn : trigObject.This;
            if (this.ScriptString != null)
            {
                // it's a spawn node, so we need to spawn an object here
                trigObject.Spawn = SpawnHandlers.Spawn(ScriptString, callingObj);
            }
            if (this.Children.Count > 0) // there is a special script associated with the spawn
            {
                ProcessChildren(trigObject); // ignore any overrides within spawn definitions
                IEntity spawnedObject = trigObject.Spawn as IEntity;
                if (spawnedObject != null && this.TriggerNodes.Count > 0)
                {
                    XmlScript script = new XmlScript(UberTreeParser.CurrentFileBeingParsed);
                    AddRootNodeIndeces(this, script.RootNodeIndeces);
                    XmlAttach.AttachTo(spawnedObject, script);
                }
            }
            // if it was already given a location in it's children, then don't give it one... otherwise place it
            // on the caller
            Item callerItem = null;
            Mobile callerMob = null;
            if (callingObj is Item) { callerItem = callingObj as Item; }
            else if (callingObj is Mobile) { callerMob = callingObj as Mobile; }
            
            Object o = trigObject.Spawn;
            Mobile m = null;
            Item item = null;
            if (o is Mobile) { m = (Mobile)o; }
            else if (o is Item) { item = (Item)o; }

            if ((m != null && m.Location == Point3D.Zero && m.Map == Map.Internal) || (item != null && item.Location == Point3D.Zero && item.Map == Map.Internal && item.RootParentEntity == null))
            {
                try
                {
                    if (m != null)
                    {
                        if (callerItem != null)
                        {
                            if (callerItem.RootParentEntity != null)
                            {
                                m.MoveToWorld(callerItem.RootParentEntity.Location, callerItem.RootParentEntity.Map);
                            }
                            else
                            {
                                m.MoveToWorld(callerItem.Location, callerItem.Map);
                            }
                        }
                        else if (callerMob != null) { m.MoveToWorld(callerMob.Location, callerMob.Map); }
                        else if (callingObj is IPoint3D)
                        {
                            m.MoveToWorld(new Point3D((IPoint3D)callingObj), Map.Felucca);
                        }
                        else throw new UberScriptException("Spawn caller (the thing XmlScript is attached to) was neither mobile or item! It was: " + callingObj);
                        //loc = GetSpawnPosition(requiresurface, packrange, packcoord, spawnpositioning, m);                  

                        if (m is BaseCreature)
                        {
                            BaseCreature c = (BaseCreature)m;
                            c.Home = c.Location;
                        }
                        trigObject.Spawn = prevSpawnedObject;
                        return o;
                    }
                    else if (item != null)
                    {
                        if (callerItem != null)
                        {
                            item.MoveToWorld(callerItem.Location, callerItem.Map);
                        }
                        else if (callerMob != null)
                        {
                            item.MoveToWorld(callerMob.Location, callerMob.Map);
                        }
                        else if (callingObj is IPoint3D)
                        {
                            item.MoveToWorld(new Point3D((IPoint3D)callingObj), Map.Felucca);
                        }
                        else throw new UberScriptException("Spawn caller (the thing XmlScript is attached to) was neither mobile or item! It was: " + callingObj);
                        trigObject.Spawn = prevSpawnedObject;
                        return o;
                    }
                }
                catch (Exception ex) { throw new UberScriptException(String.Format("When spawning {0}", o), ex); }
            }
            else if (m != null && m is BaseCreature)
            {
                BaseCreature c = (BaseCreature)m;
                c.Home = c.Location;
            }

            trigObject.Spawn = prevSpawnedObject;
            return o;
        }

        override public ProcessResult Process(ProcessResult lastSiblingResult, TriggerObject trigObject) // lastSiblingResult used for conditionals
        {
            // IF YOU EDIT THIS FUNCTION, BE SURE TO MAKE THE CORRESPONDING CHANGE TO THE SpawnAndReturnObject FUNCTION!
            Object prevSpawnedObject = trigObject.Spawn;
            Object callingObj = trigObject.Spawn != null ? trigObject.Spawn : trigObject.This;
            if (this.ScriptString != null)
            {
                // it's a spawn node, so we need to spawn an object here
                Object caller = trigObject.Spawn != null ? trigObject.Spawn : trigObject.This;
                trigObject.Spawn = SpawnHandlers.Spawn(ScriptString, caller);
            }
            if (this.Children.Count > 0) // there is a special script assoctiated with the spawn
            {
                ProcessResult lastResult = ProcessChildren(trigObject); // ignore any returns in spawn definition
                if (lastResult == ProcessResult.EndOfSequence)
                {
                    return lastResult; // we are processing a sequence
                }
                IEntity spawnedObject = trigObject.Spawn as IEntity;
                if (spawnedObject != null && this.TriggerNodes.Count > 0)
                {
                    XmlScript script = new XmlScript(UberTreeParser.CurrentFileBeingParsed);
                    // script.RootNodeIndeces.Add(0); // NO DON'T DO THIS// always has the initial 0 for the top root node (which always exists)
                    AddRootNodeIndeces(this, script.RootNodeIndeces);
                    XmlAttach.AttachTo(spawnedObject, script);
                }
            }
            // if it was already given a location in it's children, then don't give it one... otherwise place it
            // on the caller
            Item callerItem = null;
            Mobile callerMob = null;
            if (callingObj is Item) { callerItem = callingObj as Item; }
            else if (callingObj is Mobile) { callerMob = callingObj as Mobile; }

            Object o = trigObject.Spawn;
            Mobile m = null;
            Item item = null;
            if (o is Mobile) { m = (Mobile)o; }
            else if (o is Item) { item = (Item)o; }

            if ((m != null && m.Location == Point3D.Zero && m.Map == Map.Internal) || (item != null && item.Location == Point3D.Zero && item.Map == Map.Internal && item.RootParentEntity == null))
            {
                try
                {
                    if (m != null)
                    {
                        if (callerItem != null)
                        {
                            if (callerItem.RootParentEntity != null)
                            {
                                m.MoveToWorld(callerItem.RootParentEntity.Location, callerItem.RootParentEntity.Map);
                            }
                            else
                            {
                                m.MoveToWorld(callerItem.Location, callerItem.Map);
                            }
                        }
                        else if (callerMob != null) { m.MoveToWorld(callerMob.Location, callerMob.Map); }
                        else if (callingObj is IPoint3D)
                        {
                            m.MoveToWorld(new Point3D((IPoint3D)callingObj), Map.Felucca);
                        }
                        else throw new UberScriptException("Spawn caller (the thing XmlScript is attached to) was neither mobile or item! It was: " + callingObj);
                        //loc = GetSpawnPosition(requiresurface, packrange, packcoord, spawnpositioning, m);                  

                        if (m is BaseCreature)
                        {
                            BaseCreature c = (BaseCreature)m;
                            c.Home = c.Location;
                        }
                        trigObject.Spawn = prevSpawnedObject;
                        return ProcessResult.None;
                    }
                    else if (item != null)
                    {
                        if (callerItem != null)
                        {
                            item.MoveToWorld(callerItem.Location, callerItem.Map);
                        }
                        else if (callerMob != null)
                        {
                            item.MoveToWorld(callerMob.Location, callerMob.Map);
                        }
                        else if (callingObj is IPoint3D)
                        {
                            item.MoveToWorld(new Point3D((IPoint3D)callingObj), Map.Felucca);
                        }
                        else throw new UberScriptException("Spawn caller (the thing XmlScript is attached to) was neither mobile or item! It was: " + callingObj);
                        trigObject.Spawn = prevSpawnedObject;
                        return ProcessResult.None;
                    }
                }
                catch (Exception ex) { throw new UberScriptException(String.Format("When spawning {0}", o), ex); }
            }
            else if (m != null && m is BaseCreature)
            {
                BaseCreature c = (BaseCreature)m;
                c.Home = c.Location;
            }
            trigObject.Spawn = prevSpawnedObject;
            return ProcessResult.None;
        }

        public void AddRootNodeIndeces(RootNode inputNode, List<int> inputList) // gives the trail of ChildRoots indeces to follow to reach the input root node
        {
            // e.g.
            // <-- default root node (doesn't involve any spawn, always exists in all parsed scripts)
            // orc <-- if this root node is input, then you would return a list with [0]
            // {
            //      onDeath 
            //      {
            //          troll  <-- if this root node is input, then you would return a list with [0,0]
            //          {
            //              onDeath 
            //              {
            //                  MSG(TRIGMOB(),haha)
            //              }
            //          }
            //          orc  <-- if this root node is input, then you would return a list with [0,1]
            //          {
            //              onDeath 
            //              {
            //                  MSG(TRIGMOB(),haha)
            //              }
            //          }
            //      }
            //      onHit
            //      {
            //          ettin  <-- if this root node is input, then you would return a list with [0,2]
            //          {
            //              onDeath 
            //              {
            //                  MSG(TRIGMOB(),haha)
            //              }
            //          }         
            //      }
            // }
            // titan <-- if this root node is input, then you would return a list with [1]
            // {
            // }
            
            List<int> output = new List<int>();
            // first find the root node just up one from this one
            RootNode parentRoot = this.GetRootNode;
            if (parentRoot == null) 
                return;

            int index = 0;
            foreach (RootNode child in parentRoot.ChildRoots)
            {
                if (child == inputNode)
                {
                    // we have to "go backwards" up the tree, so add indeces related to the parent root first
                    AddRootNodeIndeces(parentRoot, inputList);
                    // now add the index for this, the closer ancestor
                    inputList.Add(index);
                    return;
                }
                index++;
            }
            // then traverse the rootnode tree, add indeces as you go, until the 
        }
    }
}