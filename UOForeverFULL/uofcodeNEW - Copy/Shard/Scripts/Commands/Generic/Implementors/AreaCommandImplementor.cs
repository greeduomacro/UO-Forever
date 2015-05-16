using System;
using System.Collections;
using Server;

namespace Server.Commands.Generic
{
	public class AreaCommandImplementor : BaseCommandImplementor
	{
		private static AreaCommandImplementor m_Instance;

		public static AreaCommandImplementor Instance
		{
			get { return m_Instance; }
		}

        //maximum edge length of the bounding box - used to prevent staff members from using [area commands covering the whole world
        public static readonly int MaxLength = 250;

        //minimum accesslevel for unrestricted area usage
        public static readonly AccessLevel MaxLengthOverride = AccessLevel.Administrator;

        public AreaCommandImplementor()
		{
			Accessors = new string[]{ "Area", "Group" };
			SupportRequirement = CommandSupport.Area;
			SupportsConditionals = true;
			AccessLevel = AccessLevel.GameMaster;
			Usage = "Area <command> [condition]";
			Description = "Invokes the command on all appropriate objects in a targeted area. Optional condition arguments can further restrict the set of objects.";

			m_Instance = this;
		}

		public override void Process( Mobile from, BaseCommand command, string[] args )
		{
			BoundingBoxPicker.Begin( from, new BoundingBoxCallback( OnTarget ), new object[]{ command, args } );
		}

		public void OnTarget( Mobile from, Map map, Point3D start, Point3D end, object state )
		{
			try
			{
                if ( from.AccessLevel < MaxLengthOverride && Math.Max(end.X - start.X, end.Y - start.Y) > MaxLength )
                {
                    from.SendMessage("Maximum bounding box size exceeded.");
                    return;
                }

                object[] states = (object[])state;
				BaseCommand command = (BaseCommand)states[0];
				string[] args = (string[])states[1];

				Rectangle2D rect = new Rectangle2D( start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1 );

				Extensions ext = Extensions.Parse( from, ref args );

				bool items, mobiles;

				if ( !CheckObjectTypes( from, command, ext, out items, out mobiles ) )
					return;

				IPooledEnumerable eable;

				if ( items && mobiles )
					eable = map.GetObjectsInBounds( rect );
				else if ( items )
					eable = map.GetItemsInBounds( rect );
				else if ( mobiles )
					eable = map.GetMobilesInBounds( rect );
				else
					return;

				ArrayList objs = new ArrayList();

				foreach ( object obj in eable )
				{
					if ( mobiles && obj is Mobile && !BaseCommand.IsAccessible( from, obj ) )
						continue;

					if ( ext.IsValid( obj ) )
						objs.Add( obj );
				}

				eable.Free();

				ext.Filter( objs );

				RunCommand( from, objs, command, args );
			}
			catch ( Exception ex )
			{
				from.SendMessage( ex.Message );
			}
		}
	}
}