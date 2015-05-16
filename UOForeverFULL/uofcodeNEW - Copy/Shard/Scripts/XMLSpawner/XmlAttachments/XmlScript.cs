using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Commands;
using System.Text;

namespace Server.Engines.XmlSpawner2
{
    public class XmlScript : XmlAttachment
    {
        public static Dictionary<XmlScript, bool> AllScripts = new Dictionary<XmlScript, bool>();
        public static Dictionary<int, TriggerName> ScriptTriggerLookup = new Dictionary<int, TriggerName>(); // serial to XmlScript list conversion

        public static bool HasTrigger(IEntity entity, TriggerName trigToCheck)
        {
            if (entity == null) return false;
            TriggerName hasTrigger = TriggerName.NoTrigger;
            ScriptTriggerLookup.TryGetValue(entity.Serial.Value, out hasTrigger);
            return ((long)trigToCheck & (long)hasTrigger) > 0;
        }

        public void UpdateScriptTriggerLookup()
        {
            if (this.AttachedTo == null || this.AttachedTo.Deleted) return;

            try
            {
                List<TriggerNode> trigNodes = this.ScriptRootNode.TriggerNodes;
                foreach (TriggerNode trigNode in trigNodes)
                {
                    if (XmlScript.ScriptTriggerLookup.ContainsKey(this.AttachedTo.Serial.Value))
                    {
                        ScriptTriggerLookup[this.AttachedTo.Serial.Value] = (TriggerName)((long)ScriptTriggerLookup[this.AttachedTo.Serial.Value] | (long)trigNode.TrigName);
                    }
                    else
                    {
                        ScriptTriggerLookup.Add(this.AttachedTo.Serial.Value, trigNode.TrigName);
                    }
                }
            }
            catch
            {
                Console.WriteLine("ScriptRootNode error for " + this.ScriptFile);
            }
        }
        public void RemoveScriptTriggerLookup() // used when an xmlscript is removed
        {
            if (this.AttachedTo == null) return;

            // clear it out from the triggerlookup and rebuild it
            if (XmlScript.ScriptTriggerLookup.ContainsKey(this.AttachedTo.Serial.Value)) { XmlScript.ScriptTriggerLookup.Remove(this.AttachedTo.Serial.Value); }
            
            if (this.AttachedTo.Deleted) return; // leave it empty

            List<XmlScript> scripts = XmlAttach.GetScripts(this.AttachedTo);
            foreach (XmlScript script in scripts)
            {
                if (script != this)
                {
                    script.UpdateScriptTriggerLookup();
                }
            }
        }
        
        // keep track of root nodes in addition to filename...
        // otherwise nested triggers (e.g. if you spawn something in a script that also has onDeath)
        // are not possible...

        [Flags]
        public enum TimerSubscriptionFlag : byte
        {
            None = 0x00,
            EveryTick = 0x01,
            TenMS = 0x02,
            TwentyFiveMS = 0x04,
            FiftyMS = 0x08,
            TwoFiftyMS = 0x10,
            OneSecond = 0x20,
            FiveSeconds = 0x40,
            OneMinute = 0x80
            // 0x80 - 1 byte
        }

        private TimerSubscriptionFlag m_TimerSubscriptions = TimerSubscriptionFlag.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public TimerSubscriptionFlag TimerSubscriptions { 
            get { return m_TimerSubscriptions; }
            set {
                if (value == m_TimerSubscriptions) return;
                UberScriptTimedScripts.UnsubscribeScript(this);
                m_TimerSubscriptions = value;
                if (m_TimerSubscriptions  == TimerSubscriptionFlag.None) return;

                // check whether the script has a trigger matching the subscription
                TimerSubscriptionFlag availableFlags = ParsedScripts.GetTimerTriggers(this.ScriptFile, this.RootNodeIndeces);
                // mask with available flags
                m_TimerSubscriptions &= availableFlags;
                UberScriptTimedScripts.SubscribeScript(this, m_TimerSubscriptions);
            } 
        }

