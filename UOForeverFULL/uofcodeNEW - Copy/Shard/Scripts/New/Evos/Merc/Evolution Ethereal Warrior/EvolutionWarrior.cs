//Script Modified By: Crows Kingdom Shard
 
using System;
using System.Collections; 
using Server.Mobiles;
using Server.Items;
using Server.Network; 
using Server.Targeting;
using Server.Gumps;
using Server.Engines.XmlSpawner2;
 
namespace Server.Mobiles
{
 [CorpseName( "a warrior hatchling corpse" )]
 public class EvolutionWarrior : BaseCreature
 {
  private Timer m_BreatheTimer;
  private DateTime m_EndBreathe;
  private Timer m_WarriorMatingTimer;
  private DateTime m_EndWarriorMating;
 
  private Timer m_WarriorLoyaltyTimer;
  private DateTime m_EndWarriorLoyalty;
 
  public DateTime EndWarriorMating{ get{ return m_EndWarriorMating; } set{ m_EndWarriorMating = value; } }
 
  public DateTime EndWarriorLoyalty{ get{ return m_EndWarriorLoyalty; } set{ m_EndWarriorLoyalty = value; } }
 
  public int m_Stage;
  public int m_KP;
  public bool m_AllowWarriorMating;
  public bool m_HasEgg;
  public bool m_Pregnant;
 
  public bool m_S1;
  public bool m_S2;
  public bool m_S3;
  public bool m_S4;
  public bool m_S5;
  public bool m_S6;
 
  public bool S1
  {
   get{ return m_S1; }
   set{ m_S1 = value; }
  }
  public bool S2
  {
   get{ return m_S2; }
   set{ m_S2 = value; }
  }
  public bool S3
  {
   get{ return m_S3; }
   set{ m_S3 = value; }
  }
  public bool S4
  {
   get{ return m_S4; }
   set{ m_S4 = value; }
  }
  public bool S5
  {
   get{ return m_S5; }
   set{ m_S5 = value; }
  }
  public bool S6
  {
   get{ return m_S6; }
   set{ m_S6 = value; }
  }
 
  [CommandProperty( AccessLevel.GameMaster )]
  public bool AllowWarriorMating
  {
   get{ return m_AllowWarriorMating; }
   set{ m_AllowWarriorMating = value; }
  }
 
  [CommandProperty( AccessLevel.GameMaster )]
  public bool HasEgg
  {
   get{ return m_HasEgg; }
   set{ m_HasEgg = value; }
  }
  [CommandProperty( AccessLevel.GameMaster )]
  public bool Pregnant
  {
   get{ return m_Pregnant; }
   set{ m_Pregnant = value; }
  }
 
  [CommandProperty( AccessLevel.GameMaster )]
  public int KP
  {
   get{ return m_KP; }
   set{ m_KP = value; }
  }
 
  [CommandProperty( AccessLevel.GameMaster )]
  public int Stage
  {
   get{ return m_Stage; }
   set{ m_Stage = value; }
  }
 
  [Constructable]
  public EvolutionWarrior() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
  {
   Female = Utility.RandomBool();
   Name = "a warrior hatchling";
   Body = 282;
   Hue = Utility.RandomList( 1157, 1175, 1172, 1171, 1170, 1169, 1168, 1167, 1166, 1165 );
   BaseSoundID = 191;
   Stage = 1;
 
   S1 = true;
   S2 = true;
   S3 = true;
   S4 = true;
   S5 = true;
   S6 = true;
 
   SetStr( 296, 325 );
   SetDex( 56, 75 );
   SetInt( 76, 96 );
 
   SetHits( 200, 250 );
 
   SetDamage( 11, 17 );
 
   
 
   
 
   SetSkill( SkillName.Magery, 50.1, 70.0 );
   SetSkill( SkillName.Meditation, 50.1, 70.0 );
   SetSkill( SkillName.EvalInt, 50.1, 70.0 );
   SetSkill( SkillName.MagicResist, 15.1, 20.0 );
   SetSkill( SkillName.Tactics, 19.3, 34.0 );
   SetSkill( SkillName.Wrestling, 19.3, 34.0 );
   SetSkill( SkillName.Anatomy, 19.3, 34.0 );
 
   Fame = 300;
   Karma = -300;
 
   VirtualArmor = 30;
 
   ControlSlots = 3;
 

   m_WarriorLoyaltyTimer = new WarriorLoyaltyTimer( this, TimeSpan.FromSeconds( 5.0 ) );
   m_WarriorLoyaltyTimer.Start();
   m_EndWarriorLoyalty = DateTime.UtcNow + TimeSpan.FromSeconds( 5.0 );
  }
 
