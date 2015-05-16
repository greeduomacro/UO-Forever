using System;
using Server;
using Server.Targeting;
using System.Collections;
using Server.Spells;
using Server.ContextMenus;

namespace Server.Items
{

   public class CannonBall : Item
	{
		[Constructable]
		public CannonBall() : this( 1 )
		{
		}

		[Constructable]
		public CannonBall( int amount ) : base( 0xE73 )
		{
			Stackable = true;
            Hue = 33;
			Weight = 5.0;
			Amount = amount;
		}

		public CannonBall( Serial serial ) : base( serial )
		{
		}

        public override string DefaultName { get { return "CannonBall"; } }

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

   public class CannonFire : AddonComponent
   {
   private int m_Dir;

   [CommandProperty( AccessLevel.GameMaster )]
   public int Dir{ get{ return m_Dir; } set{ m_Dir = value; } }

   public CannonFire(int dir, int peice) : base (0xE8C)
   {
   Name = "a fireable Cannon";
   Weight = 400;
   switch (peice)
           {
           case 1:
           {
           switch (dir)
              {
              case 1: ItemID = 0xE8D; break;//North
              case 2: ItemID = 0xE93; break;//South
              case 3: ItemID = 0xE94; break;//East
              case 4: ItemID = 0xE8E; break;//West
         }
           break;
           }
           case 2:
           {
           switch (dir)
              {
              case 1: ItemID = 0xE8C; break;//North
              case 2: ItemID = 0xE92; break;//South
              case 3: ItemID = 0xE95; break;//East
              case 4: ItemID = 0xE8F; break;//West
         }
           break;
           }
           case 3:
           {
           switch (dir)
              {
              case 1: ItemID = 0xE8B; break;//North
              case 2: ItemID = 0xE91; break;//South
              case 3: ItemID = 0xE96; break;//East
              case 4: ItemID = 0xE90; break;//West
         }
           break;
           }
           }
   Dir = dir;
   }

   public override void OnDoubleClick( Mobile from )
      {
      from.Target = new InternalTarget(this);
      }
   private class InternalTarget : Target
      {
      private CannonFire can;
      public InternalTarget(CannonFire m_can) : base( 20, true, TargetFlags.None )
         {
         can = m_can;
         }

         protected override void OnTarget( Mobile from, object o )
         {
         IPoint3D p = o as IPoint3D;
         IPoint3D p1 = can.Location as IPoint3D;
         bool los = false;
         int x = 0;
         int y = 0;
         // Shooting arc only +-45º
         if (can.Dir == 1 && p.Y < p1.Y)
            {
            if ( p1.X >= p.X )
               x = p1.X - p.X;
            else
               x = p.X - p1.X;
            y = p1.Y - p.Y;
            if ( (y - x) >= 0 )
               los = true;
            }
         if (can.Dir == 2 && p.Y > p1.Y)
            {
            if ( p1.X >= p.X )
               x = p1.X - p.X;
            else
               x = p.X - p1.X;
            y = p.Y - p1.Y;
            if ( (y - x) >= 0 )
               los = true;
            }
         if (can.Dir == 3 && p.X > p1.X)
            {
            x = p.X - p1.X;
            if ( p1.Y >= p.Y )
               y = p1.Y - p.Y;
            else
               y = p.Y - p1.Y;
            if ( (x - y) >= 0 )
               los = true;
            }
         if (can.Dir == 4 && p.X < p1.X)
            {
            x = p1.X - p.X;
            if ( p1.Y >= p.Y )
               y = p1.Y - p.Y;
            else
               y = p.Y - p1.Y;
            if ( (x - y) >= 0 )
               los = true;
            }
         if ( los )
         {
         if (from.InLOS(p))
         {
         int time = 0;
         Point3D p2 = new Point3D( p1.X, p1.Y, p1.Z + 10);
         if ( from.BeginAction( typeof( CannonFire ) ) )
            {
         if (  from.Backpack.GetAmount( typeof( CannonBall )) > 0 && from.Backpack.GetAmount( typeof( SulfurousAsh )) > 4 && from.InRange( can.Location, 3 ) && !from.Mounted )
            {
               from.Backpack.ConsumeTotal( typeof( SulfurousAsh ), 5 );
               from.Backpack.ConsumeTotal( typeof( CannonBall ), 1 );
               IEntity to;
               to = new Entity( Serial.Zero, new Point3D(p.X, p.Y, p.Z + 5), from.Map );
               IEntity fro;
               fro = new Entity( Serial.Zero, p2, from.Map );
               Effects.SendMovingEffect( fro, to, 0xE73, 1, 0, false, true, 0, 0 );
               Effects.PlaySound(p1,from.Map,519);
               Explode( from, new Point3D(p), from.Map );
               time = 10;
               }
         else if ( from.Backpack.GetAmount( typeof( CannonBall )) == 0)
            from.Say("You don't have any cannon balls!");
         else if ( from.Backpack.GetAmount( typeof( SulfurousAsh )) < 5)
            from.SendLocalizedMessage( 1049617 ); // You do not have enough sulfurous ash.
               else if ( from.Mounted )
                  from.SendLocalizedMessage( 1010097 ); // You cannot use this while mounted.

         else
            from.Say("You are too far from cannon!");
         new DelayTimer( from, time ).Start();
            }
         else
            from.SendMessage( "The cannon is too hot! You must wait 10 seconds before firing again.", from);


         }
         else
            from.SendLocalizedMessage( 500237 ); // Target cannot be seen.
         }
        else
           from.SendLocalizedMessage( 500237 ); // Target cannot be seen.
         }

