using System;
using System.Collections;
using System.Collections.Generic;
using Server.Spells;

namespace Server.Engines.XmlSpawner2
{
    public class TriggerObject
    {
        public TriggerName TrigName;
        public object This;
        public Mobile TrigMob;
        public Item TrigItem;
        public Object Targeted;
        public int Damage;
        public Object Spawn;
        public Spell Spell;
        public string Speech;
        public XmlScript Script;
        public SkillName SkillName;
        public double SkillValue;
        public Queue<UberNode> PausedNodeChain;
        public List<UberNode> CurrentNodeExecutionChain = new List<UberNode>();
        public string GumpID;

        public Dictionary<string, int> ints = new Dictionary<string, int>();
        public Dictionary<string, string> strings = new Dictionary<string, string>();
        public Dictionary<string, double> doubles = new Dictionary<string, double>();
        public Dictionary<string, object> objs = new Dictionary<string, object>();

        public TriggerObject()
        {
        }

        public TriggerObject Dupe()
        {
            TriggerObject output = new TriggerObject();
            output.TrigName = TrigName;
            output.This = This;
            output.TrigMob = TrigMob;
            output.TrigItem = TrigItem;
            output.Spell = Spell;
            output.Damage = Damage;
            output.Speech = Speech;
            output.Targeted = Targeted;
            output.SkillName = SkillName;
            output.SkillValue = SkillValue;
            output.Script = Script;
            output.GumpID = GumpID;
            foreach (KeyValuePair<string, int> pair in ints) { output.ints.Add(pair.Key, pair.Value); }
            foreach (KeyValuePair<string, string> pair in strings) { output.strings.Add(pair.Key, pair.Value); }
            foreach (KeyValuePair<string, double> pair in doubles) { output.doubles.Add(pair.Key, pair.Value); }
            foreach (KeyValuePair<string, object> pair in objs) { output.objs.Add(pair.Key, pair.Value); }
            return output;
        }
    }
}