  public EvolutionWarrior(Serial serial) : base(serial)
  {
  }
 
  public override void Damage( int amount, Mobile defender )
  {
   int kpgainmin, kpgainmax;
 
   if ( this.Stage == 1 )
   {
    if ( defender is BaseCreature )
    {
     BaseCreature bc = (BaseCreature)defender;
 
     if ( bc.Controlled != true )
     {
      kpgainmin = 5 + ( bc.HitsMax ) / 10;
      kpgainmax = 5 + ( bc.HitsMax ) / 5;
 
      this.KP += Utility.RandomList( kpgainmin, kpgainmax );
     }
    }
 
    if ( this.KP >= 25000 )
    {
     if ( this.S1 == true )
     {
      this.S1 = false;
      int hits, va, mindamage, maxdamage;
 
      hits = ( this.HitsMax + 500 );
 
      va = ( this.VirtualArmor + 10 );
 
      mindamage = this.DamageMin + ( 1 );
      maxdamage = this.DamageMax + ( 1 );
 
      this.Warmode = false;
      this.Say( "*"+ this.Name +" evolves*");
      this.SetDamage( mindamage, maxdamage );
      this.SetHits( hits );
      this.BodyValue = 128;
      this.BaseSoundID = 1127;
      this.VirtualArmor = va;
      this.Stage = 2;
 
      this.RawStr += 200;
      this.RawInt += 30;
      this.RawDex += 20;
     }
    }
   }
 
   else if ( this.Stage == 2 )
   {
    if ( defender is BaseCreature )
    {
     BaseCreature bc = (BaseCreature)defender;
 
     if ( bc.Controlled != true )
     {
      kpgainmin = 5 + ( bc.HitsMax ) / 20;
      kpgainmax = 5 + ( bc.HitsMax ) / 10;
 
      this.KP += Utility.RandomList( kpgainmin, kpgainmax );
     }
    }
 
    if ( this.KP >= 75000 )
    {
     if ( this.S2 == true )
     {
      this.S2 = false;
      int hits, va, mindamage, maxdamage;
 
      hits = ( this.HitsMax + 100 );
 
      va = ( this.VirtualArmor + 10 );
 
      mindamage = this.DamageMin + ( 1 );
      maxdamage = this.DamageMax + ( 1 );
 
      this.Warmode = false;
      this.Say( "*"+ this.Name +" evolves*");
      this.SetDamage( mindamage, maxdamage );
      this.SetHits( hits );
      this.BodyValue = 39;
      this.BaseSoundID = 422;
      this.VirtualArmor = va;
      this.Stage = 3;
 
      this.RawStr += 100;
      this.RawInt += 20;
      this.RawDex += 10;
     }
    }
   }
 
   else if ( this.Stage == 3 )
   {
    if ( defender is BaseCreature )
    {
     BaseCreature bc = (BaseCreature)defender;
 
     if ( bc.Controlled != true )
     {
      kpgainmin = 5 + ( bc.HitsMax ) / 30;
      kpgainmax = 5 + ( bc.HitsMax ) / 20;
 
      this.KP += Utility.RandomList( kpgainmin, kpgainmax );
     }
    }
 
    if ( this.KP >= 175000 )
    {
     if ( this.S3 == true )
     {
      this.S3 = false;
      int hits, va, mindamage, maxdamage;
 
      hits = ( this.HitsMax + 100 );
 
      va = ( this.VirtualArmor + 10 );
 
      mindamage = this.DamageMin + ( 1 );
      maxdamage = this.DamageMax + ( 1 );
 
      this.Warmode = false;
      this.Say( "*"+ this.Name +" evolves*");
      this.SetDamage( mindamage, maxdamage );
      this.SetHits( hits );
      this.BodyValue = 74;
      this.BaseSoundID = 422;
      this.VirtualArmor = va;
      this.Stage = 4;
 
      this.RawStr += 100;
      this.RawInt += 120;
      this.RawDex += 10;
     }
    }
   }
 
   else if ( this.Stage == 4 )
   {
    if ( defender is BaseCreature )
    {
     BaseCreature bc = (BaseCreature)defender;
 
     if ( bc.Controlled != true )
     {
      kpgainmin = 5 + ( bc.HitsMax ) / 50;
      kpgainmax = 5 + ( bc.HitsMax ) / 40;
 
      this.KP += Utility.RandomList( kpgainmin, kpgainmax );
     }
    }
 
    if ( this.KP >= 375000 )
    {
     if ( this.S4 == true )
     {
      this.S4 = false;
      int hits, va, mindamage, maxdamage;
 
      hits = ( this.HitsMax + 100 );
 
      va = ( this.VirtualArmor + 10 );
 
      mindamage = this.DamageMin + ( 5 );
      maxdamage = this.DamageMax + ( 5 );
 
      this.Warmode = false;
      this.Say( "*"+ this.Name +" evolves*");
      this.SetDamage( mindamage, maxdamage );
      this.SetHits( hits );
      this.BodyValue = 30;
      this.BaseSoundID = 402;
      this.VirtualArmor = va;
      this.Stage = 5;

      this.RawStr += 100;
      this.RawInt += 120;
      this.RawDex += 20;
     }
    }
   }
 
   else if ( this.Stage == 5 )
   {
    if ( defender is BaseCreature )
    {
     BaseCreature bc = (BaseCreature)defender;
 
     if ( bc.Controlled != true )
     {
      kpgainmin = 5 + ( bc.HitsMax ) / 160;
      kpgainmax = 5 + ( bc.HitsMax ) / 100;
 
      this.KP += Utility.RandomList( kpgainmin, kpgainmax );
     }
    }
 
    if ( this.KP >= 775000 )
    {
     if ( this.S5 == true )
     {
      this.S5 = false;
      int hits, va, mindamage, maxdamage;
 
      hits = ( this.HitsMax + 100 );
 
      va = ( this.VirtualArmor + 100 );
 
      mindamage = this.DamageMin + ( 5 );
      maxdamage = this.DamageMax + ( 5 );
 
      this.AllowWarriorMating = true;
      this.Warmode = false;
      this.Say( "*"+ this.Name +" evolves*");
      this.SetDamage( mindamage, maxdamage );
      this.SetHits( hits );
      this.BodyValue = 149;
      this.BaseSoundID = 1200;
      this.VirtualArmor = va;
      this.Stage = 6;
 
      this.RawStr += 100;
      this.RawInt += 120;
      this.RawDex += 20;
     }
    }
   }
 
   else if ( this.Stage == 6 )
   {
    if ( defender is BaseCreature )
    {
     BaseCreature bc = (BaseCreature)defender;
 
     if ( bc.Controlled != true )
     {
      kpgainmin = 5 + ( bc.HitsMax ) / 540;
      kpgainmax = 5 + ( bc.HitsMax ) / 480;
 
      this.KP += Utility.RandomList( kpgainmin, kpgainmax );
     }
    }
 
    if ( this.KP >= 1500000 )
    {
     if ( this.S6 == true )
     {
      this.S6 = false;
      int hits, va, mindamage, maxdamage;
 
      hits = ( this.HitsMax + 350 );
 
      va = ( this.VirtualArmor + 100 );
 
      mindamage = this.DamageMin + ( 15 );
      maxdamage = this.DamageMax + ( 15 );
 
      this.Warmode = false;
      this.Say( "*"+ this.Name +" is now an ethereal warrior*");
      this.Title = "the Ethereal Warrior";
      this.SetDamage( mindamage, maxdamage );
      this.SetHits( hits );
      this.BodyValue = 123;
      this.BaseSoundID = 0;
      this.VirtualArmor = va;
      this.Stage = 7;

      this.RawStr += 125;
      this.RawInt += 125;
      this.RawDex += 35;
     }
    }
   }
 
   else if ( this.Stage == 7 )
   {
    if ( defender is BaseCreature )
    {
     BaseCreature bc = (BaseCreature)defender;
 
     if ( bc.Controlled != true )
     {
      kpgainmin = 5 + ( bc.Hits ) / 740;
      kpgainmax = 5 + ( bc.Hits ) / 660;
 
      this.KP += Utility.RandomList( kpgainmin, kpgainmax );
     }
    }
   }
 
   base.Damage( amount, defender );
  }
 