      private class DelayTimer : Timer
      {
         private Mobile m_Mobile;

         public DelayTimer( Mobile m, int time ) : base( TimeSpan.FromSeconds( time ) )
         {
            m_Mobile = m;
         }

         protected override void OnTick()
         {
            m_Mobile.EndAction( typeof( CannonFire ) );
         }
      }
      private void Explode( Mobile from, Point3D loc, Map map )
      {
         if ( map == null )
            return;

         Effects.PlaySound( loc, map, 0x207 );
         Effects.SendLocationEffect( loc, map, 0x36BD, 20 );
         IPooledEnumerable eable = map.GetObjectsInRange( loc, 2 );
         ArrayList toExplode = new ArrayList();

         foreach ( object o in eable )
         {
            if ( o is Mobile )
            {
               toExplode.Add( o );
            }
            else if ( o is BaseExplosionPotion && o != this )
            {
               toExplode.Add( o );
            }
         }

         eable.Free();
         int d = 0;
         for ( int i = 0; i < toExplode.Count; ++i )
         {
            object o = toExplode[i];

            if ( o is Mobile )
            {
               Mobile m = (Mobile)o;
               if ( m.InRange( loc, 0 ) )
                  d = 1;
               else if ( m.InRange( loc, 1 ) )
                  d = 2;
               else if ( m.InRange( loc, 2 ) )
                  d = 3;
               if ( from == null || (SpellHelper.ValidIndirectTarget( from, m ) && from.CanBeHarmful( m, false )) )
               {
                  if ( from != null )
                     from.DoHarmful( m );

                  int damage = Utility.RandomMinMax( (60 / d), (80 / d) );

                  m.Damage(damage, from);
               }
            }
            else if ( o is BaseExplosionPotion )
            {
               BaseExplosionPotion pot = (BaseExplosionPotion)o;

               pot.Explode( from, false, pot.GetWorldLocation(), pot.Map );
            }
         }
      }

      }


   public CannonFire( Serial serial ) : base( serial )
      {
      }

   public override void Serialize( GenericWriter writer )
      {
      base.Serialize( writer );

      writer.Write( (int) 0 ); // version
      writer.Write( m_Dir );
      }

   public override void Deserialize( GenericReader reader )
      {
      base.Deserialize( reader );

      int version = reader.ReadInt();
      m_Dir = reader.ReadInt();
      }
   }

   public class fcannon : BaseAddon
   {
   private int m_Dir;

   [CommandProperty( AccessLevel.GameMaster )]
   public int Dir{ get{ return m_Dir; } set{ m_Dir = value; } }

   public override BaseAddonDeed Deed{ get{ return new fireableCannonDeed(m_Dir); } }
         [Constructable]
         public fcannon(int dir) // 1 = North - 2 = South - 3 = East - 4 = West
            {
            Name = "a Fireable Cannon";
            switch (dir)
         {
         case 1:
            {
                  AddComponent( new CannonFire(dir, 1),  0, -1, 0 );
                   AddComponent( new CannonFire(dir, 2),  0,  0, 0 );
                   AddComponent( new CannonFire(dir, 3),  0,  1, 0 ); break;
            }
         case 2:
            {
                  AddComponent( new CannonFire(dir, 1),  0, -1, 0 );
                   AddComponent( new CannonFire(dir, 2),  0,  0, 0 );
                   AddComponent( new CannonFire(dir, 3),  0,  1, 0 );break;
            }
         case 3:
            {
                  AddComponent( new CannonFire(dir, 1),  -1, 0, 0 );
                   AddComponent( new CannonFire(dir, 2),  0,  0, 0 );
                   AddComponent( new CannonFire(dir, 3),  1,  0, 0 ); break;
            }
         case 4:
            {
                  AddComponent( new CannonFire(dir, 1),  -1, 0, 0 );
                   AddComponent( new CannonFire(dir, 2), 0,  0, 0 );
                   AddComponent( new CannonFire(dir, 3),  1,  0, 0 ); break;
            }
         }
            Dir=dir;
            Movable = true;
            }

   public fcannon( Serial serial ) : base( serial )
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
   }

   public class fireableCannonDeed : BaseAddonDeed
   {
   private int m_Dir;

   [CommandProperty( AccessLevel.GameMaster )]
   public int Dir{ get{ return m_Dir; } set{ m_Dir = value; } }

      public override BaseAddon Addon{ get{ return new fcannon(m_Dir); } }

      [Constructable]
      public fireableCannonDeed(int dir)// 1 = North - 2 = South - 3 = East - 4 = West
      {
      Dir = dir;
      switch (dir)
         {
         case 1: Name = "fireable Cannon deed facing north"; break;
         case 2: Name = "fireable Cannon deed facing south"; break;
         case 3: Name = "fireable Cannon deed facing east"; break;
         case 4: Name = "fireable Cannon deed facing west"; break;
         }
      }

      public fireableCannonDeed( Serial serial ) : base( serial )
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
   }
}