        private Mobile m_SendDebugMessageTo = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile SendDebugMessageTo
        {
            get { return m_SendDebugMessageTo; }
            set { m_SendDebugMessageTo = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimerSubscriptionFlag TimerSubscribe
        {
            get { return TimerSubscriptionFlag.None; }
            set
            {
                if (value == TimerSubscriptionFlag.None)
                {
                    return;
                }
                // check whether the script has a trigger matching the subscription
                TimerSubscriptionFlag availableFlags = ParsedScripts.GetTimerTriggers(this.ScriptFile, this.RootNodeIndeces);
                if ((value & availableFlags) == 0) return; // can't subscribe
                m_TimerSubscriptions |= value;
                UberScriptTimedScripts.SubscribeScript(this, m_TimerSubscriptions);                
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimerSubscriptionFlag TimerUnsubscribe
        {
            get { return TimerSubscriptionFlag.None; }
            set
            {
                if (value == TimerSubscriptionFlag.None || (m_TimerSubscriptions & value) == 0)
                {
                    return;
                }
                if (value == TimerSubscriptionFlag.EveryTick) UberScriptTimedScripts.EveryTick.Remove(this);
                else if (value == TimerSubscriptionFlag.TenMS) UberScriptTimedScripts.EveryTenMS.Remove(this);
                else if (value == TimerSubscriptionFlag.TwentyFiveMS) UberScriptTimedScripts.EveryTwentyFiveMS.Remove(this);
                else if (value == TimerSubscriptionFlag.FiftyMS) UberScriptTimedScripts.EveryFiftyMS.Remove(this);
                else if (value == TimerSubscriptionFlag.TwoFiftyMS) UberScriptTimedScripts.EveryTwoFiftyMS.Remove(this);
                else if (value == TimerSubscriptionFlag.OneSecond) UberScriptTimedScripts.EveryOneSecond.Remove(this);
                else if (value == TimerSubscriptionFlag.FiveSeconds) UberScriptTimedScripts.EveryFiveSeconds.Remove(this);
                else if (value == TimerSubscriptionFlag.OneMinute) UberScriptTimedScripts.EveryOneMinute.Remove(this);

                m_TimerSubscriptions ^= value;
            }
        }


        private List<int> m_RootNodeIndeces = new List<int>(); //
        public List<int> RootNodeIndeces { get { return m_RootNodeIndeces; } set { m_RootNodeIndeces = value; } }

        private string m_File;
        [CommandProperty(AccessLevel.GameMaster)]
        public string ScriptFile { get { return m_File; } 
            set 
            {
                m_File = value; 
                // NEED TO PROCESS, check if it's been parsed already,
                // fill in the TriggerNodes list
                ParsedScripts.AddScript(m_File);
            }
        }

        public RootNode ScriptRootNode
        {
            get { return ParsedScripts.GetRootNode(ScriptFile, RootNodeIndeces); }
        }

        public bool ProceedToNextStage = false;

        private int m_Stage = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Stage
        {
            get { return m_Stage; }
            set { m_Stage = value; }
        }

        private bool m_Enabled = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Enabled 
        { 
            get { return m_Enabled; } 
            set { m_Enabled = value; } 
        }

        private bool m_Paused = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Paused
        {
            get { return m_Paused; }
            set { m_Paused = value; }
        }

        private bool m_BlockIfPaused = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BlockIfPaused
        {
            get { return m_BlockIfPaused; }
            set { m_BlockIfPaused = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ExecuteScript
        {
            get { return false ; }
            set
            {
                if (value == false)
                    return;
                if (m_Enabled == false) m_Enabled = true;
                TriggerObject trigObject = new TriggerObject();
                trigObject.This = this.Owner;
                Execute(trigObject, false);
            }
        }

        public List<PausedUberScript> PausedScripts = new List<PausedUberScript>();

        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // a serial constructor is REQUIRED
        public XmlScript(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlScript(string file, string name)
        {
            ScriptFile = file;
            Name = name;
        }

        [Attachable]
        public XmlScript(string file)
        {
            ScriptFile = file;
        }

        [Attachable]
        public XmlScript()
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);

            // version 3

            // Don't save Paused scripts for now... I need to redesign the NodeChain for it to work right
            /*
            List<PausedUberScript> pausedScriptsToSerialize = new List<PausedUberScript>();
            foreach (PausedUberScript pausedScript in PausedScripts)
            {
                if (pausedScript.Running)
                    pausedScriptsToSerialize.Add(pausedScript);
            }
            writer.Write((int)pausedScriptsToSerialize.Count);
            foreach (PausedUberScript pausedScript in pausedScriptsToSerialize)
            {
                pausedScript.Serialize(writer);
            }*/

            writer.Write((bool)m_BlockIfPaused);

            // version 2
            writer.Write((int)m_Stage);

            // version 1
            writer.Write((byte)m_TimerSubscriptions);

            // version 0
            writer.Write(m_File);
            writer.Write(m_Enabled);
            writer.Write(m_RootNodeIndeces.Count);
            foreach (int val in m_RootNodeIndeces)
            {
                writer.Write(val);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            if (!this.Deleted)
            {
                AllScripts.Add(this, true);
            }

            int version = reader.ReadInt();
            TimerSubscriptionFlag flags = TimerSubscriptionFlag.None;
            switch (version)
            {
                case 3:
                    //m_Paused = reader.ReadBool();
                    m_BlockIfPaused = reader.ReadBool();
                    goto case 2;
                case 2:
                    m_Stage = reader.ReadInt();
                    goto case 1;
                case 1:
                    flags = (TimerSubscriptionFlag)reader.ReadByte();
                    goto case 0;
                case 0:
                    m_File = reader.ReadString();
                    m_Enabled = reader.ReadBool();
                    int numRootIndeces = reader.ReadInt();
                    for (int i = 0; i < numRootIndeces; i++)
                    {
                        m_RootNodeIndeces.Add(reader.ReadInt());
                    }
                    break;
            }
            TimerSubscriptions = flags;
        }

        public override void OnAttach()
        {
            base.OnAttach();
            AllScripts.Add(this, true);
            /*
            List<string> timerTriggers = ParsedScripts.GetTimerTriggers(ScriptFile, RootNodeIndeces);
            if (timerTriggers != null)
            {
                // subscribe to timers
                UberScriptTimedScripts.SubscribeScript(this, timerTriggers);
            }
             * */

            UpdateScriptTriggerLookup();

            if (XmlScript.HasTrigger(AttachedTo, TriggerName.onCreate))
            {
                TriggerObject trigObject = new TriggerObject();
                trigObject.TrigName = TriggerName.onCreate;
                trigObject.This = AttachedTo;
                this.Execute(trigObject, true);
            }

            if (AttachedTo is BaseCreature)
            {
                if (XmlScript.HasTrigger(AttachedTo, TriggerName.onActivate))
                {
                    BaseCreature bc = (BaseCreature)AttachedTo;
                    if (bc.AIObject.m_Timer != null && bc.AIObject.m_Timer.Running)
                    {
                        UberScriptTriggers.Trigger(bc, bc, TriggerName.onActivate);
                    }
                }
            }
        }

        public override void OnReattach()
        {
            base.OnReattach();
            /*List<string> timerTriggers = ParsedScripts.GetTimerTriggers(ScriptFile, RootNodeIndeces);
            if (timerTriggers != null)
            {
                // subscribe to timers
                UberScriptTimedScripts.SubscribeScript(this, timerTriggers);
            }
            TriggerObject trigObject = new TriggerObject();
            trigObject.TriggerName = TriggerName.onCreate;
            trigObject.This = AttachedTo;
            this.Execute(trigObject, true);
             * */

            UpdateScriptTriggerLookup();
        }

        public override void OnDelete()
        {
            base.OnDelete();
            try
            {
                RemoveScriptTriggerLookup();
                AllScripts.Remove(this);
                UberScriptTimedScripts.UnsubscribeScript(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public virtual bool Execute(TriggerObject trigObject, bool triggersOnly, UberNode pauseNode = null)
        {
            if (!Enabled || Deleted) return false;
            if (AttachedTo != null && AttachedTo.Deleted) { Delete(); return false; }

            trigObject.Script = this;
            // true IF there was a trigger override
            bool returnOverride = false;
            try
            {
                returnOverride = ParsedScripts.TryExecuteScript(ScriptFile, trigObject, RootNodeIndeces, triggersOnly);
            }
            catch (UberScriptException e)
            {
                if (m_SendDebugMessageTo != null && !m_SendDebugMessageTo.Deleted)
                {
                    m_SendDebugMessageTo.SendMessage(36, "Error Executing script file: " + ScriptFile);
                    Exception innerMostException = e;
                    m_SendDebugMessageTo.SendMessage(36, e.Message);
                    int level = 0;
                    while (true)
                    {
                        if (innerMostException.InnerException == null) break;
                        level++;
                        innerMostException = innerMostException.InnerException;
                        string msg = "";
                        for (int i = 0; i < level; i++)
                        {
                            msg += "--";
                        }
                        msg += ">" + innerMostException.Message;
                        m_SendDebugMessageTo.SendMessage(36, msg);
                    }
                }
                if (ParsedScripts.DebugLevel >= (int)ParsedScripts.DebugLevels.ScriptDebugMessages)
                {
                    Console.WriteLine("Error Executing script file: " + ScriptFile);
                    Exception innerMostException = e;
                    Console.WriteLine(e.Message);
                    int level = 0;
                    while (true)
                    {
                        if (innerMostException.InnerException == null) break;
                        level++;
                        innerMostException = innerMostException.InnerException;
                        for (int i = 0; i < level; i++)
                        {
                            Console.Write("\t");
                        }
                        Console.WriteLine(innerMostException.Message);
                    }
                    if (ParsedScripts.DebugLevel >= (int)ParsedScripts.DebugLevels.ScriptDebugMessagesAndStackTrace)
                        Console.WriteLine(innerMostException.StackTrace);
                }
            }
            catch (Exception general)
            {
                if (m_SendDebugMessageTo != null && !m_SendDebugMessageTo.Deleted)
                {
                    m_SendDebugMessageTo.SendMessage(36, "GENERAL UNCAUGHT ERROR Executing script file: " + ScriptFile);
                    m_SendDebugMessageTo.SendMessage(36, general.Message);
                }
                Console.WriteLine("Error Executing script file: " + ScriptFile);
                Console.WriteLine("GENERAL UNCAUGHT ERROR: this should never happen!");
                Console.WriteLine(general.Message);
                Console.WriteLine(general.StackTrace);
            }
            return returnOverride;
        }
    }

    /// <summary>
    /// Specifically for the LINEEFFECT stuff
    /// </summary>
    public class UberScriptSpot : Item
    {
        [Constructable]
        public UberScriptSpot()
            : base(0x9D7)
        {
            Weight = 1.0;
            Visible = false;
            Movable = false;
        }

        public UberScriptSpot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnDelete()
        {
            if (XmlScript.HasTrigger(this, TriggerName.onDelete))
                UberScriptTriggers.Trigger(this, this.RootParentEntity as Mobile, TriggerName.onDelete);
            base.OnDelete();
        }
    }

    /// <summary>
    /// Specifically to handle movement and speech as an item
    /// </summary>
    public class UberScriptItem : Item
    {
        [Constructable]
        public UberScriptItem()
            : base(0x9D7)
        {
            Weight = 1.0;
            Movable = false;
        }

        [Constructable]
        public UberScriptItem(int itemID)
            : base(itemID)
		{
		}

        public UberScriptItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);
            if (XmlScript.HasTrigger(this, TriggerName.onNearbyMove))
                UberScriptTriggers.Trigger(this, m, TriggerName.onNearbyMove);
        }

        public override bool OnMoveOff( Mobile m )
		{
            if (XmlScript.HasTrigger(this, TriggerName.onMoveOff) && UberScriptTriggers.Trigger(this, m, TriggerName.onMoveOff))
            {
                return false;
            }
            return true;
		}

		public override bool OnMoveOver( Mobile m )
		{
            if (XmlScript.HasTrigger(this, TriggerName.onMoveOver) && UberScriptTriggers.Trigger(this, m, TriggerName.onMoveOver))
            {
                return false;
            }
            return true;
		}

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (XmlScript.HasTrigger(this, TriggerName.onSpeech) && UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
            {
                e.Handled = true;
                return;
            }
        }

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                if (XmlScript.HasTrigger(this, TriggerName.onEquip) && UberScriptTriggers.Trigger(this, (Mobile)parent, TriggerName.onEquip))
                {
                    ((Mobile)parent).AddToBackpack(this); // override, put it in their pack
                    base.OnAdded(parent);
                    return;
                }

                if (Server.Engines.XmlSpawner2.XmlAttach.CheckCanEquip(this, (Mobile)parent))
                {
                    Server.Engines.XmlSpawner2.XmlAttach.CheckOnEquip(this, (Mobile)parent);
                }
                else
                {
                    ((Mobile)parent).AddToBackpack(this);
                }
            }
            else if (parent is Item)
            {
                Item parentItem = (Item)parent;
                if (XmlScript.HasTrigger(this, TriggerName.onAdded))
                    UberScriptTriggers.Trigger(this, parentItem.RootParentEntity as Mobile, TriggerName.onAdded, parentItem);
            }

            base.OnAdded(parent);
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                UberScriptTriggers.Trigger(this, (Mobile)parent, TriggerName.onUnequip);
            }
            else if (parent is Item)
            {
                Item parentItem = (Item)parent;
                UberScriptTriggers.Trigger(this, parentItem.RootParentEntity as Mobile, TriggerName.onRemove, parentItem);
            }

            Server.Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent);
            base.OnRemoved(parent); 
        }

        public override void OnDelete()
        {
            if (XmlScript.HasTrigger(this, TriggerName.onDelete))
                UberScriptTriggers.Trigger(this, this.RootParentEntity as Mobile, TriggerName.onDelete);
            base.OnDelete();
        }
    }

    [Furniture]
    [Flipable(0xE43, 0xE42)]
    public class UberScriptChest : LockableContainer
    {
        [Constructable]
        public UberScriptChest()
            : base(0xE42)
        {
            Weight = 2.0;
        }

        public UberScriptChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);
            if (XmlScript.HasTrigger(this, TriggerName.onNearbyMove))
                UberScriptTriggers.Trigger(this, m, TriggerName.onNearbyMove);
        }

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
            {
                e.Handled = true;
                return;
            }
        }