  public override bool OnDragDrop( Mobile from, Item dropped )
  {
      // trigger returns true if returnoverride
      if (XmlScript.HasTrigger(this, TriggerName.onDragDrop) && UberScriptTriggers.Trigger(this, from, TriggerName.onDragDrop, dropped))
          return true;
   PlayerMobile player = from as PlayerMobile;
 
   if ( player != null )
   {
    if ( dropped is WarriorDust )
    {
     WarriorDust dust = ( WarriorDust )dropped;
 
     int amount = ( dust.Amount * 5 );
 
     this.PlaySound( 665 );
     this.KP += amount;
     dust.Delete();
     this.Say( "*"+ this.Name +" absorbs the warrior dust*" );
 
     return false;
    }
    else
    {
    }
   }
   return base.OnDragDrop( from, dropped );
  }
 

                private void WarriorMatingTarget_Callback( Mobile from, object obj ) 
                { 
                            if ( obj is EvolutionWarrior && obj is BaseCreature ) 
                            { 
     BaseCreature bc = (BaseCreature)obj;
     EvolutionWarrior ed = (EvolutionWarrior)obj;
 
     if ( ed.Controlled == true && ed.ControlMaster == from )
     {
      if ( ed.Female == false )
      {
       if ( ed.AllowWarriorMating == true )
       {
        this.Blessed = true;
        this.Pregnant = true;
 
        m_WarriorMatingTimer = new WarriorMatingTimer( this, TimeSpan.FromDays( 1.0 ) );
        m_WarriorMatingTimer.Start();
 
        m_EndWarriorMating = DateTime.UtcNow + TimeSpan.FromDays( 1.0 );
       }
       else
       {
        from.SendMessage( "This male warrior is not old enough to mate!" );
       }
      }
      else
      {
       from.SendMessage( "This warrior is not male!" );
      }
     }
     else if ( ed.Controlled == true )
     {
      if ( ed.Female == false )
      {
       if ( ed.AllowWarriorMating == true )
       {
        if ( ed.ControlMaster != null )
        {
         ed.ControlMaster.SendGump( new WarriorMatingGump( from, ed.ControlMaster, this, ed ) );
         from.SendMessage( "You ask the owner of the warrior if they will let your female mate with their male." );
        }
                                else
                                {
                                    from.SendMessage( "This warrior is wild." );
           }
       }
       else
       {
        from.SendMessage( "This male warrior is not old enough to mate!" );
       }
      }
      else
      {
       from.SendMessage( "This warrior is not male!" );
      }
     }
                             else 
                             { 
                                 from.SendMessage( "This warrior is wild." );
        }
                            } 
                            else 
                            { 
                                from.SendMessage( "That is not a warrior!" );
       }
  }
 
  public override void OnDoubleClick( Mobile from )
  {
   if ( this.Controlled == true && this.ControlMaster == from )
   {
    if ( this.Female == true )
    {
     if ( this.AllowWarriorMating == true )
     {
      if ( this.Pregnant == true )
      {
       if ( this.HasEgg == true )
       {
        this.HasEgg = false;
        this.Pregnant = false;
        this.Blessed = false;
        from.AddToBackpack( new WarriorEgg() );
        from.SendMessage( "A warrior's egg has been placed in your backpack" );
       }
       else
       {
        from.SendMessage( "The warrior has not yet produced an egg." );
       }
      }
      else
      {
       from.SendMessage( "Target a male warrior to mate with this female." );
                                   from.BeginTarget( -1, false, TargetFlags.Harmful, new TargetCallback( WarriorMatingTarget_Callback ) );
      }
     }
     else
     {
      from.SendMessage( "This female warrior is not old enough to mate!" );
     } 
    }
   }
  }
 
  private DateTime m_NextBreathe;
 
  public override void OnActionCombat()
  {
   Mobile combatant = Combatant;
 
   if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
    return;
 
   if ( DateTime.UtcNow >= m_NextBreathe )
   {
    Breathe( combatant );
 
    m_NextBreathe = DateTime.UtcNow + TimeSpan.FromSeconds( 12.0 + (3.0 * Utility.RandomDouble()) ); // 12-15 seconds
   }
  }
 