        public override void OnDelete()
        {
            if (XmlScript.HasTrigger(this, TriggerName.onDelete))
                UberScriptTriggers.Trigger(this, this.RootParentEntity as Mobile, TriggerName.onDelete);
            base.OnDelete();
        }
    }

    public class UberScriptContainer : BaseContainer
    {
        [Constructable]
		public UberScriptContainer() : base( 0xE76 )
		{
			Weight = 2.0;
			Dyable = true;
		}

		public override bool DisplayDyable{ get{ return false; } }

        public UberScriptContainer(Serial serial)
            : base(serial)
		{
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

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);
            if (XmlScript.HasTrigger(this, TriggerName.onNearbyMove))
                UberScriptTriggers.Trigger(this, m, TriggerName.onNearbyMove);
        }

        public override bool OnMoveOff(Mobile m)
        {
            if (XmlScript.HasTrigger(this, TriggerName.onMoveOff) && UberScriptTriggers.Trigger(this, m, TriggerName.onMoveOff))
            {
                return false;
            }
            return true;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (XmlScript.HasTrigger(this, TriggerName.onMoveOver) && UberScriptTriggers.Trigger(this, m, TriggerName.onMoveOver))
            {
                return false;
            }
            return true;
        }

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (XmlScript.HasTrigger(this, TriggerName.onSpeech) && UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
            {
                e.Handled = true;
                return;
            }
        }