  public void Breathe( Mobile m )
  {
   DoHarmful( m );
 
   m_BreatheTimer = new BreatheTimer( m, this, this, TimeSpan.FromSeconds( 1.0 ) );
   m_BreatheTimer.Start();
   m_EndBreathe = DateTime.UtcNow + TimeSpan.FromSeconds( 1.0 );
 
   this.Frozen = true;
 
   if ( this.Stage == 1 )
   {
    this.MovingParticles( m, 0x1FA8, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
   }
   else if ( this.Stage == 2 )
   {
    this.MovingParticles( m, 0x1FA9, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
   }
   else if ( this.Stage == 3 )
   {
    this.MovingParticles( m, 0x1FAB, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
   }
   else if ( this.Stage == 4 )
   {
    this.MovingParticles( m, 0x1FBC, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
   }
   else if ( this.Stage == 5 )
   {
    this.MovingParticles( m, 0x1FBD, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
   }
   else if ( this.Stage == 6 )
   {
    this.MovingParticles( m, 0x1FBF, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
   }
   else if ( this.Stage == 7 )
   {
    this.MovingParticles( m, 0x1FBE, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
   }
   else
   {
    
    this.PublicOverheadMessage( MessageType.Regular, this.SpeechHue, true, "Please call a GM if you are getting this message, they will fix the breathe, thank you :)", false );
   }
  }
 
  private class BreatheTimer : Timer
  {
   private EvolutionWarrior ed;
   private Mobile m_Mobile, m_From;
 
   public BreatheTimer( Mobile m, EvolutionWarrior owner, Mobile from, TimeSpan duration ) : base( duration ) 
   {
    ed = owner;
    m_Mobile = m;
    m_From = from;
    Priority = TimerPriority.TwoFiftyMS;
   }
 
   protected override void OnTick()
   {
    int damagemin = ed.Hits / 20;
    int damagemax = ed.Hits / 25;
    ed.Frozen = false;
 
    m_Mobile.PlaySound( 0x11D );
    m_Mobile.Damage(Utility.RandomMinMax(damagemin, damagemax), m_From);
    Stop();
   }
  }
 
  public override void Serialize(GenericWriter writer)
  {
   base.Serialize(writer);
 
   writer.Write((int) 1);
                        writer.Write( m_AllowWarriorMating ); 
                        writer.Write( m_HasEgg ); 
                        writer.Write( m_Pregnant ); 
                        writer.Write( m_S1 ); 
                        writer.Write( m_S2 ); 
                        writer.Write( m_S3 ); 
                        writer.Write( m_S4 ); 
                        writer.Write( m_S5 ); 
                        writer.Write( m_S6 ); 
   writer.Write( (int) m_KP );
   writer.Write( (int) m_Stage );
   writer.WriteDeltaTime( m_EndWarriorMating );
   writer.WriteDeltaTime( m_EndBreathe );
   writer.WriteDeltaTime( m_EndWarriorLoyalty );
  }
 
  public override void Deserialize(GenericReader reader)
  {
   base.Deserialize(reader);
 
   int version = reader.ReadInt();
 
   switch ( version )
   {
    case 1:
    {
                          m_AllowWarriorMating = reader.ReadBool(); 
                          m_HasEgg = reader.ReadBool();  
                          m_Pregnant = reader.ReadBool(); 
                          m_S1 = reader.ReadBool(); 
                          m_S2 = reader.ReadBool(); 
                          m_S3 = reader.ReadBool(); 
                          m_S4 = reader.ReadBool(); 
                          m_S5 = reader.ReadBool(); 
                          m_S6 = reader.ReadBool(); 
     m_KP = reader.ReadInt();
     m_Stage = reader.ReadInt();
 
     m_EndWarriorMating = reader.ReadDeltaTime();
     m_WarriorMatingTimer = new WarriorMatingTimer( this, m_EndWarriorMating - DateTime.UtcNow );
     m_WarriorMatingTimer.Start();
 
     m_EndBreathe = reader.ReadDeltaTime();
     m_BreatheTimer = new BreatheTimer( this, this, this, m_EndBreathe - DateTime.UtcNow );
     m_BreatheTimer.Start();
 
     m_EndWarriorLoyalty = reader.ReadDeltaTime();
     m_WarriorLoyaltyTimer = new WarriorLoyaltyTimer( this, m_EndWarriorLoyalty - DateTime.UtcNow );
     m_WarriorLoyaltyTimer.Start();
 
     break;
    }
    case 0:
    {
     TimeSpan durationbreathe = TimeSpan.FromSeconds( 1.0 );
     TimeSpan durationmating = TimeSpan.FromDays( 3.0 );
     TimeSpan durationloyalty = TimeSpan.FromSeconds( 5.0 );
 
     m_BreatheTimer = new BreatheTimer( this, this, this, durationbreathe );
     m_BreatheTimer.Start();
     m_EndBreathe = DateTime.UtcNow + durationbreathe;
 
     m_WarriorMatingTimer = new WarriorMatingTimer( this, durationmating );
     m_WarriorMatingTimer.Start();
     m_EndWarriorMating = DateTime.UtcNow + durationmating;
 
     m_WarriorLoyaltyTimer = new WarriorLoyaltyTimer( this, durationloyalty );
     m_WarriorLoyaltyTimer.Start();
     m_EndWarriorLoyalty = DateTime.UtcNow + durationloyalty;
 
     break;
    }
   }
  }
 }
 
 public class WarriorMatingTimer : Timer
 { 
  private EvolutionWarrior ed;
 
  public WarriorMatingTimer( EvolutionWarrior owner, TimeSpan duration ) : base( duration ) 
  { 
   Priority = TimerPriority.OneSecond;
   ed = owner;
  }
 
  protected override void OnTick() 
  {
   ed.Blessed = false;
   ed.HasEgg = true;
   ed.Pregnant = false;
   Stop();
  }
 }
 public class WarriorLoyaltyTimer : Timer
 { 
  private EvolutionWarrior ed;
 
  public WarriorLoyaltyTimer( EvolutionWarrior owner, TimeSpan duration ) : base( duration ) 
  { 
   Priority = TimerPriority.OneSecond;
   ed = owner;
  }
 
  protected override void OnTick() 
  {
   ed.Loyalty = 100;
 
   WarriorLoyaltyTimer lt = new WarriorLoyaltyTimer( ed, TimeSpan.FromSeconds( 5.0 ) );
   lt.Start();
   ed.EndWarriorLoyalty = DateTime.UtcNow + TimeSpan.FromSeconds( 5.0 );
 
   Stop();
  }
 }
 public class WarriorMatingGump : Gump
 {
  private Mobile m_From;
  private Mobile m_Mobile;
  private EvolutionWarrior m_ED1;
  private EvolutionWarrior m_ED2;
 
  public WarriorMatingGump( Mobile from, Mobile mobile, EvolutionWarrior ed1, EvolutionWarrior ed2 ) : base( 25, 50 )
  {
   Closable = false; 
   Dragable = false; 
 
   m_From = from;
   m_Mobile = mobile;
   m_ED1 = ed1;
   m_ED2 = ed2;
 
   AddPage( 0 );
 
   AddBackground( 25, 10, 420, 200, 5054 );
 
   AddImageTiled( 33, 20, 401, 181, 2624 );
   AddAlphaRegion( 33, 20, 401, 181 );
 
   AddLabel( 125, 148, 1152, m_From.Name +" would like to mate "+ m_ED1.Name +" with" );
   AddLabel( 125, 158, 1152, m_ED2.Name +"." );
 
   AddButton( 100, 50, 4005, 4007, 1, GumpButtonType.Reply, 0 );
   AddLabel( 130, 50, 1152, "Allow them to mate." );
   AddButton( 100, 75, 4005, 4007, 0, GumpButtonType.Reply, 0 );
   AddLabel( 130, 75, 1152, "Do not allow them to mate." );
  }
 
  public override void OnResponse( NetState state, RelayInfo info )
  {
   Mobile from = state.Mobile; 
 
   if ( from == null )
    return;
 
   if ( info.ButtonID == 0 )
   {
    m_From.SendMessage( m_Mobile.Name +" declines your request to mate the two warriors." );
    m_Mobile.SendMessage( "You decline "+ m_From.Name +"'s request to mate the two warriors." );
   }
   if ( info.ButtonID == 1 )
   {
    m_ED1.Blessed = true;
    m_ED1.Pregnant = true;
 
    WarriorMatingTimer mt = new WarriorMatingTimer( m_ED1, TimeSpan.FromDays( 1.0 ) );
    mt.Start();
    m_ED1.EndWarriorMating = DateTime.UtcNow + TimeSpan.FromDays( 1.0 );
 
    m_From.SendMessage( m_Mobile.Name +" accepts your request to mate the two warriors." );
    m_Mobile.SendMessage( "You accept "+ m_From.Name +"'s request to mate the two warriors." );
   }
  }
 }
}