        public override void OnDelete()
        {
            if (XmlScript.HasTrigger(this, TriggerName.onDelete))
                UberScriptTriggers.Trigger(this, this.RootParentEntity as Mobile, TriggerName.onDelete);
            base.OnDelete();
        }
    }

    public class UberScriptArrow: QuestArrow
    {
        private Mobile m_From;
        private Timer m_Timer;
        private bool m_Closable;

        public UberScriptArrow( Mobile from, IEntity target, int range, bool closable ) : base( from, target )
        {
            m_From = from;
            m_Timer = new UberScriptArrowTimer( from, target, range, this );
            m_Timer.Start();
            m_Closable = closable;
        }

        public override void OnClick( bool rightClick )
        {
            if (m_Closable && rightClick )
            {

                m_From = null;

                Stop();
            }
        }

        public override void OnStop()
        {
            m_Timer.Stop();

            if ( m_From != null )
            {
            }
        }
    }

    public class UberScriptArrowTimer: Timer
    {
        private Mobile m_From;
        private IEntity m_Target;
        private int m_Range;
        private int m_LastX, m_LastY;
        private QuestArrow m_Arrow;

        public UberScriptArrowTimer( Mobile from, IEntity target, int range, QuestArrow arrow ) : base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 2.5 ) )
        {
            m_From = from;
            m_Target = target;
            m_Range = range;

            m_Arrow = arrow;
        }

        protected override void OnTick()
        {
            if ( !m_Arrow.Running )
            {
                Stop();
                return;
            }
            else 
            {
                if (m_From.NetState == null || m_From.Deleted || m_Target.Deleted || m_From.Map != m_Target.Map || (m_Range != -1 && !m_From.InRange(m_Target, m_Range)) || (m_Target is Mobile && ((Mobile)m_Target).Hidden && ((Mobile)m_Target).AccessLevel > m_From.AccessLevel))
                {
                    m_Arrow.Stop();
                    Stop();
                    return;
                }
            }

            if ( m_LastX != m_Target.X || m_LastY != m_Target.Y )
            {
                m_LastX = m_Target.X;
                m_LastY = m_Target.Y;

                m_Arrow.Update();
            }
        }
    }
}
